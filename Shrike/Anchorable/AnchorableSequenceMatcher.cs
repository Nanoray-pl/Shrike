using System;
using System.Collections.Generic;

namespace Nanoray.Shrike
{
    public abstract record AnchorableSequenceMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> :
        IPointerAnchorableSequenceMatcher<TElement, AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, TPointerAnchor>,
        IBlockAnchorableSequenceMatcher<TElement, AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, TBlockAnchor>
        where TPointerAnchor : notnull
        where TBlockAnchor : notnull
        where TWrappedPointerMatcher : ISequencePointerMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
        where TWrappedBlockMatcher : ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
    {
        protected internal ISequenceMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> WrappedMatcher { get; init; }

        protected internal IReadOnlyDictionary<TPointerAnchor, int> AnchoredPointers { get; init; }

        protected internal IReadOnlyDictionary<TBlockAnchor, Range> AnchoredBlocks { get; init; }

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
        public AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> MoveToPointerAnchor(TPointerAnchor anchor)
        {
            if (this.AnchoredPointers.TryGetValue(anchor, out int anchorIndex))
                return this.MakePointerMatcher(anchorIndex);
            else
                throw new SequenceMatcherException($"Unknown pointer anchor {anchor}.");
        }

        /// <inheritdoc/>
        public AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> MoveToBlockAnchor(TBlockAnchor anchor)
        {
            if (this.AnchoredBlocks.TryGetValue(anchor, out var anchorRange))
                return ((ISequenceMatcher<TElement, AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>>)this).MakeBlockMatcher(anchorRange);
            else
                throw new SequenceMatcherException($"Unknown block anchor {anchor}.");
        }

        /// <inheritdoc/>
        public abstract AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Remove();

        /// <inheritdoc/>
        public abstract AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Replace(IEnumerable<TElement> elements);

        /// <inheritdoc/>
        public abstract AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Insert(SequenceMatcherPastBoundsDirection position, bool includeInsertionInResultingBounds, IEnumerable<TElement> elements);
    }
}
