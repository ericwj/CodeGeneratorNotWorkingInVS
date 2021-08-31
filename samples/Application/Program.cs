using System;

namespace Application
{
	internal class Program
	{
		private static void Main(string[] args) {
			CodeGenerator.Feature.Use();
			Console.WriteLine(CodeGenerator.Feature.Use("Hello World"));
		}
	}
}
