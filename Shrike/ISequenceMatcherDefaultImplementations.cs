using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike;

internal static class ISequenceMatcherDefaultImplementations<TElement>
{
    public static SequenceBlockMatcher<TElement> Insert(ISequenceMatcher<TElement> self, SequenceMatcherPastBoundsDirection position, SequenceMatcherInsertionResultingBounds resultingBounds, IEnumerable<TElement> elements)
    {
        var matcher = self.BlockMatcher();
        var elementList = elements.ToList();
        int modificationStartIndex = position switch
        {
            SequenceMatcherPastBoundsDirection.Before => matcher.StartIndexStorage,
            SequenceMatcherPastBoundsDirection.After => matcher.StartIndexStorage + matcher.LengthStorage,
            _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value.")
        };
        return new()
        {
            AllElementsStorage = position switch
            {
                SequenceMatcherPastBoundsDirection.Before => matcher.AllElementsStorage
                    .Take(matcher.StartIndexStorage)
                    .Concat(elementList)
                    .Concat(matcher.AllElementsStorage.Skip(matcher.StartIndexStorage))
                    .ToList(),
                SequenceMatcherPastBoundsDirection.After => matcher.AllElementsStorage
                    .Take(matcher.StartIndexStorage + matcher.LengthStorage)
                    .Concat(elementList)
                    .Concat(matcher.AllElementsStorage.Skip(matcher.StartIndexStorage + matcher.LengthStorage))
                    .ToList(),
                _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value.")
            },
            PointerAttachedDataStorage = SequencePointerAttachedData.GetAfterModification(
                matcher.PointerAttachedDataStorage,
                SequenceModification.Insertion,
                modificationStartIndex, elementList.Count
            ),
            BlockAttachedDataStorage = SequenceBlockAttachedData.GetAfterModification(
                matcher.BlockAttachedDataStorage,
                SequenceModification.Insertion,
                modificationStartIndex, elementList.Count
            ),
            StartIndexStorage = resultingBounds switch
            {
                SequenceMatcherInsertionResultingBounds.ExcludingInsertion => position switch
                {
                    SequenceMatcherPastBoundsDirection.Before => matcher.StartIndexStorage + elementList.Count,
                    SequenceMatcherPastBoundsDirection.After => matcher.StartIndexStorage,
                    _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value.")
                },
                SequenceMatcherInsertionResultingBounds.JustInsertion => position switch
                {
                    SequenceMatcherPastBoundsDirection.Before => matcher.StartIndexStorage,
                    SequenceMatcherPastBoundsDirection.After => matcher.StartIndexStorage + matcher.LengthStorage,
                    _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value.")
                },
                SequenceMatcherInsertionResultingBounds.IncludingInsertion => matcher.StartIndexStorage,
                _ => throw new ArgumentException($"{nameof(SequenceMatcherInsertionResultingBounds)} has an invalid value.")
            },
            LengthStorage = resultingBounds switch
            {
                SequenceMatcherInsertionResultingBounds.ExcludingInsertion => matcher.LengthStorage,
                SequenceMatcherInsertionResultingBounds.JustInsertion => elementList.Count,
                SequenceMatcherInsertionResultingBounds.IncludingInsertion => matcher.LengthStorage + elementList.Count,
                _ => throw new ArgumentException($"{nameof(SequenceMatcherInsertionResultingBounds)} has an invalid value.")
            }
        };
    }

    public static SequencePointerMatcher<TElement> Replace(ISequenceMatcher<TElement> self, TElement element)
    {
        var matcher = self.BlockMatcher();
        return new()
        {
            AllElementsStorage = matcher.AllElementsStorage
                .Take(matcher.StartIndexStorage)
                .Append(element)
                .Concat(matcher.AllElementsStorage.Skip(matcher.StartIndexStorage + matcher.LengthStorage))
                .ToList(),
            PointerAttachedDataStorage = SequencePointerAttachedData.GetAfterModification( // TODO: add Replace modification
                SequencePointerAttachedData.GetAfterModification(
                    matcher.PointerAttachedDataStorage,
                    SequenceModification.Removal,
                    matcher.StartIndexStorage, matcher.LengthStorage
                ),
                SequenceModification.Insertion,
                matcher.StartIndexStorage, 1
            ),
            BlockAttachedDataStorage = SequenceBlockAttachedData.GetAfterModification( // TODO: add Replace modification
                SequenceBlockAttachedData.GetAfterModification(
                    matcher.BlockAttachedDataStorage,
                    SequenceModification.Removal,
                    matcher.StartIndexStorage, matcher.LengthStorage
                ),
                SequenceModification.Insertion,
                matcher.StartIndexStorage, 1
            ),
            IndexStorage = matcher.StartIndexStorage
        };
    }

