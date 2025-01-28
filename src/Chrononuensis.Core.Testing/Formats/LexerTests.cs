using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Formats.Tokens.Month;
using Chrononuensis.Formats.Tokens.Quarter;
using Chrononuensis.Formats.Tokens.Semester;
using Chrononuensis.Formats.Tokens.Year;
using NUnit.Framework;

namespace Chrononuensis.Testing.Formats;
public class LexerTests
{
    [Test]
    public void Tokenize_WithLiteral_ReturnsLiteralToken()
    {
        var format = new Lexer().Tokenize("*");
        Assert.That(format.Count(), Is.EqualTo(1));
        Assert.That(format.First(), Is.TypeOf<LiteralToken>());
        Assert.That((format.First() as LiteralToken)!.Value, Is.EqualTo('*'));
    }

    [Test]
    public void Tokenize_WithLiterals_ReturnsLiteralToken()
    {
        var format = new Lexer().Tokenize("*-*");
        Assert.That(format.Count(), Is.EqualTo(3));
        Assert.That(format.First(), Is.TypeOf<LiteralToken>());
        Assert.That((format.First() as LiteralToken)!.Value, Is.EqualTo('*'));
        Assert.That(format.ElementAt(1), Is.TypeOf<LiteralToken>());
        Assert.That((format.ElementAt(1) as LiteralToken)!.Value, Is.EqualTo('-'));
        Assert.That(format.Last(), Is.TypeOf<LiteralToken>());
        Assert.That((format.Last() as LiteralToken)!.Value, Is.EqualTo('*'));
    }

    [TestCase("yy", typeof(DigitOn2YearToken))]
    [TestCase("yyyy", typeof(DigitOn4YearToken))]
    [TestCase("M", typeof(DigitMonthToken))]
    [TestCase("MM", typeof(PaddedDigitMonthToken))]
    [TestCase("MMM", typeof(AbbreviationMonthToken))]
    [TestCase("MMMM", typeof(LabelMonthToken))]
    public void Tokenize_WithTokens_ReturnsLiteralToken(string input, Type expected)
    {
        var format = new Lexer().Tokenize(input);
        Assert.That(format.Count(), Is.EqualTo(1));
        Assert.That(format.First(), Is.Not.TypeOf<LiteralToken>());
        Assert.That(format.First(), Is.TypeOf(expected));
    }

    [TestCase("'*'")]
    [TestCase("'***'")]
    [TestCase("'y'")]
    [TestCase("'yy'")]
    [TestCase("'yy*MMMM'")]
    public void Tokenize_WithQuotedTokens_ReturnsLiteralToken(string input)
    {
        var format = new Lexer().Tokenize(input);
        Assert.That(format.Count(), Is.EqualTo(input.Length-2));
        Assert.That(format, Has.All.TypeOf<LiteralToken>());
    }

    [TestCase("yyyyMM")]
    public void Tokenize_WithDistinctTokens_ReturnsTokens(string input)
    {
        var format = new Lexer().Tokenize(input);
        Assert.That(format.Count(), Is.EqualTo(2));
        Assert.That(format.First(), Is.TypeOf<DigitOn4YearToken>());
        Assert.That(format.Last(), Is.TypeOf<PaddedDigitMonthToken>());
    }

    [TestCase("yyyy-MM")]
    [TestCase("yyyy'-'MM")]
    public void Tokenize_WithThreeDistinctTokens_ReturnsTokens(string input)
    {
        var format = new Lexer().Tokenize(input);
        Assert.That(format.Count(), Is.EqualTo(3));
        Assert.That(format.First(), Is.TypeOf<DigitOn4YearToken>());
        Assert.That(format.ElementAt(1), Is.TypeOf<LiteralToken>());
        Assert.That(format.Last(), Is.TypeOf<PaddedDigitMonthToken>());
    }

    [TestCase("yyyy'-Q'q")]
    public void Tokenize_WithFourDistinctTokens_ReturnsTokens(string input)
    {
        var format = new Lexer().Tokenize(input);
        Assert.That(format.Count(), Is.EqualTo(4));
        Assert.That(format.First(), Is.TypeOf<DigitOn4YearToken>());
        Assert.That(format.ElementAt(1), Is.TypeOf<LiteralToken>());
        Assert.That(format.ElementAt(2), Is.TypeOf<LiteralToken>());
        Assert.That(format.Last(), Is.TypeOf<DigitQuarterToken>());
    }
}
