using RG.CLI;
using System;
using TestApp;

ConsoleApp app = new(exitCommand: "exit");
app["hello"] = args => app.WriteLine("Hello world!");
app["hello {name}"] = args => app.WriteLine($"Hello, {args[0]}!");
app["hello kitty"] = args => throw new NotImplementedException();
app["form"] = args => app.WriteLine(FormInput.ReadForm<Person>().ToString(), ConsoleColor.Yellow, ConsoleColor.Black);
app["cls"] = args => app.Clear();
app.Run();
