using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using PygmentSharp.Core.Formatting;
using PygmentSharp.Core.Lexing;

namespace PygmentSharp.Core
{
    /// <summary>
    /// Represents the result of a highlighting operation
    /// </summary>
    public class HighlightingResult
    {
        /// <summary>
        /// Gets the output stream
        /// </summary>
        public Stream OutputStream { get; }

        /// <summary>
        /// Gets the input text
        /// </summary>
        public string Input { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightingResult"/> class
        /// </summary>
        /// <param name="input">The input text</param>
        /// <param name="output">output stream</param>
        public HighlightingResult(string input, Stream output)
        {
            Input = input;
            OutputStream = output;
        }

        /// <summary>
        /// Outputs the stream in an optional encoding. Default is UTF8
        /// </summary>
        /// <param name="encoding">optional encoding. default is utf-8</param>
        /// <returns>the formatted text</returns>
        public string OutputAsString(Encoding encoding = null)
        {
            var outputEncoding = encoding ?? Encoding.UTF8;
            var reader = new StreamReader(OutputStream, outputEncoding);
            return reader.ReadToEnd();
        }
    }

    /// <summary>
    /// Fluen interface extension methods
    /// </summary>
    public static class FluentExtensions
    {
        //public static IPygmentizeBuilder WithLexerFor(this IPygmentizeBuilder builder, string name)
        //{
        //    throw new NotImplementedException();
        //}

        //public static IPygmentizeBuilder WithLexerForFile(this IPygmentizeBuilder builder, string filename)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Defines the process to output to html with the default HTML options
        /// </summary>
        /// <param name="builder">the builder</param>
        /// <returns></returns>
        public static IPygmentizeBuilder ToHtml(this IPygmentizeBuilder builder)
        {
            return builder.WithFormatter(new HtmlFormatter());
        }
    }


    /// <summary>
    /// Defines an interface for building a highlighting operation
    /// </summary>
    public interface IPygmentizeBuilder
    {
        /// <summary>
        /// Specifies the input encoding
        /// </summary>
        /// <param name="encoding">The encoding</param>
        /// <returns></returns>
        IPygmentizeBuilder WithInputEncoding(Encoding encoding);

        /// <summary>
        /// Specifies the output encoding
        /// </summary>
        /// <param name="encoding">The encoding</param>
        /// <returns></returns>
        IPygmentizeBuilder WithOutputEncoding(Encoding encoding);

        /// <summary>
        /// Specifies which lexer to use
        /// </summary>
        /// <param name="lexer">The lexer</param>
        /// <returns></returns>
        IPygmentizeBuilder WithLexer(Lexer lexer);

        /// <summary>
        /// Specifies which formatter to use
        /// </summary>
        /// <param name="formatter">The lexer</param>
        /// <returns></returns>
        IPygmentizeBuilder WithFormatter(Formatter formatter);

        /// <summary>
        /// Executes the highlighting process and returns a string
        /// </summary>
        /// <returns></returns>
        string AsString();

        /// <summary>
        /// Executes the process and writes to a file
        /// </summary>
        /// <param name="filename">The filename to write</param>
        void ToFile(string filename);
    }

    internal class PygmentizeContentBuilder : IPygmentizeBuilder
    {
        private readonly string _input;
        public PygmentizeContentBuilder(string content)
        {
            _input = content;
        }

        private Lexer _lexer = new PlainLexer();
        private Formatter _formatter = new NullFormatter();

        public IPygmentizeBuilder WithInputEncoding(Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public IPygmentizeBuilder WithOutputEncoding(Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public IPygmentizeBuilder WithLexer(Lexer lexer)
        {
            _lexer = lexer;
            return this;
        }

        public IPygmentizeBuilder WithFormatter(Formatter formatter)
        {
            _formatter = formatter;
            return this;
        }

        public string AsString()
        {
            var tokens = _lexer.GetTokens(_input);
            var memoryStream = new StringWriter();
            _formatter.Format(tokens, memoryStream);

            return memoryStream.ToString();
        }

        public void ToFile(string filename)
        {
            var formatter = FormatterLocator.FindByFilename(filename);
            var tokens = _lexer.GetTokens(_input);
            using (var output = new StreamWriter(File.OpenWrite(filename), Encoding.UTF8))
            {
                formatter.Format(tokens, output);
            }
        }
    }


    /// <summary>
    /// Main fluent interface starter for highlighting
    /// </summary>
    public static class Pygmentize
    {
        /// <summary>
        /// Starts building a highlighting operation against string content
        /// </summary>
        /// <param name="content">The text to highlight</param>
        /// <returns></returns>
        public static IPygmentizeBuilder Content(string content)
        {
            return new PygmentizeContentBuilder(content);
        }


        /// <summary>
        /// Starts building a highlighting operation against a file
        /// </summary>
        /// <param name="filename">The file to hightlight</param>
        /// <returns></returns>
        public static IPygmentizeBuilder File(string filename)
        {
            var lexer = LexerLocator.FindByFilename(filename);
            return new PygmentizeContentBuilder(System.IO.File.ReadAllText(filename))
                .WithLexer(lexer);
        }

    }
}
