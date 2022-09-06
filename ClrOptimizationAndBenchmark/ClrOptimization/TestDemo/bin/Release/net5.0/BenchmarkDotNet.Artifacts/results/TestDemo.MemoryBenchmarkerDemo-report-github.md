``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19042.1110 (20H2/October2020Update)
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.301
  [Host]     : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT


```
|                          Method |      Mean |     Error |    StdDev |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
|-------------------------------- |----------:|----------:|----------:|----------:|----------:|---------:|----------:|
| ConcatStringsUsingStringBuilder |  7.184 ms | 0.1414 ms | 0.2514 ms | 2929.6875 | 1820.3125 | 968.7500 |     15 MB |
|   ConcatStringsUsingGenericList | 15.292 ms | 0.2722 ms | 0.2546 ms | 1562.5000 |  687.5000 | 265.6250 |      9 MB |
