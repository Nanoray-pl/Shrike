using System;

namespace Nanoray.Shrike
{
    public interface IPointerAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> : ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TAnchor : notnull
    {
        TPointerMatcher MoveToPointerAnchor(TAnchor anchor);
    }

    public interface IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> : IPointerAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>, ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TAnchor : notnull
    {
        TPointerMatcher AnchorPointer(TAnchor anchor);
    }

    public static class IPointerAnchorableSequencePointerMatcherExt
    {
        public static TPointerMatcher AnchorPointer<TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> self, out TAnchor anchor, Func<TAnchor> generator)
            where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TAnchor : notnull
        {
            anchor = generator();
            return self.AnchorPointer(anchor);
        }

#if NET7_0_OR_GREATER
        public static TPointerMatcher AnchorPointer<TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> self, out TAnchor anchor)
            where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TAnchor : notnull, IGenerable<TAnchor>
        {
            anchor = TAnchor.Generate();
            return self.AnchorPointer(anchor);
        }
#endif
    }
}
