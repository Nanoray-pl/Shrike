using HarmonyLib;
using System.Reflection.Emit;

namespace Nanoray.Shrike.Harmony
{
    /// <summary>
    /// A static class hosting additional extensions for <see cref="ISequenceMatcher{TElement, TPointerMatcher, TBlockMatcher}"/> with <see cref="CodeInstruction"/> elements.
    /// </summary>
    public static class CodeInstructionSequenceMatcherExt
    {
        /// <summary>
        /// Moves to an instruction with the given label.
        /// </summary>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="label">The label to move to.</param>
        /// <returns>A new pointer matcher, pointing at instruction with the given label.</returns>
        public static TPointerMatcher MoveToLabel<TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, Label label)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            for (int i = 0; i < self.AllElements().Count; i++)
            {
                if (self.AllElements()[i].labels.Contains(label))
#if NET7_0_OR_GREATER
                    return TPointerMatcher.MakePointerMatcher(i);
#else
                    return self.MakePointerMatcher(i);
#endif
            }
            throw new SequenceMatcherException($"Label {label} not found.");
        }
    }
}
