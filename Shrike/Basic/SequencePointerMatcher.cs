using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike
{
    public record SequencePointerMatcher<TElement> : SequenceMatcher<TElement>, ISequencePointerMatcher<TElement, SequencePointerMatcher<TElement>, SequenceBlockMatcher<TElement>>
    {
        /// <inheritdoc/>
        public int Index { get; init; }

        public SequencePointerMatcher(IEnumerable<TElement> allElements, int index) : this(allElements.ToList(), index) { }

        private SequencePointerMatcher(IReadOnlyList<TElement> allElements, int index) : base(allElements)
        {
            this.Index = index;

            if (index < 0 || index >= this.AllElements.Count)
                throw new IndexOutOfRangeException($"Invalid value {index} for parameter `{nameof(index)}`.");
        }

        public SequencePointerMatcher<TElement> Replace(TElement element)
        {
            if (this.Index >= this.AllElements.Count)
                throw new SequenceMatcherException("No instruction to replace (the pointer is past all instructions).");

            var result = this.AllElements.ToList();
            result[this.Index] = element;
            return new(result, this.Index);
        }

        public override SequenceBlockMatcher<TElement> Remove()
        {
            List<TElement> result = new();
            result.AddRange(this.AllElements.Take(this.Index));
            result.AddRange(this.AllElements.Skip(this.Index + 1));
            return new(result, this.Index, 0);
        }

        public override SequenceBlockMatcher<TElement> Replace(IEnumerable<TElement> elements)
        {
            List<TElement> result = new();
            result.AddRange(this.AllElements.Take(this.Index));
            result.AddRange(elements);
            result.AddRange(this.AllElements.Skip(this.Index + 1));
            int lengthDifference = result.Count - this.AllElements.Count;
            return new(result, this.Index, lengthDifference + 1);
        }

        public SequenceBlockMatcher<TElement> Insert(IEnumerable<TElement> elements)
        {
            List<TElement> result = new();
            result.AddRange(this.AllElements.Take(this.Index));
            result.AddRange(elements);
            result.AddRange(this.AllElements.Skip(this.Index));
            int lengthDifference = result.Count - this.AllElements.Count;
            return new(result, this.Index, lengthDifference);
        }

        public SequencePointerMatcher<TElement> Remove(SequenceMatcherPastBoundsDirection postRemovalPosition)
        {
            if (this.Index >= this.AllElements.Count)
                throw new SequenceMatcherException("No instruction to remove (the pointer is past all instructions).");
            if (this.AllElements.Count == 0)
                throw new SequenceMatcherException("No instruction to remove.");

            var result = this.AllElements.ToList();
            result.RemoveAt(this.Index);
            return postRemovalPosition switch
            {
                SequenceMatcherPastBoundsDirection.Before => new(result, Math.Max(this.Index - 1, 0)),
                SequenceMatcherPastBoundsDirection.After => new(result, Math.Min(this.Index, this.AllElements.Count - 1)),
                _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value."),
            };
        }
    }
}