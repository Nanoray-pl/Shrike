using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace Nanoray.Shrike.Harmony
{
    public static class CodeInstructionSequencePointerMatcherExt
    {
        public static TPointerMatcher AddLabel<TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, Label label)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            if (self.Index >= self.AllElements.Count)
                throw new SequenceMatcherException("No instruction to add label to (the pointer is past all instructions).");

            var result = self.AllElements.ToList();
            result[self.Index] = new(result[self.Index]);
            result[self.Index].labels.Add(label);

#if NET7_0_OR_GREATER
            return TPointerMatcher.MakeNewPointerMatcher(result, self.Index);
#else
            return self.MakeNewPointerMatcher(result, self.Index);
#endif
        }

        public static TPointerMatcher CreateLabel<TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, ILGenerator il, out Label label)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            if (self.Index >= self.AllElements.Count)
                throw new SequenceMatcherException("No instruction to add label to (the pointer is past all instructions).");

            label = il.DefineLabel();
            return self.AddLabel<TPointerMatcher, TBlockMatcher>(label);
        }

        public static TPointerMatcher TryExtractBranchTarget<TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, out Label? label)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            if (ILMatches.AnyBranch.Matches(self.Element))
                label = (Label)self.Element.operand;
            else
                label = null;
            return self;
        }

        public static TPointerMatcher ExtractBranchTarget<TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, out Label label)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            if (ILMatches.AnyBranch.Matches(self.Element))
                label = (Label)self.Element.operand;
            else
                throw new SequenceMatcherException($"{self.Element} is not a branch instruction.");
            return self;
        }

        public static TPointerMatcher TryCreateLdlocInstruction<TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, out CodeInstruction? instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            if (self.Element.opcode == OpCodes.Ldloc_0 || self.Element.opcode == OpCodes.Stloc_0)
                instruction = new CodeInstruction(OpCodes.Ldloc_0);
            else if (self.Element.opcode == OpCodes.Ldloc_1 || self.Element.opcode == OpCodes.Stloc_1)
                instruction = new CodeInstruction(OpCodes.Ldloc_1);
            else if (self.Element.opcode == OpCodes.Ldloc_2 || self.Element.opcode == OpCodes.Stloc_2)
                instruction = new CodeInstruction(OpCodes.Ldloc_2);
            else if (self.Element.opcode == OpCodes.Ldloc_3 || self.Element.opcode == OpCodes.Stloc_3)
                instruction = new CodeInstruction(OpCodes.Ldloc_3);
            else if (self.Element.opcode == OpCodes.Ldloc || self.Element.opcode == OpCodes.Stloc || self.Element.opcode == OpCodes.Ldloc_S || self.Element.opcode == OpCodes.Stloc_S || self.Element.opcode == OpCodes.Ldloca || self.Element.opcode == OpCodes.Ldloca_S)
                instruction = new CodeInstruction(OpCodes.Ldloc, ILMatches.ExtractLocalIndex(self.Element.operand)!.Value);
            else
                instruction = null;
            return self;
        }

        public static TPointerMatcher CreateLdlocInstruction<TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, out CodeInstruction instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            self.TryCreateLdlocInstruction<TPointerMatcher, TBlockMatcher>(out var tryInstruction);
            if (tryInstruction is null)
                throw new SequenceMatcherException($"{self.Element} is not a local instruction.");
            instruction = tryInstruction;
            return self;
        }

        public static TPointerMatcher TryCreateStlocInstruction<TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, out CodeInstruction? instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            if (self.Element.opcode == OpCodes.Ldloc_0 || self.Element.opcode == OpCodes.Stloc_0)
                instruction = new CodeInstruction(OpCodes.Stloc_0);
            else if (self.Element.opcode == OpCodes.Ldloc_1 || self.Element.opcode == OpCodes.Stloc_1)
                instruction = new CodeInstruction(OpCodes.Stloc_1);
            else if (self.Element.opcode == OpCodes.Ldloc_2 || self.Element.opcode == OpCodes.Stloc_2)
                instruction = new CodeInstruction(OpCodes.Stloc_2);
            else if (self.Element.opcode == OpCodes.Ldloc_3 || self.Element.opcode == OpCodes.Stloc_3)
                instruction = new CodeInstruction(OpCodes.Stloc_3);
            else if (self.Element.opcode == OpCodes.Ldloc || self.Element.opcode == OpCodes.Stloc || self.Element.opcode == OpCodes.Ldloc_S || self.Element.opcode == OpCodes.Stloc_S || self.Element.opcode == OpCodes.Ldloca || self.Element.opcode == OpCodes.Ldloca_S)
                instruction = new CodeInstruction(OpCodes.Stloc, ILMatches.ExtractLocalIndex(self.Element.operand)!.Value);
            else
                instruction = null;
            return self;
        }

        public static TPointerMatcher CreateStlocInstruction<TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, out CodeInstruction instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            self.TryCreateStlocInstruction<TPointerMatcher, TBlockMatcher>(out var tryInstruction);
            if (tryInstruction is null)
                throw new SequenceMatcherException($"{self.Element} is not a local instruction.");
            instruction = tryInstruction;
            return self;
        }

        public static TPointerMatcher TryCreateLdlocaInstruction<TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, out CodeInstruction? instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            if (self.Element.opcode == OpCodes.Ldloc_0 || self.Element.opcode == OpCodes.Stloc_0)
                instruction = new CodeInstruction(OpCodes.Ldloca, 0);
            else if (self.Element.opcode == OpCodes.Ldloc_1 || self.Element.opcode == OpCodes.Stloc_1)
                instruction = new CodeInstruction(OpCodes.Ldloca, 1);
            else if (self.Element.opcode == OpCodes.Ldloc_2 || self.Element.opcode == OpCodes.Stloc_2)
                instruction = new CodeInstruction(OpCodes.Ldloca, 2);
            else if (self.Element.opcode == OpCodes.Ldloc_3 || self.Element.opcode == OpCodes.Stloc_3)
                instruction = new CodeInstruction(OpCodes.Ldloca, 3);
            else if (self.Element.opcode == OpCodes.Ldloc || self.Element.opcode == OpCodes.Stloc || self.Element.opcode == OpCodes.Ldloc_S || self.Element.opcode == OpCodes.Stloc_S || self.Element.opcode == OpCodes.Ldloca || self.Element.opcode == OpCodes.Ldloca_S)
                instruction = new CodeInstruction(OpCodes.Ldloca, ILMatches.ExtractLocalIndex(self.Element.operand)!.Value);
            else
                instruction = null;
            return self;
        }

        public static TPointerMatcher CreateLdlocaInstruction<TPointerMatcher, TBlockMatcher>(this TPointerMatcher self, out CodeInstruction instruction)
            where TPointerMatcher : ISequencePointerMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
            where TBlockMatcher : ISequenceBlockMatcher<CodeInstruction, TPointerMatcher, TBlockMatcher>
        {
            self.TryCreateLdlocaInstruction<TPointerMatcher, TBlockMatcher>(out var tryInstruction);
            if (tryInstruction is null)
                throw new SequenceMatcherException($"{self.Element} is not a local instruction.");
            instruction = tryInstruction;
            return self;
        }
    }
}
