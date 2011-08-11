namespace Staticity {
	using System;
	using System.IO;
	using System.Text.RegularExpressions;

	public class Post {
		const string fileBitsRegex = @"([0-9]{4}-[0-9]{2}-[0-9]{2})-([a-zA-Z0-9-]+)(\.[a-zA-Z]+)";

		private FileInfo _file;

		public DateTime Date { get; set; }
		public string RawName { get; set; }
		public string Extension { get; set; }
		public string Permalink { get; set; }
		

		public static Post GetFromFile(FileInfo file) {
			if (!file.Exists) return null;

			var regex = new Regex(fileBitsRegex);
			var matches = regex.Match(file.Name);

			if(matches.Success) {
				var date = DateTime.Parse(matches.Groups[1].Value);
				var postName = matches.Groups[2].Value;
				var extension = matches.Groups[3].Value;
				var permalink = string.Format("{0:yyyy}/{1:MM}/{2:dd}/{3}", date, date, date, postName);

				return new Post {
					Date = date,
					Extension = extension,
					Permalink = permalink,
					RawName = postName,
					_file = file
				};
			}
			else {
				return null;
			}
		}

		public string Name {
			get { return _file.Name; }
		}

		public string[] GetContent() {
			return File.ReadAllLines(_file.FullName);
		}
	}
}