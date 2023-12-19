using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Nanoray.Shrike.Harmony;

/// <summary>
/// A static class hosting additional <see cref="ElementMatch{TElement}"/> extensions for <see cref="CodeInstruction"/> elements.
/// </summary>
public static class ElementMatchExt
{
    /// <summary>
    /// Tries to create an <c>ldloc</c> instruction referencing the same local variable as the found instruction.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="instructionReference">A reference where the created instruction will be stored.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will anchor this element.</returns>
    public static ElementMatch<CodeInstruction> TryCreateLdlocInstruction(this ElementMatch<CodeInstruction> self, out NullableObjectRef<CodeInstruction> instructionReference)
    {
        NullableObjectRef<CodeInstruction> reference = new();
        instructionReference = reference;
        return self.WithDelegate((matcher, index, element) =>
        {
            matcher.PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence).Advance(index).TryCreateLdlocInstruction(out var instruction);
            reference.Value = instruction;
            return matcher;
        });
    }

    /// <summary>
    /// Creates an <c>ldloc</c> instruction referencing the same local variable as the found instruction.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="instructionReference">A reference where the created instruction will be stored.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will anchor this element.</returns>
    public static ElementMatch<CodeInstruction> CreateLdlocInstruction(this ElementMatch<CodeInstruction> self, out ObjectRef<CodeInstruction> instructionReference)
    {
        ObjectRef<CodeInstruction> reference = new(null!);
        instructionReference = reference;
        return self.WithDelegate((matcher, index, element) =>
        {
            matcher.PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence).Advance(index).CreateLdlocInstruction(out var instruction);
            reference.Value = instruction;
            return matcher;
        });
    }

    /// <summary>
    /// Tries to create an <c>stloc</c> instruction referencing the same local variable as the found instruction.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="instructionReference">A reference where the created instruction will be stored.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will anchor this element.</returns>
    public static ElementMatch<CodeInstruction> TryCreateStlocInstruction(this ElementMatch<CodeInstruction> self, out NullableObjectRef<CodeInstruction> instructionReference)
    {
        NullableObjectRef<CodeInstruction> reference = new();
        instructionReference = reference;
        return self.WithDelegate((matcher, index, element) =>
        {
            matcher.PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence).Advance(index).TryCreateStlocInstruction(out var instruction);
            reference.Value = instruction;
            return matcher;
        });
    }

    /// <summary>
    /// Creates an <c>stloc</c> instruction referencing the same local variable as the found instruction.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="instructionReference">A reference where the created instruction will be stored.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will anchor this element.</returns>
    public static ElementMatch<CodeInstruction> CreateStlocInstruction(this ElementMatch<CodeInstruction> self, out ObjectRef<CodeInstruction> instructionReference)
    {
        ObjectRef<CodeInstruction> reference = new(null!);
        instructionReference = reference;
        return self.WithDelegate((matcher, index, element) =>
        {
            matcher.PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence).Advance(index).CreateStlocInstruction(out var instruction);
            reference.Value = instruction;
            return matcher;
        });
    }

    /// <summary>
    /// Tries to create an <c>ldloc.a</c> instruction referencing the same local variable as the found instruction.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="instructionReference">A reference where the created instruction will be stored.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will anchor this element.</returns>
    public static ElementMatch<CodeInstruction> TryCreateLdlocaInstruction(this ElementMatch<CodeInstruction> self, out NullableObjectRef<CodeInstruction> instructionReference)
    {
        NullableObjectRef<CodeInstruction> reference = new();
        instructionReference = reference;
        return self.WithDelegate((matcher, index, element) =>
        {
            matcher.PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence).Advance(index).TryCreateLdlocaInstruction(out var instruction);
            reference.Value = instruction;
            return matcher;
        });
    }

    /// <summary>
    /// Creates an <c>ldloc.a</c> instruction referencing the same local variable as the found instruction.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="instructionReference">A reference where the created instruction will be stored.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will anchor this element.</returns>
    public static ElementMatch<CodeInstruction> CreateLdlocaInstruction(this ElementMatch<CodeInstruction> self, out ObjectRef<CodeInstruction> instructionReference)
    {
        ObjectRef<CodeInstruction> reference = new(null!);
        instructionReference = reference;
        return self.WithDelegate((matcher, index, element) =>
        {
            matcher.PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence).Advance(index).CreateLdlocaInstruction(out var instruction);
            reference.Value = instruction;
            return matcher;
        });
    }

    /// <summary>
    /// Creates a new label and adds it to the found instruction.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="il">The <see cref="ILGenerator"/> to use.</param>
    /// <param name="label">The newly created label.</param>
    /// <returns>A new pointer matcher, with a modified instruction without its labels.</returns>
    public static ElementMatch<CodeInstruction> CreateLabel(this ElementMatch<CodeInstruction> self, ILGenerator il, out Label label)
    {
        var createdLabel = il.DefineLabel();
        label = createdLabel;
        return self.WithDelegate((matcher, index, element) =>
        {
            int startIndex = matcher.StartIndex();
            int length = matcher.Length();
            return matcher
                .PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence)
                .Advance(index)
                .AddLabel(createdLabel)
                .PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence)
                .Advance(startIndex)
                .Encompass(SequenceMatcherEncompassDirection.After, length - 1);
        });
    }

    /// <summary>
    /// Extracts labels from the found instruction, removing them from it.
    /// </summary>
    /// <param name="self">The match.</param>
    /// <param name="labels">A reference where the created instruction will be stored.</param>
    /// <returns>A new pointer matcher, with a modified instruction without its labels.</returns>
    public static ElementMatch<CodeInstruction> ExtractLabels(this ElementMatch<CodeInstruction> self, out IReadOnlyList<Label> labels)
    {
        List<Label> labelsReference = new();
        labels = labelsReference;
        return self.WithDelegate((matcher, index, element) =>
        {
            matcher.PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence).Advance(index).ExtractLabels(out var extractedLabels);
            labelsReference.AddRange(extractedLabels);
            return matcher;
        });
    }
}
