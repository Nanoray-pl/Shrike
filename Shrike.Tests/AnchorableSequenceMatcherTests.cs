using System;
using NUnit.Framework;

namespace Nanoray.Shrike.Tests
{
    [TestFixture]
    internal sealed class AnchorableSequenceMatcherTests
    {
        [Test]
        public void TestPointerAnchoring()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            );

            var pointerMatcher = blockMatcher
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .Advance()
                .Anchors().AnchorPointer(out var startPlusOneAnchor)
                .PointerMatcher(SequenceMatcherRelativeElement.LastInWholeSequence)
                .Advance(-1)
                .Anchors().AnchorPointer(out var endMinusOneAnchor);

            var testedMatcher = pointerMatcher.Anchors().MoveToPointerAnchor(startPlusOneAnchor);

            Assert.AreEqual(1, testedMatcher.Index());
            Assert.AreEqual("b", testedMatcher.Element());

            testedMatcher = pointerMatcher.Anchors().MoveToPointerAnchor(endMinusOneAnchor);

            Assert.AreEqual(3, testedMatcher.Index());
            Assert.AreEqual("d", testedMatcher.Element());
        }

        [Test]
        public void TestPointerAnchoringWithRemoval()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            );

            blockMatcher = blockMatcher
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .Advance()
                .Anchors().AnchorPointer(out var oneAnchor)
                .Advance()
                .Anchors().AnchorPointer(out var twoAnchor)
                .Advance()
                .Anchors().AnchorPointer(out var threeAnchor)
                .PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence)
                .Advance(2)
                .Remove();

            var testedMatcher = blockMatcher.Anchors().MoveToPointerAnchor(oneAnchor);

            Assert.AreEqual(1, testedMatcher.Index());
            Assert.AreEqual("b", testedMatcher.Element());

            testedMatcher = blockMatcher.Anchors().MoveToPointerAnchor(threeAnchor);

            Assert.AreEqual(2, testedMatcher.Index());
            Assert.AreEqual("d", testedMatcher.Element());

            Assert.Throws<SequenceMatcherException>(() => _ = blockMatcher.Anchors().MoveToPointerAnchor(twoAnchor));
        }

        [Test]
        public void TestBlockAnchoring()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            );

            blockMatcher = blockMatcher
                .PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence)
                .Encompass(SequenceMatcherEncompassDirection.After, 1)
                .Anchors().AnchorBlock(out var startAnchor)
                .PointerMatcher(SequenceMatcherRelativeElement.LastInWholeSequence)
                .Encompass(SequenceMatcherEncompassDirection.Before, 1)
                .Anchors().AnchorBlock(out var endAnchor)
                .BlockMatcher(SequenceMatcherRelativeBounds.WholeSequence);

            var testedMatcher = blockMatcher.Anchors().MoveToBlockAnchor(startAnchor);

            Assert.AreEqual(0, testedMatcher.StartIndex());
            Assert.AreEqual(2, testedMatcher.EndIndex());
            Assert.AreEqual(2, testedMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d", "e" }, testedMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "a", "b" }, testedMatcher.Elements());

            testedMatcher = blockMatcher.Anchors().MoveToBlockAnchor(endAnchor);

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
            );

            blockMatcher = blockMatcher
                .PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence)
                .Encompass(SequenceMatcherEncompassDirection.After, 1)
                .Anchors().AnchorBlock(out var startAnchor)
                .PointerMatcher(SequenceMatcherRelativeElement.FirstInWholeSequence)
                .Advance(3)
                .Encompass(SequenceMatcherEncompassDirection.After, 1)
                .Anchors().AnchorBlock(out var middleAnchor)
                .PointerMatcher(SequenceMatcherRelativeElement.LastInWholeSequence)
                .Encompass(SequenceMatcherEncompassDirection.Before, 1)
                .Anchors().AnchorBlock(out var endAnchor)
                .BlockMatcher(SequenceMatcherRelativeBounds.WholeSequence)
                .Anchors().MoveToBlockAnchor(middleAnchor)
                .Remove()
                .BlockMatcher(SequenceMatcherRelativeBounds.WholeSequence);

            var testedMatcher = blockMatcher.Anchors().MoveToBlockAnchor(startAnchor);

            Assert.AreEqual(0, testedMatcher.StartIndex());
            Assert.AreEqual(2, testedMatcher.EndIndex());
            Assert.AreEqual(2, testedMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "f", "g", "h" }, testedMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "a", "b" }, testedMatcher.Elements());

            testedMatcher = blockMatcher.Anchors().MoveToBlockAnchor(endAnchor);

            Assert.AreEqual(4, testedMatcher.StartIndex());
            Assert.AreEqual(6, testedMatcher.EndIndex());
            Assert.AreEqual(2, testedMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "f", "g", "h" }, testedMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "g", "h" }, testedMatcher.Elements());

            Assert.Throws<SequenceMatcherException>(() => _ = blockMatcher.Anchors().MoveToBlockAnchor(middleAnchor));
        }

        [Test]
        public void TestFindFirstWithAutoAnchor()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "bb", "ccc", "dd", "eee"
            );

            blockMatcher = blockMatcher
                .Find(
                    new ElementMatch<string>("two chars", e => e.Length == 2),
                    new ElementMatch<string>("three chars", e => e.Length == 3).Anchor(out Guid threeCharsAnchor)
                );

            Assert.AreEqual(1, blockMatcher.StartIndex());
            Assert.AreEqual(3, blockMatcher.EndIndex());
            Assert.AreEqual(2, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "bb", "ccc", "dd", "eee" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "bb", "ccc" }, blockMatcher.Elements());

            var pointerMatcher = blockMatcher
                .Anchors().MoveToPointerAnchor(threeCharsAnchor);

            Assert.AreEqual(2, pointerMatcher.Index());
            Assert.AreEqual("ccc", pointerMatcher.Element());
        }
    }
}
