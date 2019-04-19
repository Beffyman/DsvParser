# DsvParser
[![Build Status](https://beffyman.visualstudio.com/Beffyman.Github/_apis/build/status/Beffyman.DsvParser?branchName=master)](https://beffyman.visualstudio.com/Beffyman.Github/_build/latest?definitionId=5&branchName=master)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/beffyman/Beffyman.Github/5.svg)

Attempt at a high performance and memory efficient DSV parser that follows the [RFC4180](https://tools.ietf.org/html/rfc4180) spec

---
## Exposed Types

### DsvReader
- Reads the provided data and attempts to make minimal memory allocations in order to do so.
  - This can vary based on what is in the fields parsed.  The case where there are escaped escape characters like "ABC""EFG" would cause an allocation as it can't be stored without allocating as it's real interpretation which is ABC"EFG is not a continuous sequence in the provided data.

### DsvParser
- Stores the columns (if they exist) and the rows in memory as ReadOnlyMemory\<char>s
- Utilizes the DsvReader to read the file in lines with ReadLine

### DsvParser\<TRecord, TRecordMapping>
- Works the same way as the DsvParser, but when reading fields it will convert them into an instance of TRecord by using the TRecordMapping specified

### DsvOptions
- Options object that allow custom DSV formats

---
## Benchmark
as of commit: e9b34430c226d3f47c2c8e7ffdaab0363471506a
``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.437 (1809/October2018Update/Redstone5)
AMD Ryzen 7 2700X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=3.0.100-preview4-011223
  [Host]     : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT
  DefaultJob : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT


```
|                       Method | Param_Rows | Columns |          Mean |       Error |      StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |  Allocated |
|----------------------------- |----------- |-------- |--------------:|------------:|------------:|------:|--------:|----------:|----------:|---------:|-----------:|
|           **Beffyman_DsvParser** |         **10** |      **10** |      **4.599 us** |   **0.0215 us** |   **0.0201 us** |  **1.42** |    **0.01** |    **1.1292** |         **-** |        **-** |     **4736 B** |
|           Beffyman_DsvReader |         10 |      10 |      3.246 us |   0.0182 us |   0.0161 us |  1.00 |    0.00 |    0.0381 |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |         10 |      10 |     10.266 us |   0.0159 us |   0.0132 us |  3.16 |    0.02 |    1.8921 |         - |        - |     7920 B |
| DelimiterSeparatedTextReader |         10 |      10 |      2.886 us |   0.0210 us |   0.0196 us |  0.89 |    0.01 |    0.0229 |         - |        - |       96 B |
| DelimiterSeparatedTextParser |         10 |      10 |      4.915 us |   0.0225 us |   0.0200 us |  1.51 |    0.01 |    0.5112 |         - |        - |     2152 B |
|                FastCsvParser |         10 |      10 |     19.726 us |   0.1686 us |   0.1577 us |  6.07 |    0.05 |   41.6565 |   41.6565 |  41.6565 |   207920 B |
|                              |            |         |               |             |             |       |         |           |           |          |            |
|           **Beffyman_DsvParser** |         **10** |      **50** |     **20.936 us** |   **0.1478 us** |   **0.1383 us** |  **1.15** |    **0.01** |    **3.8452** |    **1.9226** |        **-** |    **19680 B** |
|           Beffyman_DsvReader |         10 |      50 |     18.170 us |   0.1064 us |   0.0995 us |  1.00 |    0.00 |    0.0305 |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |         10 |      50 |     44.290 us |   0.2145 us |   0.1791 us |  2.44 |    0.02 |    9.3384 |    1.1597 |        - |    39296 B |
| DelimiterSeparatedTextReader |         10 |      50 |     14.037 us |   0.0636 us |   0.0595 us |  0.77 |    0.01 |    0.0153 |         - |        - |       96 B |
| DelimiterSeparatedTextParser |         10 |      50 |     25.698 us |   0.1772 us |   0.1571 us |  1.42 |    0.01 |    1.4648 |         - |        - |     6168 B |
|                FastCsvParser |         10 |      50 |     51.331 us |   1.2535 us |   1.3932 us |  2.84 |    0.08 |   41.6260 |   41.6260 |  41.6260 |   254976 B |
|                              |            |         |               |             |             |       |         |           |           |          |            |
|           **Beffyman_DsvParser** |       **1000** |      **10** |    **564.296 us** |   **2.3123 us** |   **2.1629 us** |  **1.89** |    **0.01** |   **67.3828** |   **54.6875** |  **34.1797** |   **342109 B** |
|           Beffyman_DsvReader |       1000 |      10 |    297.870 us |   1.1489 us |   1.0747 us |  1.00 |    0.00 |         - |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |       1000 |      10 |    986.411 us |   5.9024 us |   5.2324 us |  3.31 |    0.02 |  109.3750 |   66.4063 |  37.1094 |   566712 B |
| DelimiterSeparatedTextReader |       1000 |      10 |    247.794 us |   1.4557 us |   1.2904 us |  0.83 |    0.01 |         - |         - |        - |       96 B |
| DelimiterSeparatedTextParser |       1000 |      10 |    433.487 us |   2.4936 us |   2.3326 us |  1.46 |    0.01 |   36.1328 |   11.7188 |        - |   153064 B |
|                FastCsvParser |       1000 |      10 |    598.906 us |   5.9758 us |   5.5898 us |  2.01 |    0.02 |  124.0234 |   41.0156 |  41.0156 |   532640 B |
|                              |            |         |               |             |             |       |         |           |           |          |            |
|           **Beffyman_DsvParser** |       **1000** |      **50** |  **2,646.442 us** |  **16.0842 us** |  **13.4310 us** |  **1.72** |    **0.01** |  **175.7813** |   **82.0313** |  **42.9688** |  **1545020 B** |
|           Beffyman_DsvReader |       1000 |      50 |  1,541.658 us |  10.0316 us |   8.8927 us |  1.00 |    0.00 |         - |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |       1000 |      50 |  4,855.150 us |  19.1455 us |  16.9720 us |  3.15 |    0.02 |  546.8750 |  367.1875 | 156.2500 |  3053306 B |
| DelimiterSeparatedTextReader |       1000 |      50 |  1,418.011 us |  27.5713 us |  30.6455 us |  0.91 |    0.02 |         - |         - |        - |       96 B |
| DelimiterSeparatedTextParser |       1000 |      50 |  1,999.731 us |   3.4990 us |   2.9218 us |  1.30 |    0.01 |   85.9375 |   42.9688 |        - |   473880 B |
|                FastCsvParser |       1000 |      50 |  2,746.055 us |  15.4855 us |  13.7275 us |  1.78 |    0.01 |  558.5938 |  152.3438 |  74.2188 |  2295583 B |
|                              |            |         |               |             |             |       |         |           |           |          |            |
|           **Beffyman_DsvParser** |      **10000** |      **10** |  **6,595.500 us** |  **29.4609 us** |  **27.5577 us** |  **2.20** |    **0.01** |  **367.1875** |  **156.2500** |  **54.6875** |  **3605738 B** |
|           Beffyman_DsvReader |      10000 |      10 |  2,992.352 us |  16.0767 us |  15.0381 us |  1.00 |    0.00 |         - |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |      10000 |      10 | 13,424.173 us | 129.8807 us | 115.1359 us |  4.49 |    0.05 |  843.7500 |  437.5000 | 187.5000 |  5744744 B |
| DelimiterSeparatedTextReader |      10000 |      10 |  2,533.086 us |  21.0517 us |  19.6918 us |  0.85 |    0.01 |         - |         - |        - |       96 B |
| DelimiterSeparatedTextParser |      10000 |      10 |  5,451.423 us |  17.1215 us |  16.0155 us |  1.82 |    0.01 |  273.4375 |  140.6250 |  54.6875 |  1622952 B |
|                FastCsvParser |      10000 |      10 |  5,548.257 us |  21.1242 us |  19.7596 us |  1.85 |    0.01 |  835.9375 |   85.9375 |  39.0625 |  3616863 B |
|                              |            |         |               |             |             |       |         |           |           |          |            |
|           **Beffyman_DsvParser** |      **10000** |      **50** | **28,560.918 us** |  **75.8870 us** |  **70.9848 us** |  **2.13** |    **0.01** | **1531.2500** |  **625.0000** | **187.5000** | **15608740 B** |
|           Beffyman_DsvReader |      10000 |      50 | 13,419.744 us |  71.9199 us |  63.7551 us |  1.00 |    0.00 |         - |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |      10000 |      50 | 70,342.467 us | 264.0634 us | 247.0051 us |  5.24 |    0.03 | 4333.3333 | 1833.3333 | 666.6667 | 30551152 B |
| DelimiterSeparatedTextReader |      10000 |      50 | 13,029.406 us | 231.4904 us | 216.5363 us |  0.97 |    0.02 |         - |         - |        - |       96 B |
| DelimiterSeparatedTextParser |      10000 |      50 | 23,048.550 us |  92.0625 us |  86.1153 us |  1.72 |    0.01 |  843.7500 |  437.5000 | 156.2500 |  4823858 B |
|                FastCsvParser |      10000 |      50 | 25,601.322 us | 505.0928 us | 472.4641 us |  1.91 |    0.04 | 4625.0000 |   31.2500 |        - | 19653112 B |
