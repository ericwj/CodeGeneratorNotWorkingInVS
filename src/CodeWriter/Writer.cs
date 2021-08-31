using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeWriter
{
	using static Unsafe;

	public class Writer
	{
		// Uses Microsoft.Bcl.HashCode
		public static int GetHashCode(string text) => HashCode.Combine(text);
		// Uses System.Memory
		public static string Write(ReadOnlySpan<char> text) => text.ToString();
		// Uses System.Runtime.CompilerServices.Unsafe
		public static char UnsafeGetCharAt(ReadOnlySpan<char> text, int unsafeIndex)
			=> Add(ref AsRef(in text.GetPinnableReference()), unsafeIndex);
	}
}
