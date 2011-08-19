using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Staticity {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Staticity (c) Jeremy Skinner 2011");

			var engine = new StaticityEngine();
			engine.Process(Environment.CurrentDirectory);

			Console.WriteLine("All done.");
		}
	}
}
