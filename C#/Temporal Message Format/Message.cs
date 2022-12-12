using System;
using System.Collections.Generic;
using System.Text;

namespace TemporalMessageFormat
{
	public class Message<T>
	{
		public readonly long streamId;
		public readonly string type;
		public readonly int version;
		public readonly long index;
		public readonly T payload;


		internal Message(string header, T payload)
		{
		}
	}
}
