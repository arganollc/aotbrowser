using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Holds different types of builtin name tokens
    /// </summary>
    public class NameBuiltinTokenType : TokenType
    {
        internal NameBuiltinTokenType(TokenType parent) : base(parent, "Builtin")
        {
            Pseudo = CreateChild("Pseudo");
        }


        /// <summary>
        /// Gets a token for representing a pseudo name token
        /// </summary>
        public TokenType Pseudo { get; }
    }
}