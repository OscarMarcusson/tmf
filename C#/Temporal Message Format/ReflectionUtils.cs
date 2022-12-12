using System;
using System.Collections.Generic;
using System.Text;

namespace TemporalMessageFormat
{
	internal static class ReflectionUtils
	{
		internal static bool IsSimpleValue(this Type type) => type.IsValueType || type == typeof(string);
		internal static bool IsComplexType(this Type type) => type.IsClass && type != typeof(string);


		internal static Action<object, string> CreateSetter(this Type type, string name, Action<object, object> setter)
		{
			if (Nullable.GetUnderlyingType(type) != null)
				type = Nullable.GetUnderlyingType(type);

			if (type == typeof(string)) return setter;

			if (type == typeof(bool)) return (instance, raw) => setter(instance, bool.Parse(raw));

			if (type == typeof(byte)) return (instance, raw) => setter(instance, byte.Parse(raw));
			if (type == typeof(ushort)) return (instance, raw) => setter(instance, ushort.Parse(raw));
			if (type == typeof(uint)) return (instance, raw) => setter(instance, uint.Parse(raw));
			if (type == typeof(ulong)) return (instance, raw) => setter(instance, ulong.Parse(raw));

			if (type == typeof(sbyte)) return (instance, raw) => setter(instance, sbyte.Parse(raw));
			if (type == typeof(short)) return (instance, raw) => setter(instance, short.Parse(raw));
			if (type == typeof(int)) return (instance, raw) => setter(instance, int.Parse(raw));
			if (type == typeof(long)) return (instance, raw) => setter(instance, long.Parse(raw));

			if (type == typeof(float)) return (instance, raw) => setter(instance, float.Parse(raw));
			if (type == typeof(double)) return (instance, raw) => setter(instance, double.Parse(raw));
			if (type == typeof(decimal)) return (instance, raw) => setter(instance, decimal.Parse(raw));

			if (type.IsEnum) return (instance, raw) => setter(instance, Enum.Parse(type, raw));

			return (i, v) => throw new NotImplementedException($"The '{name}' variables type '{type.Name}' does not have a setter implementation");
		}
	}
}