    public static SequenceBlockMatcher<TElement> Replace(ISequenceMatcher<TElement> self, IEnumerable<TElement> elements)
    {
        var matcher = self.BlockMatcher();
        var elementList = elements.ToList();
        return new()
        {
            AllElementsStorage = matcher.AllElementsStorage
                .Take(matcher.StartIndexStorage)
                .Concat(elementList)
                .Concat(matcher.AllElementsStorage.Skip(matcher.StartIndexStorage + matcher.LengthStorage))
                .ToList(),
            PointerAttachedDataStorage = SequencePointerAttachedData.GetAfterModification( // TODO: add Replace modification
                SequencePointerAttachedData.GetAfterModification(
                    matcher.PointerAttachedDataStorage,
                    SequenceModification.Removal,
                    matcher.StartIndexStorage, matcher.LengthStorage
                ),
                SequenceModification.Insertion,
                matcher.StartIndexStorage, elementList.Count
            ),
            BlockAttachedDataStorage = SequenceBlockAttachedData.GetAfterModification( // TODO: add Replace modification
                SequenceBlockAttachedData.GetAfterModification(
                    matcher.BlockAttachedDataStorage,
                    SequenceModification.Removal,
                    matcher.StartIndexStorage, matcher.LengthStorage
                ),
                SequenceModification.Insertion,
                matcher.StartIndexStorage, elementList.Count
            ),
            StartIndexStorage = matcher.StartIndexStorage,
            LengthStorage = elementList.Count
        };
    }

    public static SequencePointerMatcher<TElement> Remove(ISequenceMatcher<TElement> self, SequenceMatcherPastBoundsDirection postRemovalPosition)
    {
        var matcher = self.BlockMatcher();
        return new()
        {
            AllElementsStorage = matcher.AllElementsStorage
                .Take(matcher.StartIndexStorage)
                .Concat(matcher.AllElementsStorage.Skip(matcher.StartIndexStorage + matcher.LengthStorage))
                .ToList(),
            PointerAttachedDataStorage = SequencePointerAttachedData.GetAfterModification(
                matcher.PointerAttachedDataStorage,
                SequenceModification.Removal,
                matcher.StartIndexStorage, matcher.LengthStorage
            ),
            BlockAttachedDataStorage = SequenceBlockAttachedData.GetAfterModification(
                matcher.BlockAttachedDataStorage,
                SequenceModification.Removal,
                matcher.StartIndexStorage, matcher.LengthStorage
            ),
            IndexStorage = matcher.StartIndexStorage + (postRemovalPosition == SequenceMatcherPastBoundsDirection.Before ? -1 : 0)
        };
    }

    public static SequenceBlockMatcher<TElement> Remove(ISequenceMatcher<TElement> self)
    {
        var matcher = self.BlockMatcher();
        return new()
        {
            AllElementsStorage = matcher.AllElementsStorage
                .Take(matcher.StartIndexStorage)
                .Concat(matcher.AllElementsStorage.Skip(matcher.StartIndexStorage + matcher.LengthStorage))
                .ToList(),
            PointerAttachedDataStorage = SequencePointerAttachedData.GetAfterModification(
                matcher.PointerAttachedDataStorage,
                SequenceModification.Removal,
                matcher.StartIndexStorage, matcher.LengthStorage
            ),
            BlockAttachedDataStorage = SequenceBlockAttachedData.GetAfterModification(
                matcher.BlockAttachedDataStorage,
                SequenceModification.Removal,
                matcher.StartIndexStorage, matcher.LengthStorage
            ),
            StartIndexStorage = matcher.StartIndexStorage,
            LengthStorage = 0
        };
    }

    public static SequenceBlockMatcher<TElement> Encompass(ISequenceMatcher<TElement> self, SequenceMatcherEncompassDirection direction, int length)
    {
        if (length < 0)
            throw new IndexOutOfRangeException($"Invalid value {length} for parameter `{nameof(length)}`.");
        var matcher = self.BlockMatcher();
        if (length == 0)
            return matcher;
        return direction switch
        {
            SequenceMatcherEncompassDirection.Before => matcher.Copy(startIndex: matcher.StartIndexStorage - length, length: matcher.LengthStorage + length),
            SequenceMatcherEncompassDirection.After => matcher.Copy(startIndex: matcher.StartIndexStorage, length: matcher.LengthStorage + length),
            SequenceMatcherEncompassDirection.Both => matcher.Copy(startIndex: matcher.StartIndexStorage - length, length: matcher.LengthStorage + length * 2),
            _ => throw new ArgumentException($"{nameof(SequenceMatcherEncompassDirection)} has an invalid value."),
        };
    }

