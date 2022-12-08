# Temporal Message Format

A simple, human-readable message format for events.

## Example

```cpp
> 423 UPDATE_UNIT:1 1
position
    x=253.643
    y=5235.634
    z=0.3256
> 424 CREATE_UNIT:3 2
position
    x=56293.62
    y=0
    z=523.46
model
    skin_color="#b03fc4"
    base_mesh=5324235
    head=89327035
    torso=32843553
    arms=0
    legs=532874335
    weapon=38253005
> 425 UPDATE-UNIT:1 1
position
    x=257.734
    y=5235.634
    z=3.6234
> 456
inventory
    5:id=-1
    6:id=53253;quantity=63
    7:quantity=6
> 426 UPDATE-UNIT:2 2
position
    x=258.663
    y=5234.844
    z=3.572
model:weapon=9352032
> 427 CHAT:1 53
to=1,3,7,8,9,34
message="Hello World!"
> 428 CHAT:1 53
to=53
message="Hello!"
> 429 CHAT:1 3
to=53
message="Hi!"
```

## Structure

The tmf format is parsed line by line, using key-value pairs and indentation-based hierarchies to describe data. Anything placed after a header is understood to belong to it, until another header is supplied or the message ends.

### Header

The header rows start with `>`, and is expected to contain the following data: `index` `type`:`version` `id`. To give a few examples:

```cpp
> 8564 ITEM_CREATED:3 A1032
> 8565 ITEM_UPDATED:1 A1032
> 8566 ITEM_UPDATED:1 B6432
> 8567 ITEM_DESTROYED:2 B6432
```

| Part    | Usage                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| ------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Index   | The index describes the order to parse messages. As in the example above it may simply be an increasing number, but it may also be something like a Unix timestamp, number of milliseconds since program start or any other integer based counting system.<br/>**Note:** Each index _must_ be greater than the previous index. They are allowed to increase by any number greater than 0.                                                                                              |
| Type    | The type tells the parser which specific handler to use to parse the given values. If one wishes to send a message the type may be something like `SEND_MSG`, but if one intends to throw a fireball if may simply be a `SPAWN_EFFECT`. This fully depends on the implementation.                                                                                                                                                                                                      |
| Version | The version is **optional**, but is very useful for a more complex event sourcing system. It allows the parser to map multiple handlers to the same type, but as the application is changed over time one may need to create new versions. Instead of rewriting existing handlers and worrying about migrating old events, one can instead append `:2` or `:3` to the type to tell the parser to use version 2 or 3 instead, and supply it with whatever variables and logic it needs. |
| Id      | The ID is simply the identifier to the handler to bind the changes to. For a real-time application this may be a players or monsters ID, but it may also be the ID for a stream of events that are connected in an event sourcing system. The usage of the ID is arbitrary, and depends on the implementation.                                                                                                                                                                         |

**Note:** All values are parsed as "global" values if no header is given. In that case the values belong to no-one and is outside of the event system. The parser may or may not allow this, as it depends on the implementation, but it may find some use in configuration files or if only the payload is saved in a database blob that later has to be parsed again.

### Values

The value rows are expected after a header, and do not have any special starting character. They are written in the `[key]=[value]` format, and typically look something like:

```cpp
position
    x=2.352
    y=0.0
    z=5.324
inventory
    0:id=644354;quantity=5
    1:id=987531;quantity=64
    2:id=873001;quantity=738
height=182
name="Knut Knutsson"
jumping=true
```

_Note that the indentation is done with spaces in the example above. The format expects tabs!_

#### Value types

There are 4 built-in value types:

| Type    | Description                                                                                                                                                                                                                                      |
| ------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| boolean | A true or false value: `is_movable=true`                                                                                                                                                                                                         |
| integer | A whole number: `number_of_items=64`                                                                                                                                                                                                             |
| number  | A decimal number, specified by the decimal point and at least one decimal value: `speed=5.36234` or `speed=0.0`                                                                                                                                  |
| text    | An arbitrary number of letters and / or digits, wrapped by quotation marks: `name="Knut Knutsson"`<br/>The following special characters may be used:<br/>`\"` = a quotation mark<br/>`\\` = a backslash<br/>`\n` = a line break<br/>`\t` = a tab |

##### Complex types

A complex type is a child object that contain its own values. This is often used for hierarchical structures, but also for things like tables and arrays. Let's take a look at the example above:

```cpp
position
    x=2.352
    y=0.0
    z=5.324
```

This will create a value named `position`, which will contain three values: `x`, `y`, and `z`. All three are of the number type. This structure is usually used for different types in a programs implementation, so this type of grouping tends to make the mapping easier. But there are also times when it's necessary, like the inventory example:

```cpp
inventory
    0:id=644354;quantity=5
    1:id=987531;quantity=64
    2:id=873001;quantity=738
```

Here we create a `inventory` value, which contains three values: `0`, `1`, and `2`. Each of these in turn contain their own values: `id` and `quantity`. This is a common pattern for creating an array of complex types, where the key becomes the array index. Note that the above example is identical to:

```cpp
inventory
    0
        id=644354
        quantity=5
    1
        id=987531
        quantity=64
    2
        id=873001
        quantity=738
```

As can be seen between these two examples, it's possible to shorten the definition of complex structures. By placing a colon `:` after a complex type, we tell the parser that anything after this point, **as long as it's on the same line**, are child values. The child values are separated by the semicolon `;`. This can only be done on the deepest node, if a child row (indented by one more) is placed underneath a shortened row it will cause a parse failure.

**Note:** Anything that does not have an equals sign after it is treated like a complex type.
