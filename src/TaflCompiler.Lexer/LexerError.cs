namespace TaflCompiler.Lexer;

/// <summary>
/// Represents a lexical error found during tokenization.
/// </summary>
public sealed class LexerError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LexerError"/> class.
    /// </summary>
    /// <param name="message">A human-readable description of the error.</param>
    /// <param name="line">The line where the error occurs (1-based).</param>
    /// <param name="column">The column where the error occurs (1-based).</param>
    public LexerError(string message, int line, int column)
    {
        Message = message;
        Line = line;
        Column = column;
    }

    /// <summary>Gets the human-readable description of the error.</summary>
    public string Message { get; }

    /// <summary>Gets the line where the error occurs (1-based).</summary>
    public int Line { get; }

    /// <summary>Gets the column where the error occurs (1-based).</summary>
    public int Column { get; }

    /// <summary>Returns a human-readable representation of the error.</summary>
    /// <returns>A string in the form <c>Line:Column: Message</c>.</returns>
    public override string ToString() => $"{Line}:{Column}: {Message}";
}
