using System;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a sequence matcher handling elements of a given type, with the ability to move to anchored blocks, with specified pointer and block matcher implementations.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
    /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
    /// <typeparam name="TAnchor">The anchor type.</typeparam>
    public interface IBlockAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> : ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TAnchor : notnull
    {
        /// <summary>
        /// Moves to an anchored block.
        /// </summary>
        /// <param name="anchor">The anchor to move to.</param>
        /// <returns>A new block matcher, pointing at a block anchored earlier.</returns>
        TBlockMatcher MoveToBlockAnchor(TAnchor anchor);
    }

    /// <summary>
    /// Represents a sequence block matcher handling elements of a given type, with the ability to anchor blocks, with specified pointer and block matcher implementations.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
    /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
    /// <typeparam name="TAnchor">The anchor type.</typeparam>
    public interface IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> : IBlockAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>, ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TAnchor : notnull
    {
        /// <summary>
        /// Anchors a block for a later use.
        /// </summary>
        /// <param name="anchor">The anchor to use.</param>
        /// <returns>A new block matcher, with an additional anchor pointing at the current elements.</returns>
        TBlockMatcher AnchorBlock(TAnchor anchor);
    }

    /// <summary>
    /// A static class hosting additional extensions for the <see cref="IBlockAnchorableSequenceBlockMatcher{TElement, TPointerMatcher, TBlockMatcher, TAnchor}"/> type.
    /// </summary>
    public static class IBlockAnchorableSequenceBlockMatcherExt
    {
        /// <summary>
        /// Anchors a block for a later use.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="anchor">The created anchor.</param>
        /// <param name="generator">A function that will generate the anchor values.</param>
        /// <returns>A new block matcher, with an additional anchor pointing at the current elements.</returns>
        public static TBlockMatcher AnchorBlock<TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> self, out TAnchor anchor, Func<TAnchor> generator)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TAnchor : notnull
        {
            anchor = generator();
            return self.AnchorBlock(anchor);
        }

#if NET7_0_OR_GREATER
        /// <summary>
        /// Anchors a block for a later use.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="anchor">The created anchor.</param>
        /// <returns>A new block matcher, with an additional anchor pointing at the current elements.</returns>
        public static TBlockMatcher AnchorBlock<TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> self, out TAnchor anchor)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TAnchor : notnull, IGenerable<TAnchor>
        {
            anchor = TAnchor.Generate();
            return self.AnchorBlock(anchor);
        }
#endif
    }

    /// <summary>
    /// A static class hosting additional extensions for the <see cref="IBlockAnchorableSequenceBlockMatcher{TElement, TPointerMatcher, TBlockMatcher, TAnchor}"/> type, for pre-specified anchor types.
    /// </summary>
    public static class IBlockAnchorableSequencePointerMatcherSpecificTypeGenerators
    {
        /// <summary>
        /// Anchors a block for a later use.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="anchor">The created anchor.</param>
        /// <returns>A new block matcher, with an additional anchor pointing at the current elements.</returns>
        public static TBlockMatcher AnchorBlock<TElement, TPointerMatcher, TBlockMatcher>(this IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, Guid> self, out Guid anchor)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, Guid>
            => self.AnchorBlock(out anchor, () => Guid.NewGuid());
    }
}
