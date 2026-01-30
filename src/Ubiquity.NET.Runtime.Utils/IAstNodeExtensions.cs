// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Extension class for <see cref="IAstNode"/></summary>
    /// <remarks>
    /// While C# has a syntax mechanism for providing default implementations of an interface, using them
    /// is sub-optimal. This will provide extensions for the interface AND any type that implements it without
    /// all the casting or runtime requirements etc... needed for the default interface methods.
    /// </remarks>
    public static class IAstNodeExtensions
    {
        /// <summary>Extension method to detect if an <see cref="IAstNode"/> has any errors associated with it</summary>
        /// <param name="self">Node to test</param>
        /// <returns><see langword="true"/> if the node has any errors</returns>
        public static bool IsValid( this IAstNode self )
        {
            return !self.Diagnostics.IsEmpty;
        }

        /// <summary>Determines if an <see cref="IAstNode"/> contains the specified index based position</summary>
        /// <param name="self">Node to test</param>
        /// <param name="position">Index based position to test for</param>
        /// <returns><see langword="true"/> if the node contains <paramref name="position"/></returns>
        public static bool Contains( this IAstNode self, int position )
        {
            return self.Location.Range.Contains( position );
        }

        /// <summary>Enumerate items in a node (deepest nodes first)</summary>
        /// <param name="root">Root node to enumerate</param>
        /// <returns>Enumerable of the nodes in a flat ordering</returns>
        /// <remarks>
        /// Neither breadth first nor Depth first traversal is what is desired here to maintain structural ordering.
        /// Effectively what is needed is a reverse Breadth-first ordering. This assumes that children
        /// are sorted according to document order [Normal condition]. This allows matching/searching, for the
        /// lowest level node based on a position. (Obviously the highest level node
        /// "contains" all valid positions for the document! So finding the first match using the depth is simple)
        /// </remarks>
        public static IEnumerable<IAstNode> Flatten( this IAstNode root )
        {
            var evalStack = new Stack<(IAstNode Root, IEnumerator<IAstNode> Iterator)>();
            evalStack.Push( (root, root.Children.GetEnumerator()) );
            while(evalStack.Count > 0)
            {
                // pop node from top of stack
                (IAstNode rootNode, IEnumerator<IAstNode> it) = evalStack.Pop();
                if(!it.MoveNext())
                {
                    // no more children or a leaf node, so yield the node itself
                    yield return rootNode;
                }
                else
                {
                    // push the current state of the node + iterator
                    evalStack.Push( (root, it) );

                    // get the child and a new iterator for it's children and push that state
                    IAstNode nextRoot = it.Current;
                    evalStack.Push( (nextRoot, nextRoot.Children.GetEnumerator()) );
                }
            }
        }
    }
}
