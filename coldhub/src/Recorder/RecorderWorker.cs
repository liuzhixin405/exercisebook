using System.Diagnostics.Metrics;
using System.Threading.Channels;
using ColdHub.Shared;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ColdHub.Recorder;

/// <summary>
/// 订阅 Reading:*，每秒把收到的读数成批追加进历史文件。
///
/// 两段式：订阅回调只做解码并放进有界队列（不阻塞 Valkey 的回调线程），写盘由独立循环每秒一次。
/// 写不进去的批次留着，下一轮连同新数据重试；磁盘故障按规则只记一条 Warning、恢复时一条 Information。
/// 收到的字节是别人发的，解码失败只计数，不让它搞垮订阅。
/// </summary>
public sealed class RecorderWorker : BackgroundService
{
    private const int Capacity = 100_000;

    private readonly IConnectionMultiplexer _valkey;
    private readonly string _directory;
    private readonly TimeProvider _time;
    private readonly ILogger<RecorderWorker> _logger;
    private readonly Channel<(Reading Reading, DateTimeOffset Received)> _queue =
        Channel.CreateBounded<(Reading, DateTimeOffset)>(new BoundedChannelOptions(Capacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleWriter = false,
            SingleReader = true
        });

    private readonly Counter<long> _written;
    private readonly Counter<long> _undecodable;
    private List<(Reading Reading, DateTimeOffset Received)> _pending = [];
    private bool _failing;
    private bool _undecodableSeen;

    public RecorderWorker(IConnectionMultiplexer valkey, IOptions<RecorderOptions> options, TimeProvider time,
        IMeterFactory meterFactory, ILogger<RecorderWorker> logger)
    {
        _valkey = valkey;
        _directory = options.Value.HistoryDir;
        _time = time;
        _logger = logger;

        Meter meter = meterFactory.Create("ColdHub.Recorder");
        _written = meter.CreateCounter<long>("coldhub.recorder.readings.written", "{reading}", "Readings appended to history files.");
        _undecodable = meter.CreateCounter<long>("coldhub.recorder.readings.undecodable", "{message}", "Messages that are not a valid reading.");
        meter.CreateObservableGauge("coldhub.recorder.readings.pending", () => _pending.Count + _queue.Reader.Count,
            "{reading}", "Readings waiting to be written.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 模式订阅：一次订阅所有 Reading:*，设备增减不用改这里
        await _valkey.GetSubscriber().SubscribeAsync(RedisChannel.Pattern(Channels.All), (_, value) => OnMessage(value));

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1), _time);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                Flush();
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }

        // 关机前把队列里剩下的写完
        Flush();
    }

    private void OnMessage(RedisValue value)
    {
        try
        {
            Reading reading = ReadingCodec.Decode((byte[])value!);
            _queue.Writer.TryWrite((reading, _time.GetUtcNow()));
        }
        catch (FormatException e)
        {
            _undecodable.Add(1);

            if (!_undecodableSeen)
            {
                _undecodableSeen = true;
                _logger.LogWarning(e, "A message on the pub/sub is not a valid reading (counted, not logged again)");
            }
        }
    }

    internal void Flush()
    {
        while (_queue.Reader.TryRead(out (Reading, DateTimeOffset) item))
        {
            _pending.Add(item);
        }

        if (_pending.Count == 0)
        {
            return;
        }

        var failed = new List<(Reading, DateTimeOffset)>();
        Exception? error = null;

        // 每个"源 + 天"一个文件，各自追加；某个文件写失败，只有它的那批留到下一轮
        foreach (var group in _pending.GroupBy(item => (item.Reading.Source, Day: HistoryFile.DayOf(item.Reading))))
        {
            try
            {
                HistoryFile.Append(HistoryFile.PathFor(_directory, group.Key.Source, group.Key.Day),
                    group.Select(item => HistoryFile.Row(item.Reading, item.Received)));
                _written.Add(group.Count());
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                failed.AddRange(group);
                error = e;
            }
        }

        _pending = failed;

        if (error is not null && !_failing)
        {
            _failing = true;
            _logger.LogWarning(error, "History cannot be written; trying again every second");
        }
        else if (error is null && _failing)
        {
            _failing = false;
            _logger.LogInformation("History is being written again");
        }
    }
}
