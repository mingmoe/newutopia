#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Utopia.Core.Translation;

/// <summary>
///     默认翻译管理器，线程安全。
/// </summary>
public class TranslationManager : ITranslationManager
{
    private readonly Lock _lock = new();
    private readonly List<ITranslationProvider> _providers = [];

    public bool TryGetTranslation(LanguageID language, string item, [NotNullWhen(true)] out string? result)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(item);

        lock (this._lock)
        {
            foreach (var provider in this._providers)
                if (provider.TryGetItem(language, item, out result))
                    return true;

            result = null;
            return false;
        }
    }

    public void AddTranslationProvider(ITranslationProvider provider)
    {
        lock (this._lock)
        {
            if (this._providers.Contains(provider)) return;
            this._providers.Add(provider);
        }
    }
}
