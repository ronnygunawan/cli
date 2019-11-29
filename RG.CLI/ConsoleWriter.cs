using System;

namespace RG.CLI {
	public class ConsoleWriter {
		private static ConsoleColor _savedForegroundColor = Console.ForegroundColor;
		private static ConsoleColor _savedBackgroundColor = Console.BackgroundColor;

		internal ConsoleWriter() { }

		internal static void SaveColors() {
			_savedForegroundColor = Console.ForegroundColor;
			_savedBackgroundColor = Console.BackgroundColor;
		}

		internal static void RestoreColors() {
			Console.ForegroundColor = _savedForegroundColor;
			Console.BackgroundColor = _savedBackgroundColor;
		}

		public ConsoleWriter Write(string text, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null) {
			SaveColors();
			if (foregroundColor.HasValue) {
				Console.ForegroundColor = foregroundColor.Value;
				Console.BackgroundColor = backgroundColor ?? ConsoleColor.Black;
			} else {
				if (backgroundColor.HasValue) Console.BackgroundColor = backgroundColor.Value;
			}
			Console.Write(text);
			RestoreColors();
			return this;
		}

		public ConsoleWriter WriteLine(string text, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null) {
			SaveColors();
			if (foregroundColor.HasValue) {
				Console.ForegroundColor = foregroundColor.Value;
				Console.BackgroundColor = backgroundColor ?? ConsoleColor.Black;
			} else {
				if (backgroundColor.HasValue) Console.BackgroundColor = backgroundColor.Value;
			}
			Console.WriteLine(text);
			RestoreColors();
			return this;
		}

		public ConsoleWriter WriteLine() {
			Console.WriteLine();
			return this;
		}

		public ConsoleWriter Clear(ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null) {
			SaveColors();
			if (foregroundColor.HasValue) {
				Console.ForegroundColor = foregroundColor.Value;
				Console.BackgroundColor = backgroundColor ?? ConsoleColor.Black;
			} else {
				if (backgroundColor.HasValue) Console.BackgroundColor = backgroundColor.Value;
			}
			Console.Clear();
			RestoreColors();
			return this;
		}
	}
}
