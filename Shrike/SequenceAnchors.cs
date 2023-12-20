using System;

namespace Nanoray.Shrike;

/// <summary>
/// A static class hosting extensions for anchoring single elements or blocks of elements for later use.
/// </summary>
public static class SequenceAnchors
{
    /// <summary>
    /// Anchor the element when it is found.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TAnchor">The anchor type.</typeparam>
    /// <param name="self">The match.</param>
    /// <param name="anchor">The anchor to use.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will anchor this element.</returns>
    public static ElementMatch<TElement> Anchor<TElement, TAnchor>(this ElementMatch<TElement> self, TAnchor anchor)
        => self.WithDelegate((matcher, index, element) => matcher.WithPointerAttachedData(index, new AnchorInfo<TAnchor> { Anchor = anchor }));

    /// <summary>
    /// Anchor the element when it is found.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TAnchor">The anchor type.</typeparam>
    /// <param name="self">The match.</param>
    /// <param name="anchor">The created anchor.</param>
    /// <param name="generator">A function that will generate the anchor values.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will anchor this element.</returns>
    public static ElementMatch<TElement> Anchor<TElement, TAnchor>(this ElementMatch<TElement> self, out TAnchor anchor, Func<TAnchor> generator)
    {
        anchor = generator();
        return self.Anchor(anchor);
    }

    /// <summary>
    /// Anchor the element when it is found.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The match.</param>
    /// <param name="anchor">The created anchor.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will anchor this element.</returns>
    public static ElementMatch<TElement> Anchor<TElement>(this ElementMatch<TElement> self, out Guid anchor)
        => self.Anchor(out anchor, Guid.NewGuid);

#if NET7_0_OR_GREATER
    /// <summary>
    /// Anchor the element when it is found.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <typeparam name="TAnchor">The anchor type.</typeparam>
    /// <param name="self">The match.</param>
    /// <param name="anchor">The created anchor.</param>
    /// <returns>A new match with a <c>Find</c> delegate that will anchor this element.</returns>
    public static ElementMatch<TElement> Anchor<TElement, TAnchor>(this ElementMatch<TElement> self, out TAnchor anchor)
        where TAnchor : IGenerable<TAnchor>
    {
        anchor = TAnchor.Generate();
        return self.Anchor(anchor);
    }
#endif

    /// <summary>
    /// Access anchoring methods.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The current matcher</param>
    /// <returns>An instance giving access to anchoring functionality.</returns>
    public static PointerApi<TElement> Anchors<TElement>(this SequencePointerMatcher<TElement> self)
        => new(self);

    /// <summary>
    /// Access anchoring methods.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    /// <param name="self">The current matcher</param>
    /// <returns>An instance giving access to anchoring functionality.</returns>
    public static BlockApi<TElement> Anchors<TElement>(this SequenceBlockMatcher<TElement> self)
        => new(self);

    /// <summary>
    /// Grants access to anchoring methods.
    /// </summary>
    /// <typeparam name="TSelf">This matcher's type.</typeparam>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    public abstract class BaseApi<TSelf, TElement>
        where TSelf : ISequenceMatcher<TSelf, TElement>
    {
        internal TSelf Matcher { get; init; }

        internal BaseApi(TSelf matcher)
        {
            this.Matcher = matcher;
        }

        /// <summary>
        /// Moves to an anchored pointer.
        /// </summary>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="anchor">The anchor to move to.</param>
        /// <returns>A new pointer matcher, pointing at an element anchored earlier.</returns>
        public SequencePointerMatcher<TElement> PointerMatcher<TAnchor>(TAnchor anchor)
        {
            var entry = this.Matcher.PointerAttachedData().FirstOrNull(e => e.Data is AnchorInfo<TAnchor> info && Equals(info.Anchor, anchor)) ?? throw new SequenceMatcherException($"Unknown anchor `{anchor}`");
            var matcher = this.Matcher.PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence);
            return matcher.Copy(index: entry.Index);
        }

        /// <summary>
        /// Moves to an anchored block.
        /// </summary>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="anchor">The anchor to move to.</param>
        /// <returns>A new block matcher, pointing at a block anchored earlier.</returns>
        public SequenceBlockMatcher<TElement> BlockMatcher<TAnchor>(TAnchor anchor)
        {
            var entry = this.Matcher.BlockAttachedData().FirstOrNull(e => e.Data is AnchorInfo<TAnchor> info && Equals(info.Anchor, anchor)) ?? throw new SequenceMatcherException($"Unknown anchor `{anchor}`");
            var matcher = this.Matcher.BlockMatcher();
            return matcher.Copy(startIndex: entry.StartIndex, length: entry.Length);
        }

