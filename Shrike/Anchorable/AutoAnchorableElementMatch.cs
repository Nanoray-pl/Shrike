using System;

namespace Nanoray.Shrike
{
    public record AutoAnchorableElementMatch<TElement, TAnchor> : IAutoAnchorableElementMatch<TElement, TAnchor>
        where TAnchor : notnull
    {
        /// <inheritdoc/>
        public string Description { get; init; }

        /// <inheritdoc/>
        public TAnchor? Anchor { get; init; }

        private Func<TElement, bool> Closure { get; init; }

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

    public static class AutoAnchorableElementMatchExt
    {
        public static IAutoAnchorableElementMatch<TElement, TAnchor> WithAutoAnchor<TElement, TAnchor>(this IElementMatch<TElement> match, TAnchor anchor)
            where TAnchor : notnull
            => new AutoAnchorableElementMatch<TElement, TAnchor>(match.Description, anchor, match.Matches);

        public static IAutoAnchorableElementMatch<TElement, TAnchor> WithAutoAnchor<TElement, TAnchor>(this IElementMatch<TElement> match, out TAnchor anchor, Func<TAnchor> generator)
            where TAnchor : notnull
        {
            anchor = generator();
            return match.WithAutoAnchor(anchor);
        }

#if NET7_0_OR_GREATER
        public static IAutoAnchorableElementMatch<TElement, TAnchor> WithAutoAnchor<TElement, TAnchor>(this IElementMatch<TElement> match, out TAnchor anchor)
            where TAnchor : notnull, IGenerable<TAnchor>
        {
            anchor = TAnchor.Generate();
            return match.WithAutoAnchor(anchor);
        }
#endif
    }

    public static class AutoAnchorableElementMatchSpecificTypeGenerators
    {
        public static IAutoAnchorableElementMatch<TElement, Guid> WithAutoAnchor<TElement>(this IElementMatch<TElement> match, out Guid anchor)
            => match.WithAutoAnchor(out anchor, () => Guid.NewGuid());
    }
}
