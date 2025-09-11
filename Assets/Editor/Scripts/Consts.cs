#if UNITY_EDITOR
namespace Thisaislan.Scriptables.Editor
{
    /// <summary>
    /// Constants class containing all string constants used throughout the Scriptables editor system.
    /// This centralizes all UI labels, tooltips, error messages, and other string literals for easy maintenance and localization.
    /// </summary>
    /// <remarks>
    /// Using constants instead of string literals provides:
    /// - Type safety and compile-time checking
    /// - Easy refactoring and maintenance
    /// - Consistent messaging throughout the editor
    /// - Potential for future localization support
    /// </remarks>
    internal class Consts
    {
        internal const string EditorDataTooltip = "Data that will be used as data if the data is not initialized from another source";
        internal const string EditorValueTooltip = "Value that will be used as data if the data is not initialized from another source";
        internal const string EditorDataLabel = "Editor Data";
        internal const string EditorValueLabel = "Editor Value";
        internal const string EditorDataField = "editorData";
        internal const string EditorValueField = "editorValue";
        internal const string RuntimeDataLabel = "Runtime Data - Editable Once Initialized";
        internal const string WaitingForDataInitializationLabel = "Waiting for data to be initialized...";
        internal const string RuntimeDataStructureLabel = "Runtime Data Structure (Readonly)";
        internal const string RuntimeDataMessage = "Runtime data becomes editable during play mode after initialization. Complex types may have limited editing support.";
        internal const string ScriptableReactiveIconName = "reactive_icon";
        internal const string ScriptableRuntimeIconName = "runtime_icon";
        internal const string ScriptableSettingsIconName = "settings_icon";
        internal const string PrintRuntimeDataLabel = "Print Runtime Data";
        internal const string ResetRuntimeDataLabel = "Reset Runtime Data";
        internal const string NotifyRuntimeDataLabel = "Notify Runtime Data";
        internal const string CustomInspectorError = "Error in custom inspector";
        internal const string TypeNotSupported = "Not Supported";
        internal const string SelfReferenceNotSupported = "Self Reference (Not Supported)";
        internal const string CircularReferenceDetected = "Circular Reference (Detected)";
        internal const string ScriptPropertyName = "m_Script";
        internal const string Matrix4x4Field = "Matrix4x4";
        internal const string ElementField = "Element";
        internal const string TypeLiteral = "Type";
        internal const string NullLiteral = "null";
        internal const string NoDataAvailableMessage = "No data available to print";
        internal const string ScriptablesMenuPath = "Scriptables/ScriptableReactive/";
        internal const string NoParametersScriptableReactiveEditorMessage = "\n    ScriptableReactive without parameters.\n\n      Usage:\n       - AddObserver(): Register callback methods\n       - RemoveObserver(): Unregister callbacks\n       - Notify(): Trigger all registered callbacks.\n";
        internal const string MenuItemPath = "Tools/Scriptables/Panel";
        internal const string WindowTitle = "Scriptables Panel";
        internal const string NoItemsLabel = "No items found.";
        internal const string ConfirmDeleteTitle = "Confirm Delete";
        internal const string ConfirmDeleteMessage = "Delete '{0}'?";
        internal const string RenameFieldName = "RenameField";
        internal const string EditorIconsPath = "EditorIcons/";
        internal const string TabScriptablesLabel = " Scriptables";
        internal const string TabSettingsLabel = " Settings";
        internal const string TabRuntimeLabel = " Runtime";
        internal const string TabReactiveLabel = " Reactive";
        internal const string TabAllLabel = " All";
        internal const string TabScriptablesIcon = "tab_scriptables";
        internal const string TabSettingsIcon = "tab_settings";
        internal const string TabRuntimeIcon = "tab_runtime";
        internal const string TabReactiveIcon = "tab_reactive";
        internal const string TabAllIcon = "tab_all";
        internal const string AssetColumnHeader = "    Asset";
        internal const string PathColumnHeader = "Path";
        internal const string ActionsColumnHeader = "  Actions";
        internal const string RenameButtonLabel = "Rename";
        internal const string SearchButtonLabel = "Search";
        internal const string ReactiveVariableName = "Value";
        internal const string DeleteButtonLabel = "Delete";
        internal const string CancelButtonLabel = "Cancel";
        internal const string FilterDebuggable1 = "t:BaseScriptableReactiveEditorDebbugable";
        internal const string FilterDebuggable2 = "t:ScriptableEditorDebbugable";
        internal const string FilterDebuggable3 = "t:NoParametersScriptableReactive";
        internal const string FilterSettings = "t:ScriptableSettings`1";
        internal const string FilterRuntime = "t:ScriptableRuntime`1";
        internal const string FilterReactive = "t:ScriptableReactive`1";
        internal const string FilterAll = "t:ScriptableObject";
        internal const string UnsupportedTypeMessage = "This type is not supported for runtime editing, but its value can be retrieved by code.";
        internal const string TypeNotSerialized = "Not Serialized";
        internal const string ScriptableReactiveName = "ScriptableReactive";
        internal const string AppendError = "<error>";
    }
}
#endif