        /// <summary>
        /// Encompasses elements until a given anchor.
        /// </summary>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="anchor">The anchor to move to.</param>
        /// <returns>A new block matcher, pointing at a block anchored earlier.</returns>
        public SequenceBlockMatcher<TElement> EncompassUntil<TAnchor>(TAnchor anchor)
        {
            var entry = this.Matcher.PointerAttachedData().FirstOrNull(e => e.Data is AnchorInfo<TAnchor> info && Equals(info.Anchor, anchor)) ?? throw new SequenceMatcherException($"Unknown anchor `{anchor}`");
            var matcher = this.Matcher.BlockMatcher();
            if (entry.Index >= matcher.StartIndexStorage && entry.Index <= matcher.StartIndexStorage + matcher.LengthStorage)
                return matcher;
            else if (entry.Index < matcher.StartIndexStorage)
                return matcher.Encompass(SequenceMatcherEncompassDirection.Before, matcher.StartIndexStorage - entry.Index);
            else
                return matcher.Encompass(SequenceMatcherEncompassDirection.After, entry.Index - matcher.StartIndexStorage);
        }
    }

    /// <summary>
    /// Grants access to anchoring methods.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    public sealed class PointerApi<TElement> : BaseApi<SequencePointerMatcher<TElement>, TElement>
    {
        internal PointerApi(SequencePointerMatcher<TElement> matcher) : base(matcher)
        {
        }

        /// <summary>
        /// Anchors a pointer for a later use.
        /// </summary>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="anchor">The anchor to use.</param>
        /// <returns>A new pointer matcher, with an additional anchor pointing at the current element.</returns>
        public SequencePointerMatcher<TElement> AnchorPointer<TAnchor>(TAnchor anchor)
            => this.Matcher.WithPointerAttachedData(new AnchorInfo<TAnchor> { Anchor = anchor });

        /// <summary>
        /// Anchors a pointer for a later use.
        /// </summary>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="anchor">The created anchor.</param>
        /// <param name="generator">A function that will generate the anchor values.</param>
        /// <returns>A new pointer matcher, with an additional anchor pointing at the current element.</returns>
        public SequencePointerMatcher<TElement> AnchorPointer<TAnchor>(out TAnchor anchor, Func<TAnchor> generator)
        {
            anchor = generator();
            return this.AnchorPointer(anchor);
        }

        /// <summary>
        /// Anchors a pointer for a later use.
        /// </summary>
        /// <param name="anchor">The created anchor.</param>
        /// <returns>A new pointer matcher, with an additional anchor pointing at the current element.</returns>
        public SequencePointerMatcher<TElement> AnchorPointer(out Guid anchor)
            => this.AnchorPointer(out anchor, Guid.NewGuid);

#if NET7_0_OR_GREATER
        /// <summary>
        /// Anchors a pointer for a later use.
        /// </summary>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="anchor">The created anchor.</param>
        /// <returns>A new pointer matcher, with an additional anchor pointing at the current element.</returns>
        public SequencePointerMatcher<TElement> AnchorPointer<TAnchor>(out TAnchor anchor)
            where TAnchor : IGenerable<TAnchor>
        {
            anchor = TAnchor.Generate();
            return this.AnchorPointer(anchor);
        }
#endif
    }

    /// <summary>
    /// Grants access to anchoring methods.
    /// </summary>
    /// <typeparam name="TElement">The type of elements this matcher uses.</typeparam>
    public sealed class BlockApi<TElement> : BaseApi<SequenceBlockMatcher<TElement>, TElement>
    {
        internal BlockApi(SequenceBlockMatcher<TElement> matcher) : base(matcher)
        {
        }

        /// <summary>
        /// Anchors a block for a later use.
        /// </summary>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="anchor">The anchor to use.</param>
        /// <returns>A new block matcher, with an additional anchor pointing at the current elements.</returns>
        public SequenceBlockMatcher<TElement> AnchorBlock<TAnchor>(TAnchor anchor)
            => this.Matcher.WithBlockAttachedData(new AnchorInfo<TAnchor> { Anchor = anchor });

        /// <summary>
        /// Anchors a block for a later use.
        /// </summary>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="anchor">The created anchor.</param>
        /// <param name="generator">A function that will generate the anchor values.</param>
        /// <returns>A new block matcher, with an additional anchor pointing at the current elements.</returns>
        public SequenceBlockMatcher<TElement> AnchorBlock<TAnchor>(out TAnchor anchor, Func<TAnchor> generator)
        {
            anchor = generator();
            return this.AnchorBlock(anchor);
        }

        /// <summary>
        /// Anchors a block for a later use.
        /// </summary>
        /// <param name="anchor">The created anchor.</param>
        /// <returns>A new block matcher, with an additional anchor pointing at the current elements.</returns>
        public SequenceBlockMatcher<TElement> AnchorBlock(out Guid anchor)
            => this.AnchorBlock(out anchor, Guid.NewGuid);

#if NET7_0_OR_GREATER
        /// <summary>
        /// Anchors a block for a later use.
        /// </summary>
        /// <typeparam name="TAnchor">The anchor type.</typeparam>
        /// <param name="anchor">The created anchor.</param>
        /// <returns>A new block matcher, with an additional anchor pointing at the current elements.</returns>
        public SequenceBlockMatcher<TElement> AnchorBlock<TAnchor>(out TAnchor anchor)
            where TAnchor : IGenerable<TAnchor>
        {
            anchor = TAnchor.Generate();
            return this.AnchorBlock(anchor);
        }
#endif
    }

    private readonly struct AnchorInfo<TAnchor>
    {
        public TAnchor Anchor { get; init; }
    }
}
