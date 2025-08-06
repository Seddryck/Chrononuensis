using System;
using System.Globalization;
using System.Resources;

namespace Chrononuensis.Localization;

public static class LocalizedStrings
{
    private static readonly ResourceManager _resourceManager =
        new ("Chrononuensis.Localization.Labels", typeof(Labels).Assembly);

    public static string Get(string key, CultureInfo? culture = null)
    {
        var value = _resourceManager.GetString(key, culture ?? CultureInfo.CurrentUICulture);

        return value ?? throw new MissingManifestResourceException(
            $"Resource key '{key}' not found for culture '{(culture ?? CultureInfo.CurrentUICulture).Name}'.");
    }
}

