﻿namespace Staticity {
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
				{ ".html", new FileTypeProcessor(basePath) },
				{ ".md", new MarkdownProcessor(basePath) }
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
				Console.WriteLine("Processing " + file.post.Permalink + "...");
			
				var permalinkDirectory = Path.Combine(destinationPath, file.post.Permalink);
				Directory.CreateDirectory(permalinkDirectory);
				var fileName = Path.Combine(permalinkDirectory, "index.html");

				file.processor.WritePost(file.post, fileName);
			
			}

			var index = Path.Combine(basePath, "index.html");
			if(File.Exists(index)) {
				Console.WriteLine("Processing index.html...");
				var indexPage = Post.GetFromFile(new FileInfo(index));
				var outputPath = Path.Combine(destinationPath, "index.html");
				_fileTypes[".html"].WritePost(indexPage, outputPath);
			}

			var contentPath = new DirectoryInfo(Path.Combine(basePath, "_content"));
			if(contentPath.Exists) {
				IOHelper.CopyFilesRecursively(contentPath, new DirectoryInfo(Path.Combine(destinationPath, "content")));
			}

			// Do the same for wp_content too, because I still have old format wordpress posts.

			contentPath = new DirectoryInfo(Path.Combine(basePath, "wp_content"));
			if (contentPath.Exists) {
				IOHelper.CopyFilesRecursively(contentPath, new DirectoryInfo(Path.Combine(destinationPath, "content")));
			}
		}
	}
}