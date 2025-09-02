#if UNITY_EDITOR
namespace Thisaislan.Scriptables.Editor
{
    internal class Consts
    {
        internal const string EDITOR_DATA_TOOLTIP = "Data that will be used as data if the data is not initialized from another source";
        internal const string RAW_DATA_LABEL = "Raw Data";
        internal const string RAW_DATA_PRINT_LABEL = "Print";
        internal const string FAILED_TO_COPY_FIELD = "Failed to copy field";
        internal const string FAILED_TO_COPY_PROPERTY = "Failed to copy property";
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
        internal const string CUSTOM_INSPECTOR_ERROR = "Error in custom inspector";
        internal const string CANNOT_DISPLAY_THIS_OBJECT = "Cannot display this object";
        internal const string MAX_RECURSION_EXCEEDED_MESSAGE = "Maximum recursion depth exceeded. Possible circular reference.";
        internal const string PROPERTY_FIELD_ERROR = "Error drawing property field";
        internal const string EDITOR_DEBUG_ERROR = "ScriptableEditorDebbugableEditor error";
        internal const string SELF_REFERENCE_NOT_SUPPORTED = "Self Reference (Not Supported)";
        internal const string CIRCULAR_REFERENCE_PREVENTED = "Circular Reference (Prevented)";
        internal const string CIRCULAR_REFERENCE_DETECTED = "Circular Reference (Detected)";
        internal const string EDITOR_DATA_PROPERTY = "editorData";
        internal const string FAILED_TO_CLONE_EDITOR_DATA = "Failed to clone editor data";
    }
}
#endif