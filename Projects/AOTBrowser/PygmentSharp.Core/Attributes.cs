using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core
{
    /// <summary>
    /// Annotates a lexer to specify it's name(s)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LexerAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the lexer (case sensitive).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets a comma separated list of alternative lexer names (case sensitive)`
        /// </summary>
        public string AlternateNames { get; set; }

        /// <summary>
        /// Constructs a new instance of the <see cref="LexerAttribute"/>
        /// </summary>
        /// <remarks>Lexer names are case sensitive, so use alternative names to specify other common names</remarks>
        /// <param name="name">The name of the Lexer</param>
        public LexerAttribute(string name)
        {
            Name = name;
        }

    }

    /// <summary>
    /// Annotates a lexer with file extensions it can process
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LexerFileExtensionAttribute : Attribute
    {
        /// <summary>
        /// Gets the wildcard pattern that matches files that can be processed by the lexer
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// Constructs a new instance of <see cref="LexerFileExtensionAttribute"/>
        /// </summary>
        /// <param name="pattern">The wildcard pattern that matches files that can be processed by the lexer</param>
        public LexerFileExtensionAttribute(string pattern)
        {
            Pattern = pattern;
        }
    }

    /// <summary>
    /// Annotates a <see cref="Formatter"/> with its name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FormatterAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the formatter
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets a comma-separated list of alternative names for a <see cref="Formatter"/>
        /// </summary>
        public string AlternateNames { get; set; }

        /// <summary>
        /// Initializes a new isntance of the <see cref="FormatterAttribute"/>
        /// </summary>
        /// <param name="name">The name of the formatter</param>
        public FormatterAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Annotates a <see cref="Formatter"/> with a file extension it can write to
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FormatterFileExtensionAttribute : Attribute
    {
        /// <summary>
        /// Gets the wildcard pattern matching file types this formatter can write
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// Initalizes a new instance of the <see cref="FormatterFileExtensionAttribute"/> class
        /// </summary>
        /// <param name="pattern">The wildcard pattern</param>
        public FormatterFileExtensionAttribute(string pattern)
        {
            Pattern = pattern;
        }
    }
}
