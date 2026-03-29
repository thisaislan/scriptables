#if UNITY_EDITOR
namespace Thisaislan.Scriptables
{
    /// <summary>
    /// Centralized constants for menu paths and metadata used across Scriptables.
    /// Keeps all UI paths, menu locations, and system strings in one place for easy maintenance.
    /// </summary>
    /// <remarks>
    /// Using this class instead of hardcoded strings ensures:
    /// - Consistent menu naming across all Scriptable types
    /// - Single source of truth for paths
    /// - Easier refactoring and updates
    /// </remarks>
    internal static class Meta
    {
        internal const string ScriptableReactiveMenuPath = "Scriptables/ScriptableReactive/";
        internal const string ScriptableRuntimeMenuPath = "Scriptables/ScriptableRuntime/";
        internal const string ScriptableSettingsMenuPath = "Scriptables/ScriptableSettings/";
        internal const string NoListenersEditorMessage = "No listeners registered for {0}";
    }
}
#endif