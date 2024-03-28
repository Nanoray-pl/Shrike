[← back to readme](README.md)

# Release notes

## Upcoming release

* Improved some XML docs.

## 3.0.0
Released 19 December 2023.

* Updated to Shrike 3.0.0.
* You can now do a lot of actions like `CreateLdlocInstruction` directly on `ElementMatch<CodeInstruction>` values. These methods will try to directly return values of correct types, or references to them via `ObjectRef`/`NullableObjectRef`/`StructRef`/`NullableStructRef` types.
* Added a lot of missing `ILMatches` methods, like `Ldflda` or `LdcR4`.
* Added additional `ILMatches` methods dealing with `MethodBase`, which removes the need to do `originalMethod.GetMethodBody()!.LocalVariables` manually.

## 2.0.2
Released 24 November 2023.

* Removed transitive Harmony dependency from the `Shrike.Harmony` package.

## 2.0.1
Released 23 November 2023.

* Fixed `ILMatches.Call` throwing on constructors.

## 2.0.0
Released 9 May 2023.

* Updated to Shrike 2.0.0.
* Renamed `ILMatches.MoveToLabel(Label label)` to `ILMatches.PointerMatcher(Label label)`.
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