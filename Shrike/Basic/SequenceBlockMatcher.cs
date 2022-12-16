using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike
{
    public record SequenceBlockMatcher<TElement> : SequenceMatcher<TElement>, ISequenceBlockMatcher<TElement, SequencePointerMatcher<TElement>, SequenceBlockMatcher<TElement>>
    {
        private int StartIndexStorage { get; init; }
        private int LengthStorage { get; init; }

        public SequenceBlockMatcher(IEnumerable<TElement> allElements) : this(allElements.ToList()) { }

        public SequenceBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length) : this(allElements.ToList(), startIndex, length) { }

        public SequenceBlockMatcher(params TElement[] allElements) : this((IEnumerable<TElement>)allElements) { }

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
            List<TElement> result = new();
            result.AddRange(this.AllElements().Take(this.StartIndex()));
            result.AddRange(this.AllElements().Skip(this.StartIndex() + this.Length()));
            return new(result, this.StartIndex(), 0);
        }

        /// <inheritdoc/>
        public override SequenceBlockMatcher<TElement> Replace(IEnumerable<TElement> elements)
        {
            List<TElement> result = new();
            result.AddRange(this.AllElements().Take(this.StartIndex()));
            result.AddRange(elements);
            result.AddRange(this.AllElements().Skip(this.StartIndex() + this.Length()));
            int lengthDifference = result.Count - this.AllElements().Count;
            return new(result, this.StartIndex(), lengthDifference + this.Length());
        }

        /// <inheritdoc/>
        public override SequenceBlockMatcher<TElement> Insert(SequenceMatcherPastBoundsDirection position, bool includeInsertionInResultingBounds, IEnumerable<TElement> elements)
        {
            List<TElement> result = new();
            switch (position)
            {
                case SequenceMatcherPastBoundsDirection.Before:
                    {
                        result.AddRange(this.AllElements().Take(this.StartIndex()));
                        result.AddRange(elements);
                        result.AddRange(this.AllElements().Skip(this.StartIndex()));
                        int lengthDifference = result.Count - this.AllElements().Count;
                        return includeInsertionInResultingBounds switch
                        {
                            false => new(result, this.StartIndex() + lengthDifference, this.Length()),
                            true => new(result, this.StartIndex(), this.Length() + lengthDifference),
                        };
                    }
                case SequenceMatcherPastBoundsDirection.After:
                    {
                        result.AddRange(this.AllElements().Take(this.EndIndex()));
                        result.AddRange(elements);
                        result.AddRange(this.AllElements().Skip(this.EndIndex()));
                        int lengthDifference = result.Count - this.AllElements().Count;
                        return includeInsertionInResultingBounds switch
                        {
                            false => new(result, this.StartIndex(), this.Length()),
                            true => new(result, this.StartIndex(), this.Length() + lengthDifference),
                        };
                    }
                default:
                    throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value.");
            }
        }

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
