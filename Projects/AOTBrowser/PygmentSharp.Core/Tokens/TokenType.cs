using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Represents a nested type for a <see cref="Token"/>
    /// </summary>
    /// <remarks>
    /// Token Types are nested. For example, a Comment will have nested types <c>Comment.Multiline</c>,
    /// <c>Comment.Single</c>, or <c>Comment.Preproc</c>. Lexers and formatters have a choice on how
    /// specific they want to get. If a formatter doesn't want to support different styles for each of
    /// those comment types, it can just implement highlighting for Comment and all the child types will
    /// fall in line.
    /// </remarks>
    public class TokenType
    {
        /// <summary>
        /// Gets the parent type
        /// </summary>
        public TokenType Parent { get; }

        /// <summary>
        /// Gets the name of this type
        /// </summary>
        public string Name { get; set; }

        private readonly List<TokenType> _subtypes;
        /// <summary>
        /// Gets the depth of this Token Type
        /// </summary>
        /// <remarks>
        /// For example, Root.Comment.Preproc has a depth of 3
        /// </remarks>
        public int Depth { get; }

        /// <summary>
        /// Gets the list of subtypes for this token type
        /// </summary>
        public IReadOnlyCollection<TokenType> Subtypes => _subtypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenType"/> class
        /// </summary>
        /// <param name="parent">The parent token type</param>
        /// <param name="name">The name of this token type</param>
        public TokenType(TokenType parent, string name)
        {
            Parent = parent;
            Name = name;

            _subtypes = new List<TokenType>();
            Depth = CalculateDepth();
        }

        /// <summary>
        /// Creates a child token type
        /// </summary>
        /// <param name="name">The name of the child token type</param>
        /// <returns></returns>
        public TokenType CreateChild(string name)
        {
            var newTokenType = new TokenType(this, name);
            _subtypes.Add(newTokenType);
            return newTokenType;
        }

        /// <summary>
        /// Adds a child to this Token
        /// </summary>
        /// <typeparam name="TChild">The type of child being added</typeparam>
        /// <param name="child">The child to add</param>
        /// <returns></returns>
        public TChild AddChild<TChild>(TChild child) where TChild : TokenType
        {
            _subtypes.Add(child);
            return child;
        }

        /// <summary>
        /// Gets a list of types for this token, starting with itself and ending at its highest level parent
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TokenType> Split()
        {
            return YieldAncestors().Reverse();
        }

        private int CalculateDepth()
        {
            return YieldAncestors().Count();
        }

        private IEnumerable<TokenType> YieldAncestors()
        {
            var current = this;

            do
            {
                yield return current;
                current = current.Parent;

            } while (current != null);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Join(".", Split().Select(t => t.Name));
        }

    }
}
