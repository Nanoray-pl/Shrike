namespace Nanoray.Shrike;

/// <summary>
/// A delegate called when an <see cref="ElementMatch{TElement}"/> gets found with the <see cref="ISequenceMatcher{TElement, TPointerMatcher, TBlockMatcher}.Find(SequenceBlockMatcherFindOccurence, SequenceMatcherRelativeBounds, System.Collections.Generic.IReadOnlyList{ElementMatch{TElement, TPointerMatcher, TBlockMatcher}})"/> method.
/// </summary>
/// <typeparam name="TElement">The type of elements this object can match.</typeparam>
/// <param name="matcher">The block matcher containing the matched element.</param>
/// <param name="position">The position at which the matched element is.</param>
/// <param name="element">The matched element.</param>
/// <return>The resulting block matcher after a transformation by this delegate.</return>
public delegate SequenceBlockMatcher<TElement> ElementMatchDelegate<TElement>(
    SequenceBlockMatcher<TElement> matcher, int position, TElement element
);
