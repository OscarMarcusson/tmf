using System;
using System.Collections.Generic;
using System.Text;

namespace TemporalMessageFormat
{
	internal class Section
	{
		public readonly string Id;
		public readonly Dictionary<string, VariableDefinition> Variables = new Dictionary<string, VariableDefinition>();

		public Section(string id)
		{
			Id = id;
		}

		public void Set(string name, object? value)
		{
			if(Variables.TryGetValue(name, out var variable))
			{
				if(value == null)
				{
					throw new NotImplementedException();
				}
				else
				{
					if (!variable.Type.IsAssignableFrom(value.GetType()))
						throw new Exception($"Could not convert '{value.GetType()}' to '{variable.Type}' for value '{value}'");

					variable.Value = value;
				}
			}
			else
			{
				variable = new VariableDefinition(value);
				Variables[name] = variable;
			}
		}
	}
}
