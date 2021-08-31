using System;

namespace CodeGenerator
{
	public class Feature
	{
		public string Use(ReadOnlySpan<char> span) => span.ToString();
	}
}
