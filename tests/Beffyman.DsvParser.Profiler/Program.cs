using System;
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

		static void Main(string[] args)
		{
			var file = FileGenerator("Column", "Data", 1000000, 50);
			var data = new DsvData(file.AsMemory(), DsvOptions.DefaultCsvOptions);

		}
	}
}
