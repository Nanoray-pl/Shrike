using System;
using System.Collections.Generic;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a sequence matcher handling elements of a given type.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    public interface ISequenceMatcher<TElement>
    {
        /// <summary>
        /// Returns all underlying elements this sequence matcher is working with.
        /// </summary>
        IReadOnlyList<TElement> AllElements();
    }

    /// <summary>
    /// Represents a sequence matcher handling elements of a given type, with specified pointer and block matcher implementations.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
    /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
    public interface ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> : ISequenceMatcher<TElement>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
    {
        /// <summary>
        /// Creates a pointer matcher pointing at an element relative to the bounds of this sequence matcher.
        /// </summary>
        /// <param name="element">The relative element.</param>
        TPointerMatcher PointerMatcher(SequenceMatcherRelativeElement element);

        /// <summary>
        /// Creates a block matcher encompassing elements in bounds relative to the bounds of this sequence matcher.
        /// </summary>
        /// <param name="bounds">The relative bounds.</param>
        TBlockMatcher BlockMatcher(SequenceMatcherRelativeBounds bounds);

        /// <summary>
        /// Performs a replace operation on the elements matched by this sequence matcher.
        /// </summary>
        /// <param name="elements">The new elements to replace the elements currently matched by this sequence matcher.</param>
        /// <returns>A new block matcher representing the state after replacing the matched elements with the new elements.</returns>
        TBlockMatcher Replace(IEnumerable<TElement> elements);

        /// <summary>
        /// Performs a remove operation on the elements matched by this sequence matcher.
        /// </summary>
        /// <returns>A new block matcher representing the state after removing the matched elements.</returns>
        TBlockMatcher Remove();

        /// <summary>
        /// Performs an insert operation before/after the elements matched by this sequence matcher.
        /// </summary>
        /// <param name="position">The position the new elements should be inserted at.</param>
        /// <param name="includeInsertionInResultingBounds">Whether the resulting block matcher should also include the newly inserted elements in the range it points at.</param>
        /// <param name="elements">The new elements to insert.</param>
        /// <returns>A new block matcher representing the state after inserting the new elements, pointing at the currently matched elements.</returns>
        TBlockMatcher Insert(SequenceMatcherPastBoundsDirection position, bool includeInsertionInResultingBounds, IEnumerable<TElement> elements);

        /// <summary>
        /// Creates a pointer matcher encompassing all underlying elements and pointing at a specific index.
        /// </summary>
        /// <param name="index">The index the created pointer matcher should point at.</param>
        TPointerMatcher MakePointerMatcher(int index)
#if NET7_0_OR_GREATER
            => TPointerMatcher.MakeNewPointerMatcher(this.AllElements(), index);
#else
            => this.MakeNewPointerMatcher(this.AllElements(), index);
#endif

        /// <summary>
        /// Creates a block matcher encompassing all underlying elements and pointing at a specific range.
        /// </summary>
        /// <param name="startIndex">The starting index the created block matcher should point at.</param>
        /// <param name="length">The length of the range the created block matcher should point at.</param>
        TBlockMatcher MakeBlockMatcher(int startIndex, int length)
#if NET7_0_OR_GREATER
            => TBlockMatcher.MakeNewBlockMatcher(this.AllElements(), startIndex, length);
#else
            => this.MakeNewBlockMatcher(this.AllElements(), startIndex, length);
#endif

        /// <summary>
        /// Creates a block matcher encompassing all underlying elements and pointing at a specific range.
        /// </summary>
        /// <param name="range">The range the created block matcher should point at.</param>
        TBlockMatcher MakeBlockMatcher(Range range)
            => this.MakeBlockMatcher(range.Start.Value, range.End.Value - range.Start.Value);

#if NET7_0_OR_GREATER
        /// <summary>
        /// Creates a new pointer matcher.
        /// </summary>
        /// <param name="allElements">The elements the new pointer matcher should work with.</param>
        /// <param name="index">The index the created pointer matcher should point at.</param>
        static abstract TPointerMatcher MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index);

        /// <summary>
        /// Creates a new block matcher.
        /// </summary>
        /// <param name="allElements">The elements the new block matcher should work with.</param>
        /// <param name="startIndex">The starting index the created block matcher should point at.</param>
        /// <param name="length">The length of the range the created block matcher should point at.</param>
        static abstract TBlockMatcher MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length);

        /// <summary>
        /// Creates a new block matcher.
        /// </summary>
        /// <param name="allElements">The elements the new block matcher should work with.</param>
        /// <param name="range">The range the created block matcher should point at.</param>
        static TBlockMatcher MakeNewBlockMatcher(IEnumerable<TElement> allElements, Range range)
            => TBlockMatcher.MakeNewBlockMatcher(allElements, range.Start.Value, range.End.Value - range.Start.Value);
