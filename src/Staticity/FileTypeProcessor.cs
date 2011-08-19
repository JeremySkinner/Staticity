namespace Staticity {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using MarkdownSharp;

	public class FileTypeProcessor {
		readonly string basePath;
		readonly JsonHeaderReader jsonHeader = new JsonHeaderReader();
		NVelocityTemplateEngine engine;

		public FileTypeProcessor(string basePath) {
			this.basePath = basePath;
			engine = new NVelocityTemplateEngine(basePath);
			engine.Initialize();
		}

		public void WritePost(Post post, string fileName) {
			var content = post.GetContent();
			string output = RecursiveApplyTemplate(content, new Dictionary<string, object> {
			                                                                               	{ "date", post.Date }
			                                                                               });
			File.WriteAllText(fileName, output);
		}

		string RecursiveApplyTemplate(string[] content, Dictionary<string, object> viewData) {
			var header = jsonHeader.StripJsonHeader(ref content);
			var layout = header.TryGetValueAndRemove<string>("layout");

			foreach(var pair in header) {
				if(!viewData.ContainsKey(pair.Key)) {
					viewData.Add(pair.Key, pair.Value);					
				}
			}

			string bodyContent = RenderTemplate("foo", string.Join(Environment.NewLine, content), viewData);
			viewData["content"] = bodyContent;

			if(layout != null) {
				viewData["isLayout"] = true;
				string[] layoutContent = File.ReadAllLines(Path.Combine(basePath, "_layouts", layout + ".html"));
				bodyContent = RecursiveApplyTemplate(layoutContent, viewData);
			}

			return bodyContent;
		}


		protected virtual string RenderTemplate(string path, string content, Dictionary<string, object> viewData) {
			string output;
			using (var writer = new StringWriter()) {
				var context = new Hashtable();
				if(viewData != null) {
					foreach(var pair in viewData) {
						context.Add(pair.Key, pair.Value);
					}
				}
				engine.Process(context, path, writer, content);
				output = writer.ToString();
			}
			return output;
		}
	}

	public class MarkdownProcessor : FileTypeProcessor {
		public MarkdownProcessor(string basePath) : base(basePath) {
		}

		protected override string RenderTemplate(string path, string content, Dictionary<string, object> viewData) {
			if (!viewData.ContainsKey("isLayout")) {
				content = base.RenderTemplate(path, content, viewData);
				content = new Markdown().Transform(content);
			}
			return base.RenderTemplate(path, content, viewData);
		}

	}


}