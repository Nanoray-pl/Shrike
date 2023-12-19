using System.Collections.Generic;
using System.Xml.Linq;

namespace Nanoray.Shrike;

/// <summary>
/// A static class hosting additional overload extension methods for sequence matchers.
/// </summary>
public static class ISequenceMatcherExtensions
{
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
}
