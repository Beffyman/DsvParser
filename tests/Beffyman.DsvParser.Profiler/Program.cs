using System;
using System.Diagnostics;
using System.Text;

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

			Stopwatch timer = new Stopwatch();
			timer.Start();

			var data = new DsvParser(file.AsMemory(), DsvOptions.DefaultCsvOptions);

			Console.WriteLine("Elapsed Time");
			Console.WriteLine(timer.Elapsed.TotalSeconds);
			Console.WriteLine($"# of Columns = {data.Columns.Length.ToString()}");
			Console.WriteLine($"# of Rows = {data.Rows.Length.ToString()}");
		}
	}
}
