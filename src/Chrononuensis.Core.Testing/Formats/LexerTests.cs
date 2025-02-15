using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Formats.Tokens.Day;
using Chrononuensis.Formats.Tokens.DayOfYear;
using Chrononuensis.Formats.Tokens.Week;
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
        Assert.That((format.First() as LiteralToken)!.Value, Is.EqualTo("*"));
    }

    [Test]
    public void Tokenize_WithLiterals_ReturnsLiteralToken()
    {
        var format = new Lexer().Tokenize("*-*");
        Assert.That(format.Count(), Is.EqualTo(3));
        Assert.That(format.First(), Is.TypeOf<LiteralToken>());
        Assert.That((format.First() as LiteralToken)!.Value, Is.EqualTo("*"));
        Assert.That(format.ElementAt(1), Is.TypeOf<LiteralToken>());
        Assert.That((format.ElementAt(1) as LiteralToken)!.Value, Is.EqualTo("-"));
        Assert.That(format.Last(), Is.TypeOf<LiteralToken>());
        Assert.That((format.Last() as LiteralToken)!.Value, Is.EqualTo("*"));
    }

    [TestCase("yy", typeof(DigitOn2YearToken))]
    [TestCase("yyyy", typeof(DigitOn4YearToken))]
    [TestCase("M", typeof(DigitMonthToken))]
    [TestCase("MM", typeof(PaddedDigitMonthToken))]
    [TestCase("MMM", typeof(AbbreviationMonthToken))]
    [TestCase("MMMM", typeof(LabelMonthToken))]
    [TestCase("q", typeof(DigitQuarterToken))]
    [TestCase("S", typeof(DigitSemesterToken))]
    [TestCase("j", typeof(DigitDayOfYearToken))]
    [TestCase("w", typeof(DigitWeekToken))]
    [TestCase("ww", typeof(PaddedDigitWeekToken))]
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
        Assert.That(format.Count(), Is.EqualTo(1));
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
    [TestCase("yyyy\"-Q\"q")]
    public void Tokenize_WithMultiCharLiteralDistinctTokens_ReturnsTokens(string input)
    {
        var format = new Lexer().Tokenize(input);
        Assert.That(format.Count(), Is.EqualTo(3));
        Assert.That(format.First(), Is.TypeOf<DigitOn4YearToken>());
        Assert.That(format.ElementAt(1), Is.TypeOf<LiteralToken>());
        Assert.That(format.Last(), Is.TypeOf<DigitQuarterToken>());
    }

    [TestCase("yyyy'\"'q", '\"')]
    [TestCase("yyyy\"'\"q", '\'')]
    public void Tokenize_WithQuoteToken_ReturnsTokens(string input, char expected)
    {
        var format = new Lexer().Tokenize(input);
        Assert.That(format.Count(), Is.EqualTo(3));
        Assert.That(format.First(), Is.TypeOf<DigitOn4YearToken>());
        Assert.That(format.ElementAt(1), Is.TypeOf<LiteralToken>());
        Assert.That(((LiteralToken)format.ElementAt(1)).Value, Is.EqualTo(expected.ToString()));
        Assert.That(format.Last(), Is.TypeOf<DigitQuarterToken>());
    }

    [Test]
    public void Tokenize_emptyString_Throws()
        => Assert.Throws<ArgumentException>(() => new Lexer().Tokenize(""));

    [TestCase("{yyyy}-{MM}")]
    [TestCase("yyyy'-'{MM}")]
    public void Tokenize_WithCurlyBrace_ReturnsTokens(string input)
    {
        var format = new Lexer().Tokenize(input);
        Assert.That(format.Count(), Is.EqualTo(3));
        Assert.That(format.First(), Is.TypeOf<DigitOn4YearToken>());
        Assert.That(format.ElementAt(1), Is.TypeOf<LiteralToken>());
        Assert.That(format.Last(), Is.TypeOf<PaddedDigitMonthToken>());
    }

    [TestCase("{d:RN}-{MM}", typeof(RomanNumeralDayToken), typeof(PaddedDigitMonthToken))]
    [TestCase("{dd:RN}-{MM}", typeof(RomanNumeralDayToken), typeof(PaddedDigitMonthToken))]
    [TestCase("{yy:RN}-{MM}", typeof(RomanNumeralShortYearToken), typeof(PaddedDigitMonthToken))]
    [TestCase("{yyyy:RN}-{MM}", typeof(RomanNumeralYearToken), typeof(PaddedDigitMonthToken))]
    [TestCase("yyyy'-'{M:RN}", typeof(DigitOn4YearToken), typeof(RomanNumeralMonthToken))]
    [TestCase("yyyy'-'{MM:RN}", typeof(DigitOn4YearToken), typeof(RomanNumeralMonthToken))]
    public void Tokenize_WithCurlyBraceAndRomanNumeral_ReturnsTokens(string input, Type year, Type month)
    {
        var format = new Lexer().Tokenize(input);
        Assert.That(format.Count(), Is.EqualTo(3));
        Assert.That(format.First(), Is.TypeOf(year));
        Assert.That(format.ElementAt(1), Is.TypeOf<LiteralToken>());
        Assert.That(format.Last(), Is.TypeOf(month));
    }

    [TestCase("{dd}['st'|'nd'|'rd'|'th']-{MM}", typeof(PaddedDigitDayToken), typeof(MutuallyExclusiveToken), typeof(PaddedDigitMonthToken))]
    [TestCase("{dd}['st' | 'nd' | 'rd' | 'th']-{MM}", typeof(PaddedDigitDayToken), typeof(MutuallyExclusiveToken), typeof(PaddedDigitMonthToken))]
    [TestCase("{dd}[\"st\" | \"nd\" | \"rd\" | \"th\"]-{MM}", typeof(PaddedDigitDayToken), typeof(MutuallyExclusiveToken), typeof(PaddedDigitMonthToken))]
    [TestCase("{dd}['st' | 'nd' | 'rd' | th]-{MM}", typeof(PaddedDigitDayToken), typeof(MutuallyExclusiveToken), typeof(PaddedDigitMonthToken))]
    public void Tokenize_WithCurlyBraceAndMutuallyExclusive_ReturnsTokens(string input, Type day, Type literal, Type month)
    {
        var format = new Lexer().Tokenize(input);
        Assert.That(format.Count(), Is.EqualTo(4));
        Assert.That(format.First(), Is.TypeOf(day));
        Assert.That(format.ElementAt(1), Is.TypeOf<MutuallyExclusiveToken>());
        var mutuallyExclusive = format.ElementAt(1) as MutuallyExclusiveToken;
        Assert.That(mutuallyExclusive!.Values.Count(), Is.EqualTo(4));
        var values = mutuallyExclusive!.Values.Select(x => ((LiteralToken)x).Value);
        Assert.That(values, Does.Contain("st"));
        Assert.That(values, Does.Contain("nd"));
        Assert.That(values, Does.Contain("rd"));
        Assert.That(values, Does.Contain("th"));
        Assert.That(format.ElementAt(2), Is.TypeOf<LiteralToken>());
        Assert.That(format.Last(), Is.TypeOf(month));
    }
}
