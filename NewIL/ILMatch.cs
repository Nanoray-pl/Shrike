using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Shockah.CommonModCode.IL
{
	public readonly struct ILMatch
	{
		internal string? AutoAnchor { get; init; }
		private string Description { get; init; }
		private Func<CodeInstruction, bool> Closure { get; init; }

		private ILMatch(string? autoAnchor, string description, Func<CodeInstruction, bool> closure)
		{
			this.AutoAnchor = autoAnchor;
			this.Description = description;
			this.Closure = closure;
		}

		public ILMatch(string description, Func<CodeInstruction, bool> closure)
		{
			this.AutoAnchor = null;
			this.Description = description;
			this.Closure = closure;
		}

		public ILMatch(Expression<Func<CodeInstruction, bool>> closure)
		{
			this.AutoAnchor = null;
			this.Description = $"{closure}";
			this.Closure = closure.Compile();
		}

		public override string ToString()
			=> Description;

		public bool Matches(CodeInstruction instruction)
			=> Closure(instruction);

		public ILMatch WithAutoAnchor(string autoAnchor)
			=> new(autoAnchor, Description, Closure);

		public ILMatch WithAutoAnchor(out string autoAnchor)
		{
			autoAnchor = $"{Guid.NewGuid()}";
			return WithAutoAnchor(autoAnchor);
		}

		public static implicit operator ILMatch(OpCode opCode)
			=> ILMatches.Instruction(opCode);

		public static implicit operator ILMatch(MethodInfo method)
			=> ILMatches.Call(method);

		public static implicit operator ILMatch(ConstructorInfo constructor)
			=> ILMatches.Newobj(constructor);

		public static implicit operator ILMatch(int value)
			=> ILMatches.LdcI4(value);

		public static implicit operator ILMatch(string value)
			=> ILMatches.Ldstr(value);
	}

	public static class ILMatches
	{
		public static ILMatch Br { get; private set; }
			= new("{br(.s)}", i => i.opcode == OpCodes.Br || i.opcode == OpCodes.Br_S);

		public static ILMatch Brfalse { get; private set; }
			= new("{brfalse(.s)}", i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S);

		public static ILMatch Brtrue { get; private set; }
			= new("{brtrue(.s)}", i => i.opcode == OpCodes.Brtrue || i.opcode == OpCodes.Brtrue_S);

		public static ILMatch AnyBoolBranch { get; private set; }
			= new("{brfalse/brtrue(.s)}", i => Brfalse.Matches(i) || Brtrue.Matches(i));

		public static ILMatch Beq { get; private set; }
			= new("{beq(.s)}", i => i.opcode == OpCodes.Beq || i.opcode == OpCodes.Beq_S);

		public static ILMatch BneUn { get; private set; }
			= new("{bne.un(.s)}", i => i.opcode == OpCodes.Bne_Un || i.opcode == OpCodes.Bne_Un_S);

		public static ILMatch Ble { get; private set; }
			= new("{ble(.s)}", i => i.opcode == OpCodes.Ble || i.opcode == OpCodes.Ble_S);

		public static ILMatch BleUn { get; private set; }
			= new("{ble.un(.s)}", i => i.opcode == OpCodes.Ble_Un || i.opcode == OpCodes.Ble_Un_S);

		public static ILMatch AnyBle { get; private set; }
			= new("{ble(.un)(.s)}", i => Ble.Matches(i) || BleUn.Matches(i));

		public static ILMatch Bge { get; private set; }
			= new("{bge(.s)}", i => i.opcode == OpCodes.Bge || i.opcode == OpCodes.Bge_S);

		public static ILMatch BgeUn { get; private set; }
			= new("{bge.un(.s)}", i => i.opcode == OpCodes.Bge_Un || i.opcode == OpCodes.Bge_Un_S);

		public static ILMatch AnyBge { get; private set; }
			= new("{bge(.un)(.s)}", i => Bge.Matches(i) || BgeUn.Matches(i));

		public static ILMatch Blt { get; private set; }
			= new("{blt(.s)}", i => i.opcode == OpCodes.Blt || i.opcode == OpCodes.Blt_S);

		public static ILMatch BltUn { get; private set; }
			= new("{blt.un(.s)}", i => i.opcode == OpCodes.Blt_Un || i.opcode == OpCodes.Blt_Un_S);

		public static ILMatch AnyBlt { get; private set; }
			= new("{blt(.un)(.s)}", i => Blt.Matches(i) || BltUn.Matches(i));

		public static ILMatch Bgt { get; private set; }
			= new("{bgt(.s)}", i => i.opcode == OpCodes.Bgt || i.opcode == OpCodes.Bgt_S);

		public static ILMatch BgtUn { get; private set; }
			= new("{bgt.un(.s)}", i => i.opcode == OpCodes.Bgt_Un || i.opcode == OpCodes.Bgt_Un_S);

		public static ILMatch AnyBgt { get; private set; }
			= new("{bgt(.un)(.s)}", i => Bgt.Matches(i) || BgtUn.Matches(i));

		public static ILMatch AnyBranch { get; private set; }
			= new("{any branch}", i => Br.Matches(i) || Beq.Matches(i) || BneUn.Matches(i) || AnyBoolBranch.Matches(i) || AnyBle.Matches(i) || AnyBge.Matches(i) || AnyBlt.Matches(i) || AnyBgt.Matches(i));

		public static ILMatch AnyLdloc { get; private set; }
			= new("{any ldloc}", i => i.opcode == OpCodes.Ldloc || i.opcode == OpCodes.Ldloc_S || i.opcode == OpCodes.Ldloc_0 || i.opcode == OpCodes.Ldloc_1 || i.opcode == OpCodes.Ldloc_2 || i.opcode == OpCodes.Ldloc_3);

		public static ILMatch AnyStloc { get; private set; }
			= new("{any stloc}", i => i.opcode == OpCodes.Stloc || i.opcode == OpCodes.Stloc_S || i.opcode == OpCodes.Stloc_0 || i.opcode == OpCodes.Stloc_1 || i.opcode == OpCodes.Stloc_2 || i.opcode == OpCodes.Stloc_3);

		public static ILMatch AnyLdloca { get; private set; }
			= new("{any ldloc}", i => i.opcode == OpCodes.Ldloca || i.opcode == OpCodes.Ldloca_S);

		public static ILMatch AnyCall { get; private set; }
			= new("{call(virt)}", i => i.opcode == OpCodes.Call || i.opcode == OpCodes.Callvirt);

		public static ILMatch AnyLdcI4 { get; private set; }
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

		public static ILMatch Ldarg(int index)
			=> new($"{{ldarg: {index}}}", i => i.IsLdarg(index));

		public static ILMatch LdcI4(int value)
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

		public static ILMatch Ldstr(string value)
			=> new($"{{ldstr `{value}`}}", i => i.opcode == OpCodes.Ldstr && (string)i.operand == value);

		public static ILMatch Ldloc(Type type, IEnumerable<LocalVariableInfo> locals)
			=> new($"{{ldloc(.s) matching type {type}}}", i =>
			{
				if (!AnyLdloc.Matches(i))
					return false;
				if (i.operand is LocalBuilder local)
				{
					return local.LocalType == type;
				}
				else
				{
					var localIndex = ExtractLocalIndex(i);
					if (localIndex is null)
						return false;
					var providedLocal = locals.FirstOrDefault(l => l.LocalIndex == localIndex.Value);
					return providedLocal is not null && providedLocal.LocalType == type;
				}
			});

		public static ILMatch Ldloc<T>(IEnumerable<LocalVariableInfo> locals)
			=> Ldloc(typeof(T), locals);

		public static ILMatch Stloc(Type type, IEnumerable<LocalVariableInfo> locals)
			=> new($"{{stloc(.s) matching type {type}}}", i =>
			{
				if (!AnyStloc.Matches(i))
					return false;
				if (i.operand is LocalBuilder local)
				{
					return local.LocalType == type;
				}
				else
				{
					var localIndex = ExtractLocalIndex(i);
					if (localIndex is null)
						return false;
					var providedLocal = locals.FirstOrDefault(l => l.LocalIndex == localIndex.Value);
					return providedLocal is not null && providedLocal.LocalType == type;
				}
			});

		public static ILMatch Stloc<T>(IEnumerable<LocalVariableInfo> locals)
			=> Stloc(typeof(T), locals);

		public static ILMatch Ldloca(Type type, IEnumerable<LocalVariableInfo> locals)
			=> new($"{{ldloc.a(.s) matching type {type}}}", i =>
			{
				if (!AnyLdloca.Matches(i))
					return false;
				if (i.operand is LocalBuilder local)
				{
					return local.LocalType == type;
				}
				else
				{
					var localIndex = ExtractLocalIndex(i);
					if (localIndex is null)
						return false;
					var providedLocal = locals.FirstOrDefault(l => l.LocalIndex == localIndex.Value);
					return providedLocal is not null && providedLocal.LocalType == type;
				}
			});

		public static ILMatch Ldloca<T>(IEnumerable<LocalVariableInfo> locals)
			=> Ldloca(typeof(T), locals);

		public static ILMatch Ldfld(FieldInfo field)
			=> new($"{{ldfld {field}}}", i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == field);

		public static ILMatch Ldfld(string fieldName)
			=> new($"{{ldfld named {fieldName}}}", i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == fieldName);

		public static ILMatch Stfld(FieldInfo field)
			=> new($"{{ldfld {field}}}", i => i.opcode == OpCodes.Stfld && (FieldInfo)i.operand == field);

		public static ILMatch Stfld(string fieldName)
			=> new($"{{stfld named {fieldName}}}", i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == fieldName);

		public static ILMatch Call(MethodInfo method)
			=> new($"{{(any) call to method {method}}}", i => i.Calls(method));

		public static ILMatch Call(string methodName)
			=> new($"{{(any) call to method named `{methodName}`}}", i => AnyCall.Matches(i) && ((MethodInfo)i.operand).Name == methodName);

		public static ILMatch Newobj(ConstructorInfo constructor)
			=> new($"{{newobj {constructor.DeclaringType} {constructor}}}", i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == constructor);

		public static ILMatch Isinst(Type type)
			=> new($"{{isinst {type}}}", i => i.opcode == OpCodes.Isinst && (Type)i.operand == type);

		public static ILMatch Isinst<T>()
			=> Isinst(typeof(T));

		public static ILMatch Instruction(OpCode? opCode, object? operand = null)
		{
			var opCodeString = opCode is null ? "<any>" : $"{opCode}";
			var operandString = operand is null ? "<any>" : $"{operand}";
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