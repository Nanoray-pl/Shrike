[← back to readme](README.md)

# Release notes

## 2.0.0
Released 9 May 2023.

* Updated to Shrike 2.0.0.
* Renamed `ILMatches.MoveToLabel(Label label)` to `ILMatches.PointerMatcher(Label label)`
* Added `ILMatches.Ldsfld` and `ILMatches.Stsfld`.
* Added `ILMatches.(Ldloc/Stloc/Ldloca)` overloads allowing matching local variable instructions by the local variable index or by referencing another instruction's local variable target.
* Fixed the debug description for `ILMatches.Stfld()` matches.

## 1.1.0
Released 9 January 2023.

* Added `AddLabels` and `ExtractLabels` methods.

## 1.0.1
Released 8 January 2023.

* Updated to Shrike 1.0.1.

## 1.0.0
Released 18 December 2022.

* Initial release.