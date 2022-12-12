using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TemporalMessageFormat
{
	internal class MessageParser
	{
		public readonly ValueSetter setter;
		public readonly Type type;

		public MessageParser(ValueSetter setter, Type type, Func<TextReader, Task<object>> parser)
		{
			this.setter = setter;
			this.type = type;
		}
	}
}
