namespace Tests
{
	class ComplexData
	{
		public ComplexData? child;
		public PropertyData? properties;
		public FieldData? fields;
		public string? name;
	}
	class PropertyData
	{
		public string? Name { get; set; }
		public int X { get; set; }
		public float Y { get; set; }
		public double Z { get; set; }
		public decimal W { get; set; }
		public bool Yes { get; set; }
	}
	class FieldData
	{
		public string? name;
		public int x;
		public float y;
		public double z;
		public decimal w;
		public bool yes;
	}

	[TestClass]
	public class StaticParser
	{
		[TestMethod]
		public void Incorrect_value_name_throws_exception()
		{
			Assert.ThrowsException<Exception>(() => Parser.Parse<PropertyData>("i_dont_exist=5"));
		}


		[TestMethod]
		public void Properties()
		{
			var instance = Parser.Parse<PropertyData>(
				"Name=Kalle Kun\n" +
				"X=5\n" +
				"Y=33.74\n" +
				"Z=74.32\n" +
				"W=16.75\n" +
				"Yes=TRUE");

			Assert.IsNotNull(instance);
			Assert.AreEqual("Kalle Kun", instance.Name);
			Assert.AreEqual(5, instance.X);
			Assert.AreEqual(33.74f, instance.Y);
			Assert.AreEqual(74.32, instance.Z);
			Assert.AreEqual(16.75m, instance.W);
			Assert.AreEqual(true, instance.Yes);
		}

		[TestMethod]
		public void Fields()
		{
			var instance = Parser.Parse<FieldData>(
				"name=Kalle Kun\n" +
				"x=5\n" +
				"y=33.74\n" +
				"z=74.32\n" +
				"w=16.75\n" +
				"yes=TRUE");

			Assert.IsNotNull(instance);
			Assert.AreEqual("Kalle Kun", instance.name);
			Assert.AreEqual(5, instance.x);
			Assert.AreEqual(33.74f, instance.y);
			Assert.AreEqual(74.32, instance.z);
			Assert.AreEqual(16.75m, instance.w);
			Assert.AreEqual(true, instance.yes);
		}

		[TestMethod]
		public void Can_create_complex()
		{
			var instance = Parser.Parse<ComplexData>(
				"name=Test\n" +
				"child\n" +
				"	name=Test child" +
				"	properties\n" +
				"		X=7\n" +
				"	fields\n" +
				"		x=8" +
				"properties\n" +
				"	X=3" +
				"fields\n" +
				"	x=2");

			Assert.IsNotNull(instance);
			Assert.AreEqual("Test", instance.name);

			Assert.IsNotNull(instance.child);

			Assert.IsNotNull(instance.properties);

			Assert.IsNotNull(instance.fields);
		}
	}

	/*
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
	*/
}