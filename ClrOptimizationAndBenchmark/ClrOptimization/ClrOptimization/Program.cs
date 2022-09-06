using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace ClrOptimization
{
    class Program
    {
        static void Main(string[] args)
        {
            //int[] arr1 = new int[] { 1, 2, 3, 4, 5 };
            //int[] arr2 = new int[10] { 0, 0, 0, 0, 0, 6, 7, 8, 9, 10 };
            //Buffer.BlockCopy(arr1, 0, arr2, 0, 19);
            //for (int i = 0; i < arr2.Length; i++)
            //{
            //    Console.Write(arr2[i] + ",");
            //}
            //string[] arr3 = new string[] { "aa","bb" };
            //string[] arr4 = new string[] { "oo", "qq", "cc", "dd" };
            //Buffer.BlockCopy(arr3, 0, arr4, 0, 20);
            //for (int i = 0; i < arr4.Length; i++)
            //{
            //    Console.Write(arr4[i] + ",");
            //}


            ////source data:
            //// 0000,0001,0002,00003,0004
            //// 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
            //int[] a = new int[] { 0, 1, 2, 3, 4 };
            //foreach (var item in a)
            //{
            //    Console.Write(item + ",");
            //}

            //Console.WriteLine("\n------\n");

            //// see : https://stackoverflow.com/questions/26455843/how-are-array-values-stored-in-little-endian-vs-big-endian-architecture
            //// memory save that data:
            //// 0000    1000    2000    3000    4000
            //for (int i = 0; i < Buffer.ByteLength(a); i++)
            //{
            //    Console.Write(Buffer.GetByte(a, i));
            //    if (i != 0 && (i + 1) % 4 == 0)
            //        Console.Write("    ");
            //}

            //// 16 进制
            //// 0000    1000    2000    3000    4000

            //Console.WriteLine("\n------\n");

            //Buffer.SetByte(a, 0, 4);
            //Buffer.SetByte(a, 4, 3);
            //Buffer.SetByte(a, 8, 2);
            //Buffer.SetByte(a, 12, 1);
            //Buffer.SetByte(a, 16, 0);

            //foreach (var item in a)
            //{
            //    Console.Write(item + ",");
            //}

            //Console.WriteLine("\n------\n");


            //// source data:  00 01 02 03 04
            //// binary data:  00000000 00000001 00000010 00000011 000001000
            //byte[] arr = new byte[] { 0, 1, 2, 3, 4, };

            //// read one int,4 byte
            //int head = BinaryPrimitives.ReadInt32BigEndian(arr);


            //// 5 byte:             00000000 00000001 00000010 00000011 000001000
            //// read 4 byte(int) :  00000000 00000001 00000010 00000011
            ////                     = 66051

            //Console.WriteLine(head);
            //Console.WriteLine($"BitConverter.IsLittleEndian={BitConverter.IsLittleEndian}");


            //// source data:  00 01 02 03 04
            //// binary data:  00000000 00000001 00000010 00000011 000001000
            //byte[] arr = new byte[] { 0, 1, 2, 3, 4, };

            //// read one int,4 byte
            //// 5 byte:             00000000 00000001 00000010 00000011 000001000
            //// read 4 byte(int) :  00000000 00000001 00000010 00000011
            ////                     = 66051

            //int head = BinaryPrimitives.ReadInt32BigEndian(arr);
            //Console.WriteLine(head);

            //// BinaryPrimitives.WriteInt32LittleEndian(arr, 1);
            //BinaryPrimitives.WriteInt32BigEndian(arr.AsSpan().Slice(0, 4), 0b00000000_00000000_00000000_00000001);
            //// to : 00000000 00000000 00000000 00000001 |  000001000
            //// read 4 byte

            //head = BinaryPrimitives.ReadInt32BigEndian(arr);
            //Console.WriteLine(head);



            //short value = 0b00000000_00000001;
            //// to endianness: 0b00000001_00000000 == 256
            //BinaryPrimitives.ReverseEndianness(0b00000000_00000000_00000000_00000001);

            //Console.WriteLine(BinaryPrimitives.ReverseEndianness(value));

            //value = 0b00000001_00000000;
            //Console.WriteLine(BinaryPrimitives.ReverseEndianness(value));
            //// 1
            ///

            //// 0b...1_00000100
            //int value = 260;

            //// byte max value:255
            //// a = 0b00000100; 丢失 int ... 00000100 之前的位数。
            //byte a = (byte)value;

            //// a = 4
            //Console.WriteLine(a);

            //// LittleEndian
            //// 0b 00000100 00000001 00000000 00000000
            //byte[] b = BitConverter.GetBytes(260);
            //Console.WriteLine(Buffer.GetByte(b, 1)); // 4

            //if (BitConverter.IsLittleEndian)
            //    Console.WriteLine(BinaryPrimitives.ReadInt32LittleEndian(b));
            //else
            //    Console.WriteLine(BinaryPrimitives.ReadInt32BigEndian(b));


            //// 1 int  = 4 byte
            //// int [] {1,2}
            //// 0001     0002
            //var byteArray = new byte[] { 1, 0, 0, 0, 2, 0, 0, 0 };
            //Span<byte> byteSpan = byteArray.AsSpan();
            //// byte to int 
            //Span<int> intSpan = MemoryMarshal.Cast<byte, int>(byteSpan);
            //foreach (var item in intSpan)
            //{
            //    Console.Write(item + ",");
            //}




            //Test test = new TestN()
            //{
            //    A = 1,
            //    B = 2,
            //    C = 3
            //};
            //var testArray = new TestN[] { test };
            //ReadOnlySpan<byte> tmp = MemoryMarshal.AsBytes(testArray.AsSpan());

            //// socket.Send(tmp); ...

            //int[] a = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            //int[] b = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            //Console.WriteLine(Compare64(a, b));
            //Console.WriteLine("SystemDefaultCharSize={0}, SystemMaxDBCSCharSize={1}",Marshal.SystemDefaultCharSize, Marshal.SystemMaxDBCSCharSize);


            //Console.WriteLine(Marshal.SizeOf(typeof(Point)));
            var summary = BenchmarkRunner.Run<Test>();

            Console.Read();
        }
       
        private static bool Compare64<T>(T[] t1, T[] t2)
          where T : struct
        {
            var l1 = MemoryMarshal.Cast<T, long>(t1);
            var l2 = MemoryMarshal.Cast<T, long>(t2);

            for (int i = 0; i < l1.Length; i++)
            {
                if (l1[i] != l2[i]) return false;
            }
            return true;
        }
    }
    public struct Point
    {
        public Int32 x, y;
    }

    public struct TestN
    {
        public int A;
        public int B;
        public int C;
    }

    [SimpleJob(RuntimeMoniker.Net50)]
    [SimpleJob(RuntimeMoniker.CoreRt50)]
    [RPlotExporter]
    public class Test
    {
        [Benchmark]
        public  bool SpanEqual()
        {
            return SpanEqual(_a, _b);
        }
        private  bool SpanEqual(byte[] a, byte[] b)
        {
            return a.AsSpan().SequenceEqual(b);
        }
        private byte[] _a = Encoding.UTF8.GetBytes("5456456456444444444444156456454564444444444444444444444444444444444444444777777777777777777777711111111111116666666666666");
        private byte[] _b = Encoding.UTF8.GetBytes("5456456456444444444444156456454564444444444444444444444444444444444444444777777777777777777777711111111111116666666666666");

        private int[] A1 = new int[] { 41544444, 4487, 841, 8787, 4415, 7, 458, 4897, 87897, 815, 485, 4848, 787, 41, 5489, 74878, 84, 89787, 8456, 4857489, 784, 85489, 47 };
        private int[] B2 = new int[] { 41544444, 4487, 841, 8787, 4415, 7, 458, 4897, 87897, 815, 485, 4848, 787, 41, 5489, 74878, 84, 89787, 8456, 4857489, 784, 85489, 47 };

        [Benchmark]
        public bool ForBytes()
        {
            for (int i = 0; i < _a.Length; i++)
            {
                if (_a[i] != _b[i]) return false;
            }
            return true;
        }

        [Benchmark]
        public bool ForArray()
        {
            return ForArray(A1, B2);
        }

        private bool ForArray<T>(T[] b1, T[] b2) where T : struct
        {
            for (int i = 0; i < b1.Length; i++)
            {
                if (!b1[i].Equals(b2[i])) return false;
            }
            return true;
        }

        [Benchmark]
        public bool EqualsArray()
        {
            return EqualArray(A1, B2);
        }

        [Benchmark]
        public bool EqualsBytes()
        {
            var a = _a.AsSpan();
            var b = _b.AsSpan();
            Span<byte> copy1 = default;
            Span<byte> copy2 = default;

            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length;)
            {
                if (a.Length - 8 > i)
                {
                    copy1 = a.Slice(i, 8);
                    copy2 = b.Slice(i, 8);
                    if (BinaryPrimitives.ReadUInt64BigEndian(copy1) != BinaryPrimitives.ReadUInt64BigEndian(copy2))
                        return false;
                    i += 8;
                    continue;
                }

                if (a[i] != b[i])
                    return false;
                i++;
            }
            return true;
        }

        private bool EqualArray<T>(T[] t1, T[] t2) where T : struct
        {
            Span<byte> b1 = MemoryMarshal.AsBytes<T>(t1.AsSpan());
            Span<byte> b2 = MemoryMarshal.AsBytes<T>(t2.AsSpan());

            Span<byte> copy1 = default;
            Span<byte> copy2 = default;

            if (b1.Length != b2.Length)
                return false;

            for (int i = 0; i < b1.Length;)
            {
                if (b1.Length - 8 > i)
                {
                    copy1 = b1.Slice(i, 8);
                    copy2 = b2.Slice(i, 8);
                    if (BinaryPrimitives.ReadUInt64BigEndian(copy1) != BinaryPrimitives.ReadUInt64BigEndian(copy2))
                        return false;
                    i += 8;
                    continue;
                }

                if (b1[i] != b2[i])
                    return false;
                i++;
            }
            return true;
        }
    }

}
