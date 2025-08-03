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

        localIndex = 0;
        return false;
    }

    private static bool TryGetOperandLocalIndex(object? operand, out int localIndex)
    {
        switch (operand)
        {
            case LocalBuilder local:
                localIndex = local.LocalIndex;
                return true;
            case int @int:
                localIndex = @int;
                return true;
            case sbyte @sbyte:
                localIndex = @sbyte;
                return true;
            default:
                localIndex = 0;
                return false;
        }
    }

    /// <summary>
    /// Tries to get the constant int value of a given instruction.
    /// </summary>
    /// <param name="instruction">The instruction that potentially loads a constant int value.</param>
    /// <param name="constant">The constant int value.</param>
    /// <returns>Whether the instruction actually loads a constant int value.</returns>
    public static bool TryGetIntConstant(this CodeInstruction instruction, out int constant)
    {
        if (instruction.opcode == OpCodes.Ldc_I4_0)
        {
            constant = 0;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldc_I4_1)
        {
            constant = 1;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldc_I4_2)
        {
            constant = 2;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldc_I4_3)
        {
            constant = 3;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldc_I4_4)
        {
            constant = 4;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldc_I4_5)
        {
            constant = 5;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldc_I4_6)
        {
            constant = 6;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldc_I4_7)
        {
            constant = 7;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldc_I4_8)
        {
            constant = 8;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldc_I4_M1)
        {
            constant = -1;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldc_I4)
        {
            constant = (int)instruction.operand;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldc_I4_S)
        {
            constant = (sbyte)instruction.operand;
            return true;
        }

        constant = 0;
        return false;
    }
}
