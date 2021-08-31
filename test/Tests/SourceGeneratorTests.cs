using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using CodeGenerator;
using Xunit;
using System.Collections.Generic;

namespace EricWJ.Ansi.CodeGenerator.Tests
{
	public class GeneratorTests
	{
		[Fact]
		public void ComplexGeneratorTest() {
			var comp = CreateCompilationFromDirectory(
				@"C:\Users\Eric\Source\Issues\CodeGeneratorNotWorkingInVS17\src\Library",
				@".*\\.*\.cs",
				@".*\\bin\\.*|.*\\obj\\.*|.*\\.*\.g\.cs");
			var newComp = RunGenerators(comp, out var generatorDiags, new SourceGenerator());
		}

		[Fact]
		public void SimpleGeneratorTest() {
			var userSource = "";
			var comp = CreateCompilation(userSource);
			var newComp = RunGenerators(comp, out var generatorDiags, new SourceGenerator());

			Assert.Empty(generatorDiags);
			Assert.Empty(newComp.GetDiagnostics());

			var expected = @"
// HashCode is 0x243fb6ae
// Last Character is \u000a
using System;

namespace CodeGenerator
{
	public partial class Feature
	{
		public static string Use(ReadOnlySpan<char> span) => span.ToString();
	}
}
";
			var actual = (
				from tree in newComp.SyntaxTrees
				let name = Path.GetFileName(tree.FilePath)
				let ext = Path.GetExtension(tree.FilePath)
				where ext == ".g.cs"
				select tree.GetText().ToString()
			).Single();

			Assert.Equal(expected, actual);
		}
		private static IEnumerable<string> GetCompilationSources(string path, string include, string exclude) {
			var f = 0
				| RegexOptions.IgnoreCase
				| RegexOptions.Singleline
				| RegexOptions.Compiled
				| RegexOptions.CultureInvariant;
			var i = new Regex(include, f);
			var e = new Regex(exclude, f);
			return GetCompilationSources(path, i, e);
		}
		private static IEnumerable<string> GetCompilationSources(string path, Regex include, Regex exclude) {
			foreach (var dir in Directory.EnumerateDirectories(path)) {
				foreach (var file in GetCompilationSources(dir, include, exclude))
					yield return file;
			}
			foreach (var file in Directory.EnumerateFiles(path, "*.cs", SearchOption.TopDirectoryOnly)) {
				if (!include.IsMatch(file)) continue;
				if (exclude.IsMatch(file)) continue;
				yield return file;
			}
		}
		private static Compilation CreateCompilationFromDirectory(string path, string include, string exclude) {
			var files = GetCompilationSources(path, include, exclude).ToList();
			return CSharpCompilation.Create("CompilationAssembly",
				files.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), new CSharpParseOptions(LanguageVersion.Preview), file)),
				new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
				new CSharpCompilationOptions(OutputKind.ConsoleApplication));
		}
		private static Compilation CreateCompilation(string source)
			=> CSharpCompilation.Create("CompilationAssembly",
				new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview)) },
				new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
				new CSharpCompilationOptions(OutputKind.ConsoleApplication));

		private static GeneratorDriver CreateDriver(Compilation c, params ISourceGenerator[] generators)
			=> CSharpGeneratorDriver.Create(
				generators: generators,
				parseOptions: (CSharpParseOptions)c.SyntaxTrees.First().Options);

		private static Compilation RunGenerators(Compilation c, out ImmutableArray<Diagnostic> diagnostics, params ISourceGenerator[] generators) {
			CreateDriver(c, generators).RunGeneratorsAndUpdateCompilation(c, out var d, out diagnostics);
			return d;
		}
	}
}
