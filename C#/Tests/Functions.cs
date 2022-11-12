namespace Tests
{
	[TestClass]
	public class Functions
	{
		const string ID = "AS123-K92341";

		[TestMethod]
		public void Add()
		{
			var parser = new Parser<TestData>();
			parser.Parse(
				$"#{ID}\n" +
				"423705600 FN_NAME");

			Assert.IsTrue(parser.TryGetFunctions(ID, out var functions));
			Assert.AreEqual(1, functions.Count);
			Assert.AreEqual(new DateTime(423705600, DateTimeKind.Utc), functions[0].UtcTime);
			Assert.AreEqual("FN_NAME", functions[0].Name);
			Assert.AreEqual(null, functions[0].Arguments);
		}

		[TestMethod]
		public void Is_sorted_by_time()
		{
			throw new NotImplementedException();
		}

		[TestMethod]
		public void On_add_callbacks()
		{
			throw new NotImplementedException();
		}
	}
}