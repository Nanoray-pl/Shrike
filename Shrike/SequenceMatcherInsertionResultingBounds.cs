namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents bounds after an insertion operation.
    /// </summary>
    public enum SequenceMatcherInsertionResultingBounds
    {
        /// <summary>
        /// The resulting bounds will not include inserted elements (the bounds will not change).
        /// </summary>
        ExcludingInsertion,

        /// <summary>
        /// The resulting bounds will only include inserted elements.
        /// </summary>
        JustInsertion,

        /// <summary>
        /// The resulting bounds will include both the inserted elements and the previous bounds.
        /// </summary>
        IncludingInsertion
    }
}
