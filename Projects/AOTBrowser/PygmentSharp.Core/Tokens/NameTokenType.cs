using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Holds different types of name tokens
    /// </summary>
    public class NameTokenType : TokenType
    {
        internal NameTokenType(TokenType parent) : base(parent, "Name")
        {
            Attribute = CreateChild("Attribute");
            Builtin = AddChild(new NameBuiltinTokenType(this));
            Class = CreateChild("Class");
            Constant = CreateChild("Constant");
            Decorator = CreateChild("Decorator");
            Entity = CreateChild("Entity");
            Exception = CreateChild("Exception");
            Function = CreateChild("Function");
            Property = CreateChild("Property");
            Label = CreateChild("Label");
            Namespace = CreateChild("Namespace");
            Other = CreateChild("Other");
            Tag = CreateChild("Tag");
            Variable = new NameVariableTokenType(this);
        }


        /// <summary>
        /// Gets a token for representing the name of an attribute
        /// </summary>
        public TokenType Attribute { get; }

        /// <summary>
        /// Gets a token for representing the name of a builtin name
        /// </summary>
        public NameBuiltinTokenType Builtin { get; }

        /// <summary>
        /// Gets a token for representing the name of a class
        /// </summary>
        public TokenType Class { get; }

        /// <summary>
        /// Gets a token for representing the name of a constant
        /// </summary>
        public TokenType Constant { get; }

        /// <summary>
        /// Gets a token for representing the name of a decorator
        /// </summary>
        public TokenType Decorator { get; }

        /// <summary>
        /// Gets a token for representing the name of an entity
        /// </summary>
        public TokenType Entity { get; }

        /// <summary>
        /// Gets a token for representing the name of an exception
        /// </summary>
        public TokenType Exception { get; }

        /// <summary>
        /// Gets a token for representing the name of a function
        /// </summary>
        public TokenType Function { get; }

        /// <summary>
        /// Gets a token for representing the name of a property
        /// </summary>
        public TokenType Property { get; }

        /// <summary>
        /// Gets a token for representing the name of a label
        /// </summary>
        public TokenType Label { get; }

        /// <summary>
        /// Gets a token for representing the name of a namespace
        /// </summary>
        public TokenType Namespace { get; }

        /// <summary>
        /// Gets a token for representing an unknown type of name
        /// </summary>
        public TokenType Other { get; }

        /// <summary>
        /// Gets a token for representing the name of a tag
        /// </summary>
        public TokenType Tag { get; }

        /// <summary>
        /// Gets a token for representing the name of a variable
        /// </summary>
        public NameVariableTokenType Variable { get; }
    }
}