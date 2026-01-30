// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions
{
    /// <summary>poly fill extensions for static methods added in .NET 7</summary>
    /// <remarks>
    /// This makes the intent a little clearer via the naming AND makes usage consistent
    /// across multiple runtime versions.
    /// </remarks>
    public static class Requires
    {
        /// <summary>Throw an <see cref="ArgumentException"/> if a string is <see langword="null"/>m empty, or all whitespace.</summary>
        /// <param name="argument">input string to test</param>
        /// <param name="paramName">expression or name of the string to test; normally provided by compiler</param>
        /// <exception cref="ArgumentException">string is <see langword="null"/>m empty, or all whitespace</exception>
        public static void NotNullOrWhiteSpace(
            [NotNull] string? argument,
            [CallerArgumentExpressionAttribute( nameof( argument ) )] string? paramName = null
            )
        {
#if NETSTANDARD2_0
            PolyFillExceptionValidators.ThrowIfNullOrWhiteSpace( argument, paramName );
#else
            ArgumentException.ThrowIfNullOrWhiteSpace( argument, paramName );
#endif
        }

        /// <summary>Throws an exception if the tested argument is <see langword="null"/></summary>
        /// <param name="argument">value to test</param>
        /// <param name="paramName">expression for the name of the value; normally provided by compiler</param>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is <see langword="null"/></exception>
        public static void NotNull(
            [NotNull] object? argument,
            [CallerArgumentExpressionAttribute( nameof( argument ) )] string? paramName = default
            )
        {
#if NETSTANDARD2_0
            PolyFillExceptionValidators.ThrowIfNull( argument, paramName );
#else
            ArgumentNullException.ThrowIfNull(argument, paramName);
#endif
        }

        /// <summary>Throws an <see cref="ObjectDisposedException"/> if <paramref name="isDisposed"/> is <see langword="true"/>.</summary>
        /// <param name="isDisposed">Condition to determine if the instance is disposed</param>
        /// <param name="instance">instance that is tested; Used to get type name for exception</param>
        /// <exception cref="ObjectDisposedException"><paramref name="isDisposed"/> is <see langword="true"/></exception>
        /// <remarks>
        /// This will throw an <see cref="ObjectDisposedException"/> if <paramref name="isDisposed"/> is true.
        /// That is, this assumes that the expression for <paramref name="isDisposed"/> is semantically some form of
        /// "IsDisposed" check. This is the same as the semantics for <c>ObjectDisposedException.ThrowIf</c> if the
        /// runtime supports that. The result is semantically the same. An exception is thrown if the expression indicates
        /// that <paramref name="instance"/> is disposed.
        /// </remarks>
        public static void NotDisposed(
            [DoesNotReturnIfAttribute( true )] bool isDisposed,
            object instance
            )
        {
#if NETSTANDARD2_0
            PolyFillExceptionValidators.ThrowIf(isDisposed, instance);
#else
            ObjectDisposedException.ThrowIf(isDisposed, instance);
#endif
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is equal to <paramref name="other"/>.</summary>
        /// <typeparam name="T">Type of values to compare</typeparam>
        /// <param name="value">The argument to validate as not equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void Equal<T>( T value, T other, [CallerArgumentExpressionAttribute( nameof( value ) )] string? paramName = null )
            where T : IEquatable<T>?
        {
#if NETSTANDARD2_0
            PolyFillExceptionValidators.ThrowIfNotEqual(value, other, paramName);
#else
            ArgumentOutOfRangeException.ThrowIfNotEqual(value, other, paramName);
#endif
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is not equal to <paramref name="other"/>.</summary>
        /// <typeparam name="T">Type of values to compare</typeparam>
        /// <param name="value">The argument to validate as equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void NotEqual<T>( T value, T other, [CallerArgumentExpressionAttribute( nameof( value ) )] string? paramName = null )
            where T : global::System.IEquatable<T>?
        {
#if NETSTANDARD2_0
            PolyFillExceptionValidators.ThrowIfEqual(value, other, paramName);
#else
            ArgumentOutOfRangeException.ThrowIfEqual( value, other, paramName );
#endif
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.</summary>
        /// <typeparam name="T">Type of values to compare</typeparam>
        /// <param name="value">The argument to validate as less or equal than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void LessThanOrEqualTo<T>( T value, T other, [CallerArgumentExpressionAttribute( nameof( value ) )] string? paramName = null )
            where T : global::System.IComparable<T>
        {
#if NETSTANDARD2_0
            PolyFillExceptionValidators.ThrowIfGreaterThan(value, other, paramName);
#else
            ArgumentOutOfRangeException.ThrowIfGreaterThan( value, other, paramName );
#endif
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal <paramref name="other"/>.</summary>
        /// <typeparam name="T">Type of values to compare</typeparam>
        /// <param name="value">The argument to validate as less than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void LessThan<T>( T value, T other, [CallerArgumentExpressionAttribute( nameof( value ) )] string? paramName = null )
            where T : global::System.IComparable<T>
        {
#if NETSTANDARD2_0
            PolyFillExceptionValidators.ThrowIfGreaterThanOrEqual(value, other, paramName);
#else
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual( value, other, paramName );
#endif
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.</summary>
        /// <typeparam name="T">Type of values to compare</typeparam>
        /// <param name="value">The argument to validate as greater than or equal than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void GreaterThanOrEqualTo<T>( T value, T other, [CallerArgumentExpressionAttribute( nameof( value ) )] string? paramName = null )
            where T : global::System.IComparable<T>
        {
#if NETSTANDARD2_0
            PolyFillExceptionValidators.ThrowIfLessThan(value, other, paramName);
#else
            ArgumentOutOfRangeException.ThrowIfLessThan( value, other, paramName );
#endif
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal <paramref name="other"/>.</summary>
        /// <typeparam name="T">Type of values to compare</typeparam>
        /// <param name="value">The argument to validate as greater than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void GreaterThan<T>( T value, T other, [CallerArgumentExpressionAttribute( nameof( value ) )] string? paramName = null )
            where T : global::System.IComparable<T>
        {
#if NETSTANDARD2_0
            PolyFillExceptionValidators.ThrowIfLessThanOrEqual(value, other, paramName);
#else
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual( value, other, paramName );
#endif
        }

        /// <summary>Tests if an enum is defined or not</summary>
        /// <typeparam name="T">Type of value to test</typeparam>
        /// <param name="self">Value to test</param>
        /// <param name="exp">Name or expression of the value in <paramref name="self"/> [Default: provided by compiler]</param>
        /// <exception cref="InvalidEnumArgumentException">The enumerated value is not defined</exception>
        /// <remarks>
        /// This is useful to prevent callers from playing tricks with casts, etc... to land with a value
        /// that is otherwise undefined. Note: This is mostly useless on an enumeration marked with
        /// <see cref="FlagsAttribute"/> as a legit value that is a combination of flags does not have
        /// a defined value (Only single bit values do)
        /// </remarks>
        [DebuggerStepThrough]
        public static void Defined<T>( this T self, [CallerArgumentExpression( nameof( self ) )] string? exp = null )
            where T : struct, Enum
        {
            exp ??= string.Empty;
            try
            {
#if NET5_0_OR_GREATER
                if(Enum.IsDefined( self ))
                {
                    return;
                }
#else
                if(Enum.IsDefined( typeof( T ), self ))
                {
                    return;
                }
#endif
                int underlyingValue = (int)Convert.ChangeType(self, typeof(int), CultureInfo.InvariantCulture);
                throw new InvalidEnumArgumentException( exp, underlyingValue, typeof( T ) );
            }
            catch(Exception ex) when(ex is InvalidCastException or FormatException or OverflowException)
            {
                // bit cast the enum to an nuint as that is a platform specific maximal value
#if NET8_0_OR_GREATER
                nuint integral = Unsafe.BitCast<T, nuint>(self);
#else
                ref byte refSelf = ref Unsafe.As<T, byte>(ref self);
                nuint integral = Unsafe.ReadUnaligned<nuint>(ref refSelf);
#endif

                // InvalidEnumArgumentException constructors ONLY provide parameter name value set for values
                // that are representable as an int. Thus, anything else requires a custom message that at
                // least includes the original value in question. (Normally an enum does fit an int, but for
                // interop might not) the resulting exception will have "ParamName" as the default of "null"!
                //
                // This matches the overloaded constructor version but allows for reporting enums with non-int underlying type.
                throw new InvalidEnumArgumentException(
                    SR.Format( CultureInfo.CurrentCulture, nameof( Resources.InvalidEnumArgument_NonInt ), exp, integral, typeof( T ) )
                    );
            }
        }
    }
}
