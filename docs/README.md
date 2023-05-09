# Shrike
A generic sequence matching library, with an additional Harmony-IL-tailored sublibrary.

## Installation

Reference [the `Nanoray.Shrike` NuGet package](https://www.nuget.org/packages/Shrike) in your project.

The NuGet package is compatible with .NET 5+, with some small API changes when using with .NET 7+.

## Usage examples

### Finding a sequence of uppercase-only, then lowercase-only, then uppercase-only strings in a row, then removing that sequence

```cs
ElementMatch<string> uppercase = new("uppercase", e => e.ToUpper() == e);
ElementMatch<string> lowercase = new("lowercase", e => e.ToLower() == e);

IEnumerable<string> newElements = new SequenceBlockMatcher<string>(
    "lorem ipsum dolor sit amet",
    "THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG",
    "oak is strong and also gives shade",
    "CATS AND DOGS EACH HATE THE OTHER",
    "the pipe began to rust while new"
)
    .Find(uppercase, lowercase, uppercase)
    .Remove()
    .AllElements();
```

### Finding a specific element, then replacing the element two places after it with two other elements

```cs
IEnumerable<string> newElements = new SequenceBlockMatcher<string>(
    "a", "b", "c", "d", "e", "f"
)
    .Find(new ElementMatch<string>("c"))
    .PointerMatcher(SequenceMatcherRelativeElement.First)
    .Advance(2)
    .Replace("1", "2")
    .AllElements();
```

### Finding a sequence of 1-, 2-, 3-, 4-, 5-long strings, anchoring the 3-long element, then removing it and its neighbors

```cs
IEnumerable<string> newElements = new SequenceBlockMatcher<string>(
    "a", "bb", "ccc", "dd", "e", "ff", "ggg", "hhhh", "iiiii", "jjjj", "kkk", "ll", "m", "nn", "ooo", "pp", "q"
).AsGuidAnchorable()
    .Find(
        new ElementMatch<string>("1-long", e => e.Length == 1),
        new ElementMatch<string>("2-long", e => e.Length == 2),
        new ElementMatch<string>("3-long", e => e.Length == 3).WithAutoAnchor(out Guid anchor),
        new ElementMatch<string>("4-long", e => e.Length == 4),
        new ElementMatch<string>("5-long", e => e.Length == 5)
    )
    .PointerMatcher(anchor)
    .Encompass(SequenceMatcherPastBoundsDirection.After, 1)
    .Encompass(SequenceMatcherPastBoundsDirection.Before, 1)
    .Remove()
    .AllElements();
```

## See also
* [Release notes for Shrike](release-notes.md)
* [Release notes for Shrike.Harmony](release-notes-harmony.md)
