namespace Staticity {
	using System;
	using PHP.Core;

	public class SyntaxHighlighter {
		public string Highlight(string code, string language) {
			ScriptContext context = ScriptContext.CurrentContext;

			// redirect PHP output to the console:
			context.Output = Console.Out; // Unicode text output
			context.OutputStream = Console.OpenStandardOutput(); // byte stream output

			context.Include("geshi.php", true);

			var geshi = (PhpObject)context.NewObject("GeSHi", code, language);
			var result = new PhpCallback(geshi, "parse_code").Invoke();
			var error = new PhpCallback(geshi, "error").Invoke();
			new PhpCallback(geshi, "enable_keyword_links").Invoke(false);

			return result.ToString();
		}
	}
}