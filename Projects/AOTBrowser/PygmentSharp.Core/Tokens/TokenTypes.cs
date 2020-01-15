using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// The set of known <see cref="TokenType"/>s
    /// </summary>
    public static class TokenTypes
    {
        /// <summary>
        /// Gets the default (root) Token
        /// </summary>
        public static readonly TokenType Token = new TokenType(null, "Token");

        /// <summary>
        /// Gets a token for representing basic text
        /// </summary>
        public static readonly TokenType Text = Token.CreateChild("Text");

        /// <summary>
        /// Gets a token for representing whitespace
        /// </summary>
        public static readonly TokenType Whitespace = Text.CreateChild("Whitespace");

        /// <summary>
        /// Gets a token for representing an escape character
        /// </summary>
        public static readonly TokenType Escape = Token.CreateChild("Escape");

        /// <summary>
        /// Gets a token for representing an error in tokenizing
        /// </summary>
        public static readonly TokenType Error = Token.CreateChild("Error");

        /// <summary>
        /// Gets a token for representing an unknwon type of token
        /// </summary>
        public static readonly TokenType Other = Token.CreateChild("Other");

        /// <summary>
        /// Gets a token for representing a language keyword
        /// </summary>
        public static readonly KeywordTokenType Keyword = Token.AddChild(new KeywordTokenType(Token));

        /// <summary>
        /// Gets a token for representing a name
        /// </summary>
        public static readonly NameTokenType Name = Token.AddChild(new NameTokenType(Token));

        /// <summary>
        /// Gets a token for representing a literal value
        /// </summary>
        public static readonly LiteralTokenType Literal = Token.AddChild(new LiteralTokenType(Token));


        /// <summary>
        /// Gets a token for representing string literal. Alias for Literal.String
        /// </summary>
        public static readonly StringTokenType String = Literal.String;

        /// <summary>
        /// Gets a token for representing a number literal. Alias for Literal.Number
        /// </summary>
        public static readonly NumberTokenType Number = Literal.Number;

        /// <summary>
        /// Gets a token for representing generic punctuation
        /// </summary>
        public static readonly TokenType Punctuation = Token.CreateChild("Punctuation");

        /// <summary>
        /// Gets a token for representing an arithmetic operator
        /// </summary>
        public static readonly OperatorTokenType Operator = Token.AddChild(new OperatorTokenType(Token));

        /// <summary>
        /// Gets a token for representing a comment
        /// </summary>
        public static readonly CommentTokenType Comment = Token.AddChild(new CommentTokenType(Token));

        /// <summary>
        /// Gets a token for representing a generic token
        /// </summary>
        public static readonly GenericTokenType Generic = Token.AddChild(new GenericTokenType(Token));
    }
}