namespace Staticity.Tests {
	using System.IO;
	using NUnit.Framework;
	using Should;
	using System.Linq;

	[TestFixture]
	public class StaticSiteTests {

		[TestFixtureSetUp]
		public void TestFixtureSetup() {
			var directory = "../../TestSite";
			var engine = new StaticityEngine();
			engine.Process(directory);
		}

		[Test]
		public void Renders_simple_page() {
			var postContent = File.ReadAllText("../../TestSite/_site/2011/07/25/simple-post/index.html");
			postContent.ShouldEqual("Test post.");
		}

		[Test]
		public void Renders_page_with_layout() {
			var postContent = File.ReadAllLines("../../TestSite/_site/2011/07/25/with-layout/index.html");
			postContent.ShouldEqual(new[] { "Layout", "Test Post.", "End Layout" });
		}

		[Test]
		public void Renders_page_with_nested_layout() {
			var postContent = File.ReadAllLines("../../TestSite/_site/2011/07/25/with-nested-layout/index.html");
			postContent.ShouldEqual(new[] { "Layout", "Nested", "Test Post.", "End Nested", "End Layout" });
		}

		[Test]
		public void Post_accesses_viewdata() {
			var postContent = File.ReadAllLines("../../TestSite/_site/2011/07/25/simple-post-categories/index.html");
			postContent.ShouldEqual(new[] { "foo", "bar" });
		}

		[Test]
		public void Layout_accesses_viewdata() {
			var postContent = File.ReadAllLines("../../TestSite/_site/2011/07/25/with-layout-and-viewdata/index.html");
			postContent.ShouldEqual(new[] { "Test Post.", "foo", "bar" });
		}

		[Test]
		public void Title_and_Date() {
			var postContent = File.ReadAllLines("../../TestSite/_site/2011/07/25/title-and-date/index.html");
			postContent.ShouldEqual(new[] { "Foo", "Posted on: 25/07/2011 00:00:00" });
		}

		[Test]
		public void MarkdownTests() {
			var postContent = File.ReadAllLines("../../TestSite/_site/2011/07/25/simple-markdown/index.html");
			postContent.ShouldEqual(new[] { "Layout", "<h1>Test 1 2 3</h1>", "", "End Layout" });
		}

		[Test]
		public void SyntaxHighlight() {
			var postContent = File.ReadAllLines("../../TestSite/_site/2011/07/25/syntax/index.html");
			postContent[0].ShouldEqual("<div class=\"syntax\"><div class=\"code\">");
			StringAssert.StartsWith("<pre class=\"csharp\" style=\"font-family:monospace;\"><span style=\"color: #FF0000;\">class</span>", postContent[1]);
			StringAssert.EndsWith("</div></div>", postContent.Last());
		}

		[Test]
		public void SyntaxHighlight_markdown() {
			var postContent = File.ReadAllLines("../../TestSite/_site/2011/07/25/syntaxmd/index.html");
			postContent[0].ShouldEqual("<h1>Test 1 2 3</h1>");
			postContent[2].ShouldEqual("<div class=\"syntax\"><div class=\"code\">");
			StringAssert.StartsWith("<pre class=\"csharp\" style=\"font-family:monospace;\"><span style=\"color: #FF0000;\">class</span>", postContent[3]);
			StringAssert.EndsWith("</div></div>", postContent.Last());
		}

		[Test]
		public void Processes_index_page() {
			var postContent = File.ReadAllLines("../../TestSite/_site/index.html");
			postContent.ShouldEqual(new[] { "Hello bar" });
		}
	}
}