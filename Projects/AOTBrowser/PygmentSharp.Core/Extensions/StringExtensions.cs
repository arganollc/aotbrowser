using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using PygmentSharp.Core.Utils;

namespace PygmentSharp.Core.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        ///     Port of the python idiom `result = s and "default"` where
        ///     the result is "default" if s is non empty
        /// </summary>
        /// <param name="s"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string PythonAnd(this string s, string def)
        {
            return string.IsNullOrEmpty(s) ? s : def;
        }

        /// <summary>
        ///     Port of the python idiom `result = s or "default" where
        ///     the result is "default" if s is null or empty
        /// </summary>
        /// <param name="s"></param>
        /// <param name="def">the default value</param>
        /// <returns></returns>
        public static string PythonOr(this string s, string def)
        {
            return string.IsNullOrEmpty(s) ? def : s;
        }

        /// <summary>
        ///     Gets a value indicating if the CSV string contains an element matching <paramref name="search" />
        /// </summary>
        /// <param name="s">the string to search</param>
        /// <param name="search">that string for which to find</param>
        /// <param name="comparison">(optional) provide string comparison rules</param>
        /// <returns></returns>
        public static bool CsvContains(this string s, string search, StringComparison comparison = default(StringComparison))
        {
            if (s == null)
            {
                return false;
            }

            return s
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Any(x => x.Equals(search, comparison));
        }

        /// <summary>
        /// Reverses a string
        /// </summary>
        /// <remarks>So named so as to not interfere with LINQ <c>Enumerable.Reverse</c>/></remarks>
        /// <param name="input">The string to reverse</param>
        /// <returns>The string in reverse</returns>
        public static string Backwards(this string input)
        {
            var sb = new StringBuilder(input.Length);

            for (var i = input.Length - 1; i >= 0; i--)
                sb.Append(input[i]);

            return sb.ToString();
        }

        /// <summary>
        ///     Gets a value indicating if a string matches a wildcard pattern, similar to what's
        ///     used in Windows file dialogs
        ///     <remarks>
        ///         <list type="bullet">
        ///             <item>
        ///                 <term>*</term>
        ///                 <description>Matches 0 or more of any character</description>
        ///             </item>
        ///             <item>
        ///                 <term>?</term>
        ///                 <description>Matches exactly one of any character</description>
        ///             </item>
        ///         </list>
        ///     </remarks>
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <param name="pattern">The patterm to match</param>
        /// <returns></returns>
        public static bool MatchesFileWildcard(this string s, string pattern)
        {
            Argument.EnsureNotNull(s, nameof(s));
            Argument.EnsureNotNull(pattern, nameof(pattern));

            var filename = Path.GetFileName(s);
            var regex = new Regex(pattern.Replace(".", "\\.").Replace("*", ".*").Replace("?", "."), RegexOptions.IgnoreCase);
            return regex.IsMatch(filename);
        }
    }
}