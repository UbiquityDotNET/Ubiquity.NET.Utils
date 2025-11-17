# About
This library contains extensions that are shared amongst multiple additional projects in
this repository. This, currently takes the place of a source generator that would inject
these types. The problem with a Roslyn source generator for this is that the "generated"
sources have a dependency on types that are poly filled by a different source generator.
([PolySharp](https://github.com/Sergio0694/PolySharp) for this repo).
Source generators all see the same input and therefore a source generator is untestable
without solving the problem of explicitly generating the sources for the poly filled types.
That is, to test the generator one must include another generator as part of the setup for
testing. That's something currently unknown and instead of investigating it the problem was
set aside to move other things forward.
