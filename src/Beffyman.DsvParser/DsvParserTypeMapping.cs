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
		private Dictionary<int, Action<T, ReadOnlyMemory<char>>> _mappings = new Dictionary<int, Action<T, ReadOnlyMemory<char>>>();

		public void MapProperty(int column, Action<T, ReadOnlyMemory<char>> setter)
		{
			_mappings.Add(column, setter);
		}

		public T Map(ReadOnlyMemory<char>[] row)
		{
			var element = new T();
			for (int i = 0; i < row.Length; i++)
			{
				if (_mappings.ContainsKey(i))
				{
					_mappings[i](element, row[i]);
				}
			}
			return element;
		}

	}
}
