using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Localization;
using NUnit.Framework;
using System.Resources;

namespace Chrononuensis.Testing.Localization;
public class LocalizedStringsTests
{
    [TestCase("en-us", "Olympiad")]
    [TestCase("fr-fr", "Olympiade")]
    [TestCase("es-es", "Olimpiada")]
    [TestCase("jp-jp", "Olympiad")]
    public void Get_ReturnsLocalizedString_WhenKeyExists(string culture, string expected)
        => Assert.That(LocalizedStrings.Get("Olympiad", new System.Globalization.CultureInfo(culture)), Is.EqualTo(expected));

    [TestCase("NonExistentKey")]
    public void Get_ThrowsMissingManifestResourceException_WhenKeyDoesNotExist(string key)
    {
        var ex = Assert.Throws<MissingManifestResourceException>(() => LocalizedStrings.Get(key, new System.Globalization.CultureInfo("en-us")));
        Assert.That(ex.Message, Does.Contain($"Resource key '{key}' not found"));
    }
}
