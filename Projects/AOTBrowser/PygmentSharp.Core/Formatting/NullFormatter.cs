using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Formatting
{
    /// <summary>
    /// A formatter that writes tokens as plain text
    /// </summary>
    /// <remarks>One could use this to test a roundtrip of tokens. A file that is tokenized with the correct lexer
    /// and written using the <see cref="NullFormatter"/> should match the original input file</remarks>
    public class NullFormatter : Formatter
    {
        /// <summary>
        /// When overriden in a child class, formats tokens into text stream
        /// </summary>
        /// <remarks>The name was borrowed from python, because it would further process the results to an encoding. That's not needed in C#</remarks>
        /// <param name="tokenSource">The input stream of Tokens</param>
        /// <param name="writer">The output stream to write text</param>
        protected override void FormatUnencoded(IEnumerable<Token> tokenSource, TextWriter writer)
        {
            foreach (var token in tokenSource)
            {
                writer.Write(token.Value);
            }
        }
    }
}
