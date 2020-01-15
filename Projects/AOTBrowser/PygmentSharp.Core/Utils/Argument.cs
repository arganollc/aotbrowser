using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Utils
{
    internal static class Argument
    {
        /// <summary>
        /// Validates a value is not null
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="arg"/> is null</exception>
        /// <param name="arg">The argument to validate</param>
        /// <param name="argName">The name of the argument</param>
        // ReSharper disable once UnusedParameter.Global
        public static void EnsureNotNull(object arg, string argName)
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }
    }
}
