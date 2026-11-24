using System.IO.Compression;
using System.Text;
using ColdHub.Shared;

namespace ColdHub.Recorder;

/// <summary>
/// 历史文件的写法。每批读数压成一个 gzip "成员"，追加到当天文件末尾：多个 gzip 成员首尾相连仍是合法的 .gz，
/// 解压得到各成员内容依次拼起来。这样写入只要追加，不用改写旧内容，进程随时被杀也不会弄坏之前的数据。
/// （真实场景会存进时序数据库，这里只保留"按源按天一个文件"的最小形态。）
/// </summary>
public static class HistoryFile
{
    public const string Header =
        "\"source\",\"device\",\"sensor_time\",\"publish_time\",\"received_time\",\"id\",\"temp_exponent\",\"probe_ids\",\"probe_mantissas\"";

    public static string PathFor(string directory, string source, DateOnly day) =>
        Path.Combine(directory, source, $"{day:yyyy-MM-dd}.csv.gz");

    /// <summary>文件按"发布时刻"的 UTC 日期归属。</summary>
    public static DateOnly DayOf(Reading reading) =>
        DateOnly.FromDateTime(UnixTime.FromNanoseconds(reading.PublishTime).UtcDateTime);

    public static string Row(Reading reading, DateTimeOffset received) => string.Join(',',
        Quote(reading.Source), Quote(reading.Device), Quote(Iso(UnixTime.FromNanoseconds(reading.SensorTime))),
        Quote(Iso(UnixTime.FromNanoseconds(reading.PublishTime))), Quote(Iso(received)), Quote(reading.Id.ToString()),
        reading.Exponent, Quote($"[{string.Join(',', reading.Probes.Select(p => p.Id))}]"),
        Quote($"[{string.Join(',', reading.Probes.Select(p => p.Mantissa))}]"));

    /// <summary>先在内存里压好，再一次追加到文件：写失败时文件里不会留下半个成员。</summary>
    public static void Append(string path, IEnumerable<string> rows)
    {
        bool isNew = !File.Exists(path);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        using var memory = new MemoryStream();

        using (var gzip = new GZipStream(memory, CompressionLevel.Fastest, leaveOpen: true))
        using (var writer = new StreamWriter(gzip, new UTF8Encoding(false)) { NewLine = "\n" })
        {
            if (isNew)
            {
                writer.WriteLine(Header);
            }

            foreach (string row in rows)
            {
                writer.WriteLine(row);
            }
        }

        File.AppendAllBytes(path, memory.ToArray());
    }

    private static string Iso(DateTimeOffset time) => time.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ");

    private static string Quote(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
}
