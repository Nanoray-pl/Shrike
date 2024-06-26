using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Nanoray.Shrike.Harmony;

/// <summary>
/// A static class hosting all kinds of pre-defined <see cref="ElementMatch{TElement}"/> objects matching <see cref="CodeInstruction"/> elements.
/// </summary>
public static class ILMatches
{
    /// <summary>
    /// Matches a <c>br</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> Br { get; private set; }
        = new("{br(.s)}", i => i.opcode == OpCodes.Br || i.opcode == OpCodes.Br_S);

    /// <summary>
    /// Matches a <c>brfalse</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> Brfalse { get; private set; }
        = new("{brfalse(.s)}", i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S);

    /// <summary>
    /// Matches a <c>brtrue</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> Brtrue { get; private set; }
        = new("{brtrue(.s)}", i => i.opcode == OpCodes.Brtrue || i.opcode == OpCodes.Brtrue_S);

    /// <summary>
    /// Matches a <c>brfalse</c>(<c>.s</c>) or <c>brtrue</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyBoolBranch { get; private set; }
        = new("{brfalse/brtrue(.s)}", i => Brfalse.Matches(i) || Brtrue.Matches(i));

    /// <summary>
    /// Matches a <c>beq</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> Beq { get; private set; }
        = new("{beq(.s)}", i => i.opcode == OpCodes.Beq || i.opcode == OpCodes.Beq_S);

    /// <summary>
    /// Matches a <c>bne.un</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> BneUn { get; private set; }
        = new("{bne.un(.s)}", i => i.opcode == OpCodes.Bne_Un || i.opcode == OpCodes.Bne_Un_S);

    /// <summary>
    /// Matches a <c>ble</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> Ble { get; private set; }
        = new("{ble(.s)}", i => i.opcode == OpCodes.Ble || i.opcode == OpCodes.Ble_S);

    /// <summary>
    /// Matches a <c>ble.un</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> BleUn { get; private set; }
        = new("{ble.un(.s)}", i => i.opcode == OpCodes.Ble_Un || i.opcode == OpCodes.Ble_Un_S);

    /// <summary>
    /// Matches a <c>ble</c>(<c>.un</c>)(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyBle { get; private set; }
        = new("{ble(.un)(.s)}", i => Ble.Matches(i) || BleUn.Matches(i));

    /// <summary>
    /// Matches a <c>bge</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> Bge { get; private set; }
        = new("{bge(.s)}", i => i.opcode == OpCodes.Bge || i.opcode == OpCodes.Bge_S);

    /// <summary>
    /// Matches a <c>bge.un</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> BgeUn { get; private set; }
        = new("{bge.un(.s)}", i => i.opcode == OpCodes.Bge_Un || i.opcode == OpCodes.Bge_Un_S);

    /// <summary>
    /// Matches a <c>bge</c>(<c>.un</c>)(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyBge { get; private set; }
        = new("{bge(.un)(.s)}", i => Bge.Matches(i) || BgeUn.Matches(i));

    /// <summary>
    /// Matches a <c>blt</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> Blt { get; private set; }
        = new("{blt(.s)}", i => i.opcode == OpCodes.Blt || i.opcode == OpCodes.Blt_S);

    /// <summary>
    /// Matches a <c>blt.un</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> BltUn { get; private set; }
        = new("{blt.un(.s)}", i => i.opcode == OpCodes.Blt_Un || i.opcode == OpCodes.Blt_Un_S);

    /// <summary>
    /// Matches a <c>blt</c>(<c>.un</c>)(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyBlt { get; private set; }
        = new("{blt(.un)(.s)}", i => Blt.Matches(i) || BltUn.Matches(i));

    /// <summary>
    /// Matches a <c>bgt</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> Bgt { get; private set; }
        = new("{bgt(.s)}", i => i.opcode == OpCodes.Bgt || i.opcode == OpCodes.Bgt_S);

    /// <summary>
    /// Matches a <c>bgt.un</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> BgtUn { get; private set; }
        = new("{bgt.un(.s)}", i => i.opcode == OpCodes.Bgt_Un || i.opcode == OpCodes.Bgt_Un_S);

    /// <summary>
    /// Matches a <c>bgt</c>(<c>.un</c>)(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyBgt { get; private set; }
        = new("{bgt(.un)(.s)}", i => Bgt.Matches(i) || BgtUn.Matches(i));

