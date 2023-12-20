[← back to readme](README.md)

# Release notes

## 3.0.0
Released 19 December 2023.

* Major architecture rewrite, while keeping all the functionality.
* Anchoring functionality is now baseline and does not need any subclasses or using `AsGuidAnchorable()`.
* Anchoring functionality is now hidden behind the `.Anchors()` namespacing method.

## 2.0.0
Released 9 May 2023.

* Changed `ISequenceMatcher.Insert` declaration: `bool includeInsertionInResultingBounds` is now `SequenceMatcherInsertionResultingBounds resultingBounds`, with values `ExcludingInsertion`/`JustInsertion`/`IncludingInsertion`.
* Changed `ISequenceMatcher.Encompass` declaration: `SequenceMatcherPastBoundsDirection direction` is now `SequenceMatcherEncompassDirection direction`, with values `Before`/`After`/`Both`.
* Changed `ISequenceBlockMatcher.Elements()` to return `IReadOnlyList<TElement>` instead of `IEnumerable<TElement>`.
* Renamed `ISequenceMatcher.MoveTo(Pointer/Block)Anchor(T(Pointer/Block)Anchor anchor)` to `ISequenceMatcher.(Pointer/Block)Matcher(T(Pointer/Block)Anchor anchor)`.
* Added `ISequencePointerMatcher.Element(out TElement element)` and `ISequenceBlockMatcher.Elements(out IReadOnlyList<TElement> elements)`.
* Added an additional `ISequence(Pointer/Block)Matcher.AsAnchorable()` overload for same `TPointerAnchor` and `TBlockAnchor` generic type arguments.
* Added `ISequence(Pointer/Block)Matcher.AsGuidAnchorable()`.
* Implemented some small potential optimizations.

## 1.1.0
Released 10 January 2023.

* Added `ElementMatch.True`, matching any element.
* Fixed `Find` not working on the very last element.

## 1.0.1
Released 8 January 2023.

* Fixed `Repeat` methods to not require specifying the generic arguments.

## 1.0.0
Released 18 December 2022.

* Initial release.