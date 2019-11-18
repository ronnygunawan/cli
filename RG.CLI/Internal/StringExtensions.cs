using System;
using System.Collections.Generic;
using System.Linq;

namespace RG.CLI.Internal {
	internal static class StringExtensions {
		internal static IEnumerable<string> Split(this string str, Func<char, bool> controller) {
			int nextPiece = 0;
			for (int c = 0; c < str.Length; c++) {
				if (controller(str[c])) {
					yield return str[nextPiece..c];
					nextPiece = c + 1;
				}
			}
			yield return str.Substring(nextPiece);
		}

		internal static string TrimMatchingQuotes(this string str, char quote) {
			if ((str.Length >= 2) &&
				(str[0] == quote) && (str[^1] == quote)) {
				return str[1..^1];
			}
			return str;
		}

		internal static IEnumerable<string> SplitArgs(this string str) {
			bool inQuotes = false;
			bool isEscaping = false;

			return str.Split(c => {
				if (c == '\\' && !isEscaping) { isEscaping = true; return false; }
				if (c == '\"' && !isEscaping) inQuotes = !inQuotes;
				isEscaping = false;
				return !inQuotes && char.IsWhiteSpace(c);
			})
				.Select(arg => arg.Trim().TrimMatchingQuotes('\"').Replace("\\\"", "\"", StringComparison.InvariantCulture))
				.Where(arg => !string.IsNullOrEmpty(arg));
		}
	}
}
