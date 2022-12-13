using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;

namespace Shockah.CommonModCode.IL
{
	public readonly struct ILBlockMatcher : ILMatcher
	{
		public sealed class PointerFromStartSubscripts
		{
			internal ILBlockMatcher Owner { get; set; }

			public ILPointerMatcher this[int index]
			{
				get => new(Owner.AllInstructions, Owner.Index + index, Owner.StoredAnchors);
			}
		}

		public sealed class PointerFromEndSubscripts
		{
			internal ILBlockMatcher Owner { get; set; }

			public ILPointerMatcher this[int index]
			{
				get => new(Owner.AllInstructions, Owner.EndIndex - index, Owner.StoredAnchors);
			}
		}

		public sealed class AnchorPointerSubscripts
		{
			internal ILBlockMatcher Owner { get; set; }

			public ILPointerMatcher this[string anchor]
			{
				get => new(Owner.AllInstructions, Owner.StoredAnchors[anchor], Owner.StoredAnchors);
			}
		}

		public enum FindOccurence
		{
			First, Last
		}

		public enum FindBounds
		{
			Before, BeforeOrEnclosed, Enclosed, AfterOrEnclosed, After, AllInstructions
		}

		public enum EncompassDirection
		{
			Before, After, Both
		}

		public enum EncompassUntilDirection
		{
			Before, After
		}

		public enum InsertionPosition
		{
			Before, After
		}

		public enum InsertionResultingBounds
		{
			ExcludeInsertion, IncludeInsertion
		}

		public const string LastFindBeforeStartPointerAnchor = "$$$LastFindBeforeStartPointer$$$";
		public const string LastFindStartPointerAnchor = "$$$LastFindStartPointer$$$";
		public const string LastFindEndPointerAnchor = "$$$LastFindEndPointer$$$";
		public const string LastFindAfterEndPointerAnchor = "$$$LastFindAfterEndPointer$$$";

		public IReadOnlyList<CodeInstruction> AllInstructions { get; init; }
		private int Index { get; init; }
		public int Length { get; init; }
		private IReadOnlyDictionary<string, int> StoredAnchors { get; init; }

		public PointerFromStartSubscripts FromStart { get; init; } = new();
		public PointerFromEndSubscripts FromEnd { get; init; } = new();
		public AnchorPointerSubscripts Anchors { get; init; } = new();

		public int StartIndex
			=> Index;

		public int EndIndex
			=> Index + Length;

		public ILPointerMatcher BeforeStartPointer
			=> new(AllInstructions, Length == 0 ? StartIndex : StartIndex - 1, StoredAnchors);

		public ILPointerMatcher StartPointer
			=> new(AllInstructions, StartIndex, StoredAnchors);

		public ILPointerMatcher EndPointer
			=> new(AllInstructions, Length == 0 ? EndIndex : EndIndex - 1, StoredAnchors);

		public ILPointerMatcher AfterEndPointer
			=> new(AllInstructions, EndIndex, StoredAnchors);

		public IEnumerable<CodeInstruction> Instructions
			=> AllInstructions.Skip(Index).Take(Length);

		public ILBlockMatcher StartEmptyBlock
			=> new(AllInstructions, BeforeStartPointer.Index, 0, StoredAnchors);

		public ILBlockMatcher EndEmptyBlock
			=> new(AllInstructions, AfterEndPointer.Index, 0, StoredAnchors);

		public ILBlockMatcher LastFindBlock
		{
			get
			{
				var startPointerIndex = Anchors[LastFindStartPointerAnchor].Index;
				var endPointerIndex = Anchors[LastFindEndPointerAnchor].Index;
				return new(AllInstructions, startPointerIndex, endPointerIndex - startPointerIndex, StoredAnchors);
			}
		}

		public ILBlockMatcher AllInstructionsBlock
			=> new(AllInstructions, 0, AllInstructions.Count, StoredAnchors);

		internal ILBlockMatcher(IReadOnlyList<CodeInstruction> instructions, int index, int length, IReadOnlyDictionary<string, int> anchors)
		{
			if (index < 0 || index > instructions.Count)
				throw new IndexOutOfRangeException($"Invalid value {index} for parameter `{nameof(index)}`.");
			if (length < 0 || length > instructions.Count)
				throw new ArgumentException($"Invalid value {length} for parameter `{nameof(length)}`.");
			if (index + length > instructions.Count)
				throw new IndexOutOfRangeException($"Invalid value {length} for parameter `{nameof(length)}`.");

			this.AllInstructions = instructions;
			this.Index = index;
			this.Length = length;
			this.StoredAnchors = anchors;
			this.FromStart.Owner = this;
			this.FromEnd.Owner = this;
			this.Anchors.Owner = this;
		}

		public ILBlockMatcher(IEnumerable<CodeInstruction> instructions, int index, int length) : this(instructions.ToList(), index, length, new Dictionary<string, int>()) { }

		public ILBlockMatcher(IEnumerable<CodeInstruction> instructions, Range range) : this(instructions.ToList(), range.Start.Value, range.End.Value - range.Start.Value, new Dictionary<string, int>()) { }

		public ILBlockMatcher(IEnumerable<CodeInstruction> instructions)
		{
			this.AllInstructions = instructions.ToList();
			this.Index = 0;
			this.Length = AllInstructions.Count;
			this.StoredAnchors = new Dictionary<string, int>();
			this.FromStart.Owner = this;
			this.FromEnd.Owner = this;
			this.Anchors.Owner = this;
		}

		public ILPointerMatcher JumpToLabel(Label label)
		{
			for (int i = 0; i < AllInstructions.Count; i++)
			{
				if (AllInstructions[i].labels.Contains(label))
					return new(AllInstructions, i, StoredAnchors);
			}
			throw new ILMatcherException($"Label {label} not found.");
		}

		public ILBlockMatcher Find(IReadOnlyList<ILMatch> instructionsToFind)
			=> Find(FindOccurence.First, Index == 0 && Length == AllInstructions.Count ? FindBounds.Enclosed : FindBounds.After, instructionsToFind);

		public ILBlockMatcher Find(params ILMatch[] instructionsToFind)
			=> Find((IReadOnlyList<ILMatch>)instructionsToFind);

		public ILBlockMatcher Find(FindOccurence occurence, FindBounds bounds, IReadOnlyList<ILMatch> instructionsToFind)
		{
			int startIndex, endIndex;
			switch (bounds)
			{
				case FindBounds.Before:
					startIndex = 0;
					endIndex = StartIndex;
					break;
				case FindBounds.BeforeOrEnclosed:
					startIndex = 0;
					endIndex = EndIndex;
					break;
				case FindBounds.Enclosed:
					startIndex = Index;
					endIndex = EndIndex;
					break;
				case FindBounds.AfterOrEnclosed:
					startIndex = StartIndex;
					endIndex = AllInstructions.Count;
					break;
				case FindBounds.After:
					startIndex = EndIndex;
					endIndex = AllInstructions.Count;
					break;
				case FindBounds.AllInstructions:
					startIndex = 0;
					endIndex = AllInstructions.Count;
					break;
				default:
					throw new ArgumentException($"{nameof(FindBounds)} has an invalid value.");
			}

			switch (occurence)
			{
				case FindOccurence.First:
					{
						var maxIndex = endIndex - instructionsToFind.Count;
						for (int index = startIndex; index < maxIndex; index++)
						{
							for (int toFindIndex = 0; toFindIndex < instructionsToFind.Count; toFindIndex++)
							{
								if (!instructionsToFind[toFindIndex].Matches(AllInstructions[index + toFindIndex]))
									goto continueOuter;
							}

							Dictionary<string, int> anchors = new(StoredAnchors);
							ILBlockMatcher intermediaryMatcher = new(AllInstructions, index, instructionsToFind.Count, anchors);
							anchors[LastFindBeforeStartPointerAnchor] = intermediaryMatcher.BeforeStartPointer.Index;
							anchors[LastFindStartPointerAnchor] = intermediaryMatcher.StartPointer.Index;
							anchors[LastFindEndPointerAnchor] = intermediaryMatcher.EndPointer.Index;
							anchors[LastFindAfterEndPointerAnchor] = intermediaryMatcher.AfterEndPointer.Index;
							for (int i = 0; i < instructionsToFind.Count; i++)
							{
								if (instructionsToFind[i].AutoAnchor is not null)
									anchors[instructionsToFind[i].AutoAnchor!] = index + i;
							}
							return new(intermediaryMatcher.AllInstructions, intermediaryMatcher.Index, intermediaryMatcher.Length, anchors);
							continueOuter:;
						}
						break;
					}
				case FindOccurence.Last:
					{
						var minIndex = startIndex + instructionsToFind.Count - 1;
						for (int index = endIndex - 1; index >= minIndex; index--)
						{
							for (int toFindIndex = instructionsToFind.Count - 1; toFindIndex >= 0; toFindIndex--)
							{
								if (!instructionsToFind[toFindIndex].Matches(AllInstructions[index + toFindIndex - instructionsToFind.Count + 1]))
									goto continueOuter;
							}

							Dictionary<string, int> anchors = new(StoredAnchors);
							ILBlockMatcher intermediaryMatcher = new(AllInstructions, index - instructionsToFind.Count + 1, instructionsToFind.Count, anchors);
							anchors[LastFindBeforeStartPointerAnchor] = intermediaryMatcher.BeforeStartPointer.Index;
							anchors[LastFindStartPointerAnchor] = intermediaryMatcher.StartPointer.Index;
							anchors[LastFindEndPointerAnchor] = intermediaryMatcher.EndPointer.Index;
							anchors[LastFindAfterEndPointerAnchor] = intermediaryMatcher.AfterEndPointer.Index;
							for (int i = 0; i < instructionsToFind.Count; i++)
							{
								if (instructionsToFind[i].AutoAnchor is not null)
									anchors[instructionsToFind[i].AutoAnchor!] = index - instructionsToFind.Count + 1 + i;
							}
							return new(intermediaryMatcher.AllInstructions, intermediaryMatcher.Index, intermediaryMatcher.Length, anchors);
							continueOuter:;
						}
						break;
					}
				default:
					throw new ArgumentException($"{nameof(FindOccurence)} has an invalid value.");
			}
			throw new ILMatcherException($"Pattern not found:\n{string.Join("\n", instructionsToFind.Select(i => $"\t{i}"))}");
		}

		public ILBlockMatcher Find(FindOccurence occurence, FindBounds bounds, params ILMatch[] instructionsToFind)
			=> Find(occurence, bounds, (IReadOnlyList<ILMatch>)instructionsToFind);

		public ILBlockMatcher Encompass(EncompassDirection direction, int length)
		{
			if (length == 0)
				return this;
			if (length < 0)
				throw new IndexOutOfRangeException($"Invalid value {length} for parameter `{nameof(length)}`.");
			return direction switch
			{
				EncompassDirection.Before => new(AllInstructions, Index - length, Length + length, StoredAnchors),
				EncompassDirection.After => new(AllInstructions, Index, Length + length, StoredAnchors),
				EncompassDirection.Both => new(AllInstructions, Index - length, Length + length * 2, StoredAnchors),
				_ => throw new ArgumentException($"{nameof(EncompassDirection)} has an invalid value."),
			};
		}

		public ILBlockMatcher EncompassUntil(EncompassUntilDirection direction, IReadOnlyList<ILMatch> instructionsToFind)
		{
			var findOccurence = direction switch
			{
				EncompassUntilDirection.Before => FindOccurence.Last,
				EncompassUntilDirection.After => FindOccurence.First,
				_ => throw new ArgumentException($"{nameof(EncompassUntilDirection)} has an invalid value."),
			};
			var findBounds = direction switch
			{
				EncompassUntilDirection.Before => FindBounds.Before,
				EncompassUntilDirection.After => FindBounds.After,
				_ => throw new ArgumentException($"{nameof(EncompassUntilDirection)} has an invalid value."),
			};

			var findMatcher = Find(findOccurence, findBounds, instructionsToFind);
			return direction switch
			{
				EncompassUntilDirection.Before => new(AllInstructions, findMatcher.Index, EndIndex - findMatcher.StartIndex, StoredAnchors),
				EncompassUntilDirection.After => new(AllInstructions, Index, findMatcher.EndIndex - StartIndex, StoredAnchors),
				_ => throw new ArgumentException($"{nameof(EncompassUntilDirection)} has an invalid value."),
			};
		}

		public ILBlockMatcher Replace(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> result = new();
			result.AddRange(AllInstructions.Take(Index));
			result.AddRange(instructions);
			result.AddRange(AllInstructions.Skip(Index + Length));
			var lengthDifference = result.Count - AllInstructions.Count;

			var anchors = StoredAnchors;
			var thisStartIndex = StartIndex;
			var thisEndIndex = StartIndex;
			if (anchors.Any(e => e.Value >= thisStartIndex && e.Value < thisEndIndex))
				anchors = anchors.Where(e => e.Value >= thisStartIndex && e.Value < thisEndIndex).ToDictionary(e => e.Key, e => e.Value);

			return new(result, Index, Length + lengthDifference, anchors);
		}

		public ILBlockMatcher Replace(params CodeInstruction[] instructions)
			=> Replace((IEnumerable<CodeInstruction>)instructions);

		public ILBlockMatcher Insert(InsertionPosition position, InsertionResultingBounds resultingBounds, IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> result = new();
			switch (position)
			{
				case InsertionPosition.Before:
					{
						result.AddRange(AllInstructions.Take(Index));
						result.AddRange(instructions);
						result.AddRange(AllInstructions.Skip(Index));
						var lengthDifference = result.Count - AllInstructions.Count;

						var anchors = StoredAnchors;
						var thisIndex = Index;
						if (anchors.Any(e => e.Value >= thisIndex))
							anchors = anchors.Select(e => e.Value >= thisIndex ? new(e.Key, e.Value + lengthDifference) : e).ToList().ToDictionary(e => e.Key, e => e.Value);

						return resultingBounds switch
						{
							InsertionResultingBounds.ExcludeInsertion => new(result, Index + lengthDifference, Length, anchors),
							InsertionResultingBounds.IncludeInsertion => new(result, Index, Length + lengthDifference, anchors),
							_ => throw new ArgumentException($"{nameof(InsertionResultingBounds)} has an invalid value."),
						};
					}
				case InsertionPosition.After:
					{
						result.AddRange(AllInstructions.Take(Index + Length));
						result.AddRange(instructions);
						result.AddRange(AllInstructions.Skip(Index + Length));
						var lengthDifference = result.Count - AllInstructions.Count;

						var anchors = StoredAnchors;
						var thisEndIndex = EndIndex;
						if (anchors.Any(e => e.Value >= thisEndIndex))
							anchors = anchors.Select(e => e.Value >= thisEndIndex ? new(e.Key, e.Value + lengthDifference) : e).ToDictionary(e => e.Key, e => e.Value);

						return resultingBounds switch
						{
							InsertionResultingBounds.ExcludeInsertion => new(result, Index, Length, anchors),
							InsertionResultingBounds.IncludeInsertion => new(result, Index, Length + lengthDifference, anchors),
							_ => throw new ArgumentException($"{nameof(InsertionResultingBounds)} has an invalid value."),
						};
					}
				default:
					throw new ArgumentException($"{nameof(InsertionPosition)} has an invalid value.");
			}
		}

		public ILBlockMatcher Insert(InsertionPosition insertionPosition, InsertionResultingBounds insertionResultingBounds, params CodeInstruction[] instructions)
			=> Insert(insertionPosition, insertionResultingBounds, (IEnumerable<CodeInstruction>)instructions);

		public ILBlockMatcher Do(Func<ILBlockMatcher, ILMatcher> closure)
		{
			ILBlockMatcher innerMatcher = new(Instructions);
			innerMatcher = closure(innerMatcher).AllInstructionsBlock;

			var resultMatcher = Replace(innerMatcher.AllInstructions);
			var anchors = StoredAnchors;
			if (innerMatcher.StoredAnchors.Count != 0)
			{
				Dictionary<string, int> newAnchors = new(anchors);
				foreach (var e in innerMatcher.StoredAnchors)
					newAnchors[e.Key] = e.Value + Index;
				anchors = newAnchors;
			}

			return new(resultMatcher.AllInstructions, resultMatcher.Index, resultMatcher.Length, anchors);
		}

		public ILBlockMatcher Repeat(int times, Func<ILBlockMatcher, ILBlockMatcher> closure)
		{
			var matcher = this;
			for (int i = 0; i < times; i++)
				matcher = closure(matcher);
			return matcher;
		}

#if DEBUG
		public ILBlockMatcher Breakpoint()
		{
			if (Debugger.IsAttached)
				Debugger.Break();
			return this;
		}
#endif
	}
}