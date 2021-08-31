using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

namespace CodeGenerator
{
	public enum DiagnosticCategory
	{
		None,
		Hidden,
		Info,
		Warning,
		Error,
	}
	public static class Diagnostics
	{
		private const string DefaultHelpLinkFormat = "https://aka.ericjonker.com/{0}";
		private static DiagnosticDescriptor Build(string id,
			DiagnosticCategory category = DiagnosticCategory.Warning,
			DiagnosticSeverity severity = DiagnosticSeverity.Warning,
			string format = "{0}",
			string title = null, string description = null, string helpLinkUri = null, 
			bool isEnabledByDefault = true,
			params string[] tags)
			=> new(id, title ?? id, format, category.ToString(),
				severity, isEnabledByDefault, description ?? id,
				helpLinkUri ?? string.Format(DefaultHelpLinkFormat, id), tags);

		private static readonly DiagnosticDescriptor ERR0000 = Build(
			nameof(ERR0000), DiagnosticCategory.Hidden, DiagnosticSeverity.Hidden);
		private static readonly DiagnosticDescriptor ERR1000 = Build(
			nameof(ERR1000), DiagnosticCategory.Info, DiagnosticSeverity.Info);
		private static readonly DiagnosticDescriptor ERR2000 = Build(
			nameof(ERR2000), DiagnosticCategory.Warning, DiagnosticSeverity.Warning);
		private static readonly DiagnosticDescriptor ERR4000 = Build(
			nameof(ERR4000), DiagnosticCategory.Error, DiagnosticSeverity.Error);
		public static void Log(in this GeneratorExecutionContext context, string message, Location location = null, DiagnosticDescriptor descriptor = null)
			=> context.ReportDiagnostic(Diagnostic.Create(descriptor ?? ERR0000, location ?? Location.None, message));
	}
}
