using System;
using System.Collections.Generic;
using System.Linq;

using PygmentSharp.Core.Extensions;

namespace PygmentSharp.Core
{
    internal static class FormatterLocator
    {
        private static IEnumerable<Type> Formatters => AttributeLocator.GetTypesWithAttribute<FormatterAttribute>();

        public static Formatter FindByName(string name)
        {
            var type = Formatters.FirstOrDefault(l => HasFormatter(l, name));
            return type?.InstantiateAs<Formatter>();
        }

        public static Formatter FindByFilename(string filename)
        {
            var type = Formatters.FirstOrDefault(l => HasMatchingWildcard(l, filename));
            return type?.InstantiateAs<Formatter>();
        }

        private static bool HasMatchingWildcard(Type lexer, string file)
        {
             return lexer.HasAttribute<FormatterFileExtensionAttribute>(a =>
                file.MatchesFileWildcard(a.Pattern));
        }

        private static bool HasFormatter(Type l, string name)
        {
            return l.HasAttribute<FormatterAttribute>(a =>
                a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) ||
                a.AlternateNames.CsvContains(name, StringComparison.InvariantCultureIgnoreCase));
        }

    }
}