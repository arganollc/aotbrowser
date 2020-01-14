using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Represents a token from the lexing file. A Token is a type, value, and position
    /// </summary>
    /// <remarks>
    /// Lexers will emit a sequence of Tokens, each with a given type. Formatters will consume
    /// Tokens and turn them into highlighted text in whatever format they support
    /// </remarks>
    public struct Token : IEquatable<Token>
    {
        /// <summary>
        /// The 0 based index into the string being lexed
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The type of this token (Comment, Text, Keyword, etc)
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// The string value of the token
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new Token at index 0
        /// </summary>
        /// <param name="type">The type of the Token</param>
        /// <param name="value">The value of the Token</param>
        public Token(TokenType type, string value) : this(0, type, value)
        {
        }

        /// <summary>
        /// Initializes a new Token at a provided index
        /// </summary>
        /// <param name="index">The index into the string being lexer</param>
        /// <param name="type">The type of the Token</param>
        /// <param name="value">The value of the Token</param>
        public Token(int index, TokenType type, string value)
        {
            Index = index;
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Creates a new token with an index adjusted by <paramref name="indexOffset"/>
        /// </summary>
        /// <remarks>This is useful for nested lexers that pass the inner lexer a substring of the full file, and nead to adjust the posititions accordingly</remarks>
        /// <param name="indexOffset">The number of characters to offset</param>
        /// <returns>A new token offset by the specified number of characters</returns>
        public Token Offset(int indexOffset)
        {
            return new Token(Index + indexOffset, Type, Value);
        }

        /// <summary>
        /// Gets a string representation of the Token
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Index}: \"{Value}\" ({Type})";

        #region R# Equality Members

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Token other)
        {
            return Index == other.Index && Type.Equals(other.Type) && string.Equals(Value, other.Value);
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Token && Equals((Token) obj);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Index;
                hashCode = (hashCode*397) ^ Type.GetHashCode();
                hashCode = (hashCode*397) ^ Value.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// compares two tokens for equality
        /// </summary>
        /// <param name="left">The LHS token</param>
        /// <param name="right">The RHS token</param>
        /// <returns></returns>
        public static bool operator ==(Token left, Token right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two tokens for inequality
        /// </summary>
        /// <param name="left">The LHS token</param>
        /// <param name="right">The RHS token</param>
        /// <returns></returns>
        public static bool operator !=(Token left, Token right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}