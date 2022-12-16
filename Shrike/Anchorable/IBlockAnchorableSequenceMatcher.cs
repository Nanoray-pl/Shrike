using System;

namespace Nanoray.Shrike
{
    public interface IBlockAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> : ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TAnchor : notnull
    {
        TBlockMatcher MoveToBlockAnchor(TAnchor anchor);
    }

    public interface IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> : IBlockAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>, ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TAnchor : notnull
    {
        TBlockMatcher AnchorBlock(TAnchor anchor);
    }

    public static class IBlockAnchorableSequenceBlockMatcherExt
    {
        public static TBlockMatcher AnchorBlock<TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> self, out TAnchor anchor, Func<TAnchor> generator)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TAnchor : notnull
        {
            anchor = generator();
            return self.AnchorBlock(anchor);
        }

#if NET7_0_OR_GREATER
        public static TBlockMatcher AnchorBlock<TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor> self, out TAnchor anchor)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TBlockMatcher : IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TAnchor : notnull, IGenerable<TAnchor>
        {
            anchor = TAnchor.Generate();
            return self.AnchorBlock(anchor);
        }
#endif
    }

    public static class IBlockAnchorableSequencePointerMatcherSpecificTypeGenerators
    {
        public static TBlockMatcher AnchorBlock<TElement, TPointerMatcher, TBlockMatcher>(this IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, Guid> self, out Guid anchor)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : IBlockAnchorableSequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher, Guid>
            => self.AnchorBlock(out anchor, () => Guid.NewGuid());
    }
}
