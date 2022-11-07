# Temporal Message Format
A simple human-readable message format made for real-time applications like games.

## Example
```C
// A made up example file for some game that includes a chat and some game object & component creation
#A531
A 5
B "Hello World"
1667849591 MSG "Some message sent"
1667849609 MSG "Some other message sent"
1667849620 CREATE 2847238642393 0.1 0 0.63 36.3 0 320.63 1.0 "Instance"
#B983
A 755
B "Another Hello World!"
#A152
1667849748 CREATE 9273596 0 0 0 0 90 0 1.0 "<temp>"
1667849788 ADD_C "<temp>" "MobSpawner" 7
1667849832 ADD_C "<temp>" "Repeater" 0.3
```

## Structure
The tmf format is parsed line by line, and is built around syncing values as well as calling functions for a given time.
Type | Structure | Parser notes
--- | --- | ---
ID | #[ID] | **Mandatory**<br>Starts with a hashtag, remainder is the ID<br>Whitespaces are **not** trimmed
Variable | [Name] [Value] | Any line that does **not** start with a hashtag or digit is parsed as a variable<br>The parse fails if multiple value exist, unless written as an array<br>Overrides existing value<br>If no value is given it will be parsed as `NULL`
Function | [Time] [Name] [Arguments...] | Starts with a digit<br>Number and type of arguments (if any) depend on the implementation


## Values
Values are generally up to the implementation, but these core types exist:
Type | Description
--- | ---
String | A standard code string like `"Hello World"` or `"Hello \"World\""`
Number | Anything that starts with a digit.<br>May contain a dot (.) for the decimal point, like `0.3` or `.3`
Array | Anything inside brackets, like `[1 3 5]` or `["Hello" "World"]`
Null | The uppercased `NULL` will be parsed as a null-value if the implementation allows it<br>The parse fails if null is not allowed
