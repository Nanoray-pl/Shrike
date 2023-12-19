namespace Nanoray.Shrike;

/// <summary>
/// Represents a set of pointers relative to a sequence matcher's bounds.
/// </summary>
public enum SequenceMatcherRelativeElement
{
    /// <summary>
    /// The very first element of the whole sequence.
    /// </summary>
    FirstInWholeSequence,

    /// <summary>
    /// The element before the first element matched by a sequence matcher.
    /// </summary>
    BeforeFirst,

    /// <summary>
    /// The first element matched by a sequence matcher.
    /// </summary>
    First,

    /// <summary>
    /// The last element matched by a sequence matcher.
    /// </summary>
    Last,

    /// <summary>
    /// The element after the last element matched by a sequence matcher.
    /// </summary>
    AfterLast,

    /// <summary>
    /// The very last element of the whole sequence.
    /// </summary>
    LastInWholeSequence,
}
