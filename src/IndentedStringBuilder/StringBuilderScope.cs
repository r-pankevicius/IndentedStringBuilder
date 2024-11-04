using System;
using System.Text;

namespace IndentedStringBuilder;

public readonly struct StringBuilderScopeOptions
{
    public StringBuilderScopeOptions(
        string indent, string blockStart = "", string blockEnd = "")
    {
        Indent = indent;
        BlockStart = blockStart;
        BlockEnd = blockEnd;
    }

    public string Indent { get; init; }
    public string BlockStart { get; init; }
    public string BlockEnd { get; init; }
}

public class StringBuilderScope : IDisposable
{
    private readonly StringBuilderScopeOptions _options;
    private readonly StringBuilder _sb;
    private readonly StringBuilderScope? _previousScope;
    private bool _isDisposed;

    /// <summary>
    /// Initial <see cref="StringBuilder"/> wrapper that has no indent.
    /// </summary>
    public StringBuilderScope() : this(options: new(string.Empty))
    {
    }

    public StringBuilderScope(StringBuilderScopeOptions options)
    {
        _options = options;
        _sb = new();
        _previousScope = null;
    }

    public StringBuilderScope(
        StringBuilderScope previousScope, StringBuilderScopeOptions options) : this(options)
    {
        _previousScope = previousScope;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing && _previousScope is not null)
            {
                _previousScope.AppendScope(this);
            }

            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public override string ToString()
    {
        // TODO: append block start,
        // create text reader over text read each line, append current Indent,
        // append block end
        return base.ToString();
    }

    private void AppendScope(StringBuilderScope childScope)
    {
        string scopedText = childScope.ToString();
        // TODO: create text reader over text read each line, append current Indent
        _sb.Append(scopedText);
    }
 }
