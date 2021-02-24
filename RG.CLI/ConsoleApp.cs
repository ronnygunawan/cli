using Microsoft.Extensions.DependencyInjection;
using RG.CLI.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RG.CLI {
	public partial class ConsoleApp {
		private readonly ConsoleWriter _consoleWriter = new();
		private readonly Dictionary<Command, Action<string[]>> _actionByCommand;
		private readonly string _exitCommand;
		private readonly IServiceCollection _serviceCollection;
		private IServiceProvider? _serviceProvider;
		private bool _applicationExited;

		public ConsoleApp(string exitCommand) {
			if (exitCommand is null) throw new ArgumentNullException(nameof(exitCommand));

			_actionByCommand = new Dictionary<Command, Action<string[]>> {
				{ Command.Parse(exitCommand), _ => { } }
			};
			_exitCommand = exitCommand;
			_serviceCollection = new ServiceCollection();
		}

		public Action<string[]> this[string command] {
			get {
				if (command is null) throw new ArgumentNullException(nameof(command));
				return _actionByCommand[Command.Parse(command)];
			}

			set {
				if (command is null) throw new ArgumentNullException(nameof(command));
				_actionByCommand[Command.Parse(command)] = value;
			}
		}

		public void Run() {
			_serviceProvider = _serviceCollection.BuildServiceProvider();
			for (; ; ) {
				string commandText = CommandInput.ReadLine(_actionByCommand.Keys);
				if (commandText == _exitCommand) {
					return;
				} else if (TryGetAction(commandText, out Action<string[]>? action, out string[]? args)) {
					try {
						using IServiceScope _ = _serviceProvider.CreateScope();
						action.Invoke(args);
						if (_applicationExited) return;
					} catch (Exception exc) {
						_consoleWriter.WriteLine(exc.Message, ConsoleColor.Red, ConsoleColor.White);
					}
				} else {
					_consoleWriter.WriteLine($"'{commandText}' is not a recognized command.", ConsoleColor.Red, ConsoleColor.White);
				}
			}
		}

		public void Exit() {
			_applicationExited = true;
		}

		public ConsoleWriter Write(string text, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null) {
			return _consoleWriter.Write(text, foregroundColor, backgroundColor);
		}

		public ConsoleWriter WriteLine(string text, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null) {
			return _consoleWriter.WriteLine(text, foregroundColor, backgroundColor);
		}

		public ConsoleWriter WriteLine() {
			return _consoleWriter.WriteLine();
		}

		public ConsoleWriter Clear(ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null) {
			return _consoleWriter.Clear(foregroundColor, backgroundColor);
		}

		private bool TryGetAction(
			string commandText,
			[NotNullWhen(true)]out Action<string[]>? action,
			[NotNullWhen(true)]out string[]? args) {
			string[] words = commandText.SplitArgs().ToArray();
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
