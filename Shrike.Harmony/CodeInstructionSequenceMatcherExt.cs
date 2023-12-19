using HarmonyLib;
using System.Reflection.Emit;

namespace Nanoray.Shrike.Harmony;

/// <summary>
/// A static class hosting additional extensions for <see cref="ISequenceMatcher{TElement}"/> with <see cref="CodeInstruction"/> elements.
/// </summary>
public static class CodeInstructionSequenceMatcherExt
{
    /// <summary>
    /// Moves to an instruction with the given label.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The current matcher.</param>
    /// <param name="label">The label to move to.</param>
    /// <returns>A new pointer matcher, pointing at instruction with the given label.</returns>
    public static SequencePointerMatcher<TElement> PointerMatcher<TElement>(this ISequenceMatcher<TElement> self, Label label)
        where TElement : CodeInstruction
    {
        var instructions = self.AllElements();
        for (int i = 0; i < instructions.Count; i++)
            if (instructions[i].labels.Contains(label))
                return self.PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence).Advance(i);
        throw new SequenceMatcherException($"Label {label} not found.");
    }
}
