using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RG.CLI {
	public static class FormInput {
		internal static readonly Dictionary<string, List<string>> _commandHistoryByKey = new Dictionary<string, List<string>>();
		internal static readonly Dictionary<string, int> _commandHistoryPosByKey = new Dictionary<string, int>();

		public static TForm ReadForm<TForm>() where TForm : class {
			try {
				ConsoleWriter.RestoreColors();
				PropertyInfo[] formProperties = typeof(TForm).GetProperties();
				if (formProperties.Any(prop => prop.PropertyType != typeof(string) || !prop.CanWrite)) {
					throw new InvalidOperationException("All properties should be string and writable.");
				}

				int maxPropertyLength = formProperties.Max(prop => prop.GetCustomAttribute<FormFieldAttribute>()?.DisplayName?.Length ?? prop.Name.Length);
				int cursorOffset = maxPropertyLength + 4;
				TForm formData = Activator.CreateInstance<TForm>();

				foreach (PropertyInfo property in formProperties) {
					FormFieldAttribute? attribute = property.GetCustomAttribute<FormFieldAttribute>();
					string displayName = attribute?.DisplayName ?? property.Name;
					bool nullIfEmpty = attribute?.NullIfEmpty ?? false;
					string key = $"{typeof(TForm).FullName}.{property.Name}";
					string line = "";
					if (!_commandHistoryByKey.ContainsKey(key)) {
						_commandHistoryByKey[key] = new List<string>();
						_commandHistoryPosByKey[key] = 0;
					}

					WriteFieldInput(displayName, line, maxPropertyLength);
					for (bool breakFor = false; !breakFor;) {
						ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
						switch (keyInfo.Key) {
							case ConsoleKey.Enter:
								WriteFieldInput(displayName, line, maxPropertyLength, 0);
								Console.WriteLine();
								if (line.Length > 0) {
									if (_commandHistoryPosByKey[key] < _commandHistoryByKey[key].Count - 1) {
										_commandHistoryByKey[key].Insert(_commandHistoryPosByKey[key] + 1, line);
									} else {
										_commandHistoryByKey[key].Add(line);
									}
									_commandHistoryPosByKey[key]++;
								}
								breakFor = true;
								break;
							case ConsoleKey.Backspace: {
									if (line.Length > 0 && Console.CursorLeft > cursorOffset) {
										if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0) {
											int pos = Console.CursorLeft - cursorOffset;
											int lastIndex = pos > 1 ? line.LastIndexOf(' ', pos - 2) : -1;
											if (lastIndex >= 0) {
												int removed = pos - lastIndex;
												line = line[..(lastIndex + 1)] + line[pos..];
												WriteFieldInput(displayName, line, maxPropertyLength, lastIndex + 1);
											} else {
												int removed = pos - lastIndex;
												line = line[pos..];
												WriteFieldInput(displayName, line, maxPropertyLength, 0);
											}
										} else {
											int pos = Console.CursorLeft - cursorOffset;
											line = line[..(pos - 1)] + line[pos..];
											WriteFieldInput(displayName, line, maxPropertyLength, pos - 1);
										}
									}
									break;
								}
							case ConsoleKey.Delete: {
									if (line.Length > 0 && Console.CursorLeft - cursorOffset < line.Length) {
										if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0) {
											int pos = Console.CursorLeft - cursorOffset;
											int index = pos < line.Length - 1 ? line.IndexOf(' ', pos + 1) : -1;
											if (index >= 0) {
												int removed = index - pos;
												line = line[..pos] + line[index..];
												WriteFieldInput(displayName, line, maxPropertyLength, pos);
											} else {
												int removed = line.Length - pos;
												line = line[..pos];
												WriteFieldInput(displayName, line, maxPropertyLength, 0);
											}
										} else {
											int pos = Console.CursorLeft - cursorOffset;
											line = line[..pos] + line[(pos + 1)..];
											WriteFieldInput(displayName, line, maxPropertyLength, pos);
										}
									}
									break;
								}
							case ConsoleKey.LeftArrow:
								if (Console.CursorLeft > cursorOffset) {
									if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0) {
										int pos = Console.CursorLeft - cursorOffset;
										int lastIndex = pos > 1 ? line.LastIndexOf(' ', pos - 2) : -1;
										if (lastIndex >= 0) {
											Console.CursorLeft = lastIndex + cursorOffset + 1;
										} else {
											Console.CursorLeft = cursorOffset;
										}
									} else {
										Console.CursorLeft--;
									}
								}
								break;
							case ConsoleKey.RightArrow:
								if (Console.CursorLeft < line.Length + cursorOffset) {
									if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0) {
										int pos = Console.CursorLeft - cursorOffset;
										int index = pos < line.Length - 1 ? line.IndexOf(' ', pos + 1) : -1;
										if (index >= 0) {
											Console.CursorLeft = index + cursorOffset + 1;
										} else {
											Console.CursorLeft = line.Length + cursorOffset;
										}
									} else {
										Console.CursorLeft++;
									}
								}
								break;
							case ConsoleKey.Home:
								Console.CursorLeft = cursorOffset;
								break;
							case ConsoleKey.End:
								Console.CursorLeft = line.Length + cursorOffset;
								break;
							case ConsoleKey.UpArrow:
								if (_commandHistoryPosByKey[key] > 0) {
									_commandHistoryPosByKey[key]--;
								}
								if (_commandHistoryPosByKey[key] < _commandHistoryByKey[key].Count) {
									line = _commandHistoryByKey[key][_commandHistoryPosByKey[key]];
									WriteFieldInput(displayName, line, maxPropertyLength, line.Length);
								}
								break;
							case ConsoleKey.DownArrow:
								if (_commandHistoryPosByKey[key] < _commandHistoryByKey[key].Count) {
									_commandHistoryPosByKey[key]++;
								}
								if (_commandHistoryPosByKey[key] < _commandHistoryByKey[key].Count) {
									line = _commandHistoryByKey[key][_commandHistoryPosByKey[key]];
								} else {
									line = "";
								}
								WriteFieldInput(displayName, line, maxPropertyLength, line.Length);
								break;
							case ConsoleKey.Tab:
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
								if (line.Length < Console.BufferWidth - cursorOffset - 2) {
									int pos = Console.CursorLeft - cursorOffset;
									if (pos >= line.Length) {
										line += keyInfo.KeyChar;
										WriteFieldInput(displayName, line, maxPropertyLength);
									} else {
										line = line[..pos] + keyInfo.KeyChar + line[pos..];
										WriteFieldInput(displayName, line, maxPropertyLength, pos + 1);
									}
								}
								break;
						}
					}

					if (nullIfEmpty && line.Length == 0) {
						property.SetValue(formData, null);
					} else {
						property.SetValue(formData, line);
					}
				}
				return formData;
			} finally {
				ConsoleWriter.RestoreColors();
			}
		}

		private static void WriteFieldInput(string displayName, string line, int maxPropertyLength, int? cursorLeft = null) {
			Console.CursorVisible = false;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write($"\r{displayName}{new string(' ', maxPropertyLength - displayName.Length)} : ");
			Console.BackgroundColor = ConsoleColor.DarkCyan;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write($" {line} \b");
			int tempCursorLeft = Console.CursorLeft;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.CursorLeft++;
			Console.Write(new string(' ', Console.BufferWidth - Console.CursorLeft));
			if (cursorLeft.HasValue) {
				Console.CursorLeft = cursorLeft.Value + maxPropertyLength + 4;
			} else {
				Console.CursorLeft = tempCursorLeft;
			}
			Console.CursorVisible = true;
		}
	}
}
