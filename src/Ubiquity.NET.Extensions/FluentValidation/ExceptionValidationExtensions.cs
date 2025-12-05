// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions.FluentValidation
{
    // TODO: [API BREAKING CHANGE] move namespace to the "extension" pattern type name
    //       so that use of the full type name can disambiguate with any existing
    //       method of the same name exists (or an extension). [i.e., An extension type
    //       with a static method that has the same arity and type of args but different
    //       return type is ambiguous.]
    /*
    internal abstract class SampleBase
    {
        public SampleBase(string foo)
        {
            Foo = foo;
        }

        private readonly string Foo;
    }

    internal class Sample
        : SampleBase
    {
        public Sample(string foo)
            : base( FluentValidation.ThrowIfNull( foo ) )
        {
        }
    }
    */

    // This does NOT use the new C# 14 extension syntax due to several reasons
    // 1) Code lens does not work https://github.com/dotnet/roslyn/issues/79006 [Sadly, marked as "not planned" - e.g., dead-end]
    // 2) MANY analyzers get things wrong and need to be suppressed (CA1000, CA1034, and many others [SAxxxx])
    // 3) Many external tools don't support the new syntax yet and it isn't clear if they will in the future.
    // 4) No clear support for Caller* attributes ([CallerArgumentExpression(...)]).
    //
    // Bottom line it's a good idea with an incomplete implementation lacking support
    // in the overall ecosystem. Don't use it unless you absolutely have to until all
    // of that is sorted out.

    /// <summary>Extension class to provide Fluent validation of arguments</summary>
    /// <remarks>
    /// These are similar to many of the built-in support checks except that
    /// they use a `Fluent' style to allow validation of parameters used as inputs
    /// to other functions that ultimately produce parameters for a base constructor.
    /// They also serve to provide validation when using body expressions for property
    /// method implementations etc... Though the C# 14 <c>field</c> keyword makes that
    /// use mostly a moot point.
    /// <note type="important">
    /// In .NET Standard 2.0 builds this can create ambiguities with the static extensions
    /// in `PolyFillExceptionValidators`. This is because they are "Poly Filled"
    /// in downstream versions and the resolution rules for extensions in the C# language.
    /// Instance methods are resolved before the static extensions and therefore the extensions
    /// here are resolved even if there is a direct static extensions. This seems broken, but
    /// is how the language is resolving things. Therefore careful use of namespace usings
    /// and global usings as well as explicit use of this type is needed to resolve this. It
    /// is NOT recommended to use explicit references to the static method in `PolyFillExceptionValidators`
    /// as the methods don't exist if the BCL type contains the method already in a given
    /// runtime. Thus, in compilation units, needing both namespaces only this one is
    /// explicitly referenced.
    /// </note>
    /// </remarks>
    public static class ExceptionValidationExtensions
    {
        /// <summary>Throws an exception if <paramref name="self"/> is <see langword="null"/></summary>
        /// <typeparam name="T">Type of reference parameter to test for</typeparam>
        /// <param name="self">Instance to test</param>
        /// <param name="exp">Name or expression of the value in <paramref name="self"/> [Default: provided by compiler]</param>
        /// <returns><paramref name="self"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="self"/> is <see langword="null"/></exception>
        [DebuggerStepThrough]
        public static T ThrowIfNull<T>( [NotNull] this T? self, [CallerArgumentExpression( nameof( self ) )] string? exp = null )
        {
            return self is null
                 ? throw new ArgumentNullException( exp )
                 : self;
        }

        /// <summary>Throws an exception if an argument is outside of a given (Inclusive) range</summary>
        /// <typeparam name="T">Type of value to test</typeparam>
        /// <param name="self">Value to test</param>
        /// <param name="min">Minimum value allowed for <paramref name="self"/></param>
        /// <param name="max">Maximum value allowed for <paramref name="self"/></param>
        /// <param name="exp">Name or expression of the value in <paramref name="self"/> [Default: provided by compiler]</param>
        /// <returns><paramref name="self"/></returns>
        [DebuggerStepThrough]
        [SuppressMessage( "Style", "IDE0046:Convert to conditional expression", Justification = "Not simpler, more readable this way" )]
        public static T ThrowIfOutOfRange<T>( this T self, T min, T max, [CallerArgumentExpression( nameof( self ) )] string? exp = null )
            where T : struct, IComparable<T>
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfLessThan(self, min, exp);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(self, max, exp);
#else
            PolyFillExceptionValidators.ThrowIfLessThan( self, min, exp );
            PolyFillExceptionValidators.ThrowIfGreaterThan( self, max, exp );
#endif
            return self;
        }

        /// <summary>Tests if an enum is defined or not</summary>
        /// <typeparam name="T">Type of value to test</typeparam>
        /// <param name="self">Value to test</param>
        /// <param name="exp">Name or expression of the value in <paramref name="self"/> [Default: provided by compiler]</param>
        /// <returns><paramref name="self"/></returns>
        /// <exception cref="InvalidEnumArgumentException">The enumerated value is not defined</exception>
        /// <remarks>
        /// This is useful to prevent callers from playing tricks with casts, etc... to land with a value
        /// that is otherwise undefined. Note: This is mostly useless on an enumeration marked with
        /// <see cref="FlagsAttribute"/> as a legit value that is a combination of flags does not have
        /// a defined value (Only single bit values do)
        /// </remarks>
        [DebuggerStepThrough]
        public static T ThrowIfNotDefined<T>( this T self, [CallerArgumentExpression( nameof( self ) )] string? exp = null )
            where T : struct, Enum
        {
            exp ??= string.Empty;
            try
            {
#if NET5_0_OR_GREATER
                if(Enum.IsDefined( self ))
                {
                    return self;
                }
#else
                if(Enum.IsDefined( typeof( T ), self ))
                {
                    return self;
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
                    SR.Format(CultureInfo.CurrentCulture, nameof( Resources.InvalidEnumArgument_NonInt ), exp, integral, typeof( T ) )
                    );
            }
        }
    }
}
