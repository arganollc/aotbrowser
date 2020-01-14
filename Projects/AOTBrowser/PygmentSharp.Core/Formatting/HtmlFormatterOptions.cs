using System;
using System.Collections.Generic;
using System.Linq;

using PygmentSharp.Core.Styles;

namespace PygmentSharp.Core.Formatting
{
    /// <summary>
    /// Holds options for HTML formatting
    /// </summary>
    public class HtmlFormatterOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlFormatterOptions"/> class
        /// </summary>
        public HtmlFormatterOptions()
        {
            HighlightLines = Enumerable.Empty<int>();
        }

        /// <summary>
        /// Gets or sets a value indicating if lines should be wrapped
        /// </summary>
        public bool NoWrap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if a full output page should be generated, including header and footer and style blocks
        /// </summary>
        public bool Full { get; set; }

        /// <summary>
        /// Gets or sets the title of the generated page. Only relevant if <see cref="Full"/> is true
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if tokens should be wrapped in inline styles rather than CSS classes
        /// </summary>
        public bool NoClasses { get; set; }

        /// <summary>
        /// Specifies a prefix to prepend to all css classes, in case of collisions with existing styles
        /// </summary>
        public string ClassPrefix { get; set; }

        /// <summary>
        /// The CSS class to apply to the main wrapping div
        /// </summary>
        public string CssClass { get; set; } = "highlight";

        /// <summary>
        /// Specifies inline styles to use on all pre tags
        /// </summary>
        public string PreStyles { get; set; } = "";


        // Not yet supported, but referenced in Python source
        //public string CssFile { get; set; }
        //public bool NoClobberCssFile { get; set; }

        /// <summary>
        /// Specifies the type of line numbers to generate
        /// </summary>
        public LineNumberStyle LineNumbers { get; set; } = LineNumberStyle.None;

        /// <summary>
        /// Specifies what line number to start at
        /// </summary>
        public int LineNumberStart { get; set; } = 1;

        /// <summary>
        /// Specifies what step to use between line numbers. Only every nth line number is is shown
        /// </summary>
        public int LineNumberStep { get; set; } = 1;

        /// <summary>
        /// If set to non zero value, every nth line number is given css class "special"
        /// </summary>
        public int LineNumberSpecial { get; set; } = 0;

        /// <summary>
        /// Gets a value indicating if the background color should be skipped
        /// </summary>
        public bool NoBackground { get; set; } = false;

        /// <summary>
        /// Gets the string to separate each line with
        /// </summary>
        public string LineSeparator { get; set; } = "\n";

        /// <summary>
        /// If set, each source line will be wrapped in span with an anchor with target "$value-$linenumber" to allow linking
        /// </summary>
        public string LineAnchors { get; set; } = "";

        /// <summary>
        /// If set, each source line will be wrapped in span with id "$value-$linenumber"
        /// </summary>
        public string LineSpans { get; set; } = "";

        /// <summary>
        /// If set, each line number will be a link
        /// </summary>
        public bool AnchorLineNumbers { get; set; } = false;

        /// <summary>
        /// If set, displays the name of the file being highlighted
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// If set, specifies a set of line numbers that should be highlighted
        /// </summary>
        public IEnumerable<int> HighlightLines { get; set; }

        /// <summary>
        /// Specifies inline styles for the wrapper div
        /// </summary>
        public string CssStyles { get; set; }

        /// <summary>
        /// Specifies the colors for the CSS
        /// </summary>
        public Style Style { get; set; }
    }

    /// <summary>
    /// Indicates the mode in which lines numbers should be displayed
    /// </summary>
    public enum LineNumberStyle
    {
        /// <summary>
        /// No line numbers displayed
        /// </summary>
        None,

        /// <summary>
        /// Line numbers showin in a table
        /// </summary>
        Table,

        /// <summary>
        /// Line numbers shown inline
        /// </summary>
        Inline
    }
}