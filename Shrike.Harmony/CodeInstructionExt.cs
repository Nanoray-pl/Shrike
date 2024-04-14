using System.Reflection.Emit;
using HarmonyLib;

namespace Nanoray.Shrike.Harmony;

/// <summary>
/// A static class hosting additional extensions for <see cref="CodeInstruction"/>s.
/// </summary>
public static class CodeInstructionExt
{
    /// <summary>
    /// Tries to get the index of a local variable referenced by a given instruction.
    /// </summary>
    /// <param name="instruction">The instruction that potentially references a local variable.</param>
    /// <param name="localIndex">The resulting index of the referenced local variable.</param>
    /// <returns>Whether the instruction actually references a local variable.</returns>
    public static bool TryGetLocalIndex(this CodeInstruction instruction, out int localIndex)
    {
        if (instruction.opcode == OpCodes.Ldloc_0 || instruction.opcode == OpCodes.Stloc_0)
        {
            localIndex = 0;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldloc_1 || instruction.opcode == OpCodes.Stloc_1)
        {
            localIndex = 1;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldloc_2 || instruction.opcode == OpCodes.Stloc_2)
        {
            localIndex = 2;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldloc_3 || instruction.opcode == OpCodes.Stloc_3)
        {
            localIndex = 3;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldloc || instruction.opcode == OpCodes.Ldloc_S || instruction.opcode == OpCodes.Ldloca || instruction.opcode == OpCodes.Ldloca_S || instruction.opcode == OpCodes.Stloc || instruction.opcode == OpCodes.Stloc_S)
            return TryGetOperandLocalIndex(instruction.operand, out localIndex);

        localIndex = default;
        return false;
    }

    private static bool TryGetOperandLocalIndex(object? operand, out int localIndex)
    {
        if (operand is LocalBuilder local)
        {
            localIndex = local.LocalIndex;
            return true;
        }
        if (operand is int @int)
        {
            localIndex = @int;
            return true;
        }
        if (operand is sbyte @sbyte)
        {
            localIndex = @sbyte;
            return true;
        }

        localIndex = default;
        return false;
    }
}
