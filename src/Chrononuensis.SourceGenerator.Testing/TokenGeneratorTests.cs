using System.Reflection;
using Chrononuensis.SourceGenerator.Definitions;
using NUnit.Framework;

namespace Chrononuensis.SourceGenerator.Testing;

public class TokenGeneratorTests
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
    public void GenerateToken_MonthAbbreviation_Expected()
    {
        var generator = new TokenGenerator();
        var output = generator.GenerateToken(
            "Month", 
            new TokenMember()
            {
                Name = "Abbreviation",
                Pattern = "MMM"
            });

        Assert.That(ReadEmbeddedFile("AbbreviationMonthToken.cs").Replace("\r\n", "\n"), Is.EqualTo(output.Replace("\r\n", "\n")));
    }

    [Test]
    public void GenerateIToken_MonthAbbreviation_Expected()
    {
        var generator = new TokenGenerator();
        var output = generator.GenerateIToken("Month");

        Assert.That(ReadEmbeddedFile("IMonthToken.cs").Replace("\r\n", "\n"), Is.EqualTo(output.Replace("\r\n", "\n")));
    }


    [Test]
    public void GenerateTokenMapper_MonthAbbreviation_Expected()
    {
        var generator = new TokenGenerator();
        var output = generator.GenerateTokenMapper(
            [
                new TokenDefinition()
                {
                    Group = "Day",
                    Members = new List<TokenMember>(new[]
                    {
                        new TokenMember()
                        {
                            Name = "Digit",
                            Pattern = "d"
                        },
                        new TokenMember()
                        {
                            Name = "PaddedDigit",
                            Pattern = "dd"
                        }
                    })
                },
                new TokenDefinition()
                {
                    Group = "DayOfYear",
                    Members = new List<TokenMember>(new[]
                    {
                        new TokenMember()
                        {
                            Name = "Digit",
                            Pattern = "j"
                        },
                        new TokenMember()
                        {
                            Name = "PaddedDigit",
                            Pattern = "jjj"
                        }
                    })
                }
            ]);


        Assert.That(ReadEmbeddedFile("TokenMapper.cs").Replace("\r\n", "\n"), Is.EqualTo(output.Replace("\r\n", "\n")));
    }
}
