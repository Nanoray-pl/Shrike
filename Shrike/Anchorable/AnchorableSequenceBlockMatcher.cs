using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents an anchorable sequence block matcher.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TPointerAnchor">The pointer anchor type.</typeparam>
    /// <typeparam name="TBlockAnchor">The block anchor type.</typeparam>
    /// <typeparam name="TWrappedPointerMatcher">The underlying pointer matcher type.</typeparam>
    /// <typeparam name="TWrappedBlockMatcher">The underlying block matcher type.</typeparam>
    public record AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> :
        AnchorableSequenceMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>,
        IBlockAnchorableSequenceBlockMatcher<TElement, AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>, TBlockAnchor>
        where TPointerAnchor : notnull
        where TBlockAnchor : notnull
        where TWrappedPointerMatcher : ISequencePointerMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
        where TWrappedBlockMatcher : ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
    {
        /// <summary>
        /// The underlying block matcher.
        /// </summary>
        protected internal ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> WrappedBlockMatcher { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchorableSequenceBlockMatcher{TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher}"/> class.
        /// </summary>
        /// <param name="wrappedMatcher"></param>
        public AnchorableSequenceBlockMatcher(ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> wrappedMatcher) : this(wrappedMatcher, new Dictionary<TPointerAnchor, int>(), new Dictionary<TBlockAnchor, Range>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchorableSequenceBlockMatcher{TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher}"/> class.
        /// </summary>
        /// <param name="wrappedMatcher">The underlying matcher.</param>
        /// <param name="anchoredPointers">The dictionary of all anchored pointers.</param>
        /// <param name="anchoredBlocks">The dictionary of all anchored blocks.</param>
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

        /// <inheritdoc/>
        public AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Find(SequenceBlockMatcherFindOccurence occurence, SequenceMatcherRelativeBounds bounds, IReadOnlyList<IElementMatch<TElement>> toFind)
        {
            Dictionary<TPointerAnchor, int> anchoredPointers = new(this.AnchoredPointers);
            var findResult = this.WrappedBlockMatcher.Find(occurence, bounds, toFind);
            for (int i = 0; i < toFind.Count; i++)
            {
                if (toFind[i] is not IAutoAnchorableElementMatch<TElement, TPointerAnchor> autoAnchorableMatch || autoAnchorableMatch.Anchor is null)
                    continue;
                anchoredPointers[autoAnchorableMatch.Anchor] = findResult.StartIndex() + i;
            }
            return new(findResult, anchoredPointers, this.AnchoredBlocks);
        }

        /// <inheritdoc/>
        public AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> AnchorBlock(TBlockAnchor anchor)
        {
            Dictionary<TBlockAnchor, Range> anchoredBlocks = new(this.AnchoredBlocks) { [anchor] = new(new(this.StartIndex()), new(this.EndIndex())) };
            return new(this.WrappedBlockMatcher, this.AnchoredPointers, anchoredBlocks);
        }

        /// <inheritdoc/>
        public override AnchorableSequencePointerMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> PointerMatcher(SequenceMatcherRelativeElement element)
            => ElementMatcherSubclassDefaultImplementations.PointerMatcher(this, element);

        /// <inheritdoc/>
        public override AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> BlockMatcher(SequenceMatcherRelativeBounds bounds)
            => ElementMatcherSubclassDefaultImplementations.BlockMatcher(this, bounds);

        /// <inheritdoc/>
        public override AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Remove()
        {
            var wrapped = this.WrappedBlockMatcher.Remove();
            return new(wrapped, this.GetModifiedAnchoredPointers(wrapped.AllElements()), this.GetModifiedAnchoredBlocks(wrapped.AllElements()));
        }

        /// <inheritdoc/>
        public override AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Replace(IEnumerable<TElement> elements)
        {
            var wrapped = this.WrappedBlockMatcher.Replace(elements);
            return new(wrapped, this.GetModifiedAnchoredPointers(wrapped.AllElements()), this.GetModifiedAnchoredBlocks(wrapped.AllElements()));
        }

        /// <inheritdoc/>
        public override AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> Insert(SequenceMatcherPastBoundsDirection position, bool includeInsertionInResultingBounds, IEnumerable<TElement> elements)
        {
            var wrapped = this.WrappedBlockMatcher.Insert(position, includeInsertionInResultingBounds, elements);
            return new(wrapped, this.GetModifiedAnchoredPointers(wrapped.AllElements()), this.GetModifiedAnchoredBlocks(wrapped.AllElements()));
        }

        /// <inheritdoc/>
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

    /// <summary>
    /// A static class hosting additional extensions for the <see cref="ISequenceBlockMatcher{TElement, TPointerMatcher, TBlockMatcher}"/> type, relating to the functionality of the <see cref="AnchorableSequenceBlockMatcher{TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher}"/> type.
    /// </summary>
    public static class AnchorableSequenceBlockMatcherExt
    {
        /// <summary>
        /// Creates an anchorable block matcher representing the same state as this matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerAnchor">The pointer anchor type.</typeparam>
        /// <typeparam name="TBlockAnchor">The block anchor type.</typeparam>
        /// <typeparam name="TWrappedPointerMatcher">The underlying pointer matcher type.</typeparam>
        /// <typeparam name="TWrappedBlockMatcher">The underlying block matcher type.</typeparam>
        public static AnchorableSequenceBlockMatcher<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher> AsAnchorable<TElement, TPointerAnchor, TBlockAnchor, TWrappedPointerMatcher, TWrappedBlockMatcher>(this ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher> self)
            where TPointerAnchor : notnull
            where TBlockAnchor : notnull
            where TWrappedPointerMatcher : ISequencePointerMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
            where TWrappedBlockMatcher : ISequenceBlockMatcher<TElement, TWrappedPointerMatcher, TWrappedBlockMatcher>
            => new(self);
    }
}
