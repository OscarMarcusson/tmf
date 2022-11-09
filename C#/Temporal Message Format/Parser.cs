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

		public void SetValue(string id, string variable, object? value)
		{
			if (!Sections.TryGetValue(id, out var section))
			{
				section = new Section(id);
				Sections[id] = section;
			}

			section.Set(variable, value);
		}

		public bool TryGetVariable(string id, string variable, out string? value)
		{
			if (!Sections.TryGetValue(id, out var section))
			{
				value = null;
				return false;
			}

			var response = section.Variables.TryGetValue(variable, out var variableDefinition);
			value = variableDefinition.Value?.ToString();
			return response;
		}

		public bool TryGetVariable<T>(string id, string variable, out T value, T defaultValue = default!)
		{
			if (!Sections.TryGetValue(id, out var section))
			{
				value = defaultValue;
				return false;
			}

			if(!section.Variables.TryGetValue(variable, out var variableDefinition))
			{
				value = defaultValue;
				return false;
			}


			value = (T)variableDefinition.Value;
			return true;
		}
	}
}
