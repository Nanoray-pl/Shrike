using System.Collections.Generic;

namespace Nanoray.Shrike
{
    /// <summary>
    /// The base type representing simple sequence matchers.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    public abstract record SequenceMatcher<TElement> : ISequenceMatcher<TElement, SequencePointerMatcher<TElement>, SequenceBlockMatcher<TElement>>
    {
        private IReadOnlyList<TElement> AllElementsStorage { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceMatcher{TElement}"/> class with the given underlying elements.
        /// </summary>
        /// <param name="allElements">All underlying elements this sequence matcher is working with.</param>
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
        public abstract SequencePointerMatcher<TElement> PointerMatcher(SequenceMatcherRelativeElement element);

        /// <inheritdoc/>
        public abstract SequenceBlockMatcher<TElement> BlockMatcher(SequenceMatcherRelativeBounds bounds);

        /// <inheritdoc/>
        public abstract SequenceBlockMatcher<TElement> Replace(IEnumerable<TElement> elements);

        /// <inheritdoc/>
        public abstract SequenceBlockMatcher<TElement> Remove();

        /// <inheritdoc/>
        public abstract SequenceBlockMatcher<TElement> Insert(SequenceMatcherPastBoundsDirection position, SequenceMatcherInsertionResultingBounds resultingBounds, IEnumerable<TElement> elements);
    }
}