    /// <summary>
    /// Matches a <c>br</c>(<c>.s</c>) or <c>beq</c>(<c>.s</c>) or <c>bne.un</c>(<c>.s</c>) or <c>brfalse</c>(<c>.s</c>) or <c>brtrue</c>(<c>.s</c>) or <c>ble</c>(<c>.un</c>)(<c>.s</c>) or <c>bge</c>(<c>.un</c>)(<c>.s</c>) or <c>blt</c>(<c>.un</c>)(<c>.s</c>) or <c>bgt</c>(<c>.un</c>)(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyBranch { get; private set; }
        = new("{any branch}", i => Br.Matches(i) || Beq.Matches(i) || BneUn.Matches(i) || AnyBoolBranch.Matches(i) || AnyBle.Matches(i) || AnyBge.Matches(i) || AnyBlt.Matches(i) || AnyBgt.Matches(i));

    /// <summary>
    /// Matches an <c>ldarg</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyLdarg { get; private set; }
        = new("{any ldarg}", i => i.IsLdarg());

    /// <summary>
    /// Matches an <c>ldloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyLdloc { get; private set; }
        = new("{any ldloc}", i => i.opcode == OpCodes.Ldloc || i.opcode == OpCodes.Ldloc_S || i.opcode == OpCodes.Ldloc_0 || i.opcode == OpCodes.Ldloc_1 || i.opcode == OpCodes.Ldloc_2 || i.opcode == OpCodes.Ldloc_3);

    /// <summary>
    /// Matches an <c>stloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyStloc { get; private set; }
        = new("{any stloc}", i => i.opcode == OpCodes.Stloc || i.opcode == OpCodes.Stloc_S || i.opcode == OpCodes.Stloc_0 || i.opcode == OpCodes.Stloc_1 || i.opcode == OpCodes.Stloc_2 || i.opcode == OpCodes.Stloc_3);

    /// <summary>
    /// Matches an <c>ldloca</c>(<c>.s</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyLdloca { get; private set; }
        = new("{any ldloc}", i => i.opcode == OpCodes.Ldloca || i.opcode == OpCodes.Ldloca_S);

    /// <summary>
    /// Matches a <c>call</c>(<c>virt</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyCall { get; private set; }
        = new("{call(virt)}", i => i.opcode == OpCodes.Call || i.opcode == OpCodes.Callvirt);

