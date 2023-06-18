using System;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a sequence pointer matcher handling elements of a given type.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    public interface ISequencePointerMatcher<TElement> : ISequenceMatcher<TElement>
    {
        /// <summary>
        /// The index of the underlying list of elements this pointer matcher is pointing at.
        /// </summary>
        int Index();

        /// <summary>
        /// The element this pointer matcher is pointing at.
        /// </summary>
        TElement Element()
            => this.AllElements()[this.Index()];
    }

    /// <summary>
    /// Represents a sequence pointer matcher handling elements of a given type, with specified pointer and block matcher implementations.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
    /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
    public interface ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher> : ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher>, ISequencePointerMatcher<TElement>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
    {
        /// <summary>
        /// Stores the element this pointer matcher is pointing at.
        /// </summary>
        /// <param name="element">The currently pointed at element.</param>
        /// <returns>An unchanged pointer matcher.</returns>
        TPointerMatcher Element(out TElement element)
        {
            element = this.Element();
            return this.MakePointerMatcher(this.Index());
        }

        /// <summary>
        /// Transforms the current element this pointer matcher is pointing at and returns the result of that transformation.
        /// </summary>
        /// <typeparam name="TResult">The result type of the transformation.</typeparam>
        /// <param name="element">The transformed element.</param>
        /// <param name="transformation">The transformation to run.</param>
        /// <returns>An unchanged pointer matcher.</returns>
        TPointerMatcher SelectElement<TResult>(out TResult element, Func<TElement, TResult> transformation)
        {
            element = transformation(this.Element());
            return this.MakePointerMatcher(this.Index());
        }

        /// <summary>
        /// Creates a pointer matcher pointing at another element offset from the current one.
        /// </summary>
        /// <param name="offset">The index offset from the current index.</param>
        TPointerMatcher Advance(int offset = 1)
            => this.MakePointerMatcher(this.Index() + offset);

        /// <summary>
        /// Performs a replace operation on the element matched by this pointer matcher.
        /// </summary>
        /// <param name="element">The new element to replace the element currently matched by this pointer matcher.</param>
        /// <returns>A new pointer matcher representing the state after replacing the matched element with the new element.</returns>
        TPointerMatcher Replace(TElement element);

        /// <summary>
        /// Performs a remove operation on the element matched by this pointer matcher.
        /// </summary>
        /// <param name="postRemovalPosition">The position the resulting pointer matcher should point at.</param>
        /// <returns>A new pointer matcher representing the state after removing the matched element.</returns>
        TPointerMatcher Remove(SequenceMatcherPastBoundsDirection postRemovalPosition);

        /// <summary>
        /// Performs a provided set of operations on the element matched by this pointer matcher.
        /// </summary>
        /// <param name="closure">The set of operations to perform.</param>
        /// <returns>A new pointer matcher representing the state after performing the provided set of operations on the element matched by this pointer matcher.</returns>
        TPointerMatcher Do(Func<TElement, TElement> closure)
            => this.Replace(closure(this.Element()));
    }

    /// <summary>
    /// A static class hosting "default implementation" extensions for <see cref="ISequencePointerMatcher{TElement}"/> and <see cref="ISequencePointerMatcher{TElement, TPointerMatcher, TBlockMatcher}"/> types.
    /// </summary>
    public static class ISequencePointerMatcherDefaultImplementations
    {
        /// <summary>
        /// The element this pointer matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <param name="self">The current matcher.</param>
        public static TElement Element<TElement>(this ISequencePointerMatcher<TElement> self)
            => self.Element();

        /// <summary>
        /// Stores the element this pointer matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="element">The currently pointed at element.</param>
        /// <returns>An unchanged pointer matcher.</returns>
        public static TPointerMatcher Element<TElement, TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher> self, out TElement element)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Element(out element);

        /// <summary>
        /// Stores the element this pointer matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <typeparam name="TResult">The result type of the transformation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="element">The transformed element.</param>
        /// <param name="transformation">The transformation to run.</param>
        /// <returns>An unchanged pointer matcher.</returns>
        public static TPointerMatcher SelectElement<TElement, TPointerMatcher, TBlockMatcher, TResult>(this ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher> self, out TResult element, Func<TElement, TResult> transformation)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.SelectElement(out element, transformation);

        /// <summary>
        /// Creates a pointer matcher pointing at another element offset from the current one.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="offset">The index offset from the current index.</param>
        public static TPointerMatcher Advance<TElement, TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int offset = 1)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Advance(offset);

        /// <summary>
        /// Performs a provided set of operations on the element matched by this pointer matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="closure">The set of operations to perform.</param>
        /// <returns>A new pointer matcher representing the state after performing the provided set of operations on the element matched by this pointer matcher.</returns>
        public static TPointerMatcher Do<TElement, TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher> self, Func<TElement, TElement> closure)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Do(closure);
    }

    /// <summary>
    /// A static class hosting additional extensions for <see cref="ISequencePointerMatcher{TElement}"/> and <see cref="ISequencePointerMatcher{TElement, TPointerMatcher, TBlockMatcher}"/> types.
    /// </summary>
    public static class ISequencePointerMatcherExt
    {
        /// <summary>
        /// Performs a provided set of operations a given amount of times on the element matched by this pointer matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="times">The number of times this set of operations should be performed.</param>
        /// <param name="closure">The set of operations to perform.</param>
        /// <returns>A new pointer matcher representing the state after performing the provided set of operations on the element matched by this pointer matcher.</returns>
        public static TPointerMatcher Repeat<TElement, TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int times, Func<TPointerMatcher, TPointerMatcher> closure)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        {
            TPointerMatcher matcher = self.PointerMatcher(SequenceMatcherRelativeElement.First);
            for (int i = 0; i < times; i++)
                matcher = closure(matcher);
            return matcher;
        }
    }
}
