namespace Nanoray.Shrike;

/// <summary>
/// Represents bounds to encompass before/after the pointed elements.
/// </summary>
public enum SequenceMatcherEncompassDirection
{
    /// <summary>
    /// Elements before the elements pointed at.
    /// </summary>
    Before,

    /// <summary>
    /// Elements after the elements pointed at.
    /// </summary>
    After,

    /// <summary>
    /// Elements both before and after the elements pointed at.
    /// </summary>
    Both,
}
