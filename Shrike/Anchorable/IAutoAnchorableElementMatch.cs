using System.Collections.Generic;

namespace Nanoray.Shrike
{
    public interface IAutoAnchorableElementMatch<TElement, TAnchor> : IElementMatch<TElement>
    {
        TAnchor? Anchor { get; }
    }

    public static class IAutoAnchorableElementMatchExt
    {
        public static TBlockMatcher FindAndAnchor<TSelf, TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this TSelf self, IReadOnlyList<IAutoAnchorableElementMatch<TElement, TAnchor>> toFind)
            where TSelf : IPointerAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>, ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TAnchor : notnull
            => FindAndAnchor<TSelf, TElement, TPointerMatcher, TBlockMatcher, TAnchor>(
                self,
                SequenceBlockMatcherFindOccurence.First,
                self.StartIndex() == 0 && self.Length() == self.AllElements().Count ? SequenceBlockMatcherFindBounds.Enclosed : SequenceBlockMatcherFindBounds.After,
                toFind
            );

        public static TBlockMatcher FindAndAnchor<TSelf, TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this TSelf self, params IAutoAnchorableElementMatch<TElement, TAnchor>[] toFind)
            where TSelf : IPointerAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>, ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TAnchor : notnull
            => FindAndAnchor<TSelf, TElement, TPointerMatcher, TBlockMatcher, TAnchor>(self, (IReadOnlyList<IAutoAnchorableElementMatch<TElement, TAnchor>>)toFind);

        public static TBlockMatcher FindAndAnchor<TSelf, TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this TSelf self, SequenceBlockMatcherFindOccurence occurence, SequenceBlockMatcherFindBounds bounds, IReadOnlyList<IAutoAnchorableElementMatch<TElement, TAnchor>> toFind)
            where TSelf : IPointerAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>, ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TAnchor : notnull
        {
            var current = self.Find(occurence, bounds, toFind);
            int startIndex = current.StartIndex();
            int length = current.Length();
            for (int i = 0; i < toFind.Count; i++)
            {
                if (toFind[i].Anchor is null)
                    continue;
                current = current
                    .MakePointerMatcher(startIndex + i)
                    .AnchorPointer(toFind[i].Anchor!)
                    .MakeBlockMatcher(startIndex, length);
            }
            return current;
        }

        public static TBlockMatcher FindAndAnchor<TSelf, TElement, TPointerMatcher, TBlockMatcher, TAnchor>(this TSelf self, SequenceBlockMatcherFindOccurence occurence, SequenceBlockMatcherFindBounds bounds, params IAutoAnchorableElementMatch<TElement, TAnchor>[] toFind)
            where TSelf : IPointerAnchorableSequenceMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>, ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TPointerMatcher : IPointerAnchorableSequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher, TAnchor>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TAnchor : notnull
            => FindAndAnchor<TSelf, TElement, TPointerMatcher, TBlockMatcher, TAnchor>(self, occurence, bounds, (IReadOnlyList<IAutoAnchorableElementMatch<TElement, TAnchor>>)toFind);
    }
}
