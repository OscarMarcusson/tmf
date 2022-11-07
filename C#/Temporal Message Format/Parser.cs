using System;
using System.Collections.Generic;

namespace TemporalMessageFormat
{
	public class Parser
	{
		readonly Dictionary<string, Section> Sections = new Dictionary<string, Section>();
		readonly Dictionary<string, Action<string>> VariableParsers = new Dictionary<string, Action<string>>();



		public Parser AddVariable<T>(string key,  Action<T>? newValueCallback = null)
		{
			// TODO:: Add callback
			return this;
		}

		public Parser AddVariablesFrom<T>(Func<string, string>? keyNameGenerator = null)
		{
			if (keyNameGenerator == null)
				keyNameGenerator = x => x;

			// TODO:: Resolve properties via reflection, call AddVariable for each of them
			return this;
		}
	}
}
