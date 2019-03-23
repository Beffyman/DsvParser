using System;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Beffyman.DsvParser.Performance
{
	[MemoryDiagnoser]
	public class Program
	{
		public static void Main() => BenchmarkRunner.Run<Program>();


		private static string FileGenerator(string headerPrefix, string dataPrefix, int rows, int columns)
		{
			StringBuilder builder = new StringBuilder();
			int x = 0;

			for (int i = 0; i < columns; i++)
			{
				if (i != 0)
				{
					builder.Append(',');
				}
				builder.Append($"{headerPrefix}{++x}");
			}

			builder.AppendLine();

			for (int i = 0; i < rows; i++)
			{
				int z = 0;
				for (int u = 0; u < columns; u++)
				{
					if (u != 0)
					{
						builder.Append(',');
					}
					builder.Append($"{dataPrefix}{++z}");
				}
				builder.AppendLine();
			}

			return builder.ToString();
		}

		private static readonly string StringBigFile = FileGenerator("column", "data", 10, 10);
		private static readonly char[] CharArrayBigFile = FileGenerator("column", "data", 10, 10).ToCharArray();
		private static readonly ReadOnlyMemory<char> MemoryBigFile = FileGenerator("column", "data", 10, 10).AsMemory();
		private static readonly byte[] ByteArrayBigFile = System.Text.Encoding.UTF8.GetBytes(FileGenerator("column", "data", 10, 10));



		[Benchmark]
		public DsvData Parse_StringBigFile()
		{
			return new DsvData(StringBigFile, DsvOptions.DefaultCsvOptions);
		}


		[Benchmark]
		public DsvData Parse_CharArrayBigFile()
		{
			return new DsvData(CharArrayBigFile, DsvOptions.DefaultCsvOptions);
		}


		[Benchmark]
		public DsvData Parse_MemoryBigFile()
		{
			return new DsvData(MemoryBigFile, DsvOptions.DefaultCsvOptions);
		}


		[Benchmark]
		public DsvData Parse_ByteArrayBigFile()
		{
			return new DsvData(ByteArrayBigFile, Encoding.UTF8, DsvOptions.DefaultCsvOptions);
		}


	}
}
