namespace Staticity {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using NVelocity.Context;
	using NVelocity.Runtime.Directive;
	using NVelocity.Runtime.Parser.Node;

	public class CodeDirective : Directive {
		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node) {

			if(node.ChildrenCount == 2) {
				string lang = node.GetChild(0).Value(context) as string;
				var codeWriter = new StringWriter();
				node.GetChild(1).Render(context, codeWriter);
				var code = codeWriter.ToString();

				if(lang != null && !string.IsNullOrEmpty(code)) {
					var highlighter = new SyntaxHighlighter();
					writer.WriteLine("<div class=\"syntax\"><div class=\"code\">");
					writer.Write(highlighter.Highlight(code, lang));
					writer.WriteLine("</div></div>");
				}
			}
	
			return true;
		}

		public override string Name {
			get { return "code"; }
			set { }
		}

		public override DirectiveType Type {
			get { return DirectiveType.BLOCK; }
		}

		public override void Init(NVelocity.Runtime.IRuntimeServices rs, IInternalContextAdapter context, INode node) {
			base.Init(rs, context, node);
		}
	}

	public class MyDirectiveManager: DirectiveManager {
		Dictionary<string, Func<Directive>> directives = new Dictionary<string, Func<Directive>> {
		   { "code", () => new CodeDirective() }                                                                                       	
		};

		public override bool Contains(string name) {
			return directives.ContainsKey(name);
		}

		public override Directive Create(string name, System.Collections.Stack directiveStack) {
			Func<Directive> directiveFactory = directives.TryGetValueOrDefault(name);
			if(directiveFactory == null) {
				return base.Create(name, directiveStack);
			}

			return directiveFactory();

		}
	}
}