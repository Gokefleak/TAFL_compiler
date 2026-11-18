namespace TaflCompiler.Lexer;

/// <summary>
/// Converts the source code text of a TAFLang program into a sequence of tokens.
/// </summary>
public sealed class Lexer
{
    private static readonly Dictionary<string, TokenKind> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["int"] = TokenKind.KwInt,
        ["float"] = TokenKind.KwFloat,
        ["bool"] = TokenKind.KwBool,
        ["string"] = TokenKind.KwString,
        ["if"] = TokenKind.KwIf,
        ["else"] = TokenKind.KwElse,
        ["while"] = TokenKind.KwWhile,
        ["for"] = TokenKind.KwFor,
        ["return"] = TokenKind.KwReturn,
        ["fn"] = TokenKind.KwFn,
        ["let"] = TokenKind.KwLet,
        ["struct"] = TokenKind.KwStruct,
        ["print"] = TokenKind.KwPrint,
        ["input"] = TokenKind.KwInput,
        ["true"] = TokenKind.True,
        ["false"] = TokenKind.False,
    };

    private readonly string _source;
    private int _position;
    private int _line = 1;
    private int _column = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="Lexer"/> class.
    /// </summary>
    /// <param name="source">The source code text to tokenize.</param>
    public Lexer(string source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary>Gets the list of lexical errors found during tokenization.</summary>
    public List<LexerError> Errors { get; } = [];

    /// <summary>
    /// Reads the next token from the source code.
    /// </summary>
    /// <returns>The next token, or <see cref="TokenKind.Eof"/> if the end of the source is reached.</returns>
    public Token Next()
    {
        SkipWhitespaceAndComments();

        if (_position >= _source.Length)
        {
            return new Token(TokenKind.Eof, string.Empty, _line, _column);
        }

        char c = Peek();

        if (IsIdentifierStart(c))
        {
            return ReadIdentifierOrKeyword();
        }

        if (char.IsAsciiDigit(c))
        {
            return ReadNumber();
        }

        if (c == '"')
        {
            return ReadString();
        }

        if (c == '=' && Peek(1) == '=')
        {
            return MakeTwoChar(TokenKind.EqEq);
        }

        if (c == '!' && Peek(1) == '=')
        {
            return MakeTwoChar(TokenKind.NotEq);
        }

        if (c == '<' && Peek(1) == '=')
        {
            return MakeTwoChar(TokenKind.LtEq);
        }

        if (c == '>' && Peek(1) == '=')
        {
            return MakeTwoChar(TokenKind.GtEq);
        }

        if (c == '&' && Peek(1) == '&')
        {
            return MakeTwoChar(TokenKind.AndAnd);
        }

        if (c == '|' && Peek(1) == '|')
        {
            return MakeTwoChar(TokenKind.OrOr);
        }

        return c switch
        {
            '+' => MakeOneChar(TokenKind.Plus),
            '-' => MakeOneChar(TokenKind.Minus),
            '*' => MakeOneChar(TokenKind.Star),
            '/' => MakeOneChar(TokenKind.Slash),
            '%' => MakeOneChar(TokenKind.Percent),
            '=' => MakeOneChar(TokenKind.Assign),
            '!' => MakeOneChar(TokenKind.Bang),
            '<' => MakeOneChar(TokenKind.Lt),
            '>' => MakeOneChar(TokenKind.Gt),
            '(' => MakeOneChar(TokenKind.LParen),
            ')' => MakeOneChar(TokenKind.RParen),
            '{' => MakeOneChar(TokenKind.LBrace),
            '}' => MakeOneChar(TokenKind.RBrace),
            '[' => MakeOneChar(TokenKind.LBracket),
            ']' => MakeOneChar(TokenKind.RBracket),
            ',' => MakeOneChar(TokenKind.Comma),
            ';' => MakeOneChar(TokenKind.Semicolon),
            ':' => MakeOneChar(TokenKind.Colon),
            '.' => MakeOneChar(TokenKind.Dot),
            _ => MakeError($"Unknown character '{c}'."),
        };
    }

    private void SkipWhitespaceAndComments()
    {
        while (true)
        {
            if (Peek() == ' ' || Peek() == '\t' || Peek() == '\r' || Peek() == '\n')
            {
                Advance();
                continue;
            }

            if (Peek() == '/' && Peek(1) == '/')
            {
                Advance();
                Advance();
                while (Peek() != '\n' && Peek() != '\r' && _position < _source.Length)
                {
                    Advance();
                }

                continue;
            }

            if (Peek() == '/' && Peek(1) == '*')
            {
                Advance();
                Advance();
                while (true)
                {
                    if (_position >= _source.Length)
                    {
                        RecordError("Unclosed comment.");
                        return;
                    }

                    if (Peek() == '*' && Peek(1) == '/')
                    {
                        Advance();
                        Advance();
                        break;
                    }

                    Advance();
                }

                continue;
            }

            break;
        }
    }

    private Token ReadIdentifierOrKeyword()
    {
        int startPos = _position;
        int startLine = _line;
        int startColumn = _column;

        while (IsIdentifierPart(Peek()))
        {
            Advance();
        }

        string text = _source[startPos.._position];
        TokenKind kind = Keywords.TryGetValue(text, out TokenKind keywordKind)
            ? keywordKind
            : TokenKind.Identifier;

        return new Token(kind, text, startLine, startColumn);
    }

    private Token ReadNumber()
    {
        int startPos = _position;
        int startLine = _line;
        int startColumn = _column;

        while (char.IsAsciiDigit(Peek()))
        {
            Advance();
        }

        bool isFloat = false;
        if (Peek() == '.' && char.IsAsciiDigit(Peek(1)))
        {
            isFloat = true;
            Advance();
            while (char.IsAsciiDigit(Peek()))
            {
                Advance();
            }
        }

        if (IsIdentifierStart(Peek()))
        {
            RecordError("Invalid number format.");
        }

        string text = _source[startPos.._position];
        TokenKind kind = isFloat ? TokenKind.FloatLiteral : TokenKind.IntLiteral;
        return new Token(kind, text, startLine, startColumn);
    }

    private Token ReadString()
    {
        int startPos = _position;
        int startLine = _line;
        int startColumn = _column;

        Advance();

        while (true)
        {
            if (_position >= _source.Length)
            {
                RecordError("Unclosed string.");
                return new Token(TokenKind.StringLiteral, _source[startPos.._position], startLine, startColumn);
            }

            switch (Peek())
            {
                case '\n':
                case '\r':
                    RecordError("Unclosed string.");
                    return new Token(TokenKind.StringLiteral, _source[startPos.._position], startLine, startColumn);

                case '"':
                    Advance();
                    return new Token(TokenKind.StringLiteral, _source[startPos.._position], startLine, startColumn);

                case '\\':
                    if (!TryReadEscape())
                    {
                        return new Token(TokenKind.StringLiteral, _source[startPos.._position], startLine, startColumn);
                    }

                    break;

                default:
                    Advance();
                    break;
            }
        }
    }

    private bool TryReadEscape()
    {
        Advance();

        if (_position >= _source.Length || Peek() == '\n' || Peek() == '\r')
        {
            RecordError("Unclosed string.");
            return false;
        }

        switch (Peek())
        {
            case '"':
            case '\\':
            case 'n':
            case 't':
                Advance();
                return true;

            default:
                RecordError($"Invalid escape sequence '\\{Peek()}'.");
                return false;
        }
    }

    private Token MakeOneChar(TokenKind kind)
    {
        int line = _line;
        int column = _column;
        char c = Peek();
        Advance();
        return new Token(kind, c.ToString(), line, column);
    }

    private Token MakeTwoChar(TokenKind kind)
    {
        int line = _line;
        int column = _column;
        string text = $"{Peek()}{Peek(1)}";
        Advance();
        Advance();
        return new Token(kind, text, line, column);
    }

    private Token MakeError(string message)
    {
        int line = _line;
        int column = _column;
        char c = Peek();
        RecordError(message);
        Advance();
        return new Token(TokenKind.Error, c.ToString(), line, column);
    }

    private static bool IsIdentifierStart(char c) => char.IsAsciiLetter(c) || c == '_';

    private static bool IsIdentifierPart(char c) => char.IsAsciiLetterOrDigit(c) || c == '_';

    private char Peek(int offset = 0)
    {
        int i = _position + offset;
        return i < _source.Length ? _source[i] : '\0';
    }

    private void Advance()
    {
        char c = _source[_position];
        if (c == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        _position++;
    }

    private void RecordError(string message) => Errors.Add(new LexerError(message, _line, _column));
}
