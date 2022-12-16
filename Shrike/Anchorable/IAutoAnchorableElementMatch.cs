namespace Nanoray.Shrike
{
    public interface IAutoAnchorableElementMatch<TElement, TAnchor> : IElementMatch<TElement>
    {
        TAnchor? Anchor { get; }
    }
}
