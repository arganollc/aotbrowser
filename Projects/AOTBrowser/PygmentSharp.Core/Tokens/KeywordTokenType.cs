using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Holds different types of keyword tokens
    /// </summary>
    public class KeywordTokenType : TokenType
    {
        internal KeywordTokenType(TokenType parent) : base(parent, "Keyword")
        {
            Constant = CreateChild("Constant");
            Declaration = CreateChild("Declaration");
            Namespace = CreateChild("Namespace");
            Pseudo = CreateChild("Pseudo");
            Reserved = CreateChild("Reserved");
            Type = CreateChild("Type");
        }

        /// <summary>
        /// Gets a token for representing a constant keyword
        /// </summary>
        public TokenType Constant { get; }

        /// <summary>
        /// Gets a token for representing a declaration keyword
        /// </summary>
        public TokenType Declaration { get; }

        /// <summary>
        /// Gets a token for representing a namespace keyword
        /// </summary>
        public TokenType Namespace { get; }

        /// <summary>
        /// Gets a token for representing a pseudo keyword
        /// </summary>
        public TokenType Pseudo { get; }

        /// <summary>
        /// Gets a token for representing a reserved word
        /// </summary>
        public TokenType Reserved { get; }

        /// <summary>
        /// Gets a token for representing a type keyword
        /// </summary>
        public TokenType Type { get; }

    }
}