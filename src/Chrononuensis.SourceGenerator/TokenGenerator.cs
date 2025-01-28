﻿using System;
using System.Linq;
using System.Text;
using Chrononuensis.SourceGenerator.Definitions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace Chrononuensis.SourceGenerator;

[Generator]
public class TokenGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var yamlFiles = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".yml"))
            .Collect();

        context.RegisterSourceOutput(yamlFiles, (ctx, additionalTexts) =>
        {
            foreach (var textFile in additionalTexts)
            {
                if (textFile.Path.EndsWith("tokens.yml", StringComparison.OrdinalIgnoreCase))
                {
                    var yamlContent = textFile.GetText(ctx.CancellationToken)?.ToString();
                    if (!string.IsNullOrWhiteSpace(yamlContent))
                    {
                        try
                        {
                            var tokens = ParseYaml(yamlContent!);
                            foreach (var tokenDef in tokens)
                            {
                                var generatedCode = GenerateIToken(tokenDef.Group);
                                ctx.AddSource($"I{tokenDef.Group}Token.g.cs", SourceText.From(generatedCode, Encoding.UTF8));

                                foreach (var tokenMember in tokenDef.Members)
                                {
                                    generatedCode = GenerateToken(tokenDef.Group, tokenMember);
                                    ctx.AddSource($"{tokenMember.Name}{tokenDef.Group}Token.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
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

    private static List<TokenDefinition> ParseYaml(string yamlContent)
    {
        var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        return deserializer.Deserialize<List<TokenDefinition>>(yamlContent);
    }

    internal string GenerateToken(string group, TokenMember token)
    {
        string templateContent;
        var assembly = typeof(TokenGenerator).Assembly;
        var ns = typeof(TokenGenerator).Namespace;
        using (var stream = assembly.GetManifestResourceStream($"{ns}.Templates.Token.scriban"))
        using (var reader = new StreamReader(stream))
            templateContent = reader.ReadToEnd();

        var scribanTemplate = Template.Parse(templateContent);
        var output = scribanTemplate.Render(new
        {
            group,
            token,
        });

        return output;
    }

    internal string GenerateIToken(string group)
    {
        string templateContent;
        var assembly = typeof(TokenGenerator).Assembly;
        var ns = typeof(TokenGenerator).Namespace;
        using (var stream = assembly.GetManifestResourceStream($"{ns}.Templates.IToken.scriban"))
        using (var reader = new StreamReader(stream))
            templateContent = reader.ReadToEnd();

        var scribanTemplate = Template.Parse(templateContent);
        var output = scribanTemplate.Render(new
        {
            group,
        });

        return output;
    }
}
