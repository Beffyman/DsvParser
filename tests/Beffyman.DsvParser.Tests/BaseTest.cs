using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.DsvParser.Tests
{
	public abstract class BaseTest
	{
		protected string FileGenerator(string headerPrefix, string dataPrefix, string lineBreak, int rows, int columns)
		{
			StringBuilder builder = new StringBuilder();
			int x = 0;

			for (int i = 0; i < columns; i++)
			{
				if (i != 0)
				{
					builder.Append(',');
				}
				builder.Append($"{headerPrefix}{++x}");
			}

			builder.Append(lineBreak);

			for (int i = 0; i < rows; i++)
			{
				int z = 0;
				for (int u = 0; u < columns; u++)
				{
					if (u != 0)
					{
						builder.Append(',');
					}
					builder.Append($"{dataPrefix}{++z}");
				}
				builder.Append(lineBreak);
			}

			return builder.ToString();
		}

	}
}
