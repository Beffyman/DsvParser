using System;
using System.Linq;
using System.Text;
using Xunit;

namespace Beffyman.DsvParser.Tests
{
	public class DsvParserTests : BaseTest
	{
		[Fact]
		public void Constructor_MemoryBytes()
		{
			string file = FileGenerator("Column", "Data", 1, 3);
			var bytes = System.Text.Encoding.UTF8.GetBytes(file).AsMemory();


			var data = new DsvParser(bytes, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);


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
		public void Constructor_SpanBytes()
		{
			string file = FileGenerator("Column", "Data", 1, 3);
			var bytes = System.Text.Encoding.UTF8.GetBytes(file).AsSpan();


			var data = new DsvParser(bytes, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);


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
		public void Constructor_ByteArray()
		{
			string file = FileGenerator("Column", "Data", 1, 3);
			var bytes = System.Text.Encoding.UTF8.GetBytes(file);


			var data = new DsvParser(bytes, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);


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
		public void Constructor_CharArray()
		{
			string file = FileGenerator("Column", "Data", 1, 3);
			var array = file.ToCharArray();


			var data = new DsvParser(array, DsvOptions.DefaultCsvOptions);


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
		public void Constructor_Memory()
		{
			string file = FileGenerator("Column", "Data", 1, 3);
			var mem = file.AsMemory();


			var data = new DsvParser(mem, DsvOptions.DefaultCsvOptions);


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
		public void Constructor_Span()
		{
			string file = FileGenerator("Column", "Data", 1, 3);
			var span = file.AsSpan();


			var data = new DsvParser(span, DsvOptions.DefaultCsvOptions);


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
