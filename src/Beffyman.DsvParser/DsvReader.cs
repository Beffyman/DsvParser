using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Beffyman.DsvParser
{
	public sealed class DsvReader
	{
		public readonly DsvOptions Options;
		public readonly ReadOnlyMemory<char> Input;
		private readonly char[] dsv;

		public int RowCount => _rowCount;
		private int _rowCount;

		public int ColumnCount => _columnCount;
		private int _columnCount;

		public int Column => _column;
		private int _column;

		public bool ColumnsFilled => _columnsFilled;
		private bool _columnsFilled = false;

		public bool NewRowNextRead => _newRowNextRead;
		private bool _newRowNextRead;

		private readonly char _delimiter;
		private readonly char _escapeChar;
		private readonly bool _hasHeaders;
		private readonly int _length;

		/// <summary>
		/// When you have a byte array and a known encoding.
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="input"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(byte[] input, Encoding encoding, in DsvOptions options) : this(encoding.GetString(input), options) { }

		/// <summary>
		/// Converts the string into a span and then parses it
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(string input, in DsvOptions options) : this(input.ToCharArray(), options) { }


		/// <summary>
		/// Converts the char array into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(in ReadOnlySpan<char> input, in DsvOptions options) : this(input.ToArray(), options) { }

		/// <summary>
		/// Converts the char array into a span and then parses it
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(char[] input, in DsvOptions options) : this(new Memory<char>(input), options) { }


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(in ReadOnlyMemory<char> input, in DsvOptions options)
		{
			Input = input;
			dsv = input.ToArray();
			Options = options;

			doubleEscapeChar = new string(new char[] { options.EscapeChar, options.EscapeChar });
			escapeCharAsString = new string(new char[] { options.EscapeChar });

			_delimiter = Options.Delimiter;
			_hasHeaders = Options.HasHeaders;
			_escapeChar = Options.EscapeChar;
			_length = dsv.Length;
		}

		private bool escaping;
		private bool didEscape;
		private bool escapedEscapeChar;
		private bool didEscapeEscapeChar;
		private string doubleEscapeChar;
		private string escapeCharAsString;
		private int lastDelimiter;
		private int index;
		private ReadOnlyMemory<char> nextValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			return MoveToNext();
		}

		public ReadOnlyMemory<char> ReadNext()
		{
			var r = nextValue;
			nextValue = null;
			return r;
		}

		public ReadOnlyMemory<char>[] ReadLine()
		{
			if (_columnsFilled)
			{
				ReadOnlyMemory<char>[] arr = new ReadOnlyMemory<char>[_columnCount];

				int col = 0;
				int startRow = _rowCount;
				do
				{
					arr[col++] = nextValue;
				}
				while (_rowCount == startRow && MoveNext());

				return arr;
			}
			else
			{
				//First pass, as we don't know how many columns there are, we need to have a resizable array as the collection
				int length = 4;
				ReadOnlyMemory<char>[] arg = new ReadOnlyMemory<char>[length];

				int col = 0;
				int startRow = _rowCount;
				do
				{
					//If we need to resize, then double the size
					if (col >= length)
					{
						length *= 2;
						Array.Resize(ref arg, length);
					}

					arg[col++] = nextValue;
				}
				while (_rowCount == startRow && MoveNext());

				//Resize back to correct length after we are done
				if (length != col)
				{
					Array.Resize(ref arg, col);
				}

				return arg;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool MoveToNext()
		{
			//Known limits
			//Uses int for enumerating, so 2,147,483,647 bytes is the size limit

			//Need to account for the following
			//Quotes
			//Escaped Quotes
			//Escaped Delimiters
			//Escaped LineFeed

			//Could probably get better performance with a switch, but would need constant delimiter

			nextValue = null;
			//? GET RID OF PROPERTIES, THEY COST A LOT!!!
			//var dsv = Input.Span;

			if (_newRowNextRead)
			{
				_column = 1;
				_newRowNextRead = false;
			}

			if (index != 0)
			{
				index++;
			}

			for (; index < _length; index++)
			{
				char indexValue = dsv[index];
				//Check if we have a delimiter
				if (indexValue == _delimiter)
				{
					if (!escaping || (escaping && escapedEscapeChar))
					{
						if (_columnsFilled && _columnCount == _column)
						{
							ThrowInvalidRowColumn();
						}

						//If we do, then slice out the chars from until the last one we found.
						if (didEscape)
						{
							//We know there was an escape char and a delimiter at the end, so take one off both sides for the escape, and 1 more from the end for the delimiter
							nextValue = dsv.AsMemory().Slice(lastDelimiter + 1, index - lastDelimiter - 2);
						}
						else
						{
							nextValue = dsv.AsMemory().Slice(lastDelimiter, index - lastDelimiter);
						}

						if (didEscapeEscapeChar)
						{
							//Need to string.replace which will cause an allocation as we can't cut items out of the middle of a span
							nextValue = nextValue.ToString().Replace(doubleEscapeChar, escapeCharAsString).AsMemory();
						}

						lastDelimiter = index + 1;
						didEscape = false;

						if (!_columnsFilled)
						{
							_columnCount++;
						}
						_column++;

						return true;
					}
				}
				else if (index == _length - 1)
				{
					//Finalize
					if (escaping)
					{
						if (CheckLineFeed(dsv, index + 1))
						{
							//This entry was escaped, that means we need to remove 1 from each side, but also take off the line feed from the end of the file
							nextValue = dsv.AsMemory().Slice(lastDelimiter + 1, (_length - lastDelimiter - 2) - 1);
						}
						else
						{
							//Take 1 off both sides and 1 more as we are still escaping
							nextValue = dsv.AsMemory().Slice(lastDelimiter + 1, (_length - lastDelimiter) - 2);
						}

					}
					else if (CheckLineFeed(dsv, index + 1))
					{
						nextValue = dsv.AsMemory().Slice(lastDelimiter, _length - lastDelimiter - 2);
					}
					else
					{
						nextValue = dsv.AsMemory().Slice(lastDelimiter, _length - lastDelimiter);
					}

					if (didEscapeEscapeChar)
					{
						//Need to string.replace which will cause an allocation as we can't cut items out of the middle of a span
						nextValue = nextValue.ToString().Replace(doubleEscapeChar, escapeCharAsString).AsMemory();
					}

					didEscape = false;
					didEscapeEscapeChar = false;
					_rowCount++;
					_newRowNextRead = false;

					return true;
				}
				else if (indexValue == _escapeChar)
				{
					//Need to figure out if we are already escaping, if not, then escape
					if (!escaping)
					{
						didEscape = true;
						escaping = true;
					}
					//? Can probably get performance gains here by checking columns before checking line feed
					//So this only gets hit if we have ",  and nothing like "", which would be in the middle of a cell, we need to make sure the quote is alone before the delimiter
					else if ((index + 1) < _length
								&& (dsv[index + 1] == _delimiter
									|| CheckLineFeed(dsv, index + 3))
						&& !escapedEscapeChar
						&& dsv[index - 1] != _escapeChar)
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
						if (!_hasHeaders
							|| (_columnsFilled
									&& _column == _columnCount)
								|| !_columnsFilled)
						{
							if ((index + 1) < _length
								&& CheckLineFeed(dsv, index + 1))
							{
								if (escaping)
								{
									//This entry was escaped, that means we need to remove 1 from each side
									nextValue = dsv.AsMemory().Slice(lastDelimiter + 1, index - lastDelimiter - 2 - 1);
								}
								else if (didEscape)
								{
									nextValue = dsv.AsMemory().Slice(lastDelimiter + 1, index - lastDelimiter - 2 - 1);
								}
								else
								{
									nextValue = dsv.AsMemory().Slice(lastDelimiter, index - lastDelimiter - 2 + 1);
								}


								if (didEscapeEscapeChar)
								{
									//Need to string.replace which will cause an allocation as we can't cut items out of the middle of a span
									nextValue = nextValue.ToString().Replace(doubleEscapeChar, escapeCharAsString).AsMemory();
								}

								//Check if we need to place the headers in
								if (_hasHeaders && !_columnsFilled)
								{
									_columnCount++;
									_columnsFilled = true;
								}

								_rowCount++;

								didEscape = false;
								didEscapeEscapeChar = false;
								//Start up a new Row
								//We treat the LineFeed as a delimiter as the last delimiter was before it
								lastDelimiter = index + 1;
								_newRowNextRead = true;

								return true;
							}
						}
					}
				}
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool CheckLineFeed(in ReadOnlySpan<char> dsv, int i)
		{
			if (i >= 2)
			{
				return ((dsv[i - 2] == '\r' || dsv[i - 2] == '\n')
					&& (dsv[i - 1] == '\r' || dsv[i - 1] == '\n'));
			}

			return false;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ThrowInvalidEscapedEscapeChar()
		{
			throw new FormatException("An escape character was detected in the cell as part of it but no escape character was after it.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ThrowInvalidRowColumn()
		{
			throw new FormatException("A row has more columns than the first line.");
		}

	}
}