    public static SequenceBlockMatcher<TElement> EncompassUntil(ISequenceMatcher<TElement> self, SequenceMatcherPastBoundsDirection direction, IReadOnlyList<ElementMatch<TElement>> toFind)
    {
        var matcher = self.BlockMatcher();
        var findOccurence = direction switch
        {
            SequenceMatcherPastBoundsDirection.Before => SequenceBlockMatcherFindOccurence.Last,
            SequenceMatcherPastBoundsDirection.After => SequenceBlockMatcherFindOccurence.First,
            _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value."),
        };
        var findBounds = direction switch
        {
            SequenceMatcherPastBoundsDirection.Before => SequenceMatcherRelativeBounds.Before,
            SequenceMatcherPastBoundsDirection.After => SequenceMatcherRelativeBounds.After,
            _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value."),
        };

        var findMatcher = self.Find(findOccurence, findBounds, toFind);
        return direction switch
        {
            SequenceMatcherPastBoundsDirection.Before => matcher.Copy(startIndex: findMatcher.StartIndex(), length: matcher.EndIndex() - findMatcher.StartIndex()),
            SequenceMatcherPastBoundsDirection.After => matcher.Copy(startIndex: matcher.StartIndex(), length: findMatcher.EndIndex() - matcher.StartIndex()),
            _ => throw new ArgumentException($"{nameof(SequenceMatcherPastBoundsDirection)} has an invalid value."),
        };
    }

    public static SequencePointerMatcher<TElement> Find(ISequenceMatcher<TElement> self, ElementMatch<TElement> toFind)
    {
        var findMatcher = self.Find(new ElementMatch<TElement>[] { toFind });
        return findMatcher.PointerMatcher(SequenceMatcherRelativeElement.First);
    }

    public static SequenceBlockMatcher<TElement> Find(ISequenceMatcher<TElement> self, IReadOnlyList<ElementMatch<TElement>> toFind)
    {
        var matcher = self.BlockMatcher();
        return self.Find(
            SequenceBlockMatcherFindOccurence.First,
            matcher.StartIndex() == 0 && matcher.Length() == self.AllElements().Count ? SequenceMatcherRelativeBounds.Enclosed : SequenceMatcherRelativeBounds.After,
            toFind
        );
    }

    public static SequencePointerMatcher<TElement> Find(ISequenceMatcher<TElement> self, SequenceBlockMatcherFindOccurence occurence, SequenceMatcherRelativeBounds bounds, ElementMatch<TElement> toFind)
    {
        var findMatcher = self.Find(occurence, bounds, new ElementMatch<TElement>[] { toFind });
        return findMatcher.PointerMatcher(SequenceMatcherRelativeElement.First);
    }

    public static SequenceBlockMatcher<TElement> Find(ISequenceMatcher<TElement> self, SequenceBlockMatcherFindOccurence occurence, SequenceMatcherRelativeBounds bounds, IReadOnlyList<ElementMatch<TElement>> toFind)
    {
        var allElements = self.AllElements();
        var findBoundsMatcher = self.BlockMatcher(bounds);
        int startIndex = findBoundsMatcher.StartIndex();
        int endIndex = findBoundsMatcher.EndIndex();

        SequenceBlockMatcher<TElement> MakeFinalMatcher(int startIndex, int length)
        {
            var matcher = self.BlockMatcher().Copy(startIndex: startIndex, length: toFind.Count);
            for (int i = 0; i < toFind.Count; i++)
                foreach (var @delegate in toFind[i].Delegates)
                    matcher = @delegate(matcher, startIndex + i, allElements[startIndex + i]);
            return matcher;
        }

        switch (occurence)
        {
            case SequenceBlockMatcherFindOccurence.First:
                {
                    int maxIndex = endIndex - toFind.Count;
                    for (int index = startIndex; index <= maxIndex; index++)
                    {
                        for (int toFindIndex = 0; toFindIndex < toFind.Count; toFindIndex++)
                        {
                            if (!toFind[toFindIndex].Matches(allElements[index + toFindIndex]))
                                goto continueOuter;
                        }
                        return MakeFinalMatcher(index, toFind.Count);
                    continueOuter:;
                    }
                    break;
                }
            case SequenceBlockMatcherFindOccurence.Last:
                {
                    int minIndex = startIndex + toFind.Count - 1;
                    for (int index = endIndex - 1; index >= minIndex; index--)
                    {
                        for (int toFindIndex = toFind.Count - 1; toFindIndex >= 0; toFindIndex--)
                        {
                            if (!toFind[toFindIndex].Matches(allElements[index + toFindIndex - toFind.Count + 1]))
                                goto continueOuter;
                        }
                        return MakeFinalMatcher(index - toFind.Count + 1, toFind.Count);
                    continueOuter:;
                    }
                    break;
                }
            default:
                throw new ArgumentException($"{nameof(SequenceBlockMatcherFindOccurence)} has an invalid value.");
        }
        throw new SequenceMatcherException($"Pattern not found:\n{string.Join("\n", toFind.Select(i => $"\t{i.Description}"))}");
    }
}
