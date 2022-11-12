using System;
using System.Collections.Generic;
using System.Text;

namespace TemporalMessageFormat
{
	public class Function
	{
		public readonly DateTime UtcTime;
		public readonly DateTime? InvalidAfterUtcTime;
		public readonly string Name;
		public readonly string[] Arguments;

		public Function(string line)
		{
			// Resolve time
			var index = line.IndexOf(' ');
			if (index < 0)
				throw new Exception("No function name found");

			var rawTicks = line.Substring(0, index);
			if (!long.TryParse(rawTicks, out var ticks))
				throw new Exception($"Could not convert '{rawTicks}' to a number");

			UtcTime = new DateTime(ticks, DateTimeKind.Utc);

			// Resolve name
			var nameEndIndex = line.IndexOf(' ', index + 1);
			
			// No arguments?
			if(nameEndIndex < 0)
			{
				Name = line.Substring(index + 1);
			}
			// At least one argument
			else
			{
				Name = line.Substring(index + 1, nameEndIndex - index - 1);

				var start = nameEndIndex + 1;
				for (var i = start+1; i< line.Length; i++)
				{
					// String
					if (line[i] == '"')
					{

					}
					// Normal value
					else
					{
						// Find the next space
						while(++i < line.Length && line[i] != ' ') { }
						var word = line.Substring(start, i - start);
						start = i+1;
					}
				}
			}
		}
	}
}
