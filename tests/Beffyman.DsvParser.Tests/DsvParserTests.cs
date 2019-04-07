using System;
using System.Linq;
using System.Text;
using Xunit;

namespace Beffyman.DsvParser.Tests
{
	public class DsvParserTests
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

			var data = new DsvParser(file, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Columns.Length);
			Assert.Equal(1, data.Rows.Length);

			Assert.Equal("Column1", data.Columns.Span[0].ToString());
			Assert.Equal("Column2", data.Columns.Span[1].ToString());
			Assert.Equal("Column3", data.Columns.Span[2].ToString());

			Assert.Equal("Data1", data.Rows.Span[0].Span[0].ToString());
			Assert.Equal("Data2", data.Rows.Span[0].Span[1].ToString());
			Assert.Equal("Data3", data.Rows.Span[0].Span[2].ToString());
		}



		[Fact]
		public void NoHeader()
		{
			string file = FileGenerator("Column", "Data", 1, 3);

			var data = new DsvParser(file, new DsvOptions(',', '"', false));


			Assert.Equal(0, data.Columns.Length);
			Assert.Equal(2, data.Rows.Length);

			Assert.Equal("Column1", data.Rows.Span[0].Span[0].ToString());
			Assert.Equal("Column2", data.Rows.Span[0].Span[1].ToString());
			Assert.Equal("Column3", data.Rows.Span[0].Span[2].ToString());

			Assert.Equal("Data1", data.Rows.Span[1].Span[0].ToString());
			Assert.Equal("Data2", data.Rows.Span[1].Span[1].ToString());
			Assert.Equal("Data3", data.Rows.Span[1].Span[2].ToString());
		}
	}
}