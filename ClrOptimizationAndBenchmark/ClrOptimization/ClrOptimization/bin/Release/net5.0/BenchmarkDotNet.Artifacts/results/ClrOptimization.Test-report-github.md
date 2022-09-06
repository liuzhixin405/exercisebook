``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19042.1110 (20H2/October2020Update)
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.301
  [Host]   : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT  [AttachedDebugger]
  .NET 5.0 : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT


```
|      Method |        Job |    Runtime |      Mean |     Error |    StdDev |
|------------ |----------- |----------- |----------:|----------:|----------:|
|   SpanEqual |   .NET 5.0 |   .NET 5.0 |  4.254 ns | 0.1057 ns | 0.0937 ns |
|    ForBytes |   .NET 5.0 |   .NET 5.0 | 87.832 ns | 0.1068 ns | 0.0892 ns |
|    ForArray |   .NET 5.0 |   .NET 5.0 | 66.419 ns | 1.3437 ns | 1.1911 ns |
| EqualsArray |   .NET 5.0 |   .NET 5.0 | 16.825 ns | 0.0241 ns | 0.0214 ns |
| EqualsBytes |   .NET 5.0 |   .NET 5.0 | 18.234 ns | 0.0776 ns | 0.0648 ns |
|   SpanEqual | CoreRT 5.0 | CoreRT 5.0 |        NA |        NA |        NA |
|    ForBytes | CoreRT 5.0 | CoreRT 5.0 |        NA |        NA |        NA |
|    ForArray | CoreRT 5.0 | CoreRT 5.0 |        NA |        NA |        NA |
| EqualsArray | CoreRT 5.0 | CoreRT 5.0 |        NA |        NA |        NA |
| EqualsBytes | CoreRT 5.0 | CoreRT 5.0 |        NA |        NA |        NA |

Benchmarks with issues:
  Test.SpanEqual: CoreRT 5.0(Runtime=CoreRT 5.0)
  Test.ForBytes: CoreRT 5.0(Runtime=CoreRT 5.0)
  Test.ForArray: CoreRT 5.0(Runtime=CoreRT 5.0)
  Test.EqualsArray: CoreRT 5.0(Runtime=CoreRT 5.0)
  Test.EqualsBytes: CoreRT 5.0(Runtime=CoreRT 5.0)
