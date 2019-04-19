using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beffyman.DsvParser.Profiler
{
	public static class Program
	{
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

		static void Main(string[] args)
		{
			//var file = FileGenerator("Column", "Data", 10000, 100);
			var file = FileGenerator("Column", "Data", 1_000_000, 100);
			//var file = FileGenerator("Column", "Data", 100_000_00, 10);
			//var file = FileGenerator("Column", "Data", 100, 1_00_000);

			Console.WriteLine("Done generating csv");
			Thread.Sleep(5000);
			Stopwatch timer = new Stopwatch();
			timer.Start();

			var parser = new DsvParser(file.AsSpan(), DsvOptions.DefaultCsvOptions);

			timer.Stop();
			Console.WriteLine(nameof(DsvParser));
			Console.WriteLine("Elapsed Time");
			Console.WriteLine(timer.Elapsed.TotalSeconds);
			Console.WriteLine($"# of Columns = {parser.Columns.Length.ToString()}");
			Console.WriteLine($"# of Rows = {parser.Rows.Count.ToString()}");

			Thread.Sleep(5000);
			timer.Restart();


			var genericParser = new DsvParser<Record100, DsvRecordMapping100>(file.AsSpan(), DsvOptions.DefaultCsvOptions);

			timer.Stop();
			Console.WriteLine($"{nameof(DsvParser)}<{nameof(Record100)}, {nameof(DsvRecordMapping100)}> ");
			Console.WriteLine("Elapsed Time");
			Console.WriteLine(timer.Elapsed.TotalSeconds);
			Console.WriteLine($"# of Columns = {genericParser.Columns.Length.ToString()}");
			Console.WriteLine($"# of Rows = {genericParser.Rows.Count.ToString()}");

		}

		private sealed class DsvRecordMapping100 : DsvParserTypeMapping<Record100>
		{
			public DsvRecordMapping100()
			{
				this.MapProperty(0, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column1 = data.ToString()));
				this.MapProperty(1, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column2 = data.ToString()));
				this.MapProperty(2, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column3 = data.ToString()));
				this.MapProperty(3, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column4 = data.ToString()));
				this.MapProperty(4, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column5 = data.ToString()));
				this.MapProperty(5, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column6 = data.ToString()));
				this.MapProperty(6, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column7 = data.ToString()));
				this.MapProperty(7, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column8 = data.ToString()));
				this.MapProperty(8, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column9 = data.ToString()));
				this.MapProperty(9, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column10 = data.ToString()));
				this.MapProperty(10, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column11 = data.ToString()));
				this.MapProperty(11, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column12 = data.ToString()));
				this.MapProperty(12, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column13 = data.ToString()));
				this.MapProperty(13, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column14 = data.ToString()));
				this.MapProperty(14, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column15 = data.ToString()));
				this.MapProperty(15, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column16 = data.ToString()));
				this.MapProperty(16, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column17 = data.ToString()));
				this.MapProperty(17, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column18 = data.ToString()));
				this.MapProperty(18, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column19 = data.ToString()));
				this.MapProperty(19, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column20 = data.ToString()));
				this.MapProperty(20, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column21 = data.ToString()));
				this.MapProperty(21, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column22 = data.ToString()));
				this.MapProperty(22, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column23 = data.ToString()));
				this.MapProperty(23, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column24 = data.ToString()));
				this.MapProperty(24, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column25 = data.ToString()));
				this.MapProperty(25, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column26 = data.ToString()));
				this.MapProperty(26, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column27 = data.ToString()));
				this.MapProperty(27, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column28 = data.ToString()));
				this.MapProperty(28, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column29 = data.ToString()));
				this.MapProperty(29, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column30 = data.ToString()));
				this.MapProperty(30, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column31 = data.ToString()));
				this.MapProperty(31, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column32 = data.ToString()));
				this.MapProperty(32, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column33 = data.ToString()));
				this.MapProperty(33, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column34 = data.ToString()));
				this.MapProperty(34, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column35 = data.ToString()));
				this.MapProperty(35, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column36 = data.ToString()));
				this.MapProperty(36, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column37 = data.ToString()));
				this.MapProperty(37, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column38 = data.ToString()));
				this.MapProperty(38, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column39 = data.ToString()));
				this.MapProperty(39, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column40 = data.ToString()));
				this.MapProperty(40, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column41 = data.ToString()));
				this.MapProperty(41, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column42 = data.ToString()));
				this.MapProperty(42, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column43 = data.ToString()));
				this.MapProperty(43, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column44 = data.ToString()));
				this.MapProperty(44, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column45 = data.ToString()));
				this.MapProperty(45, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column46 = data.ToString()));
				this.MapProperty(46, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column47 = data.ToString()));
				this.MapProperty(47, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column48 = data.ToString()));
				this.MapProperty(48, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column49 = data.ToString()));
				this.MapProperty(49, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column50 = data.ToString()));
				this.MapProperty(50, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column51 = data.ToString()));
				this.MapProperty(51, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column52 = data.ToString()));
				this.MapProperty(52, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column53 = data.ToString()));
				this.MapProperty(53, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column54 = data.ToString()));
				this.MapProperty(54, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column55 = data.ToString()));
				this.MapProperty(55, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column56 = data.ToString()));
				this.MapProperty(56, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column57 = data.ToString()));
				this.MapProperty(57, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column58 = data.ToString()));
				this.MapProperty(58, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column59 = data.ToString()));
				this.MapProperty(59, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column60 = data.ToString()));
				this.MapProperty(60, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column61 = data.ToString()));
				this.MapProperty(61, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column62 = data.ToString()));
				this.MapProperty(62, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column63 = data.ToString()));
				this.MapProperty(63, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column64 = data.ToString()));
				this.MapProperty(64, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column65 = data.ToString()));
				this.MapProperty(65, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column66 = data.ToString()));
				this.MapProperty(66, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column67 = data.ToString()));
				this.MapProperty(67, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column68 = data.ToString()));
				this.MapProperty(68, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column69 = data.ToString()));
				this.MapProperty(69, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column70 = data.ToString()));
				this.MapProperty(70, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column71 = data.ToString()));
				this.MapProperty(71, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column72 = data.ToString()));
				this.MapProperty(72, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column73 = data.ToString()));
				this.MapProperty(73, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column74 = data.ToString()));
				this.MapProperty(74, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column75 = data.ToString()));
				this.MapProperty(75, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column76 = data.ToString()));
				this.MapProperty(76, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column77 = data.ToString()));
				this.MapProperty(77, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column78 = data.ToString()));
				this.MapProperty(78, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column79 = data.ToString()));
				this.MapProperty(79, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column80 = data.ToString()));
				this.MapProperty(80, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column81 = data.ToString()));
				this.MapProperty(81, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column82 = data.ToString()));
				this.MapProperty(82, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column83 = data.ToString()));
				this.MapProperty(83, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column84 = data.ToString()));
				this.MapProperty(84, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column85 = data.ToString()));
				this.MapProperty(85, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column86 = data.ToString()));
				this.MapProperty(86, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column87 = data.ToString()));
				this.MapProperty(87, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column88 = data.ToString()));
				this.MapProperty(88, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column89 = data.ToString()));
				this.MapProperty(89, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column90 = data.ToString()));
				this.MapProperty(90, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column91 = data.ToString()));
				this.MapProperty(91, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column92 = data.ToString()));
				this.MapProperty(92, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column93 = data.ToString()));
				this.MapProperty(93, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column94 = data.ToString()));
				this.MapProperty(94, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column95 = data.ToString()));
				this.MapProperty(95, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column96 = data.ToString()));
				this.MapProperty(96, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column97 = data.ToString()));
				this.MapProperty(97, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column98 = data.ToString()));
				this.MapProperty(98, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column99 = data.ToString()));
				this.MapProperty(99, new DsvParserMapperDelegate<Record100>((ref Record100 r, in ReadOnlySpan<char> data) => r.column100 = data.ToString()));
			}
		}

		private class Record100
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
			public string column51 { get; set; }
			public string column52 { get; set; }
			public string column53 { get; set; }
			public string column54 { get; set; }
			public string column55 { get; set; }
			public string column56 { get; set; }
			public string column57 { get; set; }
			public string column58 { get; set; }
			public string column59 { get; set; }
			public string column60 { get; set; }
			public string column61 { get; set; }
			public string column62 { get; set; }
			public string column63 { get; set; }
			public string column64 { get; set; }
			public string column65 { get; set; }
			public string column66 { get; set; }
			public string column67 { get; set; }
			public string column68 { get; set; }
			public string column69 { get; set; }
			public string column70 { get; set; }
			public string column71 { get; set; }
			public string column72 { get; set; }
			public string column73 { get; set; }
			public string column74 { get; set; }
			public string column75 { get; set; }
			public string column76 { get; set; }
			public string column77 { get; set; }
			public string column78 { get; set; }
			public string column79 { get; set; }
			public string column80 { get; set; }
			public string column81 { get; set; }
			public string column82 { get; set; }
			public string column83 { get; set; }
			public string column84 { get; set; }
			public string column85 { get; set; }
			public string column86 { get; set; }
			public string column87 { get; set; }
			public string column88 { get; set; }
			public string column89 { get; set; }
			public string column90 { get; set; }
			public string column91 { get; set; }
			public string column92 { get; set; }
			public string column93 { get; set; }
			public string column94 { get; set; }
			public string column95 { get; set; }
			public string column96 { get; set; }
			public string column97 { get; set; }
			public string column98 { get; set; }
			public string column99 { get; set; }
			public string column100 { get; set; }
		}
	}
}
