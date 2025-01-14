#region

using System.Xml.Serialization;

#endregion

namespace Utopia.Core.Translation;

/// <summary>
///     This class was used for code generated and human edit.
/// </summary>
public sealed class TranslationItem
{
    /// <summary>
    ///     The translation id of this item.
    /// </summary>
    [XmlElement]
    public string Text { get; set; } = "";

    /// <summary>
    ///     The translation comment of this id.
    /// </summary>
    [XmlElement]
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    ///     The translated of this id.
    /// </summary>
    [XmlElement]
    public string Translated { get; set; } = string.Empty;
}
