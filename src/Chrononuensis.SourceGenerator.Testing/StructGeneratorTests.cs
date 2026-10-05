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

        Assert.That(output.Replace("\r\n", "\n"), Is.EqualTo(ReadEmbeddedFile("YearMonth.cs").Replace("\r\n", "\n")));
    }

    [Test]
    public void GenerateParser_YearMonth_Expected()
    {
        var generator = new StructGenerator();
        var output = StructGenerator.GenerateParser(
            new StructDefinition()
            {
                Name = "YearMonth",
                Default = "yyyy-MM",
                Parts =
                [
                    new() {Name = "Year", Type = "int" }
                    , new() {Name = "Month", Type = "int", Min = 1 , Max = 12 }
                ]
            });

        Assert.That(output.Replace("\r\n", "\n"), Is.EqualTo(ReadEmbeddedFile("YearMonthParser.cs").Replace("\r\n", "\n")));
    }

    [Test]
    public void GenerateStruct_Period_UsesExplicitBoundsEquality()
    {
        var output = StructGenerator.GenerateStruct(
            new StructDefinition()
            {
                Name = "YearMonth",
                Parts =
                [
                    new() { Name = "Year", Type = "int" },
                    new() { Name = "Month", Type = "int", Min = 1, Max = 12 }
                ],
                Period = new StructPeriod { ByYear = 12 }
            });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(output, Does.Contain("public readonly partial struct YearMonth"));
            Assert.That(output, Does.Contain("public int Year { get; } = Year;"));
            Assert.That(output, Does.Contain("public int Month { get; }")
                .And.Not.Contain("public int Month { get; init; }"));
            Assert.That(output, Does.Contain("public override bool Equals(object? obj) => obj is IPeriod other && Equals(other);"));
            Assert.That(output, Does.Contain("public override int GetHashCode() => HashCode.Combine(FirstDate, LastDate);"));
        }
    }

    [Test]
    public void GenerateExtension_UpdatesThroughValidatedConstructor()
    {
        var output = StructGenerator.GenerateExtension(
            new StructDefinition()
            {
                Name = "YearMonth",
                Parts =
                [
                    new() { Name = "Year", Type = "int" },
                    new() { Name = "Month", Type = "int", Min = 1, Max = 12 }
                ]
            });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(output, Does.Contain("return new YearMonth(value.Year + cycles, normalized);"));
            Assert.That(output, Does.Contain("=> new YearMonth(value.Year + year, value.Month);"));
            Assert.That(output, Does.Not.Contain("with"));
        }
    }
}