#else
        /// <summary>
        /// Creates an unrelated pointer matcher.
        /// </summary>
        /// <param name="allElements">The elements the new pointer matcher should work with.</param>
        /// <param name="index">The index the created pointer matcher should point at.</param>
        TPointerMatcher MakeNewPointerMatcher(IEnumerable<TElement> allElements, int index);

        /// <summary>
        /// Creates an unrelated block matcher.
        /// </summary>
        /// <param name="allElements">The elements the new block matcher should work with.</param>
        /// <param name="startIndex">The starting index the created block matcher should point at.</param>
        /// <param name="length">The length of the range the created block matcher should point at.</param>
        TBlockMatcher MakeNewBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length);

        /// <summary>
        /// Creates an unrelated block matcher.
        /// </summary>
        /// <param name="allElements">The elements the new block matcher should work with.</param>
        /// <param name="range">The range the created block matcher should point at.</param>
        TBlockMatcher MakeNewBlockMatcher(IEnumerable<TElement> allElements, Range range)
            => this.MakeNewBlockMatcher(allElements, range.Start.Value, range.End.Value - range.Start.Value);
#endif

        /// <summary>
        /// Performs a provided set of operations on each set of elements matching the given criteria.
        /// </summary>
        /// <param name="bounds">The bounds in which to do the search.</param>
        /// <param name="toFind">The sequence of criteria to find.</param>
        /// <param name="closure">The set of operations to perform.</param>
        /// <param name="minExpectedOccurences">The minimum amount of expected occurences. If less are found, the method throws.</param>
        /// <param name="maxExpectedOccurences">The maximum amount of expected occurences. If more are found, the method throws.</param>
        /// <returns>A new block matcher representing the state after performing the provided set of operations on each set of elements matching the given criteria. The new block matcher points at the whole range that was searched against.</returns>
        TBlockMatcher ForEach(SequenceMatcherRelativeBounds bounds, IReadOnlyList<IElementMatch<TElement>> toFind, Func<TBlockMatcher, TBlockMatcher> closure, int minExpectedOccurences = 0, int maxExpectedOccurences = int.MaxValue)
        {
            var matcher = this.BlockMatcher(bounds);
            bounds = SequenceMatcherRelativeBounds.Enclosed;
            int foundOccurences = 0;

            matcher = matcher
                .Do(matcher =>
                {
                    while (true)
                    {
                        try
                        {
                            matcher = matcher
                                .Find(SequenceBlockMatcherFindOccurence.First, bounds, toFind)
                                .Do(closure);
                            foundOccurences++;
                            if (bounds is not SequenceMatcherRelativeBounds.Before or SequenceMatcherRelativeBounds.After)
                                bounds = SequenceMatcherRelativeBounds.After;
                        }
                        catch (SequenceMatcherException)
                        {
                            return matcher;
                        }
                    }
                });

            if (foundOccurences < minExpectedOccurences)
                throw new SequenceMatcherException($"ForEach operation expected at least {minExpectedOccurences} occurence(s), but found {foundOccurences}.");
            if (foundOccurences > maxExpectedOccurences)
                throw new SequenceMatcherException($"ForEach operation expected at most {maxExpectedOccurences} occurence(s), but found {foundOccurences}.");
            return matcher;
        }
    }

    /// <summary>
    /// A static class hosting "default implementation" extensions for <see cref="ISequenceMatcher{TElement}"/> and <see cref="ISequenceMatcher{TElement, TPointerMatcher, TBlockMatcher}"/> types.
    /// </summary>
    public static class ISequenceMatcherDefaultImplementations
    {
        /// <summary>
        /// Creates a pointer matcher encompassing all underlying elements and pointing at a specific index.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="index">The index the created pointer matcher should point at.</param>
        public static TPointerMatcher MakePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int index)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.MakePointerMatcher(index);

        /// <summary>
        /// Creates a block matcher encompassing all underlying elements and pointing at a specific range.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="startIndex">The starting index the created block matcher should point at.</param>
        /// <param name="length">The length of the range the created block matcher should point at.</param>
        public static TBlockMatcher MakeBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int startIndex, int length)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.MakeBlockMatcher(startIndex, length);

        /// <summary>
        /// Creates a block matcher encompassing all underlying elements and pointing at a specific range.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="range">The range the created block matcher should point at.</param>
        public static TBlockMatcher MakeBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, Range range)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.MakeBlockMatcher(range);

