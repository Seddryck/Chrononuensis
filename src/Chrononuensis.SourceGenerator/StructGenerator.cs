using System.Diagnostics;
using System.Text;
using Chrononuensis.SourceGenerator.Definitions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Chrononuensis.SourceGenerator;

[Generator]
public class StructGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!Debugger.IsAttached)
        //    Debugger.Launch();

        var yamlFiles = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".yml"))
            .Collect();

        context.RegisterSourceOutput(yamlFiles, (ctx, additionalTexts) =>
        {
            foreach (var textFile in additionalTexts)
            {
                if (textFile.Path.EndsWith("structs.yml", StringComparison.OrdinalIgnoreCase))
                {
                    var yamlContent = textFile.GetText(ctx.CancellationToken)?.ToString();
                    if (!string.IsNullOrWhiteSpace(yamlContent))
                    {
                        try
                        {
                            var structs = ParseYaml(yamlContent!);

                            foreach (var structDefinition in structs)
                            {
                                try
                                {
                                    var generatedCode = GenerateStruct(structDefinition);
                                    ctx.AddSource($"{structDefinition.Name}.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
                                }
                                catch (Exception ex)
                                {
                                    ctx.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                                        "CHRONO002",
                                        "Source Generator Error",
                                        $"Error generating {structDefinition.Name}.g.cs: {ex.Message}",
                                        "ChronoYamlGenerator",
                                        DiagnosticSeverity.Error,
                                        true), Location.None));
                                }

                                try
                                {
                                    var generatedCode = GenerateExtension(structDefinition);
                                    ctx.AddSource($"{structDefinition.Name}Extension.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
                                }
                                catch (Exception ex)
                                {
                                    ctx.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                                        "CHRONO003",
                                        "Source Generator Error",
                                        $"Error generating {structDefinition.Name}Extension.g.cs: {ex.Message}",
                                        "ChronoYamlGenerator",
                                        DiagnosticSeverity.Error,
                                        true), Location.None));
                                }

                                try
                                {
                                    if (structDefinition.Period is not null)
                                    {
                                        var generatedCode = GeneratePeriod(structDefinition);
                                        ctx.AddSource($"{structDefinition.Name}.Period.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ctx.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                                        "CHRONO005",
                                        "Source Generator Error",
                                        $"Error generating {structDefinition.Name}.Period.g.cs: {ex.Message}",
                                        "ChronoYamlGenerator",
                                        DiagnosticSeverity.Error,
                                        true), Location.None));
                                }

                                try
                                {
                                    if (structDefinition.Period is not null)
                                    {
                                        var generatedCode = GeneratePeriodMethods(structDefinition);
                                        ctx.AddSource($"{structDefinition.Name}.Period.Methods.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ctx.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                                        "CHRONO006",
                                        "Source Generator Error",
                                        $"Error generating {structDefinition.Name}.Period.Methods.g.cs: {ex.Message}",
                                        "ChronoYamlGenerator",
                                        DiagnosticSeverity.Error,
                                        true), Location.None));
                                }

                                try
                                {
                                    var generatedCode = GenerateParser(structDefinition);
                                    ctx.AddSource($"{structDefinition.Name}Parser.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
                                }
                                catch (Exception ex)
                                {
                                    ctx.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                                        "CHRONO004",
                                        "Source Generator Error",
                                        $"Error generating {structDefinition.Name}Parser.g.cs: {ex.Message}",
                                        "ChronoYamlGenerator",
                                        DiagnosticSeverity.Error,
                                        true), Location.None));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ctx.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                                "CHRONO001",
                                "YAML Parsing Error",
                                $"Error parsing YAML file: {ex.Message}",
                                "ChronoYamlGenerator",
                                DiagnosticSeverity.Error,
                                true), Location.None));
                        }
                    }
                }
            }
        });
    }

    private static List<StructDefinition> ParseYaml(string yamlContent)
    {
        var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        return deserializer.Deserialize<List<StructDefinition>>(yamlContent);
    }

    internal static string GenerateStruct(StructDefinition structDefinition)
    {
        string templateContent;
        var assembly = typeof(StructGenerator).Assembly;
        var ns = typeof(StructGenerator).Namespace;
        using (var stream = assembly.GetManifestResourceStream($"{ns}.Templates.Struct.scriban-cs"))
        using (var reader = new StreamReader(stream))
            templateContent = reader.ReadToEnd();

        var scribanTemplate = Template.Parse(templateContent);
        var output = scribanTemplate.Render(new
        {
            struct_name = structDefinition.Name,
            parts = structDefinition.Parts.Select(p => new
            {
                name = p.Name,
                type = p.Type,
                min = p.Min,
                max = p.Max,
                max_f = p.MaxF,
                modulo = p.Modulo
            }).ToList()
        });

        return output;
    }

    internal static string GenerateParser(StructDefinition structDefinition)
    {
        string templateContent;
        var assembly = typeof(StructGenerator).Assembly;
        var ns = typeof(StructGenerator).Namespace;
        using (var stream = assembly.GetManifestResourceStream($"{ns}.Templates.Parser.scriban-cs"))
        using (var reader = new StreamReader(stream))
            templateContent = reader.ReadToEnd();

        var scribanTemplate = Template.Parse(templateContent);
        var output = scribanTemplate.Render(new
        {
            struct_name = structDefinition.Name,
            @default = structDefinition.Default,
            parts = structDefinition.Parts.Select(p => new
            {
                name = p.Name,
                type = p.Type,
                min = p.Min,
                max = p.Max,
                max_f = p.MaxF,
                modulo = p.Modulo
            }).ToList()
        });

        return output;
    }

    internal static string GenerateExtension(StructDefinition structDefinition)
    {
        string templateContent;
        var assembly = typeof(StructGenerator).Assembly;
        var ns = typeof(StructGenerator).Namespace;
        using (var stream = assembly.GetManifestResourceStream($"{ns}.Templates.Extension.scriban-cs"))
        using (var reader = new StreamReader(stream))
            templateContent = reader.ReadToEnd();

        var scribanTemplate = Template.Parse(templateContent);
        var output = scribanTemplate.Render(new
        {
            struct_name = structDefinition.Name,
            @default = structDefinition.Default,
            parts = structDefinition.Parts.Select(p => new
            {
                name = p.Name,
                type = p.Type,
                min = p.Min,
                max = p.Max,
                max_f = p.MaxF,
                modulo = p.Modulo
            }).ToList()
        });

        return output;
    }

    internal static string GeneratePeriod(StructDefinition structDefinition)
    {
        if (structDefinition.Period == null)
            throw new ArgumentException("Period is required", nameof(structDefinition));

        string templateContent;
        var assembly = typeof(StructGenerator).Assembly;
        var ns = typeof(StructGenerator).Namespace;
        using (var stream = assembly.GetManifestResourceStream($"{ns}.Templates.Period.scriban-cs"))
        using (var reader = new StreamReader(stream))
            templateContent = reader.ReadToEnd();

        var scribanTemplate = Template.Parse(templateContent);
        var part = structDefinition.Parts?.ElementAtOrDefault(1);
        var output = scribanTemplate.Render(new
        {
            struct_name = structDefinition.Name,
            part = part?.Name,
            by_year = structDefinition.Period.ByYear,
            first = structDefinition.Period.First,
            last = structDefinition.Period.Last,
            year = new
            {
                value = structDefinition.Period.Year,
                duration = structDefinition.Period.YearDuration
            }
        });

        return output;
    }

    internal static string GeneratePeriodMethods(StructDefinition structDefinition)
    {
        if (structDefinition.Period == null)
            throw new ArgumentException("Period is required", nameof(structDefinition));

        string templateContent;
        var assembly = typeof(StructGenerator).Assembly;
        var ns = typeof(StructGenerator).Namespace;
        using (var stream = assembly.GetManifestResourceStream($"{ns}.Templates.Period.Methods.scriban-cs"))
        using (var reader = new StreamReader(stream))
            templateContent = reader.ReadToEnd();

        var scribanTemplate = Template.Parse(templateContent);
        var output = scribanTemplate.Render(new
        {
            struct_name = structDefinition.Name,
        });

        return output;
    }
}
