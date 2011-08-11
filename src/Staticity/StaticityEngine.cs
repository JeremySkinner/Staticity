namespace Staticity {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	public class StaticityEngine {

		Dictionary<string, FileTypeProcessor> _fileTypes;


		public void Process(string basePath) {

			if (!Directory.Exists(basePath)) {
				throw new Exception(string.Format("Could not find part of the path '{0}'", basePath));
			}

			var postsPath = Path.Combine(basePath, "_posts");

			if (!Directory.Exists(postsPath)) {
				throw new Exception(string.Format("Could not find part of the path '{0}'", postsPath));
			}

			_fileTypes = new Dictionary<string, FileTypeProcessor> {
				{ ".html", new FileTypeProcessor(basePath) }
			};


			var destinationPath = Path.Combine(basePath, "_site");

			if(Directory.Exists(destinationPath)) {
				IOHelper.DeleteDirectory(destinationPath);
			}

			Directory.CreateDirectory(destinationPath);

			var files = from filePath in Directory.GetFiles(postsPath)
						let file = new FileInfo(filePath)
						let post = Post.GetFromFile(file)
						where post != null
						let processor = _fileTypes.TryGetValueOrDefault(post.Extension)
						where processor != null
						select new {
							post, processor
						};


			foreach(var file in files) {
				var permalinkDirectory = Path.Combine(destinationPath, file.post.Permalink);
				Directory.CreateDirectory(permalinkDirectory);
				var fileName = Path.Combine(permalinkDirectory, "index.html");

				file.processor.WritePost(file.post, fileName, basePath, postsPath);
			}
		}
	}
}