#if !NET7_0_OR_GREATER
        /// <summary>
        /// Creates an unrelated block matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="allElements">The elements the new block matcher should work with.</param>
        /// <param name="range">The range the created block matcher should point at.</param>
        public static TBlockMatcher MakeNewBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, IEnumerable<TElement> allElements, Range range)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.MakeNewBlockMatcher(allElements, range);
#endif

        /// <summary>
        /// Performs a provided set of operations on each set of elements matching the given criteria.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="bounds">The bounds in which to do the search.</param>
        /// <param name="toFind">The sequence of criteria to find.</param>
        /// <param name="closure">The set of operations to perform.</param>
        /// <param name="minExpectedOccurences">The minimum amount of expected occurences. If less are found, the method throws.</param>
        /// <param name="maxExpectedOccurences">The maximum amount of expected occurences. If more are found, the method throws.</param>
        /// <returns>A new block matcher representing the state after performing the provided set of operations on each set of elements matching the given criteria. The new block matcher points at the whole range that was searched against.</returns>
        public static TBlockMatcher ForEach<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherRelativeBounds bounds, IReadOnlyList<IElementMatch<TElement>> toFind, Func<TBlockMatcher, TBlockMatcher> closure, int minExpectedOccurences = 0, int maxExpectedOccurences = int.MaxValue)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.ForEach(bounds, toFind, closure, minExpectedOccurences, maxExpectedOccurences);
    }

    /// <summary>
    /// A static class hosting additional extensions for <see cref="ISequenceMatcher{TElement}"/> and <see cref="ISequenceMatcher{TElement, TPointerMatcher, TBlockMatcher}"/> types.
    /// </summary>
    public static class ISequenceMatcherExt
    {
        /// <summary>
        /// Performs a replace operation on the elements matched by this sequence matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="elements">The new elements to replace the elements currently matched by this sequence matcher.</param>
        /// <returns>A new block matcher representing the state after replacing the matched elements with the new elements.</returns>
        public static TBlockMatcher Replace<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, params TElement[] elements)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Replace(elements);

        /// <summary>
        /// Performs an insert operation before/after the elements matched by this sequence matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="position">The position the new elements should be inserted at.</param>
        /// <param name="includeInsertionInResultingBounds">Whether the resulting block matcher should also include the newly inserted elements in the range it points at.</param>
        /// <param name="elements">The new elements to insert.</param>
        /// <returns>A new block matcher representing the state after inserting the new elements, pointing at the currently matched elements.</returns>
        public static TBlockMatcher Insert<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherPastBoundsDirection position, bool includeInsertionInResultingBounds, params TElement[] elements)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Insert(position, includeInsertionInResultingBounds, elements);
    }
}
