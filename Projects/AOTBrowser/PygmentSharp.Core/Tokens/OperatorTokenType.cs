using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Holds different types of operator tokens
    /// </summary>
    public class OperatorTokenType : TokenType
    {
        internal OperatorTokenType(TokenType parent) : base(parent, "Operator")
        {
            Word = CreateChild("Word");
        }

        /// <summary>
        /// Gets a token for representing word operator
        /// </summary>
        public TokenType Word { get; }

    }
}