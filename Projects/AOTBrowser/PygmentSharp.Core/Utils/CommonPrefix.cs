using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Utils
{
    /// <summary>
    ///     Calculations for the longest common prefix of a list of strings
    ///     <remarks>
    ///         https://docs.python.org/2/library/os.path.html
    ///     </remarks>
    /// </summary>
    internal static class CommonPrefix
    {
        /// <summary>
        ///     Computes the longest common prefix of a list of <paramref name="strings" />.
        ///     The shortest common prefix is empty string
        /// </summary>
        /// <param name="strings">The list of strings to check</param>
        /// <returns>The common prefix</returns>
        public static string Of(params string[] strings)
        {
            return Of((IReadOnlyList<string>)strings);
        }

        public static string Of(IReadOnlyList<string> strings)
        {
            var sorted = strings.OrderBy(s => s.Length).ToArray();
            var check = sorted.First();

            while (check.Length > 0)
            {
                if (sorted.All(s => s.StartsWith(check, StringComparison.InvariantCulture)))
                {
                    return check;
                }

                check = check.Substring(0, check.Length - 1);
            }

            return check;
        }
    }
}