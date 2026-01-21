// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.SrcGen
{
    internal static class CommonAttributeData
    {
        public static Optional<bool> IsRequired( this EquatableAttributeData self )
        {
            return self.GetNamedArgValue<bool>( Constants.OptionAttributeNamedArgs.Required );
        }

        public static Optional<(int Min, int Max)> GetArity( this EquatableAttributeData self )
        {
            // ignore argument if both aren't available
            Optional<int> min = self.GetNamedArgValue<int>( Constants.CommonAttributeNamedArgs.ArityMin );
            Optional<int> max = self.GetNamedArgValue<int>( Constants.CommonAttributeNamedArgs.ArityMax );

            return min.HasValue && max.HasValue
                 ? new( (min.Value, max.Value) )
                 : default;
        }
    }
}
