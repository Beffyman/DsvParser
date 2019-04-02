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

		//[Fact]
		//public void SimpleData_Huge()
		//{
		//	string file = FileGenerator("Column", "Data", 50000, 3);

		//	var data = new DsvData(file, DsvOptions.DefaultCsvOptions);

		//	Assert.Equal(3, data.Headers.Length);
		//	Assert.Equal(50000, data.Rows.Length);

		//	Assert.Equal("Column1", data.Headers[0]);
		//	Assert.Equal("Column2", data.Headers[1]);
		//	Assert.Equal("Column3", data.Headers[2]);

		//	for (int i = 0; i < 50000; i++)
		//	{
		//		Assert.Equal("Data1", data.Rows[i].Span[0]);
		//		Assert.Equal("Data2", data.Rows[i].Span[1]);
		//		Assert.Equal("Data3", data.Rows[i].Span[2]);
		//	}
		//}



		[Fact]
		public void EscapedDelimiter()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}\"Da,ta1\",\"Da,ta2\",\"Da,ta3\"";

			var data = new DsvData(file, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Headers.Length);
			Assert.Equal(1, data.Rows.Length);

			Assert.Equal("Column1", data.Headers[0]);
			Assert.Equal("Column2", data.Headers[1]);
			Assert.Equal("Column3", data.Headers[2]);

			Assert.Equal("Da,ta1", data.Rows[0].Span[0]);
			Assert.Equal("Da,ta2", data.Rows[0].Span[1]);
			Assert.Equal("Da,ta3", data.Rows[0].Span[2]);
		}

		[Fact]
		public void EscapedLineFeed()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}\"Da{Environment.NewLine}ta1\",\"Da{Environment.NewLine}ta2\",\"Da{Environment.NewLine}ta3\"";

			var data = new DsvData(file, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Headers.Length);
			Assert.Equal(1, data.Rows.Length);

			Assert.Equal("Column1", data.Headers[0]);
			Assert.Equal("Column2", data.Headers[1]);
			Assert.Equal("Column3", data.Headers[2]);

			Assert.Equal($"Da{Environment.NewLine}ta1", data.Rows[0].Span[0]);
			Assert.Equal($"Da{Environment.NewLine}ta2", data.Rows[0].Span[1]);
			Assert.Equal($"Da{Environment.NewLine}ta3", data.Rows[0].Span[2]);
		}


		[Fact]
		public void EscapedEscape_Data()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}\"Da\"\"ta1\",\"Da\"\"ta2\",\"Da\"\"ta3\"";

			var data = new DsvData(file, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Headers.Length);
			Assert.Equal(1, data.Rows.Length);

			Assert.Equal("Column1", data.Headers[0]);
			Assert.Equal("Column2", data.Headers[1]);
			Assert.Equal("Column3", data.Headers[2]);

			Assert.Equal("Da\"ta1", data.Rows[0].Span[0]);
			Assert.Equal("Da\"ta2", data.Rows[0].Span[1]);
			Assert.Equal("Da\"ta3", data.Rows[0].Span[2]);
		}


		[Fact]
		public void EscapedEscape_Columns()
		{
			string file = $"\"Column\"\"1\",\"Column\"\"2\",\"Column\"\"3\"{Environment.NewLine}Data1,Data2,Data3";

			var data = new DsvData(file, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Headers.Length);
			Assert.Equal(1, data.Rows.Length);

			Assert.Equal("Column\"1", data.Headers[0]);
			Assert.Equal("Column\"2", data.Headers[1]);
			Assert.Equal("Column\"3", data.Headers[2]);

			Assert.Equal("Data1", data.Rows[0].Span[0]);
			Assert.Equal("Data2", data.Rows[0].Span[1]);
			Assert.Equal("Data3", data.Rows[0].Span[2]);
		}

		[Fact]
		public void MalformedHeader_SingleQuoteInMiddle()
		{
			string file = $"\"Col\"umn1\",Column2,Column3{Environment.NewLine},,";

			Assert.Throws<FormatException>(() =>
			{
				var data = new DsvData(file, DsvOptions.DefaultCsvOptions);
			});
		}

		[Fact]
		public void EscapedEscape_Multiple()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}\"Da\"\"\"\"ta1\",\"Data2\"\"\"\"\",\"\"\"\"\"Da\"\"ta3\"";

			var data = new DsvData(file, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Headers.Length);
			Assert.Equal(1, data.Rows.Length);

			Assert.Equal("Column1", data.Headers[0]);
			Assert.Equal("Column2", data.Headers[1]);
			Assert.Equal("Column3", data.Headers[2]);

			Assert.Equal("Da\"\"ta1", data.Rows[0].Span[0]);
			Assert.Equal("Data2\"\"", data.Rows[0].Span[1]);
			Assert.Equal("\"\"Da\"ta3", data.Rows[0].Span[2]);
		}

		[Fact]
		public void EmptyDataFields()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}Data1,,Data3";

			var data = new DsvData(file, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Headers.Length);
			Assert.Equal(1, data.Rows.Length);

			Assert.Equal("Column1", data.Headers[0]);
			Assert.Equal("Column2", data.Headers[1]);
			Assert.Equal("Column3", data.Headers[2]);

			Assert.Equal("Data1", data.Rows[0].Span[0]);
			Assert.Equal("", data.Rows[0].Span[1]);
			Assert.Equal("Data3", data.Rows[0].Span[2]);
		}

		[Fact]
		public void EndsWithNewLine()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}Data1,Data2,Data3{Environment.NewLine}";

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
	}
}
