using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a simple sequence pointer matcher.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    public record SequencePointerMatcher<TElement> : SequenceMatcher<TElement>, ISequencePointerMatcher<TElement, SequencePointerMatcher<TElement>, SequenceBlockMatcher<TElement>>
    {
        private int IndexStorage { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequencePointerMatcher{TElement}"/> class with the given underlying elements, pointing at an element at the given index.
        /// </summary>
        /// <param name="allElements">All underlying elements this sequence matcher is working with.</param>
        /// <param name="index">The index this pointer matcher should point at.</param>
        public SequencePointerMatcher(IEnumerable<TElement> allElements, int index) : this(allElements.ToList(), index) { }

        private SequencePointerMatcher(IReadOnlyList<TElement> allElements, int index) : base(allElements)
        {
            this.IndexStorage = index;

            if (index < 0 || index >= this.AllElements().Count)
                throw new IndexOutOfRangeException($"Invalid value {index} for parameter `{nameof(index)}`.");
        }

        /// <inheritdoc/>
        public int Index()
            => this.IndexStorage;

        /// <inheritdoc/>
        public SequencePointerMatcher<TElement> Replace(TElement element)
        {
            if (this.Index() >= this.AllElements().Count)
                throw new SequenceMatcherException("No instruction to replace (the pointer is past all instructions).");

            var result = this.AllElements().ToList();
            result[this.Index()] = element;
            return new(result, this.Index());
        }

        /// <inheritdoc/>
        public override SequenceBlockMatcher<TElement> Remove()
        {
            List<TElement> result = new();
            result.AddRange(this.AllElements().Take(this.Index()));
            result.AddRange(this.AllElements().Skip(this.Index() + 1));
            return new(result, this.Index(), 0);
        }

        /// <inheritdoc/>
        public override SequenceBlockMatcher<TElement> Insert(SequenceMatcherPastBoundsDirection position, SequenceMatcherInsertionResultingBounds resultingBounds, IEnumerable<TElement> elements)
        {
            List<TElement> result = new();
            switch (position)
            {
                case SequenceMatcherPastBoundsDirection.Before:
                    {
                        result.AddRange(this.AllElements().Take(this.Index()));
                        result.AddRange(elements);
                        result.AddRange(this.AllElements().Skip(this.Index()));
                        int lengthDifference = result.Count - this.AllElements().Count;
                        return resultingBounds switch
                        {
                            SequenceMatcherInsertionResultingBounds.ExcludingInsertion => new(result, this.Index() + lengthDifference, 1),
                            SequenceMatcherInsertionResultingBounds.JustInsertion => new(result, this.Index(), lengthDifference),
                            SequenceMatcherInsertionResultingBounds.IncludingInsertion => new(result, this.Index(), lengthDifference + 1),
                            _ => throw new ArgumentException($"{nameof(SequenceMatcherInsertionResultingBounds)} has an invalid value.")
                        };
                    }
                case SequenceMatcherPastBoundsDirection.After:
                    {
                        result.AddRange(this.AllElements().Take(this.Index() + 1));
                        result.AddRange(elements);
                        result.AddRange(this.AllElements().Skip(this.Index() + 1));
                        int lengthDifference = result.Count - this.AllElements().Count;
                        return resultingBounds switch
                        {
                            SequenceMatcherInsertionResultingBounds.ExcludingInsertion => new(result, this.Index(), 1),
                            SequenceMatcherInsertionResultingBounds.JustInsertion => new(result, this.Index() + 1, lengthDifference),
                            SequenceMatcherInsertionResultingBounds.IncludingInsertion => new(result, this.Index(), lengthDifference + 1),
                            _ => throw new ArgumentException($"{nameof(SequenceMatcherInsertionResultingBounds)} has an invalid value.")
                        };
                    }
                default:
                    throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value.");
            }
        }

        /// <inheritdoc/>
        public override SequenceBlockMatcher<TElement> Replace(IEnumerable<TElement> elements)
        {
            List<TElement> result = new();
            result.AddRange(this.AllElements().Take(this.Index()));
            result.AddRange(elements);
            result.AddRange(this.AllElements().Skip(this.Index() + 1));
            int lengthDifference = result.Count - this.AllElements().Count;
            return new(result, this.Index(), lengthDifference + 1);
        }

        /// <inheritdoc/>
        public SequencePointerMatcher<TElement> Remove(SequenceMatcherPastBoundsDirection postRemovalPosition)
        {
            if (this.Index() >= this.AllElements().Count)
                throw new SequenceMatcherException("No instruction to remove (the pointer is past all instructions).");
            if (this.AllElements().Count == 0)
                throw new SequenceMatcherException("No instruction to remove.");

            var result = this.AllElements().ToList();
            result.RemoveAt(this.Index());
            return postRemovalPosition switch
            {
                SequenceMatcherPastBoundsDirection.Before => new(result, Math.Max(this.Index() - 1, 0)),
                SequenceMatcherPastBoundsDirection.After => new(result, Math.Min(this.Index(), this.AllElements().Count - 1)),
                _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value."),
            };
        }

        /// <inheritdoc/>
        public override SequencePointerMatcher<TElement> PointerMatcher(SequenceMatcherRelativeElement element)
            => ElementMatcherSubclassDefaultImplementations.PointerMatcher(this, element);

        /// <inheritdoc/>
        public override SequenceBlockMatcher<TElement> BlockMatcher(SequenceMatcherRelativeBounds bounds)
            => ElementMatcherSubclassDefaultImplementations.BlockMatcher(this, bounds);
    }
}
