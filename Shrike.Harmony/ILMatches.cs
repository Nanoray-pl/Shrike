using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Nanoray.Shrike.Harmony
{
    public static class ILMatches
    {
        public static IElementMatch<CodeInstruction> Br { get; private set; }
            = new ElementMatch<CodeInstruction>("{br(.s)}", i => i.opcode == OpCodes.Br || i.opcode == OpCodes.Br_S);

        public static IElementMatch<CodeInstruction> Brfalse { get; private set; }
            = new ElementMatch<CodeInstruction>("{brfalse(.s)}", i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S);

        public static IElementMatch<CodeInstruction> Brtrue { get; private set; }
            = new ElementMatch<CodeInstruction>("{brtrue(.s)}", i => i.opcode == OpCodes.Brtrue || i.opcode == OpCodes.Brtrue_S);

        public static IElementMatch<CodeInstruction> AnyBoolBranch { get; private set; }
            = new ElementMatch<CodeInstruction>("{brfalse/brtrue(.s)}", i => Brfalse.Matches(i) || Brtrue.Matches(i));

        public static IElementMatch<CodeInstruction> Beq { get; private set; }
            = new ElementMatch<CodeInstruction>("{beq(.s)}", i => i.opcode == OpCodes.Beq || i.opcode == OpCodes.Beq_S);

        public static IElementMatch<CodeInstruction> BneUn { get; private set; }
            = new ElementMatch<CodeInstruction>("{bne.un(.s)}", i => i.opcode == OpCodes.Bne_Un || i.opcode == OpCodes.Bne_Un_S);

        public static IElementMatch<CodeInstruction> Ble { get; private set; }
            = new ElementMatch<CodeInstruction>("{ble(.s)}", i => i.opcode == OpCodes.Ble || i.opcode == OpCodes.Ble_S);

        public static IElementMatch<CodeInstruction> BleUn { get; private set; }
            = new ElementMatch<CodeInstruction>("{ble.un(.s)}", i => i.opcode == OpCodes.Ble_Un || i.opcode == OpCodes.Ble_Un_S);

        public static IElementMatch<CodeInstruction> AnyBle { get; private set; }
            = new ElementMatch<CodeInstruction>("{ble(.un)(.s)}", i => Ble.Matches(i) || BleUn.Matches(i));

        public static IElementMatch<CodeInstruction> Bge { get; private set; }
            = new ElementMatch<CodeInstruction>("{bge(.s)}", i => i.opcode == OpCodes.Bge || i.opcode == OpCodes.Bge_S);

        public static IElementMatch<CodeInstruction> BgeUn { get; private set; }
            = new ElementMatch<CodeInstruction>("{bge.un(.s)}", i => i.opcode == OpCodes.Bge_Un || i.opcode == OpCodes.Bge_Un_S);

        public static IElementMatch<CodeInstruction> AnyBge { get; private set; }
            = new ElementMatch<CodeInstruction>("{bge(.un)(.s)}", i => Bge.Matches(i) || BgeUn.Matches(i));

        public static IElementMatch<CodeInstruction> Blt { get; private set; }
            = new ElementMatch<CodeInstruction>("{blt(.s)}", i => i.opcode == OpCodes.Blt || i.opcode == OpCodes.Blt_S);

        public static IElementMatch<CodeInstruction> BltUn { get; private set; }
            = new ElementMatch<CodeInstruction>("{blt.un(.s)}", i => i.opcode == OpCodes.Blt_Un || i.opcode == OpCodes.Blt_Un_S);

        public static IElementMatch<CodeInstruction> AnyBlt { get; private set; }
            = new ElementMatch<CodeInstruction>("{blt(.un)(.s)}", i => Blt.Matches(i) || BltUn.Matches(i));

        public static IElementMatch<CodeInstruction> Bgt { get; private set; }
            = new ElementMatch<CodeInstruction>("{bgt(.s)}", i => i.opcode == OpCodes.Bgt || i.opcode == OpCodes.Bgt_S);

        public static IElementMatch<CodeInstruction> BgtUn { get; private set; }
            = new ElementMatch<CodeInstruction>("{bgt.un(.s)}", i => i.opcode == OpCodes.Bgt_Un || i.opcode == OpCodes.Bgt_Un_S);

        public static IElementMatch<CodeInstruction> AnyBgt { get; private set; }
            = new ElementMatch<CodeInstruction>("{bgt(.un)(.s)}", i => Bgt.Matches(i) || BgtUn.Matches(i));

        public static IElementMatch<CodeInstruction> AnyBranch { get; private set; }
            = new ElementMatch<CodeInstruction>("{any branch}", i => Br.Matches(i) || Beq.Matches(i) || BneUn.Matches(i) || AnyBoolBranch.Matches(i) || AnyBle.Matches(i) || AnyBge.Matches(i) || AnyBlt.Matches(i) || AnyBgt.Matches(i));

        public static IElementMatch<CodeInstruction> AnyLdloc { get; private set; }
            = new ElementMatch<CodeInstruction>("{any ldloc}", i => i.opcode == OpCodes.Ldloc || i.opcode == OpCodes.Ldloc_S || i.opcode == OpCodes.Ldloc_0 || i.opcode == OpCodes.Ldloc_1 || i.opcode == OpCodes.Ldloc_2 || i.opcode == OpCodes.Ldloc_3);

        public static IElementMatch<CodeInstruction> AnyStloc { get; private set; }
            = new ElementMatch<CodeInstruction>("{any stloc}", i => i.opcode == OpCodes.Stloc || i.opcode == OpCodes.Stloc_S || i.opcode == OpCodes.Stloc_0 || i.opcode == OpCodes.Stloc_1 || i.opcode == OpCodes.Stloc_2 || i.opcode == OpCodes.Stloc_3);

        public static IElementMatch<CodeInstruction> AnyLdloca { get; private set; }
            = new ElementMatch<CodeInstruction>("{any ldloc}", i => i.opcode == OpCodes.Ldloca || i.opcode == OpCodes.Ldloca_S);

        public static IElementMatch<CodeInstruction> AnyCall { get; private set; }
            = new ElementMatch<CodeInstruction>("{call(virt)}", i => i.opcode == OpCodes.Call || i.opcode == OpCodes.Callvirt);

        public static IElementMatch<CodeInstruction> AnyLdcI4 { get; private set; }
            = new ElementMatch<CodeInstruction>("{any ldc.i4}", i =>
            {
                return
                i.opcode == OpCodes.Ldc_I4 ||
                i.opcode == OpCodes.Ldc_I4_S ||
                i.opcode == OpCodes.Ldc_I4_0 ||
                i.opcode == OpCodes.Ldc_I4_1 ||
                i.opcode == OpCodes.Ldc_I4_2 ||
                i.opcode == OpCodes.Ldc_I4_3 ||
                i.opcode == OpCodes.Ldc_I4_4 ||
                i.opcode == OpCodes.Ldc_I4_5 ||
                i.opcode == OpCodes.Ldc_I4_6 ||
                i.opcode == OpCodes.Ldc_I4_7 ||
                i.opcode == OpCodes.Ldc_I4_8 ||
                i.opcode == OpCodes.Ldc_I4_M1;
            });

        public static IElementMatch<CodeInstruction> Ldarg(int index)
            => new ElementMatch<CodeInstruction>($"{{ldarg: {index}}}", i => i.IsLdarg(index));

        public static IElementMatch<CodeInstruction> LdcI4(int value)
            => new ElementMatch<CodeInstruction>($"{{ldc.i4: {value}}}", i =>
            {
                switch (value)
                {
                    case 0:
                        if (i.opcode == OpCodes.Ldc_I4_0)
                            return true;
                        break;
                    case 1:
                        if (i.opcode == OpCodes.Ldc_I4_1)
                            return true;
                        break;
                    case 2:
                        if (i.opcode == OpCodes.Ldc_I4_2)
                            return true;
                        break;
                    case 3:
                        if (i.opcode == OpCodes.Ldc_I4_3)
                            return true;
                        break;
                    case 4:
                        if (i.opcode == OpCodes.Ldc_I4_4)
                            return true;
                        break;
                    case 5:
                        if (i.opcode == OpCodes.Ldc_I4_5)
                            return true;
                        break;
                    case 6:
                        if (i.opcode == OpCodes.Ldc_I4_6)
                            return true;
                        break;
                    case 7:
                        if (i.opcode == OpCodes.Ldc_I4_7)
                            return true;
                        break;
                    case 8:
                        if (i.opcode == OpCodes.Ldc_I4_8)
                            return true;
                        break;
                    case -1:
                        if (i.opcode == OpCodes.Ldc_I4_M1)
                            return true;
                        break;
                }
                return (i.opcode == OpCodes.Ldc_I4 && (int)i.operand == value) ||
                    (value < 256 && i.opcode == OpCodes.Ldc_I4_S && ((i.operand is int @int && @int == value) || (i.operand is sbyte @byte && @byte == value)));
            });

        public static IElementMatch<CodeInstruction> Ldstr(string value)
            => new ElementMatch<CodeInstruction>($"{{ldstr `{value}`}}", i => i.opcode == OpCodes.Ldstr && (string)i.operand == value);

        public static IElementMatch<CodeInstruction> Ldloc(Type type, IEnumerable<LocalVariableInfo> locals)
            => new ElementMatch<CodeInstruction>($"{{ldloc(.s) matching type {type}}}", i =>
            {
                if (!AnyLdloc.Matches(i))
                    return false;
                if (i.operand is LocalBuilder local)
                {
                    return local.LocalType == type;
                }
                else
                {
                    int? localIndex = ExtractLocalIndex(i);
                    if (localIndex is null)
                        return false;
                    var providedLocal = locals.FirstOrDefault(l => l.LocalIndex == localIndex.Value);
                    return providedLocal is not null && providedLocal.LocalType == type;
                }
            });

        public static IElementMatch<CodeInstruction> Ldloc<T>(IEnumerable<LocalVariableInfo> locals)
            => Ldloc(typeof(T), locals);

        public static IElementMatch<CodeInstruction> Stloc(Type type, IEnumerable<LocalVariableInfo> locals)
            => new ElementMatch<CodeInstruction>($"{{stloc(.s) matching type {type}}}", i =>
            {
                if (!AnyStloc.Matches(i))
                    return false;
                if (i.operand is LocalBuilder local)
                {
                    return local.LocalType == type;
                }
                else
                {
                    int? localIndex = ExtractLocalIndex(i);
                    if (localIndex is null)
                        return false;
                    var providedLocal = locals.FirstOrDefault(l => l.LocalIndex == localIndex.Value);
                    return providedLocal is not null && providedLocal.LocalType == type;
                }
            });

        public static IElementMatch<CodeInstruction> Stloc<T>(IEnumerable<LocalVariableInfo> locals)
            => Stloc(typeof(T), locals);

        public static IElementMatch<CodeInstruction> Ldloca(Type type, IEnumerable<LocalVariableInfo> locals)
            => new ElementMatch<CodeInstruction>($"{{ldloc.a(.s) matching type {type}}}", i =>
            {
                if (!AnyLdloca.Matches(i))
                    return false;
                if (i.operand is LocalBuilder local)
                {
                    return local.LocalType == type;
                }
                else
                {
                    int? localIndex = ExtractLocalIndex(i);
                    if (localIndex is null)
                        return false;
                    var providedLocal = locals.FirstOrDefault(l => l.LocalIndex == localIndex.Value);
                    return providedLocal is not null && providedLocal.LocalType == type;
                }
            });

        public static IElementMatch<CodeInstruction> Ldloca<T>(IEnumerable<LocalVariableInfo> locals)
            => Ldloca(typeof(T), locals);

        public static IElementMatch<CodeInstruction> Ldfld(FieldInfo field)
            => new ElementMatch<CodeInstruction>($"{{ldfld {field}}}", i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == field);

        public static IElementMatch<CodeInstruction> Ldfld(string fieldName)
            => new ElementMatch<CodeInstruction>($"{{ldfld named {fieldName}}}", i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == fieldName);

        public static IElementMatch<CodeInstruction> Stfld(FieldInfo field)
            => new ElementMatch<CodeInstruction>($"{{ldfld {field}}}", i => i.opcode == OpCodes.Stfld && (FieldInfo)i.operand == field);

        public static IElementMatch<CodeInstruction> Stfld(string fieldName)
            => new ElementMatch<CodeInstruction>($"{{stfld named {fieldName}}}", i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == fieldName);

        public static IElementMatch<CodeInstruction> Call(MethodInfo method)
            => new ElementMatch<CodeInstruction>($"{{(any) call to method {method}}}", i => i.Calls(method));

        public static IElementMatch<CodeInstruction> Call(string methodName)
            => new ElementMatch<CodeInstruction>($"{{(any) call to method named `{methodName}`}}", i => AnyCall.Matches(i) && ((MethodInfo)i.operand).Name == methodName);

        public static IElementMatch<CodeInstruction> Newobj(ConstructorInfo constructor)
            => new ElementMatch<CodeInstruction>($"{{newobj {constructor.DeclaringType} {constructor}}}", i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == constructor);

        public static IElementMatch<CodeInstruction> Isinst(Type type)
            => new ElementMatch<CodeInstruction>($"{{isinst {type}}}", i => i.opcode == OpCodes.Isinst && (Type)i.operand == type);

        public static IElementMatch<CodeInstruction> Isinst<T>()
            => Isinst(typeof(T));

        public static IElementMatch<CodeInstruction> Instruction(OpCode? opCode, object? operand = null)
        {
            string opCodeString = opCode is null ? "<any>" : $"{opCode}";
            string operandString = operand is null ? "<any>" : $"{operand}";
            return new ElementMatch<CodeInstruction>($"{{OpCode: {opCodeString}, Operand: {operandString}}}", i =>
            {
                if (opCode is not null)
                    if (i.opcode != opCode.Value)
                        return false;
                if (operand is not null)
                    if (i.operand != operand)
                        return false;
                return true;
            });
        }

        internal static int? ExtractLocalIndex(CodeInstruction instruction)
        {
            if (instruction.opcode == OpCodes.Ldloc_0 || instruction.opcode == OpCodes.Stloc_0)
                return 0;
            else if (instruction.opcode == OpCodes.Ldloc_1 || instruction.opcode == OpCodes.Stloc_1)
                return 1;
            else if (instruction.opcode == OpCodes.Ldloc_2 || instruction.opcode == OpCodes.Stloc_2)
                return 2;
            else if (instruction.opcode == OpCodes.Ldloc_3 || instruction.opcode == OpCodes.Stloc_3)
                return 3;
            else if (instruction.opcode == OpCodes.Ldloc || instruction.opcode == OpCodes.Ldloc_S || instruction.opcode == OpCodes.Ldloca || instruction.opcode == OpCodes.Ldloca_S)
                return ExtractLocalIndex(instruction.operand);
            else
                return null;
        }

        internal static int? ExtractLocalIndex(object? operand)
        {
            if (operand is LocalBuilder local)
                return local.LocalIndex;
            else if (operand is int @int)
                return @int;
            else if (operand is sbyte @sbyte)
                return @sbyte;
            else
                return null;
        }
    }
}
