using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Holds different types of number tokens
    /// </summary>
    public class NumberTokenType : TokenType
    {
        internal NumberTokenType(TokenType parent) : base(parent, "Number")
        {

            Bin = CreateChild("Bin");
            Float = CreateChild("Float");
            Hex = CreateChild("Hex");
            Integer = AddChild(new IntegerTokenType(this));
            Oct = CreateChild("Oct");
        }


        /// <summary>
        /// Gets a token for representing a binary number
        /// </summary>
        public TokenType Bin { get; }

        /// <summary>
        /// Gets a token for representing a floating point number
        /// </summary>
        public TokenType Float { get; }

        /// <summary>
        /// Gets a token for representing a hexadecimal number
        /// </summary>
        public TokenType Hex { get; }

        /// <summary>
        /// Gets a token for representing an integer number
        /// </summary>
        public IntegerTokenType Integer { get; }

        /// <summary>
        /// Gets a token for representing an octal number
        /// </summary>
        public TokenType Oct { get; }
    }
}