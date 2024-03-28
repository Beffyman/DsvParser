using System.ComponentModel;
using Nuke.Common.Tooling;

[TypeConverter(typeof(TypeConverter<Configuration>))]
public class Configuration : Enumeration
{
	public static Configuration Debug = new Configuration
	{
		Value = "Debug"
	};
	public static Configuration Release = new Configuration
	{
		Value = "Release"
	};

	public static implicit operator string(Configuration configuration)
	{
		return configuration.Value;
	}
}