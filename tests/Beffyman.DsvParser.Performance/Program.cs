using System;
using System.Globalization;
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
				builder.Append($"{headerPrefix}{(++x).ToString()}");
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
					builder.Append($"{dataPrefix}{(++z).ToString()}");
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
		private DsvOptions csvOptions = DsvOptions.DefaultCsvOptions;
		private string csvDelimiter;
		private string newLine = Environment.NewLine;

		[GlobalSetup]
		public void Setup()
		{
			StringFile = FileGenerator("column", "data", Param_Rows, Columns);
			CharArrayFile = FileGenerator("column", "data", Param_Rows, Columns).ToCharArray();
			MemoryFile = FileGenerator("column", "data", Param_Rows, Columns).AsMemory();
			ByteArrayFile = System.Text.Encoding.UTF8.GetBytes(FileGenerator("column", "data", Param_Rows, Columns));
			csvDelimiter = csvOptions.Delimiter.ToString();
		}

		#region Beffyman.DsvParser


		[BenchmarkCategory("MemoryFile")]
		[Benchmark]
		public DsvParser Beffyman_DsvParser()
		{
			return new DsvParser(MemoryFile, Encoding.UTF8, DsvOptions.DefaultCsvOptions);
		}

		[BenchmarkCategory("MemoryFile")]
		[Benchmark()]
		public int Beffyman_DsvReaderWithValue()
		{
			var reader = new DsvReader(MemoryFile, Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			int totalLength = 0;
			while (reader.MoveNext())
			{
				var val = reader.ReadNextAsSpan();
				totalLength += val.Length;
			}

			return totalLength;
		}

		[BenchmarkCategory("MemoryFile")]
		[Benchmark(Baseline = true)]
		public void Beffyman_DsvReader()
		{
			var reader = new DsvReader(MemoryFile, Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			while (reader.MoveNext())
			{
			}

		}

		[BenchmarkCategory("MemoryFile")]
		[Benchmark]
		public void Beffyman_DsvParserGeneric()
		{
			if (Columns == 10)
			{
				var parser = new DsvParser<Record10, DsvRecordMapping10>(MemoryFile, Encoding.UTF8, DsvOptions.DefaultCsvOptions);
			}
			else
			{
				var parser = new DsvParser<Record50, DsvRecordMapping50>(MemoryFile, Encoding.UTF8, DsvOptions.DefaultCsvOptions);
			}
		}

		private sealed class DsvRecordMapping10 : DsvParserTypeMapping<Record10>
		{
			public DsvRecordMapping10()
			{
				this.MapProperty(0, new DsvParserMapperDelegate<Record10>((ref Record10 r, in ReadOnlySpan<char> data) => r.column1 = data.ToString()));
				this.MapProperty(1, new DsvParserMapperDelegate<Record10>((ref Record10 r, in ReadOnlySpan<char> data) => r.column2 = data.ToString()));
				this.MapProperty(2, new DsvParserMapperDelegate<Record10>((ref Record10 r, in ReadOnlySpan<char> data) => r.column3 = data.ToString()));
				this.MapProperty(3, new DsvParserMapperDelegate<Record10>((ref Record10 r, in ReadOnlySpan<char> data) => r.column4 = data.ToString()));
				this.MapProperty(4, new DsvParserMapperDelegate<Record10>((ref Record10 r, in ReadOnlySpan<char> data) => r.column5 = data.ToString()));
				this.MapProperty(5, new DsvParserMapperDelegate<Record10>((ref Record10 r, in ReadOnlySpan<char> data) => r.column6 = data.ToString()));
				this.MapProperty(6, new DsvParserMapperDelegate<Record10>((ref Record10 r, in ReadOnlySpan<char> data) => r.column7 = data.ToString()));
				this.MapProperty(7, new DsvParserMapperDelegate<Record10>((ref Record10 r, in ReadOnlySpan<char> data) => r.column8 = data.ToString()));
				this.MapProperty(8, new DsvParserMapperDelegate<Record10>((ref Record10 r, in ReadOnlySpan<char> data) => r.column9 = data.ToString()));
				this.MapProperty(9, new DsvParserMapperDelegate<Record10>((ref Record10 r, in ReadOnlySpan<char> data) => r.column10 = data.ToString()));
			}
		}


		private sealed class DsvRecordMapping50 : DsvParserTypeMapping<Record50>
		{
			public DsvRecordMapping50()
			{
				this.MapProperty(0, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column1 = data.ToString()));
				this.MapProperty(1, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column2 = data.ToString()));
				this.MapProperty(2, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column3 = data.ToString()));
				this.MapProperty(3, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column4 = data.ToString()));
				this.MapProperty(4, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column5 = data.ToString()));
				this.MapProperty(5, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column6 = data.ToString()));
				this.MapProperty(6, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column7 = data.ToString()));
				this.MapProperty(7, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column8 = data.ToString()));
				this.MapProperty(8, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column9 = data.ToString()));
				this.MapProperty(9, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column10 = data.ToString()));
				this.MapProperty(10, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column11 = data.ToString()));
				this.MapProperty(11, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column12 = data.ToString()));
				this.MapProperty(12, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column13 = data.ToString()));
				this.MapProperty(13, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column14 = data.ToString()));
				this.MapProperty(14, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column15 = data.ToString()));
				this.MapProperty(15, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column16 = data.ToString()));
				this.MapProperty(16, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column17 = data.ToString()));
				this.MapProperty(17, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column18 = data.ToString()));
				this.MapProperty(18, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column19 = data.ToString()));
				this.MapProperty(19, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column20 = data.ToString()));
				this.MapProperty(20, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column21 = data.ToString()));
				this.MapProperty(21, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column22 = data.ToString()));
				this.MapProperty(22, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column23 = data.ToString()));
				this.MapProperty(23, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column24 = data.ToString()));
				this.MapProperty(24, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column25 = data.ToString()));
				this.MapProperty(25, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column26 = data.ToString()));
				this.MapProperty(26, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column27 = data.ToString()));
				this.MapProperty(27, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column28 = data.ToString()));
				this.MapProperty(28, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column29 = data.ToString()));
				this.MapProperty(29, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column30 = data.ToString()));
				this.MapProperty(30, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column31 = data.ToString()));
				this.MapProperty(31, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column32 = data.ToString()));
				this.MapProperty(32, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column33 = data.ToString()));
				this.MapProperty(33, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column34 = data.ToString()));
				this.MapProperty(34, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column35 = data.ToString()));
				this.MapProperty(35, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column36 = data.ToString()));
				this.MapProperty(36, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column37 = data.ToString()));
				this.MapProperty(37, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column38 = data.ToString()));
				this.MapProperty(38, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column39 = data.ToString()));
				this.MapProperty(39, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column40 = data.ToString()));
				this.MapProperty(40, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column41 = data.ToString()));
				this.MapProperty(41, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column42 = data.ToString()));
				this.MapProperty(42, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column43 = data.ToString()));
				this.MapProperty(43, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column44 = data.ToString()));
				this.MapProperty(44, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column45 = data.ToString()));
				this.MapProperty(45, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column46 = data.ToString()));
				this.MapProperty(46, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column47 = data.ToString()));
				this.MapProperty(47, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column48 = data.ToString()));
				this.MapProperty(48, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column49 = data.ToString()));
				this.MapProperty(49, new DsvParserMapperDelegate<Record50>((ref Record50 r, in ReadOnlySpan<char> data) => r.column50 = data.ToString()));
			}
		}


		#endregion Beffyman.DsvParser

		#region DelimiterSeparatedTextParser

		[BenchmarkCategory("MemoryFile")]
		[Benchmark]
		public int DelimiterSeparatedTextReader()
		{
			var totalLength = 0;
			var reader = new DelimiterSeparatedTextParser.DsvReader(MemoryFile, csvDelimiter, newLine);

			while (reader.MoveNextRecord())
			{
				while (reader.MoveNextValue())
				{
					var value = reader.Current;
					totalLength += value.Length;
				}
			}

			return totalLength;
		}


		[BenchmarkCategory("MemoryFile")]
		[Benchmark]
		public int DelimiterSeparatedTextParser()
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

		[BenchmarkCategory("MemoryFile")]
		[Benchmark]
		public int DelimiterSeparatedTextParser_WithString()
		{
			var totalLength = 0;
			var parser = new DelimiterSeparatedTextParser.CsvParser(MemoryFile);

			var numRecords = parser.RecordsLength;
			for (var recordNum = 0; recordNum < numRecords; recordNum++)
			{
				var numValues = parser.GetRecordLength(recordNum);
				for (var valueNum = 0; valueNum < numValues; valueNum++)
				{
					var value = parser.GetValue(recordNum, valueNum).ToString();
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
		public int TinyCsvParser()
		{
			var totalLength = 0;
			if (Columns == 10)
			{
				var csvParserOptions = new TinyCsvParser.CsvParserOptions(false, ',', 4, true);
				var csvReaderOptions = new TinyCsvParser.CsvReaderOptions(NewLine);
				var csvMapper = new TinyCsvRecordMapping10();
				var csvParser = new TinyCsvParser.CsvParser<Record10>(csvParserOptions, csvMapper);

				var results = csvParser.ReadFromString(csvReaderOptions, StringFile);
				foreach (var result in results)
				{
					var record = result.Result;
					totalLength += record.column1.Length;
					totalLength += record.column2.Length;
					totalLength += record.column3.Length;
					totalLength += record.column4.Length;
					totalLength += record.column5.Length;
					totalLength += record.column6.Length;
					totalLength += record.column7.Length;
					totalLength += record.column8.Length;
					totalLength += record.column9.Length;
					totalLength += record.column10.Length;
				}

				return totalLength;
			}
			else
			{
				var csvParserOptions = new TinyCsvParser.CsvParserOptions(false, ',', 4, true);
				var csvReaderOptions = new TinyCsvParser.CsvReaderOptions(NewLine);
				var csvMapper = new TinyCsvRecordMapping50();
				var csvParser = new TinyCsvParser.CsvParser<Record50>(csvParserOptions, csvMapper);

				var results = csvParser.ReadFromString(csvReaderOptions, StringFile);
				foreach (var result in results)
				{
					var record = result.Result;
					totalLength += record.column1.Length;
					totalLength += record.column2.Length;
					totalLength += record.column3.Length;
					totalLength += record.column4.Length;
					totalLength += record.column5.Length;
					totalLength += record.column6.Length;
					totalLength += record.column7.Length;
					totalLength += record.column8.Length;
					totalLength += record.column9.Length;
					totalLength += record.column10.Length;
					totalLength += record.column11.Length;
					totalLength += record.column12.Length;
					totalLength += record.column13.Length;
					totalLength += record.column14.Length;
					totalLength += record.column15.Length;
					totalLength += record.column16.Length;
					totalLength += record.column17.Length;
					totalLength += record.column18.Length;
					totalLength += record.column19.Length;
					totalLength += record.column20.Length;
					totalLength += record.column21.Length;
					totalLength += record.column22.Length;
					totalLength += record.column23.Length;
					totalLength += record.column24.Length;
					totalLength += record.column25.Length;
					totalLength += record.column26.Length;
					totalLength += record.column27.Length;
					totalLength += record.column28.Length;
					totalLength += record.column29.Length;
					totalLength += record.column30.Length;
					totalLength += record.column31.Length;
					totalLength += record.column32.Length;
					totalLength += record.column33.Length;
					totalLength += record.column34.Length;
					totalLength += record.column35.Length;
					totalLength += record.column36.Length;
					totalLength += record.column37.Length;
					totalLength += record.column38.Length;
					totalLength += record.column39.Length;
					totalLength += record.column40.Length;
					totalLength += record.column41.Length;
					totalLength += record.column42.Length;
					totalLength += record.column43.Length;
					totalLength += record.column44.Length;
					totalLength += record.column45.Length;
					totalLength += record.column46.Length;
					totalLength += record.column47.Length;
					totalLength += record.column48.Length;
					totalLength += record.column49.Length;
					totalLength += record.column50.Length;
				}

				return totalLength;
			}

		}

		private sealed class TinyCsvRecordMapping10 : TinyCsvParser.Mapping.CsvMapping<Record10>
		{
			public TinyCsvRecordMapping10()
			{
				this.MapProperty(0, x => x.column1);
				this.MapProperty(1, x => x.column2);
				this.MapProperty(2, x => x.column3);
				this.MapProperty(3, x => x.column4);
				this.MapProperty(4, x => x.column5);
				this.MapProperty(5, x => x.column6);
				this.MapProperty(6, x => x.column7);
				this.MapProperty(7, x => x.column8);
				this.MapProperty(8, x => x.column9);
				this.MapProperty(9, x => x.column10);
			}
		}
		private sealed class TinyCsvRecordMapping50 : TinyCsvParser.Mapping.CsvMapping<Record50>
		{
			public TinyCsvRecordMapping50()
			{
				this.MapProperty(0, x => x.column1);
				this.MapProperty(1, x => x.column2);
				this.MapProperty(2, x => x.column3);
				this.MapProperty(3, x => x.column4);
				this.MapProperty(4, x => x.column5);
				this.MapProperty(5, x => x.column6);
				this.MapProperty(6, x => x.column7);
				this.MapProperty(7, x => x.column8);
				this.MapProperty(8, x => x.column9);
				this.MapProperty(9, x => x.column10);
				this.MapProperty(10, x => x.column11);
				this.MapProperty(11, x => x.column12);
				this.MapProperty(12, x => x.column13);
				this.MapProperty(13, x => x.column14);
				this.MapProperty(14, x => x.column15);
				this.MapProperty(15, x => x.column16);
				this.MapProperty(16, x => x.column17);
				this.MapProperty(17, x => x.column18);
				this.MapProperty(18, x => x.column19);
				this.MapProperty(19, x => x.column20);
				this.MapProperty(20, x => x.column21);
				this.MapProperty(21, x => x.column22);
				this.MapProperty(22, x => x.column23);
				this.MapProperty(23, x => x.column24);
				this.MapProperty(24, x => x.column25);
				this.MapProperty(25, x => x.column26);
				this.MapProperty(26, x => x.column27);
				this.MapProperty(27, x => x.column28);
				this.MapProperty(28, x => x.column29);
				this.MapProperty(29, x => x.column30);
				this.MapProperty(30, x => x.column31);
				this.MapProperty(31, x => x.column32);
				this.MapProperty(32, x => x.column33);
				this.MapProperty(33, x => x.column34);
				this.MapProperty(34, x => x.column35);
				this.MapProperty(35, x => x.column36);
				this.MapProperty(36, x => x.column37);
				this.MapProperty(37, x => x.column38);
				this.MapProperty(38, x => x.column39);
				this.MapProperty(39, x => x.column40);
				this.MapProperty(40, x => x.column41);
				this.MapProperty(41, x => x.column42);
				this.MapProperty(42, x => x.column43);
				this.MapProperty(43, x => x.column44);
				this.MapProperty(44, x => x.column45);
				this.MapProperty(45, x => x.column46);
				this.MapProperty(46, x => x.column47);
				this.MapProperty(47, x => x.column48);
				this.MapProperty(48, x => x.column49);
				this.MapProperty(49, x => x.column50);
			}
		}


		#endregion TinyCsvParser


		#region FastCsvParser

		[BenchmarkCategory("ByteArrayFile")]
		[Benchmark]
		public int FastCsvParser()
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
		public int CsvHelper()
		{
			var totalLength = 0;

			using (var reader = new StringReader(StringFile))
			using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.CurrentCulture))
			{
				while (csv.Read())
				{
					var numValues = csv.ColumnCount;
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
		public int FileHelpers()
		{
			var totalLength = 0;
			if (Columns == 10)
			{
				var engine = new FileHelpers.FileHelperEngine<Record10>();

				var records = engine.ReadString(StringFile);
				var numRecords = records.Length;
				for (var recordNum = 0; recordNum < numRecords; recordNum++)
				{
					var record = records[recordNum];
					totalLength += record.column1.Length;
					totalLength += record.column2.Length;
					totalLength += record.column3.Length;
					totalLength += record.column4.Length;
					totalLength += record.column5.Length;
					totalLength += record.column6.Length;
					totalLength += record.column7.Length;
					totalLength += record.column8.Length;
					totalLength += record.column9.Length;
					totalLength += record.column10.Length;
				}

				return totalLength;
			}
			else
			{
				var engine = new FileHelpers.FileHelperEngine<Record50>();

				var records = engine.ReadString(StringFile);
				var numRecords = records.Length;
				for (var recordNum = 0; recordNum < numRecords; recordNum++)
				{
					var record = records[recordNum];
					totalLength += record.column1.Length;
					totalLength += record.column2.Length;
					totalLength += record.column3.Length;
					totalLength += record.column4.Length;
					totalLength += record.column5.Length;
					totalLength += record.column6.Length;
					totalLength += record.column7.Length;
					totalLength += record.column8.Length;
					totalLength += record.column9.Length;
					totalLength += record.column10.Length;
					totalLength += record.column11.Length;
					totalLength += record.column12.Length;
					totalLength += record.column13.Length;
					totalLength += record.column14.Length;
					totalLength += record.column15.Length;
					totalLength += record.column16.Length;
					totalLength += record.column17.Length;
					totalLength += record.column18.Length;
					totalLength += record.column19.Length;
					totalLength += record.column20.Length;
					totalLength += record.column21.Length;
					totalLength += record.column22.Length;
					totalLength += record.column23.Length;
					totalLength += record.column24.Length;
					totalLength += record.column25.Length;
					totalLength += record.column26.Length;
					totalLength += record.column27.Length;
					totalLength += record.column28.Length;
					totalLength += record.column29.Length;
					totalLength += record.column30.Length;
					totalLength += record.column31.Length;
					totalLength += record.column32.Length;
					totalLength += record.column33.Length;
					totalLength += record.column34.Length;
					totalLength += record.column35.Length;
					totalLength += record.column36.Length;
					totalLength += record.column37.Length;
					totalLength += record.column38.Length;
					totalLength += record.column39.Length;
					totalLength += record.column40.Length;
					totalLength += record.column41.Length;
					totalLength += record.column42.Length;
					totalLength += record.column43.Length;
					totalLength += record.column44.Length;
					totalLength += record.column45.Length;
					totalLength += record.column46.Length;
					totalLength += record.column47.Length;
					totalLength += record.column48.Length;
					totalLength += record.column49.Length;
					totalLength += record.column50.Length;
				}

				return totalLength;
			}

		}


		[FileHelpers.DelimitedRecord(",")]
		private sealed class Record10
		{
			public string column1 { get; set; }

			public string column2 { get; set; }

			public string column3 { get; set; }

			public string column4 { get; set; }

			public string column5 { get; set; }

			public string column6 { get; set; }

			public string column7 { get; set; }

			public string column8 { get; set; }

			public string column9 { get; set; }

			public string column10 { get; set; }
		}

		[FileHelpers.DelimitedRecord(",")]
		private sealed class Record50
		{
			public string column1 { get; set; }

			public string column2 { get; set; }

			public string column3 { get; set; }

			public string column4 { get; set; }

			public string column5 { get; set; }

			public string column6 { get; set; }

			public string column7 { get; set; }

			public string column8 { get; set; }

			public string column9 { get; set; }

			public string column10 { get; set; }
			public string column11 { get; set; }
			public string column12 { get; set; }
			public string column13 { get; set; }
			public string column14 { get; set; }
			public string column15 { get; set; }
			public string column16 { get; set; }
			public string column17 { get; set; }
			public string column18 { get; set; }
			public string column19 { get; set; }
			public string column20 { get; set; }
			public string column21 { get; set; }
			public string column22 { get; set; }
			public string column23 { get; set; }
			public string column24 { get; set; }
			public string column25 { get; set; }
			public string column26 { get; set; }
			public string column27 { get; set; }
			public string column28 { get; set; }
			public string column29 { get; set; }
			public string column30 { get; set; }
			public string column31 { get; set; }
			public string column32 { get; set; }
			public string column33 { get; set; }
			public string column34 { get; set; }
			public string column35 { get; set; }
			public string column36 { get; set; }
			public string column37 { get; set; }
			public string column38 { get; set; }
			public string column39 { get; set; }
			public string column40 { get; set; }
			public string column41 { get; set; }
			public string column42 { get; set; }
			public string column43 { get; set; }
			public string column44 { get; set; }
			public string column45 { get; set; }
			public string column46 { get; set; }
			public string column47 { get; set; }
			public string column48 { get; set; }
			public string column49 { get; set; }
			public string column50 { get; set; }
		}
		#endregion FileHelpers


	}
}
