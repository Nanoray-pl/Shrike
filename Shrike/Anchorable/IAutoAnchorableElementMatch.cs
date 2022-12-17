using System;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a single auto-anchorable sequence element match.
    /// An auto-anchorable match allows automatically creating an anchor for it whenever a sequence matcher finds it.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this object can match.</typeparam>
    /// <typeparam name="TAnchor">The anchor type.</typeparam>
    public interface IAutoAnchorableElementMatch<TElement, TAnchor> : IElementMatch<TElement>
    {
        /// <summary>
        /// The anchor that should be automatically created when this match is found.
        /// </summary>
        TAnchor? Anchor { get; }
    }

    /// <summary>
    /// A static class hosting additional extensions for the <see cref="IElementMatch{TElement}"/> type, relating to the functionality of the <see cref="IAutoAnchorableElementMatch{TElement, TAnchor}"/> type.
    /// </summary>
    public static class IAutoAnchorableElementMatchExt
    {
        /// <summary>
        /// Creates a new auto-anchorable sequence element match out of an existing match object.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this object can match.</typeparam>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="match">The original match object.</param>
        /// <param name="anchor">The anchor that should be automatically created when this match is found.</param>
        public static IAutoAnchorableElementMatch<TElement, TAnchor> WithAutoAnchor<TElement, TAnchor>(this IElementMatch<TElement> match, TAnchor anchor)
            where TAnchor : notnull
            => new AutoAnchorableElementMatch<TElement, TAnchor>(match.Description, anchor, match.Matches);

        /// <summary>
        /// Creates a new auto-anchorable sequence element match out of an existing match object.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this object can match.</typeparam>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="match">The original match object.</param>
        /// <param name="anchor">The auto-generated anchor that will be automatically created when this match is found.</param>
        /// <param name="generator">A function that will generate the anchor values.</param>
        public static IAutoAnchorableElementMatch<TElement, TAnchor> WithAutoAnchor<TElement, TAnchor>(this IElementMatch<TElement> match, out TAnchor anchor, Func<TAnchor> generator)
            where TAnchor : notnull
        {
            anchor = generator();
            return match.WithAutoAnchor(anchor);
        }

#if NET7_0_OR_GREATER
        /// <summary>
        /// Creates a new auto-anchorable sequence element match out of an existing match object.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this object can match.</typeparam>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="match">The original match object.</param>
        /// <param name="anchor">The auto-generated anchor that will be automatically created when this match is found.</param>
        public static IAutoAnchorableElementMatch<TElement, TAnchor> WithAutoAnchor<TElement, TAnchor>(this IElementMatch<TElement> match, out TAnchor anchor)
            where TAnchor : notnull, IGenerable<TAnchor>
        {
            anchor = TAnchor.Generate();
            return match.WithAutoAnchor(anchor);
        }
#endif
    }

    /// <summary>
    /// A static class hosting additional extensions for the <see cref="ISequencePointerMatcher{TElement}"/> type, relating to the functionality of the <see cref="IAutoAnchorableElementMatch{TElement, TAnchor}"/> type, for pre-specified anchor types.
    /// </summary>
    public static class IAutoAnchorableElementMatchSpecificTypeGenerators
    {
        /// <summary>
        /// Creates a new auto-anchorable sequence element match out of an existing match object.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this object can match.</typeparam>
        /// <param name="match">The original match object.</param>
        /// <param name="anchor">The auto-generated anchor that will be automatically created when this match is found.</param>
        public static IAutoAnchorableElementMatch<TElement, Guid> WithAutoAnchor<TElement>(this IElementMatch<TElement> match, out Guid anchor)
            => match.WithAutoAnchor(out anchor, () => Guid.NewGuid());
    }
}
