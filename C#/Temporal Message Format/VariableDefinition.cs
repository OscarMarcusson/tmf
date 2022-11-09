using System;
using System.Collections.Generic;
using System.Text;

namespace TemporalMessageFormat
{
	internal class VariableDefinition
	{
		public readonly Type Type;
		public object Value { get; set; }

		public VariableDefinition(object? defaultValue)
		{
			Type = defaultValue?.GetType() ?? typeof(string);
			Value = defaultValue;
		}
	}
}
