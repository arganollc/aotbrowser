using System;
using System.Collections.Generic;
using System.Linq;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Styles
{
    /// <summary>
    /// The default style configuration
    /// </summary>
    public class DefaultStyle : Style
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultStyle"/> class
        /// </summary>
        public DefaultStyle() : base(StyleMap)
        {
            BackgroundColor = "#f8f8f8";
        }

        private static IDictionary<TokenType, string> StyleMap => new Dictionary<TokenType, string>()
        {
            {TokenTypes.Whitespace, "#bbbbbb"},
            {TokenTypes.Comment, "italic #408080"},
            {TokenTypes.Comment.Preproc, "noitalic #BC7A00"},

            //{ TokenTypes.Keyword, "bold #AA22FF" },
            {TokenTypes.Keyword, "bold #008000"},
            {TokenTypes.Keyword.Pseudo, "nobold"},
            {TokenTypes.Keyword.Type, "nobold #B00040"},
            {TokenTypes.Operator, "#666666"},
            {TokenTypes.Operator.Word, "bold #AA22FF"},
            {TokenTypes.Name.Builtin, "#008000"},
            {TokenTypes.Name.Function, "#0000FF"},
            {TokenTypes.Name.Class, "bold #0000FF"},
            {TokenTypes.Name.Namespace, "bold #0000FF"},
            {TokenTypes.Name.Exception, "bold #D2413A"},
            {TokenTypes.Name.Variable, "#19177C"},
            {TokenTypes.Name.Constant, "#880000"},
            {TokenTypes.Name.Label, "#A0A000"},
            {TokenTypes.Name.Entity, "bold #999999"},
            {TokenTypes.Name.Attribute, "#7D9029"},
            {TokenTypes.Name.Tag, "bold #008000"},
            {TokenTypes.Name.Decorator, "#AA22FF"},

            {TokenTypes.String, "#BA2121"},
            {TokenTypes.String.Doc, "italic"},
            {TokenTypes.String.Interpol, "bold #BB6688"},
            {TokenTypes.String.Escape, "bold #BB6622"},
            {TokenTypes.String.Regex, "#BB6688"},
            //{ TokenTypes.String.Symbol, "#B8860B" },
            {TokenTypes.String.Symbol, "#19177C"},
            {TokenTypes.String.Other, "#008000"},
            {TokenTypes.Number, "#666666"},

            {TokenTypes.Generic.Heading, "bold #000080"},
            {TokenTypes.Generic.Subheading, "bold #800080"},
            {TokenTypes.Generic.Deleted, "#A00000"},
            {TokenTypes.Generic.Inserted, "#00A000"},
            {TokenTypes.Generic.Error, "#FF0000"},
            {TokenTypes.Generic.Emph, "italic"},
            {TokenTypes.Generic.Strong, "bold"},
            {TokenTypes.Generic.Prompt, "bold #000080"},
            {TokenTypes.Generic.Output, "#888"},
            {TokenTypes.Generic.Traceback, "#04D"},

            {TokenTypes.Error, "border:#FF0000"}
        };
    }
}
