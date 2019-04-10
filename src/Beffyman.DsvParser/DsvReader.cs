using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Beffyman.DsvParser
{
	public sealed class DsvReader
	{
		public readonly DsvOptions Options;
		public readonly ReadOnlyMemory<char> Input;

		public int RowCount => _rowCount;
		public int ColumnCount => _columnCount;
		public int Column => _column;
		public bool ColumnsFilled => _columnsFilled;
		public bool NewRowNextRead => _newRowNextRead;

		private readonly char[] _dsv;
		private readonly char _delimiter;
		private readonly char _escapeChar;
		private readonly bool _hasHeaders;
		private readonly int _length;

		private int _rowCount;
		private int _columnCount;
		private int _column;
		private bool _columnsFilled = false;
		private bool _newRowNextRead;
		private bool escaping;
		private bool didEscape;
		private bool escapedEscapeChar;
		private bool didEscapeEscapeChar;
		private string doubleEscapeChar;
		private string escapeCharAsString;
		private int lastDelimiter;
		private int index;
		private ReadOnlyMemory<char> _nextValue;
		private bool _lastReadIsLine;



		/// <summary>
		/// Converts the byte array into a string using the encoding and then provides the string into the constructor for strings
		/// </summary>
		/// <param name="input"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(byte[] input, Encoding encoding, in DsvOptions options) : this(encoding.GetString(input), options) { }


		/// <summary>
		/// Calls <see cref="MemoryExtensions.AsMemory(string)"/> on the string and passes it into the appropriate constructor
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(string input, in DsvOptions options) : this(input.AsMemory(), options) { }


		/// <summary>
		/// Converts the Span into an array so it can be cast into a <see cref="ReadOnlyMemory{char}"/>
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(in ReadOnlySpan<char> input, in DsvOptions options) : this(input.ToArray(), options) { }

		/// <summary>
		/// Converts the Span into an array so it can be cast into a <see cref="ReadOnlyMemory{char}"/>
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(in Span<char> input, in DsvOptions options) : this(input.ToArray(), options) { }

		/// <summary>
		/// Creates a new <see cref="ReadOnlyMemory{char}"/> from the array and passes it into the appropriate constructor
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(char[] input, in DsvOptions options) : this(new ReadOnlyMemory<char>(input), options) { }

		/// <summary>
		/// Creates a reader that will step through the DSV file's values
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(in ReadOnlyMemory<char> input, in DsvOptions options)
		{
			Input = input;
			_dsv = input.ToArray();
			Options = options;

			ReadOnlySpan<char> dec = stackalloc char[] { options.EscapeChar, options.EscapeChar };
			ReadOnlySpan<char> ecas = stackalloc char[] { options.EscapeChar };

			doubleEscapeChar = dec.ToString();
			escapeCharAsString = ecas.ToString();

			_delimiter = Options.Delimiter;
			_hasHeaders = Options.HasHeaders;
			_escapeChar = Options.EscapeChar;
			_length = _dsv.Length;
		}

		/// <summary>
		/// Move to the next value in the DSV
		/// </summary>
		/// <returns>Return false if there if nothing else can be read from the data</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			return MoveToNext();
		}

		/// <summary>
		/// Gets the next value in the file, requires calling MoveNext after to move to the next value
		/// </summary>
		/// <returns></returns>
		public ReadOnlyMemory<char> ReadNext()
		{
			var r = _nextValue;
			_nextValue = null;
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
					arr[col++] = _nextValue;
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

					arg[col++] = _nextValue;
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

			_nextValue = null;
			//? GET RID OF PROPERTIES, THEY COST A LOT!!!
			//var dsv = Input.Span;

			if (_newRowNextRead)
			{
				_column = 0;
				_newRowNextRead = false;
			}

			if (index != 0)
			{
				index++;
			}

			for (; index < _length; index++)
			{
				char indexValue = _dsv[index];
				//Check if we have a delimiter
				if (indexValue == _delimiter)
				{
					if (!escaping || (escaping && escapedEscapeChar))
					{
						//If we do, then slice out the chars from until the last one we found.
						if (didEscape)
						{
							//We know there was an escape char and a delimiter at the end, so take one off both sides for the escape, and 1 more from the end for the delimiter
							_nextValue = _dsv.AsMemory().Slice(lastDelimiter + 1, index - lastDelimiter - 2);
						}
						else
						{
							_nextValue = _dsv.AsMemory().Slice(lastDelimiter, index - lastDelimiter);
						}

						if (didEscapeEscapeChar)
						{
							//Need to string.replace which will cause an allocation as we can't cut items out of the middle of a span
							_nextValue = _nextValue.ToString().Replace(doubleEscapeChar, escapeCharAsString).AsMemory();
						}

						lastDelimiter = index + 1;
						didEscape = false;
						_lastReadIsLine = false;

						if (!_columnsFilled)
						{
							_columnCount++;
						}
						_column++;

						if (_columnsFilled && _column > _columnCount)
						{
							ThrowInvalidRowColumn();
						}

						return true;
					}
				}
				else if (index == _length - 1)
				{
					//Finalize
					if (escaping)
					{
						if (CheckLineFeed(_dsv, index + 1))
						{
							//This entry was escaped, that means we need to remove 1 from each side, but also take off the line feed from the end of the file
							_nextValue = _dsv.AsMemory().Slice(lastDelimiter + 1, (_length - lastDelimiter - 2) - 1);
						}
						else
						{
							//Take 1 off both sides and 1 more as we are still escaping
							_nextValue = _dsv.AsMemory().Slice(lastDelimiter + 1, (_length - lastDelimiter) - 2);
						}

					}
					else if (CheckLineFeed(_dsv, index + 1))
					{
						_nextValue = _dsv.AsMemory().Slice(lastDelimiter, _length - lastDelimiter - 2);
					}
					else
					{
						_nextValue = _dsv.AsMemory().Slice(lastDelimiter, _length - lastDelimiter);
					}

					if (didEscapeEscapeChar)
					{
						//Need to string.replace which will cause an allocation as we can't cut items out of the middle of a span
						_nextValue = _nextValue.ToString().Replace(doubleEscapeChar, escapeCharAsString).AsMemory();
					}

					didEscape = false;
					didEscapeEscapeChar = false;
					_rowCount++;
					_column++;
					_newRowNextRead = false;
					_lastReadIsLine = false;

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
								&& (_dsv[index + 1] == _delimiter
									|| CheckLineFeed(_dsv, index + 3))
						&& !escapedEscapeChar
						&& _dsv[index - 1] != _escapeChar)
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
						if (_lastReadIsLine && CheckLineFeed(_dsv, index + 1))
						{
							_lastReadIsLine = true;
							lastDelimiter = index + 1;
							didEscape = false;
							didEscapeEscapeChar = false;
						}

						if (!_hasHeaders
							|| (_columnsFilled && _column >= _columnCount - 1)
							|| !_columnsFilled)
						{
							if (_column > _columnCount)
							{
								ThrowInvalidRowColumn();
							}

							if ((index + 1) < _length
								&& CheckLineFeed(_dsv, index + 1))
							{
								if (escaping)
								{
									//This entry was escaped, that means we need to remove 1 from each side
									_nextValue = _dsv.AsMemory().Slice(lastDelimiter + 1, index - lastDelimiter - 2 - 1);
								}
								else if (didEscape)
								{
									_nextValue = _dsv.AsMemory().Slice(lastDelimiter + 1, index - lastDelimiter - 2 - 1);
								}
								else
								{
									_nextValue = _dsv.AsMemory().Slice(lastDelimiter, index - lastDelimiter - 2 + 1);
								}


								if (didEscapeEscapeChar)
								{
									//Need to string.replace which will cause an allocation as we can't cut items out of the middle of a span
									_nextValue = _nextValue.ToString().Replace(doubleEscapeChar, escapeCharAsString).AsMemory();
								}

								//Check if we need to place the headers in
								if (_hasHeaders && !_columnsFilled)
								{
									_columnCount++;
									_columnsFilled = true;
								}
								_column++;

								_rowCount++;

								didEscape = false;
								didEscapeEscapeChar = false;
								//Start up a new Row
								//We treat the LineFeed as a delimiter as the last delimiter was before it
								lastDelimiter = index + 1;
								_newRowNextRead = true;
								_lastReadIsLine = true;

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
