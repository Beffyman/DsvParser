using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Beffyman.DsvParser.Tests
{
	public class DsvReaderTests : BaseTest
	{

		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Constructor_MemoryBytes(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);
			var bytes = System.Text.Encoding.UTF8.GetBytes(file).AsMemory();

			var data = new DsvReader(bytes, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Constructor_ByteArray(string lineBreak)
		{
			var encoding = System.Text.Encoding.UTF8;
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);
			var bytes = encoding.GetBytes(file);

			var data = new DsvReader(bytes, encoding, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Constructor_ReadOnlySpan(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);
			var span = file.AsSpan();

			var data = new DsvReader(span, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Constructor_CharArray(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);
			var array = file.ToCharArray();

			var data = new DsvReader(array, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Constructor_String(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());
		}



		[Fact]
		public void Constructor_Span()
		{
			string val = "Data1,Data2,Data3";
			Span<char> span = new Span<char>(val.ToCharArray());

			var data = new DsvReader(span, System.Text.Encoding.UTF8, new DsvOptions(',', '"', false));

			List<ReadOnlyMemory<char>> values = new List<ReadOnlyMemory<char>>();


			while (data.MoveNext())
			{
				values.Add(data.ReadNextAsMemory());
			}

			Assert.Equal(val, string.Join(",", values));
		}


		[Fact]
		public void ReadNextSequential()
		{
			Span<string> span = new Span<string>(new string[10]);
			span.Fill("Data");

			var file = string.Join(",", span.ToArray());

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> values = new List<ReadOnlyMemory<char>>();


			while (data.MoveNext())
			{
				values.Add(data.ReadNextAsMemory());
			}

			Assert.Equal(span.ToArray(), values.Select(x => x.ToString()).ToArray());
		}



		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void SimpleData_MultipleDataRows(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 10, 3);

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Equal(10, rows.Count);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			for (int i = 0; i < 10; i++)
			{
				Assert.Equal("Data1", rows[i][0].ToString());
				Assert.Equal("Data2", rows[i][1].ToString());
				Assert.Equal("Data3", rows[i][2].ToString());
			}
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void ColumnResize(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 3, 5);

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(5, columns.Count);
			Assert.Equal(3, rows.Count);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());
			Assert.Equal("Column4", columns[3].ToString());
			Assert.Equal("Column5", columns[4].ToString());

			for (int i = 0; i < 3; i++)
			{
				Assert.Equal("Data1", rows[i][0].ToString());
				Assert.Equal("Data2", rows[i][1].ToString());
				Assert.Equal("Data3", rows[i][2].ToString());
			}
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void PublicPropertyState(string lineBreak)
		{
			string file = FileGenerator("Column", "Data", lineBreak, 1, 3);

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			Assert.True(data.MoveNext());

			var column1 = data.ReadNextAsMemory().ToString();
			Assert.Equal("Column1", column1);
			Assert.Empty(data.ReadNextAsMemory().ToString());
			Assert.Equal(1, data.Column);
			Assert.Equal(1, data.ColumnCount);
			Assert.Equal(0, data.RowCount);
			Assert.False(data.ColumnsFilled);
			Assert.False(data.NewRowNextRead);

			Assert.True(data.MoveNext());

			var column2 = data.ReadNextAsMemory().ToString();
			Assert.Equal("Column2", column2);
			Assert.Empty(data.ReadNextAsMemory().ToString());
			Assert.Equal(2, data.Column);
			Assert.Equal(2, data.ColumnCount);
			Assert.Equal(0, data.RowCount);
			Assert.False(data.ColumnsFilled);
			Assert.False(data.NewRowNextRead);

			Assert.True(data.MoveNext());

			var column3 = data.ReadNextAsMemory().ToString();
			Assert.Equal("Column3", column3);
			Assert.Empty(data.ReadNextAsMemory().ToString());
			Assert.Equal(3, data.Column);
			Assert.Equal(3, data.ColumnCount);
			Assert.Equal(1, data.RowCount);
			Assert.True(data.ColumnsFilled);
			Assert.True(data.NewRowNextRead);

			Assert.True(data.MoveNext());

			var data1 = data.ReadNextAsMemory().ToString();
			Assert.Equal("Data1", data1);
			Assert.Empty(data.ReadNextAsMemory().ToString());
			Assert.Equal(1, data.Column);
			Assert.Equal(3, data.ColumnCount);
			Assert.Equal(1, data.RowCount);
			Assert.True(data.ColumnsFilled);
			Assert.False(data.NewRowNextRead);

			Assert.True(data.MoveNext());

			var data2 = data.ReadNextAsMemory().ToString();
			Assert.Equal("Data2", data2);
			Assert.Empty(data.ReadNextAsMemory().ToString());
			Assert.Equal(2, data.Column);
			Assert.Equal(3, data.ColumnCount);
			Assert.Equal(1, data.RowCount);
			Assert.True(data.ColumnsFilled);
			Assert.False(data.NewRowNextRead);

			Assert.True(data.MoveNext());

			var data3 = data.ReadNextAsMemory().ToString();
			Assert.Equal("Data3", data3);
			Assert.Empty(data.ReadNextAsMemory().ToString());
			Assert.Equal(3, data.Column);
			Assert.Equal(3, data.ColumnCount);
			Assert.Equal(2, data.RowCount);
			Assert.True(data.ColumnsFilled);
			Assert.False(data.NewRowNextRead);

			Assert.False(data.MoveNext());
		}



		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void EscapedDelimiter(string lineBreak)
		{
			string file = $"Column1,Column2,Column3{lineBreak}\"Da,ta1\",\"Da,ta2\",\"Da,ta3\"";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Da,ta1", rows[0][0].ToString());
			Assert.Equal("Da,ta2", rows[0][1].ToString());
			Assert.Equal("Da,ta3", rows[0][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void EscapedLineFeed(string lineBreak)
		{
			string file = $"Column1,Column2,Column3{lineBreak}\"Da{lineBreak}ta1\",\"Da{lineBreak}ta2\",\"Da{lineBreak}ta3\"";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal($"Da{lineBreak}ta1", rows[0][0].ToString());
			Assert.Equal($"Da{lineBreak}ta2", rows[0][1].ToString());
			Assert.Equal($"Da{lineBreak}ta3", rows[0][2].ToString());
		}



		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void EscapedEscape_Data(string lineBreak)
		{
			string file = $"Column1,Column2,Column3{lineBreak}\"Da\"\"ta1\",\"Da\"\"ta2\",\"Da\"\"ta3\"";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Da\"ta1", rows[0][0].ToString());
			Assert.Equal("Da\"ta2", rows[0][1].ToString());
			Assert.Equal("Da\"ta3", rows[0][2].ToString());
		}



		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void EscapedEscape_Columns(string lineBreak)
		{
			string file = $"\"Column\"\"1\",\"Column\"\"2\",\"Column\"\"3\"{lineBreak}Data1,Data2,Data3";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column\"1", columns[0].ToString());
			Assert.Equal("Column\"2", columns[1].ToString());
			Assert.Equal("Column\"3", columns[2].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Escaped_Columns(string lineBreak)
		{
			string file = $"\"Column1\",\"Column2\",\"Column3\"{lineBreak}Data1,Data2,Data3";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Escaped_EmptyData(string lineBreak)
		{
			string file = $"\"Column1\",\"Column2\",\"Column3\"{lineBreak}\"\",Data2,Data3{lineBreak},\"\",";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Equal(2, rows.Count);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());

			Assert.Equal("", rows[1][0].ToString());
			Assert.Equal("", rows[1][1].ToString());
			Assert.Equal("", rows[1][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void Escaped_LastValue(string lineBreak)
		{
			string file = $"\"Column1\",\"Column2\",\"Column3\"{lineBreak}\"\",Data2,Data3{lineBreak},\"\",\"HELLO\"{lineBreak}";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Equal(2, rows.Count);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());

			Assert.Equal("", rows[1][0].ToString());
			Assert.Equal("", rows[1][1].ToString());
			Assert.Equal("HELLO", rows[1][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void HeaderWithWhitespace(string lineBreak)
		{
			string file = $"Column1,Column2, Column3{lineBreak}Data1,Data2,Data3{lineBreak}";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());
		}

		private void EscapeBomWithEncoding(Encoding encoding, string lineBreak, string bomPrefix = null)
		{
			var BOM = bomPrefix ?? encoding.GetString(encoding.GetPreamble());
			string file = $"{BOM}\"Column1\",\"Column2\",\"Column3\"{lineBreak}\"\",Data2,Data3{lineBreak},\"\",";

			var data = new DsvReader(file, encoding, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Equal(2, rows.Count);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());

			Assert.Equal("", rows[1][0].ToString());
			Assert.Equal("", rows[1][1].ToString());
			Assert.Equal("", rows[1][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void EscapeBOM(string lineBreak)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			EscapeBomWithEncoding(new UTF8Encoding(true), lineBreak);
			EscapeBomWithEncoding(new UTF8Encoding(false), lineBreak);
			EscapeBomWithEncoding(new UTF32Encoding(true, true), lineBreak);
			EscapeBomWithEncoding(new UTF32Encoding(true, false), lineBreak);
			EscapeBomWithEncoding(new UTF32Encoding(false, false), lineBreak);
			EscapeBomWithEncoding(new UTF32Encoding(false, true), lineBreak);
			EscapeBomWithEncoding(Encoding.GetEncoding(1252), lineBreak);
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void MalformedHeader_SingleQuoteInMiddle(string lineBreak)
		{
			string file = $"\"Col\"umn1\",Column2,Column3{lineBreak},,";

			Assert.Throws<FormatException>(() =>
			{
				var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

				List<ReadOnlyMemory<char>> columns = null;
				List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


				while (data.MoveNext())
				{
					if (!data.ColumnsFilled)
					{
						columns = data.ReadLine().ToList();
					}
					else
					{
						rows.Add(data.ReadLine().ToList());
					}
				}
			});
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void MalformedData_ExtraColumns(string lineBreak)
		{
			string file = $"Column1,Column2{lineBreak},,,,,";

			Assert.Throws<FormatException>(() =>
			{
				var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

				List<ReadOnlyMemory<char>> columns = null;
				List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


				while (data.MoveNext())
				{
					if (!data.ColumnsFilled)
					{
						columns = data.ReadLine().ToList();
					}
					else
					{
						rows.Add(data.ReadLine().ToList());
					}
				}
			});
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void MalformedData_Escapes(string lineBreak)
		{
			string file = $"\"\"Column1\",Column2{lineBreak}Data1,Data2";

			Assert.Throws<FormatException>(() =>
			{
				var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

				List<ReadOnlyMemory<char>> columns = null;
				List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


				while (data.MoveNext())
				{
					if (!data.ColumnsFilled)
					{
						columns = data.ReadLine().ToList();
					}
					else
					{
						rows.Add(data.ReadLine().ToList());
					}
				}
			});
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void DelimiterBetweenLines(string lineBreak)
		{
			string file = $"Column1,Column2{lineBreak},{lineBreak}Data1,Data2";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(2, columns.Count);
			Assert.Equal(2, rows.Count);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());

			Assert.Equal("", rows[0][0].ToString());
			Assert.Equal("", rows[0][1].ToString());

			Assert.Equal("Data1", rows[1][0].ToString());
			Assert.Equal("Data2", rows[1][1].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void DataHasLessColumnsThanHeader(string lineBreak)
		{
			string file = $"Column1,Column2,Column3{lineBreak}Data1,Data2";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("", rows[0][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void MultipleLinesBetweenData(string lineBreak)
		{
			string file = $"Column1,Column2{lineBreak}{lineBreak}Data1,Data2{lineBreak}{lineBreak}Data1,Data2";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(2, columns.Count);
			Assert.Equal(2, rows.Count);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());

			Assert.Equal("Data1", rows[1][0].ToString());
			Assert.Equal("Data2", rows[1][1].ToString());
		}

		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void BlankRowStartColumns(string lineBreak)
		{
			string file = $",Column2{lineBreak}{lineBreak},Data2{lineBreak}{lineBreak},Data2";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(2, columns.Count);
			Assert.Equal(2, rows.Count);

			Assert.Equal("", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());

			Assert.Equal("", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());

			Assert.Equal("", rows[1][0].ToString());
			Assert.Equal("Data2", rows[1][1].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void EscapedEscape_Multiple(string lineBreak)
		{
			string file = $"Column1,Column2,Column3{lineBreak}\"Da\"\"\"\"ta1\",\"Data2\"\"\"\"\",\"\"\"\"\"Da\"\"ta3\"";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Da\"\"ta1", rows[0][0].ToString());
			Assert.Equal("Data2\"\"", rows[0][1].ToString());
			Assert.Equal("\"\"Da\"ta3", rows[0][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void EmptyDataFields(string lineBreak)
		{
			string file = $"Column1,Column2,Column3{lineBreak}Data1,,Data3";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void EndsWithNewLine(string lineBreak)
		{
			string file = $"Column1,Column2,Column3{lineBreak}Data1,Data2,Data3{lineBreak}";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Data1", rows[0][0].ToString());
			Assert.Equal("Data2", rows[0][1].ToString());
			Assert.Equal("Data3", rows[0][2].ToString());
		}

		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void SpecialCharacters(string lineBreak)
		{
			string file = $"Column1,Column2,Column3{lineBreak}Düsseldorf,Datüa2,ü123{lineBreak}";

			var bytes = Encoding.UTF8.GetBytes(file);

			var data = new DsvReader(bytes, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);

			List<ReadOnlyMemory<char>> columns = null;
			List<List<ReadOnlyMemory<char>>> rows = new List<List<ReadOnlyMemory<char>>>();


			while (data.MoveNext())
			{
				if (!data.ColumnsFilled)
				{
					columns = data.ReadLine().ToList();
				}
				else
				{
					rows.Add(data.ReadLine().ToList());
				}
			}

			Assert.Equal(3, columns.Count);
			Assert.Single(rows);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			Assert.Equal("Düsseldorf", rows[0][0].ToString());
			Assert.Equal("Datüa2", rows[0][1].ToString());
			Assert.Equal("ü123", rows[0][2].ToString());
		}




		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public void ReadLine(string lineBreak)
		{
			string file = $"Column1,Column2,Column3{lineBreak}Data1,Data2,Data3";

			var data = new DsvReader(file, System.Text.Encoding.UTF8, DsvOptions.DefaultCsvOptions);
			int rows = 0;

			Assert.Equal(rows, data.RowCount);

			while (data.MoveNextLine())
			{
				rows++;
				Assert.Equal(rows, data.RowCount);
			}


			Assert.Equal(rows, data.RowCount);
		}
	}
}
