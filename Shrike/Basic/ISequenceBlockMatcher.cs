using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a sequence block matcher handling elements of a given type.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    public interface ISequenceBlockMatcher<TElement> : ISequenceMatcher<TElement>
    {
        /// <summary>
        /// The (inclusive) start index of the underlying list of elements this block matcher is pointing at.
        /// </summary>
        int StartIndex();

        /// <summary>
        /// The (exclusive) end index of the underlying list of elements this block matcher is pointing at.
        /// </summary>
        int EndIndex();

        /// <summary>
        /// The length of the range this block matcher is pointing at.
        /// </summary>
        int Length();

        /// <summary>
        /// The elements this block matcher is pointing at.
        /// </summary>
        IReadOnlyList<TElement> Elements()
            => this.AllElements().Skip(this.StartIndex()).Take(this.Length()).ToList();
    }

    /// <summary>
    /// Represents a sequence block matcher handling elements of a given type, with specified pointer and block matcher implementations.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
    /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
    public interface ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> : ISequenceMatcher<TElement, TPointerMatcher, TBlockMatcher>, ISequenceBlockMatcher<TElement>
        where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
        where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
    {
        /// <summary>
        /// Stores the elements this block matcher is pointing at.
        /// </summary>
        /// <param name="elements">The currently pointed at elements.</param>
        /// <returns>An unchanged block matcher.</returns>
        TBlockMatcher Elements(out IReadOnlyList<TElement> elements)
        {
            elements = this.Elements();
            return this.MakeBlockMatcher(this.StartIndex(), this.Length());
        }

        /// <summary>
        /// Creates a block matcher pointing at the elements just before the first element this block matcher is pointing at.
        /// </summary>
        /// <param name="length">How many elements before the first element this block matcher is pointing at to look at.</param>
        TBlockMatcher BlockMatcherBeforeFirst(int length = 0)
            => this.MakeBlockMatcher(this.StartIndex() - length, length);

        /// <summary>
        /// Creates a block matcher pointing at the elements just after the last element this block matcher is pointing at.
        /// </summary>
        /// <param name="length">How many elements after the last element this block matcher is pointing at to look at.</param>
        TBlockMatcher BlockMatcherAfterLast(int length = 0)
            => this.MakeBlockMatcher(this.EndIndex(), length);

        /// <summary>
        /// Performs a provided set of operations on the elements matched by this block matcher.
        /// </summary>
        /// <param name="closure">The set of operations to perform.</param>
        /// <returns>A new block matcher representing the state after performing the provided set of operations on the elements matched by this block matcher.</returns>
        TBlockMatcher Do(Func<TBlockMatcher, TBlockMatcher> closure);

        /// <summary>
        /// Performs a provided set of operations on the elements matched by this block matcher.
        /// </summary>
        /// <param name="closure">The set of operations to perform.</param>
        /// <returns>A new block matcher representing the state after performing the provided set of operations on the elements matched by this block matcher.</returns>
        TBlockMatcher Do(Func<TBlockMatcher, TPointerMatcher> closure)
            => this.Do(matcher => closure(matcher).BlockMatcher());
    }

    /// <summary>
    /// A static class hosting "default implementation" extensions for <see cref="ISequenceBlockMatcher{TElement}"/> and <see cref="ISequenceBlockMatcher{TElement, TPointerMatcher, TBlockMatcher}"/> types.
    /// </summary>
    public static class ISequenceBlockMatcherDefaultImplementations
    {
        /// <summary>
        /// The elements this block matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <param name="self">The current matcher.</param>
        public static IReadOnlyList<TElement> Elements<TElement>(this ISequenceBlockMatcher<TElement> self)
            => self.Elements();

        /// <summary>
        /// Stores the elements this block matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="elements">The currently pointed at elements.</param>
        /// <returns>An unchanged block matcher.</returns>
        public static TBlockMatcher Elements<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, out IReadOnlyList<TElement> elements)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Elements(out elements);

        /// <summary>
        /// Creates a block matcher pointing at the elements just before the first element this block matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="length">How many elements before the first element this block matcher is pointing at to look at.</param>
        public static TBlockMatcher BlockMatcherBeforeFirst<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int length = 0)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.BlockMatcherBeforeFirst(length);

        /// <summary>
        /// Creates a block matcher pointing at the elements just after the last element this block matcher is pointing at.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="length">How many elements before the first element this block matcher is pointing at to look at.</param>
        public static TBlockMatcher BlockMatcherAfterLast<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int length = 0)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.BlockMatcherAfterLast(length);

        /// <summary>
        /// Performs a provided set of operations on the elements matched by this block matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="closure">The set of operations to perform.</param>
        /// <returns>A new block matcher representing the state after performing the provided set of operations on the elements matched by this block matcher.</returns>
        public static TBlockMatcher Do<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, Func<TBlockMatcher, TPointerMatcher> closure)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => self.Do(closure);
    }

    /// <summary>
    /// A static class hosting additional extensions for <see cref="ISequenceBlockMatcher{TElement}"/> and <see cref="ISequenceBlockMatcher{TElement, TPointerMatcher, TBlockMatcher}"/> types.
    /// </summary>
    public static class ISequenceBlockMatcherExt
    {
        /// <summary>
        /// Performs a provided set of operations a given amount of times on the elements matched by this block matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="times">The number of times this set of operations should be performed.</param>
        /// <param name="closure">The set of operations to perform.</param>
        /// <returns>A new block matcher representing the state after performing the provided set of operations on the elements matched by this block matcher.</returns>
        public static TBlockMatcher Repeat<TElement, TPointerMatcher, TBlockMatcher>(this ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, int times, Func<TBlockMatcher, TBlockMatcher> closure)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
        {
            TBlockMatcher matcher = self.BlockMatcher();
            for (int i = 0; i < times; i++)
                matcher = closure(matcher);
            return matcher;
        }
    }
}
