using System;

namespace RG.CLI {
	[AttributeUsage(AttributeTargets.Property)]
	public class FormFieldAttribute : Attribute {
		public string? DisplayName { get; }
		public bool NullIfEmpty { get; set; }

		public FormFieldAttribute(string? displayName = null) {
			DisplayName = displayName;
		}
	}
}
