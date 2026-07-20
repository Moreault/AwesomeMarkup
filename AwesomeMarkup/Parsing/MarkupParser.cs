using System.Text;

namespace ToolBX.AwesomeMarkup.Parsing;

internal sealed class MarkupParser : IMarkupParser
{
    public IReadOnlyList<MetaString> Parse(string value, MarkupLanguageSpecifications? specifications = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw string.IsNullOrEmpty(value)
                ? new ArgumentNullException(nameof(value))
                : new ArgumentException(Exceptions.ValueCannotBeWhitespace, nameof(value));

        return new ParseOperation(value, specifications ?? MarkupLanguageSpecifications.Dml).Run();
    }
}

/// <summary>
/// Single-pass markup parser. It scans the input exactly once, lexing text runs and tags on the fly and using an
/// explicit stack to build the resulting <see cref="MetaString"/> list. This avoids the re-scanning, O(n²) tag
/// linking and large intermediate allocations of a multi-stage pipeline.
/// </summary>
internal sealed class ParseOperation
{
    private readonly string _input;
    private readonly MarkupLanguageSpecifications _specifications;
    private readonly char _open;
    private readonly char _close;
    private readonly char _separator;
    private readonly char _assignation;
    private readonly char? _escape;

    private readonly List<MetaString> _output = [];
    private readonly List<StackFrame> _stack = [];
    private StringBuilder? _builder;

    public ParseOperation(string input, MarkupLanguageSpecifications specifications)
    {
        _input = input;
        _specifications = specifications;
        _open = specifications.Brackets.Opening;
        _close = specifications.Brackets.Closing;
        _separator = specifications.Attributes.Separator;
        _assignation = specifications.Attributes.Assignation;
        _escape = specifications.EscapeCharacter;
    }

    public IReadOnlyList<MetaString> Run()
    {
        var i = 0;
        while (i < _input.Length)
            i = _input[i] == _open ? HandleTag(i) : HandleText(i);

        if (_stack.Count > 0)
            throw new MarkupParsingException(string.Format(Exceptions.OpeningTagWithoutClosingTag, _stack[^1].Tag.Name));

        return _output;
    }

    private int HandleText(int start)
    {
        // Fast path : no escaping, so a text run is simply everything up to the next opening bracket.
        if (_escape is null)
        {
            var relative = _input.AsSpan(start).IndexOf(_open);
            var end = relative < 0 ? _input.Length : start + relative;
            EmitText(_input.AsSpan(start, end - start));
            return end;
        }

        var escape = _escape.Value;
        var builder = RentBuilder();
        var i = start;
        while (i < _input.Length)
        {
            var c = _input[i];
            if (c == escape && i + 1 < _input.Length)
            {
                var next = _input[i + 1];
                if (next == _open || next == _close || next == escape)
                {
                    builder.Append(next);
                    i += 2;
                    continue;
                }
            }
            else if (c == _open)
                break;

            builder.Append(c);
            i++;
        }

        EmitText(builder);
        return i;
    }

    private int HandleTag(int openIndex)
    {
        var tag = BuildTag(ScanTagContent(openIndex, out var nextIndex));

        switch (tag.Kind)
        {
            case TagKind.Opening:
                _stack.Add(new StackFrame(tag, _output.Count));
                break;
            case TagKind.Closing:
                CloseTag(tag);
                break;
            default: // SelfClosing or Processing : self-contained, emitted with the current stack plus itself.
                _output.Add(new MetaString { Tags = SnapshotStack(tag) });
                break;
        }

        return nextIndex;
    }

    private void CloseTag(MarkupTag tag)
    {
        if (_stack.Count == 0)
            throw new MarkupParsingException(Exceptions.UnmatchedClosingTag);

        var top = _stack[^1];
        if (!string.Equals(top.Tag.Name, tag.Name, StringComparison.OrdinalIgnoreCase))
            throw new MarkupParsingException(string.Format(Exceptions.MismatchedClosingTag, tag.Name, top.Tag.Name));

        // If nothing was emitted while this tag was open it is an empty element, which still yields a (text-less) MetaString.
        if (_output.Count == top.EmissionsAtPush)
            _output.Add(new MetaString { Tags = SnapshotStack() });

        _stack.RemoveAt(_stack.Count - 1);
    }

    private void EmitText(ReadOnlySpan<char> text)
    {
        if (text.IsWhiteSpace()) return;
        _output.Add(new MetaString { Tags = SnapshotStack(), Text = text.ToString() });
    }

    private void EmitText(StringBuilder builder)
    {
        for (var i = 0; i < builder.Length; i++)
        {
            if (char.IsWhiteSpace(builder[i])) continue;
            _output.Add(new MetaString { Tags = SnapshotStack(), Text = builder.ToString() });
            return;
        }
    }

    private IReadOnlyList<MarkupTag> SnapshotStack()
    {
        if (_stack.Count == 0) return [];
        var tags = new MarkupTag[_stack.Count];
        for (var i = 0; i < _stack.Count; i++) tags[i] = _stack[i].Tag;
        return tags;
    }

    private IReadOnlyList<MarkupTag> SnapshotStack(MarkupTag extra)
    {
        var tags = new MarkupTag[_stack.Count + 1];
        for (var i = 0; i < _stack.Count; i++) tags[i] = _stack[i].Tag;
        tags[^1] = extra;
        return tags;
    }

