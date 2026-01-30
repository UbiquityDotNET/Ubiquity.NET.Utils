// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Implementation of <see cref="ISyntaxNode"/> for a <see cref="RuleContext"/></summary>
    public class RuleContextNode
        : SyntaxNodeBase
    {
        /// <summary>Initializes a new instance of the <see cref="RuleContextNode"/> class</summary>
        /// <param name="rule">Rule context this node wraps</param>
        public RuleContextNode( RuleContext rule )
            : base( rule.GetSourceLocation() )
        {
            Rule = rule;
        }

        /// <summary>Gets the <see cref="RuleContext"/> this node wraps</summary>
        public RuleContext Rule { get; }
    }
}
