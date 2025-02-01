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
                                var generatedCode = GenerateStruct(structDefinition);
                                ctx.AddSource($"{structDefinition.Name}.g.cs", SourceText.From(generatedCode, Encoding.UTF8));

                                generatedCode = GenerateParser(structDefinition);
                                ctx.AddSource($"{structDefinition.Name}Parser.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
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
        using (var stream = assembly.GetManifestResourceStream($"{ns}.Templates.Struct.scriban"))
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
                max = p.Max
            }).ToList()
        });

        return output;
    }

    internal static string GenerateParser(StructDefinition structDefinition)
    {
        string templateContent;
        var assembly = typeof(StructGenerator).Assembly;
        var ns = typeof(StructGenerator).Namespace;
        using (var stream = assembly.GetManifestResourceStream($"{ns}.Templates.Parser.scriban"))
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
                max = p.Max
            }).ToList()
        });

        return output;
    }
}
