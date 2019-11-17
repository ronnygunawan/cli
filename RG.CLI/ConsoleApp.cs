using RG.CLI.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RG.CLI {
	public class ConsoleApp {
		private readonly Dictionary<Command, Action<string[]>> _actionByCommand;
		private readonly string _exitCommand;

		public ConsoleApp(string exitCommand) {
			_actionByCommand = new Dictionary<Command, Action<string[]>> {
				{ Command.Parse(exitCommand), args => { } }
			};
			_exitCommand = exitCommand;
		}

		public Action<string[]> this[string command] {
			get => _actionByCommand[Command.Parse(command)];
			set => _actionByCommand[Command.Parse(command)] = value;
		}

		public void Run() {
			for (; ; ) {
				string line = Input.ReadLine(_actionByCommand.Keys);
				if (line == _exitCommand) {
					return;
				} else if (TryGetAction(line, out Action<string[]>? action, out string[]? args)) {
					try {
						action.Invoke(args);
					} catch (Exception exc) {
						Console.BackgroundColor = ConsoleColor.Red;
						Console.ForegroundColor = ConsoleColor.White;
						Console.WriteLine(exc.Message);
					}
				} else {
					Console.BackgroundColor = ConsoleColor.Red;
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine($"'{line}' is not a recognized command.");
				}
			}
		}

		private bool TryGetAction(
			string line,
			[NotNullWhen(true)]out Action<string[]>? action,
			[NotNullWhen(true)]out string[]? args) {
			string[] words = line.SplitArgs().ToArray();
			foreach((Command c, Action<string[]> a) in _actionByCommand.OrderByDescending(kvp => kvp.Key.Keywords.Length)) {
				int keywordCount = c.Keywords.Count(c => c == ' ') + 1;
				if (words.Length == c.Parameters.Count + keywordCount
					&& c.Keywords.Equals(string.Join(' ', words.Take(keywordCount)), StringComparison.InvariantCultureIgnoreCase)) {
					action = a;
					args = words.Skip(keywordCount).ToArray();
					return true;
				}
			}
			action = null;
			args = null;
			return false;
		}
	}
}
