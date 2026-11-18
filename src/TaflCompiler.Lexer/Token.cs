namespace TaflCompiler.Lexer;

/// <summary>
/// Represents a single lexeme (token) produced by the lexer.
/// </summary>
public sealed class Token
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Token"/> class.
    /// </summary>
    /// <param name="kind">The kind of the token.</param>
    /// <param name="text">The original text of the token.</param>
    /// <param name="line">The line where the token starts (1-based).</param>
    /// <param name="column">The column where the token starts (1-based).</param>
    public Token(TokenKind kind, string text, int line, int column)
    {
        Kind = kind;
        Text = text;
        Line = line;
        Column = column;
    }

    /// <summary>Gets the kind of the token.</summary>
    public TokenKind Kind { get; }

    /// <summary>Gets the original text of the token as it appears in the source code.</summary>
    public string Text { get; }

    /// <summary>Gets the line where the token starts (1-based).</summary>
    public int Line { get; }

    /// <summary>Gets the column where the token starts (1-based).</summary>
    public int Column { get; }

    /// <summary>Returns a human-readable representation of the token.</summary>
    /// <returns>A string in the form <c>Kind('Text') @ Line:Column</c>.</returns>
    public override string ToString() => $"{Kind}('{Text}') @ {Line}:{Column}";
}
