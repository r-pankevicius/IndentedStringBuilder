using System;
using System.Collections.Generic;
using System.IO;
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
        StringBuilder sb = new(_options.BlockStart);

        if (string.IsNullOrEmpty(_options.Indent))
        {
            sb.Append(_sb.ToString());
        }
        else
        {
            string text = _sb.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                // Append Indent to each line
                using TextReader reader = new StringReader(text);
                string? line;
                while ((line = reader.ReadLine()) is not null)
                {
                    // TODO: don't AppendLine for last line!
                    sb.Append(_options.Indent).AppendLine(line);
                }
            }
        }

        sb.Append(_options.BlockEnd);

        return sb.ToString();
    }

    private void AppendScope(StringBuilderScope childScope)
    {
        string scopedText = childScope.ToString();
        // TODO: create text reader over text read each line, append current Indent
        _sb.Append(scopedText);
    }

    /// <summary>Appends the string representation of a specified Boolean value to this instance.</summary>
    /// <param name="value">The Boolean value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(bool value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified 8-bit unsigned integer to this instance.</summary>
    /// <param name="value">The value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(byte value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified <see cref="T:System.Char" /> object to this instance.</summary>
    /// <param name="value">The UTF-16-encoded code unit to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(char value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends an array of Unicode characters starting at a specified address to this instance.</summary>
    /// <param name="value">A pointer to an array of characters.</param>
    /// <param name="valueCount">The number of characters in the array.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref name="valueCount" /> is less than zero.  
    /// -or-  
    /// Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    /// <exception cref="T:System.NullReferenceException">
    ///   <paramref name="value" /> is a null pointer.</exception>
    public unsafe StringBuilderScope Append(char* value, int valueCount)
    {
        _sb.Append(value, valueCount);
        return this;
    }

    /// <summary>Appends a specified number of copies of the string representation of a Unicode character to this instance.</summary>
    /// <param name="value">The character to append.</param>
    /// <param name="repeatCount">The number of times to append <paramref name="value" />.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref name="repeatCount" /> is less than zero.  
    /// -or-  
    /// Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">Out of memory.</exception>
    public StringBuilderScope Append(char value, int repeatCount)
    {
        _sb.Append(value, repeatCount);
        return this;
    }

    /// <summary>Appends the string representation of the Unicode characters in a specified array to this instance.</summary>
    /// <param name="value">The array of characters to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(char[] value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified subarray of Unicode characters to this instance.</summary>
    /// <param name="value">A character array.</param>
    /// <param name="startIndex">The starting position in <paramref name="value" />.</param>
    /// <param name="charCount">The number of characters to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is <see langword="null" />, and <paramref name="startIndex" /> and <paramref name="charCount" /> are not zero.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref name="charCount" /> is less than zero.  
    /// -or-  
    /// <paramref name="startIndex" /> is less than zero.  
    /// -or-  
    /// <paramref name="startIndex" /> + <paramref name="charCount" /> is greater than the length of <paramref name="value" />.  
    /// -or-  
    /// Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(char[] value, int startIndex, int charCount)
    {
        _sb.Append(value, startIndex, charCount);
        return this;
    }

    /// <summary>Appends the string representation of a specified decimal number to this instance.</summary>
    /// <param name="value">The value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(decimal value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified double-precision floating-point number to this instance.</summary>
    /// <param name="value">The value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(double value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified 16-bit signed integer to this instance.</summary>
    /// <param name="value">The value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(short value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified 32-bit signed integer to this instance.</summary>
    /// <param name="value">The value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(int value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified 64-bit signed integer to this instance.</summary>
    /// <param name="value">The value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(long value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified object to this instance.</summary>
    /// <param name="value">The object to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(object value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified read-only character span to this instance.</summary>
    /// <param name="value">The read-only character span to append.</param>
    /// <returns>A reference to this instance after the append operation is completed.</returns>
    public StringBuilderScope Append(ReadOnlySpan<char> value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified 8-bit signed integer to this instance.</summary>
    /// <param name="value">The value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(sbyte value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified single-precision floating-point number to this instance.</summary>
    /// <param name="value">The value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(float value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends a copy of the specified string to this instance.</summary>
    /// <param name="value">The string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(string value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends a copy of a specified substring to this instance.</summary>
    /// <param name="value">The string that contains the substring to append.</param>
    /// <param name="startIndex">The starting position of the substring within <paramref name="value" />.</param>
    /// <param name="count">The number of characters in <paramref name="value" /> to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is <see langword="null" />, and <paramref name="startIndex" /> and <paramref name="count" /> are not zero.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref name="count" /> less than zero.  
    /// -or-  
    /// <paramref name="startIndex" /> less than zero.  
    /// -or-  
    /// <paramref name="startIndex" /> + <paramref name="count" /> is greater than the length of <paramref name="value" />.  
    /// -or-  
    /// Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(string value, int startIndex, int count)
    {
        _sb.Append(value, startIndex, count);
        return this;
    }

    /// <summary>Appends the string representation of a specified string builder to this instance.</summary>
    /// <param name="value">The string builder to append.</param>
    /// <returns>A reference to this instance after the append operation is completed.</returns>
    public StringBuilderScope Append(StringBuilder value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends a copy of a substring within a specified string builder to this instance.</summary>
    /// <param name="value">The string builder that contains the substring to append.</param>
    /// <param name="startIndex">The starting position of the substring within <paramref name="value" />.</param>
    /// <param name="count">The number of characters in <paramref name="value" /> to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    public StringBuilderScope Append(StringBuilder value, int startIndex, int count)
    {
        _sb.Append(value, startIndex, count);
        return this;
    }

    /// <summary>Appends the string representation of a specified 16-bit unsigned integer to this instance.</summary>
    /// <param name="value">The value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(ushort value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified 32-bit unsigned integer to this instance.</summary>
    /// <param name="value">The value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(uint value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string representation of a specified 64-bit unsigned integer to this instance.</summary>
    /// <param name="value">The value to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope Append(ulong value)
    {
        _sb.Append(value);
        return this;
    }

    /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument using a specified format provider.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">The object to format.</param>
    /// <returns>A reference to this instance after the append operation has completed. After the append operation, this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format" /> in which any format specification is replaced by the string representation of <paramref name="arg0" />.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///   <paramref name="format" /> is invalid.  
    /// -or-  
    /// The index of a format item is less than 0 (zero), or greater than or equal to one (1).</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope AppendFormat(IFormatProvider provider, string format, object arg0)
    {
        _sb.AppendFormat(provider, format, arg0);
        return this;
    }

    /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of either of two arguments using a specified format provider.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">The first object to format.</param>
    /// <param name="arg1">The second object to format.</param>
    /// <returns>A reference to this instance after the append operation has completed. After the append operation, this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format" /> where any format specification is replaced by the string representation of the corresponding object argument.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///   <paramref name="format" /> is invalid.  
    /// -or-  
    /// The index of a format item is less than 0 (zero), or greater than or equal to 2 (two).</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope AppendFormat(IFormatProvider provider, string format, object arg0, object arg1)
    {
        _sb.AppendFormat(provider, format, arg0, arg1);
        return this;
    }

    /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of either of three arguments using a specified format provider.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">The first object to format.</param>
    /// <param name="arg1">The second object to format.</param>
    /// <param name="arg2">The third object to format.</param>
    /// <returns>A reference to this instance after the append operation has completed. After the append operation, this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format" /> where any format specification is replaced by the string representation of the corresponding object argument.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///   <paramref name="format" /> is invalid.  
    /// -or-  
    /// The index of a format item is less than 0 (zero), or greater than or equal to 3 (three).</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope AppendFormat(IFormatProvider provider, string format, object arg0, object arg1, object arg2)
    {
        _sb.AppendFormat(provider, format, arg0, arg1, arg2);
        return this;
    }

    /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to format.</param>
    /// <returns>A reference to this instance after the append operation has completed. After the append operation, this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format" /> where any format specification is replaced by the string representation of the corresponding object argument.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///   <paramref name="format" /> is invalid.  
    /// -or-  
    /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args" /> array.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope AppendFormat(IFormatProvider provider, string format, params object[] args)
    {
        _sb.AppendFormat(provider, format, args);
        return this;
    }

    /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument.</summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">An object to format.</param>
    /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of <paramref name="arg0" />.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///   <paramref name="format" /> is invalid.  
    /// -or-  
    /// The index of a format item is less than 0 (zero), or greater than or equal to 1.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope AppendFormat(string format, object arg0)
    {
        _sb.AppendFormat(format, arg0);
        return this;
    }

    /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of either of two arguments.</summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">The first object to format.</param>
    /// <param name="arg1">The second object to format.</param>
    /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of the corresponding object argument.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///   <paramref name="format" /> is invalid.  
    /// -or-  
    /// The index of a format item is less than 0 (zero), or greater than or equal to 2.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope AppendFormat(string format, object arg0, object arg1)
    {
        _sb.AppendFormat(format, arg0, arg1);
        return this;
    }

    /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of either of three arguments.</summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">The first object to format.</param>
    /// <param name="arg1">The second object to format.</param>
    /// <param name="arg2">The third object to format.</param>
    /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of the corresponding object argument.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///   <paramref name="format" /> is invalid.  
    /// -or-  
    /// The index of a format item is less than 0 (zero), or greater than or equal to 3.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope AppendFormat(string format, object arg0, object arg1, object arg2)
    {
        _sb.AppendFormat(format, arg0, arg1, arg2);
        return this;
    }

    /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to format.</param>
    /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of the corresponding object argument.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="format" /> or <paramref name="args" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///   <paramref name="format" /> is invalid.  
    /// -or-  
    /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args" /> array.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope AppendFormat(string format, params object[] args)
    {
        _sb.AppendFormat(format, args);
        return this;
    }
    public StringBuilderScope AppendJoin(char separator, params object[] values)
    {
        _sb.AppendJoin(separator, values);
        return this;
    }

    public StringBuilderScope AppendJoin(char separator, params string[] values)
    {
        _sb.AppendJoin(separator, values);
        return this;
    }

    public StringBuilderScope AppendJoin(string separator, params object[] values)
    {
        _sb.AppendJoin(separator, values);
        return this;
    }

    public StringBuilderScope AppendJoin(string separator, params string[] values)
    {
        _sb.AppendJoin(separator, values);
        return this;
    }

    public StringBuilderScope AppendJoin<T>(char separator, IEnumerable<T> values)
    {
        _sb.AppendJoin(separator, values);
        return this;
    }

    public StringBuilderScope AppendJoin<T>(string separator, IEnumerable<T> values)
    {
        _sb.AppendJoin(separator, values);
        return this;
    }

    /// <summary>Appends the default line terminator to the end of the current <see cref="T:System.Text.StringBuilder" /> object.</summary>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope AppendLine()
    {
        _sb.AppendLine();
        return this;
    }

    /// <summary>Appends a copy of the specified string followed by the default line terminator to the end of the current <see cref="T:System.Text.StringBuilder" /> object.</summary>
    /// <param name="value">The string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />.</exception>
    public StringBuilderScope AppendLine(string value)
    {
        _sb.AppendLine(value);
        return this;
    }
}
