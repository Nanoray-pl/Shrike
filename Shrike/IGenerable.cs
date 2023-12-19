namespace Nanoray.Shrike;

#if NET7_0_OR_GREATER
/// <summary>
/// Represents a type which has the ability to generate its own values.
/// </summary>
/// <typeparam name="T">The type of elements.</typeparam>
public interface IGenerable<T> where T : IGenerable<T>
{
    /// <summary>
    /// Generates a value.
    /// </summary>
    static abstract T Generate();
}
#endif
