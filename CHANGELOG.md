# Changelog

#### v3.0.0:
- **New UI** — intuitive editor interface with custom widgets: `ButtonPalette`, `DisplayDialog`, `LoadingIndicator`, `Pagination`, `SearchBar`, `TooltipHelper`, `PopupWithTooltip`; new tab system with icons; custom styles (`ScriptablesStyles`, `ScriptablesStylesColors`, `ScriptablesStylesIcons`, `ScriptablesStylesTextures`)
- **Renamed main methods** — `AddObserver` → `Subscribe`, `RemoveObserver` → `Unsubscribe`, `SetValue` → `Set`, `SetWithoutNotify` → `SetSilently`, `Notify` → `Emit`, `ResetData` → `Reset`, `InitializeData` → `Initialize` — consistent, discoverable API across all base classes
- **Standard interfaces** — `ISubscribable<T>` / `ISubscribable`, `IEmitable`, `ISettable<T>`, `ISilentSettable<T>`, `IResettable`, `IInitializable<T>`, `IPinnable` — users can program against contracts, not concrete types
- **Improved panel** — fully rewritten `ScriptablesPanelWindow` with tab navigation (Scriptables, Settings, Runtime, Reactive, ScriptableObject), paginated asset list, real-time search/filter, details card with description and metadata, centralised icon and style system
- **Creation wizard** — new `ScriptablesWizardWindow` to create scriptable assets with category selection, data type configuration, script generation, and file management — no more manual asset creation
- **Pin feature** — `ReferenceKeeper` static class + `IPinnable` interface; prevents ScriptableObjects from being unloaded during domain reload; backed by `ScriptableCache` MonoBehaviour in the scene with DontDestroyOnLoad
- **Tracking feature** — observer list drawn in every editor inspector showing all active subscribers to reactive events; adds `DrawSubscribersListEditorHelper` for live subscription visibility
- **Description feature** — dedicated description field on every scriptable asset, displayed in both the panel details card and the custom inspector
- **Performance** — removed `GetInvocationList()` allocation (null-safe emit), cached all `GUIContent`/`GUIStyle` statics (zero per-frame allocations), swapped `List` for `HashSet` in `ReferenceKeeper`, reused buffers and cached layout calculations, added `[NonSerialized]` annotations
- **Serialization** — `ScriptableCache` scene persistence for pinned objects; serialised runtime data with `ResetRuntimeDataEditorOnly`/`ClearRuntimeDataEditorOnly` for clean play mode transitions; deprecated old editor/runtime split model
- **Architecture** — removed `Data` base class and all `where T : Data` constraints — types are now freely generic; removed `RuntimeConsts` / `EditorConsts`; removed all old legacy editor classes: `BaseScriptableEditor`, `ScriptableReactiveEditorDebbugableBaseEditor`, `CreateCustomScriptableEditorMenu`, `ScriptableEditorHelper`; new base hierarchy (`BaseEditorDebuggableScriptable`, `RuntimeEditorDebuggableScriptable`, `SettingsEditorDebuggableScriptable`, `ReactiveEditorDebuggableScriptable`, `BaseEditorDebuggableDualDataScriptable`, `BaseEditorDebuggableTransientScriptable`); extracted reusable draw helpers (`DrawEditorHelper`, `DrawPropertyEditorHelper`, `DrawSubscribersListEditorHelper`, `DrawDataEditorHelper`); `AssetScanner`, `ScriptFileManager`, `Printer` utilities
- **Type simplifications** — runtime/reactive/setting types renamed without the `Scriptable` infix (e.g. `BooleanScriptableReactive` → `BooleanReactive`, `IntScriptableRuntime` → `IntRuntime`, `FloatScriptableSettings` → `FloatSetting`); added `NoParamsReactive` standalone class
- **Editor tooling** — organised editor code into `Abstracts/Bases/`, `Inspectors/`, `Utilities/DrawHelpers/`, `Utilities/Widgets/`, `Windows/`; dedicated custom inspectors per type (reactive, runtime, settings, cache, dual-data, transient); centralised icon loading via `ScriptablesStylesIcons`

#### v1.6.1:
- Remove value necessity on the notify method
- Improve RequiresConstantRepaint flag

#### v1.6.0:
- Add arrow control on scriptables panel
- Add individual notification to reactive observable list
- Inspector Improvements

#### v1.5.0:
- Add observer list on inspector
- Add basic runtimes an settings scriptables
- Improve performance of reaction on scriptable reactive
- Minor improvements

#### v1.4.1:
- Fix menu item folder structure

#### v1.4.0:
- Add context menu to help create scriptable scripts

#### v1.3.0:
- Add Search feature in the panel

#### v1.2.0:
- Add ScriptableReavtive
- Add Scriptable Panel

#### v1.0.1:
- Fix cyclic references
- Fix Scriptable Settings Print

#### v1.0.0:
- Add settings and runtime scriptables
