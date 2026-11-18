using TaflCompiler.Lexer;
using Xunit;

namespace TaflCompiler.Lexer.Tests;

public class LexerTests
{
    [Theory]
    [InlineData("+", TokenKind.Plus)]
    [InlineData("-", TokenKind.Minus)]
    [InlineData("*", TokenKind.Star)]
    [InlineData("/", TokenKind.Slash)]
    [InlineData("%", TokenKind.Percent)]
    [InlineData("=", TokenKind.Assign)]
    [InlineData("!", TokenKind.Bang)]
    [InlineData("<", TokenKind.Lt)]
    [InlineData(">", TokenKind.Gt)]
    [InlineData("(", TokenKind.LParen)]
    [InlineData(")", TokenKind.RParen)]
    [InlineData("{", TokenKind.LBrace)]
    [InlineData("}", TokenKind.RBrace)]
    [InlineData("[", TokenKind.LBracket)]
    [InlineData("]", TokenKind.RBracket)]
    [InlineData(",", TokenKind.Comma)]
    [InlineData(";", TokenKind.Semicolon)]
    [InlineData(":", TokenKind.Colon)]
    [InlineData(".", TokenKind.Dot)]
    public void SingleCharToken(string source, TokenKind expected)
        => Assert.Equal(expected, Single(source).Kind);

    [Theory]
    [InlineData("==", TokenKind.EqEq)]
    [InlineData("!=", TokenKind.NotEq)]
    [InlineData("<=", TokenKind.LtEq)]
    [InlineData(">=", TokenKind.GtEq)]
    [InlineData("&&", TokenKind.AndAnd)]
    [InlineData("||", TokenKind.OrOr)]
    public void TwoCharToken(string source, TokenKind expected)
        => Assert.Equal(expected, Single(source).Kind);

    [Theory]
    [InlineData("int", TokenKind.KwInt)]
    [InlineData("float", TokenKind.KwFloat)]
    [InlineData("bool", TokenKind.KwBool)]
    [InlineData("string", TokenKind.KwString)]
    [InlineData("if", TokenKind.KwIf)]
    [InlineData("else", TokenKind.KwElse)]
    [InlineData("while", TokenKind.KwWhile)]
    [InlineData("for", TokenKind.KwFor)]
    [InlineData("return", TokenKind.KwReturn)]
    [InlineData("fn", TokenKind.KwFn)]
    [InlineData("let", TokenKind.KwLet)]
    [InlineData("struct", TokenKind.KwStruct)]
    [InlineData("print", TokenKind.KwPrint)]
    [InlineData("input", TokenKind.KwInput)]
    public void Keywords(string source, TokenKind expected)
        => Assert.Equal(expected, Single(source).Kind);

    [Theory]
    [InlineData("true", TokenKind.True)]
    [InlineData("false", TokenKind.False)]
    public void BoolLiterals(string source, TokenKind expected)
        => Assert.Equal(expected, Single(source).Kind);

    [Theory]
    [InlineData("0")]
    [InlineData("31")]
    [InlineData("123456789")]
    public void IntLiteral(string source)
        => Assert.Equal(TokenKind.IntLiteral, Single(source).Kind);

    [Theory]
    [InlineData("3.14")]
    [InlineData("0.5")]
    [InlineData("123.456")]
    public void FloatLiteral(string source)
    {
        Token token = Single(source);
        Assert.Equal(TokenKind.FloatLiteral, token.Kind);
        Assert.Equal(source, token.Text);
    }

    [Fact]
    public void MaximalMunch_EqEq_NotTwoAssign()
    {
        Token[] tokens = Tokenize("==");
        Assert.Equal(2, tokens.Length);
        Assert.Equal(TokenKind.EqEq, tokens[0].Kind);
        Assert.Equal(TokenKind.Eof, tokens[1].Kind);
    }

    [Fact]
    public void MaximalMunch_LtEq()
    {
        Token[] tokens = Tokenize("<=");
        Assert.Equal(2, tokens.Length);
        Assert.Equal(TokenKind.LtEq, tokens[0].Kind);
        Assert.Equal(TokenKind.Eof, tokens[1].Kind);
    }

    [Theory]
    [InlineData("IF")]
    [InlineData("If")]
    [InlineData("iF")]
    public void Keywords_CaseInsensitive(string source)
    {
        Token token = Single(source);
        Assert.Equal(TokenKind.KwIf, token.Kind);
        Assert.Equal(source, token.Text);
    }

    [Fact]
    public void BoolLiterals_CaseInsensitive()
    {
        Assert.Equal(TokenKind.True, Single("TRUE").Kind);
        Assert.Equal(TokenKind.False, Single("False").Kind);
    }

    [Fact]
    public void Identifier_Ifx_NotKeyword()
    {
        Token token = Single("ifx");
        Assert.Equal(TokenKind.Identifier, token.Kind);
        Assert.Equal("ifx", token.Text);
    }

    [Fact]
    public void Identifier_PreservesOriginalCase()
    {
        Token token = Single("FooBar");
        Assert.Equal(TokenKind.Identifier, token.Kind);
        Assert.Equal("FooBar", token.Text);
    }

    [Fact]
    public void Float_RequiresDigitAfterDot()
    {
        Token[] tokens = Tokenize("123.");
        Assert.Equal(3, tokens.Length);
        Assert.Equal(TokenKind.IntLiteral, tokens[0].Kind);
        Assert.Equal("123", tokens[0].Text);
        Assert.Equal(TokenKind.Dot, tokens[1].Kind);
        Assert.Equal(TokenKind.Eof, tokens[2].Kind);
    }

