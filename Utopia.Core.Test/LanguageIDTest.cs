#region

using FluentAssertions;
using Utopia.Core.Translation;

#endregion

namespace Utopia.Core.Test.Translation;

public class LanguageIDTest
{
    [Fact]
    public void ConstructTest()
    {
        _ = new LanguageID("aa", "aa");
    }

    [Fact]
    public void BadConstructTest()
    {
        var lambda = () => new LanguageID(string.Empty, "aa");
        lambda.Should().ThrowExactly<FormatException>();
        lambda = () => new LanguageID(null!, "aa");
        lambda.Should().ThrowExactly<ArgumentNullException>();
        lambda = () => new LanguageID("a", "aa");
        lambda.Should().ThrowExactly<FormatException>();
        lambda = () => new LanguageID("a", "aaa");
        lambda.Should().ThrowExactly<FormatException>();
        lambda = () => new LanguageID("aaa", "aaaa");
        lambda.Should().ThrowExactly<FormatException>();
        lambda = () => new LanguageID("aa", "aaa");
        lambda.Should().ThrowExactly<FormatException>();
    }

    [Fact]
    public void ParseTest()
    {
        LanguageID.Parse("aa-aa");
        LanguageID.Parse("aa_aa");
        LanguageID.Parse("aa aa");
    }

    [Fact]
    public void BadParseTest()
    {
        LanguageID.TryParse("aaa-aa",out _).Should().BeFalse();
        LanguageID.TryParse("aa_a",out _).Should().BeFalse();
        LanguageID.TryParse("aa+aa",out _).Should().BeFalse();
        LanguageID.TryParse("aa=aa",out _).Should().BeFalse();
        LanguageID.TryParse("aa  aa",out _).Should().BeFalse();
    }

    [Fact]
    public void EqualTest()
    {
        LanguageID one = new("zh", "cn");
        LanguageID two = new("Zh", "cN");

        Assert.Equal(one.Language, two.Language);
        Assert.Equal(one.Location, two.Location);
        Assert.Equal(one, two);
        Assert.True(one == two);
        Assert.False(one != two);
        Assert.Equal(one.GetHashCode(), two.GetHashCode());
        Assert.Equal(one.ToString(), two.ToString());
    }

    [Fact]
    public void NotEqualTest()
    {
        LanguageID one = new("zH", "Cn");
        LanguageID two = new("EN", "us");

        Assert.NotEqual(one.Language, two.Language);
        Assert.NotEqual(one.Location, two.Location);
        Assert.NotEqual(one, two);
        Assert.False(one == two);
        Assert.True(one != two);
        Assert.NotEqual(one.GetHashCode(), two.GetHashCode());
        Assert.NotEqual(one.ToString(), two.ToString());
    }
}
