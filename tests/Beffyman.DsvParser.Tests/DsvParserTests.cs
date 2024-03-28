using System;
using System.Text;
using Xunit;

namespace Beffyman.DsvParser.Tests
{
	public class DsvParserTests : BaseTest
	{
		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Constructor_MemoryBytes(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);
			var bytes = System.Text.Encoding.UTF8.GetBytes(file).AsMemory();


			var data = new DsvParser(bytes, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Columns.Length);
			Assert.Single(data.Rows);

			Assert.Equal("Column1", data.Columns.Span[0].ToString());
			Assert.Equal("Column2", data.Columns.Span[1].ToString());
			Assert.Equal("Column3", data.Columns.Span[2].ToString());

			Assert.Equal("Data1", data.Rows[0].Span[0].ToString());
			Assert.Equal("Data2", data.Rows[0].Span[1].ToString());
			Assert.Equal("Data3", data.Rows[0].Span[2].ToString());
		}

		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Constructor_SpanBytes(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);
			var bytes = System.Text.Encoding.UTF8.GetBytes(file).AsSpan();


			var data = new DsvParser(bytes, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Columns.Length);
			Assert.Single(data.Rows);

			Assert.Equal("Column1", data.Columns.Span[0].ToString());
			Assert.Equal("Column2", data.Columns.Span[1].ToString());
			Assert.Equal("Column3", data.Columns.Span[2].ToString());

			Assert.Equal("Data1", data.Rows[0].Span[0].ToString());
			Assert.Equal("Data2", data.Rows[0].Span[1].ToString());
			Assert.Equal("Data3", data.Rows[0].Span[2].ToString());
		}

		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Constructor_ByteArray(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);
			var bytes = System.Text.Encoding.UTF8.GetBytes(file);


			var data = new DsvParser(bytes, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Columns.Length);
			Assert.Single(data.Rows);

			Assert.Equal("Column1", data.Columns.Span[0].ToString());
			Assert.Equal("Column2", data.Columns.Span[1].ToString());
			Assert.Equal("Column3", data.Columns.Span[2].ToString());

			Assert.Equal("Data1", data.Rows[0].Span[0].ToString());
			Assert.Equal("Data2", data.Rows[0].Span[1].ToString());
			Assert.Equal("Data3", data.Rows[0].Span[2].ToString());
		}

		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Constructor_CharArray(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);
			var array = file.ToCharArray();


			var data = new DsvParser(array, Encoding.UTF8, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Columns.Length);
			Assert.Single(data.Rows);

			Assert.Equal("Column1", data.Columns.Span[0].ToString());
			Assert.Equal("Column2", data.Columns.Span[1].ToString());
			Assert.Equal("Column3", data.Columns.Span[2].ToString());

			Assert.Equal("Data1", data.Rows[0].Span[0].ToString());
			Assert.Equal("Data2", data.Rows[0].Span[1].ToString());
			Assert.Equal("Data3", data.Rows[0].Span[2].ToString());
		}

		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Constructor_Memory(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);
			var mem = file.AsMemory();


			var data = new DsvParser(mem, Encoding.UTF8, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Columns.Length);
			Assert.Single(data.Rows);

			Assert.Equal("Column1", data.Columns.Span[0].ToString());
			Assert.Equal("Column2", data.Columns.Span[1].ToString());
			Assert.Equal("Column3", data.Columns.Span[2].ToString());

			Assert.Equal("Data1", data.Rows[0].Span[0].ToString());
			Assert.Equal("Data2", data.Rows[0].Span[1].ToString());
			Assert.Equal("Data3", data.Rows[0].Span[2].ToString());
		}

		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Constructor_Span(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);
			var span = file.AsSpan();


			var data = new DsvParser(span, Encoding.UTF8, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Columns.Length);
			Assert.Single(data.Rows);

			Assert.Equal("Column1", data.Columns.Span[0].ToString());
			Assert.Equal("Column2", data.Columns.Span[1].ToString());
			Assert.Equal("Column3", data.Columns.Span[2].ToString());

			Assert.Equal("Data1", data.Rows[0].Span[0].ToString());
			Assert.Equal("Data2", data.Rows[0].Span[1].ToString());
			Assert.Equal("Data3", data.Rows[0].Span[2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void SimpleData_OneDataRow(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);

			var data = new DsvParser(file, Encoding.UTF8, DsvOptions.DefaultCsvOptions);


			Assert.Equal(3, data.Columns.Length);
			Assert.Single(data.Rows);

			Assert.Equal("Column1", data.Columns.Span[0].ToString());
			Assert.Equal("Column2", data.Columns.Span[1].ToString());
			Assert.Equal("Column3", data.Columns.Span[2].ToString());

			Assert.Equal("Data1", data.Rows[0].Span[0].ToString());
			Assert.Equal("Data2", data.Rows[0].Span[1].ToString());
			Assert.Equal("Data3", data.Rows[0].Span[2].ToString());
		}



		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void NoHeader(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);

			var data = new DsvParser(file, Encoding.UTF8, new DsvOptions(',', '"', false));


			Assert.Equal(0, data.Columns.Length);
			Assert.Equal(2, data.Rows.Count);

			Assert.Equal("Column1", data.Rows[0].Span[0].ToString());
			Assert.Equal("Column2", data.Rows[0].Span[1].ToString());
			Assert.Equal("Column3", data.Rows[0].Span[2].ToString());

			Assert.Equal("Data1", data.Rows[1].Span[0].ToString());
			Assert.Equal("Data2", data.Rows[1].Span[1].ToString());
			Assert.Equal("Data3", data.Rows[1].Span[2].ToString());
		}
	}
}
