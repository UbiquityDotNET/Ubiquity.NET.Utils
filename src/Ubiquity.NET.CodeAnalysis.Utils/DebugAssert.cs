// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Diagnostics;

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Utility class to support Debug asserts</summary>
    public static class DebugAssert
    {
        /// <summary>Tests if a structure size is &lt; 16 bytes and generates a debug assertion if not</summary>
        /// <typeparam name="T">Type of the struct to test</typeparam>
        /// <remarks>
        /// <para>This uses a runtime debug assert as it isn't possible to know the size at compile time of a managed struct.
        /// The `sizeof` doesn't apply for anything with a managed reference or a native pointer sized member
        /// as such sizes depend on the actual runtime used.</para>
        /// <note type="important">
        /// This function ONLY operates in a debug build. That is, the compiler will elide calls to this method
        /// at the <em><b>call site</b></em> unless the "DEBUG" symbol is defined as it has a <see cref="ConditionalAttribute"/> attached to it.
        /// </note>
        /// </remarks>
        /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct"/>
        [Conditional( "DEBUG" )]
        public static void StructSizeOK<T>( )
            where T : struct
        {
            Debug.Assert( Unsafe.SizeOf<T>() <= 16, $"{nameof( T )} size is > 16 bytes; Make it a class" );
        }
    }
}
