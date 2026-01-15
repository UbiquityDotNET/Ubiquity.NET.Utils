// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.SrcGen.UT
{
    /// <summary>Provides an implementation of <see cref="IVerifier"/> for Ms Test <see cref="Assert"/>.</summary>
    /// <remarks>
    /// This verifier is dependent on the MsTest framework. Each verification method uses <see cref="Assert"/>
    /// in an adapter pattern.
    /// </remarks>
    public class MsTestVerifier
        : IVerifier
    {
        /// <summary>Initializes a new instance of the <see cref="MsTestVerifier"/> class.</summary>
        public MsTestVerifier( )
            : this( [] )
        {
        }

        /// <inheritdoc/>
        public virtual void Empty<T>( string collectionName, IEnumerable<T> collection )
        {
            Assert.IsEmpty( collection, CreateMessage( $"collection '{collectionName}' is not empty" ) );
        }

        /// <inheritdoc/>
        public virtual void NotEmpty<T>( string collectionName, IEnumerable<T> collection )
        {
            Assert.IsNotEmpty( collection, CreateMessage( $"collection '{collectionName}' is empty" ) );
        }

        /// <inheritdoc/>
        public virtual void LanguageIsSupported( string language )
        {
            Assert.IsTrue( language == LanguageNames.CSharp || language == LanguageNames.VisualBasic, CreateMessage( $"Unsupported Language: '{language}'" ) );
        }

        /// <inheritdoc/>
        public virtual void Equal<T>( T expected, T actual, string? message = null )
        {
            Assert.AreEqual( expected, actual, CreateMessage( message ) );
        }

        /// <inheritdoc/>
        public virtual void True( [DoesNotReturnIf( false )] bool assert, string? message = null )
        {
            Assert.IsTrue( assert, CreateMessage( message ) );
        }

        /// <inheritdoc/>
        public virtual void False( [DoesNotReturnIf( true )] bool assert, string? message = null )
        {
            Assert.IsFalse( assert, CreateMessage( message ) );
        }

        /// <inheritdoc/>
        [DoesNotReturn]
        public virtual void Fail( string? message = null )
        {
            Assert.Fail( CreateMessage( message ) );
        }

        /// <inheritdoc/>
        public virtual void SequenceEqual<T>(
            IEnumerable<T> expected,
            IEnumerable<T> actual,
            IEqualityComparer<T>? equalityComparer = null,
            string? message = null
            )
        {
            var comparer = new SequenceComparer<T>(equalityComparer);
            bool areEqual = comparer.Equals(expected, actual);
            if(!areEqual)
            {
                throw new InvalidOperationException( CreateMessage( message ?? $"Sequences are not equal" ) );
            }
        }

        /// <inheritdoc/>
        public virtual IVerifier PushContext( string context )
        {
            return new MsTestVerifier( Context.Push( context ) );
        }

        /// <summary>Initializes a new instance of the <see cref="MsTestVerifier"/> class with the specified context.</summary>
        /// <param name="context">The verification context, with the innermost verification context label at the top of the stack.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="context"/> is <see langword="null"/>.</exception>
        private MsTestVerifier( ImmutableStack<string> context )
        {
            Context = context ?? throw new ArgumentNullException( nameof( context ) );
        }

        /// <summary>Gets the current verification context. The innermost verification context label is the top item on the stack.</summary>
        private ImmutableStack<string> Context { get; }

        /// <summary>
        /// Creates a full message for a verifier failure combining the current verification <see cref="Context"/> with
        /// the <paramref name="message"/> for the current verification.
        /// </summary>
        /// <param name="message">The failure message to report.</param>
        /// <returns>A full failure message containing both the verification context and the failure message for the current test.</returns>
        private string CreateMessage( string? message )
        {
            message ??= string.Empty;
            foreach(string frame in Context)
            {
                message = "Context: " + frame + Environment.NewLine + message;
            }

            return message;
        }
    }
}
