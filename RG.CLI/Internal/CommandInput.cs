using System;
using System.Collections.Generic;
using System.Linq;

namespace RG.CLI.Internal {
	internal static class CommandInput {
		internal static readonly List<string> _commandHistory = new List<string>();
		internal static int _commandHistoryPos;

		internal static string ReadLine(ICollection<Command> commands) {
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			string line = "";
			string? lookupKeyword = null;
			WriteColorizedLine(line, commands, 0);
			for (; ; ) {
				ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
				switch (keyInfo.Key) {
					case ConsoleKey.Enter:
						if (line.Length == 0) break;
						WriteColorizedLine(line, commands, 0);
						Console.WriteLine();
						if (_commandHistoryPos < _commandHistory.Count - 1) {
							_commandHistory.Insert(_commandHistoryPos + 1, line);
						} else {
							_commandHistory.Add(line);
						}
						_commandHistoryPos++;
						return line;
					case ConsoleKey.Backspace: {
							if (line.Length > 0 && Console.CursorLeft > 2) {
								if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0) {
									int pos = Console.CursorLeft - 2;
									int lastIndex = pos > 1 ? line.LastIndexOf(' ', pos - 2) : -1;
									if (lastIndex >= 0) {
										int removed = pos - lastIndex;
										line = line[..(lastIndex + 1)] + line[pos..];
										WriteColorizedLine(line, commands, removed, lastIndex + 3);
									} else {
										int removed = pos - lastIndex;
										line = line[pos..];
										WriteColorizedLine(line, commands, removed, 2);
									}
								} else {
									int pos = Console.CursorLeft - 2;
									line = line[..(pos - 1)] + line[pos..];
									WriteColorizedLine(line, commands, 1, pos + 1);
								}
							}
							break;
						}
					case ConsoleKey.Delete: {
							if (line.Length > 0 && Console.CursorLeft - 2 < line.Length) {
								if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0) {
									int pos = Console.CursorLeft - 2;
									int index = pos < line.Length - 1 ? line.IndexOf(' ', pos + 1) : -1;
									if (index >= 0) {
										int removed = index - pos;
										line = line[..pos] + line[index..];
										WriteColorizedLine(line, commands, removed, pos + 2);
									} else {
										int removed = line.Length - pos;
										line = line[..pos];
										WriteColorizedLine(line, commands, removed, 2);
									}
								} else {
									int pos = Console.CursorLeft - 2;
									line = line[..pos] + line[(pos + 1)..];
									WriteColorizedLine(line, commands, 1, pos + 2);
								}
							}
							break;
						}
					case ConsoleKey.LeftArrow:
						if (Console.CursorLeft > 2) {
							if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0) {
								int pos = Console.CursorLeft - 2;
								int lastIndex = pos > 1 ? line.LastIndexOf(' ', pos - 2) : -1;
								if (lastIndex >= 0) {
									Console.CursorLeft = lastIndex + 3;
								} else {
									Console.CursorLeft = 2;
								}
							} else {
								Console.CursorLeft--;
							}
						}
						break;
					case ConsoleKey.RightArrow:
						if (Console.CursorLeft < line.Length + 2) {
							if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0) {
								int pos = Console.CursorLeft - 2;
								int index = pos < line.Length - 1 ? line.IndexOf(' ', pos + 1) : -1;
								if (index >= 0) {
									Console.CursorLeft = index + 3;
								} else {
									Console.CursorLeft = line.Length + 2;
								}
							} else {
								Console.CursorLeft++;
							}
						}
						break;
					case ConsoleKey.Home:
						Console.CursorLeft = 2;
						break;
					case ConsoleKey.End:
						Console.CursorLeft = line.Length + 2;
						break;
					case ConsoleKey.UpArrow:
						if (_commandHistoryPos > 0) {
							_commandHistoryPos--;
						}
						if (_commandHistoryPos < _commandHistory.Count) {
							line = _commandHistory[_commandHistoryPos];
							WriteColorizedLine(line, commands, Console.BufferWidth - line.Length - 3, line.Length + 2);
						}
						break;
					case ConsoleKey.DownArrow:
						if (_commandHistoryPos < _commandHistory.Count) {
							_commandHistoryPos++;
						}
						if (_commandHistoryPos < _commandHistory.Count) {
							line = _commandHistory[_commandHistoryPos];
						} else {
							line = "";
						}
						WriteColorizedLine(line, commands, Console.BufferWidth - line.Length - 3, line.Length + 2);
						break;
					case ConsoleKey.Tab:
						string? completion = AutoCompleteLine(line, commands, out List<Command>? matchingCommands);
						if (completion != null) {
							line = completion;
							WriteColorizedLine(line, commands, 0);
						} else if (matchingCommands != null && lookupKeyword != line) {
							Console.WriteLine();
							WriteCommandTable(matchingCommands);
							WriteColorizedLine(line, commands, 0);
							lookupKeyword = line;
						}
						break;
					case ConsoleKey.Clear:
					case ConsoleKey.Pause:
					case ConsoleKey.Escape:
					case ConsoleKey.PageUp:
					case ConsoleKey.PageDown:
					case ConsoleKey.Select:
					case ConsoleKey.Print:
					case ConsoleKey.Execute:
					case ConsoleKey.PrintScreen:
					case ConsoleKey.Insert:
					case ConsoleKey.Help:
					case ConsoleKey.LeftWindows:
					case ConsoleKey.RightWindows:
					case ConsoleKey.Applications:
					case ConsoleKey.Sleep:
					case ConsoleKey.F1:
					case ConsoleKey.F2:
					case ConsoleKey.F3:
					case ConsoleKey.F4:
					case ConsoleKey.F5:
					case ConsoleKey.F6:
					case ConsoleKey.F7:
					case ConsoleKey.F8:
					case ConsoleKey.F9:
					case ConsoleKey.F10:
					case ConsoleKey.F11:
					case ConsoleKey.F12:
					case ConsoleKey.F13:
					case ConsoleKey.F14:
					case ConsoleKey.F15:
					case ConsoleKey.F16:
					case ConsoleKey.F17:
					case ConsoleKey.F18:
					case ConsoleKey.F19:
					case ConsoleKey.F20:
					case ConsoleKey.F21:
					case ConsoleKey.F22:
					case ConsoleKey.F23:
					case ConsoleKey.F24:
					case ConsoleKey.BrowserBack:
					case ConsoleKey.BrowserForward:
					case ConsoleKey.BrowserRefresh:
					case ConsoleKey.BrowserStop:
					case ConsoleKey.BrowserSearch:
					case ConsoleKey.BrowserFavorites:
					case ConsoleKey.BrowserHome:
					case ConsoleKey.VolumeMute:
					case ConsoleKey.VolumeDown:
					case ConsoleKey.VolumeUp:
					case ConsoleKey.MediaNext:
					case ConsoleKey.MediaPrevious:
					case ConsoleKey.MediaStop:
					case ConsoleKey.MediaPlay:
					case ConsoleKey.LaunchMail:
					case ConsoleKey.LaunchMediaSelect:
					case ConsoleKey.LaunchApp1:
					case ConsoleKey.LaunchApp2:
					case ConsoleKey.Process:
					case ConsoleKey.Packet:
					case ConsoleKey.Attention:
					case ConsoleKey.CrSel:
					case ConsoleKey.ExSel:
					case ConsoleKey.EraseEndOfFile:
					case ConsoleKey.Play:
					case ConsoleKey.Zoom:
					case ConsoleKey.NoName:
					case ConsoleKey.Pa1:
					case ConsoleKey.OemClear:
						break;
					default:
						if (line.Length < Console.BufferWidth - 4) {
							int pos = Console.CursorLeft - 2;
							if (pos >= line.Length) {
								line += keyInfo.KeyChar;
								WriteColorizedLine(line, commands, 0);
							} else {
								line = line[..pos] + keyInfo.KeyChar + line[pos..];
								WriteColorizedLine(line, commands, 0, pos + 3);
							}
						}
						break;
				}
			}
		}

		private static void WriteColorizedLine(string line, ICollection<Command> commands, int trailingSpaces, int? cursorLeft = null) {
			Console.CursorVisible = false;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("\r> ");
			Command? matchingCommand = commands
				.Where(command => command.Keywords.StartsWith(line, StringComparison.InvariantCultureIgnoreCase)
					|| (line.StartsWith(command.Keywords + " ", StringComparison.InvariantCultureIgnoreCase)
						&& line[command.Keywords.Length..].TrimStart(' ').SplitArgs().Count() <= command.Parameters.Count)
				)
				.OrderByDescending(command => command.Keywords.Length)
				.FirstOrDefault();
			if (matchingCommand is null) {
				Console.Write(line);
			} else {
				int matchLength = Math.Min(matchingCommand.Keywords.Length, line.Length);
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write(line[..matchLength]);
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(line[matchLength..]);
				Console.ForegroundColor = ConsoleColor.Gray;
			}
			Console.Write(new string(' ', trailingSpaces) + new string('\b', trailingSpaces));
			if (cursorLeft.HasValue) {
				Console.CursorLeft = cursorLeft.Value;
			}
			Console.CursorVisible = true;
		}

		private static void WriteCommandTable(List<Command> commands, int maxWidth = 60) {
			Console.CursorVisible = false;
			int maxLength = commands.Max(command => command.Keywords.Length + command.Parameters.Sum(p => p.Length + 1));
			int columnWidth = maxLength + 3;
			int columns = Math.Max(maxWidth / columnWidth, 1);
			for (int i = 0, col = 0; i < commands.Count; i++) {
				Command command = commands[i];
				int length = command.Keywords.Length + command.Parameters.Sum(p => p.Length + 1);
				WriteColorizedCommand(command);
				Console.Write(new string(' ', columnWidth - length));
				col++;
				if (col >= columns && i < commands.Count - 1) {
					if (i < commands.Count - 1) {
						Console.WriteLine();
					}
					col = 0;
				}
			}
			Console.WriteLine();
			Console.WriteLine();
		}

		private static void WriteColorizedCommand(Command command) {
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.Write(command.Keywords);
			if (command.Parameters.Count > 0) {
				Console.ForegroundColor = ConsoleColor.DarkGray;
				foreach (string parameter in command.Parameters) {
					Console.Write($" {parameter}");
				}
			}
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		private static string? AutoCompleteLine(string line, ICollection<Command> commands, out List<Command>? matchingCommands) {
			matchingCommands = commands.Where(command => command.Keywords.StartsWith(line, StringComparison.InvariantCultureIgnoreCase)).ToList();
			if (matchingCommands.Count == 1) {
				string keywords = matchingCommands[0].Keywords;
				matchingCommands = null;
				return keywords;
			} else if (matchingCommands.Count > 1) {
				List<string> possibleCompletions = matchingCommands
					.Select(command => {
						if (command.Keywords.Length >= line.Length + 1) {
							int spacePos = command.Keywords.IndexOf(' ', line.Length + 1);
							if (spacePos >= 0) {
								return command.Keywords[..spacePos];
							} else {
								return command.Keywords;
							}
						} else {
							return command.Keywords;
						}
					})
					.Distinct(StringComparer.InvariantCultureIgnoreCase)
					.ToList();
				if (possibleCompletions.Count == 1) {
					matchingCommands = null;
					return possibleCompletions.Single();
				}
				return null;
			} else {
				matchingCommands = null;
				return null;
			}
		}
	}
}
