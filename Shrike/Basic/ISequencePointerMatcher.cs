using System.Collections.Generic;

namespace Nanoray.Shrike
{
    public interface ISequencePointerMatcher<TElement> : ISequenceMatcher<TElement>
    {
        int Index { get; }

        TElement Element
            => this.AllElements[this.Index];
    }

    public interface ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher> : ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher>, ISequencePointerMatcher<TElement>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
    {
        TBlockMatcher BlockMatcher
            => this.MakeBlockMatcher(this.Index, 1);

        TPointerMatcher Advance(int offset = 1)
            => this.MakePointerMatcher(this.Index + offset);

        TPointerMatcher Replace(TElement element);

        TBlockMatcher Insert(IEnumerable<TElement> elements);

        TPointerMatcher Remove(SequenceMatcherPastBoundsDirection postRemovalPosition);
    }

    public static class ISequencePointerMatcherExt
    {
        public static TBlockMatcher Replace<TElement, TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, params TElement[] elements)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Replace(elements);

        public static TBlockMatcher Insert<TElement, TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, params TElement[] elements)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Insert(elements);
    }
}