    private string ScanTagContent(int openIndex, out int nextIndex)
    {
        var builder = RentBuilder();
        var quote = '\0';
        var i = openIndex + 1;
        while (i < _input.Length)
        {
            var c = _input[i];

            if (_escape is char escape && c == escape && i + 1 < _input.Length)
            {
                var next = _input[i + 1];
                if (next == _open || next == _close || next == escape)
                {
                    builder.Append(next);
                    i += 2;
                    continue;
                }
            }

            if (quote != '\0')
            {
                builder.Append(c);
                if (c == quote) quote = '\0';
            }
            else if (c == _close)
            {
                nextIndex = i + 1;
                return builder.ToString();
            }
            else if (c == _open)
                throw new MarkupParsingException(string.Format(Exceptions.UnescapedBracketInTag, i));
            else if (c is '"' or '\'')
            {
                quote = c;
                builder.Append(c);
            }
            else
                builder.Append(c);

            i++;
        }

        throw new MarkupParsingException(string.Format(Exceptions.UnclosedTag, openIndex));
    }

    private MarkupTag BuildTag(string rawContent)
    {
        var content = rawContent.Trim();
        if (content.Length == 0)
            throw new MarkupParsingException(string.Format(Exceptions.StringDoesNotContainValidParameters, rawContent));

        var startsProcessing = content[0] == '?';
        var endsProcessing = content[^1] == '?';
        var startsSlash = !startsProcessing && content[0] == '/';
        var endsSlash = !endsProcessing && content[^1] == '/';

        if (startsSlash && endsSlash)
            throw new MarkupParsingException(Exceptions.ContainsSelfClosingAndClosingSlashes);

        var kind = TagKind.Opening;
        if (startsSlash) kind = TagKind.Closing;
        else if (endsSlash) kind = TagKind.SelfClosing;
        else if (startsProcessing && endsProcessing) kind = TagKind.Processing;

        content = content.Trim('/', '?').Trim();

        var rules = _specifications.Attributes.QuoteRules;
        if (!rules.Double && content.Contains('"'))
            throw new MarkupParsingException(Exceptions.DoubleQuotesDisallowed);
        if (!rules.Single && content.Contains('\''))
            throw new MarkupParsingException(Exceptions.SingleQuotesDisallowed);

        var words = SplitWords(content);
        if (words.Count == 0)
            throw new MarkupParsingException(string.Format(Exceptions.StringDoesNotContainValidParameters, content));

        var name = ParseParameter(words[0], content);

        MarkupParameter[] attributes;
        if (words.Count == 1)
            attributes = [];
        else
        {
            attributes = new MarkupParameter[words.Count - 1];
            for (var i = 1; i < words.Count; i++)
                attributes[i - 1] = ParseParameter(words[i], content);
        }

        return new MarkupTag
        {
            Name = name.Name,
            Value = name.Value,
            Attributes = attributes,
            Kind = kind
        };
    }

    private List<string> SplitWords(string content)
    {
        var words = new List<string>();
        var quote = '\0';
        var start = 0;
        for (var i = 0; i < content.Length; i++)
        {
            var c = content[i];
            if (quote != '\0')
            {
                if (c == quote) quote = '\0';
            }
            else if (c is '"' or '\'')
                quote = c;
            else if (c == _separator)
            {
                AddWord(words, content, start, i);
                start = i + 1;
            }
        }

        if (quote != '\0')
            throw new MarkupParsingException(string.Format(Exceptions.OddNumberOfQuotes, CountOccurrences(content, quote)));

        AddWord(words, content, start, content.Length);
        return words;
    }

    private static void AddWord(List<string> words, string content, int start, int end)
    {
        if (end <= start) return;
        if (content.AsSpan(start, end - start).IsWhiteSpace()) return;
        words.Add(content.Substring(start, end - start));
    }

    private MarkupParameter ParseParameter(string word, string content)
    {
        var index = word.IndexOf(_assignation);
        if (index < 0)
            return new MarkupParameter { Name = word };

        if (word.IndexOf(_assignation, index + 1) >= 0)
            throw new MarkupParsingException(string.Format(Exceptions.TooManyAssignationSymbols, word, content, _assignation));

        var rawValue = word.Substring(index + 1);
        if (!_specifications.Attributes.QuoteRules.Quoteless && rawValue.Length > 0 && !rawValue.Contains('"') && !rawValue.Contains('\''))
            throw new MarkupParsingException(Exceptions.AttributeValueMustBeQuoted);

        return new MarkupParameter
        {
            Name = word.Substring(0, index),
            Value = rawValue.Trim('"', '\'')
        };
    }

    private static int CountOccurrences(string value, char c)
    {
        var count = 0;
        foreach (var current in value)
            if (current == c) count++;
        return count;
    }

    private StringBuilder RentBuilder()
    {
        if (_builder is null) return _builder = new StringBuilder();
        _builder.Clear();
        return _builder;
    }

    private readonly struct StackFrame(MarkupTag tag, int emissionsAtPush)
    {
        public MarkupTag Tag { get; } = tag;
        public int EmissionsAtPush { get; } = emissionsAtPush;
    }
}
