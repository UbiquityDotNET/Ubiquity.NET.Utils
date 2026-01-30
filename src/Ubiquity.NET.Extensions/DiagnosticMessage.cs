// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions
{
    /// <summary>Tool Message category</summary>
    public enum MessageLevel
    {
        /// <summary>All channels off</summary>
        None = 0,

        /// <summary>Verbose messages (or higher) are enabled</summary>
        Verbose = 100,

        /// <summary>Informational messages (or higher) are enabled.</summary>
        Information = 200,

        /// <summary>Warning messages (or higher) are enabled. [This is the default value]</summary>
        Warning = 300, // Default level is warning & error only

        /// <summary>Error messages (or higher) are enabled.</summary>
        Error = 400,
    }

    /// <summary>Diagnostic message for reporting diagnostics as part of end-user facing experience (UX)</summary>
    /// <remarks>
    /// <see cref="Code"/> must NOT be localized. It is required to universally identify a
    /// particular message. <see cref="Text"/> should be localized if the source tool supports
    /// localization.
    /// <para>Diagnostic codes (<see cref="Code"/>) are of the form &lt;prefix&gt;&lt;number&gt;
    /// (example: FOO01234). This is a unique identifier for the message that allows a user to reference
    /// it for support or other diagnostic analysis. The &lt;prefix&gt; portion of the code indicates
    /// the application source of the message.</para>
    /// </remarks>
    public readonly record struct DiagnosticMessage
        : IFormattable
    {
        /// <summary>Gets the location for the origin of this message</summary>
        public SourceLocation SourceLocation { get; init; }

#if NET10_0_OR_GREATER
        /// <summary>Gets the subcategory of the message</summary>
        public string? Subcategory
        {
            get;
            init
            {
                if(value is not null && value.Any( ( c ) => char.IsWhiteSpace( c ) ))
                {
                    throw new ArgumentException( "If provided, value must not contain whitespace", nameof( value ) );
                }

                field = value;
            }
        }

        /// <summary>Gets the Level/Category of the message</summary>
        public MessageLevel Level
        {
            get;
            init
            {
                value.ThrowIfNotDefined();
                if(value == MessageLevel.None)
                {
                    throw new InvalidEnumArgumentException( nameof( value ), 0, typeof( MessageLevel ) );
                }

                field = value;
            }
        }

        /// <summary>Gets the code for the message (No spaces)</summary>
        public string? Code
        {
            get;
            init
            {
                if(value is not null && value.Any( ( c ) => char.IsWhiteSpace( c ) ))
                {
                    throw new ArgumentException( "If provided, value must not contain whitespace", nameof( value ) );
                }

                field = value;
            }
        }

        /// <summary>Gets the text of the message</summary>
        public string Text
        {
            get;
            init
            {
                ArgumentException.ThrowIfNullOrWhiteSpace( value);
                field = value;
            }
        }
#else
        /// <summary>Gets the subcategory of the message</summary>
        public string? Subcategory
        {
            get => SubcategoryBackingField;
            init
            {
                if(value is not null && value.Any( ( c ) => char.IsWhiteSpace( c ) ))
                {
                    throw new ArgumentException( "If provided, value must not contain whitespace", nameof( value ) );
                }

                SubcategoryBackingField = value;
            }
        }

        /// <summary>Gets the Level/Category of the message</summary>
        public MessageLevel Level
        {
            get => LevelBackingField;
            init
            {
                value.ThrowIfNotDefined();
                if(value == MessageLevel.None)
                {
                    throw new InvalidEnumArgumentException( nameof( value ), 0, typeof( MessageLevel ) );
                }

                LevelBackingField = value;
            }
        }

        /// <summary>Gets the code for the message (No spaces)</summary>
        public string? Code
        {
            get => CodeBackingField;
            init
            {
                if(value is not null && value.Any( ( c ) => char.IsWhiteSpace( c ) ))
                {
                    throw new ArgumentException( "If provided, value must not contain whitespace", nameof( value ) );
                }

                CodeBackingField = value;
            }
        }

        /// <summary>Gets the text of the message</summary>
        public string Text
        {
            get => TextBackingField;
            init
            {
                Requires.NotNullOrWhiteSpace( value );
                TextBackingField = value;
            }
        }

#pragma warning disable IDE0032 // Use auto property
        private readonly string? SubcategoryBackingField;
        private readonly MessageLevel LevelBackingField;
        private readonly string? CodeBackingField;
        private readonly string TextBackingField;
#pragma warning restore IDE0032 // Use auto property
#endif

        /// <summary>Formats this instance using the general runtime specific format</summary>
        /// <returns>Formatted string for the message</returns>
        public override string ToString( )
        {
            // use runtime default formatting
            return ToString("G", CultureInfo.CurrentCulture);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Accepted format strings are:
        /// "M" for MSBuild format (used for Windows/MSBUILD build tools).
        /// "G" for runtime specific (For Windows, this is the same as the MSBuild format['M'])
        /// [Format strings for other runtimes TBD (L:Linux, A:Apple ... ????)]
        /// </remarks>
        /// <seealso href="https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-diagnostic-format-for-tasks"/>
        public string ToString( string? format, IFormatProvider? formatProvider )
        {
            formatProvider ??= CultureInfo.CurrentCulture;
            return format switch
            {
                "M" => FormatMsBuild( ),
                "G" => FormatRuntime( formatProvider ),
                _ => throw new FormatException($"{format} is not a valid format specifier for {nameof(DiagnosticMessage)}")
            };
        }

        // Does not use a format provider as the format is specified by external sources
        private string FormatMsBuild()
        {
            if(SourceLocation.Source is null || string.IsNullOrWhiteSpace(SourceLocation.Source.AbsoluteUri))
            {
                return Text;
            }

            string locString = string.Empty;
            if(SourceLocation != default)
            {
                locString = SourceLocation.Range.ToString( "M", CultureInfo.InvariantCulture );
            }

            // account for optional values with leading space so that values not provided use no space
            string subCat = Subcategory is not null ? $" {Subcategory}" : string.Empty;
            string code = Code is not null ? $" {Code}" : string.Empty;
            string origin = SourceLocation.Source.IsFile ? SourceLocation.Source.LocalPath : SourceLocation.Source.ToString();
            return $"{origin}{locString} :{subCat} {Level}{code} : {Text}";
        }

        [SuppressMessage( "Style", "IDE0046:Convert to conditional expression", Justification = "Place holder for future work" )]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1108:BlockStatementsMustNotContainEmbeddedComments", Justification = "Reviewed.")]
        private string FormatRuntime(IFormatProvider _)
        {
#if NETSTANDARD2_0
            bool isWindows = PolyFillOperatingSystem.IsWindows();
#else
            bool isWindows = OperatingSystem.IsWindows();
#endif
            if(isWindows)
            {
                return FormatMsBuild();
            }
            else // TODO: Adjust this to format based on styles of additional runtimes
            {
                // For now - always use MSBUILD format
                // platform specific formatting MAY use formatter (but probably doesn't)
                return FormatMsBuild(/*formatProvider*/);
            }
        }
    }
}
