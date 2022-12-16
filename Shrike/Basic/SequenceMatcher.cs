using System.Collections.Generic;

namespace Nanoray.Shrike
{
    public abstract record SequenceMatcher<TElement> : ISequenceMatcher<TElement, SequencePointerMatcher<TElement>, SequenceBlockMatcher<TElement>>
    {
        private IReadOnlyList<TElement> AllElementsStorage { get; init; }

        protected internal SequenceMatcher(IReadOnlyList<TElement> allElements)
        {
            this.AllElementsStorage = allElements;
        }

        /// <inheritdoc/>
        public IReadOnlyList<TElement> AllElements()
            => this.AllElementsStorage;

#if NET7_0_OR_GREATER
        /// <inheritdoc/>
        public static SequencePointerMatcher<TElement> MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index)
            => new(allElements, index);

        /// <inheritdoc/>
        public static SequenceBlockMatcher<TElement> MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length)
            => new(allElements, startIndex, length);
#else
        /// <inheritdoc/>
        public SequencePointerMatcher<TElement> MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index)
            => new(allElements, index);

        /// <inheritdoc/>
        public SequenceBlockMatcher<TElement> MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length)
            => new(allElements, startIndex, length);
#endif

        /// <inheritdoc/>
        public abstract SequenceBlockMatcher<TElement> Replace(IEnumerable<TElement> elements);

        /// <inheritdoc/>
        public abstract SequenceBlockMatcher<TElement> Remove();

        /// <inheritdoc/>
        public abstract SequenceBlockMatcher<TElement> Insert(SequenceMatcherPastBoundsDirection position, bool includeInsertionInResultingBounds, IEnumerable<TElement> elements);
    }
}
