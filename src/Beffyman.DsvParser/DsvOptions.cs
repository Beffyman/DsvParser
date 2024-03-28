using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.DsvParser
{
	/// <summary>
	/// Options for how the DSV data will be parsed
	/// </summary>
	public readonly struct DsvOptions
	{
		/// <summary>
		/// What is the character that will be used to indicate that one data field ended
		/// </summary>
		public readonly char Delimiter;

		/// <summary>
		/// What is the character that will be used to indicate that the column is being escaped
		/// </summary>
		public readonly char EscapeChar;

		/// <summary>
		/// Should the first row be parsed as a header?
		/// </summary>
		public readonly bool HasHeaders;

		//Other options
		//Ignore malformed endings?
		//ignore quote escapes?

		/// <summary>
		/// Full Options list
		/// </summary>
		/// <param name="delimiter"></param>
		/// <param name="escapeChar"></param>
		/// <param name="hasHeaders"></param>
		public DsvOptions(char delimiter, char escapeChar, bool hasHeaders)
		{
			Delimiter = delimiter;
			EscapeChar = escapeChar;
			HasHeaders = hasHeaders;
		}

		/// <summary>
		/// Easiest option for an unknown file with columns
		/// </summary>
		/// <param name="delimiter"></param>
		/// <param name="escapeChar"></param>
		public DsvOptions(char delimiter, char escapeChar)
		{
			Delimiter = delimiter;
			EscapeChar = escapeChar;
			HasHeaders = true;
		}

		/// <summary>
		/// Default CSV options, comma separated, has headers, and has 0 knowns
		/// </summary>
		public readonly static DsvOptions DefaultCsvOptions = new DsvOptions(',', '"');

		/// <summary>
		/// Default CSV options, tab separated, has headers, and has 0 knowns
		/// </summary>
		public readonly static DsvOptions DefaultTsvOptions = new DsvOptions('\t', '"');

		/// <summary>
		/// Default CSV options, pipe separated, has headers, and has 0 knowns
		/// </summary>
		public readonly static DsvOptions DefaultPsvOptions = new DsvOptions('|', '"');

	}
}
