using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TemporalMessageFormat
{
	internal class ValueSetter
	{
		public readonly Dictionary<string, Action<object, string>> simpleValueSetters = new Dictionary<string, Action<object, string>>();
		public readonly Dictionary<string, Func<object, ValueSetterInstance>> complexInstanceCreator = new Dictionary<string, Func<object, ValueSetterInstance>>();

		static readonly Dictionary<Type, ValueSetter> allSetters = new Dictionary<Type, ValueSetter>();


		public ValueSetter(Type type)
		{
			// Do NOT remove this, see GetOrCreateValueSetter for details
			allSetters[type] = this;

			// Create setters for public properties with read / write access
			var allProperties = type.GetProperties(
									   BindingFlags.Public |
									   BindingFlags.Instance |
									   BindingFlags.GetProperty |
									   BindingFlags.SetProperty);
			AddSimple(allProperties, x => x.PropertyType, x => x.Name, x => x.SetValue);
			AddComplex(allProperties, x => x.PropertyType, x => x.Name, x => x.GetValue, x => x.SetValue);


			// Create setters for public fields
			var allFields = type.GetFields(
								   BindingFlags.Public |
								   BindingFlags.Instance);
			AddSimple(allFields, x => x.FieldType, x => x.Name, x => x.SetValue);
			AddComplex(allFields, x => x.FieldType, x => x.Name, x => x.GetValue, x => x.SetValue);
		}


		// For simple values we just load the type and SetValue action, and wrap it in our own "string -> value" parsers
		// Since we're resolving this the first time the runtime performance is decent
		void AddSimple<T>(IEnumerable<T> sources, Func<T, Type> typeSelector, Func<T, string> nameSelector, Func<T, Action<object,object>> setterSelector)
		{
			foreach (var source in sources)
			{
				var type = typeSelector(source);
				if (type.IsSimpleValue())
				{
					var name = nameSelector(source);
					var setter = setterSelector(source);
					simpleValueSetters[name] = type.CreateSetter(name, setter);
				}
			}
		}


		// FThe complex types need an instance creator / setter, as well as need
		// their own value setters for the content
		//
		// Since the complex types may be a type that can't be null they may already
		// exist, so we also include a getter to ensure that we don't try and
		// overwrite existing data
		void AddComplex<T>(
			IEnumerable<T> sources,
			Func<T, Type> typeSelector,
			Func<T, string> nameSelector,
			Func<T, Func<object, object>> getterSelector,
			Func<T, Action<object, object>> setterSelector)
		{
			foreach (var source in sources)
			{
				var type = typeSelector(source);
				if (type.IsComplexType())
				{
					var name = nameSelector(source);
					var setter = setterSelector(source);
					var valueGetter = getterSelector(source);
					var valueSetter = GetOrCreateValueSetter(type);
					Func<object, object> createInstance = instance =>
					{
						var value = valueGetter(instance);
						if(value == null)
						{
							value = Activator.CreateInstance(type);
							setter(instance, value);
						}
						return value;
					};
					complexInstanceCreator[name] = instance => new ValueSetterInstance(valueSetter, createInstance(instance));
				}
			}
		}


		internal static ValueSetter GetOrCreateValueSetter(Type forType)
		{
			if (allSetters.TryGetValue(forType, out var setter))
				return setter;

			// So why don't we just do "allSetters[forType] = setter;"? To
			// avoid an infinite loop we HAVE to set the dictionary value for
			// this type in the constructor, since the constructor itself will
			// call this method. Otherwise we'll be stuck in an infinite loop
			// if a type contains itself or a parent type, since these are
			// added after the constructor is done if the = is written here
			return new ValueSetter(forType);
		}
	}


	internal class ValueSetterInstance
	{
		public readonly ValueSetter setter;
		public readonly object instance;

		public ValueSetterInstance(ValueSetter setter, object instance)
		{
			this.setter = setter;
			this.instance = instance;
		}
	}
}
