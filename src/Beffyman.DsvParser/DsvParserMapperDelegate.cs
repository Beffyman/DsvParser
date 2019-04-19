using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.DsvParser
{
	public delegate void DsvParserMapperDelegate<T>(ref T obj, in ReadOnlySpan<char> data) where T : new();
}
