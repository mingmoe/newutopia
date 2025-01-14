#region

using System.Collections.Frozen;

#endregion

namespace Utopia.Core.Translation;

public sealed class TranslationProject
{
    public required LanguageID Identification { get; init; }

    public required FrozenDictionary<string, string> Items { get; init; }
}
