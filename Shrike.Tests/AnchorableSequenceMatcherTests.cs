using System;
using NUnit.Framework;

namespace Nanoray.Shrike.Tests
{
    [TestFixture]
    internal sealed class AnchorableSequenceMatcherTests
    {
        [Test]
        public void TestSimpleProperties()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            ).AsAnchorable<string, Guid, Guid, SequencePointerMatcher<string>, SequenceBlockMatcher<string>>();

            Assert.AreEqual(0, blockMatcher.StartIndex());
            Assert.AreEqual(5, blockMatcher.EndIndex());
            Assert.AreEqual(5, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d", "e" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d", "e" }, blockMatcher.Elements());

            var pointerMatcher = blockMatcher.PointerMatcher(SequenceMatcherRelativeElement.Last);

            Assert.AreEqual(4, pointerMatcher.Index());
            Assert.AreEqual("e", pointerMatcher.Element());

            pointerMatcher = blockMatcher.PointerMatcher(SequenceMatcherRelativeElement.First);

            Assert.AreEqual(0, pointerMatcher.Index());
            Assert.AreEqual("a", pointerMatcher.Element());

            blockMatcher = pointerMatcher.BlockMatcher();

            Assert.AreEqual(0, blockMatcher.StartIndex());
            Assert.AreEqual(1, blockMatcher.EndIndex());
            Assert.AreEqual(1, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d", "e" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "a" }, blockMatcher.Elements());
        }

        [Test]
        public void TestPointerAnchoring()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            ).AsAnchorable<string, Guid, Guid, SequencePointerMatcher<string>, SequenceBlockMatcher<string>>();

            var pointerMatcher = blockMatcher
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .Advance()
                .AnchorPointer(out var startPlusOneAnchor)
                .PointerMatcher(SequenceMatcherRelativeElement.LastInWholeSequence)
                .Advance(-1)
                .AnchorPointer(out var endMinusOneAnchor);

            var testedMatcher = pointerMatcher.MoveToPointerAnchor(startPlusOneAnchor);

            Assert.AreEqual(1, testedMatcher.Index());
            Assert.AreEqual("b", testedMatcher.Element());

            testedMatcher = pointerMatcher.MoveToPointerAnchor(endMinusOneAnchor);

            Assert.AreEqual(3, testedMatcher.Index());
            Assert.AreEqual("d", testedMatcher.Element());
        }

        [Test]
        public void TestPointerAnchoringWithRemoval()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            ).AsAnchorable<string, Guid, Guid, SequencePointerMatcher<string>, SequenceBlockMatcher<string>>();

            blockMatcher = blockMatcher
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .Advance()
                .AnchorPointer(out var oneAnchor)
                .Advance()
                .AnchorPointer(out var twoAnchor)
                .Advance()
                .AnchorPointer(out var threeAnchor)
                .PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence)
                .Advance(2)
                .Remove();

            var testedMatcher = blockMatcher.MoveToPointerAnchor(oneAnchor);

            Assert.AreEqual(1, testedMatcher.Index());
            Assert.AreEqual("b", testedMatcher.Element());

            testedMatcher = blockMatcher.MoveToPointerAnchor(threeAnchor);

            Assert.AreEqual(2, testedMatcher.Index());
            Assert.AreEqual("d", testedMatcher.Element());

            Assert.Throws<SequenceMatcherException>(() => _ = blockMatcher.MoveToPointerAnchor(twoAnchor));
        }

        [Test]
        public void TestBlockAnchoring()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            ).AsAnchorable<string, Guid, Guid, SequencePointerMatcher<string>, SequenceBlockMatcher<string>>();

            blockMatcher = blockMatcher
                .BlockMatcherBeforeFirst()
                .Encompass(SequenceMatcherPastBoundsDirection.After, 2)
                .AnchorBlock(out var startAnchor)
                .BlockMatcher(SequenceMatcherRelativeBounds.WholeSequence)
                .BlockMatcherAfterLast()
                .Encompass(SequenceMatcherPastBoundsDirection.Before, 2)
                .AnchorBlock(out var endAnchor)
                .BlockMatcher(SequenceMatcherRelativeBounds.WholeSequence);

            var testedMatcher = blockMatcher.MoveToBlockAnchor(startAnchor);

            Assert.AreEqual(0, testedMatcher.StartIndex());
            Assert.AreEqual(2, testedMatcher.EndIndex());
            Assert.AreEqual(2, testedMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d", "e" }, testedMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "a", "b" }, testedMatcher.Elements());

            testedMatcher = blockMatcher.MoveToBlockAnchor(endAnchor);

            Assert.AreEqual(3, testedMatcher.StartIndex());
            Assert.AreEqual(5, testedMatcher.EndIndex());
            Assert.AreEqual(2, testedMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d", "e" }, testedMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "d", "e" }, testedMatcher.Elements());
        }

        [Test]
        public void TestBlockAnchoringWithRemoval()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e", "f", "g", "h"
            ).AsAnchorable<string, Guid, Guid, SequencePointerMatcher<string>, SequenceBlockMatcher<string>>();

            blockMatcher = blockMatcher
                .BlockMatcherBeforeFirst()
                .Encompass(SequenceMatcherPastBoundsDirection.After, 2)
                .AnchorBlock(out var startAnchor)
                .PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence)
                .Advance(3)
                .Encompass(SequenceMatcherPastBoundsDirection.After, 1)
                .AnchorBlock(out var middleAnchor)
                .BlockMatcher(SequenceMatcherRelativeBounds.WholeSequence)
                .BlockMatcherAfterLast()
                .Encompass(SequenceMatcherPastBoundsDirection.Before, 2)
                .AnchorBlock(out var endAnchor)
                .BlockMatcher(SequenceMatcherRelativeBounds.WholeSequence)
                .MoveToBlockAnchor(middleAnchor)
                .Remove()
                .BlockMatcher(SequenceMatcherRelativeBounds.WholeSequence);

            var testedMatcher = blockMatcher.MoveToBlockAnchor(startAnchor);

            Assert.AreEqual(0, testedMatcher.StartIndex());
            Assert.AreEqual(2, testedMatcher.EndIndex());
            Assert.AreEqual(2, testedMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "f", "g", "h" }, testedMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "a", "b" }, testedMatcher.Elements());

            testedMatcher = blockMatcher.MoveToBlockAnchor(endAnchor);

            Assert.AreEqual(4, testedMatcher.StartIndex());
            Assert.AreEqual(6, testedMatcher.EndIndex());
            Assert.AreEqual(2, testedMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "f", "g", "h" }, testedMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "g", "h" }, testedMatcher.Elements());

            Assert.Throws<SequenceMatcherException>(() => _ = blockMatcher.MoveToBlockAnchor(middleAnchor));
        }

        [Test]
        public void TestFindFirstWithAutoAnchor()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "bb", "ccc", "dd", "eee"
            ).AsAnchorable<string, Guid, Guid, SequencePointerMatcher<string>, SequenceBlockMatcher<string>>();

            blockMatcher = blockMatcher
                .Find(
                    new ElementMatch<string>("two chars", e => e.Length == 2),
                    new ElementMatch<string>("three chars", e => e.Length == 3).WithAutoAnchor(out Guid threeCharsAnchor)
                );

            Assert.AreEqual(1, blockMatcher.StartIndex());
            Assert.AreEqual(3, blockMatcher.EndIndex());
            Assert.AreEqual(2, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "bb", "ccc", "dd", "eee" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "bb", "ccc" }, blockMatcher.Elements());

            var pointerMatcher = blockMatcher
                .MoveToPointerAnchor(threeCharsAnchor);

            Assert.AreEqual(2, pointerMatcher.Index());
            Assert.AreEqual("ccc", pointerMatcher.Element());
        }
    }
}
