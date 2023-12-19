using System;
using System.Collections.Generic;

namespace Nanoray.Shrike;

/// <summary>
/// A static class hosting additional overload extension methods for sequence matchers.
/// </summary>
public static class ISequenceMatcherExtensions
{
    /// <summary>
    /// Creates a new pointer matcher, pointing at an arbitrary element.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The current matcher.</param>
    /// <param name="index">The index to point at.</param>
    /// <returns>A new pointer matcher, pointing at the given element.</returns>
    public static SequencePointerMatcher<TElement> MakePointerMatcher<TElement>(this ISequenceMatcher<TElement> self, int index)
        => SequencePointerMatcher<TElement>.From(self, index);

    /// <summary>
    /// Creates a new block matcher, pointing at an arbitrary range of elements.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The current matcher.</param>
    /// <param name="startIndex">The starting index of the range of elements to point at.</param>
    /// <param name="length">The length of the range of elements to point at.</param>
    /// <returns>A new pointer matcher, pointing at the given element.</returns>
    public static SequenceBlockMatcher<TElement> MakeBlockMatcher<TElement>(this ISequenceMatcher<TElement> self, int startIndex, int length)
        => SequenceBlockMatcher<TElement>.From(self, startIndex, length);

    /// <summary>
    /// Performs an insert operation before/after the elements matched by this sequence matcher.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The current matcher.</param>
    /// <param name="position">The position the new elements should be inserted at.</param>
    /// <param name="resultingBounds">The resulting bounds after insertion.</param>
    /// <param name="elements">The new elements to insert.</param>
    /// <returns>A new block matcher representing the state after inserting the new element.</returns>
    public static SequenceBlockMatcher<TElement> Insert<TElement>(this ISequenceMatcher<TElement> self, SequenceMatcherPastBoundsDirection position, SequenceMatcherInsertionResultingBounds resultingBounds, params TElement[] elements)
        => self.Insert(position, resultingBounds, elements);

    /// <summary>
    /// Performs a replace operation on the elements matched by this sequence matcher.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The current matcher.</param>
    /// <param name="elements">The new elements to replace the elements currently matched by this sequence matcher.</param>
    /// <returns>A new block matcher representing the state after replacing the matched elements with the new elements.</returns>
    public static SequenceBlockMatcher<TElement> Replace<TElement>(this ISequenceMatcher<TElement> self, params TElement[] elements)
        => self.Replace(elements);

    /// <summary>
    /// Encompasses elements until a sequence of elements matching the given criteria.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The current matcher.</param>
    /// <param name="direction">The direction to encompass elements at.</param>
    /// <param name="toFind">The sequence of criteria to find.</param>
    /// <returns>A new block matcher pointing at the same elements this sequence matcher points at, and additionally elements up until (including) a sequence of elements matching the given criteria.</returns>
    public static SequenceBlockMatcher<TElement> EncompassUntil<TElement>(this ISequenceMatcher<TElement> self, SequenceMatcherPastBoundsDirection direction, params ElementMatch<TElement>[] toFind)
        => self.EncompassUntil(direction, toFind);

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
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The current matcher.</param>
    /// <param name="toFind">The sequence of criteria to find.</param>
    /// <returns>A new block matcher pointing at the sequence of elements matching the given criteria.</returns>
    public static SequenceBlockMatcher<TElement> Find<TElement>(this ISequenceMatcher<TElement> self, params ElementMatch<TElement>[] toFind)
        => self.Find(toFind);

    /// <summary>
    /// Finds a sequence of elements matching the given criteria.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The current matcher.</param>
    /// <param name="occurence">Whether to find the first or last occurence of the match.</param>
    /// <param name="bounds">The bounds in which to do the search.</param>
    /// <param name="toFind">The sequence of criteria to find.</param>
    /// <returns>A new block matcher pointing at the sequence of elements matching the given criteria.</returns>
    public static SequenceBlockMatcher<TElement> Find<TElement>(this ISequenceMatcher<TElement> self, SequenceBlockMatcherFindOccurence occurence, SequenceMatcherRelativeBounds bounds, params ElementMatch<TElement>[] toFind)
        => self.Find(occurence, bounds, toFind);

    /// <summary>
    /// Performs a provided set of operations on each set of elements matching the given criteria.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The current matcher.</param>
    /// <param name="bounds">The bounds in which to do the search.</param>
    /// <param name="toFind">The sequence of criteria to find.</param>
    /// <param name="closure">The set of operations to perform.</param>
    /// <param name="minExpectedOccurences">The minimum amount of expected occurences. If less are found, the method throws.</param>
    /// <param name="maxExpectedOccurences">The maximum amount of expected occurences. If more are found, the method throws.</param>
    /// <returns>A new block matcher representing the state after performing the provided set of operations on each set of elements matching the given criteria. The new block matcher points at the whole range that was searched against.</returns>
    public static SequenceBlockMatcher<TElement> ForEach<TElement>(this ISequenceMatcher<TElement> self, SequenceMatcherRelativeBounds bounds, IReadOnlyList<ElementMatch<TElement>> toFind, Func<SequenceBlockMatcher<TElement>, SequenceBlockMatcher<TElement>> closure, int minExpectedOccurences = 0, int maxExpectedOccurences = int.MaxValue)
    {
        var matcher = self.BlockMatcher(bounds);
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
