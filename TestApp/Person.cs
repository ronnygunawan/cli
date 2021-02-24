using RG.CLI;

namespace TestApp
{
	class Person
	{
		public string Name { get; set; }

		[FormField("Gender (M/F)")]
		public string Gender { get; set; }

		public override string ToString() => $"{Name} ({Gender})";
	}
}
