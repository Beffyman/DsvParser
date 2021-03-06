using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Beffyman.DsvParser.Tests
{
	public class DsvReaderTests : BaseTest
	{

		[Fact]
		public void Constructor_MemoryBytes()
		{
			string file = FileGenerator("Column", "Data", 1, 3);
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

		[Fact]
		public void Constructor_ByteArray()
		{
			var encoding = System.Text.Encoding.UTF8;
			string file = FileGenerator("Column", "Data", 1, 3);
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

		[Fact]
		public void Constructor_ReadOnlySpan()
		{
			string file = FileGenerator("Column", "Data", 1, 3);
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

		[Fact]
		public void Constructor_CharArray()
		{
			string file = FileGenerator("Column", "Data", 1, 3);
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

		[Fact]
		public void Constructor_String()
		{
			string file = FileGenerator("Column", "Data", 1, 3);

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


		[Fact]
		public void SimpleData_MultipleDataRows()
		{
			string file = FileGenerator("Column", "Data", 10, 3);

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

		[Fact]
		public void ColumnResize()
		{
			string file = FileGenerator("Column", "Data", 3, 5);

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

		[Fact]
		public void PublicPropertyState()
		{
			string file = FileGenerator("Column", "Data", 1, 3);

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


		[Fact]
		public void EscapedDelimiter()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}\"Da,ta1\",\"Da,ta2\",\"Da,ta3\"";

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

		[Fact]
		public void EscapedLineFeed()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}\"Da{Environment.NewLine}ta1\",\"Da{Environment.NewLine}ta2\",\"Da{Environment.NewLine}ta3\"";

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

			Assert.Equal($"Da{Environment.NewLine}ta1", rows[0][0].ToString());
			Assert.Equal($"Da{Environment.NewLine}ta2", rows[0][1].ToString());
			Assert.Equal($"Da{Environment.NewLine}ta3", rows[0][2].ToString());
		}


		[Fact]
		public void EscapedEscape_Data()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}\"Da\"\"ta1\",\"Da\"\"ta2\",\"Da\"\"ta3\"";

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


		[Fact]
		public void EscapedEscape_Columns()
		{
			string file = $"\"Column\"\"1\",\"Column\"\"2\",\"Column\"\"3\"{Environment.NewLine}Data1,Data2,Data3";

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

		[Fact]
		public void Escaped_Columns()
		{
			string file = $"\"Column1\",\"Column2\",\"Column3\"{Environment.NewLine}Data1,Data2,Data3";

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
		public void Escaped_EmptyData()
		{
			string file = $"\"Column1\",\"Column2\",\"Column3\"{Environment.NewLine}\"\",Data2,Data3{Environment.NewLine},\"\",";

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

		[Fact]
		public void Escaped_LastValue()
		{
			string file = $"\"Column1\",\"Column2\",\"Column3\"{Environment.NewLine}\"\",Data2,Data3{Environment.NewLine},\"\",\"HELLO\"{Environment.NewLine}";

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

		[Fact]
		public void HeaderWithWhitespace()
		{
			string file = $"Column1,Column2, Column3{Environment.NewLine}Data1,Data2,Data3{Environment.NewLine}";

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

		private void EscapeBomWithEncoding(Encoding encoding, string bomPrefix = null)
		{
			var BOM = bomPrefix ?? encoding.GetString(encoding.GetPreamble());
			string file = $"{BOM}\"Column1\",\"Column2\",\"Column3\"{Environment.NewLine}\"\",Data2,Data3{Environment.NewLine},\"\",";

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

		[Fact]
		public void EscapeBOM()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			EscapeBomWithEncoding(new UTF8Encoding(true));
			EscapeBomWithEncoding(new UTF8Encoding(false));
			EscapeBomWithEncoding(new UTF32Encoding(true, true));
			EscapeBomWithEncoding(new UTF32Encoding(true, false));
			EscapeBomWithEncoding(new UTF32Encoding(false, false));
			EscapeBomWithEncoding(new UTF32Encoding(false, true));
			EscapeBomWithEncoding(Encoding.GetEncoding(1252));
		}

		[Fact]
		public void MalformedHeader_SingleQuoteInMiddle()
		{
			string file = $"\"Col\"umn1\",Column2,Column3{Environment.NewLine},,";

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

		[Fact]
		public void MalformedData_ExtraColumns()
		{
			string file = $"Column1,Column2{Environment.NewLine},,,,,";

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

		[Fact]
		public void MalformedData_Escapes()
		{
			string file = $"\"\"Column1\",Column2{Environment.NewLine}Data1,Data2";

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

		[Fact]
		public void DelimiterBetweenLines()
		{
			string file = $"Column1,Column2{Environment.NewLine},{Environment.NewLine}Data1,Data2";

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

		[Fact]
		public void DataHasLessColumnsThanHeader()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}Data1,Data2";

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

		[Fact]
		public void MultipleLinesBetweenData()
		{
			string file = $"Column1,Column2{Environment.NewLine}{Environment.NewLine}Data1,Data2{Environment.NewLine}{Environment.NewLine}Data1,Data2";

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

		[Fact]
		public void EscapedEscape_Multiple()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}\"Da\"\"\"\"ta1\",\"Data2\"\"\"\"\",\"\"\"\"\"Da\"\"ta3\"";

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

		[Fact]
		public void EmptyDataFields()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}Data1,,Data3";

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

		[Fact]
		public void EndsWithNewLine()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}Data1,Data2,Data3{Environment.NewLine}";

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
		public void SpecialCharacters()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}D�sseldorf,Dat�a2,�123{Environment.NewLine}";

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

			Assert.Equal("D�sseldorf", rows[0][0].ToString());
			Assert.Equal("Dat�a2", rows[0][1].ToString());
			Assert.Equal("�123", rows[0][2].ToString());
		}



		[Fact]
		public void ReadLine()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}Data1,Data2,Data3";

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