    /// <summary>
    /// Matches an <c>ldc.i4</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>/<c>.4</c>/<c>.5</c>/<c>.6</c>/<c>.7</c>/<c>.8</c>/<c>.m1</c>) instruction.
    /// </summary>
    public static ElementMatch<CodeInstruction> AnyLdcI4 { get; private set; }
        = new("{any ldc.i4}", i =>
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
    public static ElementMatch<CodeInstruction> Ldarg(int index)
        => new($"{{ldarg: {index}}}", i => i.IsLdarg(index));

    /// <summary>
    /// Matches an <c>ldc.i4</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>/<c>.4</c>/<c>.5</c>/<c>.6</c>/<c>.7</c>/<c>.8</c>/<c>.m1</c>) instruction with a given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public static ElementMatch<CodeInstruction> LdcI4(int value)
        => new($"{{ldc.i4: {value}}}", i =>
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
    /// Matches an <c>ldc.r4</c> instruction with a given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public static ElementMatch<CodeInstruction> LdcR4(float value)
        => new($"{{ldc.r4: {value}}}", i => i.opcode == OpCodes.Ldc_R4 && (float)i.operand == value);

    /// <summary>
    /// Matches an <c>ldc.r8</c> instruction with a given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public static ElementMatch<CodeInstruction> LdcR8(double value)
        => new($"{{ldc.r8: {value}}}", i => i.opcode == OpCodes.Ldc_R8 && (double)i.operand == value);

    /// <summary>
    /// Matches an <c>ldstr</c> instruction with a given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public static ElementMatch<CodeInstruction> Ldstr(string value)
        => new($"{{ldstr: `{value}`}}", i => i.opcode == OpCodes.Ldstr && (string)i.operand == value);

    /// <summary>
    /// Matches an <c>ldloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given local index.
    /// </summary>
    /// <param name="index">The index.</param>
    public static ElementMatch<CodeInstruction> Ldloc(int index)
        => new($"{{ldloc(.s) {index}}}", i => AnyLdloc.Matches(i) && i.TryGetLocalIndex(out int localIndex) && localIndex == index);

    /// <summary>
    /// Matches an <c>ldloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given instruction.
    /// </summary>
    /// <param name="instruction">The instruction to match against.</param>
    public static ElementMatch<CodeInstruction> Ldloc(CodeInstruction instruction)
        => new($"{{ldloc matching {instruction}}}", i => Ldloc(i).Matches(i));

    /// <summary>
    /// Matches an <c>ldloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="locals">The method local variable list.</param>
    public static ElementMatch<CodeInstruction> Ldloc(Type type, IEnumerable<LocalVariableInfo> locals)
        => new($"{{ldloc(.s) matching type {type}}}", i =>
        {
            if (!AnyLdloc.Matches(i))
                return false;

            if (i.operand is LocalBuilder local)
                return local.LocalType == type;
            else if (i.TryGetLocalIndex(out int localIndex))
                return locals.FirstOrDefault(l => l.LocalIndex == localIndex) is { } providedLocal && providedLocal.LocalType == type;
            else
                return false;
        });

    /// <summary>
    /// Matches an <c>ldloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="method">The method to match local variables against.</param>
    public static ElementMatch<CodeInstruction> Ldloc(Type type, MethodBase method)
        => Ldloc(type, method.GetMethodBody()?.LocalVariables ?? Enumerable.Empty<LocalVariableInfo>());

    /// <summary>
    /// Matches an <c>ldloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="method">The method to match local variables against.</param>
    public static ElementMatch<CodeInstruction> Ldloc<T>(MethodBase method)
        => Ldloc(typeof(T), method);

    /// <summary>
    /// Matches an <c>ldloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="locals">The method local variable list.</param>
    public static ElementMatch<CodeInstruction> Ldloc<T>(IEnumerable<LocalVariableInfo> locals)
        => Ldloc(typeof(T), locals);

    /// <summary>
    /// Matches an <c>stloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given local index.
    /// </summary>
    /// <param name="index">The index.</param>
    public static ElementMatch<CodeInstruction> Stloc(int index)
        => new($"{{stloc(.s) {index}}}", i => AnyStloc.Matches(i) && i.TryGetLocalIndex(out int localIndex) && localIndex == index);

    /// <summary>
    /// Matches an <c>stloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given instruction.
    /// </summary>
    /// <param name="instruction">The instruction to match against.</param>
    public static ElementMatch<CodeInstruction> Stloc(CodeInstruction instruction)
        => new($"{{stloc matching {instruction}}}", i => Stloc(i).Matches(i));

    /// <summary>
    /// Matches an <c>stloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="locals">The method local variable list.</param>
    public static ElementMatch<CodeInstruction> Stloc(Type type, IEnumerable<LocalVariableInfo> locals)
        => new($"{{stloc(.s) matching type {type}}}", i =>
        {
            if (!AnyStloc.Matches(i))
                return false;

            if (i.operand is LocalBuilder local)
                return local.LocalType == type;
            else if (i.TryGetLocalIndex(out int localIndex))
                return locals.FirstOrDefault(l => l.LocalIndex == localIndex) is { } providedLocal && providedLocal.LocalType == type;
            else
                return false;
        });

    /// <summary>
    /// Matches an <c>stloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="method">The method to match local variables against.</param>
    public static ElementMatch<CodeInstruction> Stloc(Type type, MethodBase method)
        => Stloc(type, method.GetMethodBody()?.LocalVariables ?? Enumerable.Empty<LocalVariableInfo>());

    /// <summary>
    /// Matches an <c>stloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="method">The method to match local variables against.</param>
    public static ElementMatch<CodeInstruction> Stloc<T>(MethodBase method)
        => Stloc(typeof(T), method);

    /// <summary>
    /// Matches an <c>stloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="locals">The method local variable list.</param>
    public static ElementMatch<CodeInstruction> Stloc<T>(IEnumerable<LocalVariableInfo> locals)
        => Stloc(typeof(T), locals);

    /// <summary>
    /// Matches an <c>ldloca</c>(<c>.s</c>) instruction matching the given local index.
    /// </summary>
    /// <param name="index">The index.</param>
    public static ElementMatch<CodeInstruction> Ldloca(int index)
        => new($"{{ldloc.a(.s) {index}}}", i => AnyLdloca.Matches(i) && i.TryGetLocalIndex(out int localIndex) && localIndex == index);

    /// <summary>
    /// Matches an <c>ldloc</c>(<c>.s</c>/<c>.0</c>/<c>.1</c>/<c>.2</c>/<c>.3</c>) instruction matching the given instruction.
    /// </summary>
    /// <param name="instruction">The instruction to match against.</param>
    public static ElementMatch<CodeInstruction> Ldloca(CodeInstruction instruction)
        => new($"{{ldloc.a(.s) matching {instruction}}}", i => Ldloca(i).Matches(i));

    /// <summary>
    /// Matches an <c>ldloca</c>(<c>.s</c>) instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="locals">The method local variable list.</param>
    public static ElementMatch<CodeInstruction> Ldloca(Type type, IEnumerable<LocalVariableInfo> locals)
        => new($"{{ldloc.a(.s) matching type {type}}}", i =>
        {
            if (!AnyLdloca.Matches(i))
                return false;

            if (i.operand is LocalBuilder local)
                return local.LocalType == type;
            else if (i.TryGetLocalIndex(out int localIndex))
                return locals.FirstOrDefault(l => l.LocalIndex == localIndex) is { } providedLocal && providedLocal.LocalType == type;
            else
                return false;
        });

    /// <summary>
    /// Matches an <c>ldloca</c>(<c>.s</c>) instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="method">The method to match local variables against.</param>
    public static ElementMatch<CodeInstruction> Ldloca(Type type, MethodBase method)
        => Ldloca(type, method.GetMethodBody()?.LocalVariables ?? Enumerable.Empty<LocalVariableInfo>());

    /// <summary>
    /// Matches an <c>ldloca</c>(<c>.s</c>) instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="method">The method to match local variables against.</param>
    public static ElementMatch<CodeInstruction> Ldloca<T>(MethodBase method)
        => Ldloca(typeof(T), method);

    /// <summary>
    /// Matches an <c>ldloca</c>(<c>.s</c>) instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="locals">The method local variable list.</param>
    public static ElementMatch<CodeInstruction> Ldloca<T>(IEnumerable<LocalVariableInfo> locals)
        => Ldloca(typeof(T), locals);

    /// <summary>
    /// Matches an <c>ldsfld</c> instruction matching the given field.
    /// </summary>
    /// <param name="field">The field.</param>
    public static ElementMatch<CodeInstruction> Ldsfld(FieldInfo field)
        => new($"{{ldsfld {field}}}", i => i.opcode == OpCodes.Ldsfld && (FieldInfo)i.operand == field);

    /// <summary>
    /// Matches an <c>ldsfld</c> instruction matching the given field name.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    public static ElementMatch<CodeInstruction> Ldsfld(string fieldName)
        => new($"{{ldsfld named {fieldName}}}", i => i.opcode == OpCodes.Ldsfld && ((FieldInfo)i.operand).Name == fieldName);

    /// <summary>
    /// Matches an <c>ldsfld</c> instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    public static ElementMatch<CodeInstruction> Ldsfld(Type type)
        => new($"{{ldsfld matching type {type}}}", i => i.opcode == OpCodes.Ldsfld && ((FieldInfo)i.operand).FieldType == type);

    /// <summary>
    /// Matches an <c>ldsfld</c> instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public static ElementMatch<CodeInstruction> Ldsfld<T>()
        => Ldsfld(typeof(T));

    /// <summary>
    /// Matches an <c>stsfld</c> instruction matching the given field.
    /// </summary>
    /// <param name="field">The field.</param>
    public static ElementMatch<CodeInstruction> Stsfld(FieldInfo field)
        => new($"{{stsfld {field}}}", i => i.opcode == OpCodes.Stsfld && (FieldInfo)i.operand == field);

    /// <summary>
    /// Matches an <c>stsfld</c> instruction matching the given field name.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    public static ElementMatch<CodeInstruction> Stsfld(string fieldName)
        => new($"{{stsfld named {fieldName}}}", i => i.opcode == OpCodes.Stsfld && ((FieldInfo)i.operand).Name == fieldName);

    /// <summary>
    /// Matches an <c>stsfld</c> instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    public static ElementMatch<CodeInstruction> Stsfld(Type type)
        => new($"{{stsfld matching type {type}}}", i => i.opcode == OpCodes.Stsfld && ((FieldInfo)i.operand).FieldType == type);

    /// <summary>
    /// Matches an <c>stsfld</c> instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public static ElementMatch<CodeInstruction> Stsfld<T>()
        => Stsfld(typeof(T));

    /// <summary>
    /// Matches an <c>ldfld</c> instruction matching the given field.
    /// </summary>
    /// <param name="field">The field.</param>
    public static ElementMatch<CodeInstruction> Ldfld(FieldInfo field)
        => new($"{{ldfld {field}}}", i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == field);

    /// <summary>
    /// Matches an <c>ldfld</c> instruction matching the given field name.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    public static ElementMatch<CodeInstruction> Ldfld(string fieldName)
        => new($"{{ldfld named {fieldName}}}", i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == fieldName);

    /// <summary>
    /// Matches an <c>ldfld</c> instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    public static ElementMatch<CodeInstruction> Ldfld(Type type)
        => new($"{{ldfld matching type {type}}}", i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).FieldType == type);

    /// <summary>
    /// Matches an <c>ldfld</c> instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public static ElementMatch<CodeInstruction> Ldfld<T>()
        => Ldfld(typeof(T));

    /// <summary>
    /// Matches an <c>stfld</c> instruction matching the given field.
    /// </summary>
    /// <param name="field">The field.</param>
    public static ElementMatch<CodeInstruction> Stfld(FieldInfo field)
        => new($"{{stfld {field}}}", i => i.opcode == OpCodes.Stfld && (FieldInfo)i.operand == field);

    /// <summary>
    /// Matches an <c>stfld</c> instruction matching the given field name.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    public static ElementMatch<CodeInstruction> Stfld(string fieldName)
        => new($"{{stfld named {fieldName}}}", i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == fieldName);

    /// <summary>
    /// Matches an <c>stfld</c> instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    public static ElementMatch<CodeInstruction> Stfld(Type type)
        => new($"{{stfld matching type {type}}}", i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).FieldType == type);

