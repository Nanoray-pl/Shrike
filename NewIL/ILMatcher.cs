using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Shockah.CommonModCode.IL
{
	public interface ILMatcher
	{
		IReadOnlyList<CodeInstruction> AllInstructions { get; }
		ILBlockMatcher AllInstructionsBlock { get; }

		ILPointerMatcher JumpToLabel(Label label);
	}
}