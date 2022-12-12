using System;
using System.Collections.Generic;
using System.Text;

namespace TemporalMessageFormat
{
	public enum ParserErrorType
	{
		Header,
		Value
	}
	public class ParserError
	{
		public long StreamId { get; set; }
		public string Type { get; set; } = string.Empty;
		public string Error { get; set; } = string.Empty;
		public string Source { get; set; } = string.Empty;
		public ParserErrorType ErrorType { get; set; }
		public Exception? Exception { get; set; }
	}
}
