using System;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a simple single auto-anchorable sequence element match.
    /// An auto-anchorable match allows automatically creating an anchor for it whenever a sequence matcher finds it.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this object can match.</typeparam>
    /// <typeparam name="TAnchor">The anchor type.</typeparam>
    public record AutoAnchorableElementMatch<TElement, TAnchor> : IAutoAnchorableElementMatch<TElement, TAnchor>
        where TAnchor : notnull
    {
        /// <inheritdoc/>
        public string Description { get; init; }

        /// <inheritdoc/>
        public TAnchor? Anchor { get; init; }

        private Func<TElement, bool> Closure { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoAnchorableElementMatch{TElement, TAnchor}"/> class.
        /// </summary>
        /// <param name="description">A description of the match, used mostly for debugging purposes.</param>
        /// <param name="anchor">The anchor that should be automatically created when this match is found.</param>
        /// <param name="closure">The function that tests whether a given element matches this match.</param>
        public AutoAnchorableElementMatch(string description, TAnchor? anchor, Func<TElement, bool> closure)
        {
            this.Description = description;
            this.Anchor = anchor;
            this.Closure = closure;
        }

        /// <inheritdoc/>
        public bool Matches(TElement element)
            => this.Closure(element);
    }
}
