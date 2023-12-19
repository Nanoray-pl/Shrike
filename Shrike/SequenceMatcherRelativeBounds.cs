namespace Nanoray.Shrike;

/// <summary>
/// Represents a set of bounds relative to a sequence matcher's bounds.
/// </summary>
public enum SequenceMatcherRelativeBounds
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
    WholeSequence,
}
