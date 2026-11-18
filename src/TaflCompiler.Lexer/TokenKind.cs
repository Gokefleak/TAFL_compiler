namespace TaflCompiler.Lexer;

/// <summary>
/// Types of language lexemes TAFLang.
/// </summary>
public enum TokenKind
{
    // Literals

    /// <summary>Integer literal.</summary>
    IntLiteral,

    /// <summary>Float literal.</summary>
    FloatLiteral,

    /// <summary>String literal.</summary>
    StringLiteral,

    /// <summary>True literal.</summary>
    True,

    /// <summary>False literal.</summary>
    False,

    // Identifier

    /// <summary>Identifier (name of a variable, function, or type).</summary>
    Identifier,

    // Keywords: data type

    /// <summary>Keyword int.</summary>
    KwInt,

    /// <summary>Keyword float.</summary>
    KwFloat,

    /// <summary>Keyword bool.</summary>
    KwBool,

    /// <summary>Keyword string.</summary>
    KwString,

    // Keyword: flow control

    /// <summary>Keyword if.</summary>
    KwIf,

    /// <summary>Keyword else.</summary>
    KwElse,

    /// <summary>Keyword while.</summary>
    KwWhile,

    /// <summary>Keyword for.</summary>
    KwFor,

    /// <summary>Keyword return.</summary>
    KwReturn,

    // Keyword: declarations and input/output

    /// <summary>Keyword fn.</summary>
    KwFn,

    /// <summary>Keyword let.</summary>
    KwLet,

    /// <summary>Keyword struct.</summary>
    KwStruct,

    /// <summary>Keyword print.</summary>
    KwPrint,

    /// <summary>Keyword input.</summary>
    KwInput,

    // Arithmetic operators

    /// <summary>Operator +.</summary>
    Plus,

    /// <summary>Operator -.</summary>
    Minus,

    /// <summary>Operator *.</summary>
    Star,

    /// <summary>Operator /.</summary>
    Slash,

    /// <summary>Operator %.</summary>
    Percent,

    // Comparison operators

    /// <summary>Operator ==.</summary>
    EqEq,

    /// <summary>Operator !=.</summary>
    NotEq,

    /// <summary>Operator &lt;.</summary>
    Lt,

    /// <summary>Operator &gt;.</summary>
    Gt,

    /// <summary>Operator &lt;=.</summary>
    LtEq,

    /// <summary>Operator &gt;=.</summary>
    GtEq,

    // logical operators

    /// <summary>Operator &amp;&amp;.</summary>
    AndAnd,

    /// <summary>Operator ||.</summary>
    OrOr,

    /// <summary>Operator !.</summary>
    Bang,

    /// <summary>Operator =.</summary>
    Assign,

    // Punctuation

    /// <summary>Opening parenthesis.</summary>
    LParen,

    /// <summary>Closing parenthesis.</summary>
    RParen,

    /// <summary>Opening curly brace.</summary>
    LBrace,

    /// <summary>Closing curly brace.</summary>
    RBrace,

    /// <summary>Opening square bracket.</summary>
    LBracket,

    /// <summary>Closing square bracket.</summary>
    RBracket,

    /// <summary>Comma.</summary>
    Comma,

    /// <summary>Semicolon.</summary>
    Semicolon,

    /// <summary>Colon.</summary>
    Colon,

    /// <summary>Dot.</summary>
    Dot,

    // Functional

    /// <summary>End.</summary>
    Eof,

    /// <summary>Err.</summary>
    Error,
}
