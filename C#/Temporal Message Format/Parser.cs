using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TemporalMessageFormat
{
	public class Parser<T> where T : class, new()
	{
		readonly Dictionary<string, T> Instances = new Dictionary<string, T>();
		readonly Dictionary<string, Action<T, string>> Setters = new Dictionary<string, Action<T, string>>();
		readonly Dictionary<string, List<Function>> Functions = new Dictionary<string, List<Function>>();
		public event Action<T>? OnInstanceChanged;

		public Parser()
		{
			var type = typeof(T);
			
			var properties = type.GetProperties();
			foreach (var property in properties)
				CreateSetter(property.Name, property.PropertyType, property.SetValue);

			var fields = type.GetFields();
			foreach (var field in fields)
				CreateSetter(field.Name, field.FieldType, field.SetValue);
		}


		void CreateSetter(string name, Type type, Action<T,object> setter)
		{
			if (Nullable.GetUnderlyingType(type) != null)
				type = Nullable.GetUnderlyingType(type);

			if (type == typeof(string))       Setters[name] = setter;

			else if (type == typeof(bool))    Setters[name] = (instance, raw) => setter(instance, bool.Parse(raw));
			
			else if (type == typeof(byte))    Setters[name] = (instance, raw) => setter(instance, byte.Parse(raw));
			else if (type == typeof(ushort))  Setters[name] = (instance, raw) => setter(instance, ushort.Parse(raw));
			else if (type == typeof(uint))    Setters[name] = (instance, raw) => setter(instance, uint.Parse(raw));
			else if (type == typeof(ulong))   Setters[name] = (instance, raw) => setter(instance, ulong.Parse(raw));
			
			else if (type == typeof(sbyte))   Setters[name] = (instance, raw) => setter(instance, sbyte.Parse(raw));
			else if (type == typeof(short))   Setters[name] = (instance, raw) => setter(instance, short.Parse(raw));
			else if (type == typeof(int))     Setters[name] = (instance, raw) => setter(instance, int.Parse(raw));
			else if (type == typeof(long))    Setters[name] = (instance, raw) => setter(instance, long.Parse(raw));
			
			else if (type == typeof(float))   Setters[name] = (instance, raw) => setter(instance, float.Parse(raw));
			else if (type == typeof(double))  Setters[name] = (instance, raw) => setter(instance, double.Parse(raw));
			else if (type == typeof(decimal)) Setters[name] = (instance, raw) => setter(instance, decimal.Parse(raw));

			else if(type.IsEnum) Setters[name] = (instance, raw) => setter(instance, Enum.Parse(type, raw));

			else Setters[name] = (i,v) => throw new NotImplementedException($"The '{name}' variables type '{type.Name}' does not have a setter implementation");
		}


		public async Task Parse(TextReader stream)
		{
			string? id = null;
			T? instance = null;
			var lineNumber = 0;
			var i = 0;
			var anyChanged = false;
			while (true)
			{
				lineNumber++;
				var line = await stream.ReadLineAsync();
				if (line == null)
					break;

				// Id
				if (line.StartsWith("#"))
				{
					if(instance != null && anyChanged)
						OnInstanceChanged?.Invoke(instance);

					anyChanged = false;
					id = line.Substring(1);
					if(!Instances.TryGetValue(id, out instance))
					{
						instance = new T();
						Instances[id] = instance;
					}
				}

				// Validation
				else if(id == null || instance == null)
				{
					throw new Exception($"Message did not start with an ID");
				}
				else if(string.IsNullOrWhiteSpace(line))
				{
					throw new Exception($"Line {lineNumber} was empty");
				}

				// Function
				else if (char.IsDigit(line[0]))
				{
					var function = new Function(line);
					if(!Functions.TryGetValue(id, out var functions))
					{
						functions = new List<Function>();
						Functions[id] = functions;
					}
					functions.Add(function);
				}

				// Variable
				else
				{
					i = line.IndexOf(' ');
					if (i < 0)
						throw new Exception($"Line {lineNumber} does not have a value");

					var name = line.Substring(0, i);
					var value = line.Substring(i + 1);

					if(!Setters.TryGetValue(name, out var setter))
						throw new Exception($"Could not find a variable named '{name}' (line {lineNumber})");

					setter(instance, value);
					anyChanged = true;
				}
			}

			if (instance != null && anyChanged)
				OnInstanceChanged?.Invoke(instance);
		}

		public void Parse(string message)
		{
			using (var stream = new StringReader(message))
			{
				Parse(stream).ConfigureAwait(false).GetAwaiter().GetResult();
			}
		}

		public bool TryGetInstance(string id, out T instance) => Instances.TryGetValue(id, out instance);



		// TODO:: Implement
		public void ClearMemoryBefore(long time) => throw new NotImplementedException();

		public bool TryGetFunctions(string id, out List<Function> functions) => Functions.TryGetValue(id, out functions);
	}
}
