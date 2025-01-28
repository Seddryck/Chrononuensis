using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Chrononuensis.SourceGenerator;

[Generator]
public class SymbolCharsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Define the incremental value provider for symbol characters
        var symbolCharsProvider = context.CompilationProvider
            .Select((compilation, _) => new[] { 'y', 'S', 'q', 'M', 'd', 'h' });

        // Register the source generation step
        context.RegisterSourceOutput(symbolCharsProvider, (context, symbolChars) =>
        {
            // Generate and add the source
            var source = GenerateSymbolCharsImplementation(symbolChars);
            context.AddSource("Lexer.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }

    internal string GenerateSymbolCharsImplementation(char[] symbolChars)
    {
        string templateContent;
        var assembly = typeof(StructGenerator).Assembly;
        var ns = typeof(StructGenerator).Namespace;
        using (var stream = assembly.GetManifestResourceStream($"{ns}.Templates.SymbolChars.scriban"))
        using (var reader = new StreamReader(stream))
            templateContent = reader.ReadToEnd();

        var scribanTemplate = Template.Parse(templateContent);
        var output = scribanTemplate.Render(new
        {
            symbol_chars = symbolChars
        });

        return output;
    }
}
