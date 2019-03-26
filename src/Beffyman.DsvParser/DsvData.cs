using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Beffyman.DsvParser
{
	/// <summary>
	/// Parses data that has separated values into a consumable format
	/// </summary>
	public readonly ref struct DsvData
	{
		/// <summary>
		/// Headers that are contained within the parsed data
		/// </summary>
		public readonly ReadOnlySpan<string> Headers;
		/// <summary>
		/// Rows that are contained within the parsed data
		/// </summary>
		public readonly ReadOnlySpan<ReadOnlyMemory<string>> Rows;

		/// <summary>
		/// When you have a byte array and a known encoding.
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		public DsvData(byte[] dsv, Encoding encoding, in DsvOptions options) : this(encoding.GetString(dsv), options) { }

		/// <summary>
		/// Converts the char array into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		public DsvData(char[] dsv, in DsvOptions options) : this(dsv.AsSpan(), options) { }

		/// <summary>
		/// Converts the string into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		public DsvData(string dsv, in DsvOptions options) : this(dsv.AsSpan(), options) { }

		/// <summary>
		/// Converts the <see cref="ReadOnlyMemory{T}"/> into a <see cref="ReadOnlySpan{T}"/> and uses that to parse the data
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		public DsvData(in ReadOnlyMemory<char> dsv, in DsvOptions options) : this(dsv.Span, options) { }

		/// <summary>
		/// Directly uses the Span provided to parse the data, most performant option
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		public DsvData(in ReadOnlySpan<char> dsv, in DsvOptions options)
		{
			Headers = default;//Implicit cast from Span to ReadOnlySpan
			Rows = default;//Implicit cast from Span to ReadOnlySpan

			//Known limits
			//Uses int for enumerating, so 2,147,483,647 bytes is the size limit

			//int lastUnescapedQuote = -1;
			//bool lastWasQuote = false;

			//Need to account for the following
			//Quotes
			//Escaped Quotes
			//Escaped Delimiters
			//Escaped LineFeed

			var lineBreak = options.LineBreak.AsSpan();
			int columns = options.KnownColumns;

			//Could probably get better performance with a switch, but would need constant delimiter
			bool firstPass = columns == 0;
			/*
			Steps
			1) If KnownColumns is provided, then allocate then skip the first pass since we know how big to make the columns span else follow 1a
				1a) Run through first line until the LineFeed, then reset i to 0 so we know how many columns there are.
				1b) Allocate a span so hold the columns, then start parsing
			2) Allocate a
			 */
			bool headersFilled = false;
			Span<string> headers = default;


			bool checkRowBounds = true;
			int rowCount = 0;
			int columnCount = 0;
			Span<ReadOnlyMemory<string>> rows = default;
			Memory<string> row = default;


			bool escaping = false;


			if (options.KnownRows != 0)
			{
				checkRowBounds = false;
				//int expectedRows = Math.Max((dsv.Length / columns) >> 4, 1);
				rows = new Span<ReadOnlyMemory<string>>(new ReadOnlyMemory<string>[options.KnownRows]);
			}

			if (!firstPass)
			{
				headers = new Span<string>(new string[columns]);
				row = new Memory<string>(new string[columns]);
			}

			int lastDelimiter = 0;
			for (int i = 0; i < dsv.Length; i++)
			{
				//On first pass we operate differently as we just want to find out how many columns there are
				if (firstPass)
				{
					//If we find a delimiter we know that is a column
					//TODO: Escaped Delimiters
					if (dsv[i] == options.Delimiter)
					{
						if (!escaping)
						{
							columns++;
						}
					}
					else if (dsv[i] == options.EscapeChar)
					{
						if (!escaping)
						{
							escaping = true;
						}
						else
						{

						}
					}
					else
					{
						if (!escaping)
						{
							if (CheckLineFeed(dsv, lineBreak, i))
							{
								//When we match the line feed we know we have all the columns we need, end the first pass and reset the index
								firstPass = false;
								columns++;
								headers = new Span<string>(new string[columns]);
								row = new Memory<string>(new string[columns]);
								i = 0;
							}
						}
					}
				}
				else
				{
					//Check if we have a delimiter
					if (dsv[i] == options.Delimiter)
					{
						//If we do, then slice out the chars from until the last one we found.
						row.Span[columnCount] = dsv.Slice(lastDelimiter, i - lastDelimiter).ToString();
						columnCount++;
						lastDelimiter = i + 1;
					}
					else if (i == dsv.Length - 1)
					{
						//Finalize
						row.Span[columnCount] = dsv.Slice(lastDelimiter, i - lastDelimiter - lineBreak.Length + 1).ToString();
						rows[rowCount] = row;
						rowCount++;
					}
					else
					{
						if (CheckLineFeed(dsv, lineBreak, i))
						{
							row.Span[columnCount] = dsv.Slice(lastDelimiter, i - lastDelimiter - lineBreak.Length).ToString();
							//columnCount++;

							//Need to check if we need to expand the Rows Span
							if (checkRowBounds)
							{
								ExpandRowAllocation(ref rows, rowCount);
							}

							//Check if we need to place the headers in
							if (options.HasHeaders && !headersFilled)
							{
								headers = row.Span;
								headersFilled = true;
							}
							else
							{
								//Place row inside span
								rows[rowCount] = row;
								rowCount++;
							}

							//Start up a new Row
							row = new Memory<string>(new string[columns]);
							//We treat the LineFeed as a delimiter as the last delimiter was before it
							lastDelimiter = i;
							columnCount = 0;
						}
					}
				}
			}

			//Do the final resize needed
			if (rows.Length != rowCount)
			{
				rows = rows.Slice(0, rowCount);
			}



			//Need to resize Rows to match actual size
			Headers = headers;//Implicit cast from Span to ReadOnlySpan
			Rows = rows;//Implicit cast from Span to ReadOnlySpan
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ExpandRowAllocation(ref Span<ReadOnlyMemory<string>> rows, in int rowCount)
		{
			//We need to expand
			if (rows.Length - 1 <= rowCount)
			{
				//Keep ref to existing rows
				var prevRows = rows;
				//Make a new one with double the length
				int size = Math.Max(rows.Length * 2, 4);
				rows = new Span<ReadOnlyMemory<string>>(new ReadOnlyMemory<string>[size]);
				//Copy existing to new one
				prevRows.CopyTo(rows);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool CheckLineFeed(in ReadOnlySpan<char> dsv, in ReadOnlySpan<char> lineBreak, in int i)
		{
			//TODO: Escaped LineFeed
			if (i >= lineBreak.Length)
			{
				//TODO: Escaped LineFeed
				var lineBreakSlice = dsv.Slice(i - lineBreak.Length, lineBreak.Length);
				return MemoryExtensions.SequenceEqual(lineBreakSlice, lineBreak);
			}

			return false;
		}

	}
}
