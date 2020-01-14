using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Tokens
{
    /// <summary>
    /// Holds different types of generic tokens
    /// </summary>
    public class GenericTokenType : TokenType
    {
        internal GenericTokenType(TokenType parent) : base(parent, "Generic")
        {
            Deleted = CreateChild("Deleted");
            Emph = CreateChild("Emph");
            Error = CreateChild("Error");
            Heading = CreateChild("Heading");
            Inserted = CreateChild("Inserted");
            Output = CreateChild("Output");
            Prompt = CreateChild(nameof(Prompt));
            Strong = CreateChild(nameof(Strong));
            Subheading = CreateChild(nameof(Subheading));
            Traceback = CreateChild(nameof(Traceback));

        }


        /// <summary>
        /// Gets a token for representing deleted text
        /// </summary>
        public TokenType Deleted { get; }

        /// <summary>
        /// Gets a token for representing emphasized text
        /// </summary>
        public TokenType Emph { get; }

        /// <summary>
        /// Gets a token for representing error text
        /// </summary>
        public TokenType Error { get; }

        /// <summary>
        /// Gets a token for representing heading text
        /// </summary>
        public TokenType Heading { get; }

        /// <summary>
        /// Gets a token for representing inserted text
        /// </summary>
        public TokenType Inserted { get; }

        /// <summary>
        /// Gets a token for representing output text
        /// </summary>
        public TokenType Output { get; }

        /// <summary>
        /// Gets a token for representing prompt text
        /// </summary>
        public TokenType Prompt { get; }

        /// <summary>
        /// Gets a token for representing strong text
        /// </summary>
        public TokenType Strong { get; }

        /// <summary>
        /// Gets a token for representing subheading text
        /// </summary>
        public TokenType Subheading { get; }

        /// <summary>
        /// Gets a token for representing traceback text
        /// </summary>
        public TokenType Traceback { get; }
    }
}