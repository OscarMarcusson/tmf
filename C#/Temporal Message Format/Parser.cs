using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace TemporalMessageFormat
{
	public class Parser
	{
		readonly Dictionary<string, MessageParser> Parsers = new Dictionary<string, MessageParser>();


		// Callbacks
		event Action<ParserError> ErrorCallback;



		public Parser ListenToErrors(Action<ParserError> errorCallback)
		{
			ErrorCallback += errorCallback;
			return this;
		}




		public Parser Add<T>(string type, int version, Func<Message<T>, Task> handler) where T : new()
		{
			// Validation
			if (version < 1) 
				throw new ArgumentException("Version must be 1 or higher");

			if (type.Contains(":"))
				throw new ArgumentException("The type can't contain ':'");

			type = $"{type}:{version}";
			if (Parsers.ContainsKey(type))
				throw new ArgumentException($"A type called '{type}' already exists");

			// Setup to allow the parser to work
			var setter = ValueSetter.GetOrCreateValueSetter(typeof(T));


			// Create the parser
			var parser = new MessageParser(setter, typeof(T), async reader => await ParseUntilNewHeader<T>(reader, setter));
			Parsers.Add(type, parser);
			return this;
		}



		static async Task<T> ParseUntilNewHeader<T>(TextReader reader, ValueSetter setter) where T : new()
		{
			var instance = new T();

			while (true)
			{
				var peek = reader.Peek();
				if (peek < 0 || peek == '>')
					return instance;

				var line = await reader.ReadLineAsync();
				var index = line.IndexOf('=');

				// Simple value
				if(index > -1)
				{
					var name = line.Substring(0, index);
					var value = line.Substring(index + 1);

					if (!setter.simpleValueSetters.TryGetValue(name, out var valueSetter))
						throw new Exception($"Could not apply value '{value}' to '{name}'. The class '{typeof(T).Name}' does not contain a variable by that name");

					valueSetter(instance, value);
				}
				// Complex value
				else
				{
					throw new NotImplementedException();
				}
			}
		}


		static async Task SkipUntilNewHeader(TextReader reader)
		{
			while (true)
			{
				var peek = reader.Peek();
				if (peek < 0 || peek == '>')
					return;

				await reader.ReadLineAsync();
			}
		}





		public void Parse(string rawString)
		{
			using var stream = new StringReader(rawString);
			ParseAsync(stream).Wait();
		}

		public void Parse(TextReader stream) => ParseAsync(stream).Wait();

		public async Task ParseAsync(TextReader stream)
		{
			while (true)
			{
				var line = await stream.ReadLineAsync();
				if (line == null)
					break;

				if (line.Length == 0)
					continue;

				// Header
				if (line.StartsWith(">"))
				{
					long streamId = -1;
					string type;
					long? messageIndex = null;
					string error = string.Empty;
					try
					{
						// > 4256323 UPDATE-UNIT:1 1
						// ^^
						// Ensure the syntax of the start marker
						if (!line.StartsWith("> "))
						{
							error = "Invalid header, expected it to start with '> '";
							throw null!;
						}


						// > 4256323 UPDATE-UNIT:1 1
						//   ^^^^^^^
						//   Load the stream ID
						var startIndex = 2;
						var endIndex = line.IndexOf(' ', startIndex);
						if (endIndex < 0)
						{
							error = "No space after the stream ID";
							throw null!;
						}
						var rawStreamId = line.Substring(startIndex, endIndex - startIndex);
						if (!long.TryParse(rawStreamId, out streamId))
						{
							error = $"Invalid stream ID: {rawStreamId}";
							throw null!;
						}
						startIndex = endIndex + 1;
						if (startIndex >= line.Length)
						{
							error = "No type after the stream ID";
							throw null!;
						}


						// > 4256323 UPDATE-UNIT:1 1
						//           ^^^^^^^^^^^^^
						//           Load the type and (optional) version
						endIndex = line.IndexOf(' ', startIndex);
						if (endIndex < 0)
						{
							error = "No space after the type";
							throw null!;
						}
						type = line.Substring(startIndex, endIndex - startIndex);

						// Ensure the type isnt just ""
						if (type.Length == 0)
						{
							error = "Empty type";
							throw null!; 
						}

						// > 4256323 UPDATE-UNIT:1 1
						//                      ^^
						//                      Extract the version if one exists
						int version = 1;
						startIndex = type.IndexOf(':');
						if (startIndex > -1)
						{
							// Ensure that the type isnt just ":1", we expect at least "A:1"
							if(startIndex == 0)
							{
								error = $"Invalid type, expected at least one character before the version marker: {type}";
								throw null!;
							}

							// Extract the version, and remove from type. "A:1" is split into "A" and "1"
							var rawVersion = type.Substring(startIndex + 1);
							type = type.Substring(0, startIndex);

							// Parse the version as a number 
							if (!int.TryParse(rawVersion, out version))
							{
								error = $"Invalid type version, expected an integer: {rawVersion}";
								throw null!;
							}
						}

						// > 4256323 UPDATE-UNIT:1 1
						//                         ^
						//                         Get the message index
						startIndex = endIndex + 1;
						if(startIndex >= line.Length)
						{
							error = "Expected a message index at the end, or '?' if it no index validation should be done";
							throw null!;
						}
						var rawMessageIndex = line.Substring(startIndex);
						if(rawMessageIndex != "?")
						{
							// Parse the index as a number
							if(!long.TryParse(rawMessageIndex, out var parsedIndex))
							{
								error = $"Invalid message index, expected '?' or an integer: {rawMessageIndex}";
								throw null!;
							}

							// Check that the given version is valid

							messageIndex = parsedIndex;
						}
					}
					catch(Exception? e)
					{
						ErrorCallback?.Invoke(new ParserError
						{
							StreamId = streamId,
							Error = error,
							ErrorType = ParserErrorType.Header,
							Source = line,
							Exception = e
						});
						await SkipUntilNewHeader(stream);
						continue;
					}
				}

				// Global data

			}
		}


		public static T Parse<T>(string rawString) where T : new()
		{
			using var stream = new StringReader(rawString);
			return Parse<T>(stream).GetAwaiter().GetResult();
		}

		public static async Task<T> Parse<T>(TextReader stream) where T : new()
		{
			var setter = ValueSetter.GetOrCreateValueSetter(typeof(T));
			var instance = await ParseUntilNewHeader<T>(stream, setter);
			if (stream.Peek() > -1)
				throw new ArgumentException($"Unexpected header");

			return instance;
		}
	}
}
