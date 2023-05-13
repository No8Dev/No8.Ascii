## StringHelper

### ParseArguments()

ParseArguments returns a string array containing substrings split by multi-lines, tabs, spaces, non-breaking-spaces, other white-space. Supports double and single quoted strings. 

`"one\ttwo\tthree".ParseArguments()` = `["one","two", "three"]`

`"before 'one two three' after".ParseArguments()` = `["before", "one two three", "after"]`

`"sin'gle'".ParseArguments()` = `["sin", "gle"]`

## Parameters

Parameters is a *dynamic* object, supporting converting an object into a dictionary.
Parameters can be used to support command-line, or other string, processing.

## ArgsParser

Generic command-line, string, bot message, parser.

```c#
var parser = new ArgsParser()
    .AddCommand<PlanArgs>((args, extras) => { })
    .AddCommand<BattleArgs>((args) => { }, isDefault: true );
parser.Parse("Plan --name Allo --value:World! and stuff");
parser.Parse("--name Joe --value Bloggs");      // hits default command
```

***
```
PLAN --verbose -language=pirate sea battle
```

what this means is:

The command verb is `PLAN`, `verbose` set to `true`, the `language` parameter is `pirate` and `sea` and `battle` are the extra parameters.

***

```
BATTLE --intensity=insane
```

`BATTLE` is the command and the `intensity` will be `insane`.

