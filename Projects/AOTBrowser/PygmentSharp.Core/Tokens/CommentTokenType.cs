using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Holds different types of comment tokesn
    /// </summary>
    public class CommentTokenType : TokenType
    {
        internal CommentTokenType(TokenType parent) : base(parent, "Comment")
        {
            Hashbang = CreateChild("Hashbang");
            Multiline = CreateChild("Multiline");
            Preproc = CreateChild("Preproc");
            PreprocFile = CreateChild("PreprocFile");
            Single = CreateChild("Single");
            Special = CreateChild("Special");
        }

        /// <summary>
        /// Gets a token for representing a hashbang line
        /// </summary>
        public TokenType Hashbang { get; }

        /// <summary>
        /// Gets a token for representing a multiline comment
        /// </summary>
        public TokenType Multiline { get; }

        /// <summary>
        /// Gets a token for representing a preprocessor commenet
        /// </summary>
        public TokenType Preproc { get; }

        /// <summary>
        /// Gets a token for representing a prepreocessor file
        /// </summary>
        public TokenType PreprocFile { get; }

        /// <summary>
        /// Gets a token for representing a single line comment
        /// </summary>
        public TokenType Single { get; }

        /// <summary>
        /// Gets a token for representing a special comment
        /// </summary>
        public TokenType Special { get; }
    }
}