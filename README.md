![AwesomeMarkup](https://github.com/Moreault/AwesomeMarkup/blob/master/awesomemarkup.png)
# AwesomeMarkup
Extracts any kind of markup information from a string of characters.

## Good to know

This library is a high-level, general-purpose markup parser meant to make it easy to extract markup information from a string. It is geared towards relatively short strings (such as dialog lines or UI text) rather than parsing entire documents, though the 4.0.0 rewrite made it considerably faster and lighter.

If you're looking for a complete and ready-to-use dialog parsing library, use the [DML.NET](https://github.com/Moreault/DML.NET) library instead.

## Getting started

### Setup

Register AwesomeMarkup's services in your initialization code :

```c#
services.AddAwesomeMarkup();
```

Registration is fully explicit and reflection-free, so the library is trimming/NativeAOT-safe. By default services are registered as `Singleton`; pass a different `ServiceLifetime` if you need to override it :

```c#
services.AddAwesomeMarkup(ServiceLifetime.Scoped);
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

## Supported features

- Tags with a value (`<color=red>`), attributes (`<color red=200 green=12 blue=54>`) or both.
- Arbitrarily nested tags. Each piece of text is returned with the flattened list of all the tags that enclose it.
- Quoted attribute values, including values that contain the attribute separator (`<note type="some thing or another">`).
- Self-closing tags (`<br/>` or `<br />`).
- XML-style processing tags (`<?xml version="1.0"?>`).
- Optional bracket escaping (see below).
- Unescaped brackets inside a tag are rejected so that malformed markup fails fast.

## Customizing the markup language

`Parse` accepts an optional `MarkupLanguageSpecifications`. When omitted it defaults to `MarkupLanguageSpecifications.Dml`. You can change the brackets, the attribute separator/assignation characters, the quote rules, and the escape character.

```c#
var specifications = MarkupLanguageSpecifications.Dml with { EscapeCharacter = '\\' };

//With escaping enabled, escaped brackets are treated as literal text instead of markup.
var parsed = _markupParser.Parse(@"price is \<100\>", specifications);
```

Escaping is opt-in : it is disabled (`null`) by default and only escapes the opening bracket, the closing bracket and the escape character itself, so characters such as Windows path separators are left untouched.

## Performance

As of 4.0.0 the parser performs a single pass over the input using a span-based lexer and an explicit tag stack, instead of repeatedly extracting and re-parsing nested content. This removes the previous quadratic tag-linking and the large intermediate allocations, so it now handles longer strings and deeper nesting comfortably.
