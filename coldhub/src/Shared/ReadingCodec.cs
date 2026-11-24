using System.Buffers.Binary;
using System.Text;

namespace ColdHub.Shared;

/// <summary>
/// 二进制线上格式，手写版的 SBE（Simple Binary Encoding）。
///
/// 布局（全部小端）：
///   消息头 8 字节   blockLength u16 | templateId u16 | schemaId u16 | version u16
///   固定块 33 字节  id Guid(16) | sensorTime i64 | publishTime i64 | exponent i8
///   重复组          组头 4 字节 (blockLength u16 | count u16)，然后 count 个 { probeId u8 | mantissa i64 }
///   变长数据        source: 长度 u8 + UTF-8 | device: 长度 u8 + UTF-8
///
/// SBE 的要点都在这里：消息头让旧读者能跳过未知的新字段（blockLength），定长字段按偏移直接读，
/// 重复组和变长数据放在固定块之后。
/// </summary>
public static class ReadingCodec
{
    public const ushort TemplateId = 1;
    public const ushort SchemaId = 1;
    public const ushort Version = 1;

    private const int HeaderSize = 8;
    private const int FixedBlock = 16 + 8 + 8 + 1;
    private const int GroupHeaderSize = 4;
    private const int ProbeSize = 1 + 8;

    /// <summary>低于绝对零度（-273.15℃）的值一定是传感器故障，编码器拒绝它。</summary>
    private const decimal AbsoluteZero = -273.15m;

    /// <summary>
    /// 编码。探头必须按 Id 严格递增；顺序错了说明上游出了问题，宁可拒绝也不发出去。
    /// 编码器拒绝时抛 <see cref="ArgumentException"/>，由发布方记录并丢弃。
    /// </summary>
    public static byte[] Encode(string source, string device, Guid id, long sensorTime, long publishTime,
        sbyte exponent, ReadOnlySpan<Probe> probes)
    {
        byte[] sourceBytes = Name(source, nameof(source));
        byte[] deviceBytes = Name(device, nameof(device));
        Validate(exponent, probes);

        int size = HeaderSize + FixedBlock + GroupHeaderSize + probes.Length * ProbeSize + 1 + sourceBytes.Length + 1 +
                   deviceBytes.Length;
        var buffer = new byte[size];
        Span<byte> span = buffer;

        BinaryPrimitives.WriteUInt16LittleEndian(span, FixedBlock);
        BinaryPrimitives.WriteUInt16LittleEndian(span[2..], TemplateId);
        BinaryPrimitives.WriteUInt16LittleEndian(span[4..], SchemaId);
        BinaryPrimitives.WriteUInt16LittleEndian(span[6..], Version);

        Span<byte> body = span[HeaderSize..];
        id.TryWriteBytes(body, bigEndian: true, out _);
        BinaryPrimitives.WriteInt64LittleEndian(body[16..], sensorTime);
        BinaryPrimitives.WriteInt64LittleEndian(body[24..], publishTime);
        body[32] = (byte)exponent;

        Span<byte> group = body[FixedBlock..];
        BinaryPrimitives.WriteUInt16LittleEndian(group, ProbeSize);
        BinaryPrimitives.WriteUInt16LittleEndian(group[2..], (ushort)probes.Length);

        Span<byte> at = group[GroupHeaderSize..];

        foreach (Probe probe in probes)
        {
            at[0] = probe.Id;
            BinaryPrimitives.WriteInt64LittleEndian(at[1..], probe.Mantissa);
            at = at[ProbeSize..];
        }

        at[0] = (byte)sourceBytes.Length;
        sourceBytes.CopyTo(at[1..]);
        at = at[(1 + sourceBytes.Length)..];
        at[0] = (byte)deviceBytes.Length;
        deviceBytes.CopyTo(at[1..]);

        return buffer;
    }

    /// <summary>解码；数据不合格式时抛 <see cref="FormatException"/>（收到的是别人发来的字节，不可信）。</summary>
    public static Reading Decode(ReadOnlySpan<byte> message)
    {
        if (message.Length < HeaderSize + FixedBlock + GroupHeaderSize)
        {
            throw new FormatException("message too short");
        }

        ushort blockLength = BinaryPrimitives.ReadUInt16LittleEndian(message);
        ushort templateId = BinaryPrimitives.ReadUInt16LittleEndian(message[2..]);
        ushort schemaId = BinaryPrimitives.ReadUInt16LittleEndian(message[4..]);

        if (templateId != TemplateId || schemaId != SchemaId)
        {
            throw new FormatException($"unknown template {templateId} of schema {schemaId}");
        }

        // 用头里的 blockLength 而不是常量 FixedBlock：新版本在固定块末尾加字段时，旧读者照样能跳到重复组
        ReadOnlySpan<byte> body = message[HeaderSize..];

        if (body.Length < blockLength + GroupHeaderSize)
        {
            throw new FormatException("fixed block cut off");
        }

        var id = new Guid(body[..16], bigEndian: true);
        long sensorTime = BinaryPrimitives.ReadInt64LittleEndian(body[16..]);
        long publishTime = BinaryPrimitives.ReadInt64LittleEndian(body[24..]);
        sbyte exponent = (sbyte)body[32];

        ReadOnlySpan<byte> group = body[blockLength..];
        ushort probeBlock = BinaryPrimitives.ReadUInt16LittleEndian(group);
        ushort count = BinaryPrimitives.ReadUInt16LittleEndian(group[2..]);
        ReadOnlySpan<byte> at = group[GroupHeaderSize..];

        if (probeBlock < ProbeSize || at.Length < count * probeBlock)
        {
            throw new FormatException("probe group cut off");
        }

        var probes = new Probe[count];

        for (int i = 0; i < count; i++)
        {
            probes[i] = new Probe(at[0], BinaryPrimitives.ReadInt64LittleEndian(at[1..]));
            at = at[probeBlock..];
        }

        string source = ReadName(ref at);
        string device = ReadName(ref at);

        return new Reading(source, device, id, sensorTime, publishTime, exponent, probes);
    }

    private static string ReadName(ref ReadOnlySpan<byte> at)
    {
        if (at.Length < 1 || at.Length < 1 + at[0])
        {
            throw new FormatException("name cut off");
        }

        string name = Encoding.UTF8.GetString(at.Slice(1, at[0]));
        at = at[(1 + at[0])..];
        return name;
    }

    private static byte[] Name(string value, string parameter)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);

        if (bytes.Length is 0 or > byte.MaxValue)
        {
            throw new ArgumentException("a name is 1 to 255 bytes", parameter);
        }

        return bytes;
    }

    private static void Validate(sbyte exponent, ReadOnlySpan<Probe> probes)
    {
        if (probes.IsEmpty || probes.Length > ushort.MaxValue)
        {
            throw new ArgumentException("a reading has 1 to 65535 probes", nameof(probes));
        }

        decimal scale = Reading.Pow10(exponent);

        for (int i = 0; i < probes.Length; i++)
        {
            if (i > 0 && probes[i].Id <= probes[i - 1].Id)
            {
                throw new ArgumentException("probes are not in ascending id order", nameof(probes));
            }

            // decimal 的范围远大于 long × 10^±20，这里不会溢出
            if (probes[i].Mantissa * scale < AbsoluteZero)
            {
                throw new ArgumentException($"probe {probes[i].Id} reads below absolute zero", nameof(probes));
            }
        }
    }
}
