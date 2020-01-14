using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Holds different types of string tokens
    /// </summary>
    public class StringTokenType : TokenType
    {
        internal StringTokenType(TokenType parent) : base(parent, "String")
        {
            Backtick = CreateChild("Backtick");
            Char = CreateChild("Char");
            Doc = CreateChild("Doc");
            Double = CreateChild("Double");
            Escape = CreateChild("Escape");
            Heredoc = CreateChild("Heredoc");
            Interpol = CreateChild("Interpol");
            Other = CreateChild("Other");
            Regex = CreateChild("Regex");
            Single = CreateChild("Single");
            Symbol = CreateChild("Symbol");

        }


        /// <summary>
        /// Gets a token for representing a backtick string
        /// </summary>
        public TokenType Backtick { get; }

        /// <summary>
        /// Gets a token for representing a character
        /// </summary>
        public TokenType Char { get; }

        /// <summary>
        /// Gets a token for representing a documentation string
        /// </summary>
        public TokenType Doc { get; }

        /// <summary>
        /// Gets a token for representing a double quoted string
        /// </summary>
        public TokenType Double { get; }

        /// <summary>
        /// Gets a token for representing a single quoted string
        /// </summary>
        public TokenType Escape { get; }

        /// <summary>
        /// Gets a token for representing a heredoc string
        /// </summary>
        public TokenType Heredoc { get; }

        /// <summary>
        /// Gets a token for representing an interpolated string
        /// </summary>
        public TokenType Interpol { get; }

        /// <summary>
        /// Gets a token for representing some other string
        /// </summary>
        public TokenType Other { get; }

        /// <summary>
        /// Gets a token for representing a regular expression string
        /// </summary>
        public TokenType Regex { get; }

        /// <summary>
        /// Gets a token for representing a single quote string
        /// </summary>
        public TokenType Single { get; }

        /// <summary>
        /// Gets a token for representing a symbol
        /// </summary>
        public TokenType Symbol { get; }
    }
}