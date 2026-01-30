// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Scoped error ID</summary>
    /// <remarks>
    /// This library supports three distinct forms of a diagnostic ID.<br/>
    /// 1) The scoped form (this type)<br/>
    /// 2) The unified form (a simple <see cref="int"/>)<br/>
    /// 3) String form used in UX and <see cref="DiagnosticMessage"/> instances.<br/>
    /// <para>
    /// This allows the parse scopes to remain independent of each other. They can use diagnostic IDs that overlap.
    /// <see cref="IDiagnosticIdMap"/> is used to provide application specific unification of the scoped ID into a
    /// unique <see cref="int"/> value. [For example adding an offset to the ID based on the scope etc...] This
    /// mapping is provided by the application as an extensibility point that allows custom handling of the ID value.
    /// </para><para>
    /// Creation of a diagnostic message will use an instance of <see cref="IDiagnosticIdFormatter"/> to
    /// format an instance of this type into a string form expected. This is usually an instance of
    /// <see cref="DefaultPrefixFormatter"/> but an application is free to use any formatting needed.
    /// </para>
    /// </remarks>
    public readonly record struct ScopedDiagnosticId
    {
        /// <summary>Initializes a new instance of the <see cref="ScopedDiagnosticId"/> struct.</summary>
        /// <param name="scope">Source scope for the ID</param>
        /// <param name="code">Id relative to <paramref name="scope"/></param>
        /// <remarks>
        /// This represents a scoped ID code, to get a singular integral value callers can use
        /// an instance of <see cref="IDiagnosticIdMap"/> to get a unified integral value or use
        /// an instance of <see cref="IDiagnosticIdFormatter"/> to get a string form. For convenience
        /// the <see cref="AsDiagnostic(IMessageLevelMap, IDiagnosticIdFormatter, string, SourceLocation?, string?)"/>
        /// method provides a standard means of producing a <see cref="DiagnosticMessage"/> from an instance of
        /// a <see cref="ScopedDiagnosticId"/>.
        /// </remarks>
        public ScopedDiagnosticId(ParseSource scope, int code)
        {
            Scope = scope;
            Code = code;
        }

        /// <summary>Gets the scope of the ID</summary>
        public ParseSource Scope { get; }

        /// <summary>Gets the scoped ID value</summary>
        public int Code { get; }

        /// <summary>Creates a <see cref="DiagnosticMessage"/> from this instance and provided parameters</summary>
        /// <param name="messageLevelMap">Map to use for determining the message level</param>
        /// <param name="formatter">Formatter to use for conversion of this instance to a string form</param>
        /// <param name="message">Text message of the diagnostic</param>
        /// <param name="location">Location of the diagnostic</param>
        /// <param name="subcategory">Subcategory of the diagnostic</param>
        /// <returns><see cref="DiagnosticMessage"/> formed from this instance and the parameters provided.</returns>
        public DiagnosticMessage AsDiagnostic(
            IMessageLevelMap messageLevelMap,
            IDiagnosticIdFormatter formatter,
            string message,
            SourceLocation? location = null,
            string? subcategory = null
            )
        {
            return new DiagnosticMessage()
            {
                Code = formatter.FormatCode(this),
                Level = messageLevelMap.GetLevel(this),
                SourceLocation = location ?? default,
                Subcategory = subcategory,
                Text = message,
            };
        }
    }
}
