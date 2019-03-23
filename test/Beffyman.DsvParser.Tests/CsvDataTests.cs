using System;
using System.Linq;
using System.Text;
using Xunit;

namespace Beffyman.DsvParser.Tests
{
	public class CsvDataTests
	{
		private string FileGenerator(string headerPrefix, string dataPrefix, int rows, int columns)
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

		[Fact]
		public void SimpleData_OneDataRow()
		{
			string file = FileGenerator("Column", "Data", 1, 3);

			var data = new DsvData(file, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Headers.Length);
			Assert.Equal(1, data.Rows.Length);

			Assert.Equal("Column1", data.Headers[0]);
			Assert.Equal("Column2", data.Headers[1]);
			Assert.Equal("Column3", data.Headers[2]);

			Assert.Equal("Data1", data.Rows[0].Span[0]);
			Assert.Equal("Data2", data.Rows[0].Span[1]);
			Assert.Equal("Data3", data.Rows[0].Span[2]);
		}

		[Fact]
		public void SimpleData_MultipleDataRows()
		{
			string file = FileGenerator("Column", "Data", 5, 3);

			var data = new DsvData(file, DsvOptions.DefaultCsvOptions);

			Assert.Equal(3, data.Headers.Length);
			Assert.Equal(5, data.Rows.Length);

			Assert.Equal("Column1", data.Headers[0]);
			Assert.Equal("Column2", data.Headers[1]);
			Assert.Equal("Column3", data.Headers[2]);

			for (int i = 0; i < 5; i++)
			{
				Assert.Equal("Data1", data.Rows[i].Span[0]);
				Assert.Equal("Data2", data.Rows[i].Span[1]);
				Assert.Equal("Data3", data.Rows[i].Span[2]);
			}
		}

		[Fact]
		public void SimpleData_Huge()
		{
			string file = FileGenerator("Column", "Data", 50000, 3);

			var data = new DsvData(file, DsvOptions.DefaultCsvOptions);

			Assert.Equal(3, data.Headers.Length);
			Assert.Equal(50000, data.Rows.Length);

			Assert.Equal("Column1", data.Headers[0]);
			Assert.Equal("Column2", data.Headers[1]);
			Assert.Equal("Column3", data.Headers[2]);

			for (int i = 0; i < 50000; i++)
			{
				Assert.Equal("Data1", data.Rows[i].Span[0]);
				Assert.Equal("Data2", data.Rows[i].Span[1]);
				Assert.Equal("Data3", data.Rows[i].Span[2]);
			}
		}
	}
}
