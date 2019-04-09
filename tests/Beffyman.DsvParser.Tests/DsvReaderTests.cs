using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Beffyman.DsvParser.Tests
{
	public class DsvReaderTests
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

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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
		public void SimpleData_MultipleDataRows()
		{
			string file = FileGenerator("Column", "Data", 5, 3);

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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
			Assert.Equal(5, rows.Count);

			Assert.Equal("Column1", columns[0].ToString());
			Assert.Equal("Column2", columns[1].ToString());
			Assert.Equal("Column3", columns[2].ToString());

			for (int i = 0; i < 5; i++)
			{
				Assert.Equal("Data1", rows[i][0].ToString());
				Assert.Equal("Data2", rows[i][1].ToString());
				Assert.Equal("Data3", rows[i][2].ToString());
			}
		}


		[Fact]
		public void EscapedDelimiter()
		{
			string file = $"Column1,Column2,Column3{Environment.NewLine}\"Da,ta1\",\"Da,ta2\",\"Da,ta3\"";

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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
		public void MalformedHeader_SingleQuoteInMiddle()
		{
			string file = $"\"Col\"umn1\",Column2,Column3{Environment.NewLine},,";

			Assert.Throws<FormatException>(() =>
			{
				var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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
				var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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
				var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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

			var data = new DsvReader(file, DsvOptions.DefaultCsvOptions);

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


	}
}
