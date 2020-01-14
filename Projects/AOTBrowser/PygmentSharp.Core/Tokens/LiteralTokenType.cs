using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Holds different types of literal tokens
    /// </summary>
    public class LiteralTokenType : TokenType
    {
        internal LiteralTokenType(TokenType parent) : base(parent, "Literal")
        {
            Date = CreateChild("Date");
            String = AddChild(new StringTokenType(this));
            Number = AddChild(new NumberTokenType(this));
        }

        /// <summary>
        /// Gets a token for representing a date literal
        /// </summary>
        public TokenType Date { get; }

        /// <summary>
        /// Gets a token for representing a string literal
        /// </summary>
        public StringTokenType String { get; }

        /// <summary>
        /// Gets a token for representing a number literal
        /// </summary>
        public NumberTokenType Number { get; }
    }
}