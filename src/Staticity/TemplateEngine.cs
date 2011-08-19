namespace Staticity {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using Commons.Collections;
	using NVelocity;
	using NVelocity.App;
	using NVelocity.Context;
	using NVelocity.Runtime;

	// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
	// 
	// Licensed under the Apache License, Version 2.0 (the "License");
	// you may not use this file except in compliance with the License.
	// You may obtain a copy of the License at
	// 
	//     http://www.apache.org/licenses/LICENSE-2.0
	// 
	// Unless required by applicable law or agreed to in writing, software
	// distributed under the License is distributed on an "AS IS" BASIS,
	// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	// See the License for the specific language governing permissions and
	// limitations under the License.


	// From Castle Project's NVelocity Template Engine. Modified.

	public class NVelocityTemplateEngine {
		VelocityEngine vengine;

		String templateDir = ".";
		bool enableCache = true;

		/// <summary>
		/// Constructs a NVelocityTemplateEngine instance
		/// assuming the default values
		/// </summary>
		public NVelocityTemplateEngine() {
		}

		/// <summary>
		/// Constructs a NVelocityTemplateEngine instance
		/// specifing the template directory
		/// </summary>
		/// <param name="templateDir"></param>
		public NVelocityTemplateEngine(String templateDir) {
			TemplateDir = templateDir;
		}

		/// <summary>
		/// Gets or sets the template directory
		/// </summary>
		public string TemplateDir {
			get { return templateDir; }
			set {
				if (vengine != null) {
					throw new InvalidOperationException("Could not change the TemplateDir after Template Engine initialization.");
				}

				templateDir = value;
			}
		}

		/// <summary>
		/// Enable/Disable caching. Default is <c>true</c>
		/// </summary>
		public bool EnableCache {
			get { return enableCache; }
			set { enableCache = value; }
		}

		/// <summary>
		/// Starts/configure NVelocity based on the properties.
		/// </summary>
		public void Initialize() {
			vengine = new VelocityEngine();

			ExtendedProperties props = new ExtendedProperties();
			String expandedTemplateDir = ExpandTemplateDir(templateDir);

			FileInfo propertiesFile = new FileInfo(Path.Combine(expandedTemplateDir, "nvelocity.properties"));
			if (propertiesFile.Exists) {
				using (Stream stream = propertiesFile.OpenRead()) {
					props.Load(stream);
				}
			}

			props.SetProperty(RuntimeConstants.RESOURCE_LOADER, "file");
			props.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, expandedTemplateDir);
			props.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_CACHE, EnableCache.ToString().ToLower());
			props.SetProperty("directive.manager", "Staticity.MyDirectiveManager; Staticity");

			vengine.Init(props);
		}

		/// <summary>
		/// Returns <c>true</c> only if the 
		/// specified template exists and can be used
		/// </summary>
		/// <param name="templateName"></param>
		/// <returns></returns>
		public bool HasTemplate(String templateName) {
			if (vengine == null)
				throw new InvalidOperationException("Template Engine not yet initialized.");

			try {
				vengine.GetTemplate(templateName);

				return true;
			}
			catch (Exception) {
				return false;
			}
		}

		/// <summary>
		/// Process the template with data from the context.
		/// </summary>
		public bool Process(IDictionary context, String templateName, TextWriter output) {
			if (vengine == null)
				throw new InvalidOperationException("Template Engine not yet initialized.");

			Template template = vengine.GetTemplate(templateName);

			template.Merge(CreateContext(context), output);

			return true;
		}

		/// <summary>
		/// Process the input template with data from the context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="templateName">Name of the template.  Used only for information during logging</param>
		/// <param name="output">The output.</param>
		/// <param name="inputTemplate">The input template.</param>
		/// <returns></returns>
		public bool Process(IDictionary context, string templateName, TextWriter output, string inputTemplate) {
			return Process(context, templateName, output, new StringReader(inputTemplate));
		}

		/// <summary>
		/// Process the input template with data from the context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="templateName">Name of the template.  Used only for information during logging</param>
		/// <param name="output">The output.</param>
		/// <param name="inputTemplate">The input template.</param>
		/// <returns></returns>
		public bool Process(IDictionary context, string templateName, TextWriter output, TextReader inputTemplate) {
			if (vengine == null)
				throw new InvalidOperationException("Template Engine not yet initialized.");

			return vengine.Evaluate(CreateContext(context), output, templateName, inputTemplate);
		}

		static IContext CreateContext(IDictionary context) {
			return new VelocityContext(new Hashtable(context));
		}

		String ExpandTemplateDir(String templateDir) {

			// if nothing to expand, then exit
			if (templateDir == null)
				templateDir = String.Empty;

			// expand web application root
/*
			if (templateDir.StartsWith("~/")) {
				HttpContext webContext = HttpContext.Current;
				if (webContext != null && webContext.Request != null)
					templateDir = webContext.Server.MapPath(templateDir);
			}
*/

			// normalizes the path (including ".." notation, for parent directories)
			templateDir = new DirectoryInfo(templateDir).FullName;

			return templateDir;
		}
	}
}