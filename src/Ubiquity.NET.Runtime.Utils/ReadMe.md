# Ubiquity.NET.Runtime.Utils
This library contains support functionality to aid in building a Domain Specific Language
(DSL) runtime, including common implementation of a Read-Evaluate-Print Loop (REPL) used for
interactive languages. Generally, this is used in conjunction with the `Ubiquity.NET.Llvm`
library to provide custom DSL JIT support though it is useful on it's own for language
servers etc...

See the [`Kaleidoscope`](https://ubiquitydotnet.github.io/Llvm.NET/llvm/articles/Samples/Kaleidoscope/Kaleidoscope-Overview.html)
tutorial in the LLVM repository for an example use of this library.
