namespace Tests
{
	[TestClass]
	public class Variables
	{
		const string ID = "AS123-K92341";
		const string FindError = "Could not find the variable";
		const string WrongValueError = "The variable existed, but had the wrong value";

		[TestMethod]
		public void Add()
		{
			var parser = new Parser().AddVariable<string>("A");
			parser.SetValue(ID, "A", "5");
			Assert.IsTrue(parser.TryGetVariable(ID, "A", out var a), FindError);
			Assert.AreEqual("5", a, WrongValueError);
		}

		[TestMethod]
		public void Override()
		{
			var parser = new Parser().AddVariable<string>("A");
			parser.SetValue(ID, "A", "5");
			Assert.IsTrue(parser.TryGetVariable(ID, "A", out var a), FindError);
			Assert.AreEqual("5", a, WrongValueError);

			parser.SetValue(ID, "A", "9");
			Assert.IsTrue(parser.TryGetVariable(ID, "A", out a), FindError);
			Assert.AreEqual("9", a, WrongValueError);
		}

		[TestMethod]
		public void On_value_changed_callbacks()
		{
			var numberOfCallbacks = 0;
			var parser = new Parser().AddVariable<string>("A", x => numberOfCallbacks++);
			parser.SetValue(ID, "A", "5");
			parser.SetValue(ID, "A", "5");
			parser.SetValue(ID, "A", "5");
			parser.SetValue(ID, "A", "5");
			parser.SetValue(ID, "A", "9");
			parser.SetValue(ID, "A", "5");
			Assert.AreEqual(3, numberOfCallbacks, "Expected 3 callbacks since most of the value setters are the same");

			parser.AddVariable<string>("B", x => Assert.AreEqual("VALUE", x));
			parser.SetValue(ID, "B", "VALUE");
		}
	}
}