using FluentAssertions;
using Xunit;

namespace IndentedStringBuilder;

public class StringBuilderScopeTests
{
    [Fact]
    public void OneLevelScope_Should_ActLikeStringBuilder()
    {
        using StringBuilderScope sb = new();
        sb.Append("abc");
        sb.Append("xyz");
        sb.ToString().Should().Be("abcxyz");
    }

    [Fact]
    public void InnerScope_Should_BeIntended()
    {
        using StringBuilderScope sb = new();

        sb.Append("for (var i = 0; i < array.length; i++)");

        using (StringBuilderScope forBody = new(sb, new(indent: "  ", blockStart: " {\n", blockEnd: "\n}\n")))
        {
            forBody.AppendLine("sum += array[i];");
        }

        sb.AppendLine("console.log(sum);");

        sb.ToString().Should().Be("for (var i = 0; i < array.length; i++) {\n  sum += array[i];\n}\nconsole.log(sum);\n");
    }
}


