using RG.CLI;
using System;

namespace TestApp {
	class Program {
		static void Main(string[] args) {
			ConsoleApp app = new ConsoleApp(exitCommand: "exit");
			app["hello"] = args => Console.WriteLine("Hello world!");
			app["hello {name}"] = args => Console.WriteLine($"Hello, {args[0]}!");
			app["hello kitty"] = args => throw new NotImplementedException();
			app["form"] = args => Console.WriteLine(FormInput.ReadForm<Person>());
			app.Run();
		}
	}

	class Person {
		public string Name { get; set; }

		[FormField("Gender (M/F)")]
		public string Gender { get; set; }

		public override string ToString() => $"{Name} ({Gender})";
	}
}
