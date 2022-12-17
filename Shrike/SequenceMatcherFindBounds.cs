namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a set of bounds that could be searched against by a sequence matcher.
    /// </summary>
    public enum SequenceMatcherFindBounds
    {
        /// <summary>
        /// The elements before the elements currently pointed at.
        /// </summary>
        Before,

        /// <summary>
        /// The elements before and including the elements currently pointed at.
        /// </summary>
        BeforeOrEnclosed,

        /// <summary>
        /// The elements currently pointed at.
        /// </summary>
        Enclosed,

        /// <summary>
        /// The elements including and after the elements currently pointed at.
        /// </summary>
        AfterOrEnclosed,

        /// <summary>
        /// The elements after the elements currently pointed at.
        /// </summary>
        After,

        /// <summary>
        /// All the underlying elements.
        /// </summary>
        WholeSequence
    }
}
