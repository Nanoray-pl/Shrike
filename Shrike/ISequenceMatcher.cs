using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike;

/// <summary>
/// Represents a sequence matcher handling elements of a given type.
/// </summary>
/// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
public interface ISequenceMatcher<TElement>
{
    #region Current state
    /// <summary>
    /// Returns all underlying elements this sequence matcher is working with.
    /// </summary>
    IReadOnlyList<TElement> AllElements();

    /// <summary>
    /// Data attached to single elements of the matcher.
    /// </summary>
    IReadOnlyList<SequencePointerAttachedData> PointerAttachedData();

    /// <summary>
    /// Data attached to ranges of elements of the matcher.
    /// </summary>
    IReadOnlyList<SequenceBlockAttachedData> BlockAttachedData();
    #endregion

    #region Pointer<->Block conversions
    /// <summary>
    /// Creates a block matcher pointing at the same elements this sequence matcher points at, or the same matcher if it is a block matcher already.
    /// </summary>
    SequenceBlockMatcher<TElement> BlockMatcher();

    /// <summary>
    /// Creates a pointer matcher pointing at an element relative to the bounds of this sequence matcher.
    /// </summary>
    /// <param name="element">The relative element.</param>
    SequencePointerMatcher<TElement> PointerMatcher(SequenceMatcherRelativeElement element);

    /// <summary>
    /// Creates a block matcher encompassing elements in bounds relative to the bounds of this sequence matcher.
    /// </summary>
    /// <param name="bounds">The relative bounds.</param>
    SequenceBlockMatcher<TElement> BlockMatcher(SequenceMatcherRelativeBounds bounds);
    #endregion

    #region Sequence modification
    /// <summary>
    /// Performs an insert operation before/after the elements matched by this sequence matcher.
    /// </summary>
    /// <param name="position">The position the new elements should be inserted at.</param>
    /// <param name="resultingBounds">The resulting bounds after insertion.</param>
    /// <param name="elements">The new elements to insert.</param>
    /// <returns>A new block matcher representing the state after inserting the new element.</returns>
    SequenceBlockMatcher<TElement> Insert(SequenceMatcherPastBoundsDirection position, SequenceMatcherInsertionResultingBounds resultingBounds, IEnumerable<TElement> elements);

    /// <summary>
    /// Performs a replace operation on the elements matched by this sequence matcher.
    /// </summary>
    /// <param name="element">The new element to replace the elements currently matched by this sequence matcher.</param>
    /// <returns>A new pointer matcher representing the state after replacing the matched elements with the new element.</returns>
    SequencePointerMatcher<TElement> Replace(TElement element);

    /// <summary>
    /// Performs a replace operation on the elements matched by this sequence matcher.
    /// </summary>
    /// <param name="elements">The new elements to replace the elements currently matched by this sequence matcher.</param>
    /// <returns>A new block matcher representing the state after replacing the matched elements with the new elements.</returns>
    SequenceBlockMatcher<TElement> Replace(IEnumerable<TElement> elements);

    /// <summary>
    /// Performs a remove operation on the element matched by this sequence matcher.
    /// </summary>
    /// <param name="postRemovalPosition">The position the resulting pointer matcher should point at.</param>
    /// <returns>A new pointer matcher representing the state after removing the matched element.</returns>
    SequencePointerMatcher<TElement> Remove(SequenceMatcherPastBoundsDirection postRemovalPosition);

    /// <summary>
    /// Performs a remove operation on the elements matched by this sequence matcher.
    /// </summary>
    /// <returns>A new block matcher representing the state after removing the matched elements.</returns>
    SequenceBlockMatcher<TElement> Remove();
    #endregion

    #region Cursor manipulation
    /// <summary>
    /// Encompasses the next/previous elements.
    /// </summary>
    /// <param name="direction">The direction to encompass elements at.</param>
    /// <param name="length">The number of next/previous elements to encompass.</param>
    /// <returns>A new block matcher pointing at the same elements this sequence matcher points at, and additionally the given number of next/previous elements.</returns>
    SequenceBlockMatcher<TElement> Encompass(SequenceMatcherEncompassDirection direction, int length);

