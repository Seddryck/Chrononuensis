using System.Reflection;
using Chrononuensis.SourceGenerator.Definitions;
using NUnit.Framework;

namespace Chrononuensis.SourceGenerator.Testing;

public class StructGeneratorTest
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
    public void GenerateStruct_YearMonth_Expected()
    {
        var generator = new StructGenerator();
        var output = StructGenerator.GenerateStruct(
            new StructDefinition()
            {
                Name = "YearMonth",
                Parts =
                [
                    new() {Name = "Year", Type = "int" }
                    , new() {Name = "Month", Type = "int", Min = 1 , Max = 12 }
                ]
            });

        Assert.That(ReadEmbeddedFile("YearMonth.cs").Replace("\r\n", "\n"), Is.EqualTo(output.Replace("\r\n", "\n")));
    }
}
