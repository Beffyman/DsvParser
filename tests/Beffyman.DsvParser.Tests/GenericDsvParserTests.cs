using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Beffyman.DsvParser.Tests
{
	public class GenericDsvParserTests : BaseTest
	{

		public class Record
		{
			public int c0;
			public DateTime c1;
			public string c2;
			public bool c3;
			public TimeSpan c4;
		}

		public class RecordMapping : DsvParserTypeMapping<Record>
		{
			public RecordMapping()
			{
				this.MapProperty(0, new DsvParserMapperDelegate<Record>((ref Record r, in ReadOnlySpan<char> data) => r.c0 = int.Parse(data)));
				this.MapProperty(1, new DsvParserMapperDelegate<Record>((ref Record r, in ReadOnlySpan<char> data) => r.c1 = DateTime.Parse(data)));
				this.MapProperty(2, new DsvParserMapperDelegate<Record>((ref Record r, in ReadOnlySpan<char> data) => r.c2 = data.ToString()));
				this.MapProperty(3, new DsvParserMapperDelegate<Record>((ref Record r, in ReadOnlySpan<char> data) => r.c3 = bool.Parse(data)));
				this.MapProperty(4, new DsvParserMapperDelegate<Record>((ref Record r, in ReadOnlySpan<char> data) => r.c4 = TimeSpan.Parse(data)));
			}
		}

		public class NoRecordMapping : DsvParserTypeMapping<Record>
		{
			public NoRecordMapping()
			{

			}
		}


		[Fact]
		public void Constructor_MemoryChar()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";

			var parser = new DsvParser<Record, RecordMapping>(data.AsMemory(), DsvOptions.DefaultCsvOptions);
		}


		[Fact]
		public void Constructor_MemoryChar_MapperArg()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";

			var parser = new DsvParser<Record, RecordMapping>(data.AsMemory(), DsvOptions.DefaultCsvOptions, new RecordMapping());
		}


		[Fact]
		public void Constructor_byteArray()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";
			var bytes = System.Text.Encoding.UTF8.GetBytes(data);

			var parser = new DsvParser<Record, RecordMapping>(bytes, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);
		}

		[Fact]
		public void Constructor_byteArray_MapperArg()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";
			var bytes = System.Text.Encoding.UTF8.GetBytes(data);

			var parser = new DsvParser<Record, RecordMapping>(bytes, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions, new RecordMapping());
		}

		[Fact]
		public void Constructor_charArray()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";

			var parser = new DsvParser<Record, RecordMapping>(data.ToCharArray(), DsvOptions.DefaultCsvOptions);
		}

		[Fact]
		public void Constructor_charArray_MapperArg()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";

			var parser = new DsvParser<Record, RecordMapping>(data.ToCharArray(), DsvOptions.DefaultCsvOptions, new RecordMapping());
		}

		[Fact]
		public void Constructor_string()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";

			var parser = new DsvParser<Record, RecordMapping>(data, DsvOptions.DefaultCsvOptions);
		}

		[Fact]
		public void Constructor_string_MapperArg()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";

			var parser = new DsvParser<Record, RecordMapping>(data, DsvOptions.DefaultCsvOptions, new RecordMapping());
		}

		[Fact]
		public void Constructor_MemoryByte()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";
			var bytes = System.Text.Encoding.UTF8.GetBytes(data);

			var parser = new DsvParser<Record, RecordMapping>(bytes.AsMemory(), System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);
		}

		[Fact]
		public void Constructor_MemoryByte_MapperArg()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";
			var bytes = System.Text.Encoding.UTF8.GetBytes(data);

			var parser = new DsvParser<Record, RecordMapping>(bytes.AsMemory(), System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions, new RecordMapping());
		}


		[Fact]
		public void Constructor_SpanByte()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";
			var bytes = System.Text.Encoding.UTF8.GetBytes(data);

			var parser = new DsvParser<Record, RecordMapping>(bytes.AsSpan(), System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);
		}

		[Fact]
		public void Constructor_SpanByte_MapperArg()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";
			var bytes = System.Text.Encoding.UTF8.GetBytes(data);

			var parser = new DsvParser<Record, RecordMapping>(bytes.AsSpan(), System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions, new RecordMapping());
		}

		[Fact]
		public void Constructor_Span()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";

			var parser = new DsvParser<Record, RecordMapping>(data.AsSpan(), DsvOptions.DefaultCsvOptions);
		}

		[Fact]
		public void Constructor_Span_MapperArg()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";

			var parser = new DsvParser<Record, RecordMapping>(data.AsSpan(), DsvOptions.DefaultCsvOptions, new RecordMapping());
		}

		[Fact]
		public void ValidateParser()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";

			var parser = new DsvParser<Record, RecordMapping>(data, DsvOptions.DefaultCsvOptions);

			Assert.Equal(1, parser.Rows.Count);

			var record = parser.Rows[0];

			Assert.Equal(1, record.c0);
			Assert.Equal(DateTime.Parse("1/1/2017"), record.c1);
			Assert.Equal("hello", record.c2);
			Assert.True(record.c3);
			Assert.Equal(TimeSpan.Parse("02:10:01"), record.c4);
		}

		[Fact]
		public void TestNoMappings()
		{
			string data = $"c0,c1,c2,c3,c4{Environment.NewLine}1,1/1/2017,hello,true,02:10:01";

			var parser = new DsvParser<Record, NoRecordMapping>(data, DsvOptions.DefaultCsvOptions);

			Assert.Equal(1, parser.Rows.Count);

			var record = parser.Rows[0];

			Assert.Equal(default, record.c0);
			Assert.Equal(default, record.c1);
			Assert.Null(record.c2);
			Assert.False(record.c3);
			Assert.Equal(default, record.c4);
		}

	}
}
