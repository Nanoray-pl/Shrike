using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a simple sequence block matcher.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    public record SequenceBlockMatcher<TElement> : SequenceMatcher<TElement>, ISequenceBlockMatcher<TElement, SequencePointerMatcher<TElement>, SequenceBlockMatcher<TElement>>
    {
        private int StartIndexStorage { get; init; }
        private int LengthStorage { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceBlockMatcher{TElement}"/> class with the given underlying elements, pointing at all of those elements.
        /// </summary>
        /// <param name="allElements">All underlying elements this sequence matcher is working with.</param>
        public SequenceBlockMatcher(IEnumerable<TElement> allElements) : this(allElements.ToList()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceBlockMatcher{TElement}"/> class with the given underlying elements, pointing at all of those elements.
        /// </summary>
        /// <param name="allElements">All underlying elements this sequence matcher is working with.</param>
        public SequenceBlockMatcher(params TElement[] allElements) : this((IEnumerable<TElement>)allElements) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceBlockMatcher{TElement}"/> class with the given underlying elements, pointing at the given range of elements.
        /// </summary>
        /// <param name="allElements">All underlying elements this sequence matcher is working with.</param>
        /// <param name="startIndex">The starting index the block matcher should point at.</param>
        /// <param name="length">The length of the range the block matcher should point at.</param>
        public SequenceBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length) : this(allElements.ToList(), startIndex, length) { }

        private SequenceBlockMatcher(IReadOnlyList<TElement> allElements) : this(allElements, 0, allElements.Count) { }

        private SequenceBlockMatcher(IReadOnlyList<TElement> allElements, int startIndex, int length) : base(allElements)
        {
            this.StartIndexStorage = startIndex;
            this.LengthStorage = length;

            if (startIndex < 0 || startIndex > this.AllElements().Count)
                throw new IndexOutOfRangeException($"Invalid value {startIndex} for parameter `{nameof(startIndex)}`.");
            if (length < 0 || length > this.AllElements().Count)
                throw new ArgumentException($"Invalid value {length} for parameter `{nameof(length)}`.");
            if (startIndex + length > this.AllElements().Count)
                throw new IndexOutOfRangeException($"Invalid value {length} for parameter `{nameof(length)}`.");
        }

        /// <inheritdoc/>
        public int StartIndex()
            => this.StartIndexStorage;

        /// <inheritdoc/>
        public int Length()
            => this.LengthStorage;

        /// <inheritdoc/>
        public int EndIndex()
            => this.StartIndex() + this.Length();

        /// <inheritdoc/>
        public override SequenceBlockMatcher<TElement> Remove()
        {
            var allElements = this.AllElements();
            List<TElement> result = new();
            result.AddRange(allElements.Take(this.StartIndex()));
            result.AddRange(allElements.Skip(this.StartIndex() + this.Length()));
            return new(result, this.StartIndex(), 0);
        }

        /// <inheritdoc/>
        public override SequenceBlockMatcher<TElement> Replace(IEnumerable<TElement> elements)
        {
            var allElements = this.AllElements();
            List<TElement> result = new();
            result.AddRange(allElements.Take(this.StartIndex()));
            result.AddRange(elements);
            result.AddRange(allElements.Skip(this.StartIndex() + this.Length()));
            int lengthDifference = result.Count - allElements.Count;
            return new(result, this.StartIndex(), lengthDifference + this.Length());
        }

        /// <inheritdoc/>
        public override SequenceBlockMatcher<TElement> Insert(SequenceMatcherPastBoundsDirection position, SequenceMatcherInsertionResultingBounds resultingBounds, IEnumerable<TElement> elements)
        {
            var allElements = this.AllElements();
            List<TElement> result = new();
            switch (position)
            {
                case SequenceMatcherPastBoundsDirection.Before:
                    {
                        result.AddRange(allElements.Take(this.StartIndex()));
                        result.AddRange(elements);
                        result.AddRange(allElements.Skip(this.StartIndex()));
                        int lengthDifference = result.Count - allElements.Count;
                        return resultingBounds switch
                        {
                            SequenceMatcherInsertionResultingBounds.ExcludingInsertion => new(result, this.StartIndex() + lengthDifference, this.Length()),
                            SequenceMatcherInsertionResultingBounds.JustInsertion => new(result, this.StartIndex(), lengthDifference),
                            SequenceMatcherInsertionResultingBounds.IncludingInsertion => new(result, this.StartIndex(), this.Length() + lengthDifference),
                            _ => throw new ArgumentException($"{nameof(SequenceMatcherInsertionResultingBounds)} has an invalid value.")
                        };
                    }
                case SequenceMatcherPastBoundsDirection.After:
                    {
                        result.AddRange(allElements.Take(this.EndIndex()));
                        result.AddRange(elements);
                        result.AddRange(allElements.Skip(this.EndIndex()));
                        int lengthDifference = result.Count - allElements.Count;
                        return resultingBounds switch
                        {
                            SequenceMatcherInsertionResultingBounds.ExcludingInsertion => new(result, this.StartIndex(), this.Length()),
                            SequenceMatcherInsertionResultingBounds.JustInsertion => new(result, this.EndIndex(), lengthDifference),
                            SequenceMatcherInsertionResultingBounds.IncludingInsertion => new(result, this.StartIndex(), this.Length() + lengthDifference),
                            _ => throw new ArgumentException($"{nameof(SequenceMatcherInsertionResultingBounds)} has an invalid value.")
                        };
                    }
                default:
                    throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value.");
            }
        }

        /// <inheritdoc/>
        public override SequencePointerMatcher<TElement> PointerMatcher(SequenceMatcherRelativeElement element)
            => ElementMatcherSubclassDefaultImplementations.PointerMatcher(this, element);

        /// <inheritdoc/>
        public override SequenceBlockMatcher<TElement> BlockMatcher(SequenceMatcherRelativeBounds bounds)
            => ElementMatcherSubclassDefaultImplementations.BlockMatcher(this, bounds);

        /// <inheritdoc/>
        public SequenceBlockMatcher<TElement> Do(Func<SequenceBlockMatcher<TElement>, SequenceBlockMatcher<TElement>> closure)
        {
#if NET7_0_OR_GREATER
            var innerMatcher = MakeNewBlockMatcher(this.Elements(), 0, this.Length());
#else
            var innerMatcher = this.MakeNewBlockMatcher(this.Elements(), 0, this.Length());
#endif
            var modifiedMatcher = closure(innerMatcher);
            return this.Replace(modifiedMatcher.AllElements());
        }
    }
}
