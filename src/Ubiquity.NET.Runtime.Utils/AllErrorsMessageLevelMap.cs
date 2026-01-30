// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Default implementation of <see cref="IMessageLevelMap"/> (Always returns <see cref="MessageLevel.Error"/></summary>
    public class AllErrorsMessageLevelMap
        : IMessageLevelMap
    {
        /// <inheritdoc/>
        public MessageLevel GetLevel( ScopedDiagnosticId id ) => MessageLevel.Error;

        /// <inheritdoc/>
        public MessageLevel GetLevel( int id ) => MessageLevel.Error;
    }
}
