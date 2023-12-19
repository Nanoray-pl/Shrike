using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike;

/// <summary>
/// Represents a simple single sequence element match.
/// </summary>
/// <typeparam name="TElement">The type of elements this object can match.</typeparam>
public readonly struct ElementMatch<TElement>
{
    /// <summary>
    /// An element match matching any element.
    /// </summary>
    public static ElementMatch<TElement> True
        => new("<anything>", _ => true);

    /// <summary>
    /// A description of the match, used mostly for debugging purposes.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// A list of delegates which will be called whenever this matcher matches any method during the use of the <see cref="ISequenceMatcher{TElement, TPointerMatcher, TBlockMatcher}.Find(SequenceBlockMatcherFindOccurence, SequenceMatcherRelativeBounds, IReadOnlyList{ElementMatch{TElement, TPointerMatcher, TBlockMatcher}})"/> method.
    /// </summary>
    public IReadOnlyList<ElementMatchDelegate<TElement>> Delegates { get; init; }

    private Func<TElement, bool> Closure { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementMatch{TElement}"/> class.
    /// </summary>
    /// <param name="description">A description of the match, used mostly for debugging purposes.</param>
    /// <param name="closure">The function that tests whether a given element matches this match.</param>
    public ElementMatch(string description, Func<TElement, bool> closure)
    {
        this.Description = description;
        this.Closure = closure;
        this.Delegates = new List<ElementMatchDelegate<TElement>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementMatch{TElement}"/> class, matching a specific element.
    /// </summary>
    /// <param name="element">The element that satisfies this match.</param>
    /// <remarks>The element's <see cref="object.Equals(object?)"/> method will be used for matching.</remarks>
    public ElementMatch(TElement element) : this($"{element}", e => Equals(e, element)) { }

    /// <summary>
    /// Tests whether a given element matches this match.
    /// </summary>
    /// <param name="element">The element to test against.</param>
    /// <returns>Whether the given element matches this match.</returns>
    public bool Matches(TElement element)
        => this.Closure(element);

    /// <summary>
    /// Creates a copy of the match with an additional match delegate, which will be called whenever this matcher matches any method during the use of the <see cref="ISequenceMatcher{TElement, TPointerMatcher, TBlockMatcher}.Find(SequenceBlockMatcherFindOccurence, SequenceMatcherRelativeBounds, IReadOnlyList{ElementMatch{TElement, TPointerMatcher, TBlockMatcher}})"/> method.
    /// </summary>
    /// <param name="delegate"></param>
    /// <returns>A new element match with an additional match delegate.</returns>
    public ElementMatch<TElement> WithDelegate(ElementMatchDelegate<TElement> @delegate)
        => new()
        {
            Description = this.Description,
            Closure = this.Closure,
            Delegates = this.Delegates.Append(@delegate).ToList()
        };
}
