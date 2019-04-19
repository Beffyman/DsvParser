using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Beffyman.DsvParser
{
	/// <summary>
	/// Implement to create a mapping for a dsv row to be converted into an instance of <see cref="T"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class DsvParserTypeMapping<T> where T : new()
	{
		private Dictionary<int, DsvParserMapperDelegate<T>> _mappings = new Dictionary<int, DsvParserMapperDelegate<T>>();

		public void MapProperty(int column, DsvParserMapperDelegate<T> setter)
		{
			_mappings.Add(column, setter);
		}

		public void Map(ref T obj, int column, in ReadOnlySpan<char> data)
		{
			if (_mappings.ContainsKey(column))
			{
				_mappings[column](ref obj, data);
			}

			//var element = new T();
			//for (int i = 0; i < row.Length; i++)
			//{
			//	if (_mappings.ContainsKey(i))
			//	{
			//		_mappings[i](element, row[i]);
			//	}
			//}
			//return element;
		}

	}
}
