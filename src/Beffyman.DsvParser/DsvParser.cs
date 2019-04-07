using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Beffyman.DsvParser
{
	/// <summary>
	/// Parses data that has separated values into a consumable format
	/// </summary>
	public sealed class DsvParser
	{
		/// <summary>
		/// Headers that are contained within the parsed data
		/// </summary>
		public readonly ReadOnlyMemory<ReadOnlyMemory<char>> Columns;
		/// <summary>
		/// Rows that are contained within the parsed data
		/// </summary>
		public readonly ReadOnlyMemory<ReadOnlyMemory<ReadOnlyMemory<char>>> Rows;

		/// <summary>
		/// When you have a byte array and a known encoding.
		/// Least performant option as it needs to parse the bytes as a string via the encoding then convert the string that comes back from that as a Span
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="encoding"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(byte[] dsv, Encoding encoding, in DsvOptions options) : this(encoding.GetString(dsv), options) { }

		/// <summary>
		/// Converts the char array into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(char[] dsv, in DsvOptions options) : this(dsv.AsSpan(), options) { }

		/// <summary>
		/// Converts the string into a span and then parses it
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(string dsv, in DsvOptions options) : this(dsv.AsSpan(), options) { }

		/// <summary>
		/// Converts the <see cref="ReadOnlyMemory{T}"/> into a <see cref="ReadOnlySpan{T}"/> and uses that to parse the data
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlyMemory<char> dsv, in DsvOptions options) : this(dsv.Span, options) { }

		/// <summary>
		/// Directly uses the Span provided to parse the data, most performant option
		/// </summary>
		/// <param name="dsv"></param>
		/// <param name="options"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DsvParser(in ReadOnlySpan<char> dsv, in DsvOptions options)
		{
			var reader = new DsvReader(dsv, options);

			bool firstPass = options.HasHeaders;
			var rows = new List<ReadOnlyMemory<ReadOnlyMemory<char>>>();
			while (reader.MoveNext())
			{
				if (firstPass)
				{
					Columns = reader.ReadLine();
					firstPass = false;
				}
				else
				{
					rows.Add(reader.ReadLine());
				}
			}

			Rows = rows.ToArray();
		}

	}
}
