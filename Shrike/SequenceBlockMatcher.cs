using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike;

/// <summary>
/// Represents a sequence block matcher handling elements of a given type.
/// </summary>
/// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
public readonly struct SequenceBlockMatcher<TElement> : ISequenceMatcher<SequenceBlockMatcher<TElement>, TElement>
{
    internal IReadOnlyList<TElement> AllElementsStorage { get; init; }
    internal IReadOnlyList<SequencePointerAttachedData> PointerAttachedDataStorage { get; init; }
    internal IReadOnlyList<SequenceBlockAttachedData> BlockAttachedDataStorage { get; init; }
    internal int StartIndexStorage { get; init; }
    internal int LengthStorage { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceBlockMatcher{TElement}"/> class with the given underlying elements, pointing at all of those elements.
    /// </summary>
    /// <param name="allElements">All underlying elements this sequence matcher is working with.</param>
    public SequenceBlockMatcher(IEnumerable<TElement> allElements)
    {
        this.AllElementsStorage = allElements.ToList();
        this.PointerAttachedDataStorage = new List<SequencePointerAttachedData>();
        this.BlockAttachedDataStorage = new List<SequenceBlockAttachedData>();
        this.StartIndexStorage = 0;
        this.LengthStorage = this.AllElementsStorage.Count;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceBlockMatcher{TElement}"/> class with the given underlying elements, pointing at specific elements out of these.
    /// </summary>
    /// <param name="allElements">All underlying elements this sequence matcher is working with.</param>
    /// <param name="startIndex">All underlying elements this sequence matcher is working with.</param>
    /// <param name="length">All underlying elements this sequence matcher is working with.</param>
    public SequenceBlockMatcher(IEnumerable<TElement> allElements, int startIndex, int length)
    {
        this.AllElementsStorage = allElements.ToList();
        if (startIndex < 0 || startIndex + length > this.AllElementsStorage.Count)
            throw new IndexOutOfRangeException();
        this.PointerAttachedDataStorage = new List<SequencePointerAttachedData>();
        this.BlockAttachedDataStorage = new List<SequenceBlockAttachedData>();
        this.StartIndexStorage = 0;
        this.LengthStorage = this.AllElementsStorage.Count;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceBlockMatcher{TElement}"/> class with the given underlying elements, pointing at all of those elements.
    /// </summary>
    /// <param name="allElements">All underlying elements this sequence matcher is working with.</param>
    public SequenceBlockMatcher(params TElement[] allElements) : this((IEnumerable<TElement>)allElements) { }

    internal static SequenceBlockMatcher<TElement> From(ISequenceMatcher<TElement> matcher, int startIndex, int length, IReadOnlyList<TElement>? allElements = null, IReadOnlyList<SequencePointerAttachedData>? pointerAttachedDataStorage = null, IReadOnlyList<SequenceBlockAttachedData>? blockAttachedDataStorage = null)
    {
        if (length < 0)
            throw new ArgumentException($"Invalid `{length}`");
        allElements ??= matcher.AllElements();
        if (startIndex < 0 || startIndex + length > allElements.Count)
            throw new IndexOutOfRangeException();
        return new()
        {
            AllElementsStorage = allElements,
            PointerAttachedDataStorage = pointerAttachedDataStorage ?? matcher.PointerAttachedData(),
            BlockAttachedDataStorage = blockAttachedDataStorage ?? matcher.BlockAttachedData(),
            StartIndexStorage = startIndex,
            LengthStorage = length
        };
    }

    internal SequenceBlockMatcher<TElement> Copy(IReadOnlyList<TElement>? allElements = null, IReadOnlyList<SequencePointerAttachedData>? pointerAttachedDataStorage = null, IReadOnlyList<SequenceBlockAttachedData>? blockAttachedDataStorage = null, int? startIndex = null, int? length = null)
    {
        length ??= this.LengthStorage;
        if (length < 0)
            throw new ArgumentException($"Invalid `{length}`");
        allElements ??= this.AllElementsStorage;
        startIndex ??= this.StartIndexStorage;
        if (startIndex.Value < 0 || startIndex.Value + length.Value > allElements.Count)
            throw new IndexOutOfRangeException();
        return new()
        {
            AllElementsStorage = allElements,
            PointerAttachedDataStorage = pointerAttachedDataStorage ?? this.PointerAttachedDataStorage,
            BlockAttachedDataStorage = blockAttachedDataStorage ?? this.BlockAttachedDataStorage,
            StartIndexStorage = startIndex.Value,
            LengthStorage = length.Value
        };
    }

    #region Current state
    /// <inheritdoc/>
    public IReadOnlyList<TElement> AllElements()
        => this.AllElementsStorage;

    /// <summary>
    /// The (inclusive) start index of the underlying list of elements this block matcher is pointing at.
    /// </summary>
    public int StartIndex()
        => this.StartIndexStorage;

    /// <summary>
    /// The (exclusive) end index of the underlying list of elements this block matcher is pointing at.
    /// </summary>
    public int EndIndex()
        => this.StartIndex() + this.Length();

    /// <summary>
    /// The length of the range this block matcher is pointing at.
    /// </summary>
    public int Length()
        => this.LengthStorage;

    /// <summary>
    /// The elements this block matcher is pointing at.
    /// </summary>
    public IReadOnlyList<TElement> Elements()
        => this.AllElementsStorage.Skip(this.StartIndexStorage).Take(this.LengthStorage).ToList();

    /// <summary>
    /// Stores the elements this block matcher is pointing at.
    /// </summary>
    /// <param name="elements">The currently pointed at elements.</param>
    /// <returns>An unchanged block matcher.</returns>
    public SequenceBlockMatcher<TElement> Elements(out IReadOnlyList<TElement> elements)
    {
        elements = this.Elements();
        return this;
    }

    /// <inheritdoc/>
    public IReadOnlyList<SequencePointerAttachedData> PointerAttachedData()
        => this.PointerAttachedDataStorage;

    /// <inheritdoc/>
    public IReadOnlyList<SequenceBlockAttachedData> BlockAttachedData()
        => this.BlockAttachedDataStorage;
    #endregion

    #region Pointer<->Block conversions
    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> BlockMatcher()
        => this;

    /// <inheritdoc/>
    public SequencePointerMatcher<TElement> PointerMatcher(SequenceMatcherRelativeElement element)
        => element switch
        {
            SequenceMatcherRelativeElement.FirstInWholeSequence => SequencePointerMatcher<TElement>.From(this, 0),
            SequenceMatcherRelativeElement.BeforeFirst => SequencePointerMatcher<TElement>.From(this, this.StartIndexStorage - 1),
            SequenceMatcherRelativeElement.First => SequencePointerMatcher<TElement>.From(this, this.StartIndexStorage),
            SequenceMatcherRelativeElement.Last => SequencePointerMatcher<TElement>.From(this, this.StartIndexStorage + this.LengthStorage - 1),
            SequenceMatcherRelativeElement.AfterLast => SequencePointerMatcher<TElement>.From(this, this.StartIndexStorage + this.LengthStorage),
            SequenceMatcherRelativeElement.LastInWholeSequence => SequencePointerMatcher<TElement>.From(this, this.AllElementsStorage.Count - 1),
            _ => throw new ArgumentException($"{nameof(SequenceMatcherRelativeElement)} has an invalid value."),
        };

    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> BlockMatcher(SequenceMatcherRelativeBounds bounds)
        => bounds switch
        {
            SequenceMatcherRelativeBounds.Before => this.Copy(startIndex: 0, length: this.StartIndexStorage),
            SequenceMatcherRelativeBounds.BeforeOrEnclosed => this.Copy(startIndex: 0, length: this.StartIndexStorage + this.LengthStorage),
            SequenceMatcherRelativeBounds.Enclosed => this,
            SequenceMatcherRelativeBounds.AfterOrEnclosed => this.Copy(startIndex: this.StartIndexStorage, length: this.AllElementsStorage.Count - this.StartIndexStorage),
            SequenceMatcherRelativeBounds.After => this.Copy(startIndex: this.StartIndexStorage + this.LengthStorage, length: this.AllElementsStorage.Count - this.StartIndexStorage - this.LengthStorage),
            SequenceMatcherRelativeBounds.WholeSequence => this.Copy(startIndex: 0, length: this.AllElementsStorage.Count),
            _ => throw new ArgumentException($"{nameof(SequenceMatcherRelativeBounds)} has an invalid value."),
        };
    #endregion

    #region Basic data modification
    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> WithPointerAttachedData(int index, object data)
        => this.Copy(pointerAttachedDataStorage: this.PointerAttachedDataStorage.Append(new SequencePointerAttachedData { Index = index, Data = data }).ToList());

    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> WithBlockAttachedData(int startIndex, int length, object data)
        => this.Copy(blockAttachedDataStorage: this.BlockAttachedDataStorage.Append(new SequenceBlockAttachedData { StartIndex = startIndex, Length = length, Data = data }).ToList());

    /// <summary>
    /// Creates a matcher with additional pointer-attached data.
    /// </summary>
    /// <param name="data">The attached data.</param>
    /// <returns>A new matcher containing the additional data.</returns>
    public SequenceBlockMatcher<TElement> WithBlockAttachedData(object data)
        => this.WithBlockAttachedData(this.StartIndexStorage, this.LengthStorage, data);
    #endregion

    #region Default implementation delegating
    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> Insert(SequenceMatcherPastBoundsDirection position, SequenceMatcherInsertionResultingBounds resultingBounds, IEnumerable<TElement> elements)
        => ISequenceMatcherDefaultImplementations<TElement>.Insert(this, position, resultingBounds, elements);

    /// <inheritdoc/>
    public SequencePointerMatcher<TElement> Replace(TElement element)
        => ISequenceMatcherDefaultImplementations<TElement>.Replace(this, element);

    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> Replace(IEnumerable<TElement> elements)
        => ISequenceMatcherDefaultImplementations<TElement>.Replace(this, elements);

    /// <inheritdoc/>
    public SequencePointerMatcher<TElement> Remove(SequenceMatcherPastBoundsDirection postRemovalPosition)
        => ISequenceMatcherDefaultImplementations<TElement>.Remove(this, postRemovalPosition);

    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> Remove()
        => ISequenceMatcherDefaultImplementations<TElement>.Remove(this);

    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> Encompass(SequenceMatcherEncompassDirection direction, int length)
        => ISequenceMatcherDefaultImplementations<TElement>.Encompass(this, direction, length);

    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> EncompassUntil(SequenceMatcherPastBoundsDirection direction, IReadOnlyList<ElementMatch<TElement>> toFind)
        => ISequenceMatcherDefaultImplementations<TElement>.EncompassUntil(this, direction, toFind);

    /// <inheritdoc/>
    public SequencePointerMatcher<TElement> Find(ElementMatch<TElement> toFind)
        => ISequenceMatcherDefaultImplementations<TElement>.Find(this, toFind);

    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> Find(IReadOnlyList<ElementMatch<TElement>> toFind)
        => ISequenceMatcherDefaultImplementations<TElement>.Find(this, toFind);

    /// <inheritdoc/>
    public SequencePointerMatcher<TElement> Find(SequenceBlockMatcherFindOccurence occurence, SequenceMatcherRelativeBounds bounds, ElementMatch<TElement> toFind)
        => ISequenceMatcherDefaultImplementations<TElement>.Find(this, occurence, bounds, toFind);

    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> Find(SequenceBlockMatcherFindOccurence occurence, SequenceMatcherRelativeBounds bounds, IReadOnlyList<ElementMatch<TElement>> toFind)
        => ISequenceMatcherDefaultImplementations<TElement>.Find(this, occurence, bounds, toFind);
    #endregion
}
