// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Extension class for parser rules</summary>
    public static class RuleContextExtensions
    {
        /// <summary>Gets the <see cref="SourceLocation"/> from a <see cref="RuleContext"/></summary>
        /// <param name="ctx">Context to get the location from</param>
        /// <returns>Source Location of the context</returns>
        /// <remarks>
        /// The rule must be a <see cref="ParserRuleContext"/> or a default location is returned
        /// as the base <see cref="RuleContext"/> doesn't provide access to location information
        /// </remarks>
        public static SourceLocation GetSourceLocation( this RuleContext ctx )
        {
            // RuleContext doesn't track line information. Only the character index from beginning of the input...
            // ParserRuleContext does track enough to form a line based location.
            return ctx is ParserRuleContext parserRuleCtx
                ? ParserRuleContextExtensions.GetSourceLocation( parserRuleCtx )
                : default;
        }

        /// <summary>Gets an enumerable from the children of a <see cref="RuleContext"/></summary>
        /// <param name="ctx">Context to get children from</param>
        /// <returns>A non-null, but possibly empty enumeration of children</returns>
        public static IEnumerable<IParseTree> GetChildren( this RuleContext ctx )
        {
            for(int i = 0; i < ctx.ChildCount; ++i)
            {
                yield return ctx.GetChild( i );
            }
        }
    }
}
