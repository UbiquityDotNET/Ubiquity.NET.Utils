// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine
{
    /// <summary>Utility class to host extensions to <see cref="ICommandLineOptions{T}"/></summary>
    /// <remarks>
    /// If C# 14 and the `extension` everything feature is supported these will leverage it such that
    /// the class name is not needed. Otherwise, for older targets the type name is required.
    /// While default static methods on an interface is allowed, use of them requires explicitly
    /// using the interface name. Additionally, since the interface is generic, this runs afoul of
    /// `CA1000: Do not declare static members on generic types`. Thus, this compromise is used to
    /// at least simplify the usage as much as possible.
    /// </remarks>
    public static class CommandLineOptions
    {
#if NET10_0_OR_GREATER
        /// <summary>C# 14 static Extensions for common methods support</summary>
        /// <typeparam name="T">Type to bind the options to</typeparam>
        extension<T>( ICommandLineOptions<T> )
            where T : ICommandLineOptions<T>
        {
            /// <summary>Build a root command with handler</summary>
            /// <param name="settings">Settings to use for the command</param>
            /// <param name="action">Action to handle the command and returns an exit code</param>
            /// <returns><see cref="AppControlledDefaultsRootCommand"/> built with <paramref name="action"/> as the handler.</returns>
            public static AppControlledDefaultsRootCommand BuildRootCommand( CmdLineSettings settings, Func<T, int> action )
            {
                var retVal = T.BuildRootCommand(settings);
                retVal.SetAction( pr => action( T.Bind( pr ) ) );
                return retVal;
            }

            public static AppControlledDefaultsRootCommand BuildRootCommand( CmdLineSettings settings, Func<T, CancellationToken, Task<int>> action )
            {
                var retVal = T.BuildRootCommand(settings);
                retVal.SetAction( (pr, ct) => action( T.Bind( pr ), ct ) );
                return retVal;
            }

            public static AppControlledDefaultsRootCommand BuildRootCommand( CmdLineSettings settings, Func<T, CancellationToken, Task> action )
            {
                var retVal = T.BuildRootCommand(settings);
                retVal.SetAction( (pr, ct) => action( T.Bind( pr ), ct ) );
                return retVal;
            }

            public static AppControlledDefaultsRootCommand BuildRootCommand( CmdLineSettings settings, Action<T> action )
            {
                var retVal = T.BuildRootCommand(settings);
                retVal.SetAction( pr => action( T.Bind( pr ) ) );
                return retVal;
            }
        }
#else
        /// <summary>Build a root command with a synchronous handler</summary>
        /// <typeparam name="T">Type to bind the options to</typeparam>
        /// <param name="settings">Settings to use for the command</param>
        /// <param name="action">Action to handle the command and returns an exit code</param>
        /// <returns><see cref="AppControlledDefaultsRootCommand"/> built with <paramref name="action"/> as the handler.</returns>
        public static AppControlledDefaultsRootCommand BuildRootCommand<T>( CmdLineSettings settings, Func<T, int> action )
            where T : ICommandLineOptions<T>
        {
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentNullException.ThrowIfNull(action);

            var retVal = T.BuildRootCommand(settings);
            retVal.SetAction( pr => action( T.Bind( pr ) ) );
            return retVal;
        }

        /// <summary>Build a root command with async handler that returns an application specific exit code (0 == Success/No Error)</summary>
        /// <typeparam name="T">Type to bind the options to</typeparam>
        /// <param name="settings">Settings to use for the command</param>
        /// <param name="action">Async action to handle the command and returns an exit code (via <see cref="Task{TResult}"/>)</param>
        /// <returns><see cref="AppControlledDefaultsRootCommand"/> built with <paramref name="action"/> as the handler.</returns>
        public static AppControlledDefaultsRootCommand BuildRootCommand<T>( CmdLineSettings settings, Func<T, CancellationToken, Task<int>> action )
            where T : ICommandLineOptions<T>
        {
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentNullException.ThrowIfNull(action);

            var retVal = T.BuildRootCommand(settings);
            retVal.SetAction( (pr, ct) => action( T.Bind( pr ), ct ) );
            return retVal;
        }

        /// <summary>Build a root command with async handler</summary>
        /// <typeparam name="T">Type to bind the options to</typeparam>
        /// <param name="settings">Settings to use for the command</param>
        /// <param name="action">Action to handle the command and returns an exit code</param>
        /// <returns><see cref="AppControlledDefaultsRootCommand"/> built with <paramref name="action"/> as the handler.</returns>
        public static AppControlledDefaultsRootCommand BuildRootCommand<T>( CmdLineSettings settings, Func<T, CancellationToken, Task> action )
            where T : ICommandLineOptions<T>
        {
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentNullException.ThrowIfNull(action);

            var retVal = T.BuildRootCommand(settings);
            retVal.SetAction( (pr, ct) => action( T.Bind( pr ), ct ) );
            return retVal;
        }

        /// <summary>Build a root command with a synchronous handler</summary>
        /// <typeparam name="T">Type to bind the options to</typeparam>
        /// <param name="settings">Settings to use for the command</param>
        /// <param name="action">Action to handle the command</param>
        /// <returns><see cref="AppControlledDefaultsRootCommand"/> built with <paramref name="action"/> as the handler.</returns>
        public static AppControlledDefaultsRootCommand BuildRootCommand<T>( CmdLineSettings settings, Action<T> action )
            where T : ICommandLineOptions<T>
        {
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentNullException.ThrowIfNull(action);

            var retVal = T.BuildRootCommand(settings);
            retVal.SetAction( pr => action( T.Bind( pr ) ) );
            return retVal;
        }
#endif
    }
}
