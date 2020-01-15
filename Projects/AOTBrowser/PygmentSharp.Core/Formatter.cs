using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core
{
    /// <summary>
    /// Base class for all output formatters
    /// </summary>
    public abstract class Formatter
    {
        /// <summary>
        /// Formats the token stream into a string representation
        /// </summary>
        /// <param name="tokenSource">The stream of Tokens to process</param>
        /// <param name="writer">The output stream to write formatted text</param>
        public void Format(IEnumerable<Token> tokenSource, TextWriter writer)
        {
            FormatUnencoded(tokenSource, writer);
        }

        /// <summary>
        /// When overriden in a child class, formats tokens into text stream
        /// </summary>
        /// <remarks>The name was borrowed from python, because it would further process the results to an encoding. That's not needed in C#</remarks>
        /// <param name="tokenSource">The input stream of Tokens</param>
        /// <param name="writer">The output stream to write text</param>
        protected abstract void FormatUnencoded(IEnumerable<Token> tokenSource, TextWriter writer);
    }
}
