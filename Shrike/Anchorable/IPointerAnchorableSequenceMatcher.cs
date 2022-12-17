using System;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a sequence matcher handling elements of a given type, with the ability to move to anchored pointers, with specified pointer and block matcher implementations.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
    /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
    /// <typeparam name="TAnchor">The anchor type.</typeparam>
    public interface IPointerAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> : ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TAnchor : notnull
    {
        /// <summary>
        /// Moves to an anchored pointer.
        /// </summary>
        /// <param name="anchor">The anchor to move to.</param>
        /// <returns>A new pointer matcher, pointing at an pointer anchored earlier.</returns>
        TPointerMatcher MoveToPointerAnchor(TAnchor anchor);
    }

    /// <summary>
    /// Represents a sequence pointer matcher handling elements of a given type, with the ability to anchor pointers, with specified pointer and block matcher implementations.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
    /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
    /// <typeparam name="TAnchor">The anchor type.</typeparam>
    public interface IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> : IPointerAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>, ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TAnchor : notnull
    {
        /// <summary>
        /// Anchors a pointer for a later use.
        /// </summary>
        /// <param name="anchor">The anchor to use.</param>
        /// <returns>A new pointer matcher, with an additional anchor pointing at the current element.</returns>
        TPointerMatcher AnchorPointer(TAnchor anchor);
    }

    /// <summary>
    /// A static class hosting additional extensions for the <see cref="IPointerAnchorableSequencePointerMatcher{TElement, TPointerMatcher, TBlockMatcher, TAnchor}"/> type.
    /// </summary>
    public static class IPointerAnchorableSequencePointerMatcherExt
    {
        /// <summary>
        /// Anchors a pointer for a later use.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="anchor">The created anchor.</param>
        /// <param name="generator">A function that will generate the anchor values.</param>
        /// <returns>A new pointer matcher, with an additional anchor pointing at the current element.</returns>
        public static TPointerMatcher AnchorPointer<TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> self, out TAnchor anchor, Func<TAnchor> generator)
            where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TAnchor : notnull
        {
            anchor = generator();
            return self.AnchorPointer(anchor);
        }

#if NET7_0_OR_GREATER
        /// <summary>
        /// Anchors a pointer for a later use.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="anchor">The created anchor.</param>
        /// <returns>A new pointer matcher, with an additional anchor pointing at the current element.</returns>
        public static TPointerMatcher AnchorPointer<TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> self, out TAnchor anchor)
            where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TAnchor : notnull, IGenerable<TAnchor>
        {
            anchor = TAnchor.Generate();
            return self.AnchorPointer(anchor);
        }
#endif
    }

    /// <summary>
    /// A static class hosting additional extensions for the <see cref="IPointerAnchorableSequencePointerMatcher{TElement, TPointerMatcher, TBlockMatcher, TAnchor}"/> type, for pre-specified anchor types.
    /// </summary>
    public static class IPointerAnchorableSequencePointerMatcherSpecificTypeGenerators
    {
        /// <summary>
        /// Anchors a pointer for a later use.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="anchor">The created anchor.</param>
        /// <returns>A new pointer matcher, with an additional anchor pointing at the current element.</returns>
        public static TPointerMatcher AnchorPointer<TElement, TPointerMatcher, TBlockMatcher>(this IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, Guid> self, out Guid anchor)
            where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, Guid>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.AnchorPointer(out anchor, () => Guid.NewGuid());
    }
}
