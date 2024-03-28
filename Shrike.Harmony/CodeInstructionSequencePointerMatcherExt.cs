using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace Nanoray.Shrike.Harmony;

/// <summary>
/// A static class hosting additional extensions for <see cref="SequencePointerMatcher{TElement}"/> with <see cref="CodeInstruction"/> elements.
/// </summary>
public static class CodeInstructionSequencePointerMatcherExt
{
    /// <summary>
    /// Add a label to the current instruction.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="label">The label to add.</param>
    /// <returns>A new pointer matcher, with a modified instruction including the given label.</returns>
    public static SequencePointerMatcher<CodeInstruction> AddLabel(this SequencePointerMatcher<CodeInstruction> self, Label label)
    {
        if (self.Index() >= self.AllElements().Count)
            throw new SequenceMatcherException("No instruction to add label to (the pointer is past all instructions).");
        return self.Replace(new CodeInstruction(self.Element()).WithLabels(label));
    }

    /// <summary>
    /// Add labels to the current instruction.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="labels">The labels to add.</param>
    /// <returns>A new pointer matcher, with a modified instruction including the given labels.</returns>
    public static SequencePointerMatcher<CodeInstruction> AddLabels(this SequencePointerMatcher<CodeInstruction> self, IEnumerable<Label> labels)
    {
        if (self.Index() >= self.AllElements().Count)
            throw new SequenceMatcherException("No instruction to add labels to (the pointer is past all instructions).");
        return self.Replace(new CodeInstruction(self.Element()).WithLabels(labels));
    }

    /// <summary>
    /// Extracts labels from the current instruction, removing them from it.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="labels">The extracted labels.</param>
    /// <returns>A new pointer matcher, with a modified instruction without its labels.</returns>
    public static SequencePointerMatcher<CodeInstruction> ExtractLabels(this SequencePointerMatcher<CodeInstruction> self, out IReadOnlySet<Label> labels)
    {
        if (self.Index() >= self.AllElements().Count)
            throw new SequenceMatcherException("No instruction to extract labels from (the pointer is past all instructions).");

        var instruction = self.Element();
        labels = instruction.labels.ToHashSet();
        return self.Replace(new CodeInstruction(instruction.opcode, instruction.operand)
        {
            blocks = instruction.blocks.ToList()
        });
    }

    /// <summary>
    /// Creates a new label and adds it to the current instruction.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="il">The <see cref="ILGenerator"/> to use.</param>
    /// <param name="label">The newly created label.</param>
    /// <returns>A new pointer matcher, with a modified instruction including the given label.</returns>
    public static SequencePointerMatcher<CodeInstruction> CreateLabel(this SequencePointerMatcher<CodeInstruction> self, ILGenerator il, out Label label)
    {
        if (self.Index() >= self.AllElements().Count)
            throw new SequenceMatcherException("No instruction to add label to (the pointer is past all instructions).");

        label = il.DefineLabel();
        return self.AddLabel(label);
    }

    /// <summary>
    /// Tries to retrieve a branch instruction target label.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="label">The retrieved label, or <c>null</c>.</param>
    /// <returns>The current matcher.</returns>
    public static SequencePointerMatcher<CodeInstruction> TryGetBranchTarget(this SequencePointerMatcher<CodeInstruction> self, out Label? label)
    {
        if (ILMatches.AnyBranch.Matches(self.Element()))
            label = (Label)self.Element().operand;
        else
            label = null;
        return self;
    }

    /// <summary>
    /// Retrieves a branch instruction target label.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="label">The retrieved label.</param>
    /// <returns>The current matcher.</returns>
    public static SequencePointerMatcher<CodeInstruction> GetBranchTarget(this SequencePointerMatcher<CodeInstruction> self, out Label label)
    {
        if (ILMatches.AnyBranch.Matches(self.Element()))
            label = (Label)self.Element().operand;
        else
            throw new SequenceMatcherException($"{self.Element()} is not a branch instruction.");
        return self;
    }

    /// <summary>
    /// Tries to create an <c>ldloc</c> instruction referencing the same local variable the current instruction does.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="instruction">The created instruction, or <c>null</c>.</param>
    /// <returns>The current matcher.</returns>
    public static SequencePointerMatcher<CodeInstruction> TryCreateLdlocInstruction(this SequencePointerMatcher<CodeInstruction> self, out CodeInstruction? instruction)
    {
        if (self.Element().opcode == OpCodes.Ldloc_0 || self.Element().opcode == OpCodes.Stloc_0)
            instruction = new CodeInstruction(OpCodes.Ldloc_0);
        else if (self.Element().opcode == OpCodes.Ldloc_1 || self.Element().opcode == OpCodes.Stloc_1)
            instruction = new CodeInstruction(OpCodes.Ldloc_1);
        else if (self.Element().opcode == OpCodes.Ldloc_2 || self.Element().opcode == OpCodes.Stloc_2)
            instruction = new CodeInstruction(OpCodes.Ldloc_2);
        else if (self.Element().opcode == OpCodes.Ldloc_3 || self.Element().opcode == OpCodes.Stloc_3)
            instruction = new CodeInstruction(OpCodes.Ldloc_3);
        else if (self.Element().opcode == OpCodes.Ldloc || self.Element().opcode == OpCodes.Stloc || self.Element().opcode == OpCodes.Ldloc_S || self.Element().opcode == OpCodes.Stloc_S || self.Element().opcode == OpCodes.Ldloca || self.Element().opcode == OpCodes.Ldloca_S)
            instruction = new CodeInstruction(OpCodes.Ldloc, ILMatches.ExtractLocalIndex(self.Element().operand)!.Value);
        else
            instruction = null;
        return self;
    }

