using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Holds diffenent types of variable scopes
    /// </summary>
    public class NameVariableTokenType : TokenType
    {
        internal NameVariableTokenType(TokenType parent) : base(parent, "Variable")
        {
            Class = CreateChild("Class");
            Global = CreateChild("Global");
            Instance = CreateChild("Instance");
        }

        /// <summary>
        /// Gets a token for representing the name of a class variable
        /// </summary>
        public TokenType Class { get; }

        /// <summary>
        /// Gets a token for representing the name of a global variable
        /// </summary>
        public TokenType Global { get; }

        /// <summary>
        /// Gets a token for representing the name of an instance variable
        /// </summary>
        public TokenType Instance { get; }
    }
}