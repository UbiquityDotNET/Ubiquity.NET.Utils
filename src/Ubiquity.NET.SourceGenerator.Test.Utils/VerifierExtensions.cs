// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace Ubiquity.NET.SourceGenerator.Test.Utils
{
    /// <summary>Utility class to implement extensions for <see cref="IVerifier"/></summary>
    [SuppressMessage( "Design", "CA1034: Nested types should not be visible", Justification = "Bogus; extension see: https://github.com/dotnet/sdk/issues/51681, and https://github.com/dotnet/roslyn-analyzers/issues/7765" )]
    public static class VerifierExtensions
    {
        extension( IVerifier Verify )
        {
            /// <summary>Verifies a collection has a specific count</summary>
            /// <typeparam name="T">Type of elements</typeparam>
            /// <param name="expected">Expected count of the elements in the collection</param>
            /// <param name="collection">Collection to test</param>
            /// <param name="message">Message to include in assertions</param>
            /// <param name="collectionExpression">Expression for the collection; Normally provided by the compiler</param>
            public void HasCount<T>(
                int expected,
                IEnumerable<T> collection,
                string? message = null,
                [CallerArgumentExpression( nameof( collection ) )] string collectionExpression = "" )
            {
                int count = collection.Count();
                Verify.Equal( expected, count, message ?? $"{collectionExpression} [Count: {count}] does not have expected count '{expected}'" );
            }

            /// <summary>Verifies if <paramref name="value"/> is NOT an instance of the type specified by <paramref name="wrongType"/></summary>
            /// <param name="value">Value to test</param>
            /// <param name="wrongType">Type to validate against</param>
            /// <param name="message">Message to use; If none provided a default message is used</param>
            /// <param name="valueExpression">Expression for the <paramref name="value"/>; Normally provided by the compiler</param>
            public void IsNotInstanceOfType(
                object? value,
                Type wrongType,
                string? message = "",
                [CallerArgumentExpression( nameof( value ) )] string valueExpression = ""
                )
            {
                ArgumentNullException.ThrowIfNull( wrongType );

                bool isInstanceOfType = value != null && wrongType.IsInstanceOfType( value );

                // If it is an instance of the specified type then fail the test
                if(isInstanceOfType)
                {
                    if(string.IsNullOrEmpty( message ))
                    {
                        Verify.Fail( $"'{valueExpression}' is an instance of type '{wrongType}'" );
                    }
                    else
                    {
                        Verify.Fail( message );
                    }
                }
            }

            /// <summary>Verifies if <paramref name="value"/> is NOT an instance of the type specified by <typeparamref name="T"/></summary>
            /// <param name="value">Value to test</param>
            /// <param name="message">Message to use; If none provided a default message is used</param>
            /// <param name="valueExpression">Expression for the <paramref name="value"/>; Normally provided by the compiler</param>
            public void IsNotInstanceOfType<T>(
                object? value,
                string? message = "",
                [CallerArgumentExpression( nameof( value ) )] string valueExpression = ""
                )
            {
                Verify.IsNotInstanceOfType( value, typeof( T ), message, valueExpression );
            }

            /// <summary>Verifies if <paramref name="value"/> is an instance of the type specified by <paramref name="expectedType"/></summary>
            /// <param name="value">Value to test</param>
            /// <param name="expectedType">Type to validate against</param>
            /// <param name="message">Message to use; If none provided a default message is used</param>
            /// <param name="valueExpression">Expression for the <paramref name="value"/>; Normally provided by the compiler</param>
            public void IsInstanceOfType( object? value, Type expectedType, string? message = "", [CallerArgumentExpression( nameof( value ) )] string valueExpression = "" )
            {
                ArgumentNullException.ThrowIfNull( expectedType );

                bool isInstanceOfType = value != null && expectedType.IsInstanceOfType( value );

                // If it is NOT an instance of the specified type then fail the test
                if(!isInstanceOfType)
                {
                    if(string.IsNullOrEmpty( message ))
                    {
                        Verify.Fail( $"'{valueExpression}' is not an instance of type '{expectedType}'" );
                    }
                    else
                    {
                        Verify.Fail( message );
                    }
                }
            }

            /// <summary>Verifies if <paramref name="value"/> is NOT an instance of the type specified by <typeparamref name="T"/></summary>
            /// <param name="value">Value to test</param>
            /// <param name="message">Message to use; If none provided a default message is used</param>
            /// <param name="valueExpression">Expression for the <paramref name="value"/>; Normally provided by the compiler</param>
            public void IsInstanceOfType<T>(
                object? value,
                string? message = "",
                [CallerArgumentExpression( nameof( value ) )] string valueExpression = ""
                )
            {
                Verify.IsInstanceOfType( value, typeof( T ), message, valueExpression );
            }

            /// <summary>Validates two <see cref="GeneratorDriverRunResult"/> are equivalent</summary>
            /// <param name="r1">Results of first run</param>
            /// <param name="r2">Results of second run</param>
            /// <param name="trackingNames">Names of custom tracking steps to validate</param>
            public void AreEqual(
                GeneratorDriverRunResult r1,
                GeneratorDriverRunResult r2,
                ImmutableArray<string> trackingNames
                )
            {
                var trackedSteps1 = r1.GetTrackedSteps(trackingNames);
                var trackedSteps2 = r2.GetTrackedSteps(trackingNames);

                // Assert the static requirements
                Verify.False( trackedSteps1.Count == 0, "Should not be an empty set of steps matching tracked names" );
                Verify.Equal( trackedSteps1.Count, trackedSteps2.Count, "Both runs should have same number of tracked steps" );
                bool hasSameKeys = trackedSteps1.Zip(trackedSteps2, ( s1, s2 ) => trackedSteps2.ContainsKey(s1.Key) && trackedSteps1.ContainsKey(s2.Key))
                                                .All(x => x);

                Verify.True( hasSameKeys, "Both sets of runs should have the same keys" );

                // loop through all KVPs of name to step in result set 1
                // assert that the second run steps for the same tracking name are equal.
                foreach(var (trackingName, runSteps1) in trackedSteps1)
                {
                    var runSteps2 = trackedSteps2[trackingName];
                    Verify.AreEqual( runSteps1, runSteps2, trackingName );
                }
            }

            /// <summary>Verifies each member of a pair of <see cref="ImmutableArray{IncrementalGeneratorRunStep}"/> are equivalent.</summary>
            /// <param name="steps1">Array of steps to test against</param>
            /// <param name="steps2">Array of steps to assert are equal to the elements of <paramref name="steps1"/></param>
            /// <param name="stepTrackingName">Tracking name of the step for use in diagnostic messages</param>
            /// <remarks>
            /// Each item is tested for equality and this only passes if ALL members are equal.
            /// </remarks>
            public void AreEqual(
                ImmutableArray<IncrementalGeneratorRunStep> steps1,
                ImmutableArray<IncrementalGeneratorRunStep> steps2,
                string stepTrackingName
                )
            {
                Verify.HasCount( steps1.Length, steps2, "Step lengths should be equal" );
                for(int i = 0; i < steps1.Length; ++i)
                {
                    var runStep1 = steps1[i];
                    var runStep2 = steps2[i];

                    IEnumerable<object> outputs1 = runStep1.Outputs.Select(x => x.Value);
                    IEnumerable<object> outputs2 = runStep2.Outputs.Select(x => x.Value);
                    Verify.SequenceEqual<object>( outputs1, outputs2, EqualityComparer<object>.Default, $"{stepTrackingName} should produce cacheable outputs" );
                    Verify.OutputsCachedOrUnchanged( runStep2, stepTrackingName );
                    Verify.ObjectGraphContainsValidSymbols( runStep1, stepTrackingName );
                }
            }

            /// <summary>Verifies that all of the tracked output steps are cached</summary>
            /// <param name="driverRunResult">Run results to test for cached outputs</param>
            public void Cached( GeneratorDriverRunResult driverRunResult )
            {
                // verify the second run only generated cached source outputs
                var uncachedSteps = from generatorRunResult in driverRunResult.Results
                                    from trackedStepKvp in generatorRunResult.TrackedOutputSteps
                                    from runStep in trackedStepKvp.Value // name is used in select if condition passes
                                    from valueReasonTuple in runStep.Outputs // all outputs must have a cached reason.
                                    where valueReasonTuple.Reason != IncrementalStepRunReason.Cached
                                    select runStep.Name;

                foreach(string stepTrackingName in uncachedSteps)
                {
                    Verify.Fail( $"Step name {stepTrackingName ?? "<null>"} contains uncached results for second run!" );
                }
            }

            /// <summary>Validates that an object is not of a banned type</summary>
            /// <param name="node">object node to test</param>
            /// <param name="message">reason message for any failures</param>
            /// <param name="parameters">parameters for construction of any exceptions</param>
            public void NotBannedType(
                object? node,
                [StringSyntax( StringSyntaxAttribute.CompositeFormat )] string message,
                params string[] parameters
                )
            {
                // can't validate anything for the type of a null
                if(node is not null)
                {
                    string msg = string.Format(CultureInfo.CurrentCulture, message, parameters);

                    // While this is not a comprehensive list. it covers the most common mistakes directly
                    Verify.IsNotInstanceOfType<Compilation>( node, msg );
                    Verify.IsNotInstanceOfType<ISymbol>( node, msg );
                    Verify.IsNotInstanceOfType<SyntaxNode>( node, msg );
                }
            }

            /// <summary>Validates that all <see cref="IncrementalGeneratorRunStep.Outputs"/> are either <see cref="IncrementalStepRunReason.Cached"/> or <see cref="IncrementalStepRunReason.Unchanged"/></summary>
            /// <param name="runStep">Step of the run to test</param>
            /// <param name="stepTrackingName">Tracking name to use in assertion messages on failures</param>
            public void OutputsCachedOrUnchanged(
                IncrementalGeneratorRunStep runStep,
                string stepTrackingName
                )
            {
                Verify.False(
                    runStep.Outputs.Any( x => x.Reason != IncrementalStepRunReason.Cached && x.Reason != IncrementalStepRunReason.Unchanged ),
                    $"{stepTrackingName} should have only cached or unchanged reasons!"
                    );
            }

            /// <summary>Validates that the output of a <see cref="IncrementalGeneratorRunStep"/> doesn't use any banned types.</summary>
            /// <param name="runStep">Run step to validate</param>
            /// <param name="stepTrackingName">Name of the step to aid in diagnostics</param>
            /// <remarks>
            /// <note title="Ideally in an analyzer">
            /// It is debatable if this should be used in a test or an analyzer for generators. In a test
            /// it is easy to omit from the tests (or not test at all in early development cycles).
            /// An analyzer can operate as you type code in the editor or when you compile the code so
            /// has a greater chance of catching erroneous use. Unfortunately no such analyzer exists
            /// as of yet. [It's actually hard to define the rules an analyzer should follow]. So this
            /// will do the best it can for now...</note>
            /// </remarks>
            public void ObjectGraphContainsValidSymbols(
                IncrementalGeneratorRunStep runStep,
                string stepTrackingName
                )
            {
                // Including the stepTrackingName in error messages to make it easier to isolate issues
                const string because = "Step shouldn't contain banned symbols or non-equatable types. [{0}; {1}]";
                var visited = new HashSet<object>();

                // Check all of the outputs - probably overkill, but why not
                foreach(var (obj, _) in runStep.Outputs)
                {
                    Visit( obj, visited, because, stepTrackingName, runStep.Name ?? "<null>" );
                }

                // Private function to recursively validate an object is cacheable
                void Visit(
                    object? node,
                    HashSet<object> visitedNodes,
                    [StringSyntax( StringSyntaxAttribute.CompositeFormat )] string message,
                    params string[] parameters
                    )
                {
                    // If we've already seen this object, or it's null, stop.
                    if(node is null || !visitedNodes.Add( node ))
                    {
                        return;
                    }

                    Verify.NotBannedType( node, message, parameters );

                    // Skip basic types and anything equatable, this includes
                    // any equatable collections such as EquatableArray<T> as
                    // that implies all elements are equatable already.
                    // For now equatable type skipping is disabled as testing for
                    // that is complex...
                    Type type = node.GetType();
                    if(type.IsBasicType() /*|| type.IsEquatable()*/)
                    {
                        return;
                    }

                    // If the object is a collection, check each of the values
                    if(node is IEnumerable collection and not string && !IsDefaultImmutable( node ))
                    {
                        foreach(object element in collection)
                        {
                            // recursively check each element in the collection
                            Visit( element, visitedNodes, message, parameters );
                        }
                    }
                    else
                    {
                        // Recursively check each field in the object
                        foreach(FieldInfo field in type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance ))
                        {
                            object? fieldValue = field.GetValue(node);
                            Visit( fieldValue, visitedNodes, message, parameters );
                        }
                    }
                }
            }

            /// <summary>Assert that the contents of two <see cref="SourceText"/> instances are valid</summary>
            /// <param name="expected">Expected text</param>
            /// <param name="actual">Actual text to compare</param>
            /// <param name="message">Message to include as a prefix to any differences found (default: empty string)</param>
            /// <remarks>
            /// This will detect differences between <paramref name="expected"/> and <paramref name="actual"/> and
            /// will report the differences found in any assertion triggered.
            /// </remarks>
            public void AreEqual(
                SourceText expected,
                SourceText actual,
                string? message = null
                )
            {
                message ??= string.Empty;

                // if message is not empty make sure it ends in a newline
                if(!string.IsNullOrEmpty( message ) && message.EndsWith( '\n' ))
                {
                    message = $"{message}\n";
                }

                string uniDiff = expected.UniDiff(actual);
                Verify.True( string.IsNullOrWhiteSpace( uniDiff ), $"{message}Differences:\n{uniDiff}" );
            }
        }

        // This prevents visiting an Immutable collection that is default constructed
        // Sadly, that will throw an exception on enumeration instead of just completing.
        // So it is NOT safe to just cast to IEnumerable and party on - it might throw!
        private static bool IsDefaultImmutable( object? o )
        {
            if(o is null)
            {
                return false;
            }

            // This applies to a pattern of types that implement the IsDefault
            // property. The most common is ImmutableArray<T> but there are many
            // others. This will skip all the generic type cruft and try to get
            // the common property - if it isn't there. Then it's not one of the
            // types to care about for this check.
            PropertyInfo? propInfo = o.GetType().GetProperty("IsDefault");
            if(propInfo is null)
            {
                return false;
            }

            object? propVal = propInfo.GetValue(o);
            return propVal != null
                && propVal is bool propBoolVal
                && propBoolVal;
        }
    }
}
