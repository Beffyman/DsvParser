﻿using System;
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvData(byte[] dsv, Encoding encoding, in DsvOptions options) : this(encoding.GetString(dsv), options) { }

		/// <summary>
		/// Converts the char array into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvData(char[] dsv, in DsvOptions options) : this(dsv.AsSpan(), options) { }

		/// <summary>
		/// Converts the string into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvData(string dsv, in DsvOptions options) : this(dsv.AsSpan(), options) { }

		/// <summary>
		/// Converts the <see cref="ReadOnlyMemory{T}"/> into a <see cref="ReadOnlySpan{T}"/> and uses that to parse the data
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvData(in ReadOnlyMemory<char> dsv, in DsvOptions options) : this(dsv.Span, options) { }

		/// <summary>
		/// Directly uses the Span provided to parse the data, most performant option
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

			bool headersFilled = false;
			Span<string> headers = default;


			bool checkRowBounds = true;
			int rowCount = 0;
			int columnCount = 0;
			Span<ReadOnlyMemory<string>> rows = default;
			Memory<string> row = default;


			bool escaping = false;
			bool didEscape = false;
			bool escapedEscapeChar = false;
			bool didEscapeEscapeChar = false;


			if (options.KnownRows != 0)
			{
				checkRowBounds = false;
				rows = new Span<ReadOnlyMemory<string>>(new ReadOnlyMemory<string>[options.KnownRows]);
			}

			if (!firstPass)
			{
				headers = new Span<string>(new string[columns]);
				row = new Memory<string>(new string[columns]);
			}

			var doubleEscapeChar = new string(new char[] { options.EscapeChar, options.EscapeChar });
			var escapeCharAsString = new string(new char[] { options.EscapeChar });

			int lastDelimiter = 0;
			for (int i = 0; i < dsv.Length; i++)
			{
				//On first pass we operate differently as we just want to find out how many columns there are
				if (firstPass)
				{
					FirstPass(dsv, options, lineBreak, ref columns, ref firstPass, ref headers, ref row, ref escaping, ref didEscape, ref escapedEscapeChar, ref didEscapeEscapeChar, ref lastDelimiter, ref i);
				}
				else
				{
					SecondPass(dsv, options, lineBreak, columns, ref headersFilled, ref headers, checkRowBounds, ref rowCount, ref columnCount, ref rows, ref row, ref escaping, ref didEscape, ref escapedEscapeChar, ref didEscapeEscapeChar, doubleEscapeChar, escapeCharAsString, ref lastDelimiter, i);
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
		private void SecondPass(in ReadOnlySpan<char> dsv, in DsvOptions options, in ReadOnlySpan<char> lineBreak, int columns, ref bool headersFilled, ref Span<string> headers, bool checkRowBounds, ref int rowCount, ref int columnCount, ref Span<ReadOnlyMemory<string>> rows, ref Memory<string> row, ref bool escaping, ref bool didEscape, ref bool escapedEscapeChar, ref bool didEscapeEscapeChar, string doubleEscapeChar, string escapeCharAsString, ref int lastDelimiter, int i)
		{
			//Check if we have a delimiter
			if (dsv[i] == options.Delimiter)
			{
				if (!escaping || (escaping && escapedEscapeChar))
				{
					//If we do, then slice out the chars from until the last one we found.
					if (didEscape)
					{
						//We know there was an escape char and a delimiter at the end, so take one off both sides for the escape, and 1 more from the end for the delimiter
						row.Span[columnCount] = dsv.Slice(lastDelimiter + 1, i - lastDelimiter - 2).ToString();
					}
					else
					{
						row.Span[columnCount] = dsv.Slice(lastDelimiter, i - lastDelimiter).ToString();
					}

					if (didEscapeEscapeChar)
					{
						//Need to string.replace which will cause an allocation as we can't cut items out of the middle of a span
						row.Span[columnCount] = row.Span[columnCount].Replace(doubleEscapeChar, escapeCharAsString);
					}

					lastDelimiter = i + 1;
					columnCount++;
					didEscape = false;
				}
			}
			else if (i == dsv.Length - 1)
			{
				//Finalize
				if (escaping)
				{
					if (CheckLineFeed(dsv, lineBreak, i + 1))
					{
						//This entry was escaped, that means we need to remove 1 from each side, but also take off the line feed from the end of the file
						row.Span[columnCount] = dsv.Slice(lastDelimiter + 1, (dsv.Length - lastDelimiter - lineBreak.Length) - 1).ToString();
					}
					else
					{
						//Take 1 off both sides and 1 more as we are still escaping
						row.Span[columnCount] = dsv.Slice(lastDelimiter + 1, (dsv.Length - lastDelimiter) - 2).ToString();
					}

				}
				else if (CheckLineFeed(dsv, lineBreak, i + 1))
				{
					row.Span[columnCount] = dsv.Slice(lastDelimiter, dsv.Length - lastDelimiter - lineBreak.Length).ToString();
				}
				else
				{
					row.Span[columnCount] = dsv.Slice(lastDelimiter, dsv.Length - lastDelimiter).ToString();
				}

				if (didEscapeEscapeChar)
				{
					//Need to string.replace which will cause an allocation as we can't cut items out of the middle of a span
					row.Span[columnCount] = row.Span[columnCount].Replace(doubleEscapeChar, escapeCharAsString);
				}

				didEscape = false;
				didEscapeEscapeChar = false;
				rows[rowCount] = row;
				rowCount++;
			}
			else if (dsv[i] == options.EscapeChar)
			{
				//Need to figure out if we are already escaping, if not, then escape
				if (!escaping)
				{
					didEscape = true;
					escaping = true;
				}
				//So this only gets hit if we have ",  and nothing like "", which would be in the middle of a cell, we need to make sure the quote is alone before the delimiter
				else if ((i + 1) < dsv.Length && (dsv[i + 1] == options.Delimiter || CheckLineFeed(dsv, lineBreak, i + lineBreak.Length + 1))
					&& !escapedEscapeChar
					&& dsv[i - 1] != options.EscapeChar)
				{
					escaping = false;
				}
				else if (!escapedEscapeChar)
				{
					escapedEscapeChar = true;
					didEscapeEscapeChar = true;
				}
				else
				{
					escapedEscapeChar = false;
				}
			}
			else
			{
				if (escapedEscapeChar)
				{
					ThrowInvalidEscapedEscapeChar();
				}

				if (!escaping)
				{
					if ((i + 1) < dsv.Length && CheckLineFeed(dsv, lineBreak, i + 1))
					{
						if (escaping)
						{
							//This entry was escaped, that means we need to remove 1 from each side
							row.Span[columnCount] = dsv.Slice(lastDelimiter + 1, i - lastDelimiter - lineBreak.Length - 1).ToString();
						}
						else if (didEscape)
						{
							row.Span[columnCount] = dsv.Slice(lastDelimiter + 1, i - lastDelimiter - lineBreak.Length - 1).ToString();
						}
						else
						{
							row.Span[columnCount] = dsv.Slice(lastDelimiter, i - lastDelimiter - lineBreak.Length + 1).ToString();
						}


						if (didEscapeEscapeChar)
						{
							//Need to string.replace which will cause an allocation as we can't cut items out of the middle of a span
							row.Span[columnCount] = row.Span[columnCount].Replace(doubleEscapeChar, escapeCharAsString);
						}

						//columnCount++;

						//Need to check if we need to expand the Rows Span
						if (checkRowBounds)
						{
#if NETSTANDARD2_1
							ExpandRowAllocation(ref rows, rowCount);
#else
							ExpandRowAllocation(ref rows, ref rowCount);
#endif
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

						didEscape = false;
						didEscapeEscapeChar = false;
						//Start up a new Row
						row = new Memory<string>(new string[columns]);
						//We treat the LineFeed as a delimiter as the last delimiter was before it
						lastDelimiter = i + 1;
						columnCount = 0;
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FirstPass(in ReadOnlySpan<char> dsv, in DsvOptions options, in ReadOnlySpan<char> lineBreak, ref int columns, ref bool firstPass, ref Span<string> headers, ref Memory<string> row, ref bool escaping, ref bool didEscape, ref bool escapedEscapeChar, ref bool didEscapeEscapeChar, ref int lastDelimiter, ref int i)
		{
			//If we find a delimiter we know that is a column
			if (dsv[i] == options.Delimiter)
			{
				if (!escaping)
				{
					lastDelimiter = i;
					columns++;
				}
			}
			else if (dsv[i] == options.EscapeChar)
			{
				//Need to figure out if we are already escaping, if not, then escape
				if (!escaping)
				{
					escaping = true;
				}
				//So this only gets hit if we have ",  and nothing like "", which would be in the middle of a cell, we need to make sure the quote is alone before the delimiter
				else if ((i + 1) < dsv.Length
						&& (dsv[i + 1] == options.Delimiter || CheckLineFeed(dsv, lineBreak, i + lineBreak.Length + 1))
					&& !escapedEscapeChar
					&& dsv[i - 1] != options.EscapeChar)
				{
					escaping = false;
				}
				else if (!escapedEscapeChar)
				{
					escapedEscapeChar = true;
					didEscapeEscapeChar = true;
				}
				else
				{
					escapedEscapeChar = false;
				}
			}
			else
			{
				if (escapedEscapeChar)
				{
					ThrowInvalidEscapedEscapeChar();
				}

				if (!escaping
					&& (i + 1) < dsv.Length
						&& CheckLineFeed(dsv, lineBreak, i + 1))
				{
					//When we match the line feed we know we have all the columns we need, end the first pass and reset the index
					firstPass = false;
					columns++;
					headers = new Span<string>(new string[columns]);
					row = new Memory<string>(new string[columns]);
					i = -1;//For loop will add one after this so we need to start back at 0
					escaping = false;
					didEscapeEscapeChar = false;
					didEscape = false;
					lastDelimiter = 0;
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NETSTANDARD2_1
		private void ExpandRowAllocation(ref Span<ReadOnlyMemory<string>> rows, in int rowCount)

#else
		private void ExpandRowAllocation(ref Span<ReadOnlyMemory<string>> rows, ref int rowCount)
#endif

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
#if NETSTANDARD2_1
		private bool CheckLineFeed(in ReadOnlySpan<char> dsv, in ReadOnlySpan<char> lineBreak, in int i)

#else
		private bool CheckLineFeed(in ReadOnlySpan<char> dsv, in ReadOnlySpan<char> lineBreak, int i)
#endif
		{
			if (i >= lineBreak.Length)
			{
				var lineBreakSlice = dsv.Slice(i - lineBreak.Length, lineBreak.Length);
				return MemoryExtensions.SequenceEqual(lineBreakSlice, lineBreak);
			}

			return false;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ThrowInvalidEscapedEscapeChar()
		{
			throw new FormatException("An escape character was detected in the cell as part of it but no escape character was after it.");
		}
	}
}
