namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a single sequence element match.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this object can match.</typeparam>
    public interface IElementMatch<TElement>
    {
        /// <summary>
        /// A description of the match, used mostly for debugging purposes.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Tests whether a given element matches this match.
        /// </summary>
        /// <param name="element">The element to test against.</param>
        /// <returns>Whether the given element matches this match.</returns>
        bool Matches(TElement element);
    }
}
