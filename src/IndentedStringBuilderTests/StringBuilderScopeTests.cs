using FluentAssertions;
using Xunit;

namespace IndentedStringBuilder;

public class StringBuilderScopeTests
{
    [Fact]
    public void OneLevelScope_Should_ActLikeStringBuilder()
    {
        var sb = new StringBuilderScope();
        sb.Append("abc");
        sb.Append("xyz");
        sb.ToString().Should().Be("abcxyz");
    }
}


