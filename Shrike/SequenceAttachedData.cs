using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike;

/// <summary>
/// Represents data attached to a single element of a <see cref="ISequenceMatcher{TElement}"/>.
/// </summary>
public readonly struct SequencePointerAttachedData
{
    /// <summary>
    /// The element index to which this data is attached.
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// The attached data.
    /// </summary>
    public object Data { get; init; }

    /// <summary>
    /// Calculates new attached data entries after a sequence modification. The returned values may contain less elements, if a removal modification contained them.
    /// </summary>
    /// <param name="entries">The original entries.</param>
    /// <param name="modification">The modification type.</param>
    /// <param name="changeStartIndex">The starting index of the modification.</param>
    /// <param name="changeLength">The length of the modification</param>
    /// <returns>A recalculated list of attached data entries after the modification.</returns>
    public static IReadOnlyList<SequencePointerAttachedData> GetAfterModification(IReadOnlyList<SequencePointerAttachedData> entries, SequenceModification modification, int changeStartIndex, int changeLength)
    {
        if (changeLength == 0)
            return entries;
        else if (changeLength < 0)
            throw new ArgumentException($"`{nameof(changeLength)}` cannot be less than 0.");

        return modification switch
        {
            SequenceModification.Insertion => entries
                .Select(d => new SequencePointerAttachedData { Index = d.Index < changeStartIndex ? d.Index : d.Index + changeLength, Data = d.Data })
                .ToList(),
            SequenceModification.Removal => entries
                .Where(d => d.Index < changeStartIndex || d.Index >= changeStartIndex + changeLength)
                .Select(d => new SequencePointerAttachedData { Index = d.Index < changeStartIndex ? d.Index : d.Index - changeLength, Data = d.Data })
                .ToList(),
            _ => throw new ArgumentException($"{nameof(SequenceModification)} has an invalid value."),
        };
    }
}

/// <summary>
/// Represents data attached to a range of elements of a <see cref="ISequenceMatcher{TElement}"/>.
/// </summary>
public readonly struct SequenceBlockAttachedData
{
    /// <summary>
    /// The starting index of the range of elements to which this data is attached.
    /// </summary>
    public int StartIndex { get; init; }

    /// <summary>
    /// The length of the range of elements to which this data is attached.
    /// </summary>
    public int Length { get; init; }

    /// <summary>
    /// The attached data.
    /// </summary>
    public object Data { get; init; }

    /// <summary>
    /// Calculates new attached data entries after a sequence modification. The returned values may contain less elements, if a removal modification contained them.
    /// </summary>
    /// <param name="entries">The original entries.</param>
    /// <param name="modification">The modification type.</param>
    /// <param name="changeStartIndex">The starting index of the modification.</param>
    /// <param name="changeLength">The length of the modification</param>
    /// <returns>A recalculated list of attached data entries after the modification.</returns>
    public static IReadOnlyList<SequenceBlockAttachedData> GetAfterModification(IReadOnlyList<SequenceBlockAttachedData> entries, SequenceModification modification, int changeStartIndex, int changeLength)
    {
        if (changeLength == 0)
            return entries;
        else if (changeLength < 0)
            throw new ArgumentException($"`{nameof(changeLength)}` cannot be less than 0.");

        return modification switch
        {
            SequenceModification.Insertion => entries
                .Select(d =>
                {
                    if (d.StartIndex + d.Length <= changeStartIndex)
                        return d;
                    else if (d.StartIndex >= changeStartIndex + changeLength)
                        return new SequenceBlockAttachedData { StartIndex = d.StartIndex + changeLength, Length = d.Length, Data = d.Data };
                    else
                        return new SequenceBlockAttachedData { StartIndex = Math.Min(d.StartIndex, changeStartIndex), Length = d.Length + changeLength, Data = d.Data };
                })
                .ToList(),
            SequenceModification.Removal => entries
                .Select(d =>
                {
                    if (d.StartIndex + d.Length <= changeStartIndex) // removal before range
                        return d;
                    if (d.StartIndex >= changeStartIndex + changeLength) // removal after range
                        return new SequenceBlockAttachedData { StartIndex = d.StartIndex - changeLength, Length = d.Length, Data = d.Data };
                    if (d.StartIndex >= changeStartIndex && d.StartIndex + d.Length <= changeStartIndex + changeLength) // removal contains whole range
                        return new SequenceBlockAttachedData { StartIndex = 0, Length = -1, Data = d.Data };
                    if (changeStartIndex >= d.StartIndex && changeStartIndex + changeLength <= d.StartIndex + d.Length) // range contains whole removal
                        return new SequenceBlockAttachedData { StartIndex = d.StartIndex, Length = d.Length - changeLength, Data = d.Data };

                    int overlapStart = Math.Max(d.StartIndex, changeStartIndex);
                    int overlapEnd = Math.Min(d.StartIndex + d.Length, changeStartIndex + changeLength);
                    int overlapLength = overlapEnd - overlapStart;
                    if (changeStartIndex > d.StartIndex && changeStartIndex < d.StartIndex + d.Length) // removal contains the tail of the range
                        return new SequenceBlockAttachedData { StartIndex = d.StartIndex, Length = d.Length - overlapLength, Data = d.Data };
                    else // removal contains the head of the range
                        return new SequenceBlockAttachedData { StartIndex = d.StartIndex - overlapLength, Length = d.Length - overlapLength, Data = d.Data };
                })
                .Where(d => d.Length >= 0)
                .ToList(),
            _ => throw new ArgumentException($"{nameof(SequenceModification)} has an invalid value."),
        };
    }
}
