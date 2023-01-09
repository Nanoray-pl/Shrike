using System;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a simple single sequence element match.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this object can match.</typeparam>
    public record ElementMatch<TElement> : IElementMatch<TElement>
    {
        /// <summary>
        /// An element match matching any element.
        /// </summary>
        public static ElementMatch<TElement> True
            => new("<anything>", _ => true);

        /// <inheritdoc/>
        public string Description { get; init; }

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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementMatch{TElement}"/> class, matching a specific element.
        /// </summary>
        /// <param name="element">The element that satisfies this match.</param>
        /// <remarks>The element's <see cref="object.Equals(object?)"/> method will be used for matching.</remarks>
        public ElementMatch(TElement element) : this($"{element}", e => Equals(e, element)) { }

        /// <inheritdoc/>
        public bool Matches(TElement element)
            => this.Closure(element);
    }
}
