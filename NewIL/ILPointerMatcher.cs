using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;

namespace Shockah.CommonModCode.IL
{
	public readonly struct ILPointerMatcher : ILMatcher
	{
		public sealed class AnchorPointerSubscripts
		{
			internal ILPointerMatcher Owner { get; set; }

			public ILPointerMatcher this[string anchor]
			{
				get => new(Owner.AllInstructions, Owner.StoredAnchors[anchor], Owner.StoredAnchors);
			}
		}

		public enum PostInsertionIndex
		{
			Start, End
		}

		public enum PostRemovalIndex
		{
			Previous, Next
		}

		public IReadOnlyList<CodeInstruction> AllInstructions { get; init; }
		public int Index { get; init; }
		private IReadOnlyDictionary<string, int> StoredAnchors { get; init; }

		public AnchorPointerSubscripts Anchors { get; init; } = new();

		public CodeInstruction Instruction
			=> AllInstructions[Index];

		public ILBlockMatcher Block
			=> new(AllInstructions, Index, 1, StoredAnchors);

		public ILBlockMatcher AllInstructionsBlock
			=> new(AllInstructions, 0, AllInstructions.Count, StoredAnchors);

		internal ILPointerMatcher(IReadOnlyList<CodeInstruction> instructions, int index, IReadOnlyDictionary<string, int> anchors)
		{
			if (index < 0 || index >= instructions.Count)
				throw new IndexOutOfRangeException($"Invalid value {index} for parameter `{nameof(index)}`.");

			this.AllInstructions = instructions;
			this.Index = index;
			this.StoredAnchors = anchors;
			this.Anchors.Owner = this;
		}

		public ILPointerMatcher(IEnumerable<CodeInstruction> instructions, int index)
		{
			this.AllInstructions = instructions.ToList();

			if (index < 0 || index >= AllInstructions.Count)
				throw new IndexOutOfRangeException($"Invalid value {index} for parameter `{nameof(index)}`.");

			this.Index = index;
			this.StoredAnchors = new Dictionary<string, int>();
			this.Anchors.Owner = this;
		}

		public ILPointerMatcher Advance(int offset = 1)
			=> new(AllInstructions, Index + offset, StoredAnchors);

		public ILPointerMatcher JumpToLabel(Label label)
		{
			for (int i = 0; i < AllInstructions.Count; i++)
			{
				if (AllInstructions[i].labels.Contains(label))
					return new(AllInstructions, i, StoredAnchors);
			}
			throw new ILMatcherException($"Label {label} not found.");
		}

		public ILPointerMatcher Anchor(string anchor)
		{
			if (Index >= AllInstructions.Count)
				throw new ILMatcherException("No instruction to anchor to (the pointer is past all instructions).");

			Dictionary<string, int> anchors = new(StoredAnchors) { [anchor] = Index };
			return new(AllInstructions, Index, anchors);
		}

		public ILPointerMatcher Anchor(out string anchor)
		{
			anchor = Guid.NewGuid().ToString();
			return Anchor(anchor);
		}

		public ILPointerMatcher Replace(CodeInstruction instruction)
		{
			if (Index >= AllInstructions.Count)
				throw new ILMatcherException("No instruction to replace (the pointer is past all instructions).");

			var result = AllInstructions.ToList();
			result[Index] = instruction;

			var anchors = StoredAnchors;
			var thisIndex = Index;
			if (anchors.Any(e => e.Value == thisIndex))
				anchors = anchors.Where(e => e.Value != thisIndex).ToDictionary(e => e.Key, e => e.Value);

			return new(result, Index, anchors);
		}

		public ILBlockMatcher Insert(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> result = new();
			result.AddRange(AllInstructions.Take(Index));
			result.AddRange(instructions);
			result.AddRange(AllInstructions.Skip(Index));
			var lengthDifference = result.Count - AllInstructions.Count;

			var anchors = StoredAnchors;
			var thisIndex = Index;
			if (anchors.Any(e => e.Value >= thisIndex))
				anchors = anchors.Select(e => e.Value >= thisIndex ? new(e.Key, e.Value + lengthDifference) : e).ToDictionary(e => e.Key, e => e.Value);

			return new(result, Index, lengthDifference, anchors);
		}

		public ILBlockMatcher Insert(params CodeInstruction[] instructions)
			=> Insert((IEnumerable<CodeInstruction>)instructions);

		public ILPointerMatcher Insert(PostInsertionIndex postInsertionIndex, IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> result = new();
			result.AddRange(AllInstructions.Take(Index));
			result.AddRange(instructions);
			result.AddRange(AllInstructions.Skip(Index));
			var lengthDifference = result.Count - AllInstructions.Count;

			var anchors = StoredAnchors;
			var thisIndex = Index;
			if (anchors.Any(e => e.Value >= thisIndex))
				anchors = anchors.Select(e => e.Value >= thisIndex ? new(e.Key, e.Value + lengthDifference) : e).ToDictionary(e => e.Key, e => e.Value);

			return postInsertionIndex switch
			{
				PostInsertionIndex.Start => new(result, Index, anchors),
				PostInsertionIndex.End => new(result, Index + lengthDifference, anchors),
				_ => throw new ArgumentException($"{nameof(PostInsertionIndex)} has an invalid value."),
			};
		}

		public ILPointerMatcher Insert(PostInsertionIndex insertionIndex, params CodeInstruction[] instructions)
			=> Insert(insertionIndex, (IEnumerable<CodeInstruction>)instructions);

		public ILPointerMatcher Remove(PostRemovalIndex postRemovalIndex)
		{
			if (Index >= AllInstructions.Count)
				throw new ILMatcherException("No instruction to remove (the pointer is past all instructions).");
			if (AllInstructions.Count == 0)
				throw new ILMatcherException("No instruction to remove.");

			var result = AllInstructions.ToList();
			result.RemoveAt(Index);

			var anchors = StoredAnchors;
			var thisIndex = Index;
			if (anchors.Any(e => e.Value >= thisIndex))
				anchors = anchors.Where(e => e.Value != thisIndex).Select(e => e.Value > thisIndex ? new(e.Key, e.Value - 1) : e).ToDictionary(e => e.Key, e => e.Value);

			return postRemovalIndex switch
			{
				PostRemovalIndex.Previous => new(result, Math.Max(Index - 1, 0), anchors),
				PostRemovalIndex.Next => new(result, Math.Min(Index, AllInstructions.Count - 1), anchors),
				_ => throw new ArgumentException($"{nameof(PostRemovalIndex)} has an invalid value."),
			};
		}

		public ILPointerMatcher AddLabel(Label label)
		{
			if (Index >= AllInstructions.Count)
				throw new ILMatcherException("No instruction to add label to (the pointer is past all instructions).");

			var result = AllInstructions.ToList();
			result[Index] = new(result[Index]);
			result[Index].labels.Add(label);
			return new(result, Index, StoredAnchors);
		}

		public ILPointerMatcher CreateLabel(ILGenerator generator, out Label label)
		{
			if (Index >= AllInstructions.Count)
				throw new ILMatcherException("No instruction to create label on (the pointer is past all instructions).");

			label = generator.DefineLabel();
			return AddLabel(label);
		}

		public ILPointerMatcher TryExtractBranchTarget(out Label? label)
		{
			if (ILMatches.AnyBranch.Matches(Instruction))
				label = (Label)Instruction.operand;
			else
				label = null;
			return this;
		}

		public ILPointerMatcher ExtractBranchTarget(out Label label)
		{
			if (ILMatches.AnyBranch.Matches(Instruction))
				label = (Label)Instruction.operand;
			else
				throw new ILMatcherException($"{Instruction} is not a branch instruction.");
			return this;
		}

		public ILPointerMatcher TryCreateLdlocInstruction(out CodeInstruction? instruction)
		{
			if (Instruction.opcode == OpCodes.Ldloc_0 || Instruction.opcode == OpCodes.Stloc_0)
				instruction = new CodeInstruction(OpCodes.Ldloc_0);
			else if (Instruction.opcode == OpCodes.Ldloc_1 || Instruction.opcode == OpCodes.Stloc_1)
				instruction = new CodeInstruction(OpCodes.Ldloc_1);
			else if (Instruction.opcode == OpCodes.Ldloc_2 || Instruction.opcode == OpCodes.Stloc_2)
				instruction = new CodeInstruction(OpCodes.Ldloc_2);
			else if (Instruction.opcode == OpCodes.Ldloc_3 || Instruction.opcode == OpCodes.Stloc_3)
				instruction = new CodeInstruction(OpCodes.Ldloc_3);
			else if (Instruction.opcode == OpCodes.Ldloc || Instruction.opcode == OpCodes.Stloc || Instruction.opcode == OpCodes.Ldloc_S || Instruction.opcode == OpCodes.Stloc_S || Instruction.opcode == OpCodes.Ldloca || Instruction.opcode == OpCodes.Ldloca_S)
				instruction = new CodeInstruction(OpCodes.Ldloc, ILMatches.ExtractLocalIndex(Instruction.operand)!.Value);
			else
				instruction = null;
			return this;
		}

		public ILPointerMatcher CreateLdlocInstruction(out CodeInstruction instruction)
		{
			TryCreateLdlocInstruction(out var tryInstruction);
			if (tryInstruction is null)
				throw new ILMatcherException($"{Instruction} is not a local instruction.");
			instruction = tryInstruction;
			return this;
		}

		public ILPointerMatcher TryCreateStlocInstruction(out CodeInstruction? instruction)
		{
			if (Instruction.opcode == OpCodes.Ldloc_0 || Instruction.opcode == OpCodes.Stloc_0)
				instruction = new CodeInstruction(OpCodes.Stloc_0);
			else if (Instruction.opcode == OpCodes.Ldloc_1 || Instruction.opcode == OpCodes.Stloc_1)
				instruction = new CodeInstruction(OpCodes.Stloc_1);
			else if (Instruction.opcode == OpCodes.Ldloc_2 || Instruction.opcode == OpCodes.Stloc_2)
				instruction = new CodeInstruction(OpCodes.Stloc_2);
			else if (Instruction.opcode == OpCodes.Ldloc_3 || Instruction.opcode == OpCodes.Stloc_3)
				instruction = new CodeInstruction(OpCodes.Stloc_3);
			else if (Instruction.opcode == OpCodes.Ldloc || Instruction.opcode == OpCodes.Stloc || Instruction.opcode == OpCodes.Ldloc_S || Instruction.opcode == OpCodes.Stloc_S || Instruction.opcode == OpCodes.Ldloca || Instruction.opcode == OpCodes.Ldloca_S)
				instruction = new CodeInstruction(OpCodes.Stloc, ILMatches.ExtractLocalIndex(Instruction.operand)!.Value);
			else
				instruction = null;
			return this;
		}

		public ILPointerMatcher CreateStlocInstruction(out CodeInstruction instruction)
		{
			TryCreateStlocInstruction(out var tryInstruction);
			if (tryInstruction is null)
				throw new ILMatcherException($"{Instruction} is not a local instruction.");
			instruction = tryInstruction;
			return this;
		}

		public ILPointerMatcher TryCreateLdlocaInstruction(out CodeInstruction? instruction)
		{
			if (Instruction.opcode == OpCodes.Ldloc_0 || Instruction.opcode == OpCodes.Stloc_0)
				instruction = new CodeInstruction(OpCodes.Ldloca, 0);
			else if (Instruction.opcode == OpCodes.Ldloc_1 || Instruction.opcode == OpCodes.Stloc_1)
				instruction = new CodeInstruction(OpCodes.Ldloca, 1);
			else if (Instruction.opcode == OpCodes.Ldloc_2 || Instruction.opcode == OpCodes.Stloc_2)
				instruction = new CodeInstruction(OpCodes.Ldloca, 2);
			else if (Instruction.opcode == OpCodes.Ldloc_3 || Instruction.opcode == OpCodes.Stloc_3)
				instruction = new CodeInstruction(OpCodes.Ldloca, 3);
			else if (Instruction.opcode == OpCodes.Ldloc || Instruction.opcode == OpCodes.Stloc || Instruction.opcode == OpCodes.Ldloc_S || Instruction.opcode == OpCodes.Stloc_S || Instruction.opcode == OpCodes.Ldloca || Instruction.opcode == OpCodes.Ldloca_S)
				instruction = new CodeInstruction(OpCodes.Ldloca, ILMatches.ExtractLocalIndex(Instruction.operand)!.Value);
			else
				instruction = null;
			return this;
		}

		public ILPointerMatcher CreateLdlocaInstruction(out CodeInstruction instruction)
		{
			TryCreateLdlocaInstruction(out var tryInstruction);
			if (tryInstruction is null)
				throw new ILMatcherException($"{Instruction} is not a local instruction.");
			instruction = tryInstruction;
			return this;
		}

#if DEBUG
		public ILPointerMatcher Breakpoint()
		{
			if (Debugger.IsAttached)
				Debugger.Break();
			return this;
		}
#endif
	}
}