    /// <summary>
    /// Encompasses elements until a sequence of elements matching the given criteria.
    /// </summary>
    /// <param name="direction">The direction to encompass elements at.</param>
    /// <param name="toFind">The sequence of criteria to find.</param>
    /// <returns>A new block matcher pointing at the same elements this sequence matcher points at, and additionally elements up until (including) a sequence of elements matching the given criteria.</returns>
    SequenceBlockMatcher<TElement> EncompassUntil(SequenceMatcherPastBoundsDirection direction, IReadOnlyList<ElementMatch<TElement>> toFind);

    /// <summary>
    /// Finds the first element matching the given criteria.
    /// </summary>
    /// <remarks>
    /// The search will be performed:
    /// <list type="bullet">
    /// <item>On all elements, if this matcher points at all underlying elements, or</item>
    /// <item>On elements after the elements this matcher points at.</item>
    /// </list>
    /// </remarks>
    /// <param name="toFind">The criteria of an element to find.</param>
    /// <returns>A new pointer matcher pointing at the element matching the given criteria.</returns>
    SequencePointerMatcher<TElement> Find(ElementMatch<TElement> toFind);

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
    SequenceBlockMatcher<TElement> Find(IReadOnlyList<ElementMatch<TElement>> toFind);

    /// <summary>
    /// Finds the first element matching the given criteria.
    /// </summary>
    /// <param name="occurence">Whether to find the first or last occurence of the match.</param>
    /// <param name="bounds">The bounds in which to do the search.</param>
    /// <param name="toFind">The criteria of an element to find.</param>
    /// <returns>A new block matcher pointing at the sequence of elements matching the given criteria.</returns>
    SequencePointerMatcher<TElement> Find(SequenceBlockMatcherFindOccurence occurence, SequenceMatcherRelativeBounds bounds, ElementMatch<TElement> toFind);

    /// <summary>
    /// Finds a sequence of elements matching the given criteria.
    /// </summary>
    /// <param name="occurence">Whether to find the first or last occurence of the match.</param>
    /// <param name="bounds">The bounds in which to do the search.</param>
    /// <param name="toFind">The sequence of criteria to find.</param>
    /// <returns>A new block matcher pointing at the sequence of elements matching the given criteria.</returns>
    SequenceBlockMatcher<TElement> Find(SequenceBlockMatcherFindOccurence occurence, SequenceMatcherRelativeBounds bounds, IReadOnlyList<ElementMatch<TElement>> toFind);
    #endregion
}

/// <summary>
/// Represents a sequence matcher handling elements of a given type.
/// </summary>
/// <typeparam name="TSelf">This matcher's type.</typeparam>
/// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
public interface ISequenceMatcher<TSelf, TElement> : ISequenceMatcher<TElement>
    where TSelf : ISequenceMatcher<TSelf, TElement>
{
    #region Basic data modification
    /// <summary>
    /// Creates a matcher with additional pointer-attached data.
    /// </summary>
    /// <param name="index">The index of the element to attach the data to.</param>
    /// <param name="data">The attached data.</param>
    /// <returns>A new matcher containing the additional data.</returns>
    TSelf WithPointerAttachedData(int index, object data);

    /// <summary>
    /// Creates a matcher with additional block-attached data.
    /// </summary>
    /// <param name="startIndex">The starting index of the element range to attach the data to.</param>
    /// <param name="length">The length of the element range to attach the data to.</param>
    /// <param name="data">The attached data.</param>
    /// <returns>A new matcher containing the additional data.</returns>
    TSelf WithBlockAttachedData(int startIndex, int length, object data);
    #endregion

    #region Sequence modification
    /// <summary>
    /// Performs a provided set of operations on <i>only</i> the elements matched by this matcher.
    /// </summary>
    /// <param name="closure">The set of operations to perform.</param>
    /// <returns>A new block matcher representing the state after performing the provided set of operations on the elements matched by this matcher.</returns>
    SequenceBlockMatcher<TElement> Do(Func<TSelf, SequenceBlockMatcher<TElement>> closure);
    #endregion
}
