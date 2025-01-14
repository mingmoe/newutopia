using FluentAssertions;
using MemoryPack;
using System.Text.RegularExpressions;
using Utopia.Shared;

namespace Utopia.Core.Test;

public class GuuidTest
{
    private readonly Regex _pattern = new Regex(GuuidStandard.Pattern);

    [Fact]
    public void TestGuuidToStringAndParseStringWorksWell()
    {
        var guuid = new Guuid("root", "node");
        var str = guuid.ToString();

        var parsed = Guuid.Parse(str);

        guuid.Should().BeEquivalentTo(parsed);
        guuid.GetHashCode().Should().Be(parsed.GetHashCode());
        this._pattern.Match(guuid.ToString()).Success.Should().BeTrue();
    }

    [Fact]
    public void GuuidEqualTest()
    {
        Guuid one = new("root", "one");
        Guuid oneToo = new("root", "one");
        Guuid two = new("root", "two");

        one.Should().Equal(one);
        one.Should().Equal(oneToo);
        one.Should().NotEqual(two);
        oneToo.Should().NotEqual(two);
    }

    [Fact]
    public void CheckIllegalNames()
    {
        var parsed = Guuid.CheckName(string.Empty);

        parsed.Should().BeFalse();
    }

    [Fact]
    public void CheckMemoryPack()
    {
        var guuid = new Guuid("root", "node");

        var bytes = MemoryPackSerializer.Serialize(guuid);

        MemoryPackSerializer.Deserialize<Guuid>(bytes).Should().BeEquivalentTo(guuid);
    }

    [Theory]
    [InlineData("", "nonempty")]
    [InlineData(null, "nonempty")]
    [InlineData("nonempty", "")]
    [InlineData("nonempty", null)]
    [InlineData("nonempty", "1startWithNumber")]
    [InlineData("1startWithNumber", "nonempty")]
    [InlineData("withUnderLine_", "nonempty")]
    [InlineData("nonempty", "withUnderLine_")]
    public void TestGuuidParseStringParseIllegal(string? root, string? node)
    {
        var lambda = () => { _ = new Guuid(root!, node!); };
        lambda.Should().ThrowExactly<FormatException>();
    }

    [Theory]
    [InlineData("a", new[] { "b" })]
    [InlineData("a", new[] { "b", "c" })]
    [InlineData("a", new[] { "b", "c", "d" })]
    public void TestGuuidChildCheckFailureMethod(string root, params string[] fatherNodes)
    {
        var father = new Guuid(root, fatherNodes);
        var child = new Guuid("a", "b", "c", "d");

        var success = father.HasChild(child).Should().BeTrue();
    }

    [Theory]
    [InlineData("a", new[] { "b" })]
    [InlineData("a", new[] { "b", "c" })]
    public void TestGuuidChildCheckMethod(string root, params string[] fatherNodes)
    {
        var father = new Guuid(root, fatherNodes);
        var child = new Guuid("a", "b", "c", "d");

        var failure = child.HasChild(father).Should().BeFalse();
    }

    [Fact]
    public void TestGuuidGetParent()
    {
        var guuid = new Guuid("a", "b", "c", "d");

        new Guuid("a", "b", "c").Should().Equal(guuid.GetParent());
        new Guuid("a", "b").Should().Equal(guuid.GetParent()!.Value.GetParent());
        guuid.GetParent()!.Value.GetParent()!.Value.GetParent().Should().BeNull();
    }
}
