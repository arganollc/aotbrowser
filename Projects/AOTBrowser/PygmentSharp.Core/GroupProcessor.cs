using System;
using System.Collections.Generic;
using System.Linq;

using PygmentSharp.Core.Lexing;
using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core
{
    /// <summary>
    /// Inheritors of this class process a Regex group to yield additional tokens
    /// </summary>
    /// <remarks>
    /// This feature supports a regex to match and yield multiple tokens. For example you might write a single regex
    /// to match an attribute, =, and attribute value, and yield all those tokens at once. A set of GroupProcessors
    /// can handle each match group and provide the right tokens
    /// </remarks>
    public abstract class GroupProcessor
    {
        /// <summary>
        /// Gets the tokens for a matched group value
        /// </summary>
        /// <param name="context">The lexer context</param>
        /// <param name="value">The matched group value to process</param>
        /// <returns></returns>
        public abstract IEnumerable<Token> GetTokens(RegexLexerContext context, string value);
    }

    /// <summary>
    /// Processes a regex group by executing a different lexer over the match
    /// </summary>
    /// <remarks>
    /// This is useful for situations where a proper lexing needs to
    /// execute a different language lexer over a subsection of the file. For
    /// example, in HTML you might have embedded Javascript or CSS tags who's
    /// contents should be appropriately styled
    /// </remarks>
    public class LexerGroupProcessor : GroupProcessor
    {
        /// <summary>
        /// Gets the lexer that will execute over the match group
        /// </summary>
        public Lexer Lexer { get; }

        /// <summary>
        /// Iniitializes a new instance of the <see cref="LexerGroupProcessor"/> class
        /// </summary>
        /// <param name="lexer">The <see cref="Lexer"/> to execute over the match group</param>
        public LexerGroupProcessor(Lexer lexer)
        {
            Lexer = lexer;
        }

        /// <summary>
        /// Gets the tokens in the match context by executing a different lexer
        /// </summary>
        /// <param name="context">The context of the current lexing process</param>
        /// <param name="value">The string to lex</param>
        /// <returns></returns>
        public override IEnumerable<Token> GetTokens(RegexLexerContext context, string value)
        {
            var tokens = Lexer.GetTokens(value);

            context.Position += value.Length;

            return tokens;
        }
    }

    /// <summary>
    /// Processes a match group for single token
    /// </summary>
    public class TokenGroupProcessor : GroupProcessor
    {
        /// <summary>
        /// Gets the type of token that should be yielded
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenGroupProcessor"/> class
        /// </summary>
        /// <param name="type">The type of token that should be yielded for the group</param>
        public TokenGroupProcessor(TokenType type)
        {
            Type = type;
        }

        /// <summary>
        /// Processes a match group and yields a single token for the vale
        /// </summary>
        /// <param name="context">The context of the lexer</param>
        /// <param name="value">The group value that should be turned into a token</param>
        /// <returns></returns>
        public override IEnumerable<Token> GetTokens(RegexLexerContext context, string value)
        {
            yield return new Token(context.Position, Type, value);
            context.Position += value.Length;
        }
    }
}