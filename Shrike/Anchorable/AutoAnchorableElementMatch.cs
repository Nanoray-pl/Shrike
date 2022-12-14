using System;

namespace Nanoray.Shrike
{
    public record AutoAnchorableElementMatch<TElement, TAnchor> : IAutoAnchorableElementMatch<TElement, TAnchor>
        where TAnchor : notnull
    {
        public string Description { get; init; }
        public TAnchor? Anchor { get; init; }
        private Func<TElement, bool> Closure { get; init; }

        public AutoAnchorableElementMatch(string description, TAnchor? anchor, Func<TElement, bool> closure)
        {
            this.Description = description;
            this.Anchor = anchor;
            this.Closure = closure;
        }

        public bool Matches(TElement element)
            => this.Closure(element);
    }

    public static class IElementMatchExt
    {
        public static IAutoAnchorableElementMatch<TElement, TAnchor> WithAutoAnchor<TElement, TAnchor>(this IElementMatch<TElement> match, TAnchor anchor)
            where TAnchor : notnull
            => new AutoAnchorableElementMatch<TElement, TAnchor>(match.Description, anchor, match.Matches);
    }
}
