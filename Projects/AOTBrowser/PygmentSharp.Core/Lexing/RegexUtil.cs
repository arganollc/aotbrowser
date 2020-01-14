using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using PygmentSharp.Core.Extensions;
using PygmentSharp.Core.Utils;

namespace PygmentSharp.Core.Lexing
{
    /// <summary>
    /// Utility methods for working with regular expressions
    /// </summary>
    public static class RegexUtil
    {
        private static readonly string[] AllClasses =
        {
            "Cc",
            "Cf",
            "Cn",
            "Co",
            "Cs",
            "Ll",
            "Lm",
            "Lo",
            "Lt",
            "Lu",
            "Mc",
            "Me",
            "Mn",
            "Nd",
            "Nl",
            "No",
            "Pc",
            "Pd",
            "Pe",
            "Pf",
            "Pi",
            "Po",
            "Ps",
            "Sc",
            "Sk",
            "Sm",
            "So",
            "Zl",
            "Zp",
            "Zs"
        };

        /// <summary>
        /// creates a regular expression match set that matches a character from unicode classes
        /// </summary>
        /// <param name="classes">The set of unicdeo classes to match characters from</param>
        /// <returns></returns>
        public static string Combine(params string[] classes)
        {
            return string.Join("",
                classes.Select(c => $@"\p{{{c}}}"));
        }

        /// <summary>
        /// Matches all characters except those in the provided unicode character classes
        /// </summary>
        /// <param name="classes">The unicode character classes to exclude</param>
        /// <returns></returns>
        public static string AllExcept(params string[] classes)
        {
            var passed = AllClasses.Where(c => !classes.Contains(c)).ToArray();
            return Combine(passed);
        }

        private const string CharsetEscaper = @"[\^\\\-\]]";

        /// <summary>
        ///     Creates a character set that could match any of the characters in the string
        /// </summary>
        /// <remarks>
        ///     For example: <c>"abc"</c> would become <c>"[abc]"</c>
        /// </remarks>
        /// <param name="letters">THe letters to be included in the character set</param>
        /// <returns>A regex string for the character set</returns>
        public static string MakeCharset(string letters)
        {
            return "[" +
                   Regex.Replace(letters, CharsetEscaper, m => "\\" + m.Value) +
                   "]";
        }

        /// <summary>
        ///     Creates a character set that could match any of the characters in the list
        /// </summary>
        /// <remarks>
        ///     For example: <c>{ "a","b","c" }</c> would become <c>"[abc]"</c>
        /// </remarks>
        /// <param name="letters">THe letters to be included in the character set</param>
        /// <returns>A regex string for the character set</returns>
        public static string MakeCharset(params char[] letters)
        {
            return MakeCharset(new string(letters));
        }

        /// <summary>
        ///     Returns a regex that matches any string in a list
        /// </summary>
        /// <remarks>
        ///     The strings to match must be literal strings -- they will be escaped.
        /// </remarks>
        /// <param name="strings">List of strings to match</param>
        /// <param name="prefix">a regex string to prepend</param>
        /// <param name="suffix">A regex string to append</param>
        /// <returns></returns>
        public static string OptimizedRegex(IEnumerable<string> strings, string prefix = "", string suffix = "")
        {
            var sorted = strings.OrderBy(s => s).ToArray();
            return prefix + OptimizedRegexInner(sorted, "(") + suffix;
        }


        private static string OptimizedRegexInner(Slice<string> strings, string openParen)
        {
            var closeParen = openParen.PythonAnd(")").PythonOr("");
            if (strings.Length == 0)
                return "";

            var first = strings[0];
            if (strings.Length == 1)
                return openParen + Escape(first) + closeParen;

            if (string.IsNullOrEmpty(first))
            {
                //first string empty
                return openParen + OptimizedRegexInner(strings.SubSlice(1), "(?:") +
                       "?" + closeParen;
            }

            if (first.Length == 1)
            {
                //multiple one-char strings? make a charset
                StringBuilder oneletter = new StringBuilder();
                List<string> rest = new List<string>();

                foreach (var s in strings)
                {
                    if (s.Length == 1)
                        oneletter.Append(s);
                    else
                    {
                        rest.Add(s);
                    }
                }

                if (oneletter.Length > 1) // do we have more than one oneletter string?
                {
                    if (rest.Count > 0)
                    {
                        // 1-character + rest
                        return openParen + OptimizedRegexInner(rest.ToArray(), "") +
                               "|" + MakeCharset(oneletter.ToString()) + closeParen;
                    }
                    return MakeCharset(oneletter.ToString());
                }
            }

            var prefix = CommonPrefix.Of(strings);
            if (prefix.Length > 0)
            {
                var plen = prefix.Length;
                // we have a prefix for all strings
                //  print '-> prefix:', prefix
                return openParen + Escape(prefix)
                       + OptimizedRegexInner(strings.Select(s => s.Substring(plen)).ToArray(), "(?:")
                       + closeParen;
            }

            // is there a suffix?
            var stringsReversed = strings.Select(s => s.Backwards()).ToArray();
            var suffix = CommonPrefix.Of(stringsReversed);
            if (suffix.Length > 0)
            {
                var slen = suffix.Length;
                return openParen
                    + OptimizedRegexInner(strings.Select(s => s.Substring(s.Length - slen)).ToArray(), "(?:")
                    + Escape(suffix.Backwards()) + closeParen;
            }

            //# recurse on common 1-string prefixes
            //# print '-> last resort'

            var groups = strings.GroupBy(s => s[0] == first[0]);


            return openParen +
                string.Join("|", groups.Select(g => OptimizedRegexInner(g.ToArray(), ""))) +
                closeParen;
        }


        private static string Escape(string s)
        {
            return Regex.Escape(s);
        }

        /// <summary>
        /// Creates an optimized regex that will match any of the listed words
        /// </summary>
        /// <param name="words"></param>
        /// <param name="prefix">A regex prefix to insert at the front of the optimized regex</param>
        /// <param name="suffix">A regex suffix to append at the end of the optimized regex</param>
        /// <returns></returns>
        public static string Words(string[] words, string prefix = "", string suffix = "")
        {
            return OptimizedRegex(words, prefix, suffix);
        }
    }

}