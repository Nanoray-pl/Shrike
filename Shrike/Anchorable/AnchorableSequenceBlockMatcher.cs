using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike
{
    public record AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> :
        AnchorableSequenceMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>,
        IBlockAnchorableSequenceBlockMatcher<TElement, AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, TBlockAnchor>
        where TPointerAnchor : notnull
        where TBlockAnchor : notnull
        where TWrappedPointerMatcher : ISequencePointerMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
        where TWrappedBlockMatcher : ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
    {
        protected internal ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> WrappedBlockMatcher { get; init; }

        public AnchorableSequenceBlockMatcher(ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> wrappedMatcher) : this(wrappedMatcher, new Dictionary<TPointerAnchor, int>(), new Dictionary<TBlockAnchor, Range>()) { }

        protected internal AnchorableSequenceBlockMatcher(ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> wrappedMatcher, IReadOnlyDictionary<TPointerAnchor, int> anchoredPointers, IReadOnlyDictionary<TBlockAnchor, Range> anchoredBlocks) : base(wrappedMatcher, anchoredPointers, anchoredBlocks)
        {
            this.WrappedBlockMatcher = wrappedMatcher;
        }

        /// <inheritdoc/>
        public int StartIndex()
            => this.WrappedBlockMatcher.StartIndex();

        /// <inheritdoc/>
        public int Length()
            => this.WrappedBlockMatcher.Length();

        /// <inheritdoc/>
        public int EndIndex()
            => this.WrappedBlockMatcher.EndIndex();

        private IReadOnlyDictionary<TPointerAnchor, int> GetModifiedAnchoredPointers(IReadOnlyList<TElement> newElements)
        {
            int lengthDifference = newElements.Count - this.AllElements().Count;
            return this.AnchoredPointers
                .Where(kvp => !(kvp.Value >= this.StartIndex() && kvp.Value < this.EndIndex()))
                .Select(kvp => new KeyValuePair<TPointerAnchor, int>(kvp.Key, kvp.Value <= this.StartIndex() ? kvp.Value : kvp.Value + lengthDifference))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private IReadOnlyDictionary<TBlockAnchor, Range> GetModifiedAnchoredBlocks(IReadOnlyList<TElement> newElements)
        {
            int lengthDifference = newElements.Count - this.AllElements().Count;
            return this.AnchoredBlocks
                .Where(kvp => !(kvp.Value.Start.Value < this.EndIndex() && this.StartIndex() < kvp.Value.End.Value))
                .Select(kvp => new KeyValuePair<TBlockAnchor, Range>(kvp.Key, kvp.Value.End.Value <= this.StartIndex() ? kvp.Value : new Range(new(kvp.Value.Start.Value + lengthDifference), new(kvp.Value.End.Value + lengthDifference))))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> AnchorBlock(TBlockAnchor anchor)
        {
            Dictionary<TBlockAnchor, Range> anchoredBlocks = new(this.AnchoredBlocks) { [anchor] = new(new(this.StartIndex()), new(this.EndIndex())) };
            return new(this.WrappedBlockMatcher, this.AnchoredPointers, anchoredBlocks);
        }

        public override AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Remove()
        {
            var wrapped = this.WrappedBlockMatcher.Remove();
            return new(wrapped, this.GetModifiedAnchoredPointers(wrapped.AllElements()), this.GetModifiedAnchoredBlocks(wrapped.AllElements()));
        }

        public override AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Replace(IEnumerable<TElement> elements)
        {
            var wrapped = this.WrappedBlockMatcher.Replace(elements);
            return new(wrapped, this.GetModifiedAnchoredPointers(wrapped.AllElements()), this.GetModifiedAnchoredBlocks(wrapped.AllElements()));
        }

        public AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Insert(SequenceMatcherPastBoundsDirection position, bool includeInsertionInResultingBounds, IEnumerable<TElement> elements)
        {
            var wrapped = this.WrappedBlockMatcher.Insert(position, includeInsertionInResultingBounds, elements);
            return new(wrapped, this.GetModifiedAnchoredPointers(wrapped.AllElements()), this.GetModifiedAnchoredBlocks(wrapped.AllElements()));
        }

        public AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Do(Func<AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>> closure)
        {
#if NET7_0_OR_GREATER
            var innerMatcher = MakeNewBlockMatcher(this.Elements(), 0, this.Length());
#else
            var innerMatcher = this.MakeNewBlockMatcher(this.Elements(), 0, this.Length());
#endif
            var modifiedMatcher = closure(innerMatcher);
            var current = this.Replace(modifiedMatcher.AllElements());

            Dictionary<TPointerAnchor, int> anchoredPointers = new(this.AnchoredPointers);
            foreach (var anchor in modifiedMatcher.AnchoredPointers)
                anchoredPointers[anchor.Key] = this.StartIndex() + anchor.Value;

            Dictionary<TBlockAnchor, Range> anchoredBlocks = new(this.AnchoredBlocks);
            foreach (var anchor in modifiedMatcher.AnchoredBlocks)
                anchoredBlocks[anchor.Key] = new(new(this.StartIndex() + anchor.Value.Start.Value), new(this.StartIndex() + anchor.Value.End.Value));

            return new(current.WrappedBlockMatcher, anchoredPointers, anchoredBlocks);
        }
    }

    public static class AnchorableSequenceBlockMatcherExt
    {
        public static AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> AsAnchorable<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>(this ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> self)
            where TPointerAnchor : notnull
            where TBlockAnchor : notnull
            where TWrappedPointerMatcher : ISequencePointerMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
            where TWrappedBlockMatcher : ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
            => new(self);
    }
}