    /// <summary>
    /// Matches an <c>stfld</c> instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public static ElementMatch<CodeInstruction> Stfld<T>()
        => Stfld(typeof(T));

    /// <summary>
    /// Matches an <c>ldfld.a</c> instruction matching the given field.
    /// </summary>
    /// <param name="field">The field.</param>
    public static ElementMatch<CodeInstruction> Ldflda(FieldInfo field)
        => new($"{{ldfld.a {field}}}", i => i.opcode == OpCodes.Ldflda && (FieldInfo)i.operand == field);

    /// <summary>
    /// Matches an <c>ldfld.a</c> instruction matching the given field name.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    public static ElementMatch<CodeInstruction> Ldflda(string fieldName)
        => new($"{{ldfld.a named {fieldName}}}", i => i.opcode == OpCodes.Ldflda && ((FieldInfo)i.operand).Name == fieldName);

    /// <summary>
    /// Matches an <c>ldfld.a</c> instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    public static ElementMatch<CodeInstruction> Ldflda(Type type)
        => new($"{{ldflda matching type {type}}}", i => i.opcode == OpCodes.Ldflda && ((FieldInfo)i.operand).FieldType == type);

    /// <summary>
    /// Matches an <c>ldfld.a</c> instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public static ElementMatch<CodeInstruction> Ldflda<T>()
        => Ldflda(typeof(T));

