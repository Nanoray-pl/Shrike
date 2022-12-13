using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike
{
    public record AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> :
        AnchorableSequenceMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>,
        IPointerAnchorableSequencePointerMatcher<TElement, AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, TPointerAnchor>
        where TPointerAnchor : notnull
        where TBlockAnchor : notnull
        where TWrappedPointerMatcher : ISequencePointerMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
        where TWrappedBlockMatcher : ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
    {
        protected internal ISequencePointerMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> WrappedPointerMatcher { get; init; }

        /// <inheritdoc/>
        public int Index
            => this.WrappedPointerMatcher.Index;

        public AnchorableSequencePointerMatcher(ISequencePointerMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> wrappedMatcher) : this(wrappedMatcher, new Dictionary<TPointerAnchor, int>(), new Dictionary<TBlockAnchor, Range>()) { }

        protected internal AnchorableSequencePointerMatcher(ISequencePointerMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> wrappedMatcher, IReadOnlyDictionary<TPointerAnchor, int> anchoredPointers, IReadOnlyDictionary<TBlockAnchor, Range> anchoredBlocks) : base(wrappedMatcher, anchoredPointers, anchoredBlocks)
        {
            this.WrappedPointerMatcher = wrappedMatcher;
        }

        private IReadOnlyDictionary<TPointerAnchor, int> GetModifiedAnchoredPointers(IReadOnlyList<TElement> newElements)
        {
            int lengthDifference = newElements.Count - this.AllElements.Count;
            return this.AnchoredPointers
                .Where(kvp => kvp.Value != this.Index)
                .Select(kvp => new KeyValuePair<TPointerAnchor, int>(kvp.Key, kvp.Value < this.Index ? kvp.Value : kvp.Value + lengthDifference))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private IReadOnlyDictionary<TBlockAnchor, Range> GetModifiedAnchoredBlocks(IReadOnlyList<TElement> newElements)
        {
            int lengthDifference = newElements.Count - this.AllElements.Count;
            return this.AnchoredBlocks
                .Where(kvp => !(this.Index >= kvp.Value.Start.Value && this.Index < kvp.Value.End.Value))
                .Select(kvp => new KeyValuePair<TBlockAnchor, Range>(kvp.Key, this.Index < kvp.Value.End.Value ? kvp.Value : new Range(new(kvp.Value.Start.Value + lengthDifference), new(kvp.Value.End.Value + lengthDifference))))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> AnchorPointer(TPointerAnchor anchor)
        {
            Dictionary<TPointerAnchor, int> anchoredPointers = new(this.AnchoredPointers) { [anchor] = this.Index };
            return new(this.WrappedPointerMatcher, anchoredPointers, this.AnchoredBlocks);
        }

        public override AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Remove()
        {
            var wrapped = this.WrappedPointerMatcher.Remove();
            return new(wrapped, this.GetModifiedAnchoredPointers(wrapped.AllElements), this.GetModifiedAnchoredBlocks(wrapped.AllElements));
        }

        public override AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Replace(IEnumerable<TElement> elements)
        {
            var wrapped = this.WrappedPointerMatcher.Replace(elements);
            return new(wrapped, this.GetModifiedAnchoredPointers(wrapped.AllElements), this.GetModifiedAnchoredBlocks(wrapped.AllElements));
        }

        public AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Replace(TElement element)
        {
            var wrapped = this.WrappedPointerMatcher.Replace(element);
            return new(wrapped, this.GetModifiedAnchoredPointers(wrapped.AllElements), this.GetModifiedAnchoredBlocks(wrapped.AllElements));
        }

        public AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Insert(IEnumerable<TElement> elements)
        {
            var wrapped = this.WrappedPointerMatcher.Insert(elements);
            return new(wrapped, this.GetModifiedAnchoredPointers(wrapped.AllElements), this.GetModifiedAnchoredBlocks(wrapped.AllElements));
        }

        public AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Remove(SequenceMatcherPastBoundsDirection postRemovalPosition)
        {
            var wrapped = this.WrappedPointerMatcher.Remove(postRemovalPosition);
            return new(wrapped, this.GetModifiedAnchoredPointers(wrapped.AllElements), this.GetModifiedAnchoredBlocks(wrapped.AllElements));
        }
    }
}
