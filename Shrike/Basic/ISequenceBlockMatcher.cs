using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a sequence block matcher handling elements of a given type.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    public interface ISequenceBlockMatcher<TElement> : ISequenceMatcher<TElement>
    {
        /// <summary>
        /// The (inclusive) start index of the underlying list of elements this block matcher is pointing at.
        /// </summary>
        int StartIndex();

        /// <summary>
        /// The (exclusive) end index of the underlying list of elements this block matcher is pointing at.
        /// </summary>
        int EndIndex();

        /// <summary>
        /// The length of the range this block matcher is pointing at.
        /// </summary>
        int Length();

        /// <summary>
        /// The elements this block matcher is pointing at.
        /// </summary>
        IEnumerable<TElement> Elements()
            => this.AllElements().Skip(this.StartIndex()).Take(this.Length());
    }

    /// <summary>
    /// Represents a sequence block matcher handling elements of a given type, with specified pointer and block matcher implementations.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
    /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
    public interface ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> : ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher>, ISequenceBlockMatcher<TElement>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
    {
        /// <summary>
        /// Creates a pointer matcher pointing at the element just before the first element this block matcher is pointing at.
        /// </summary>
        TPointerMatcher PointerMatcherBeforeFirst()
            => this.MakePointerMatcher(this.StartIndex() - 1);

        /// <summary>
        /// Creates a pointer matcher pointing at the first element this block matcher is pointing at.
        /// </summary>
        TPointerMatcher PointerMatcherAtFirst()
            => this.MakePointerMatcher(this.StartIndex());

        /// <summary>
        /// Creates a pointer matcher pointing at the last element this block matcher is pointing at.
        /// </summary>
        TPointerMatcher PointerMatcherAtLast()
            => this.MakePointerMatcher(this.EndIndex() - 1);

        /// <summary>
        /// Creates a pointer matcher pointing at the element just after the last element this block matcher is pointing at.
        /// </summary>
        TPointerMatcher PointerMatcherAfterLast()
            => this.MakePointerMatcher(this.EndIndex());

        /// <summary>
        /// Creates a block matcher pointing at the elements just before the first element this block matcher is pointing at.
        /// </summary>
        /// <param name="length">How many elements before the first element this block matcher is pointing at to look at.</param>
        TBlockMatcher BlockMatcherBeforeFirst(int length = 0)
            => this.MakeBlockMatcher(this.StartIndex() - length, length);

        /// <summary>
        /// Creates a block matcher pointing at the elements just after the last element this block matcher is pointing at.
        /// </summary>
        /// <param name="length">How many elements after the last element this block matcher is pointing at to look at.</param>
        TBlockMatcher BlockMatcherAfterLast(int length = 0)
            => this.MakeBlockMatcher(this.EndIndex(), length);

        /// <summary>
        /// Finds the first sequence of elements matching the given criteria.
        /// </summary>
        /// <remarks>
        /// The search will be performed:
        /// <list type="bullet">
        /// <item>On all elements, if this matcher points at all underlying elements, or</item>
        /// <item>On elements after the elements this matcher points at.</item>
        /// </list>
        /// </remarks>
        /// <param name="toFind">The sequence of criteria to find.</param>
        /// <returns>A new block matcher pointing at the sequence of elements matching the given criteria.</returns>
        TBlockMatcher Find(IReadOnlyList<IElementMatch<TElement>> toFind)
            => this.Find(
                SequenceBlockMatcherFindOccurence.First,
                this.StartIndex() == 0 && this.Length() == this.AllElements().Count ? SequenceMatcherFindBounds.Enclosed : SequenceMatcherFindBounds.After,
                toFind
            );

        /// <summary>
        /// Finds a sequence of elements matching the given criteria.
        /// </summary>
        /// <param name="occurence">Whether to find the first or last occurence of the match.</param>
        /// <param name="bounds">The bounds in which to do the search.</param>
        /// <param name="toFind">The sequence of criteria to find.</param>
        /// <returns>A new block matcher pointing at the sequence of elements matching the given criteria.</returns>
        TBlockMatcher Find(SequenceBlockMatcherFindOccurence occurence, SequenceMatcherFindBounds bounds, IReadOnlyList<IElementMatch<TElement>> toFind)
        {
            int startIndex, endIndex;
            switch (bounds)
            {
                case SequenceMatcherFindBounds.Before:
                    startIndex = 0;
                    endIndex = this.StartIndex();
                    break;
                case SequenceMatcherFindBounds.BeforeOrEnclosed:
                    startIndex = 0;
                    endIndex = this.EndIndex();
                    break;
                case SequenceMatcherFindBounds.Enclosed:
                    startIndex = this.StartIndex();
                    endIndex = this.EndIndex();
                    break;
                case SequenceMatcherFindBounds.AfterOrEnclosed:
                    startIndex = this.StartIndex();
                    endIndex = this.AllElements().Count;
                    break;
                case SequenceMatcherFindBounds.After:
                    startIndex = this.EndIndex();
                    endIndex = this.AllElements().Count;
                    break;
                case SequenceMatcherFindBounds.WholeSequence:
                    startIndex = 0;
                    endIndex = this.AllElements().Count;
                    break;
                default:
                    throw new ArgumentException($"{nameof(SequenceMatcherFindBounds)} has an invalid value.");
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

        /// <summary>
        /// Encompasses the next/previous elements.
        /// </summary>
        /// <param name="direction">The direction to encompass elements at.</param>
        /// <param name="length">The number of next/previous elements to encompass.</param>
        /// <returns>A new block matcher pointing at the same elements this block matcher points at, and additionally the given number of next/previous elements.</returns>
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

        /// <summary>
        /// Encompasses elements until a sequence of elements matching the given criteria.
        /// </summary>
        /// <param name="direction">The direction to encompass elements at.</param>
        /// <param name="toFind">The sequence of criteria to find.</param>
        /// <returns>A new block matcher pointing at the same elements this block matcher points at, and additionally elements up until (including) a sequence of elements matching the given criteria.</returns>
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
                SequenceMatcherPastBoundsDirection.Before => SequenceMatcherFindBounds.Before,
                SequenceMatcherPastBoundsDirection.After => SequenceMatcherFindBounds.After,
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

        /// <summary>
        /// Performs a provided set of operations on the elements matched by this block matcher.
        /// </summary>
        /// <param name="closure">The set of operations to perform.</param>
        /// <returns>A new block matcher representing the state after performing the provided set of operations on the elements matched by this block matcher.</returns>
        TBlockMatcher Do(Func<TBlockMatcher, TBlockMatcher> closure);

        /// <summary>
        /// Performs a provided set of operations on the elements matched by this block matcher.
        /// </summary>
        /// <param name="closure">The set of operations to perform.</param>
        /// <returns>A new block matcher representing the state after performing the provided set of operations on the elements matched by this block matcher.</returns>
        TBlockMatcher Do(Func<TBlockMatcher, TPointerMatcher> closure)
            => this.Do(matcher => closure(matcher).BlockMatcher());
    }

    /// <summary>
    /// A static class hosting "default implementation" extensions for <see cref="ISequenceBlockMatcher{TElement}"/> and <see cref="ISequenceBlockMatcher{TElement, TPointerMatcher, TBlockMatcher}"/> types.
    /// </summary>
    public static class ISequenceBlockMatcherDefaultImplementations
    {
        /// <summary>
        /// The elements this block matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <param name="self">The current matcher.</param>
        public static IEnumerable<TElement> Elements<TElement>(this ISequenceBlockMatcher<TElement> self)
            => self.Elements();

        /// <summary>
        /// Creates a pointer matcher pointing at the element just before the first element this block matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        public static TPointerMatcher PointerMatcherBeforeFirst<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.PointerMatcherBeforeFirst();

        /// <summary>
        /// Creates a pointer matcher pointing at the first element this block matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        public static TPointerMatcher PointerMatcherAtFirst<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.PointerMatcherAtFirst();

        /// <summary>
        /// Creates a pointer matcher pointing at the last element this block matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        public static TPointerMatcher PointerMatcherAtLast<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.PointerMatcherAtLast();

        /// <summary>
        /// Creates a pointer matcher pointing at the element just after the last element this block matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        public static TPointerMatcher PointerMatcherAfterLast<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.PointerMatcherAfterLast();

        /// <summary>
        /// Creates a block matcher pointing at the elements just before the first element this block matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="length">How many elements before the first element this block matcher is pointing at to look at.</param>
        public static TBlockMatcher BlockMatcherBeforeFirst<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int length = 0)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.BlockMatcherBeforeFirst(length);

        /// <summary>
        /// Creates a block matcher pointing at the elements just after the last element this block matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="length">How many elements before the first element this block matcher is pointing at to look at.</param>
        public static TBlockMatcher BlockMatcherAfterLast<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int length = 0)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.BlockMatcherAfterLast(length);

        /// <summary>
        /// Finds a sequence of elements matching the given criteria.
        /// </summary>
        /// <remarks>
        /// The search will be performed:
        /// <list type="bullet">
        /// <item>On all elements, if this matcher points at all underlying elements, or</item>
        /// <item>On elements after the elements this matcher points at.</item>
        /// </list>
        /// </remarks>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="toFind">The sequence of criteria to find.</param>
        /// <returns>A new block matcher pointing at the sequence of elements matching the given criteria.</returns>
        public static TBlockMatcher Find<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, IReadOnlyList<IElementMatch<TElement>> toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Find(toFind);

        /// <summary>
        /// Finds a sequence of elements matching the given criteria.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="occurence">Whether to find the first or last occurence of the match.</param>
        /// <param name="bounds">The bounds in which to do the search.</param>
        /// <param name="toFind">The sequence of criteria to find.</param>
        /// <returns>A new block matcher pointing at the sequence of elements matching the given criteria.</returns>
        public static TBlockMatcher Find<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceBlockMatcherFindOccurence occurence, SequenceMatcherFindBounds bounds, IReadOnlyList<IElementMatch<TElement>> toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Find(occurence, bounds, toFind);

        /// <summary>
        /// Encompasses the next/previous elements.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="direction">The direction to encompass elements at.</param>
        /// <param name="length">The number of next/previous elements to encompass.</param>
        /// <returns>A new block matcher pointing at the same elements this block matcher points at, and additionally the given number of next/previous elements.</returns>
        public static TBlockMatcher Encompass<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherPastBoundsDirection direction, int length)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Encompass(direction, length);

        /// <summary>
        /// Encompasses elements until a sequence of elements matching the given criteria.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="direction">The direction to encompass elements at.</param>
        /// <param name="toFind">The sequence of criteria to find.</param>
        /// <returns>A new block matcher pointing at the same elements this block matcher points at, and additionally elements up until (including) a sequence of elements matching the given criteria.</returns>
        public static TBlockMatcher EncompassUntil<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherPastBoundsDirection direction, IReadOnlyList<IElementMatch<TElement>> toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.EncompassUntil(direction, toFind);

        /// <summary>
        /// Performs a provided set of operations on the elements matched by this block matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="closure">The set of operations to perform.</param>
        /// <returns>A new block matcher representing the state after performing the provided set of operations on the elements matched by this block matcher.</returns>
        public static TBlockMatcher Do<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, Func<TBlockMatcher, TPointerMatcher> closure)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Do(closure);
    }

    /// <summary>
    /// A static class hosting additional extensions for <see cref="ISequenceBlockMatcher{TElement}"/> and <see cref="ISequenceBlockMatcher{TElement, TPointerMatcher, TBlockMatcher}"/> types.
    /// </summary>
    public static class ISequenceBlockMatcherExt
    {
        /// <summary>
        /// Finds a sequence of elements matching the given criteria.
        /// </summary>
        /// <remarks>
        /// The search will be performed:
        /// <list type="bullet">
        /// <item>On all elements, if this matcher points at all underlying elements, or</item>
        /// <item>On elements after the elements this matcher points at.</item>
        /// </list>
        /// </remarks>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="toFind">The sequence of criteria to find.</param>
        /// <returns>A new block matcher pointing at the sequence of elements matching the given criteria.</returns>
        public static TBlockMatcher Find<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, params IElementMatch<TElement>[] toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Find(toFind);

        /// <summary>
        /// Finds a sequence of elements matching the given criteria.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="occurence">Whether to find the first or last occurence of the match.</param>
        /// <param name="bounds">The bounds in which to do the search.</param>
        /// <param name="toFind">The sequence of criteria to find.</param>
        /// <returns>A new block matcher pointing at the sequence of elements matching the given criteria.</returns>
        public static TBlockMatcher Find<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceBlockMatcherFindOccurence occurence, SequenceMatcherFindBounds bounds, params IElementMatch<TElement>[] toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Find(occurence, bounds, toFind);

        /// <summary>
        /// Encompasses elements until a sequence of elements matching the given criteria.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="direction">The direction to encompass elements at.</param>
        /// <param name="toFind">The sequence of criteria to find.</param>
        /// <returns>A new block matcher pointing at the same elements this block matcher points at, and additionally elements up until (including) a sequence of elements matching the given criteria.</returns>
        public static TBlockMatcher EncompassUntil<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherPastBoundsDirection direction, params IElementMatch<TElement>[] toFind)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.EncompassUntil(direction, toFind);

        /// <summary>
        /// Performs a provided set of operations a given amount of times on the elements matched by this block matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="times">The number of times this set of operations should be performed.</param>
        /// <param name="closure">The set of operations to perform.</param>
        /// <returns>A new block matcher representing the state after performing the provided set of operations on the elements matched by this block matcher.</returns>
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
