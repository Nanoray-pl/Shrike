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
        public static SequencePointerMatcher<TElement> MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index)
            => new(allElements, index);

        public static SequenceBlockMatcher<TElement> MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length)
            => new(allElements, startIndex, length);
#else
        public SequencePointerMatcher<TElement> MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index)
            => new(allElements, index);

        public SequenceBlockMatcher<TElement> MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length)
            => new(allElements, startIndex, length);
#endif

        public abstract SequenceBlockMatcher<TElement> Remove();

        public abstract SequenceBlockMatcher<TElement> Replace(IEnumerable<TElement> elements);
    }
}
