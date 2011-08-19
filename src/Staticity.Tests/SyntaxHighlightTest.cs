namespace Staticity.Tests {
	using System;
	using System.IO;
	using NUnit.Framework;
	using PHP.Core;
	using PHP.Library;

	[TestFixture]
	public class SyntaxHighlightTest {
		[Test]
		public void Highlights_csharp_code() {
			var code = "class Foo { int Bar() { } }";

			var highlighter = new SyntaxHighlighter();
			var result = highlighter.Highlight(code, "csharp");
			Console.WriteLine(result);

			StringAssert.StartsWith("<pre class=\"csharp\" style=\"font-family:monospace;\"><span style=\"color: #FF0000;\">class</span>", result);
		}
	}
}