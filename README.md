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

### DsvOptions
- Options object that allow custom DSV formats

---
## Benchmark

``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.437 (1809/October2018Update/Redstone5)
AMD Ryzen 7 2700X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=3.0.100-preview3-010431
  [Host]     : .NET Core 3.0.0-preview3-27503-5 (CoreCLR 4.6.27422.72, CoreFX 4.7.19.12807), 64bit RyuJIT
  DefaultJob : .NET Core 3.0.0-preview3-27503-5 (CoreCLR 4.6.27422.72, CoreFX 4.7.19.12807), 64bit RyuJIT


```
|                       Method | Param_Rows | Columns |           Mean |         Error |        StdDev |  Ratio | RatioSD |      Gen 0 |     Gen 1 |     Gen 2 |  Allocated |
|----------------------------- |----------- |-------- |---------------:|--------------:|--------------:|-------:|--------:|-----------:|----------:|----------:|-----------:|
|           **Beffyman_DsvParser** |         **10** |      **10** |       **4.220 us** |     **0.0215 us** |     **0.0201 us** |   **1.26** |    **0.01** |     **1.1749** |    **0.0076** |         **-** |     **4928 B** |
|           Beffyman_DsvReader |         10 |      10 |       3.358 us |     0.0251 us |     0.0210 us |   1.00 |    0.00 |     0.0381 |         - |         - |      168 B |
| DelimiterSeparatedTextReader |         10 |      10 |       2.825 us |     0.0143 us |     0.0127 us |   0.84 |    0.01 |     0.0229 |    0.0038 |         - |       96 B |
| DelimiterSeparatedTextParser |         10 |      10 |       5.169 us |     0.0520 us |     0.0486 us |   1.54 |    0.02 |     0.5112 |    0.0076 |         - |     2152 B |
|                TinyCsvParser |         10 |      10 |     734.483 us |     1.4048 us |     1.2453 us | 218.72 |    1.42 |    25.3906 |   12.6953 |    0.9766 |    74188 B |
|                FastCsvParser |         10 |      10 |      19.832 us |     0.1705 us |     0.1595 us |   5.90 |    0.07 |    41.6565 |   41.6565 |   41.6565 |   207920 B |
|                    CsvHelper |         10 |      10 |      19.596 us |     0.1076 us |     0.0954 us |   5.83 |    0.04 |     6.4697 |         - |         - |    27096 B |
|                  FileHelpers |         10 |      10 |     586.909 us |     2.6434 us |     2.3433 us | 174.67 |    1.39 |     6.8359 |    1.9531 |         - |    29364 B |
|                              |            |         |                |               |               |        |         |            |           |           |            |
|           **Beffyman_DsvParser** |         **10** |      **50** |      **20.555 us** |     **0.1368 us** |     **0.1280 us** |   **1.03** |    **0.01** |     **4.7302** |    **0.0305** |         **-** |    **19872 B** |
|           Beffyman_DsvReader |         10 |      50 |      19.960 us |     0.2173 us |     0.1927 us |   1.00 |    0.00 |     0.0305 |         - |         - |      168 B |
| DelimiterSeparatedTextReader |         10 |      50 |      13.737 us |     0.0788 us |     0.0699 us |   0.69 |    0.01 |     0.0153 |         - |         - |       96 B |
| DelimiterSeparatedTextParser |         10 |      50 |      23.502 us |     0.0948 us |     0.0841 us |   1.18 |    0.01 |     1.4648 |    0.0305 |         - |     6168 B |
|                TinyCsvParser |         10 |      50 |   3,525.334 us |     7.1533 us |     6.3412 us | 176.64 |    1.70 |   105.4688 |   50.7813 |    7.8125 |   321233 B |
|                FastCsvParser |         10 |      50 |      47.951 us |     0.3662 us |     0.3246 us |   2.40 |    0.04 |    41.6260 |   41.6260 |   41.6260 |   254976 B |
|                    CsvHelper |         10 |      50 |      88.646 us |     0.5950 us |     0.4968 us |   4.44 |    0.06 |    21.4844 |    0.1221 |         - |    90096 B |
|                  FileHelpers |         10 |      50 |   2,497.746 us |    15.4550 us |    14.4567 us | 125.16 |    1.08 |    31.2500 |         - |         - |   137384 B |
|                              |            |         |                |               |               |        |         |            |           |           |            |
|           **Beffyman_DsvParser** |       **1000** |      **10** |     **482.252 us** |     **9.0037 us** |     **9.2461 us** |   **1.51** |    **0.03** |    **63.9648** |   **38.0859** |   **38.0859** |   **358088 B** |
|           Beffyman_DsvReader |       1000 |      10 |     320.041 us |     1.2593 us |     1.1780 us |   1.00 |    0.00 |          - |         - |         - |      168 B |
| DelimiterSeparatedTextReader |       1000 |      10 |     255.747 us |     2.3307 us |     2.0661 us |   0.80 |    0.01 |          - |         - |         - |       96 B |
| DelimiterSeparatedTextParser |       1000 |      10 |     453.844 us |     6.5849 us |     6.1596 us |   1.42 |    0.02 |    36.1328 |    0.4883 |         - |   153064 B |
|                TinyCsvParser |       1000 |      10 |   1,778.944 us |     3.6005 us |     3.3679 us |   5.56 |    0.03 |   617.1875 |  187.5000 |         - |   224758 B |
|                FastCsvParser |       1000 |      10 |     632.202 us |     2.6190 us |     2.4498 us |   1.98 |    0.01 |   123.0469 |   81.0547 |   41.0156 |   532640 B |
|                    CsvHelper |       1000 |      10 |   1,487.611 us |    20.2475 us |    18.9395 us |   4.65 |    0.07 |   308.5938 |    1.9531 |         - |  1299272 B |
|                  FileHelpers |       1000 |      10 |   1,924.342 us |    11.7435 us |    10.9849 us |   6.01 |    0.04 |   115.2344 |   42.9688 |         - |   615925 B |
|                              |            |         |                |               |               |        |         |            |           |           |            |
|           **Beffyman_DsvParser** |       **1000** |      **50** |   **3,011.493 us** |    **14.7047 us** |    **13.7548 us** |   **1.79** |    **0.01** |   **179.6875** |   **85.9375** |   **42.9688** |  **1561061 B** |
|           Beffyman_DsvReader |       1000 |      50 |   1,681.902 us |     8.2759 us |     7.3363 us |   1.00 |    0.00 |          - |         - |         - |      168 B |
| DelimiterSeparatedTextReader |       1000 |      50 |   1,268.477 us |    17.5154 us |    15.5269 us |   0.75 |    0.01 |          - |         - |         - |       96 B |
| DelimiterSeparatedTextParser |       1000 |      50 |   2,025.104 us |     7.3000 us |     6.8284 us |   1.20 |    0.01 |    85.9375 |   42.9688 |         - |   473880 B |
|                TinyCsvParser |       1000 |      50 |   9,294.381 us |    72.2563 us |    64.0533 us |   5.53 |    0.04 |  2250.0000 |  312.5000 |  140.6250 |  1026250 B |
|                FastCsvParser |       1000 |      50 |   2,826.773 us |    18.7662 us |    17.5539 us |   1.68 |    0.02 |   558.5938 |  152.3438 |   74.2188 |  2295638 B |
|                    CsvHelper |       1000 |      50 |   7,734.984 us |    40.3844 us |    33.7228 us |   4.60 |    0.02 |  1664.0625 |    7.8125 |         - |  6992632 B |
|                  FileHelpers |       1000 |      50 |   9,530.308 us |    64.6815 us |    60.5031 us |   5.67 |    0.04 |   500.0000 |  250.0000 |         - |  3179315 B |
|                              |            |         |                |               |               |        |         |            |           |           |            |
|           **Beffyman_DsvParser** |      **10000** |      **10** |   **7,150.047 us** |    **94.7478 us** |    **88.6271 us** |   **2.23** |    **0.03** |   **375.0000** |  **164.0625** |   **62.5000** |  **3765722 B** |
|           Beffyman_DsvReader |      10000 |      10 |   3,201.067 us |    14.0252 us |    13.1192 us |   1.00 |    0.00 |          - |         - |         - |      168 B |
| DelimiterSeparatedTextReader |      10000 |      10 |   2,514.378 us |    19.8429 us |    18.5611 us |   0.79 |    0.01 |          - |         - |         - |       96 B |
| DelimiterSeparatedTextParser |      10000 |      10 |   5,706.119 us |    25.8299 us |    24.1613 us |   1.78 |    0.01 |   273.4375 |  140.6250 |   54.6875 |  1623017 B |
|                TinyCsvParser |      10000 |      10 |  14,748.822 us |   289.3516 us |   284.1820 us |   4.61 |    0.10 |  5718.7500 |  671.8750 |  218.7500 |  1592821 B |
|                FastCsvParser |      10000 |      10 |   5,352.265 us |    28.5536 us |    26.7091 us |   1.67 |    0.01 |   835.9375 |   85.9375 |   39.0625 |  3616800 B |
|                    CsvHelper |      10000 |      10 |  17,184.862 us |   132.3429 us |   117.3185 us |   5.37 |    0.05 |  3062.5000 |   31.2500 |         - | 12861456 B |
|                  FileHelpers |      10000 |      10 |  25,483.525 us |   388.5038 us |   363.4067 us |   7.96 |    0.11 |  1062.5000 |  468.7500 |  156.2500 |  6046284 B |
|                              |            |         |                |               |               |        |         |            |           |           |            |
|           **Beffyman_DsvParser** |      **10000** |      **50** |  **25,857.643 us** |   **386.2540 us** |   **361.3022 us** |   **1.77** |    **0.03** |  **1562.5000** |  **656.2500** |  **218.7500** | **15768934 B** |
|           Beffyman_DsvReader |      10000 |      50 |  14,593.899 us |    49.0148 us |    43.4504 us |   1.00 |    0.00 |          - |         - |         - |      168 B |
| DelimiterSeparatedTextReader |      10000 |      50 |  12,210.810 us |   115.9586 us |    90.5329 us |   0.84 |    0.01 |          - |         - |         - |       96 B |
| DelimiterSeparatedTextParser |      10000 |      50 |  24,257.323 us |   460.6411 us |   430.8840 us |   1.66 |    0.03 |   843.7500 |  437.5000 |  156.2500 |  4823888 B |
|                TinyCsvParser |      10000 |      50 |  68,703.486 us | 1,335.7965 us | 1,915.7594 us |   4.71 |    0.11 | 22625.0000 | 3125.0000 | 1000.0000 |  7434248 B |
|                FastCsvParser |      10000 |      50 |  27,488.568 us |   289.0247 us |   270.3538 us |   1.88 |    0.02 |  4625.0000 |   31.2500 |         - | 19653112 B |
|                    CsvHelper |      10000 |      50 |  72,239.958 us |   334.8600 us |   313.2282 us |   4.95 |    0.03 | 16571.4286 |  142.8571 |         - | 69717824 B |
|                  FileHelpers |      10000 |      50 | 111,978.491 us | 1,658.8533 us | 1,470.5303 us |   7.67 |    0.10 |  5400.0000 | 2400.0000 |  800.0000 | 30929587 B |
