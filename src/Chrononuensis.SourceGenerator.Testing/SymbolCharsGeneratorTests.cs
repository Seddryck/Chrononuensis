using System.Reflection;
using Chrononuensis.SourceGenerator.Definitions;
using NUnit.Framework;

namespace Chrononuensis.SourceGenerator.Testing;

public class SymbolCharsGeneratorTests
{
    private static string ReadEmbeddedFile(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fullResourceName = assembly.GetManifestResourceNames()
                                       .FirstOrDefault(name => name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
                                       ?? throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

        using var stream = assembly.GetManifestResourceStream(fullResourceName)
            ?? throw new FileNotFoundException($"Failed to load embedded resource '{fullResourceName}'.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    [Test]
    public void GenerateSymbolChars_FewItems_Expected()
    {
        var generator = new SymbolCharsGenerator();
        var output = generator.GenerateSymbolCharsImplementation(['a', 'b', 'c']);
        Assert.That(ReadEmbeddedFile("Lexer.cs").Replace("\r\n", "\n"), Is.EqualTo(output.Replace("\r\n", "\n")));
    }
}
