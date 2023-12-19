using System;
using NUnit.Framework;

namespace Nanoray.Shrike.Tests
{
    [TestFixture]
    internal sealed class ReadmeExampleTests
    {
        [Test]
        public void TestUpperLowerUpperRemove()
        {
            ElementMatch<string> uppercase = new("uppercase", e => e.ToUpper() == e);
            ElementMatch<string> lowercase = new("lowercase", e => e.ToLower() == e);

            var matcher = new SequenceBlockMatcher<string>(
                "lorem ipsum dolor sit amet",
                "THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG",
                "oak is strong and also gives shade",
                "CATS AND DOGS EACH HATE THE OTHER",
                "the pipe began to rust while new"
            )
                .Find(uppercase, lowercase, uppercase)
                .Remove();

            Assert.AreEqual(1, matcher.StartIndex());
            Assert.AreEqual(1, matcher.EndIndex());
            Assert.AreEqual(0, matcher.Length());
            CollectionAssert.AreEqual(new string[] { "lorem ipsum dolor sit amet", "the pipe began to rust while new" }, matcher.AllElements());
            CollectionAssert.AreEqual(Array.Empty<string>(), matcher.Elements());
        }

        [Test]
        public void TestReplace2AfterSpecific()
        {
            var matcher = new SequenceBlockMatcher<string>(
                "a", "b", "c", "d", "e", "f"
            )
                .Find(new ElementMatch<string>("c"))
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .Advance(2)
                .Replace("1", "2");

            Assert.AreEqual(4, matcher.StartIndex());
            Assert.AreEqual(6, matcher.EndIndex());
            Assert.AreEqual(2, matcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d", "1", "2", "f" }, matcher.AllElements());
            CollectionAssert.AreEqual(new string[] { "1", "2" }, matcher.Elements());
        }

        [Test]
        public void Test12345LongAndAnchor3AndEncompassAndRemove()
        {
            var matcher = new SequenceBlockMatcher<string>(
                "a", "bb", "ccc", "dd", "e", "ff", "ggg", "hhhh", "iiiii", "jjjj", "kkk", "ll", "m", "nn", "ooo", "pp", "q"
            )
                .Find(
                    new ElementMatch<string>("1-long", e => e.Length == 1),
                    new ElementMatch<string>("2-long", e => e.Length == 2),
                    new ElementMatch<string>("3-long", e => e.Length == 3).Anchor(out Guid anchor),
                    new ElementMatch<string>("4-long", e => e.Length == 4),
                    new ElementMatch<string>("5-long", e => e.Length == 5)
                )
                .Anchors().MoveToPointerAnchor(anchor)
                .Encompass(SequenceMatcherEncompassDirection.Both, 1)
                .Remove();

            Assert.AreEqual(5, matcher.StartIndex());
            Assert.AreEqual(5, matcher.EndIndex());
            Assert.AreEqual(0, matcher.Length());
            CollectionAssert.AreEqual(new string[] { "a", "bb", "ccc", "dd", "e", "iiiii", "jjjj", "kkk", "ll", "m", "nn", "ooo", "pp", "q" }, matcher.AllElements());
            CollectionAssert.AreEqual(Array.Empty<string>(), matcher.Elements());
        }
    }
}
