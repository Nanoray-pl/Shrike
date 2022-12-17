using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace Nanoray.Shrike.Harmony
{
    /// <summary>
    /// A static class hosting additional extensions for <see cref="ISequencePointerMatcher{TElement, TPointerMatcher, TBlockMatcher}"/> with <see cref="CodeInstruction"/> elements.
    /// </summary>
    public static class CodeInstructionSequencePointerMatcherExt
    {
        /// <summary>
        /// Add a label to the current instruction.
        /// </summary>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="label">The label to add.</param>
        /// <returns>A new pointer matcher, with a modified instruction including the given label.</returns>
        public static TPointerMatcher AddLabel<TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, Label label)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            if (self.Index() >= self.AllElements().Count)
                throw new SequenceMatcherException("No instruction to add label to (the pointer is past all instructions).");

            var result = self.AllElements().ToList();
            result[self.Index()] = new(result[self.Index()]);
            result[self.Index()].labels.Add(label);

#if NET7_0_OR_GREATER
            return TPointerMatcher.MakeNewPointerMatcher(result, self.Index());
#else
            return self.MakeNewPointerMatcher(result, self.Index());
#endif
        }

        /// <summary>
        /// Creates a new label and adds it to the current instruction.
        /// </summary>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="il">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="label">The newly created label.</param>
        /// <returns>A new pointer matcher, with a modified instruction including the given label.</returns>
        public static TPointerMatcher CreateLabel<TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, ILGenerator il, out Label label)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            if (self.Index() >= self.AllElements().Count)
                throw new SequenceMatcherException("No instruction to add label to (the pointer is past all instructions).");

            label = il.DefineLabel();
            return self.AddLabel(label);
        }

        /// <summary>
        /// Tries to extract a branch instruction target label.
        /// </summary>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="label">The extracted label, or <c>null</c>.</param>
        /// <returns>The current matcher.</returns>
        public static TPointerMatcher TryExtractBranchTarget<TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, out Label? label)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            if (ILMatches.AnyBranch.Matches(self.Element()))
                label = (Label)self.Element().operand;
            else
                label = null;
            return self.MakePointerMatcher(self.Index());
        }

        /// <summary>
        /// Extracts a branch instruction target label.
        /// </summary>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="label">The extracted label.</param>
        /// <returns>The current matcher.</returns>
        public static TPointerMatcher ExtractBranchTarget<TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, out Label label)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            if (ILMatches.AnyBranch.Matches(self.Element()))
                label = (Label)self.Element().operand;
            else
                throw new SequenceMatcherException($"{self.Element()} is not a branch instruction.");
            return self.MakePointerMatcher(self.Index());
        }

        /// <summary>
        /// Tries to create an <c>ldloc</c> instruction referencing the same local variable the current instruction does.
        /// </summary>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="instruction">The created instruction, or <c>null</c>.</param>
        /// <returns>The current matcher.</returns>
        public static TPointerMatcher TryCreateLdlocInstruction<TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, out CodeInstruction? instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
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
            return self.MakePointerMatcher(self.Index());
        }

        /// <summary>
        /// Creates an <c>ldloc</c> instruction referencing the same local variable the current instruction does.
        /// </summary>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="instruction">The created instruction.</param>
        /// <returns>The current matcher.</returns>
        public static TPointerMatcher CreateLdlocInstruction<TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, out CodeInstruction instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            self.TryCreateLdlocInstruction(out var tryInstruction);
            if (tryInstruction is null)
                throw new SequenceMatcherException($"{self.Element()} is not a local instruction.");
            instruction = tryInstruction;
            return self.MakePointerMatcher(self.Index());
        }

        /// <summary>
        /// Tries to create an <c>stloc</c> instruction referencing the same local variable the current instruction does.
        /// </summary>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="instruction">The created instruction, or <c>null</c>.</param>
        /// <returns>The current matcher.</returns>
        public static TPointerMatcher TryCreateStlocInstruction<TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, out CodeInstruction? instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
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
            return self.MakePointerMatcher(self.Index());
        }

        /// <summary>
        /// Creates an <c>stloc</c> instruction referencing the same local variable the current instruction does.
        /// </summary>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="instruction">The created instruction.</param>
        /// <returns>The current matcher.</returns>
        public static TPointerMatcher CreateStlocInstruction<TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, out CodeInstruction instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            self.TryCreateStlocInstruction(out var tryInstruction);
            if (tryInstruction is null)
                throw new SequenceMatcherException($"{self.Element()} is not a local instruction.");
            instruction = tryInstruction;
            return self.MakePointerMatcher(self.Index());
        }

        /// <summary>
        /// Tries to create an <c>ldloc.a</c> instruction referencing the same local variable the current instruction does.
        /// </summary>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="instruction">The created instruction, or <c>null</c>.</param>
        /// <returns>The current matcher.</returns>
        public static TPointerMatcher TryCreateLdlocaInstruction<TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, out CodeInstruction? instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
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
            return self.MakePointerMatcher(self.Index());
        }

        /// <summary>
        /// Creates an <c>ldloc.a</c> instruction referencing the same local variable the current instruction does.
        /// </summary>
        /// <typeparam name="TPointerMatcher">The pointer matcher implementation.</typeparam>
        /// <typeparam name="TBlockMatcher">The block matcher implementation.</typeparam>
        /// <param name="self">The current matcher.</param>
        /// <param name="instruction">The created instruction.</param>
        /// <returns>The current matcher.</returns>
        public static TPointerMatcher CreateLdlocaInstruction<TPointerMatcher, TBlockMatcher>(this ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher> self, out CodeInstruction instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            self.TryCreateLdlocaInstruction(out var tryInstruction);
            if (tryInstruction is null)
                throw new SequenceMatcherException($"{self.Element()} is not a local instruction.");
            instruction = tryInstruction;
            return self.MakePointerMatcher(self.Index());
        }
    }
}
