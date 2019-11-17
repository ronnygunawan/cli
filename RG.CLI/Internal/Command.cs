using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RG.CLI.Internal {
	internal class Command {
		internal string Keywords { get; }
		internal ImmutableList<string> Parameters { get; }

		internal Command(string keywords, ImmutableList<string> parameters) {
			Keywords = keywords;
			Parameters = parameters;
		}

		internal static Command Parse(string commandString) {
			string[] words = commandString.Split(' ');
			if (words.Any(word => word.Length == 0)) throw new ArgumentException("Command string cannot contain redundant spaces.", nameof(commandString));
			if (words.Length == 0) throw new ArgumentException("Command string cannot be empty.", nameof(commandString));
			if (!IsCommandWord(words[0])) throw new ArgumentException($"Invalid command: {words[0]}", nameof(commandString));
			List<string> keywords = new List<string>();
			int i = 0;
			do {
				keywords.Add(words[i++]);
			} while (i < words.Length && IsCommandWord(words[i]));
			List<string> parameterWords = new List<string>();
			while(i < words.Length && IsArgumentWord(words[i])) {
				parameterWords.Add(words[i++]);
			}
			if (i != words.Length) throw new ArgumentException($"Invalid parameter: {words[i]}", nameof(commandString));
			return new Command(string.Join(' ', keywords), parameterWords.ToImmutableList());
		}

		private static bool IsCommandWord(string word) => word.Length > 0 && char.IsLetter(word[0]) && word.All(c => char.IsLetterOrDigit(c));
		private static bool IsArgumentWord(string word) => word.Length > 0 && word.StartsWith("{") && word.EndsWith("}") && IsCommandWord(word[1..^1]);

		public override bool Equals(object obj) => obj is Command command && Keywords == command.Keywords && EqualityComparer<ImmutableList<string>>.Default.Equals(Parameters, command.Parameters);
		public override int GetHashCode() => HashCode.Combine(Keywords, Parameters);

		public static bool operator ==(Command left, Command right) => EqualityComparer<Command>.Default.Equals(left, right);
		public static bool operator !=(Command left, Command right) => !(left == right);
	}
}
