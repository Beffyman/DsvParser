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
		public readonly ReadOnlyMemory<TRecord> Rows;

		/// <summary>
		/// When you have a span of bytes and a known encoding
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(ReadOnlySpan<byte> dsv, Encoding encoding, in DsvOptions options) : this(dsv.ToArray(), encoding, options, new TRecordMapping()) { }

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
		public DsvParser(ReadOnlySpan<byte> dsv, Encoding encoding, in DsvOptions options, TRecordMapping mapping) : this(dsv.ToArray(), encoding, options, mapping) { }

		/// <summary>
		/// When you have a Memory of bytes and a known encoding.
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(ReadOnlyMemory<byte> dsv, Encoding encoding, in DsvOptions options) : this(dsv.ToArray(), encoding, options, new TRecordMapping()) { }

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
		public DsvParser(ReadOnlyMemory<byte> dsv, Encoding encoding, in DsvOptions options, TRecordMapping mapping) : this(dsv.ToArray(), encoding, options, mapping) { }

		/// <summary>
		/// When you have a byte array and a known encoding.
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(byte[] dsv, Encoding encoding, in DsvOptions options) : this(encoding.GetString(dsv), options, new TRecordMapping()) { }

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
		public DsvParser(byte[] dsv, Encoding encoding, in DsvOptions options, TRecordMapping mapping) : this(encoding.GetString(dsv), options, mapping) { }

		/// <summary>
		/// Converts the char array into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(char[] dsv, in DsvOptions options) : this(dsv.AsSpan(), options, new TRecordMapping()) { }

		/// <summary>
		/// Converts the char array into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <param name="mapping"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(char[] dsv, in DsvOptions options, TRecordMapping mapping) : this(dsv.AsSpan(), options, mapping) { }

		/// <summary>
		/// Converts the string into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(string dsv, in DsvOptions options) : this(dsv.AsSpan(), options, new TRecordMapping()) { }

		/// <summary>
		/// Converts the string into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <param name="mapping"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(string dsv, in DsvOptions options, TRecordMapping mapping) : this(dsv.AsSpan(), options, mapping) { }

		/// <summary>
		/// Converts the <see cref="ReadOnlyMemory{T}"/> into a <see cref="ReadOnlySpan{T}"/> and uses that to parse the data
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlyMemory<char> dsv, in DsvOptions options) : this(dsv.Span, options, new TRecordMapping()) { }

		/// <summary>
		/// Converts the <see cref="ReadOnlyMemory{T}"/> into a <see cref="ReadOnlySpan{T}"/> and uses that to parse the data
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <param name="mapping"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlyMemory<char> dsv, in DsvOptions options, TRecordMapping mapping) : this(dsv.Span, options, mapping) { }

		/// <summary>
		/// Directly uses the Span provided to parse the data, most performant option
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlySpan<char> dsv, in DsvOptions options) : this(dsv, options, new TRecordMapping()) { }

		/// <summary>
		/// Directly uses the Span provided to parse the data, most performant option
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		/// <param name="mapping"></param>
		/// <exception cref="FormatException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlySpan<char> dsv, in DsvOptions options, TRecordMapping mapping)
		{
			var reader = new DsvReader(dsv, options);

			bool firstPass = options.HasHeaders;
			var rows = new List<TRecord>();
			while (reader.MoveNext())
			{
				if (firstPass)
				{
					Columns = reader.ReadLine();
					firstPass = false;
				}
				else
				{
					rows.Add(mapping.Map(reader.ReadLine()));
				}
			}

			Rows = rows.ToArray();
		}
	}
}
