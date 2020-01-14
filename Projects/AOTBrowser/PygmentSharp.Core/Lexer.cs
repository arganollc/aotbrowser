using System;
using System.Collections.Generic;

using PygmentSharp.Core.Tokens;
using PygmentSharp.Core.Utils;

namespace PygmentSharp.Core
{
    /// <summary>
    /// Base class for all Lexers
    /// </summary>
    /// <remarks>
    /// Lexers convert the input source to a sequence of <see cref="Token"/>s.
    /// The token types determine how they will
    /// be highlighted by supported output <see cref="Formatter"/>s.
    /// </remarks>
    public abstract class Lexer
    {
        /// <summary>
        /// Gets the tokens from an input text
        /// </summary>
        /// <param name="text">The text to process</param>
        /// <returns></returns>
        public IEnumerable<Token> GetTokens(string text)
        {
            Argument.EnsureNotNull(text, nameof(text));

            text = text.Replace("\r\n", "\n");
            text = text.Replace("\r", "\n");

            return GetTokensUnprocessed(text);
        }

        /// <summary>
        /// When overridden in a child class, gets all the <see cref="Token"/>s for the given string
        /// </summary>
        /// <param name="text">The string to tokenize</param>
        /// <returns>A sequence of <see cref="Token"/> structs</returns>
        protected abstract IEnumerable<Token> GetTokensUnprocessed(string text);
    }
}
