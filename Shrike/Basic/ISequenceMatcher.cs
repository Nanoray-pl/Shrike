using System;
using System.Collections.Generic;

namespace Nanoray.Shrike
{
    public interface ISequenceMatcher<TElement>
    {
        IReadOnlyList<TElement> AllElements { get; }
    }

    public interface ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> : ISequenceMatcher<TElement>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
    {
        TBlockMatcher AllElementsBlockMatcher
            => this.MakeBlockMatcher(0, this.AllElements.Count);

        TBlockMatcher Replace(IEnumerable<TElement> elements);

        TBlockMatcher Remove();

        TPointerMatcher MakePointerMatcher(int index)
#if NET7_0_OR_GREATER
            => TPointerMatcher.MakeNewPointerMatcher(this.AllElements, index);
#else
            => this.MakeNewPointerMatcher(this.AllElements, index);
#endif

        TBlockMatcher MakeBlockMatcher(int startIndex, int length)
#if NET7_0_OR_GREATER
            => TBlockMatcher.MakeNewBlockMatcher(this.AllElements, startIndex, length);
#else
            => this.MakeNewBlockMatcher(this.AllElements, startIndex, length);
#endif

        TBlockMatcher MakeBlockMatcher(Range range)
            => this.MakeBlockMatcher(range.Start.Value, range.End.Value - range.Start.Value);

#if NET7_0_OR_GREATER
        static abstract TPointerMatcher MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index);

        static abstract TBlockMatcher MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length);

        static TBlockMatcher MakeNewBlockMatcher(IEnumerable<TElement> allElements, Range range)
            => TBlockMatcher.MakeNewBlockMatcher(allElements, range.Start.Value, range.End.Value - range.Start.Value);
#else
        TPointerMatcher MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index);

        TBlockMatcher MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length);

        TBlockMatcher MakeNewBlockMatcher(IEnumerable<TElement> allElements, Range range)
            => this.MakeNewBlockMatcher(allElements, range.Start.Value, range.End.Value - range.Start.Value);
#endif
    }

    public static class ISequenceMatcherExt
    {
        public static TBlockMatcher Replace<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, params TElement[] elements)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Replace(elements);
    }
}
