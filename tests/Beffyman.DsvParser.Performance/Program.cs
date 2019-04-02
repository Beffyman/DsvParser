using System;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using TinyCsvParser;
using FastCsvParser = global::CsvParser.CsvReader;

namespace Beffyman.DsvParser.Performance
{
	[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
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

		[Params(10, 1_000, 10_000)]
		public int Param_Rows { get; set; }
		[Params(10, 50)]
		public int Columns { get; set; }

		private string StringFile;
		private char[] CharArrayFile;
		private ReadOnlyMemory<char> MemoryFile;
		private byte[] ByteArrayFile;

		[GlobalSetup]
		public void Setup()
		{
			StringFile = FileGenerator("column", "data", Param_Rows, Columns);
			CharArrayFile = FileGenerator("column", "data", Param_Rows, Columns).ToCharArray();
			MemoryFile = FileGenerator("column", "data", Param_Rows, Columns).AsMemory();
			ByteArrayFile = System.Text.Encoding.UTF8.GetBytes(FileGenerator("column", "data", Param_Rows, Columns));
		}

		#region Beffyman.DsvParser


		[BenchmarkCategory("MemoryFile")]
		[Benchmark]
		public DsvData Beffyman_DsvParser_MemoryFile()
		{
			return new DsvData(MemoryFile, DsvOptions.DefaultCsvOptions);
		}

		#endregion Beffyman.DsvParser

		#region DelimiterSeparatedTextParser


		[BenchmarkCategory("MemoryFile")]
		[Benchmark]
		public int DelimiterSeparatedTextParser_MemoryFile()
		{
			var totalLength = 0;
			var parser = new DelimiterSeparatedTextParser.CsvParser(MemoryFile);

			var numRecords = parser.RecordsLength;
			for (var recordNum = 0; recordNum < numRecords; recordNum++)
			{
				var numValues = parser.GetRecordLength(recordNum);
				for (var valueNum = 0; valueNum < numValues; valueNum++)
				{
					var value = parser.GetValue(recordNum, valueNum);
					totalLength += value.Length;
				}
			}

			return totalLength;
		}

		#endregion DelimiterSeparatedTextParser


		#region TinyCsvParser

		private string[] NewLine = new string[] { Environment.NewLine };

		[BenchmarkCategory("StringFile")]
		[Benchmark]
		public int TinyCsvParser_StringFile()
		{
			var totalLength = 0;

			var csvParserOptions = new TinyCsvParser.CsvParserOptions(false, ',', 4, true);
			var csvReaderOptions = new TinyCsvParser.CsvReaderOptions(NewLine);
			var csvMapper = new TinyCsvRecordMapping();
			var csvParser = new TinyCsvParser.CsvParser<Record>(csvParserOptions, csvMapper);

			var results = csvParser.ReadFromString(csvReaderOptions, StringFile);
			foreach (var result in results)
			{
				var record = result.Result;
				totalLength += record.Value0.Length;
				totalLength += record.Value1.Length;
				totalLength += record.Value2.Length;
				totalLength += record.Value3.Length;
				totalLength += record.Value4.Length;
				totalLength += record.Value5.Length;
				totalLength += record.Value6.Length;
				totalLength += record.Value7.Length;
				totalLength += record.Value8.Length;
				totalLength += record.Value9.Length;
			}

			return totalLength;
		}

		private sealed class TinyCsvRecordMapping : TinyCsvParser.Mapping.CsvMapping<Record>
		{
			public TinyCsvRecordMapping()
			{
				this.MapProperty(0, x => x.Value0);
				this.MapProperty(1, x => x.Value1);
				this.MapProperty(2, x => x.Value2);
				this.MapProperty(3, x => x.Value3);
				this.MapProperty(4, x => x.Value4);
				this.MapProperty(5, x => x.Value5);
				this.MapProperty(6, x => x.Value6);
				this.MapProperty(7, x => x.Value7);
				this.MapProperty(8, x => x.Value8);
				this.MapProperty(9, x => x.Value9);
			}
		}


		#endregion TinyCsvParser


		#region FastCsvParser

		[BenchmarkCategory("ByteArrayFile")]
		[Benchmark]
		public int FastCsvParser_ByteArrayFile()
		{
			var totalLength = 0;

			using (var stream = new MemoryStream(ByteArrayFile))
			using (var parser = new FastCsvParser(stream, Encoding.UTF8))
			{
				while (parser.MoveNext())
				{
					var values = parser.Current;
					var numValues = values.Count;
					for (var valueNum = 0; valueNum < numValues; valueNum++)
					{
						var value = values[valueNum];
						totalLength += value.Length;
					}
				}
			}

			return totalLength;
		}
		#endregion FastCsvParser



		#region CsvTextFieldParser

		#endregion CsvTextFieldParser



		#region CsvHelper

		[BenchmarkCategory("StringFile")]
		[Benchmark]
		public int CsvHelper_StringFile()
		{
			var totalLength = 0;

			using (var reader = new StringReader(StringFile))
			using (var csv = new CsvHelper.CsvReader(reader))
			{
				while (csv.Read())
				{
					var numValues = csv.Context.ColumnCount;
					for (var valueNum = 0; valueNum < numValues; valueNum++)
					{
						var value = csv[valueNum];
						totalLength += value.Length;
					}
				}
			}

			return totalLength;
		}

		#endregion CsvHelper


		#region FileHelpers

		[BenchmarkCategory("StringFile")]
		[Benchmark]
		public int FileHelpers_StringFile()
		{
			var totalLength = 0;

			var engine = new FileHelpers.FileHelperEngine<Record>();

			var records = engine.ReadString(StringFile);
			var numRecords = records.Length;
			for (var recordNum = 0; recordNum < numRecords; recordNum++)
			{
				var record = records[recordNum];
				totalLength += record.Value0.Length;
				totalLength += record.Value1.Length;
				totalLength += record.Value2.Length;
				totalLength += record.Value3.Length;
				totalLength += record.Value4.Length;
				totalLength += record.Value5.Length;
				totalLength += record.Value6.Length;
				totalLength += record.Value7.Length;
				totalLength += record.Value8.Length;
				totalLength += record.Value9.Length;
			}

			return totalLength;
		}


		[FileHelpers.DelimitedRecord(",")]
		private sealed class Record
		{
			public string Value0 { get; set; }

			public string Value1 { get; set; }

			public string Value2 { get; set; }

			public string Value3 { get; set; }

			public string Value4 { get; set; }

			public string Value5 { get; set; }

			public string Value6 { get; set; }

			public string Value7 { get; set; }

			public string Value8 { get; set; }

			public string Value9 { get; set; }
		}
		#endregion FileHelpers


	}
}
