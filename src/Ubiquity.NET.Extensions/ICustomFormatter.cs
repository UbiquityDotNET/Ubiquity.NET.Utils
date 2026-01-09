// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions
{
    /// <summary>Custom formatter for a specific type</summary>
    /// <typeparam name="T">Type the formatter supports</typeparam>
    public interface ICustomFormatter<T>
        : ICustomFormatter
    {
        /// <summary>Converts the value of an instance of <typeparamref name="T"/> to an equivalent string representation</summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">The value to format</param>
        /// <param name="formatProvider">An object that supplies format information about the current instance.</param>
        /// <returns>The string representation of the value of arg, formatted as specified by format and formatProvider.</returns>
        string Format( string format, T arg, IFormatProvider formatProvider );
    }
}
