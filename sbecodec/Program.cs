// Demo 3: a hand-written SBE-style codec for a mini Snapshot, to see why QuoteHub uses SBE.
// Layout (little endian):  [header 8][root block 9][group header 4][N x level 17][symbol: len u8 + utf8]
//   header : blockLength u16, templateId u16, schemaId u16, version u16
//   root   : timestampNs i64, priceExponent i8
//   group  : blockLength u16, numInGroup u16, then fixed-size entries (side u8, mantissa i64, qty i64)
//   vardata: always last (SBE rule), length-prefixed
// QuoteHub counterparts: src/QuoteHub.Wire (generated SBE codec; read its Snapshot encoder/decoder next)
using System.Buffers.Binary;
using System.Text;
using System.Text.Json;

var symbol = Encoding.UTF8.GetBytes("BTCUSD");
Level[] levels = [new(0, 6_500_012, 3), new(0, 6_500_011, 9), new(1, 6_500_020, 5)]; // price = mantissa * 10^exp
var buf = new byte[256];

int n = Snapshot.Encode(buf, 1_700_000_000_000_000_000, -2, levels, symbol);
Console.WriteLine($"SBE bytes: {n}");
Console.WriteLine(Convert.ToHexString(buf.AsSpan(0, n)));

var view = new SnapshotView(buf.AsSpan(0, n));
Console.WriteLine($"{view.Symbol} ts={view.TimestampNs} levels={view.LevelCount}");
for (int i = 0; i < view.LevelCount; i++)
{
    var l = view.Level(i);
    Console.WriteLine($"  {(l.Side == 0 ? "bid" : "ask")} {l.Mantissa * Math.Pow(10, view.Exponent)} x {l.Quantity}");
}

var json = JsonSerializer.Serialize(new { symbol = "BTCUSD", ts = 1_700_000_000_000_000_000, levels = levels.Select(l => new { l.Side, price = l.Mantissa / 100.0, l.Quantity }) });
Console.WriteLine($"same data as JSON: {json.Length} bytes");

// Allocation check: encode + read 1M times. Run with -c Release (Debug may allocate more).
long before = GC.GetAllocatedBytesForCurrentThread();
long sum = 0;
for (int i = 0; i < 1_000_000; i++)
{
    int len = Snapshot.Encode(buf, i, -2, levels, symbol);
    var v = new SnapshotView(buf.AsSpan(0, len));
    sum += v.Level(0).Mantissa + v.SymbolBytes.Length;
}
Console.WriteLine($"allocated by 1M encode+decode: {GC.GetAllocatedBytesForCurrentThread() - before} bytes (sum={sum})");

readonly record struct Level(byte Side, long Mantissa, long Quantity);

static class Snapshot
{
    public const int HeaderSize = 8, RootSize = 9, GroupHeaderSize = 4, LevelSize = 17;

    public static int Encode(Span<byte> b, long tsNs, sbyte exp, ReadOnlySpan<Level> levels, ReadOnlySpan<byte> symbol)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(b, RootSize);      // blockLength: lets old readers skip new fields
        BinaryPrimitives.WriteUInt16LittleEndian(b[2..], 1);        // templateId
        BinaryPrimitives.WriteUInt16LittleEndian(b[4..], 1);        // schemaId
        BinaryPrimitives.WriteUInt16LittleEndian(b[6..], 0);        // version
        BinaryPrimitives.WriteInt64LittleEndian(b[8..], tsNs);
        b[16] = (byte)exp;
        BinaryPrimitives.WriteUInt16LittleEndian(b[17..], LevelSize);
        BinaryPrimitives.WriteUInt16LittleEndian(b[19..], (ushort)levels.Length);
        int p = HeaderSize + RootSize + GroupHeaderSize;
        foreach (var l in levels)
        {
            b[p] = l.Side;
            BinaryPrimitives.WriteInt64LittleEndian(b[(p + 1)..], l.Mantissa);
            BinaryPrimitives.WriteInt64LittleEndian(b[(p + 9)..], l.Quantity);
            p += LevelSize;
        }
        b[p++] = (byte)symbol.Length;
        symbol.CopyTo(b[p..]);
        return p + symbol.Length;
    }
}

// A view reads straight from the buffer: no objects, no copies. ref struct => cannot be stored on the heap.
readonly ref struct SnapshotView
{
    readonly ReadOnlySpan<byte> b;
    public SnapshotView(ReadOnlySpan<byte> buffer) => b = buffer;
    public long TimestampNs => BinaryPrimitives.ReadInt64LittleEndian(b[8..]);
    public sbyte Exponent => (sbyte)b[16];
    public int LevelCount => BinaryPrimitives.ReadUInt16LittleEndian(b[19..]);
    public Level Level(int i)
    {
        int p = Snapshot.HeaderSize + Snapshot.RootSize + Snapshot.GroupHeaderSize + i * Snapshot.LevelSize;
        return new Level(b[p], BinaryPrimitives.ReadInt64LittleEndian(b[(p + 1)..]), BinaryPrimitives.ReadInt64LittleEndian(b[(p + 9)..]));
    }
    public ReadOnlySpan<byte> SymbolBytes
    {
        get
        {
            int p = Snapshot.HeaderSize + Snapshot.RootSize + Snapshot.GroupHeaderSize + LevelCount * Snapshot.LevelSize;
            return b.Slice(p + 1, b[p]);
        }
    }
    public string Symbol => Encoding.UTF8.GetString(SymbolBytes); // allocates; only for display
}
