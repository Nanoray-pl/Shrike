using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Nanoray.Shrike.Harmony
{
    /// <summary>
    /// A static class hosting all kinds of pre-defined <see cref="IElementMatch{TElement}"/> objects matching <see cref="CodeInstruction"/> elements.
    /// </summary>
    public static class ILMatches
    {
        /// <summary>
        /// Matches a <c>br</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> Br { get; private set; }
            = new ElementMatch<CodeInstruction>("{br(.s)}", i => i.opcode == OpCodes.Br || i.opcode == OpCodes.Br_S);

        /// <summary>
        /// Matches a <c>brfalse</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> Brfalse { get; private set; }
            = new ElementMatch<CodeInstruction>("{brfalse(.s)}", i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S);

        /// <summary>
        /// Matches a <c>brtrue</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> Brtrue { get; private set; }
            = new ElementMatch<CodeInstruction>("{brtrue(.s)}", i => i.opcode == OpCodes.Brtrue || i.opcode == OpCodes.Brtrue_S);

        /// <summary>
        /// Matches a <c>brfalse</c>(<c>.s</c>) or <c>brtrue</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> AnyBoolBranch { get; private set; }
            = new ElementMatch<CodeInstruction>("{brfalse/brtrue(.s)}", i => Brfalse.Matches(i) || Brtrue.Matches(i));

        /// <summary>
        /// Matches a <c>beq</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> Beq { get; private set; }
            = new ElementMatch<CodeInstruction>("{beq(.s)}", i => i.opcode == OpCodes.Beq || i.opcode == OpCodes.Beq_S);

        /// <summary>
        /// Matches a <c>bne.un</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> BneUn { get; private set; }
            = new ElementMatch<CodeInstruction>("{bne.un(.s)}", i => i.opcode == OpCodes.Bne_Un || i.opcode == OpCodes.Bne_Un_S);

        /// <summary>
        /// Matches a <c>ble</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> Ble { get; private set; }
            = new ElementMatch<CodeInstruction>("{ble(.s)}", i => i.opcode == OpCodes.Ble || i.opcode == OpCodes.Ble_S);

        /// <summary>
        /// Matches a <c>ble.un</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> BleUn { get; private set; }
            = new ElementMatch<CodeInstruction>("{ble.un(.s)}", i => i.opcode == OpCodes.Ble_Un || i.opcode == OpCodes.Ble_Un_S);

        /// <summary>
        /// Matches a <c>ble</c>(<c>.un</c>)(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> AnyBle { get; private set; }
            = new ElementMatch<CodeInstruction>("{ble(.un)(.s)}", i => Ble.Matches(i) || BleUn.Matches(i));

        /// <summary>
        /// Matches a <c>bge</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> Bge { get; private set; }
            = new ElementMatch<CodeInstruction>("{bge(.s)}", i => i.opcode == OpCodes.Bge || i.opcode == OpCodes.Bge_S);

        /// <summary>
        /// Matches a <c>bge.un</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> BgeUn { get; private set; }
            = new ElementMatch<CodeInstruction>("{bge.un(.s)}", i => i.opcode == OpCodes.Bge_Un || i.opcode == OpCodes.Bge_Un_S);

        /// <summary>
        /// Matches a <c>bge</c>(<c>.un</c>)(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> AnyBge { get; private set; }
            = new ElementMatch<CodeInstruction>("{bge(.un)(.s)}", i => Bge.Matches(i) || BgeUn.Matches(i));

        /// <summary>
        /// Matches a <c>blt</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> Blt { get; private set; }
            = new ElementMatch<CodeInstruction>("{blt(.s)}", i => i.opcode == OpCodes.Blt || i.opcode == OpCodes.Blt_S);

        /// <summary>
        /// Matches a <c>blt.un</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> BltUn { get; private set; }
            = new ElementMatch<CodeInstruction>("{blt.un(.s)}", i => i.opcode == OpCodes.Blt_Un || i.opcode == OpCodes.Blt_Un_S);

        /// <summary>
        /// Matches a <c>blt</c>(<c>.un</c>)(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> AnyBlt { get; private set; }
            = new ElementMatch<CodeInstruction>("{blt(.un)(.s)}", i => Blt.Matches(i) || BltUn.Matches(i));

        /// <summary>
        /// Matches a <c>bgt</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> Bgt { get; private set; }
            = new ElementMatch<CodeInstruction>("{bgt(.s)}", i => i.opcode == OpCodes.Bgt || i.opcode == OpCodes.Bgt_S);

        /// <summary>
        /// Matches a <c>bgt.un</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> BgtUn { get; private set; }
            = new ElementMatch<CodeInstruction>("{bgt.un(.s)}", i => i.opcode == OpCodes.Bgt_Un || i.opcode == OpCodes.Bgt_Un_S);

        /// <summary>
        /// Matches a <c>bgt</c>(<c>.un</c>)(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> AnyBgt { get; private set; }
            = new ElementMatch<CodeInstruction>("{bgt(.un)(.s)}", i => Bgt.Matches(i) || BgtUn.Matches(i));

        /// <summary>
        /// Matches a <c>br</c>(<c>.s</c>) or <c>beq</c>(<c>.s</c>) or <c>bne.un</c>(<c>.s</c>) or <c>brfalse</c>(<c>.s</c>) or <c>brtrue</c>(<c>.s</c>) or <c>ble</c>(<c>.un</c>)(<c>.s</c>) or <c>bge</c>(<c>.un</c>)(<c>.s</c>) or <c>blt</c>(<c>.un</c>)(<c>.s</c>) or <c>bgt</c>(<c>.un</c>)(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> AnyBranch { get; private set; }
            = new ElementMatch<CodeInstruction>("{any branch}", i => Br.Matches(i) || Beq.Matches(i) || BneUn.Matches(i) || AnyBoolBranch.Matches(i) || AnyBle.Matches(i) || AnyBge.Matches(i) || AnyBlt.Matches(i) || AnyBgt.Matches(i));

        /// <summary>
        /// Matches an <c>ldarg</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> AnyLdarg { get; private set; }
            = new ElementMatch<CodeInstruction>("{any ldarg}", i => i.IsLdarg());

        /// <summary>
        /// Matches an <c>ldloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> AnyLdloc { get; private set; }
            = new ElementMatch<CodeInstruction>("{any ldloc}", i => i.opcode == OpCodes.Ldloc || i.opcode == OpCodes.Ldloc_S || i.opcode == OpCodes.Ldloc_0 || i.opcode == OpCodes.Ldloc_1 || i.opcode == OpCodes.Ldloc_2 || i.opcode == OpCodes.Ldloc_3);

        /// <summary>
        /// Matches an <c>stloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> AnyStloc { get; private set; }
            = new ElementMatch<CodeInstruction>("{any stloc}", i => i.opcode == OpCodes.Stloc || i.opcode == OpCodes.Stloc_S || i.opcode == OpCodes.Stloc_0 || i.opcode == OpCodes.Stloc_1 || i.opcode == OpCodes.Stloc_2 || i.opcode == OpCodes.Stloc_3);

        /// <summary>
        /// Matches an <c>ldloca</c>(<c>.s</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> AnyLdloca { get; private set; }
            = new ElementMatch<CodeInstruction>("{any ldloc}", i => i.opcode == OpCodes.Ldloca || i.opcode == OpCodes.Ldloca_S);

        /// <summary>
        /// Matches a <c>call</c>(<c>virt</c>) instruction.
        /// </summary>
        public static IElementMatch<CodeInstruction> AnyCall { get; private set; }
            = new ElementMatch<CodeInstruction>("{call(virt)}", i => i.opcode == OpCodes.Call || i.opcode == OpCodes.Callvirt);

        /// <summary>
        /// Matches an <c>ldc.i4</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>/<c>.4</c>/<c>.5</c>/<c>.6</c>/<c>.7</c>/<c>.8</c>/<c>.m1</c>) instruction.
        /// </summary>
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

        /// <summary>
        /// Matches an <c>ldarg</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction with the given index.
        /// </summary>
        /// <param name="index">The argument index.</param>
        public static IElementMatch<CodeInstruction> Ldarg(int index)
            => new ElementMatch<CodeInstruction>($"{{ldarg: {index}}}", i => i.IsLdarg(index));

        /// <summary>
        /// Matches an <c>ldc.i4</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>/<c>.4</c>/<c>.5</c>/<c>.6</c>/<c>.7</c>/<c>.8</c>/<c>.m1</c>) instruction with a given value.
        /// </summary>
        /// <param name="value">The value.</param>
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

        /// <summary>
        /// Matches an <c>ldstr</c> instruction with a given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public static IElementMatch<CodeInstruction> Ldstr(string value)
            => new ElementMatch<CodeInstruction>($"{{ldstr `{value}`}}", i => i.opcode == OpCodes.Ldstr && (string)i.operand == value);

        /// <summary>
        /// Matches an <c>ldloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="locals">The method local variable list.</param>
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

        /// <summary>
        /// Matches an <c>ldloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="locals">The method local variable list.</param>
        public static IElementMatch<CodeInstruction> Ldloc<T>(IEnumerable<LocalVariableInfo> locals)
            => Ldloc(typeof(T), locals);

        /// <summary>
        /// Matches an <c>stloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="locals">The method local variable list.</param>
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

        /// <summary>
        /// Matches an <c>stloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="locals">The method local variable list.</param>
        public static IElementMatch<CodeInstruction> Stloc<T>(IEnumerable<LocalVariableInfo> locals)
            => Stloc(typeof(T), locals);

        /// <summary>
        /// Matches an <c>ldloca</c>(<c>.s</c>) instruction matching the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="locals">The method local variable list.</param>
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

        /// <summary>
        /// Matches an <c>ldloca</c>(<c>.s</c>) instruction matching the given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="locals">The method local variable list.</param>
        public static IElementMatch<CodeInstruction> Ldloca<T>(IEnumerable<LocalVariableInfo> locals)
            => Ldloca(typeof(T), locals);

        /// <summary>
        /// Matches an <c>ldsfld</c> instruction matching the given field.
        /// </summary>
        /// <param name="field">The field.</param>
        public static IElementMatch<CodeInstruction> Ldsfld(FieldInfo field)
            => new ElementMatch<CodeInstruction>($"{{ldsfld {field}}}", i => i.opcode == OpCodes.Ldsfld && (FieldInfo)i.operand == field);

        /// <summary>
        /// Matches an <c>ldsfld</c> instruction matching the given field name.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        public static IElementMatch<CodeInstruction> Ldsfld(string fieldName)
            => new ElementMatch<CodeInstruction>($"{{ldsfld named {fieldName}}}", i => i.opcode == OpCodes.Ldsfld && ((FieldInfo)i.operand).Name == fieldName);

        /// <summary>
        /// Matches an <c>stsfld</c> instruction matching the given field.
        /// </summary>
        /// <param name="field">The field.</param>
        public static IElementMatch<CodeInstruction> Stsfld(FieldInfo field)
            => new ElementMatch<CodeInstruction>($"{{stsfld {field}}}", i => i.opcode == OpCodes.Stsfld && (FieldInfo)i.operand == field);

        /// <summary>
        /// Matches an <c>stsfld</c> instruction matching the given field name.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        public static IElementMatch<CodeInstruction> Stsfld(string fieldName)
            => new ElementMatch<CodeInstruction>($"{{stsfld named {fieldName}}}", i => i.opcode == OpCodes.Stsfld && ((FieldInfo)i.operand).Name == fieldName);

        /// <summary>
        /// Matches an <c>ldfld</c> instruction matching the given field.
        /// </summary>
        /// <param name="field">The field.</param>
        public static IElementMatch<CodeInstruction> Ldfld(FieldInfo field)
            => new ElementMatch<CodeInstruction>($"{{ldfld {field}}}", i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == field);

        /// <summary>
        /// Matches an <c>ldfld</c> instruction matching the given field name.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        public static IElementMatch<CodeInstruction> Ldfld(string fieldName)
            => new ElementMatch<CodeInstruction>($"{{ldfld named {fieldName}}}", i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == fieldName);

        /// <summary>
        /// Matches an <c>stfld</c> instruction matching the given field.
        /// </summary>
        /// <param name="field">The field.</param>
        public static IElementMatch<CodeInstruction> Stfld(FieldInfo field)
            => new ElementMatch<CodeInstruction>($"{{stfld {field}}}", i => i.opcode == OpCodes.Stfld && (FieldInfo)i.operand == field);

        /// <summary>
        /// Matches an <c>stfld</c> instruction matching the given field name.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        public static IElementMatch<CodeInstruction> Stfld(string fieldName)
            => new ElementMatch<CodeInstruction>($"{{stfld named {fieldName}}}", i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == fieldName);

        /// <summary>
        /// Matches a <c>call</c>(<c>virt</c>) instruction matching the given method.
        /// </summary>
        /// <param name="method">The method.</param>
        public static IElementMatch<CodeInstruction> Call(MethodInfo method)
            => new ElementMatch<CodeInstruction>($"{{(any) call to method {method}}}", i => i.Calls(method));

        /// <summary>
        /// Matches a <c>call</c>(<c>virt</c>) instruction matching the given method name.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        public static IElementMatch<CodeInstruction> Call(string methodName)
            => new ElementMatch<CodeInstruction>($"{{(any) call to method named `{methodName}`}}", i => AnyCall.Matches(i) && ((MethodInfo)i.operand).Name == methodName);

        /// <summary>
        /// Matches a <c>newobj</c> instruction matching the given constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        public static IElementMatch<CodeInstruction> Newobj(ConstructorInfo constructor)
            => new ElementMatch<CodeInstruction>($"{{newobj {constructor.DeclaringType} {constructor}}}", i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == constructor);

        /// <summary>
        /// Matches an <c>isinst</c> instruction matching the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        public static IElementMatch<CodeInstruction> Isinst(Type type)
            => new ElementMatch<CodeInstruction>($"{{isinst {type}}}", i => i.opcode == OpCodes.Isinst && (Type)i.operand == type);

        /// <summary>
        /// Matches an <c>isinst</c> instruction matching the given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        public static IElementMatch<CodeInstruction> Isinst<T>()
            => Isinst(typeof(T));

        /// <summary>
        /// Matches an instruction with the given opcode and operand.
        /// </summary>
        /// <param name="opCode">The opcode.</param>
        /// <param name="operand">The operand.</param>
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
