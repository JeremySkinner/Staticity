namespace Staticity.Tests {
	using System;
	using System.IO;
	using NUnit.Framework;
	using Should;

	[TestFixture]
	public class PostTests {
		[Test]
		public void Builds_post_for_file() {
			var post = Post.GetFromFile(new FileInfo("../../TestSite/_posts/2011-07-25-simple-post.html"));
			post.Extension.ShouldEqual(".html");
			post.Date.ShouldEqual(new DateTime(2011, 7, 25));
			post.RawName.ShouldEqual("simple-post");
			post.Permalink.ShouldEqual("2011/07/25/simple-post");
		}
	}
}