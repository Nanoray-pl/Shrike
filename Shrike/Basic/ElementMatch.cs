using System;

namespace Nanoray.Shrike
{
    public record ElementMatch<TElement> : IElementMatch<TElement>
    {
        public string Description { get; init; }
        private Func<TElement, bool> Closure { get; init; }

        public ElementMatch(string description, Func<TElement, bool> closure)
        {
            this.Description = description;
            this.Closure = closure;
        }

        public bool Matches(TElement element)
            => this.Closure(element);
    }
}
