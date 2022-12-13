namespace Nanoray.Shrike
{
    public interface IElementMatch<TElement>
    {
        string Description { get; }

        bool Matches(TElement element);
    }
}
