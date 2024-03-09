using System;

namespace Nanoray.Shrike;

/// <summary>
/// A static class hosting additional <see cref="ElementMatch{TElement}"/> extensions.
/// </summary>
public static class ElementMatchClassExt
{
    /// <summary>
    /// Stores the found instruction.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="elementReference">A reference where the element will be stored.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will set the value of the given reference.</returns>
    public static ElementMatch<TElement> Element<TElement>(this ElementMatch<TElement> self, out ObjectRef<TElement> elementReference) where TElement : class
    {
        ObjectRef<TElement> reference = new(null!);
        elementReference = reference;
        return self.WithDelegate((matcher, index, element) =>
        {
            reference.Value = matcher.MakePointerMatcher(index).Element();
            return matcher;
        });
    }

    /// <summary>
    /// Transforms the found element and stores the result of that transformation.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="elementReference">A reference where the transformed element will be stored.</param>
    /// <param name="transformation">The transformation to run.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will set the value of the given reference.</returns>
    public static ElementMatch<TElement> SelectElement<TElement, TResult>(this ElementMatch<TElement> self, out ObjectRef<TResult> elementReference, Func<TElement, TResult> transformation) where TResult : class
    {
        ObjectRef<TResult> reference = new(null!);
        elementReference = reference;
        return self.WithDelegate((matcher, index, element) =>
        {
            reference.Value = transformation(matcher.MakePointerMatcher(index).Element());
            return matcher;
        });
    }
}

/// <summary>
/// A static class hosting additional <see cref="ElementMatch{TElement}"/> extensions.
/// </summary>
public static class ElementMatchStructExt
{
    /// <summary>
    /// Stores the found instruction.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="elementReference">A reference where the element will be stored.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will set the value of the given reference.</returns>
    public static ElementMatch<TElement> Element<TElement>(this ElementMatch<TElement> self, out StructRef<TElement> elementReference) where TElement : struct
    {
        StructRef<TElement> reference = new(default);
        elementReference = reference;
        return self.WithDelegate((matcher, index, element) =>
        {
            reference.Value = matcher.MakePointerMatcher(index).Element();
            return matcher;
        });
    }

    /// <summary>
    /// Transforms the found element and stores the result of that transformation.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="elementReference">A reference where the transformed element will be stored.</param>
    /// <param name="transformation">The transformation to run.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will set the value of the given reference.</returns>
    public static ElementMatch<TElement> SelectElement<TElement, TResult>(this ElementMatch<TElement> self, out StructRef<TResult> elementReference, Func<TElement, TResult> transformation) where TResult : struct
    {
        StructRef<TResult> reference = new(default);
        elementReference = reference;
        return self.WithDelegate((matcher, index, element) =>
        {
            reference.Value = transformation(matcher.MakePointerMatcher(index).Element());
            return matcher;
        });
    }
}
