using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PygmentSharp.Core
{
    /// <summary>
    /// Finds classes with a given attribute
    /// </summary>
    internal static class AttributeLocator
    {
        public static IReadOnlyCollection<Type> GetTypesWithAttribute<TAttr>() where TAttr : Attribute
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes()
                .Where(t => t.GetCustomAttributes<TAttr>(true).Any())
                .ToList();
        }
    }
}