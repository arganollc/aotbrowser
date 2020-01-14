using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Holds different types of integer tokens
    /// </summary>
    public class IntegerTokenType : TokenType
    {
        internal IntegerTokenType(TokenType parent) : base(parent, "Integer")
        {
            Long = CreateChild("Long");
        }

        /// <summary>
        /// Gets a token for representing a long integer
        /// </summary>
        public TokenType Long { get; }
    }
}