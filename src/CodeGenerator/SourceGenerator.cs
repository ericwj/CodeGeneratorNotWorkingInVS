using System;
using System.Text;

using CodeWriter;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace CodeGenerator
{
	[Generator]
	public partial class SourceGenerator : ISourceGenerator
	{
		public void Execute(GeneratorExecutionContext context) {
			var cs = @"using System;

namespace CodeGenerator
{
	public partial class Feature
	{
		public static string Use(ReadOnlySpan<char> span) => span.ToString();
	}
}
";
			// Just use all API's in Writer
			cs = $@"
// HashCode is 0x{Writer.GetHashCode(cs):x8}
// Last Character is \u{(int)cs[cs.Length - 1]:x4}
{cs}";

			cs = Writer.Write(cs.AsSpan());
			context.AddSource("GeneratedCode.g.cs", SourceText.From(cs, Encoding.Unicode));
		}

		public void Initialize(GeneratorInitializationContext context) { }
	}
}
