using System;
using System.Collections.Generic;
using System.Linq;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Lexing
{
    /// <summary>
    /// Lexes text as a single big ol' <see cref="Token"/>
    /// </summary>
    [Lexer("Plain", AlternateNames = "Text,Plain Text")]
    [LexerFileExtension("*.txt")]
    public class PlainLexer : Lexer
    {
        /// <summary>
        /// When overridden in a child class, gets all the <see cref="Token"/>s for the given string
        /// </summary>
        /// <param name="text">The string to tokenize</param>
        /// <returns>A sequence of <see cref="Token"/> structs</returns>
        protected override IEnumerable<Token> GetTokensUnprocessed(string text)
        {
            yield return new Token(0, TokenTypes.Text, text);
        }

    }
}