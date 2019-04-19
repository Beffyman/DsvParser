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
as of commit: 6746888298bba9f6f282962d8acbd580d0667dc4
``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.437 (1809/October2018Update/Redstone5)
AMD Ryzen 7 2700X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=3.0.100-preview4-011223
  [Host]     : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT
  DefaultJob : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT


```
|                       Method | Param_Rows | Columns |          Mean |       Error |      StdDev |  Ratio | RatioSD |      Gen 0 |     Gen 1 |    Gen 2 |  Allocated |
|----------------------------- |----------- |-------- |--------------:|------------:|------------:|-------:|--------:|-----------:|----------:|---------:|-----------:|
|           **Beffyman_DsvParser** |         **10** |      **10** |      **5.370 us** |   **0.0235 us** |   **0.0208 us** |   **1.51** |    **0.01** |     **1.1292** |         **-** |        **-** |     **4736 B** |
|           Beffyman_DsvReader |         10 |      10 |      3.565 us |   0.0168 us |   0.0149 us |   1.00 |    0.00 |     0.0381 |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |         10 |      10 |      9.992 us |   0.0216 us |   0.0191 us |   2.80 |    0.01 |     1.8921 |         - |        - |     7920 B |
| DelimiterSeparatedTextReader |         10 |      10 |      3.194 us |   0.0638 us |   0.0709 us |   0.89 |    0.02 |     0.0229 |         - |        - |       96 B |
| DelimiterSeparatedTextParser |         10 |      10 |      4.945 us |   0.0223 us |   0.0209 us |   1.39 |    0.01 |     0.5112 |         - |        - |     2152 B |
|                TinyCsvParser |         10 |      10 |    786.804 us |   1.1529 us |   0.9627 us | 220.61 |    0.93 |    25.3906 |   12.6953 |   0.9766 |    74188 B |
|                FastCsvParser |         10 |      10 |     19.116 us |   0.1546 us |   0.1446 us |   5.36 |    0.05 |    41.6565 |   41.6565 |  41.6565 |   207920 B |
|                    CsvHelper |         10 |      10 |     18.977 us |   0.1094 us |   0.0970 us |   5.32 |    0.04 |     6.4697 |         - |        - |    27096 B |
|                  FileHelpers |         10 |      10 |    629.623 us |   1.3837 us |   1.2943 us | 176.56 |    0.85 |     6.8359 |    3.9063 |        - |    29364 B |
|                              |            |         |               |             |             |        |         |            |           |          |            |
|           **Beffyman_DsvParser** |         **10** |      **50** |     **22.824 us** |   **0.1735 us** |   **0.1538 us** |   **1.24** |    **0.01** |     **3.8452** |    **1.9226** |        **-** |    **19680 B** |
|           Beffyman_DsvReader |         10 |      50 |     18.434 us |   0.0484 us |   0.0453 us |   1.00 |    0.00 |     0.0305 |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |         10 |      50 |     46.277 us |   0.9095 us |   1.3613 us |   2.53 |    0.08 |     9.3384 |    1.1597 |        - |    39296 B |
| DelimiterSeparatedTextReader |         10 |      50 |     14.093 us |   0.0996 us |   0.0932 us |   0.76 |    0.01 |     0.0153 |         - |        - |       96 B |
| DelimiterSeparatedTextParser |         10 |      50 |     21.342 us |   0.1376 us |   0.1287 us |   1.16 |    0.01 |     1.4648 |         - |        - |     6168 B |
|                TinyCsvParser |         10 |      50 |  3,798.785 us |   4.0555 us |   3.5951 us | 206.05 |    0.59 |   101.5625 |   46.8750 |   7.8125 |   321233 B |
|                FastCsvParser |         10 |      50 |     47.804 us |   0.2637 us |   0.2467 us |   2.59 |    0.02 |    41.6260 |   41.6260 |  41.6260 |   254976 B |
|                    CsvHelper |         10 |      50 |     84.186 us |   0.2992 us |   0.2799 us |   4.57 |    0.02 |    21.4844 |         - |        - |    90096 B |
|                  FileHelpers |         10 |      50 |  2,693.119 us |  22.9208 us |  21.4402 us | 146.09 |    1.23 |    31.2500 |   11.7188 |        - |   137384 B |
|                              |            |         |               |             |             |        |         |            |           |          |            |
|           **Beffyman_DsvParser** |       **1000** |      **10** |    **489.410 us** |   **2.5134 us** |   **2.3510 us** |   **1.64** |    **0.01** |    **66.4063** |   **55.6641** |  **33.2031** |   **342116 B** |
|           Beffyman_DsvReader |       1000 |      10 |    298.566 us |   0.9506 us |   0.8892 us |   1.00 |    0.00 |          - |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |       1000 |      10 |    994.286 us |  19.6403 us |  23.3804 us |   3.32 |    0.07 |   109.3750 |   66.4063 |  37.1094 |   566712 B |
| DelimiterSeparatedTextReader |       1000 |      10 |    256.936 us |   0.6893 us |   0.6111 us |   0.86 |    0.00 |          - |         - |        - |       96 B |
| DelimiterSeparatedTextParser |       1000 |      10 |    454.940 us |   1.6941 us |   1.4146 us |   1.52 |    0.01 |    36.1328 |   11.7188 |        - |   153064 B |
|                TinyCsvParser |       1000 |      10 |  1,783.951 us |   5.3063 us |   4.7039 us |   5.97 |    0.02 |   617.1875 |  181.6406 |        - |   224758 B |
|                FastCsvParser |       1000 |      10 |    613.378 us |   6.5976 us |   6.1714 us |   2.05 |    0.02 |   124.0234 |   41.0156 |  41.0156 |   532640 B |
|                    CsvHelper |       1000 |      10 |  1,496.548 us |   2.0336 us |   1.8027 us |   5.01 |    0.01 |   310.5469 |         - |        - |  1299272 B |
|                  FileHelpers |       1000 |      10 |  1,796.953 us |  16.4095 us |  15.3495 us |   6.02 |    0.06 |   115.2344 |   48.8281 |        - |   615925 B |
|                              |            |         |               |             |             |        |         |            |           |          |            |
|           **Beffyman_DsvParser** |       **1000** |      **50** |  **2,785.126 us** |  **13.4982 us** |  **12.6262 us** |   **1.84** |    **0.01** |   **175.7813** |   **82.0313** |  **46.8750** |  **1545031 B** |
|           Beffyman_DsvReader |       1000 |      50 |  1,510.707 us |   9.6033 us |   8.5131 us |   1.00 |    0.00 |          - |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |       1000 |      50 |  4,865.760 us |  18.3629 us |  17.1767 us |   3.22 |    0.02 |   562.5000 |  375.0000 | 164.0625 |  3053558 B |
| DelimiterSeparatedTextReader |       1000 |      50 |  1,419.470 us |   1.9323 us |   1.7130 us |   0.94 |    0.01 |          - |         - |        - |       96 B |
| DelimiterSeparatedTextParser |       1000 |      50 |  2,166.956 us |   3.2045 us |   2.9975 us |   1.43 |    0.01 |    85.9375 |   42.9688 |        - |   473880 B |
|                TinyCsvParser |       1000 |      50 |  9,093.217 us |  38.7631 us |  36.2590 us |   6.02 |    0.05 |  2250.0000 |  296.8750 | 140.6250 |  1026248 B |
|                FastCsvParser |       1000 |      50 |  2,756.854 us |   9.3086 us |   8.7073 us |   1.83 |    0.01 |   558.5938 |  152.3438 |  74.2188 |  2295584 B |
|                    CsvHelper |       1000 |      50 |  7,363.637 us |  11.3476 us |  10.6146 us |   4.88 |    0.03 |  1664.0625 |         - |        - |  6992632 B |
|                  FileHelpers |       1000 |      50 |  8,819.738 us |  73.9704 us |  69.1919 us |   5.84 |    0.06 |   500.0000 |  250.0000 |        - |  3179315 B |
|                              |            |         |               |             |             |        |         |            |           |          |            |
|           **Beffyman_DsvParser** |      **10000** |      **10** |  **6,346.013 us** |  **37.7487 us** |  **35.3101 us** |   **2.14** |    **0.01** |   **367.1875** |  **156.2500** |  **54.6875** |  **3605709 B** |
|           Beffyman_DsvReader |      10000 |      10 |  2,962.788 us |   5.7218 us |   5.3522 us |   1.00 |    0.00 |          - |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |      10000 |      10 | 12,458.369 us |  53.6604 us |  50.1940 us |   4.20 |    0.02 |   843.7500 |  468.7500 | 234.3750 |  5744837 B |
| DelimiterSeparatedTextReader |      10000 |      10 |  2,618.015 us |  40.7375 us |  38.1058 us |   0.88 |    0.01 |          - |         - |        - |       96 B |
| DelimiterSeparatedTextParser |      10000 |      10 |  5,351.309 us |  14.8791 us |  13.1899 us |   1.81 |    0.01 |   273.4375 |  140.6250 |  54.6875 |  1623008 B |
|                TinyCsvParser |      10000 |      10 | 12,506.556 us | 137.9875 us | 122.3223 us |   4.22 |    0.04 |  5671.8750 |  578.1250 | 187.5000 |  1592821 B |
|                FastCsvParser |      10000 |      10 |  5,544.757 us |  34.4279 us |  32.2039 us |   1.87 |    0.01 |   835.9375 |   85.9375 |  39.0625 |  3616838 B |
|                    CsvHelper |      10000 |      10 | 15,223.088 us |  55.6789 us |  52.0821 us |   5.14 |    0.02 |  3062.5000 |   15.6250 |        - | 12861456 B |
|                  FileHelpers |      10000 |      10 | 20,698.829 us | 105.2601 us |  98.4603 us |   6.99 |    0.04 |  1062.5000 |  468.7500 | 156.2500 |  6046361 B |
|                              |            |         |               |             |             |        |         |            |           |          |            |
|           **Beffyman_DsvParser** |      **10000** |      **50** | **28,057.323 us** | **134.4875 us** | **119.2197 us** |   **2.10** |    **0.01** |  **1531.2500** |  **625.0000** | **187.5000** | **15608740 B** |
|           Beffyman_DsvReader |      10000 |      50 | 13,327.111 us |  59.4265 us |  55.5876 us |   1.00 |    0.00 |          - |         - |        - |      168 B |
|    Beffyman_DsvParserGeneric |      10000 |      50 | 79,190.781 us | 235.8311 us | 209.0581 us |   5.94 |    0.03 |  4428.5714 | 2000.0000 | 714.2857 | 30551513 B |
| DelimiterSeparatedTextReader |      10000 |      50 | 15,706.589 us | 291.6552 us | 286.4444 us |   1.18 |    0.02 |          - |         - |        - |       96 B |
| DelimiterSeparatedTextParser |      10000 |      50 | 23,413.460 us |  76.2552 us |  71.3291 us |   1.76 |    0.01 |   843.7500 |  437.5000 | 156.2500 |  4823838 B |
|                TinyCsvParser |      10000 |      50 | 61,123.966 us | 740.8558 us | 692.9970 us |   4.59 |    0.06 | 22625.0000 | 2625.0000 | 875.0000 |  7434248 B |
|                FastCsvParser |      10000 |      50 | 26,395.176 us | 398.6174 us | 372.8670 us |   1.98 |    0.03 |  4625.0000 |   31.2500 |        - | 19653112 B |
|                    CsvHelper |      10000 |      50 | 77,620.844 us | 862.3019 us | 806.5978 us |   5.82 |    0.07 | 16571.4286 |         - |        - | 69717824 B |
|                  FileHelpers |      10000 |      50 | 93,439.892 us | 392.4096 us | 347.8609 us |   7.01 |    0.04 |  5500.0000 | 2333.3333 | 833.3333 | 30930733 B |
