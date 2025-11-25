# About
Ubiquity.NET.Extensions contains general extensions for .NET. This is a bit of a "grab bag"
of functionality used by but not actually part of multiple other Ubiquity.NET projects. A
core principal is that this library has NO dependencies beyond the runtime itself. That is,
this library should remain at the bottom of any dependency chain.

## Key support
* Computing a hash code for a ReadOnlySpan of bytes using
  [System.IO.System.IO.Hashing.XxHash3](https://learn.microsoft.com/en-us/dotnet/api/system.io.hashing.xxhash3)
* DisposableAction for building actions that must occur on Dispose
    - This is useful for implementing the RAII pattern in .NET.
* MustUseReturnValueAttribute that is compatible with the [MustUseRetVal](https://github.com/mykolav/must-use-ret-val-fs)
  package.
* StringNormalizer extensions to support converting line endings of strings
  for interoperability across OS platforms and compatibility with "on disk" representations.
* A custom ValidatedNotNullAttribute to allow compiler to assume a parameter
  value is validated as not null.
* DictionaryBuilder to enable dictionary initializer style initialization of
  `ImmutableDictionary<TKey, TValue>` with significantly reduced overhead.
    - This leverages an `ImmutableDictionary<TKey, TValue>.Builder` under the hood to build
      the dictionary. When the `ToImmutable()` method is called the builder is converted to
      the immutable state without any overhead of a copy or re-construction of hash tables
      etc...
* KvpArrayBuilder to enable initializer style initialization of
  `ImmutableArray<KeyValuePair<TKey, TValue>>` with significantly reduced overhead.
    - This leverages an `ImmutableArray<T>.Builder` under the hood to build the array
      directly. When the `ToImmutable()` method is called the builder is ***converted*** to
      the immutable state without any overhead of a copy.
    - Since this is an array and not a dictionary there is no overhead for allocating,
      initializing or copying any hash mapping for the keys.

## Fluent Validation
The library includes extensions that support fluent validation to allow use in property
accessors and constructors that forward values to a base or another constructor. This is
normally used when the value itself isn't passed on but some transformed value is.

|Method | Description |
|-------|-------------|
|`ThrowIfNull()`| Throws an exception if the argument is null or returns it as-is |
|`ThrowIfOutOfRange()` | Throws an exception if a value is out of the specified range |
|`ThrowIfNotDefined()` | Throws an exception if an enum value is undefined |

## Runtime Dependencies
System.Collections.Immutable for NET Standard 2.0 builds.

There are dependencies on various compile time analyzers but no runtime dependencies are
allowed that aren't poly filled.
