using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Beffyman.DsvParser
{
	public sealed class DsvParser<TRecord, TRecordMapping> where TRecord : new()
		where TRecordMapping : DsvParserTypeMapping<TRecord>, new()
	{
		/// <summary>
		/// Headers that are contained within the parsed data
		/// </summary>
		public readonly ReadOnlyMemory<ReadOnlyMemory<char>> Columns;

		/// <summary>
		/// Rows that are contained within the parsed data
		/// </summary>
		public readonly IReadOnlyList<TRecord> Rows;

		/// <summary>
		/// When you have a span of bytes and a known encoding
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlySpan<byte> dsv, Encoding encoding, in DsvOptions options) : this(dsv.ToArray(), encoding, options, new TRecordMapping()) { }

		/// <summary>
		/// When you have a span of bytes and a known encoding
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		/// <param name="mapping"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlySpan<byte> dsv, Encoding encoding, in DsvOptions options, TRecordMapping mapping) : this(dsv.ToArray(), encoding, options, mapping) { }

		/// <summary>
		/// When you have a Memory of bytes and a known encoding.
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlyMemory<byte> dsv, Encoding encoding, in DsvOptions options) : this(dsv.ToArray(), encoding, options, new TRecordMapping()) { }

		/// <summary>
		/// When you have a Memory of bytes and a known encoding.
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		/// <param name="mapping"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlyMemory<byte> dsv, Encoding encoding, in DsvOptions options, TRecordMapping mapping) : this(dsv.ToArray(), encoding, options, mapping) { }

		/// <summary>
		/// When you have a byte array and a known encoding.
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(byte[] dsv, Encoding encoding, in DsvOptions options) : this(encoding.GetString(dsv), encoding, options, new TRecordMapping()) { }

		/// <summary>
		/// When you have a byte array and a known encoding.
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		/// <param name="mapping"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(byte[] dsv, Encoding encoding, in DsvOptions options, TRecordMapping mapping) : this(encoding.GetString(dsv), encoding, options, mapping) { }

		/// <summary>
		/// Converts the char array into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(char[] dsv, Encoding encoding, in DsvOptions options) : this(dsv.AsSpan(), encoding, options, new TRecordMapping()) { }

		/// <summary>
		/// Converts the char array into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <param name="mapping"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(char[] dsv, Encoding encoding, in DsvOptions options, TRecordMapping mapping) : this(dsv.AsSpan(), encoding, options, mapping) { }

		/// <summary>
		/// Converts the string into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(string dsv, Encoding encoding, in DsvOptions options) : this(dsv.AsSpan(), encoding, options, new TRecordMapping()) { }

		/// <summary>
		/// Converts the string into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <param name="mapping"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(string dsv, Encoding encoding, in DsvOptions options, TRecordMapping mapping) : this(dsv.AsSpan(), encoding, options, mapping) { }

		/// <summary>
		/// Converts the <see cref="ReadOnlyMemory{T}"/> into a <see cref="ReadOnlySpan{T}"/> and uses that to parse the data
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlyMemory<char> dsv, Encoding encoding, in DsvOptions options) : this(dsv.Span, encoding, options, new TRecordMapping()) { }

		/// <summary>
		/// Converts the <see cref="ReadOnlyMemory{T}"/> into a <see cref="ReadOnlySpan{T}"/> and uses that to parse the data
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <param name="mapping"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlyMemory<char> dsv, Encoding encoding, in DsvOptions options, TRecordMapping mapping) : this(dsv.Span, encoding, options, mapping) { }

		/// <summary>
		/// Directly uses the Span provided to parse the data, most performant option
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlySpan<char> dsv, Encoding encoding, in DsvOptions options) : this(dsv, encoding, options, new TRecordMapping()) { }

		/// <summary>
		/// Directly uses the Span provided to parse the data, most performant option
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <param name="mapping"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlySpan<char> dsv, Encoding encoding, in DsvOptions options, TRecordMapping mapping)
		{
			var reader = new DsvReader(dsv, encoding, options);

			//? This can be greatly improved upon performance wise
			//? Use ReadNextAsSpan, manually check for next line/handle columns
			//? This way we can use the Span<char> for the parsing

			var rows = new List<TRecord>();

			while (reader.MoveNext())
			{
				if (reader.ColumnsFilled)
				{
					TRecord record = new TRecord();

					int col = 0;
					int startRow = reader.RowCount;
					do
					{
						mapping.Map(ref record, col++, reader.ReadNextAsSpan());
					}
					while (reader.RowCount == startRow && reader.MoveNext());

					rows.Add(record);
				}
				else
				{
					//First pass, as we don't know how many columns there are, we need to have a resizable array as the collection
					int length = 4;
					ReadOnlyMemory<char>[] arg = new ReadOnlyMemory<char>[length];

					int col = 0;
					int startRow = reader.RowCount;
					do
					{
						//If we need to resize, then double the size
						if (col >= length)
						{
							length *= 2;
							Array.Resize(ref arg, length);
						}

						arg[col++] = reader.ReadNextAsMemory();
					}
					while (reader.RowCount == startRow && reader.MoveNext());

					//Resize back to correct length after we are done
					if (length != col)
					{
						Array.Resize(ref arg, col);
					}

					Columns = arg;
				}
			}

			Rows = rows;
		}
	}
}
