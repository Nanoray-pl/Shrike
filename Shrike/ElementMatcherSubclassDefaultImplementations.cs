using System;

namespace Nanoray.Shrike
{
    /// <summary>
    /// A static class hosting default implementations for <see cref="ISequenceMatcher{TElement}"/> subclasses.
    /// </summary>
    public static class ElementMatcherSubclassDefaultImplementations
    {
        /// <summary>
        /// Creates a pointer matcher pointing at an element relative to the bounds of this sequence matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="element">The relative element.</param>
        public static TPointerMatcher PointerMatcher<TElement, TPointerMatcher, TBlockMatcher>(ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherRelativeElement element)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => element switch
            {
                SequenceMatcherRelativeElement.FirstInWholeSequence => self.MakePointerMatcher(0),
                SequenceMatcherRelativeElement.BeforeFirst => self.MakePointerMatcher(self.Index() - 1),
                SequenceMatcherRelativeElement.First => self.MakePointerMatcher(self.Index()),
                SequenceMatcherRelativeElement.Last => self.MakePointerMatcher(self.Index()),
                SequenceMatcherRelativeElement.AfterLast => self.MakePointerMatcher(self.Index() + 1),
                SequenceMatcherRelativeElement.LastInWholeSequence => self.MakePointerMatcher(self.AllElements().Count - 1),
                _ => throw new ArgumentException($"{nameof(SequenceMatcherRelativeElement)} has an invalid value."),
            };

        /// <summary>
        /// Creates a pointer matcher pointing at an element relative to the bounds of this sequence matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="element">The relative element.</param>
        public static TPointerMatcher PointerMatcher<TElement, TPointerMatcher, TBlockMatcher>(ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherRelativeElement element)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => element switch
            {
                SequenceMatcherRelativeElement.FirstInWholeSequence => self.MakePointerMatcher(0),
                SequenceMatcherRelativeElement.BeforeFirst => self.MakePointerMatcher(self.StartIndex() - 1),
                SequenceMatcherRelativeElement.First => self.MakePointerMatcher(self.StartIndex()),
                SequenceMatcherRelativeElement.Last => self.MakePointerMatcher(self.EndIndex() - 1),
                SequenceMatcherRelativeElement.AfterLast => self.MakePointerMatcher(self.EndIndex()),
                SequenceMatcherRelativeElement.LastInWholeSequence => self.MakePointerMatcher(self.AllElements().Count - 1),
                _ => throw new ArgumentException($"{nameof(SequenceMatcherRelativeElement)} has an invalid value."),
            };

        /// <summary>
        /// Creates a block matcher encompassing elements in bounds relative to the bounds of this sequence matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="bounds">The relative bounds.</param>
        public static TBlockMatcher BlockMatcher<TElement, TPointerMatcher, TBlockMatcher>(ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherRelativeBounds bounds)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => bounds switch
            {
                SequenceMatcherRelativeBounds.Before => self.MakeBlockMatcher(0, self.Index()),
                SequenceMatcherRelativeBounds.BeforeOrEnclosed => self.MakeBlockMatcher(0, self.Index() + 1),
                SequenceMatcherRelativeBounds.Enclosed => self.MakeBlockMatcher(self.Index(), 1),
                SequenceMatcherRelativeBounds.AfterOrEnclosed => self.MakeBlockMatcher(self.Index(), self.AllElements().Count - self.Index()),
                SequenceMatcherRelativeBounds.After => self.MakeBlockMatcher(self.Index() + 1, self.AllElements().Count - self.Index() - 1),
                SequenceMatcherRelativeBounds.WholeSequence => self.MakeBlockMatcher(0, self.AllElements().Count),
                _ => throw new ArgumentException($"{nameof(SequenceMatcherRelativeBounds)} has an invalid value."),
            };

        /// <summary>
        /// Creates a block matcher encompassing elements in bounds relative to the bounds of this sequence matcher.
        /// </summary>
        /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="bounds">The relative bounds.</param>
        public static TBlockMatcher BlockMatcher<TElement, TPointerMatcher, TBlockMatcher>(ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher> self, SequenceMatcherRelativeBounds bounds)
            where TPointerMatcher : ISequencePointerMatcher<TElement, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<TElement, TPointerMatcher, TBlockMatcher>
            => bounds switch
            {
                SequenceMatcherRelativeBounds.Before => self.MakeBlockMatcher(0, self.StartIndex()),
                SequenceMatcherRelativeBounds.BeforeOrEnclosed => self.MakeBlockMatcher(0, self.EndIndex()),
                SequenceMatcherRelativeBounds.Enclosed => self.MakeBlockMatcher(self.StartIndex(), self.Length()),
                SequenceMatcherRelativeBounds.AfterOrEnclosed => self.MakeBlockMatcher(self.StartIndex(), self.AllElements().Count - self.StartIndex()),
                SequenceMatcherRelativeBounds.After => self.MakeBlockMatcher(self.EndIndex(), self.AllElements().Count - self.EndIndex()),
                SequenceMatcherRelativeBounds.WholeSequence => self.MakeBlockMatcher(0, self.AllElements().Count),
                _ => throw new ArgumentException($"{nameof(SequenceMatcherRelativeBounds)} has an invalid value."),
            };
    }
}
