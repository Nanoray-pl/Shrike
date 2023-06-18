using System;
using System.Collections.Generic;

namespace Nanoray.Shrike
{
    /// <summary>
    /// The base type representing anchorable sequence matchers.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TPointerAnchor">The pointer anchor type.</typeparam>
    /// <typeparam name="TBlockAnchor">The block anchor type.</typeparam>
    /// <typeparam name="TWrappedPointerMatcher">The underlying pointer matcher type.</typeparam>
    /// <typeparam name="TWrappedBlockMatcher">The underlying block matcher type.</typeparam>
    public abstract record AnchorableSequenceMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>
        where TPointerAnchor : notnull
        where TBlockAnchor : notnull
        where TWrappedPointerMatcher : ISequencePointerMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
        where TWrappedBlockMatcher : ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
    {
        /// <summary>
        /// The underlying matcher.
        /// </summary>
        protected internal ISequenceMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> WrappedMatcher { get; init; }

        /// <summary>
        /// The dictionary of all anchored pointers.
        /// </summary>
        protected internal IReadOnlyDictionary<TPointerAnchor, int> AnchoredPointers { get; init; }

        /// <summary>
        /// The dictionary of all anchored blocks.
        /// </summary>
        protected internal IReadOnlyDictionary<TBlockAnchor, Range> AnchoredBlocks { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchorableSequenceMatcher{TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher}"/> class.
        /// </summary>
        /// <param name="wrappedMatcher">The underlying matcher.</param>
        /// <param name="anchoredPointers">The dictionary of all anchored pointers.</param>
        /// <param name="anchoredBlocks">The dictionary of all anchored blocks.</param>
        protected internal AnchorableSequenceMatcher(ISequenceMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> wrappedMatcher, IReadOnlyDictionary<TPointerAnchor, int> anchoredPointers, IReadOnlyDictionary<TBlockAnchor, Range> anchoredBlocks)
        {
            this.WrappedMatcher = wrappedMatcher;
            this.AnchoredPointers = anchoredPointers;
            this.AnchoredBlocks = anchoredBlocks;
        }

        /// <inheritdoc/>
        public IReadOnlyList<TElement> AllElements()
            => this.WrappedMatcher.AllElements();

#if NET7_0_OR_GREATER
        /// <inheritdoc/>
        public static AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index)
            => new(TWrappedPointerMatcher.MakeNewPointerMatcher(allElements, index));

        /// <inheritdoc/>
        public static AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length)
            => new(TWrappedBlockMatcher.MakeNewBlockMatcher(allElements, startIndex, length));
#else
        /// <inheritdoc/>
        public AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index)
            => new(this.WrappedMatcher.MakeNewPointerMatcher(allElements, index));

        /// <inheritdoc/>
        public AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length)
            => new(this.WrappedMatcher.MakeNewBlockMatcher(allElements, startIndex, length));
#endif

        /// <inheritdoc/>
        public AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> MakePointerMatcher(int index)
            => new(this.WrappedMatcher.MakePointerMatcher(index), this.AnchoredPointers, this.AnchoredBlocks);

        /// <inheritdoc/>
        public AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> MakeBlockMatcher(int startIndex, int length)
            => new(this.WrappedMatcher.MakeBlockMatcher(startIndex, length), this.AnchoredPointers, this.AnchoredBlocks);

        /// <inheritdoc/>
        public AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> PointerMatcher(TPointerAnchor anchor)
        {
            if (this.AnchoredPointers.TryGetValue(anchor, out int anchorIndex))
                return this.MakePointerMatcher(anchorIndex);
            else
                throw new SequenceMatcherException($"Unknown pointer anchor {anchor}.");
        }

        /// <inheritdoc/>
        public AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> BlockMatcher(TBlockAnchor anchor)
        {
            if (this.AnchoredBlocks.TryGetValue(anchor, out var anchorRange))
                return ((ISequenceMatcher<TElement, AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>>)this).MakeBlockMatcher(anchorRange);
            else
                throw new SequenceMatcherException($"Unknown block anchor {anchor}.");
        }

        /// <inheritdoc/>
        public abstract AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> PointerMatcher(SequenceMatcherRelativeElement element);

        /// <inheritdoc/>
        public abstract AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> BlockMatcher(SequenceMatcherRelativeBounds bounds);

        /// <inheritdoc/>
        public abstract AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Remove();

        /// <inheritdoc/>
        public abstract AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Replace(IEnumerable<TElement> elements);

        /// <inheritdoc/>
        public abstract AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Insert(SequenceMatcherPastBoundsDirection position, SequenceMatcherInsertionResultingBounds resultingBounds, IEnumerable<TElement> elements);
    }
}
