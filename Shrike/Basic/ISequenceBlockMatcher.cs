using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike
{
    public interface ISequenceBlockMatcher<TElement> : ISequenceMatcher<TElement>
    {
        int StartIndex();

        int EndIndex();

        int Length();

        IEnumerable<TElement> Elements()
            => this.AllElements().Skip(this.StartIndex()).Take(this.Length());
    }

    public interface ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> : ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher>, ISequenceBlockMatcher<TElement>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
    {
        TPointerMatcher PointerMatcherBeforeStart()
            => this.MakePointerMatcher(this.StartIndex() - 1);

        TPointerMatcher PointerMatcherAtStart()
            => this.MakePointerMatcher(this.StartIndex());

        TPointerMatcher PointerMatcherAtEnd()
            => this.MakePointerMatcher(this.EndIndex() - 1);

        TPointerMatcher PointerMatcherAfterEnd()
            => this.MakePointerMatcher(this.EndIndex());

        TBlockMatcher BlockMatcherBeforeStart(int length = 0)
            => this.MakeBlockMatcher(this.StartIndex() - length, length);

        TBlockMatcher BlockMatcherAfterEnd(int length = 0)
            => this.MakeBlockMatcher(this.EndIndex(), length);

        TBlockMatcher Find(IReadOnlyList<IElementMatch<TElement>> toFind)
            => this.Find(
                SequenceBlockMatcherFindOccurence.First,
                this.StartIndex() == 0 && this.Length() == this.AllElements().Count ? SequenceBlockMatcherFindBounds.Enclosed : SequenceBlockMatcherFindBounds.After,
                toFind
            );

        TBlockMatcher Find(SequenceBlockMatcherFindOccurence occurence, SequenceBlockMatcherFindBounds bounds, IReadOnlyList<IElementMatch<TElement>> toFind)
        {
            int startIndex, endIndex;
            switch (bounds)
            {
                case SequenceBlockMatcherFindBounds.Before:
                    startIndex = 0;
                    endIndex = this.StartIndex();
                    break;
                case SequenceBlockMatcherFindBounds.BeforeOrEnclosed:
                    startIndex = 0;
                    endIndex = this.EndIndex();
                    break;
                case SequenceBlockMatcherFindBounds.Enclosed:
                    startIndex = this.StartIndex();
                    endIndex = this.EndIndex();
                    break;
                case SequenceBlockMatcherFindBounds.AfterOrEnclosed:
                    startIndex = this.StartIndex();
                    endIndex = this.AllElements().Count;
                    break;
                case SequenceBlockMatcherFindBounds.After:
                    startIndex = this.EndIndex();
                    endIndex = this.AllElements().Count;
                    break;
                case SequenceBlockMatcherFindBounds.WholeSequence:
                    startIndex = 0;
                    endIndex = this.AllElements().Count;
                    break;
                default:
                    throw new ArgumentException($"{nameof(SequenceBlockMatcherFindBounds)} has an invalid value.");
            }

            switch (occurence)
            {
                case SequenceBlockMatcherFindOccurence.First:
                    {
                        int maxIndex = endIndex - toFind.Count;
                        for (int index = startIndex; index < maxIndex; index++)
                        {
                            for (int toFindIndex = 0; toFindIndex < toFind.Count; toFindIndex++)
                            {
                                if (!toFind[toFindIndex].Matches(this.AllElements()[index + toFindIndex]))
                                    goto continueOuter;
                            }
                            return this.MakeBlockMatcher(index, toFind.Count);
                        continueOuter:;
                        }
                        break;
                    }
                case SequenceBlockMatcherFindOccurence.Last:
                    {
                        int minIndex = startIndex + toFind.Count - 1;
                        for (int index = endIndex - 1; index >= minIndex; index--)
                        {
                            for (int toFindIndex = toFind.Count - 1; toFindIndex >= 0; toFindIndex--)
                            {
                                if (!toFind[toFindIndex].Matches(this.AllElements()[index + toFindIndex - toFind.Count + 1]))
                                    goto continueOuter;
                            }
                            return this.MakeBlockMatcher(index - toFind.Count + 1, toFind.Count);
                        continueOuter:;
                        }
                        break;
                    }
                default:
                    throw new ArgumentException($"{nameof(SequenceBlockMatcherFindOccurence)} has an invalid value.");
            }
            throw new SequenceMatcherException($"Pattern not found:\n{string.Join("\n", toFind.Select(i => $"\t{i.Description}"))}");
        }

        TBlockMatcher Encompass(SequenceMatcherPastBoundsDirection direction, int length)
        {
            if (length == 0)
                return (TBlockMatcher)this;
            if (length < 0)
                throw new IndexOutOfRangeException($"Invalid value {length} for parameter `{nameof(length)}`.");
            return direction switch
            {
                SequenceMatcherPastBoundsDirection.Before => this.MakeBlockMatcher(this.StartIndex() - length, this.Length() + length),
                SequenceMatcherPastBoundsDirection.After => this.MakeBlockMatcher(this.StartIndex(), this.Length() + length),
                _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value."),
            };
        }

        TBlockMatcher EncompassUntil(SequenceMatcherPastBoundsDirection direction, IReadOnlyList<IElementMatch<TElement>> toFind)
        {
            var findOccurence = direction switch
            {
                SequenceMatcherPastBoundsDirection.Before => SequenceBlockMatcherFindOccurence.Last,
                SequenceMatcherPastBoundsDirection.After => SequenceBlockMatcherFindOccurence.First,
                _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value."),
            };
            var findBounds = direction switch
            {
                SequenceMatcherPastBoundsDirection.Before => SequenceBlockMatcherFindBounds.Before,
                SequenceMatcherPastBoundsDirection.After => SequenceBlockMatcherFindBounds.After,
                _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value."),
            };

            var findMatcher = this.Find(findOccurence, findBounds, toFind);
            return direction switch
            {
                SequenceMatcherPastBoundsDirection.Before => this.MakeBlockMatcher(findMatcher.StartIndex(), this.EndIndex() - findMatcher.StartIndex()),
                SequenceMatcherPastBoundsDirection.After => this.MakeBlockMatcher(this.StartIndex(), findMatcher.EndIndex() - this.StartIndex()),
                _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value."),
            };
        }

        TBlockMatcher Insert(SequenceMatcherPastBoundsDirection position, bool includeInsertionInResultingBounds, IEnumerable<TElement> elements);

        TBlockMatcher Do(Func<TBlockMatcher, TBlockMatcher> closure);

        TBlockMatcher Do(Func<TBlockMatcher, TPointerMatcher> closure)
            => this.Do(matcher => closure(matcher).BlockMatcher());
    }

    public static class ISequenceBlockMatcherDefaultImplementations
    {
        public static IEnumerable<TElement> Elements<TElement>(this ISequenceBlockMatcher<TElement> self)
            => self.Elements();

        public static TPointerMatcher PointerMatcherBeforeStart<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.PointerMatcherBeforeStart();

        public static TPointerMatcher PointerMatcherAtStart<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.PointerMatcherAtStart();

        public static TPointerMatcher PointerMatcherAtEnd<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.PointerMatcherAtEnd();

        public static TPointerMatcher PointerMatcherAfterEnd<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.PointerMatcherAfterEnd();

        public static TBlockMatcher BlockMatcherBeforeStart<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int length = 0)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.BlockMatcherBeforeStart(length);

        public static TBlockMatcher BlockMatcherAfterEnd<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int length = 0)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.BlockMatcherAfterEnd(length);

        public static TBlockMatcher Find<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, IReadOnlyList<IElementMatch<TElement>> toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Find(toFind);

        public static TBlockMatcher Find<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceBlockMatcherFindOccurence occurence, SequenceBlockMatcherFindBounds bounds, IReadOnlyList<IElementMatch<TElement>> toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Find(occurence, bounds, toFind);

        public static TBlockMatcher Encompass<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherPastBoundsDirection direction, int length)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Encompass(direction, length);

        public static TBlockMatcher EncompassUntil<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherPastBoundsDirection direction, IReadOnlyList<IElementMatch<TElement>> toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.EncompassUntil(direction, toFind);

        public static TBlockMatcher Do<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, Func<TBlockMatcher, TPointerMatcher> closure)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Do(closure);
    }

    public static class ISequenceBlockMatcherExt
    {
        public static TBlockMatcher Find<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, params IElementMatch<TElement>[] toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Find(toFind);

        public static TBlockMatcher Find<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceBlockMatcherFindOccurence occurence, SequenceBlockMatcherFindBounds bounds, params IElementMatch<TElement>[] toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Find(occurence, bounds, toFind);

        public static TBlockMatcher EncompassUntil<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherPastBoundsDirection direction, params IElementMatch<TElement>[] toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.EncompassUntil(direction, toFind);

        public static TBlockMatcher Insert<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherPastBoundsDirection position, bool includeInsertionInResultingBounds, params TElement[] elements)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Insert(position, includeInsertionInResultingBounds, elements);

        public static TBlockMatcher Repeat<TElement, TPointerMatcher, TBlockMatcher>(this TBlockMatcher self, int times, Func<TBlockMatcher, TBlockMatcher> closure)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        {
            var matcher = self;
            for (int i = 0; i < times; i++)
                matcher = closure(matcher);
            return matcher;
        }
    }
}
