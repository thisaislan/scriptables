#if UNITY_EDITOR
namespace Thisaislan.Scriptables.Editor.Consts
{
    internal class MetadataEditor
    {
        internal class Scriptable
        {
            internal const string EDITOR_DEFAULT_ICON_NAME = "d_ScriptableObject Icon";
        }

        internal class ScriptableSettings
        {
            internal const string EDITOR_DATA_TOOLTIP = "Data that will be used as data if the data is not initialized from another source";
            internal const string EDITOR_ICON_NAME = "settings_icon";
            internal const string EDITOR_ICON_NOT_FOUND_MESSAGE = "Scriptable Settings icon not found";
        }

        internal class ScriptableRuntime
        {            
            internal const string EDITOR_ICON_NAME = "runtime_icon";
            internal const string EDITOR_ICON_NOT_FOUND_MESSAGE = "Runtime Settings icon not found";
        }

        internal class ScriptableEditorDebbugablCustomEditor
        {
            internal const string RAW_DATA_LABEL = "Raw Data";
            internal const string RAW_DATA_PRINT_LABEL = "Print";
        }
        
        internal class ReflectionUtility
        {
            internal const string FAILED_TO_COPY_FIELD = "Failed to copy field";
            internal const string FAILED_TO_COPY_PROPERTY = "Failed to copy property";
        }

        internal class ScriptableObjectEditor
        {
            internal const string RUNTIME_DATA_LABEL = "Runtime Data - Editable Once Initialized";
            internal const string WAITING_FOR_DATA_INITIALIZATION_LABEL = "Waiting for data to be initialized...";
            internal const string PRINT_RUNTIME_DATA_LABEL = "Print Runtime Data";
            internal const string RESET_RUNTIME_DATA_LABEL = "Reset Runtime Data";
            internal const string RUNTIME_DATA_STRUCTURE_LABEL = "Runtime Data Structure (Readonly)";
            internal const string RUNTIME_DATA_MESSAGE = "Runtime data becomes editable during play mode after initialization.Complex types may have limited editing support.";
            internal const string EDITOR_DATA_LABEL = "Editor Data";
            internal const string CHANGES_RUNTIME_MESSAGE = "Changes to runtime data won't affect editor data. Editor data remains unchanged.";
            internal const string EDITOR_DATA_FIELD = "editorData";
            internal const string MATRIX_4X4_FIELD = "Matrix4x4";
            internal const string ELEMENT_FIELD = "Element";
        }
    }
}
#endif