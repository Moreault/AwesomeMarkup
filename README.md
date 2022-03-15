![AwesomeMarkup](https://github.com/Moreault/AwesomeMarkup/blob/master/awesomemarkup.png)
# AwesomeMarkup
Extracts any kind of markup information from a string of characters.

## Good to know

This is meant to be used for short strings of text such as spoken dialog in a video game and not for entire documents. I don't believe this library would perform very well in the latter scenario.

If you're looking for a complete and ready-to-use dialog parsing library, use the DML.NET library instead.

## Getting started

### Setup

This library makes use of AutoInject to automatically inject its dependencies. If you already use AutoInject or AssemblyInitializer then you have nothing to do for this step.

If you are using dependency injection without AutoInject or AssemblyInitializer then you'll need to use the following in your initialization code :

```c#
services.AddAwesomeMarkup();
```

### Injection

You also need to inject the IMarkupParser interface wherever you want to use it.

```c#
private readonly IMarkupParser _markupParser;

public SomeService(IMarkupParser markupParser)
{
	_markupParser = markupParser;
}
```

### Parsing

```c#
//parsed.Text contains the text surrounded by the tags while the tags property contains information about the tags themselves
var parsed = _markupParser.Parse("Some <color=red>text</color> <underline>containing</underline> DML.");

//in the case of the color tag, its name is "color" and its value is "red"

//However, if we used attributes rather than the tag value...
var parsed = _markupParser.Parse("Some <color red=200 green=12 blue=54>text</color> <underline>containing</underline> DML.");

//the color tag would have no value but the attribute names "red", "green" and "blue" with values 200, 12 and 54 respectively.

//multiple nested tags are also supported if you want bold underlined colored text
var parsed = _markupParser.Parse("Some <color red=200 green=12 blue=54><underline><bold>text</bold></underline></color> <underline>containing</underline> DML.");

```

## Known issues / TODO
AwesomeMarkup is an ambitious project that is actively developped but still in beta. Standard use cases as discussed above are covered but it may not work very well for edge/weird cases.

1.0.0 is planned to be released once the following is implemented :
-Allow spaces inside attribute values that are surrounded by quotes (important, this next on the list)
-Escapable brackets
-Support for self-closing tags (such as <br /> in HTML)
-You shouldn't be able to use unescaped tag brackets inside tags
-Support for XML processing tags (ex : <?version?>) (not very critical since I don't expect short strings to have this but it should be at the very least basically supported)