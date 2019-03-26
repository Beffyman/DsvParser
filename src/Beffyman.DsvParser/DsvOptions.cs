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
		/// Value of the LineBreak that is to be detected
		/// </summary>
		public readonly string LineBreak;

		/// <summary>
		/// Should the first row be parsed as a header?
		/// </summary>
		public readonly bool HasHeaders;

		/// <summary>
		/// Known amount of headers to prevent resizing of collections during parsing, IF NOT 0, it will skip the first pass to detect how many columns
		/// </summary>
		public readonly int KnownColumns;
		/// <summary>
		/// Known amount of rows to prevent resizing of collections during parsing, IF NOT 0, it will skip resizing of the Spans during parsing
		/// </summary>
		public readonly int KnownRows;


		//Other options
		//Ignore malformed endings?
		//ignore quote escapes?

		/// <summary>
		/// Full Options list
		/// </summary>
		/// <param name="delimiter"></param>
		/// <param name="escapeChar"></param>
		/// <param name="lineBreak"></param>
		/// <param name="hasHeaders"></param>
		/// <param name="knownColumns"></param>
		/// <param name="knownRows"></param>
		public DsvOptions(char delimiter, char escapeChar, string lineBreak, bool hasHeaders, int knownColumns, int knownRows)
		{
			Delimiter = delimiter;
			EscapeChar = escapeChar;
			LineBreak = lineBreak;
			HasHeaders = hasHeaders;
			KnownColumns = knownColumns;
			KnownRows = knownRows;
		}

		/// <summary>
		/// Easiest option for an unknown file with columns
		/// </summary>
		/// <param name="delimiter"></param>
		/// <param name="escapeChar"></param>
		/// <param name="lineBreak"></param>
		public DsvOptions(char delimiter, char escapeChar, string lineBreak)
		{
			Delimiter = delimiter;
			EscapeChar = escapeChar;
			LineBreak = lineBreak;
			HasHeaders = true;
			KnownColumns = 0;
			KnownRows = 0;
		}

		/// <summary>
		/// Easiest option for an unknown file
		/// </summary>
		/// <param name="delimiter"></param>
		/// <param name="escapeChar"></param>
		/// <param name="lineBreak"></param>
		/// <param name="hasHeaders"></param>
		public DsvOptions(char delimiter, char escapeChar, string lineBreak, bool hasHeaders)
		{
			Delimiter = delimiter;
			EscapeChar = escapeChar;
			HasHeaders = hasHeaders;
			LineBreak = lineBreak;
			KnownColumns = 0;
			KnownRows = 0;
		}

		/// <summary>
		/// Default CSV options, comma separated, has headers, uses \r\n as the line break, and has 0 knowns
		/// </summary>
		public readonly static DsvOptions DefaultCsvOptions = new DsvOptions(',', '"', "\r\n");

		/// <summary>
		/// Default CSV options, tab separated, has headers, uses \r\n as the line break, and has 0 knowns
		/// </summary>
		public readonly static DsvOptions DefaultTsvOptions = new DsvOptions('\t', '"', "\r\n");

		/// <summary>
		/// Default CSV options, pipe separated, has headers, uses \r\n as the line break, and has 0 knowns
		/// </summary>
		public readonly static DsvOptions DefaultPsvOptions = new DsvOptions('|', '"', "\r\n");

	}
}
