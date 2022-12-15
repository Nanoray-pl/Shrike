using System;
using System.Collections.Generic;

namespace Nanoray.Shrike
{
    public interface ISequenceMatcher<TElement>
    {
        IReadOnlyList<TElement> AllElements();
    }

    public interface ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> : ISequenceMatcher<TElement>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
    {
        TBlockMatcher MakeAllElementsBlockMatcher()
            => this.MakeBlockMatcher(0, this.AllElements().Count);

        TBlockMatcher Replace(IEnumerable<TElement> elements);

        TBlockMatcher Remove();

        TPointerMatcher MakePointerMatcher(int index)
#if NET7_0_OR_GREATER
            => TPointerMatcher.MakeNewPointerMatcher(this.AllElements(), index);
#else
            => this.MakeNewPointerMatcher(this.AllElements(), index);
#endif

        TBlockMatcher MakeBlockMatcher(int startIndex, int length)
#if NET7_0_OR_GREATER
            => TBlockMatcher.MakeNewBlockMatcher(this.AllElements(), startIndex, length);
#else
            => this.MakeNewBlockMatcher(this.AllElements(), startIndex, length);
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

    public static class ISequenceMatcherDefaultImplementations
    {
        public static TBlockMatcher MakeAllElementsBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.MakeAllElementsBlockMatcher();

        public static TPointerMatcher MakePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int index)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.MakePointerMatcher(index);

        public static TBlockMatcher MakeBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int startIndex, int length)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.MakeBlockMatcher(startIndex, length);

        public static TBlockMatcher MakeBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, Range range)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.MakeBlockMatcher(range);

#if !NET7_0_OR_GREATER
        public static TBlockMatcher MakeNewBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, IEnumerable<TElement> allElements, Range range)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.MakeNewBlockMatcher(allElements, range);
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
