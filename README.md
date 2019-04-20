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
as of commit: 65315f6722abdf9bffbbb0f1e46d0be8119fc5cf
``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.437 (1809/October2018Update/Redstone5)
AMD Ryzen 7 2700X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=3.0.100-preview4-011223
  [Host]     : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT
  DefaultJob : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT


```
|                                  Method | Param_Rows | Columns |          Mean |         Error |        StdDev |  Ratio | RatioSD |      Gen 0 |     Gen 1 |     Gen 2 |  Allocated |
|---------------------------------------- |----------- |-------- |--------------:|--------------:|--------------:|-------:|--------:|-----------:|----------:|----------:|-----------:|
|                      **Beffyman_DsvParser** |         **10** |      **10** |      **5.433 us** |     **0.0275 us** |     **0.0229 us** |   **1.61** |    **0.01** |     **1.1292** |         **-** |         **-** |     **4736 B** |
|             Beffyman_DsvReaderWithValue |         10 |      10 |      3.345 us |     0.0200 us |     0.0177 us |   0.99 |    0.01 |     0.0381 |         - |         - |      168 B |
|                      Beffyman_DsvReader |         10 |      10 |      3.384 us |     0.0230 us |     0.0204 us |   1.00 |    0.00 |     0.0381 |         - |         - |      168 B |
|               Beffyman_DsvParserGeneric |         10 |      10 |     10.439 us |     0.1744 us |     0.1631 us |   3.08 |    0.05 |     1.8921 |         - |         - |     7920 B |
|            DelimiterSeparatedTextReader |         10 |      10 |      2.925 us |     0.0131 us |     0.0123 us |   0.86 |    0.01 |     0.0229 |         - |         - |       96 B |
|            DelimiterSeparatedTextParser |         10 |      10 |      4.991 us |     0.0593 us |     0.0554 us |   1.48 |    0.02 |     0.5112 |         - |         - |     2152 B |
| DelimiterSeparatedTextParser_WithString |         10 |      10 |      5.792 us |     0.1030 us |     0.0964 us |   1.71 |    0.03 |     1.3885 |         - |         - |     5832 B |
|                           TinyCsvParser |         10 |      10 |    805.789 us |    12.4962 us |    11.6890 us | 238.35 |    3.14 |    25.3906 |   12.6953 |    0.9766 |    74188 B |
|                           FastCsvParser |         10 |      10 |     19.787 us |     0.2902 us |     0.2714 us |   5.85 |    0.11 |    41.6565 |   41.6565 |   41.6565 |   207920 B |
|                               CsvHelper |         10 |      10 |     19.071 us |     0.3299 us |     0.3240 us |   5.64 |    0.12 |     6.4697 |         - |         - |    27096 B |
|                             FileHelpers |         10 |      10 |    641.164 us |     4.0465 us |     3.5871 us | 189.47 |    1.90 |     6.8359 |    3.9063 |         - |    29364 B |
|                                         |            |         |               |               |               |        |         |            |           |           |            |
|                      **Beffyman_DsvParser** |         **10** |      **50** |     **23.307 us** |     **0.3868 us** |     **0.3618 us** |   **1.36** |    **0.02** |     **3.8452** |    **1.9226** |         **-** |    **19680 B** |
|             Beffyman_DsvReaderWithValue |         10 |      50 |     18.277 us |     0.1735 us |     0.1623 us |   1.06 |    0.01 |     0.0305 |         - |         - |      168 B |
|                      Beffyman_DsvReader |         10 |      50 |     17.199 us |     0.1039 us |     0.0972 us |   1.00 |    0.00 |     0.0305 |         - |         - |      168 B |
|               Beffyman_DsvParserGeneric |         10 |      50 |     44.935 us |     0.8138 us |     1.1671 us |   2.63 |    0.08 |     9.3384 |    1.1597 |         - |    39296 B |
|            DelimiterSeparatedTextReader |         10 |      50 |     14.300 us |     0.1013 us |     0.0948 us |   0.83 |    0.01 |     0.0153 |         - |         - |       96 B |
|            DelimiterSeparatedTextParser |         10 |      50 |     21.688 us |     0.2186 us |     0.2045 us |   1.26 |    0.02 |     1.4648 |         - |         - |     6168 B |
| DelimiterSeparatedTextParser_WithString |         10 |      50 |     29.298 us |     0.5664 us |     0.5563 us |   1.70 |    0.03 |     6.5613 |         - |         - |    27448 B |
|                           TinyCsvParser |         10 |      50 |  3,854.720 us |    14.4551 us |    12.8141 us | 224.16 |    1.45 |   101.5625 |   46.8750 |    7.8125 |   321233 B |
|                           FastCsvParser |         10 |      50 |     46.558 us |     0.3110 us |     0.2909 us |   2.71 |    0.02 |    41.6260 |   41.6260 |   41.6260 |   254976 B |
|                               CsvHelper |         10 |      50 |     93.996 us |     0.9887 us |     0.8765 us |   5.47 |    0.07 |    21.4844 |         - |         - |    90096 B |
|                             FileHelpers |         10 |      50 |  2,761.168 us |    43.2089 us |    40.4177 us | 160.54 |    2.61 |    31.2500 |    7.8125 |         - |   137384 B |
|                                         |            |         |               |               |               |        |         |            |           |           |            |
|                      **Beffyman_DsvParser** |       **1000** |      **10** |    **485.451 us** |     **5.5303 us** |     **5.1730 us** |   **1.72** |    **0.03** |    **67.3828** |   **56.6406** |   **33.2031** |   **342106 B** |
|             Beffyman_DsvReaderWithValue |       1000 |      10 |    303.950 us |     1.8866 us |     1.7647 us |   1.08 |    0.01 |          - |         - |         - |      168 B |
|                      Beffyman_DsvReader |       1000 |      10 |    282.574 us |     2.6904 us |     2.5166 us |   1.00 |    0.00 |          - |         - |         - |      168 B |
|               Beffyman_DsvParserGeneric |       1000 |      10 |    987.089 us |     7.9691 us |     7.4543 us |   3.49 |    0.04 |   109.3750 |   66.4063 |   37.1094 |   566712 B |
|            DelimiterSeparatedTextReader |       1000 |      10 |    254.123 us |     2.4899 us |     2.2072 us |   0.90 |    0.01 |          - |         - |         - |       96 B |
|            DelimiterSeparatedTextParser |       1000 |      10 |    435.302 us |     4.0981 us |     3.8333 us |   1.54 |    0.02 |    36.1328 |   11.7188 |         - |   153064 B |
| DelimiterSeparatedTextParser_WithString |       1000 |      10 |    492.661 us |     5.1212 us |     4.7904 us |   1.74 |    0.02 |   114.2578 |    2.9297 |         - |   481464 B |
|                           TinyCsvParser |       1000 |      10 |  1,855.427 us |    21.7176 us |    19.2521 us |   6.57 |    0.10 |   617.1875 |  185.5469 |         - |   224758 B |
|                           FastCsvParser |       1000 |      10 |    621.452 us |     8.7688 us |     8.2023 us |   2.20 |    0.03 |   124.0234 |   41.0156 |   41.0156 |   532640 B |
|                               CsvHelper |       1000 |      10 |  1,510.175 us |    12.2425 us |    11.4517 us |   5.34 |    0.07 |   310.5469 |         - |         - |  1299272 B |
|                             FileHelpers |       1000 |      10 |  1,864.193 us |    14.3811 us |    13.4521 us |   6.60 |    0.06 |   115.2344 |   48.8281 |         - |   615925 B |
|                                         |            |         |               |               |               |        |         |            |           |           |            |
|                      **Beffyman_DsvParser** |       **1000** |      **50** |  **2,824.555 us** |    **23.1692 us** |    **21.6725 us** |   **1.93** |    **0.03** |   **175.7813** |   **82.0313** |   **46.8750** |  **1545058 B** |
|             Beffyman_DsvReaderWithValue |       1000 |      50 |  1,570.687 us |    16.2963 us |    15.2436 us |   1.08 |    0.02 |          - |         - |         - |      168 B |
|                      Beffyman_DsvReader |       1000 |      50 |  1,460.711 us |    16.3620 us |    15.3050 us |   1.00 |    0.00 |          - |         - |         - |      168 B |
|               Beffyman_DsvParserGeneric |       1000 |      50 |  5,130.470 us |    31.2874 us |    29.2663 us |   3.51 |    0.04 |   562.5000 |  382.8125 |  164.0625 |  3053446 B |
|            DelimiterSeparatedTextReader |       1000 |      50 |  1,440.983 us |    38.6914 us |    36.1919 us |   0.99 |    0.03 |          - |         - |         - |       96 B |
|            DelimiterSeparatedTextParser |       1000 |      50 |  2,179.433 us |     6.6280 us |     5.8755 us |   1.49 |    0.02 |    85.9375 |   42.9688 |         - |   473880 B |
| DelimiterSeparatedTextParser_WithString |       1000 |      50 |  2,466.437 us |     8.5223 us |     7.9718 us |   1.69 |    0.02 |   398.4375 |  195.3125 |         - |  2403880 B |
|                           TinyCsvParser |       1000 |      50 |  9,422.022 us |    63.1361 us |    59.0575 us |   6.45 |    0.08 |  2265.6250 |  312.5000 |  156.2500 |  1026248 B |
|                           FastCsvParser |       1000 |      50 |  2,701.715 us |    21.8570 us |    17.0645 us |   1.85 |    0.03 |   558.5938 |  152.3438 |   74.2188 |  2295654 B |
|                               CsvHelper |       1000 |      50 |  7,069.994 us |    36.4791 us |    34.1226 us |   4.84 |    0.05 |  1664.0625 |         - |         - |  6992632 B |
|                             FileHelpers |       1000 |      50 |  8,749.851 us |    64.8672 us |    60.6769 us |   5.99 |    0.08 |   500.0000 |  250.0000 |         - |  3179315 B |
|                                         |            |         |               |               |               |        |         |            |           |           |            |
|                      **Beffyman_DsvParser** |      **10000** |      **10** |  **6,270.082 us** |    **24.3603 us** |    **22.7867 us** |   **2.24** |    **0.01** |   **367.1875** |  **156.2500** |   **54.6875** |  **3605731 B** |
|             Beffyman_DsvReaderWithValue |      10000 |      10 |  3,057.488 us |     8.2831 us |     6.9168 us |   1.09 |    0.00 |          - |         - |         - |      168 B |
|                      Beffyman_DsvReader |      10000 |      10 |  2,802.950 us |     8.5857 us |     8.0311 us |   1.00 |    0.00 |          - |         - |         - |      168 B |
|               Beffyman_DsvParserGeneric |      10000 |      10 | 12,705.519 us |    44.2288 us |    39.2077 us |   4.53 |    0.02 |   843.7500 |  468.7500 |  234.3750 |  5745014 B |
|            DelimiterSeparatedTextReader |      10000 |      10 |  2,936.235 us |    48.4533 us |    45.3232 us |   1.05 |    0.02 |          - |         - |         - |       96 B |
|            DelimiterSeparatedTextParser |      10000 |      10 |  5,418.742 us |    32.1441 us |    30.0676 us |   1.93 |    0.01 |   273.4375 |  140.6250 |   54.6875 |  1622960 B |
| DelimiterSeparatedTextParser_WithString |      10000 |      10 |  6,393.986 us |    23.9554 us |    21.2359 us |   2.28 |    0.01 |   890.6250 |  343.7500 |  164.0625 |  4903479 B |
|                           TinyCsvParser |      10000 |      10 | 12,022.817 us |   129.3606 us |   121.0040 us |   4.29 |    0.04 |  5718.7500 |  640.6250 |  218.7500 |  1592818 B |
|                           FastCsvParser |      10000 |      10 |  5,754.387 us |    47.6463 us |    44.5684 us |   2.05 |    0.02 |   835.9375 |   85.9375 |   39.0625 |  3616922 B |
|                               CsvHelper |      10000 |      10 | 15,306.199 us |    74.9647 us |    66.4543 us |   5.46 |    0.03 |  3062.5000 |   15.6250 |         - | 12861456 B |
|                             FileHelpers |      10000 |      10 | 20,760.544 us |    63.9170 us |    59.7880 us |   7.41 |    0.03 |  1062.5000 |  468.7500 |  156.2500 |  6046391 B |
|                                         |            |         |               |               |               |        |         |            |           |           |            |
|                      **Beffyman_DsvParser** |      **10000** |      **50** | **26,319.359 us** |    **76.1525 us** |    **63.5908 us** |   **1.40** |    **0.00** |  **1531.2500** |  **625.0000** |  **187.5000** | **15608740 B** |
|             Beffyman_DsvReaderWithValue |      10000 |      50 | 13,613.101 us |    84.6595 us |    70.6945 us |   0.72 |    0.00 |          - |         - |         - |      168 B |
|                      Beffyman_DsvReader |      10000 |      50 | 18,856.470 us |    13.9330 us |    13.0329 us |   1.00 |    0.00 |          - |         - |         - |      168 B |
|               Beffyman_DsvParserGeneric |      10000 |      50 | 73,831.387 us |   734.7520 us |   651.3385 us |   3.92 |    0.03 |  4428.5714 | 2000.0000 |  714.2857 | 30551890 B |
|            DelimiterSeparatedTextReader |      10000 |      50 | 14,541.223 us |   260.1878 us |   243.3799 us |   0.77 |    0.01 |          - |         - |         - |       96 B |
|            DelimiterSeparatedTextParser |      10000 |      50 | 23,072.655 us |    62.1338 us |    58.1200 us |   1.22 |    0.00 |   843.7500 |  437.5000 |  156.2500 |  4824021 B |
| DelimiterSeparatedTextParser_WithString |      10000 |      50 | 32,358.974 us |   165.1374 us |   154.4696 us |   1.72 |    0.01 |  5687.5000 | 2000.0000 |  937.5000 | 24105750 B |
|                           TinyCsvParser |      10000 |      50 | 60,921.473 us | 1,073.1774 us | 1,003.8508 us |   3.23 |    0.05 | 22625.0000 | 3000.0000 | 1000.0000 |  7434248 B |
|                           FastCsvParser |      10000 |      50 | 27,286.917 us |   536.5185 us |   734.3926 us |   1.46 |    0.05 |  4625.0000 |   31.2500 |         - | 19653112 B |
|                               CsvHelper |      10000 |      50 | 80,208.201 us | 1,525.7175 us | 1,695.8310 us |   4.27 |    0.09 | 16571.4286 |         - |         - | 69717824 B |
|                             FileHelpers |      10000 |      50 | 99,695.631 us |   782.7273 us |   732.1636 us |   5.29 |    0.04 |  5400.0000 | 2400.0000 |  800.0000 | 30929880 B |
