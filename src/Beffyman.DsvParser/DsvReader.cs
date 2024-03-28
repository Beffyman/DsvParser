using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Beffyman.DsvParser
{
	/// <summary>
	/// Used to read through dsv data
	/// </summary>
	public sealed class DsvReader
	{
		public readonly DsvOptions Options;
		public readonly ReadOnlyMemory<char> Input;
		public readonly Encoding Encoding;
#if NETSTANDARD2_0
		private char[] EncodingPreamble;
#endif

		public int RowCount => _rowCount;
		public int ColumnCount => _columnCount;
		public int Column => _column;
		public bool ColumnsFilled => _columnsFilled;
		public bool NewRowNextRead => _newRowNextRead;

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
		private bool _lastReadIsLine;




		private int _reader_Start;
		private int _reader_Length;
		private bool _reader_Escaped;
		private int _reader_NumberOfEscapedEscapeCharacters;


		/// <summary>
		/// Converts the Span of bytes into an array and passes that with it's encoding into the byte[] constructor
		/// </summary>
		/// <param name="input"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(in ReadOnlySpan<byte> input, Encoding encoding, in DsvOptions options) : this(input.ToArray(), encoding, options) { }

		/// <summary>
		/// Converts the Memory of bytes into an array and passes that with it's encoding into the byte[] constructor
		/// </summary>
		/// <param name="input"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(in ReadOnlyMemory<byte> input, Encoding encoding, in DsvOptions options) : this(input.ToArray(), encoding, options) { }


		/// <summary>
		/// Converts the byte array into a string using the encoding and then provides the string into the constructor for strings
		/// </summary>
		/// <param name="input"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(byte[] input, Encoding encoding, in DsvOptions options) : this(encoding.GetString(input), encoding, options) { }


		/// <summary>
		/// Calls <see cref="MemoryExtensions.AsMemory(string)"/> on the string and passes it into the appropriate constructor
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(string input, Encoding encoding, in DsvOptions options) : this(input.AsMemory(), encoding, options) { }


		/// <summary>
		/// Converts the Span into an array so it can be cast into a <see cref="ReadOnlyMemory{char}"/>
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(in ReadOnlySpan<char> input, Encoding encoding, in DsvOptions options) : this(input.ToArray(), encoding, options) { }

		/// <summary>
		/// Converts the Span into an array so it can be cast into a <see cref="ReadOnlyMemory{char}"/>
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(in Span<char> input, Encoding encoding, in DsvOptions options) : this(input.ToArray(), encoding, options) { }

		/// <summary>
		/// Creates a new <see cref="ReadOnlyMemory{char}"/> from the array and passes it into the appropriate constructor
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(char[] input, Encoding encoding, in DsvOptions options) : this(new ReadOnlyMemory<char>(input), encoding, options) { }

		/// <summary>
		/// Creates a reader that will step through the DSV file's values
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvReader(in ReadOnlyMemory<char> input, Encoding encoding, in DsvOptions options)
		{
			Input = input;
			Options = options;

			ReadOnlySpan<char> dec = stackalloc char[] { options.EscapeChar, options.EscapeChar };
			ReadOnlySpan<char> ecas = stackalloc char[] { options.EscapeChar };

			doubleEscapeChar = dec.ToString();
			escapeCharAsString = ecas.ToString();

			_delimiter = Options.Delimiter;
			_hasHeaders = Options.HasHeaders;
			_escapeChar = Options.EscapeChar;
			_length = Input.Length;

			Encoding = encoding;
#if NETSTANDARD2_0
			EncodingPreamble = Encoding.GetChars(encoding.GetPreamble());
#endif
		}

		/// <summary>
		/// Gets the next value in the file, requires calling MoveNext after to move to the next value
		/// </summary>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<char> ReadNextAsSpan()
		{
			//? Input.Span, THIS PROPERTY CALL IS VERY EXPENSIVE, not sure if there is a way to get rid of it...

			if (_reader_Length > 0)
			{
				//Skip whitespace at the start of a column unless it's inside quotes

				var inputSpan = Input.Span;
				var startSkip = 0;
				for (int i = 0; i < _reader_Length; i++)
				{
					if (char.IsWhiteSpace(inputSpan[_reader_Start + i]))
					{
						startSkip++;
					}
					else
					{
						//When we hit our first non-whitespace, we skip
						break;
					}
				}

				_reader_Start += startSkip;
				_reader_Length -= startSkip;

				if (_reader_Length == 0)
				{
					ResetReader();
					return ReadOnlySpan<char>.Empty;
				}

				ReadOnlySpan<char> span = default;
				//If we do, then slice out the chars from until the last one we found.
				if (_reader_Escaped)
				{
					if (_reader_NumberOfEscapedEscapeCharacters > 0)
					{
#if NETSTANDARD2_1
						int length = _reader_Length - _reader_NumberOfEscapedEscapeCharacters;
						int start = _reader_Start;

						//? With netstandard2.1/netcoreapp, we get access to string.Create which allows us to make only 1 allocation for a replace operation in the way we need it
						span = string.Create(length, (this, _escapeChar, start, length), ReplaceEscapedChars).AsSpan();
#else
						//? Since we don't have access to string.Create, we need to allocate two new strings in order to perform the replace operation
						span = inputSpan.Slice(_reader_Start, _reader_Length).ToString().Replace(doubleEscapeChar, escapeCharAsString).AsSpan();
#endif
					}
					else
					{
						span = inputSpan.Slice(_reader_Start, _reader_Length);
					}
					//We know there was an escape char and a delimiter at the end, so take one off both sides for the escape, and 1 more from the end for the delimiter
				}
				else
				{
					span = inputSpan.Slice(_reader_Start, _reader_Length);
				}

				ResetReader();

				return span;
			}
			return default;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory<char> ReadNextAsMemory()
		{
			//? Input.Span, THIS PROPERTY CALL IS VERY EXPENSIVE, not sure if there is a way to get rid of it...

			if (_reader_Length > 0)
			{

				//Skip whitespace at the start of a column unless it's inside quotes

				var inputSpan = Input.Span;
				var startSkip = 0;
				for (int i = 0; i < _reader_Length; i++)
				{
					if (char.IsWhiteSpace(inputSpan[_reader_Start + i]))
					{
						startSkip++;
					}
					else
					{
						//When we hit our first non-whitespace, we skip
						break;
					}
				}

				_reader_Start += startSkip;
				_reader_Length -= startSkip;

				if (_reader_Length == 0)
				{
					ResetReader();
					return ReadOnlyMemory<char>.Empty;
				}

				ReadOnlyMemory<char> memory = default;
				//If we do, then slice out the chars from until the last one we found.
				if (_reader_Escaped)
				{
					if (_reader_NumberOfEscapedEscapeCharacters > 0)
					{
#if NETSTANDARD2_1
						int length = _reader_Length - _reader_NumberOfEscapedEscapeCharacters;
						int start = _reader_Start;

						//? With netstandard2.1/netcoreapp, we get access to string.Create which allows us to make only 1 allocation for a replace operation in the way we need it
						memory = string.Create(length, (this, _escapeChar, start, length), ReplaceEscapedChars).AsMemory();
#else
						//? Since we don't have access to string.Create, we need to allocate two new strings in order to perform the replace operation
						memory = Input.Slice(_reader_Start, _reader_Length).ToString().Replace(doubleEscapeChar, escapeCharAsString).AsMemory();
#endif
					}
					else
					{
						memory = Input.Slice(_reader_Start, _reader_Length);
					}
					//We know there was an escape char and a delimiter at the end, so take one off both sides for the escape, and 1 more from the end for the delimiter
				}
				else
				{
					memory = Input.Slice(_reader_Start, _reader_Length);
				}

				ResetReader();

				return memory;
			}
			return default;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ResetReader()
		{
			_reader_Start = 0;
			_reader_Length = 0;
			_reader_NumberOfEscapedEscapeCharacters = 0;
			_reader_Escaped = false;
		}

		/// <summary>
		/// Reads the next line of the DSV, but this allocates memory for the array and data inside it since it can't return a collection of Spans
		/// </summary>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory<char>[] ReadLine()
		{
			if (_columnsFilled)
			{
				ReadOnlyMemory<char>[] arr = new ReadOnlyMemory<char>[_columnCount];

				int col = 0;
				int startRow = _rowCount;
				do
				{
					arr[col++] = ReadNextAsMemory();
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

					arg[col++] = ReadNextAsMemory();
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

		/// <summary>
		/// Move to the next line in the DSV
		/// </summary>
		/// <exception cref="FormatException" />
		/// <returns>Return false if there if nothing else can be read from the data</returns>
		public bool MoveNextLine()
		{
			bool canMove = true;
			int row = _rowCount;
			while (row == _rowCount && canMove)
			{
				canMove = MoveNext();
			}

			return canMove;
		}

		/// <summary>
		/// Move to the next value in the DSV
		/// </summary>
		/// <exception cref="FormatException" />
		/// <returns>Return false if there if nothing else can be read from the data</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			//Known limits
			//Uses int for enumerating, so 2,147,483,647 bytes is the size limit

			//Need to account for the following
			//Quotes
			//Escaped Quotes
			//Escaped Delimiters
			//Escaped LineFeed, Carriage Returns
			//Skip BOM on any encoding

			//Could probably get better performance with a switch, but would need constant delimiter

			ResetReader();

			//? THIS PROPERTY CALL IS VERY EXPENSIVE, not sure if there is a way to get rid of it...
			var dsv = Input.Span;

			if (_newRowNextRead)
			{
				_column = 0;
				_newRowNextRead = false;
			}

			if (index != 0)
			{
				index++;
			}
			//Case where we have a start comma, would just loop endlessly on index=0
			else if (index == 0 && _column != 0)
			{
				index++;
			}
			else
			{
				if (Encoding != null)
				{
					//On first pass, skip over BOM value
#if NETSTANDARD2_0
					var encodingPreamble = EncodingPreamble;
#else
					var encodingPreamble = Encoding.GetChars(Encoding.Preamble.ToArray());
#endif
					int i = 0;
					for (i = 0; i < encodingPreamble.Length; i++)
					{
						char indexValue = dsv[i];
						char preambleValue = encodingPreamble[i];
						if (indexValue != preambleValue)
						{
							break;
						}
					}
					index += i;
					lastDelimiter = index;
				}
			}

			for (; index < _length; index++)
			{
				char indexValue = dsv[index];
				//Check if we have a delimiter
				if (indexValue == _delimiter)
				{
					if (!escaping || (escaping && escapedEscapeChar))
					{
						//If we do, then slice out the chars from until the last one we found.
						if (didEscape)
						{
							//We know there was an escape char and a delimiter at the end, so take one off both sides for the escape, and 1 more from the end for the delimiter
							_reader_Start = lastDelimiter + 1;
							_reader_Length = index - lastDelimiter - 2;
						}
						else
						{
							_reader_Start = lastDelimiter;
							_reader_Length = index - lastDelimiter;
						}

						if (didEscapeEscapeChar)
						{
							_reader_Escaped = true;
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
					var newline = CheckLineFeed(dsv, index + 1);
					//Finalize
					if (escaping)
					{
						switch (newline)
						{
							case NewLineType.LineFeed:
							case NewLineType.CarriageReturn:
								//This entry was escaped, that means we need to remove 1 from each side, but also take off the line feed from the end of the file
								_reader_Start = lastDelimiter + 1;
								_reader_Length = (_length - lastDelimiter - GetNewLineLength(newline)) - 1;
								break;
							case NewLineType.None:
								//Take 1 off both sides and 1 more as we are still escaping
								_reader_Start = lastDelimiter + 1;
								_reader_Length = _length - lastDelimiter - 2;
								break;
						}
					}
					else if (newline != NewLineType.None)
					{
						if (didEscape)
						{
							//This entry was escaped, that means we need to remove 1 from each side, but also take off the line feed from the end of the file
							_reader_Start = lastDelimiter + 1;
							_reader_Length = (_length - lastDelimiter - GetNewLineLength(newline)) - 2;
						}
						else
						{
							_reader_Start = lastDelimiter;
							_reader_Length = _length - lastDelimiter - GetNewLineLength(newline);
						}
					}
					else
					{
						_reader_Start = lastDelimiter;
						_reader_Length = _length - lastDelimiter;
					}

					if (didEscapeEscapeChar)
					{
						_reader_Escaped = true;
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
					var newline = CheckLineFeed_ForwardLook(dsv, index);
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
									|| newline != NewLineType.None)
						&& !escapedEscapeChar)
					{
						escaping = false;
					}
					else if (!escapedEscapeChar)
					{
						_reader_NumberOfEscapedEscapeCharacters++;
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
						var newline = CheckLineFeed(dsv, index + 1);
						if (_lastReadIsLine && newline != NewLineType.None)
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
								&& newline != NewLineType.None)
							{
								if (escaping)
								{
									//This entry was escaped, that means we need to remove 1 from each side
									_reader_Start = lastDelimiter + 1;
									_reader_Length = index - lastDelimiter - GetNewLineLength(newline) - 1;
								}
								else if (didEscape)
								{
									_reader_Start = lastDelimiter + 1;
									_reader_Length = index - lastDelimiter - GetNewLineLength(newline) - 1;
								}
								else
								{
									_reader_Start = lastDelimiter;
									_reader_Length = index - lastDelimiter - GetNewLineLength(newline) + 1;
								}


								if (didEscapeEscapeChar)
								{
									_reader_Escaped = true;
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

#pragma warning disable EPS05
		/// <summary>
		/// Used to construct a string that has the escapee chars replaced
		/// </summary>
		/// <param name="chars"></param>
		/// <param name="data"></param>
		private static void ReplaceEscapedChars(Span<char> chars, (DsvReader reader, char escapeChar, int start, int length) data)
		{
			int offset = 0;
			for (int i = 0; i < chars.Length; i++)
			{
				if (data.reader.Input.Span[data.start + i + offset] == data.escapeChar)
				{
					offset++;
				}
				chars[i] = data.reader.Input.Span[data.start + i + offset];
			}
		}

#pragma warning restore EPS05
		/// <summary>
		/// Backwards check for newline, this is called after the newline is processed
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private NewLineType CheckLineFeed(in ReadOnlySpan<char> dsv, int i)
		{
			if (i >= 2)
			{
				//Check for \r\n
				if (dsv[i - 1] == '\n')
				{
					if (dsv[i - 2] == '\r')
					{
						return NewLineType.CarriageReturn;
					}
					else
					{
						return NewLineType.LineFeed;
					}
				}
			}
			else if (i >= 1)
			{
				//Only do \n check since not enough chars passed
				if (dsv[i - 1] == '\n')
				{
					return NewLineType.LineFeed;
				}
			}

			return NewLineType.None;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private NewLineType CheckLineFeed_ForwardLook(in ReadOnlySpan<char> dsv, int i)
		{
			if (i + 2 < dsv.Length)
			{
				//Check for \r\n
				if (dsv[i + 1] == '\n')
				{
					return NewLineType.LineFeed;
				}
				else if (dsv[i + 1] == '\r'
					&& dsv[i + 2] == '\n')
				{
					return NewLineType.CarriageReturn;
				}
			}
			else if (i + 1 < dsv.Length)
			{
				//Only do \n check since not enough chars in string left
				if (dsv[i + 1] == '\n')
				{
					return NewLineType.LineFeed;
				}
			}

			return NewLineType.None;
		}

		/// <summary>
		/// Gets the length of the newline characters used \r\n or \n
		/// </summary>
		/// <param name="newline"></param>
		/// <returns></returns>
		private int GetNewLineLength(NewLineType newline)
		{
			switch (newline)
			{
				case NewLineType.CarriageReturn:
					return 2;
				case NewLineType.LineFeed:
					return 1;
				case NewLineType.None:
				default:
					return 0;
			}
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
