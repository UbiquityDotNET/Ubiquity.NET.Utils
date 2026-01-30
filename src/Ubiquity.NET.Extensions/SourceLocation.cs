// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions
{
    /// <summary>Represents a location in a source file</summary>
    public readonly record struct SourceLocation
        : IFormattable
    {
        /// <summary>Initializes a new instance of the <see cref="SourceLocation"/> struct.</summary>
        /// <param name="source">Reference to the source file (if any)</param>
        /// <param name="range">Range in the source</param>
        [SetsRequiredMembers]
        public SourceLocation( Uri? source, SourceRange range )
        {
            Source = source;
            Range = range;
        }

        /// <summary>Initializes a new instance of the <see cref="SourceLocation"/> struct.</summary>
        /// <param name="range">Range in the source</param>
        [SetsRequiredMembers]
        public SourceLocation( SourceRange range )
            : this( (Uri?)null, range )
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SourceLocation"/> struct.</summary>
        /// <param name="source">Reference to the source file (if any)</param>
        /// <param name="start">Position for the start of the range</param>
        /// <param name="end">Position for the end of the line</param>
        /// <remarks>
        /// If <paramref name="end"/> is <see langword="default"/> then this creates a simple range
        /// that cannot slice (<see cref="SourceRange.CanSlice"/> is <see langword="false"/>).
        /// </remarks>
        [SetsRequiredMembers]
        public SourceLocation( Uri? source, SourcePosition start, SourcePosition end = default )
            : this( source, new SourceRange( start, end ) )
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SourceLocation"/> struct.</summary>
        /// <param name="start">Position for the start of the range</param>
        /// <param name="end">Position for the end of the line</param>
        /// <remarks>
        /// If <paramref name="end"/> is <see langword="default"/> then this creates a simple range
        /// that cannot slice (<see cref="SourceRange.CanSlice"/> is <see langword="false"/>).
        /// </remarks>
        [SetsRequiredMembers]
        public SourceLocation( SourcePosition start, SourcePosition end = default )
            : this( (Uri?)null, new SourceRange( start, end ) )
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SourceLocation"/> struct.</summary>
        /// <param name="filePath">Path to the source file</param>
        /// <param name="range">Range in the source</param>
        [SetsRequiredMembers]
        public SourceLocation( string? filePath, SourceRange range )
            : this( filePath is null ? new Uri( $"file://{filePath}" ) : null, range )
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SourceLocation"/> struct.</summary>
        /// <param name="filePath">Path to the source file</param>
        [SetsRequiredMembers]
        public SourceLocation( string? filePath )
            : this( filePath, default )
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SourceLocation"/> struct.</summary>
        /// <param name="source">Path of the source file</param>
        /// <param name="start">Position for the start of the range</param>
        /// <param name="end">Position for the end of the line</param>
        /// <remarks>
        /// If <paramref name="end"/> is <see langword="default"/> then this creates a simple range
        /// that cannot slice (<see cref="SourceRange.CanSlice"/> is <see langword="false"/>).
        /// </remarks>
        [SetsRequiredMembers]
        public SourceLocation( string? source, SourcePosition start, SourcePosition end = default )
            : this( source, new SourceRange( start, end ) )
        {
        }

        /// <summary>Gets the source file location, if available</summary>
        public Uri? Source { get; }

        /// <summary>Gets the location in the source</summary>
        public required SourceRange Range { get; init; }

        /// <summary>Offset this location by amounts specified</summary>
        /// <param name="offset">relative location to offset</param>
        /// <returns>New location offset from this instance</returns>
        /// <remarks>
        /// In essence this location is considered an absolute location and <paramref name="offset"/>
        /// is relative to it. The result is that of adding the relative offset to this location.
        /// </remarks>
        public SourceLocation Offset( SourceLocation offset )
        {
            return Offset( this, offset );
        }

        /// <summary>Formats this location using the current culture and the generic format</summary>
        /// <returns>Formatted string</returns>
        /// <remarks>
        /// If specific formatting is required then use <see cref="ToString(string?, IFormatProvider?)"/>
        /// instead.
        /// <note type="tip">
        /// This ONLY formats the source location and the range portion of a full message. That is it formats
        /// the location represented by this instance. Additional formatting and content is usually required
        /// to form a full diagnostic message.
        /// </note>
        /// </remarks>
        public override string ToString( )
        {
            // use runtime default formatting
            return ToString( "G", CultureInfo.CurrentCulture );
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Accepted format strings are:
        /// <list type="table">
        /// <listheader><term>format string</term><term>Description</term></listheader>
        /// <item><term>M</term><term>MSBuild format used for Windows build tools.</term></item>
        /// <item><term>G</term><term>Runtime specific (For Windows, this is the MSBuild format)</term></item>
        /// </list>
        /// [Format strings for other runtimes TBD]
        /// </remarks>
        public string ToString( string? format, IFormatProvider? formatProvider )
        {
            formatProvider ??= CultureInfo.CurrentCulture;
            return format switch
            {
                "M" => FormatMsBuild( formatProvider ),
                "G" => FormatRuntime( formatProvider ),
                _ => ToString()
            };
        }

        /// <summary>Offset an absolute location by amounts specified in a relative offset</summary>
        /// <param name="baseValue">Base/Absolute location to apply the offset to</param>
        /// <param name="offset">relative location to offset</param>
        /// <returns>New location offset from <paramref name="baseValue"/></returns>
        /// <remarks>
        /// In essence <paramref name="baseValue"/> is considered an absolute location and <paramref name="offset"/>
        /// is relative to it. The result is that of adding the relative offset to the absolute location.
        /// </remarks>
        public static SourceLocation Offset( SourceLocation baseValue, SourceLocation offset )
        {
            // Intentionally not verifying or using the source name as the result is ALWAYS that
            // of the  base value. Offsetting is most commonly used to test a sub string of an original
            // stream and that sub string doesn't usually refer to the original stream... So in that
            // case, which the offsetting feature is designed for, the sources are ALWAYS different.
            return new SourceLocation( baseValue.Source, SourceRange.Offset( baseValue.Range, offset.Range ) );
        }

        // SEE: https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-diagnostic-format-for-tasks?view=vs-2022
        [SuppressMessage( "Style", "IDE0046:Convert to conditional expression", Justification = "Result is NOT simpler" )]
        private string FormatMsBuild( IFormatProvider? formatProvider )
        {
#if NET6_0_OR_GREATER // string.Create (Interpolated strings)
            return string.Create(formatProvider, $"{Source?.LocalPath ?? "<unknown source>"}{Range:M}");
#else
            return new StringBuilder(Source?.LocalPath ?? "<unknown source>")
                   .AppendFormat(formatProvider, "{0:M}: ", Range)
                   .ToString();
#endif
        }

        [SuppressMessage( "Style", "IDE0046:Convert to conditional expression", Justification = "Place holder for future work" )]
        [SuppressMessage( "StyleCop.CSharp.ReadabilityRules", "SA1108:BlockStatementsMustNotContainEmbeddedComments", Justification = "Reviewed." )]
        private string FormatRuntime( IFormatProvider? formatProvider )
        {
#if NET5_0_OR_GREATER
            bool isWindows = OperatingSystem.IsWindows();
#else
            bool isWindows = PolyFillOperatingSystem.IsWindows();
#endif
            if(isWindows)
            {
                return FormatMsBuild( formatProvider );
            }
            else // TODO: Adjust this to format based on styles of additional runtimes
            {
                // for now - always use MSBUILD format
                return FormatMsBuild( formatProvider );
            }
        }
    }
}