    /// <summary>
    /// Matches a <c>call</c>(<c>virt</c>) instruction matching the given method.
    /// </summary>
    /// <param name="method">The method.</param>
    public static ElementMatch<CodeInstruction> Call(MethodBase method)
        => new($"{{(any) call to method {method}}}", i => (i.opcode == OpCodes.Call || i.opcode == OpCodes.Callvirt) && Equals(i.operand, method));

    /// <summary>
    /// Matches a <c>call</c>(<c>virt</c>) instruction matching the given method name.
    /// </summary>
    /// <param name="methodName">The method name.</param>
    public static ElementMatch<CodeInstruction> Call(string methodName)
        => new($"{{(any) call to method named `{methodName}`}}", i => AnyCall.Matches(i) && (i.operand as MethodBase)?.Name == methodName);

    /// <summary>
    /// Matches a <c>newobj</c> instruction matching the given constructor.
    /// </summary>
    /// <param name="constructor">The constructor.</param>
    public static ElementMatch<CodeInstruction> Newobj(ConstructorInfo constructor)
        => new($"{{newobj {constructor.DeclaringType} {constructor}}}", i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == constructor);

    /// <summary>
    /// Matches an <c>isinst</c> instruction matching the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    public static ElementMatch<CodeInstruction> Isinst(Type type)
        => new($"{{isinst {type}}}", i => i.opcode == OpCodes.Isinst && (Type)i.operand == type);

    /// <summary>
    /// Matches an <c>isinst</c> instruction matching the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public static ElementMatch<CodeInstruction> Isinst<T>()
        => Isinst(typeof(T));

    /// <summary>
    /// Matches an instruction with the given opcode and operand.
    /// </summary>
    /// <param name="opCode">The opcode.</param>
    /// <param name="operand">The operand.</param>
    public static ElementMatch<CodeInstruction> Instruction(OpCode? opCode, object? operand = null)
    {
        string opCodeString = opCode is null ? "<any>" : $"{opCode}";
        string operandString = operand is null ? "<any>" : $"{operand}";
        return new($"{{OpCode: {opCodeString}, Operand: {operandString}}}", i =>
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
}
