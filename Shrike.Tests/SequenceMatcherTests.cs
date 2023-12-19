using System;
using NUnit.Framework;

namespace Nanoray.Shrike.Tests
{
    [TestFixture]
    internal sealed class SequenceMatcherTests
    {
        [Test]
        public void TestSimpleProperties()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            );

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
        public void TestPointerAdvance()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c"
            );
            var pointerMatcher = blockMatcher.PointerMatcher(SequenceMatcherRelativeElement.First);

            Assert.AreEqual(0, pointerMatcher.Index());
            Assert.AreEqual("a", pointerMatcher.Element());

            pointerMatcher = pointerMatcher.Advance();

            Assert.AreEqual(1, pointerMatcher.Index());
            Assert.AreEqual("b", pointerMatcher.Element());

            pointerMatcher = pointerMatcher.Advance();

            Assert.AreEqual(2, pointerMatcher.Index());
            Assert.AreEqual("c", pointerMatcher.Element());

            Assert.Throws<IndexOutOfRangeException>(() => _ = pointerMatcher.Advance());
        }

        [Test]
        public void TestPointerRemoveWithPointerResultWithPostRemovalPositionBefore()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            );

            var pointerMatcher = blockMatcher
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .Advance(2)
                .Remove(postRemovalPosition: SequenceMatcherPastBoundsDirection.Before);

            Assert.AreEqual(1, pointerMatcher.Index());
            Assert.AreEqual("b", pointerMatcher.Element());
            CollectionAssert.AreEqual(new string[] { "a", "b", "d", "e" }, pointerMatcher.AllElements());
        }

        [Test]
        public void TestPointerRemoveWithPointerResultWithPostRemovalPositionAfter()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            );

            var pointerMatcher = blockMatcher
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .Advance(2)
                .Remove(postRemovalPosition: SequenceMatcherPastBoundsDirection.After);

            Assert.AreEqual(2, pointerMatcher.Index());
            Assert.AreEqual("d", pointerMatcher.Element());
            CollectionAssert.AreEqual(new string[] { "a", "b", "d", "e" }, pointerMatcher.AllElements());
        }

        [Test]
        public void TestPointerRemoveWithBlockResult()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            );

            blockMatcher = blockMatcher
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .Advance(2)
                .Remove();

            Assert.AreEqual(2, blockMatcher.StartIndex());
            Assert.AreEqual(2, blockMatcher.EndIndex());
            Assert.AreEqual(0, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "d", "e" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(Array.Empty<string>(), blockMatcher.Elements());
        }

        [Test]
        public void TestBlockRemove()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e"
            );

            blockMatcher = blockMatcher
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .Advance(1)
                .Encompass(SequenceMatcherEncompassDirection.After, 2)
                .Remove();

            Assert.AreEqual(1, blockMatcher.StartIndex());
            Assert.AreEqual(1, blockMatcher.EndIndex());
            Assert.AreEqual(0, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "e" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(Array.Empty<string>(), blockMatcher.Elements());
        }

        [Test]
        public void TestInsertBeforeIncludingInsertionInResultingBounds()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c"
            );

            blockMatcher = blockMatcher
                .Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, "1", "2", "3");

            Assert.AreEqual(0, blockMatcher.StartIndex());
            Assert.AreEqual(6, blockMatcher.EndIndex());
            Assert.AreEqual(6, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "1", "2", "3", "a", "b", "c" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "1", "2", "3", "a", "b", "c" }, blockMatcher.Elements());
        }

        [Test]
        public void TestInsertBeforeWithJustInsertionInResultingBounds()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c"
            );

            blockMatcher = blockMatcher
                .Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.JustInsertion, "1", "2", "3");

            Assert.AreEqual(0, blockMatcher.StartIndex());
            Assert.AreEqual(3, blockMatcher.EndIndex());
            Assert.AreEqual(3, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "1", "2", "3", "a", "b", "c" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "1", "2", "3" }, blockMatcher.Elements());
        }

        [Test]
        public void TestInsertBeforeExcludingInsertionInResultingBounds()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c"
            );

            blockMatcher = blockMatcher
                .Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.ExcludingInsertion, "1", "2", "3");

            Assert.AreEqual(3, blockMatcher.StartIndex());
            Assert.AreEqual(6, blockMatcher.EndIndex());
            Assert.AreEqual(3, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "1", "2", "3", "a", "b", "c" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c" }, blockMatcher.Elements());
        }

        [Test]
        public void TestBlockInsertAfterIncludingInsertionInResultingBounds()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c"
            );

            blockMatcher = blockMatcher
                .Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, "1", "2", "3");

            Assert.AreEqual(0, blockMatcher.StartIndex());
            Assert.AreEqual(6, blockMatcher.EndIndex());
            Assert.AreEqual(6, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "1", "2", "3" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "1", "2", "3" }, blockMatcher.Elements());
        }

        [Test]
        public void TestBlockInsertAfterWithJustInsertionInResultingBounds()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c"
            );

            blockMatcher = blockMatcher
                .Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.JustInsertion, "1", "2", "3");

            Assert.AreEqual(3, blockMatcher.StartIndex());
            Assert.AreEqual(6, blockMatcher.EndIndex());
            Assert.AreEqual(3, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "1", "2", "3" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "1", "2", "3" }, blockMatcher.Elements());
        }

        [Test]
        public void TestBlockInsertAfterExcludingInsertionInResultingBounds()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "b", "c"
            );

            blockMatcher = blockMatcher
                .Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.ExcludingInsertion, "1", "2", "3");

            Assert.AreEqual(0, blockMatcher.StartIndex());
            Assert.AreEqual(3, blockMatcher.EndIndex());
            Assert.AreEqual(3, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "1", "2", "3" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c" }, blockMatcher.Elements());
        }

        [Test]
        public void TestFindFirst()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "bb", "ccc", "dd", "eee"
            );

            blockMatcher = blockMatcher
                .Find(
                    SequenceBlockMatcherFindOccurence.First, SequenceMatcherRelativeBounds.WholeSequence,
                    new ElementMatch<string>("two chars", e => e.Length == 2),
                    new ElementMatch<string>("three chars", e => e.Length == 3)
                );

            Assert.AreEqual(1, blockMatcher.StartIndex());
            Assert.AreEqual(3, blockMatcher.EndIndex());
            Assert.AreEqual(2, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "bb", "ccc", "dd", "eee" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "bb", "ccc" }, blockMatcher.Elements());
        }

        [Test]
        public void TestFindLast()
        {
            var blockMatcher = new SequenceBlockMatcher<string>(
                "a", "bb", "ccc", "dd", "eee"
            );

            blockMatcher = blockMatcher
                .Find(
                    SequenceBlockMatcherFindOccurence.Last, SequenceMatcherRelativeBounds.WholeSequence,
                    new ElementMatch<string>("two chars", e => e.Length == 2),
                    new ElementMatch<string>("three chars", e => e.Length == 3)
                );

            Assert.AreEqual(3, blockMatcher.StartIndex());
            Assert.AreEqual(5, blockMatcher.EndIndex());
            Assert.AreEqual(2, blockMatcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "bb", "ccc", "dd", "eee" }, blockMatcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "dd", "eee" }, blockMatcher.Elements());
        }

        // TODO: reimplement ForEach
        //[Test]
        //public void TestForEachRemove()
        //{
        //    var blockMatcher = new SequenceBlockMatcher<string>(
        //        "a", "bb", "ccc", "dd", "eee"
        //    );

        //    blockMatcher = blockMatcher
        //        .ForEach(
        //            SequenceMatcherRelativeBounds.WholeSequence,
        //            new ElementMatch<string>[]
        //            {
        //                new ElementMatch<string>("three chars", e => e.Length == 3)
        //            },
        //            matcher => matcher.Remove(),
        //            minExpectedOccurences: 2,
        //            maxExpectedOccurences: 2
        //        );

        //    Assert.AreEqual(0, blockMatcher.StartIndex());
        //    Assert.AreEqual(3, blockMatcher.EndIndex());
        //    Assert.AreEqual(3, blockMatcher.Length());
        //    CollectionAssert.AreEqual(new string[] { "a", "bb", "dd" }, blockMatcher.AllElements());
        //    CollectionAssert.AreEqual(new string[] { "a", "bb", "dd" }, blockMatcher.Elements());
        //}
    }
}
