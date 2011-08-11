namespace Staticity.Tests {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using Newtonsoft.Json.Linq;
	using Should;
	using System.Linq;

	[TestFixture]
	public class JsonHeaderReaderTests {
		string content;

		[SetUp]
		public void Setup() {
			content = @"###
title: ""foo""
something: ""bar""
categories: [1, 2, 3]
###
abc";

		}

		[Test]
		public void Strips_header() {
			var reader = new JsonHeaderReader();
			var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			reader.StripJsonHeader(ref lines);
			lines.Length.ShouldEqual(1);
			lines[0].ShouldEqual("abc");

		}

		[Test]
		public void Builds_json_object() {
			var reader = new JsonHeaderReader();

			var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			var jsonHeader = reader.StripJsonHeader(ref lines);

			string title = (string) jsonHeader["title"];
			string something = (string) jsonHeader["something"];
			int numCategories = ((object[])jsonHeader["categories"]).Length;

			title.ShouldEqual("foo");
			something.ShouldEqual("bar");
			numCategories.ShouldEqual(3);
		}

	}
}