    [Fact]
    public void Float_DotBeforeDigit_IsDotAndInt()
    {
        Token[] tokens = Tokenize(".5");
        Assert.Equal(3, tokens.Length);
        Assert.Equal(TokenKind.Dot, tokens[0].Kind);
        Assert.Equal(TokenKind.IntLiteral, tokens[1].Kind);
        Assert.Equal("5", tokens[1].Text);
        Assert.Equal(TokenKind.Eof, tokens[2].Kind);
    }

    [Theory]
    [InlineData("12abc")]
    [InlineData("12_e")]
    public void Number_LetterOrUnderscoreAfter_Error(string source)
    {
        (Token[] Tokens, List<LexerError> Errors) result = TokenizeWithErrors(source);
        List<LexerError> errors = result.Errors;
        Assert.NotEmpty(errors);
    }

    [Theory]
    [InlineData("12.5abc")]
    [InlineData("12.5_")]
    public void Float_LetterOrUnderscoreAfter_Error(string source)
    {
        (Token[] Tokens, List<LexerError> Errors) result = TokenizeWithErrors(source);
        List<LexerError> errors = result.Errors;
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void StringLiteral_ValidEscapes()
    {
        Token token = Single("\"hello\\n\\t\\\\\\\"world\"");
        Assert.Equal(TokenKind.StringLiteral, token.Kind);
    }

    [Fact]
    public void StringLiteral_RawNewline_Error()
    {
        (Token[] Tokens, List<LexerError> Errors) result = TokenizeWithErrors("\"line1\nline2\"");
        List<LexerError> errors = result.Errors;
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void StringLiteral_UnknownEscape_Error()
    {
        (Token[] Tokens, List<LexerError> Errors) result = TokenizeWithErrors("\"bad\\q\"");
        List<LexerError> errors = result.Errors;
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void StringLiteral_Unclosed_Error()
    {
        (Token[] Tokens, List<LexerError> Errors) result = TokenizeWithErrors("\"unclosed");
        List<LexerError> errors = result.Errors;
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void StringLiteral_BackslashAtEnd_Error()
    {
        (Token[] Tokens, List<LexerError> Errors) result = TokenizeWithErrors("\"abc\\");
        List<LexerError> errors = result.Errors;
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void LineComment_Skipped()
    {
        Token[] tokens = Tokenize("a // comment\nb");
        Assert.Equal(3, tokens.Length);
        Assert.Equal(TokenKind.Identifier, tokens[0].Kind);
        Assert.Equal("a", tokens[0].Text);
        Assert.Equal(TokenKind.Identifier, tokens[1].Kind);
        Assert.Equal("b", tokens[1].Text);
        Assert.Equal(TokenKind.Eof, tokens[2].Kind);
    }

    [Fact]
    public void LineComment_AtEof()
    {
        Token[] tokens = Tokenize("a // comment");
        Assert.Equal(2, tokens.Length);
        Assert.Equal(TokenKind.Identifier, tokens[0].Kind);
        Assert.Equal("a", tokens[0].Text);
        Assert.Equal(TokenKind.Eof, tokens[1].Kind);
    }

    [Fact]
    public void BlockComment_Skipped()
    {
        Token[] tokens = Tokenize("a /* comment */ b");
        Assert.Equal(3, tokens.Length);
        Assert.Equal(TokenKind.Identifier, tokens[0].Kind);
        Assert.Equal("a", tokens[0].Text);
        Assert.Equal(TokenKind.Identifier, tokens[1].Kind);
        Assert.Equal("b", tokens[1].Text);
        Assert.Equal(TokenKind.Eof, tokens[2].Kind);
    }

    [Fact]
    public void BlockComment_Unclosed_Error()
    {
        (Token[] Tokens, List<LexerError> Errors) result = TokenizeWithErrors("a /* unclosed");
        List<LexerError> errors = result.Errors;
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void Whitespace_Skipped()
    {
        Token[] tokens = Tokenize("  \t\r\n  ");
        Assert.Single(tokens);
        Assert.Equal(TokenKind.Eof, tokens[0].Kind);
    }

    [Fact]
    public void EmptySource_OnlyEof()
    {
        Token[] tokens = Tokenize("");
        Assert.Single(tokens);
        Assert.Equal(TokenKind.Eof, tokens[0].Kind);
    }

    [Theory]
    [InlineData("@")]
    [InlineData("\u00A0")]
    [InlineData("&")]
    [InlineData("|")]
    public void UnknownChar_Error(string source)
    {
        (Token[] Tokens, List<LexerError> Errors) result = TokenizeWithErrors(source);
        List<LexerError> errors = result.Errors;
        Assert.NotEmpty(errors);
        Assert.Contains(result.Tokens, (Token t) => t.Kind == TokenKind.Error);
    }

    private static Token[] Tokenize(string source)
    {
        Lexer lexer = new Lexer(source);
        List<Token> list = new List<Token>();
        while (true)
        {
            Token t = lexer.Next();
            list.Add(t);
            if (t.Kind == TokenKind.Eof)
            {
                break;
            }
        }

        return list.ToArray();
    }

    private static Token Single(string source)
    {
        Token[] tokens = Tokenize(source);
        Assert.Equal(2, tokens.Length);
        Assert.Equal(TokenKind.Eof, tokens[1].Kind);
        return tokens[0];
    }

    private static (Token[] Tokens, List<LexerError> Errors) TokenizeWithErrors(string source)
    {
        Lexer lexer = new Lexer(source);
        List<Token> tokens = new List<Token>();
        while (true)
        {
            Token t = lexer.Next();
            tokens.Add(t);
            if (t.Kind == TokenKind.Eof)
            {
                break;
            }
        }

        return (tokens.ToArray(), lexer.Errors);
    }
}
