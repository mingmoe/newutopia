#region

using System.Xml.Serialization;
using Utopia.Shared;

#endregion

namespace Utopia.Core.Translation;

[XmlRoot(nameof(TranslationItems), Namespace = XmlNamespace.Utopia)]
public class TranslationItems
{
    public const string TranslationsElementName = "Translations";

    public const string TranslationItemElementName = "Translation";

    [XmlArray(TranslationsElementName)]
    [XmlArrayItem(TranslationItemElementName)]
    public List<TranslationItem> Translations { get; set; } = [];
}
