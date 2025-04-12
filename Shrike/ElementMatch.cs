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
    [Obsolete($"Use the `{nameof(Any)}` member instead.")]
    public static ElementMatch<TElement> True
        => new("<anything>", _ => true);

    /// <summary>
    /// An element match matching any element.
    /// </summary>
    public static ElementMatch<TElement> Any
        => new("<anything>", _ => true);

    /// <summary>
    /// A description of the match, used mostly for debugging purposes.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// A list of delegates which will be called whenever this matcher matches any method during the use of the <see cref="ISequenceMatcher{TElement}.Find(SequenceBlockMatcherFindOccurence, SequenceMatcherRelativeBounds, IReadOnlyList{ElementMatch{TElement}})"/> method.
    /// </summary>
    public IReadOnlyList<ElementMatchDelegate<TElement>> Delegates { get; init; } = new List<ElementMatchDelegate<TElement>>();

    /// <summary>
    /// The function that tests whether a given element matches this match.
    /// </summary>
    public Func<TElement, bool> Closure { get; init; }

    /// <summary>
    /// A list of delegates that get called each time a find operation is started at a new position of a sequence.
    /// </summary>
    public IReadOnlyList<Action>? SetupDelegates { get; init; }

    /// <summary>
    /// A list of postcondition delegates which decide whether the match was actually successful as a whole.
    /// </summary>
    public IReadOnlyList<Func<TElement, bool>>? Postconditions { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementMatch{TElement}"/> class.
    /// </summary>
    /// <param name="description">A description of the match, used mostly for debugging purposes.</param>
    /// <param name="closure">The function that tests whether a given element matches this match.</param>
    public ElementMatch(string description, Func<TElement, bool> closure)
    {
        this.Description = description;
        this.Closure = closure;
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
    /// Creates a copy of the match with an additional match delegate, which will be called whenever this matcher matches any method during the use of the <see cref="ISequenceMatcher{TElement}.Find(SequenceBlockMatcherFindOccurence, SequenceMatcherRelativeBounds, IReadOnlyList{ElementMatch{TElement}})"/> method.
    /// </summary>
    /// <param name="delegate">The delegate.</param>
    /// <returns>A new element match with an additional match delegate.</returns>
    public ElementMatch<TElement> WithDelegate(ElementMatchDelegate<TElement> @delegate)
        => this with { Delegates = this.Delegates.Append(@delegate).ToList() };

    /// <summary>
    /// Creates a copy of the match with an additional setup delegate, which will be called each time a find operation is started at a new position of a sequence.
    /// </summary>
    /// <param name="delegate">The delegate.</param>
    /// <returns>A new element match with an additional match delegate.</returns>
    public ElementMatch<TElement> WithSetupDelegate(Action @delegate)
        => this with { SetupDelegates = this.SetupDelegates is null ? new List<Action> { @delegate } : this.SetupDelegates.Append(@delegate).ToList() };

    /// <summary>
    /// Creates a copy of the match with an additional postcondition that decides whether the match was actually successful as a whole.
    /// </summary>
    /// <param name="postcondition">The postcondition delegate.</param>
    /// <returns>A new element match with an additional postcondition.</returns>
    public ElementMatch<TElement> WithPostcondition(Func<TElement, bool> postcondition)
        => this with { Postconditions = this.Postconditions is null ? new List<Func<TElement, bool>> { postcondition } : this.Postconditions.Append(postcondition).ToList() };
}
