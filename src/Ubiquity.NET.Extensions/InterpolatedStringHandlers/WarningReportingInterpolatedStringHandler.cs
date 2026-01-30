// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions.InterpolatedStringHandlers
{
    // Unfortunately, in C#, structs can't inherit AND generics don't support a non-type constant argument (for the fixed level)
    // Therefore this must use classic OO containment instead.

    /// <summary>Interpolated string handler for an <see cref="IDiagnosticReporter"/> using a fixed <see cref="MessageLevel.Warning"/></summary>
    /// <inheritdoc cref="DiagnosticReporterInterpolatedStringHandler"/>
    [InterpolatedStringHandler]
    [SuppressMessage( "Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not relevant for an interpolated string handler" )]
    public readonly struct WarningReportingInterpolatedStringHandler
    {
        /// <summary>Initializes a new instance of the <see cref="WarningReportingInterpolatedStringHandler"/> struct.</summary>
        /// <inheritdoc cref="DiagnosticReporterInterpolatedStringHandler.DiagnosticReporterInterpolatedStringHandler(int, int, IDiagnosticReporter, MessageLevel, IFormatProvider?)"/>
        public WarningReportingInterpolatedStringHandler(
            int literalLength,
            int formattedCount,
            IDiagnosticReporter reporter,
            IFormatProvider? formatProvider = null
        )
        {
            InnerHandler = new DiagnosticReporterInterpolatedStringHandler(literalLength, formattedCount, reporter, MessageLevel.Warning, formatProvider);
        }

        /// <inheritdoc cref="DiagnosticReporterInterpolatedStringHandler.IsEnabled"/>
        public bool IsEnabled => InnerHandler.IsEnabled;

        /// <inheritdoc cref="DiagnosticReporterInterpolatedStringHandler.AppendLiteral"/>
        public bool AppendLiteral( string s )
        {
            return InnerHandler.AppendLiteral( s );
        }

        /// <inheritdoc cref="DiagnosticReporterInterpolatedStringHandler.AppendFormatted{T}(T)"/>
        public readonly bool AppendFormatted<T>( T t )
        {
            return InnerHandler.AppendFormatted<T>(t);
        }

        /// <inheritdoc cref="DiagnosticReporterInterpolatedStringHandler.AppendFormatted{T}(T, string)"/>
        public readonly bool AppendFormatted<T>( T t, string format )
            where T : IFormattable
        {
            return InnerHandler.AppendFormatted<T>(t, format);
        }

        /// <inheritdoc cref="DiagnosticReporterInterpolatedStringHandler.GetFormattedText"/>
        public string GetFormattedText( )
        {
            return InnerHandler.GetFormattedText( );
        }

        private readonly DiagnosticReporterInterpolatedStringHandler InnerHandler;
    }
}
