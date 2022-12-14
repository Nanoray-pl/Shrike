using HarmonyLib;
using System.Reflection.Emit;

namespace Nanoray.Shrike.Harmony
{
    public static class CodeInstructionSequenceMatcherExt
    {
        public static TPointerMatcher CreateLabel<TPointerMatcher, TBlockMatcher>(this ISequenceMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, Label label)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            for (int i = 0; i < self.AllElements.Count; i++)
            {
                if (self.AllElements[i].labels.Contains(label))
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
