namespace Tests
{
	class TestData
	{
		public string Name { get; set; }
		public int X { get; set; }
		public float Y { get; set; }
		public double Z { get; set; }
		public decimal W { get; set; }
		public bool Yes { get; set; }
	}

	[TestClass]
	public class Variables
	{
		const string ID = "AS123-K92341";

		[TestMethod]
		public void Add()
		{
			var parser = new Parser<TestData>();
			parser.Parse(
				$"#{ID}\n" +
				"Name Kalle Kun\n" +
				"X 5\n" +
				"Y 33.74\n" +
				"Z 74.32\n" +
				"W 16.75\n" +
				"Yes TRUE");

			Assert.IsTrue(parser.TryGetInstance(ID, out var instance));
			Assert.AreEqual("Kalle Kun", instance.Name);
			Assert.AreEqual(5, instance.X);
			Assert.AreEqual(33.74f, instance.Y);
			Assert.AreEqual(74.32, instance.Z);
			Assert.AreEqual(16.75m, instance.W);
			Assert.AreEqual(true, instance.Yes);
		}

		[TestMethod]
		public void Override()
		{
			var parser = new Parser<TestData>();
			parser.Parse(
				$"#{ID}\n" +
				"Name No 123\n" +
				"X 0\n" +
				"Y -74.23\n" +
				"Z -36.74\n" +
				"W -62.89\n" +
				"Yes FALSE");
			parser.Parse(
				$"#{ID}\n" +
				"Name Kalle Kun\n" +
				"X 5\n" +
				"Y 33.74\n" +
				"Z 74.32\n" +
				"W 16.75\n" +
				"Yes TRUE");

			Assert.IsTrue(parser.TryGetInstance(ID, out var instance));
			Assert.AreEqual("Kalle Kun", instance.Name);
			Assert.AreEqual(5, instance.X);
			Assert.AreEqual(33.74f, instance.Y);
			Assert.AreEqual(74.32, instance.Z);
			Assert.AreEqual(16.75m, instance.W);
			Assert.AreEqual(true, instance.Yes);
		}

		[TestMethod]
		public void On_value_changed_callbacks()
		{
			var parser = new Parser<TestData>();
			var numberOfCallbacks = 0;
			parser.OnInstanceChanged += _ => numberOfCallbacks++;

			parser.Parse(
				$"#{ID}\n" +
				"Name No 123\n" +
				"X 0\n" +
				"Y -74.23\n" +
				"Z -36.74\n" +
				"W -62.89\n" +
				"Yes FALSE");
			parser.Parse(
				$"#{ID}\n" +
				"Name Kalle Kun\n" +
				"X 5\n" +
				"Y 33.74\n" +
				"Z 74.32\n" +
				"W 16.75\n" +
				"Yes TRUE");

			Assert.AreEqual(2, numberOfCallbacks);
		}
	}
}