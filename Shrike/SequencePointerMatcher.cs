using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoray.Shrike;

/// <summary>
/// Represents a sequence pointer matcher handling elements of a given type.
/// </summary>
/// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
public readonly struct SequencePointerMatcher<TElement> : ISequenceMatcher<SequencePointerMatcher<TElement>, TElement>
{
    internal IReadOnlyList<TElement> AllElementsStorage { get; init; }
    internal IReadOnlyList<SequencePointerAttachedData> PointerAttachedDataStorage { get; init; }
    internal IReadOnlyList<SequenceBlockAttachedData> BlockAttachedDataStorage { get; init; }
    internal int IndexStorage { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SequencePointerMatcher{TElement}"/> class with the given underlying elements, pointing at an element at the given index.
    /// </summary>
    /// <param name="allElements">All underlying elements this sequence matcher is working with.</param>
    /// <param name="index">The index this pointer matcher should point at.</param>
    public SequencePointerMatcher(IEnumerable<TElement> allElements, int index = 0)
    {
        this.AllElementsStorage = allElements.ToList();
        if (index < 0 || index >= this.AllElementsStorage.Count)
            throw new IndexOutOfRangeException();
        this.PointerAttachedDataStorage = new List<SequencePointerAttachedData>();
        this.BlockAttachedDataStorage = new List<SequenceBlockAttachedData>();
        this.IndexStorage = index;
    }

    internal static SequencePointerMatcher<TElement> From(ISequenceMatcher<TElement> matcher, int index, IReadOnlyList<TElement>? allElements = null, IReadOnlyList<SequencePointerAttachedData>? pointerAttachedDataStorage = null, IReadOnlyList<SequenceBlockAttachedData>? blockAttachedDataStorage = null)
    {
        allElements ??= matcher.AllElements();
        if (index < 0 || index >= allElements.Count)
            throw new IndexOutOfRangeException();
        return new()
        {
            AllElementsStorage = allElements,
            PointerAttachedDataStorage = pointerAttachedDataStorage ?? matcher.PointerAttachedData(),
            BlockAttachedDataStorage = blockAttachedDataStorage ?? matcher.BlockAttachedData(),
            IndexStorage = index
        };
    }

    internal SequencePointerMatcher<TElement> Copy(IReadOnlyList<TElement>? allElements = null, IReadOnlyList<SequencePointerAttachedData>? pointerAttachedDataStorage = null, IReadOnlyList<SequenceBlockAttachedData>? blockAttachedDataStorage = null, int? index = null)
    {
        allElements ??= this.AllElementsStorage;
        index ??= this.IndexStorage;
        if (index.Value < 0 || index.Value >= allElements.Count)
            throw new IndexOutOfRangeException();
        return new()
        {
            AllElementsStorage = allElements,
            PointerAttachedDataStorage = pointerAttachedDataStorage ?? this.PointerAttachedDataStorage,
            BlockAttachedDataStorage = blockAttachedDataStorage ?? this.BlockAttachedDataStorage,
            IndexStorage = index.Value
        };
    }

    #region Current state
    /// <inheritdoc/>
    public IReadOnlyList<TElement> AllElements()
        => this.AllElementsStorage;

    /// <summary>
    /// The index of the underlying list of elements this pointer matcher is pointing at.
    /// </summary>
    public int Index()
        => this.IndexStorage;

    /// <summary>
    /// The element this pointer matcher is pointing at.
    /// </summary>
    public TElement Element()
        => this.AllElements()[this.Index()];

    /// <summary>
    /// Stores the element this pointer matcher is pointing at.
    /// </summary>
    /// <param name="element">The currently pointed at element.</param>
    /// <returns>An unchanged pointer matcher.</returns>
    public SequencePointerMatcher<TElement> Element(out TElement element)
    {
        element = this.Element();
        return this;
    }

    /// <summary>
    /// Transforms the current element this pointer matcher is pointing at and stores the result of that transformation.
    /// </summary>
    /// <typeparam name="TResult">The result type of the transformation.</typeparam>
    /// <param name="element">The transformed element.</param>
    /// <param name="transformation">The transformation to run.</param>
    /// <returns>An unchanged pointer matcher.</returns>
    SequencePointerMatcher<TElement> SelectElement<TResult>(out TResult element, Func<TElement, TResult> transformation)
    {
        element = transformation(this.Element());
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
        => SequenceBlockMatcher<TElement>.From(this, this.IndexStorage, 1);

    /// <inheritdoc/>
    public SequencePointerMatcher<TElement> PointerMatcher(SequenceMatcherRelativeElement element)
        => element switch
        {
            SequenceMatcherRelativeElement.FirstInWholeSequence => this.Copy(index: 0),
            SequenceMatcherRelativeElement.BeforeFirst => this.Copy(index: this.IndexStorage - 1),
            SequenceMatcherRelativeElement.First => this,
            SequenceMatcherRelativeElement.Last => this,
            SequenceMatcherRelativeElement.AfterLast => this.Copy(index: this.IndexStorage + 1),
            SequenceMatcherRelativeElement.LastInWholeSequence => this.Copy(index: this.AllElementsStorage.Count - 1),
            _ => throw new ArgumentException($"{nameof(SequenceMatcherRelativeElement)} has an invalid value."),
        };

    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> BlockMatcher(SequenceMatcherRelativeBounds bounds)
        => bounds switch
        {
            SequenceMatcherRelativeBounds.Before => SequenceBlockMatcher<TElement>.From(this, 0, this.IndexStorage),
            SequenceMatcherRelativeBounds.BeforeOrEnclosed => SequenceBlockMatcher<TElement>.From(this, 0, this.IndexStorage + 1),
            SequenceMatcherRelativeBounds.Enclosed => SequenceBlockMatcher<TElement>.From(this, this.IndexStorage, 1),
            SequenceMatcherRelativeBounds.AfterOrEnclosed => SequenceBlockMatcher<TElement>.From(this, this.IndexStorage, this.AllElementsStorage.Count - this.IndexStorage),
            SequenceMatcherRelativeBounds.After => SequenceBlockMatcher<TElement>.From(this, this.IndexStorage + 1, this.AllElementsStorage.Count - this.IndexStorage - 1),
            SequenceMatcherRelativeBounds.WholeSequence => SequenceBlockMatcher<TElement>.From(this, 0, this.AllElementsStorage.Count),
            _ => throw new ArgumentException($"{nameof(SequenceMatcherRelativeBounds)} has an invalid value."),
        };
    #endregion

    #region Basic data modification
    /// <inheritdoc/>
    public SequencePointerMatcher<TElement> WithPointerAttachedData(int index, object data)
        => this.Copy(pointerAttachedDataStorage: this.PointerAttachedDataStorage.Append(new SequencePointerAttachedData { Index = index, Data = data }).ToList());

    /// <inheritdoc/>
    public SequencePointerMatcher<TElement> WithBlockAttachedData(int startIndex, int length, object data)
        => this.Copy(blockAttachedDataStorage: this.BlockAttachedDataStorage.Append(new SequenceBlockAttachedData { StartIndex = startIndex, Length = length, Data = data }).ToList());

    /// <summary>
    /// Creates a matcher with additional pointer-attached data.
    /// </summary>
    /// <param name="data">The attached data.</param>
    /// <returns>A new matcher containing the additional data.</returns>
    public SequencePointerMatcher<TElement> WithPointerAttachedData(object data)
        => this.WithPointerAttachedData(this.IndexStorage, data);
    #endregion

    #region Sequence modification
    /// <inheritdoc/>
    public SequenceBlockMatcher<TElement> Do(Func<SequencePointerMatcher<TElement>, SequenceBlockMatcher<TElement>> closure)
    {
        var innerMatcher = new SequencePointerMatcher<TElement>(new[] { this.Element() });
        var modifiedMatcher = closure(innerMatcher);
        return this.Replace(modifiedMatcher.AllElements());
    }
    #endregion

    #region Cursor manipulation
    /// <summary>
    /// Creates a pointer matcher pointing at another element offset from the current one.
    /// </summary>
    /// <param name="offset">The index offset from the current index.</param>
    public SequencePointerMatcher<TElement> Advance(int offset = 1)
        => this.Copy(index: this.IndexStorage + offset);
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
