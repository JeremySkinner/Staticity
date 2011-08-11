namespace Staticity {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public class JsonHeaderReader {
		public Dictionary<string, object> StripJsonHeader(ref string[] lines) {
			if(lines[0] == "###") {
				int lastLineOfHeader = Array.FindLastIndex(lines, x => x == "###");

				var json = "{" + string.Join(", ", lines.Skip(1).TakeWhile(x => x != "###")) + "}";
				// Remove double commas - might have put a comma on the line ending
				json = json.Replace(",,", ",");

				var newLines = new string[lines.Length - lastLineOfHeader - 1];


				Array.Copy(lines, lastLineOfHeader + 1, newLines, 0, lines.Length - lastLineOfHeader - 1);
				lines = newLines;
				dynamic d = JsonConvert.DeserializeObject(json);

				var result = new Dictionary<string, object>();

				foreach(JProperty pair in d) {
					pair.Value.WhenItIs<JValue>(v => result.Add(pair.Name, v.Value));
					pair.Value.WhenItIs<JArray>(v => {
						result.Add(pair.Name, v.OfType<JValue>().Cast<object>().ToArray());
					});
				}

				return result;
			}


			return new Dictionary<string, object>();
		}

	}
}