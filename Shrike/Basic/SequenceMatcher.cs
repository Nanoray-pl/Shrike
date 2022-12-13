using System.Collections.Generic;

namespace Nanoray.Shrike
{
    public abstract record SequenceMatcher<TElement> : ISequenceMatcher<TElement, SequencePointerMatcher<TElement>, SequenceBlockMatcher<TElement>>
    {
        /// <inheritdoc/>
        public IReadOnlyList<TElement> AllElements { get; init; }

        protected internal SequenceMatcher(IReadOnlyList<TElement> allElements)
        {
            this.AllElements = allElements;
        }

#if NET7_0_OR_GREATER
        public static override SequencePointerMatcher<TElement> MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index)
            => new(this.AllElements, index);

        public static override SequenceBlockMatcher<TElement> MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length)
            => new(this.AllElements, startIndex, length);
#else
        public SequencePointerMatcher<TElement> MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index)
            => new(this.AllElements, index);

        public SequenceBlockMatcher<TElement> MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length)
            => new(this.AllElements, startIndex, length);
#endif

        public abstract SequenceBlockMatcher<TElement> Remove();

        public abstract SequenceBlockMatcher<TElement> Replace(IEnumerable<TElement> elements);
    }
}
