// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions
{
    /// <summary>Process related extensions/support</summary>
    public static class ProcessInfo
    {
#if NET10_0_OR_GREATER // "field keyword"
        /// <summary>Gets the active assembly as of the first use of this property</summary>
        /// <remarks>
        /// The active assembly is the entry assembly which may be null if called from native
        /// code as no such assembly exists for that scenario.
        /// </remarks>
        public static Assembly? ActiveAssembly => field ??= Assembly.GetEntryAssembly();

#elif NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER // nullability annotations
        /// <summary>Gets the active assembly as of the first use of this property</summary>
        /// <remarks>
        /// The active assembly is the entry assembly which may be null if called from native
        /// code as no such assembly exists for that scenario.
        /// </remarks>
        public static Assembly? ActiveAssembly => field ??= Assembly.GetEntryAssembly();
#else // Legacy build
        /// <summary>Gets the active assembly as of the first use of this property</summary>
        /// <remarks>
        /// The active assembly is the entry assembly which may be null if called from native
        /// code as no such assembly exists for that scenario.
        /// </remarks>
        public static Assembly ActiveAssembly
        {
            get
            {
                if(ActiveAssemblyBackingField == null)
                {
                    ActiveAssemblyBackingField = Assembly.GetEntryAssembly();
                }

                return ActiveAssemblyBackingField;
            }
        }

        // Again, [Sigh!] compiler/IDE is @#$% confused, it doesn't take language version of the target runtime into account.
        // They sure do make multi-targeting REALLY HARD. On the one hand they say "don't set 'LangVersion' in the project",
        // on the other they do dumb S#$% like this!
        [SuppressMessage( "Style", "IDE0032:Use auto property", Justification = "Can't do that in C# 7.3 analyzer isn't taking language version into account" )]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private static Assembly ActiveAssemblyBackingField;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#endif

        /// <summary>Gets the executable path for this instance of an application</summary>
        /// <remarks>This is a short hand for <see cref="Environment.GetCommandLineArgs()"/>[ 0 ]</remarks>
        public static string ExecutablePath => Environment.GetCommandLineArgs()[ 0 ];

        /// <summary>Gets the name of the executable for this instance of an application</summary>
        /// <remarks>
        /// This is a short hand for <see cref="Path.GetFileNameWithoutExtension(string)"/> using
        /// <see cref="ExecutablePath"/> as the path.
        /// </remarks>
        public static string ExecutableName => Path.GetFileNameWithoutExtension( ExecutablePath );
    }
}