    /// <summary>
    /// Creates an <c>ldloc</c> instruction referencing the same local variable the current instruction does.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="instruction">The created instruction.</param>
    /// <returns>The current matcher.</returns>
    public static SequencePointerMatcher<CodeInstruction> CreateLdlocInstruction(this SequencePointerMatcher<CodeInstruction> self, out CodeInstruction instruction)
    {
        self.TryCreateLdlocInstruction(out var tryInstruction);
        if (tryInstruction is null)
            throw new SequenceMatcherException($"{self.Element()} is not a local instruction.");
        instruction = tryInstruction;
        return self;
    }

    /// <summary>
    /// Tries to create an <c>stloc</c> instruction referencing the same local variable the current instruction does.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="instruction">The created instruction, or <c>null</c>.</param>
    /// <returns>The current matcher.</returns>
    public static SequencePointerMatcher<CodeInstruction> TryCreateStlocInstruction(this SequencePointerMatcher<CodeInstruction> self, out CodeInstruction? instruction)
    {
        if (self.Element().opcode == OpCodes.Ldloc_0 || self.Element().opcode == OpCodes.Stloc_0)
            instruction = new CodeInstruction(OpCodes.Stloc_0);
        else if (self.Element().opcode == OpCodes.Ldloc_1 || self.Element().opcode == OpCodes.Stloc_1)
            instruction = new CodeInstruction(OpCodes.Stloc_1);
        else if (self.Element().opcode == OpCodes.Ldloc_2 || self.Element().opcode == OpCodes.Stloc_2)
            instruction = new CodeInstruction(OpCodes.Stloc_2);
        else if (self.Element().opcode == OpCodes.Ldloc_3 || self.Element().opcode == OpCodes.Stloc_3)
            instruction = new CodeInstruction(OpCodes.Stloc_3);
        else if (self.Element().opcode == OpCodes.Ldloc || self.Element().opcode == OpCodes.Stloc || self.Element().opcode == OpCodes.Ldloc_S || self.Element().opcode == OpCodes.Stloc_S || self.Element().opcode == OpCodes.Ldloca || self.Element().opcode == OpCodes.Ldloca_S)
            instruction = new CodeInstruction(OpCodes.Stloc, ILMatches.ExtractLocalIndex(self.Element().operand)!.Value);
        else
            instruction = null;
        return self;
    }

    /// <summary>
    /// Creates an <c>stloc</c> instruction referencing the same local variable the current instruction does.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="instruction">The created instruction.</param>
    /// <returns>The current matcher.</returns>
    public static SequencePointerMatcher<CodeInstruction> CreateStlocInstruction(this SequencePointerMatcher<CodeInstruction> self, out CodeInstruction instruction)
    {
        self.TryCreateStlocInstruction(out var tryInstruction);
        if (tryInstruction is null)
            throw new SequenceMatcherException($"{self.Element()} is not a local instruction.");
        instruction = tryInstruction;
        return self;
    }

    /// <summary>
    /// Tries to create an <c>ldloc.a</c> instruction referencing the same local variable the current instruction does.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="instruction">The created instruction, or <c>null</c>.</param>
    /// <returns>The current matcher.</returns>
    public static SequencePointerMatcher<CodeInstruction> TryCreateLdlocaInstruction(this SequencePointerMatcher<CodeInstruction> self, out CodeInstruction? instruction)
    {
        if (self.Element().opcode == OpCodes.Ldloc_0 || self.Element().opcode == OpCodes.Stloc_0)
            instruction = new CodeInstruction(OpCodes.Ldloca, 0);
        else if (self.Element().opcode == OpCodes.Ldloc_1 || self.Element().opcode == OpCodes.Stloc_1)
            instruction = new CodeInstruction(OpCodes.Ldloca, 1);
        else if (self.Element().opcode == OpCodes.Ldloc_2 || self.Element().opcode == OpCodes.Stloc_2)
            instruction = new CodeInstruction(OpCodes.Ldloca, 2);
        else if (self.Element().opcode == OpCodes.Ldloc_3 || self.Element().opcode == OpCodes.Stloc_3)
            instruction = new CodeInstruction(OpCodes.Ldloca, 3);
        else if (self.Element().opcode == OpCodes.Ldloc || self.Element().opcode == OpCodes.Stloc || self.Element().opcode == OpCodes.Ldloc_S || self.Element().opcode == OpCodes.Stloc_S || self.Element().opcode == OpCodes.Ldloca || self.Element().opcode == OpCodes.Ldloca_S)
            instruction = new CodeInstruction(OpCodes.Ldloca, ILMatches.ExtractLocalIndex(self.Element().operand)!.Value);
        else
            instruction = null;
        return self;
    }

    /// <summary>
    /// Creates an <c>ldloc.a</c> instruction referencing the same local variable the current instruction does.
    /// </summary>
    /// <param name="self">The current matcher.</param>
    /// <param name="instruction">The created instruction.</param>
    /// <returns>The current matcher.</returns>
    public static SequencePointerMatcher<CodeInstruction> CreateLdlocaInstruction(this SequencePointerMatcher<CodeInstruction> self, out CodeInstruction instruction)
    {
        self.TryCreateLdlocaInstruction(out var tryInstruction);
        if (tryInstruction is null)
            throw new SequenceMatcherException($"{self.Element()} is not a local instruction.");
        instruction = tryInstruction;
        return self;
    }
}
