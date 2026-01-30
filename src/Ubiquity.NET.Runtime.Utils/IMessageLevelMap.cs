// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Interface for an application provided severity mapping</summary>
    /// <remarks>
    /// Each application of a parser may have or need different severity values for
    /// a given unified error. Some may even allow user configuration of diagnostic
    /// messaging. This interface provides the extension point to allow such mapping.
    /// </remarks>
    public interface IMessageLevelMap
    {
        /// <summary>Gets the severity for a given <see cref="ScopedDiagnosticId"/></summary>
        /// <param name="id">Scoped id for the diagnostic</param>
        /// <returns>Severity or a default value if the diagnostic id isn't mapped. (MUST NOT THROW)</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="id"/> is the default value, or otherwise not mappable, then the result is a default.
        /// The exact value for a default depends on the implementation.
        /// </para>
        /// <para>This is provided to an API that allows per application instance mapping of severity levels for a
        /// given diagnostic ID. It is common for a "driver" application to allow user overrides for message severity
        /// levels. This allows conversion of the code to whatever the application wants. A default that maps all
        /// diagnostics to <see cref="MessageLevel.Error"/> is provided in <see cref="AllErrorsMessageLevelMap"/>.
        /// </para>
        /// </remarks>
        MessageLevel GetLevel(ScopedDiagnosticId id);

        /// <summary>Gets the severity for a given unified error ID</summary>
        /// <param name="id">Unified id for the error</param>
        /// <returns>Severity or a default value if the error id isn't mapped. (MUST NOT THROW)</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="id"/> is the default value, or otherwise not mappable, then the result is a default.
        /// The exact value for a default depends on the implementation.
        /// </para>
        /// <para>This is provided to an API that allows per application instance mapping of severity levels for a
        /// given diagnostic ID. It is common for a "driver" application to allow user overrides for message severity
        /// levels. This allows conversion of the code to whatever the application wants. A default that maps all
        /// errors to <see cref="MessageLevel.Error"/> is provided in <see cref="AllErrorsMessageLevelMap"/>.
        /// </para>
        /// </remarks>
        MessageLevel GetLevel( int id );
    }
}
