using System;
using System.Collections.Generic;
using System.Text;

namespace TemporalMessageFormat
{
	public class Section
	{
		public readonly string Id;
		public readonly Dictionary<string, string?> Variables = new Dictionary<string, string?>();

		public Section(string id)
		{
			Id = id;
		}
	}
}
