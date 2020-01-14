using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PygmentSharp.Core.Extensions
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Creates an instance of the type. Useful for when you know the base type at compile time, but not necessarily the derived type
        /// </summary>
        /// <typeparam name="T">The type to create</typeparam>
        /// <param name="type">The type to create</param>
        /// <returns>A new instance of the specified type</returns>
        public static T InstantiateAs<T>(this Type type)
        {
            return (T)Activator.CreateInstance(type);
        }

        /// <summary>
        /// Gets a value indicating if the <see cref="Type"/> has an <see cref="Attribute"/> matching a pattern
        /// </summary>
        /// <typeparam name="TAttr">The type of attributes to check</typeparam>
        /// <param name="type">The type to search</param>
        /// <param name="predicate">A prediciate to apply to the attribute</param>
        /// <returns>true if the type has a matching <see cref="Attribute"/>, false otherwise</returns>
        public static bool HasAttribute<TAttr>(this Type type, Func<TAttr, bool> predicate) where TAttr:Attribute
        {
            return type.GetCustomAttributes<TAttr>()
                .Any(predicate);
        }
    }
}
