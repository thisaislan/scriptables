#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Inspectors.Abstracts;
using Thisaislan.Scriptables.Editor.Abstracts.Bases;
using Thisaislan.Scriptables.Editor.Utilities;
using Thisaislan.Scriptables.Editor.Utilities.DrawHelpers;
using Thisaislan.Scriptables.Editor.Utilities.DrawHelpers.Enums;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Inspectors
{
    /// <summary>
    /// Custom editor for RuntimeEditorDebuggableScriptable objects.
    /// Provides inspector GUI for runtime ScriptableObjects with print and reset functionality.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RuntimeEditorDebuggableScriptable), true)]
    internal class ScriptableRuntimeDebuggableEditor :  BaseTransientDataScriptableDebuggableEditor<RuntimeEditorDebuggableScriptable>
    {
        /// <summary>
        /// Determines whether the inspector requires constant repaints. Returns true during play mode.
        /// </summary>
        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        /// <summary>
        /// Draws the inspector GUI when a single target object is selected.
        /// Override this method to implement custom inspector logic for single-object editing.
        /// </summary>
        protected override void OnSingleTargetInspectorGUI()
        {
            RuntimeEditorDebuggableScriptable scriptable = Scriptable;

            DrawEditorHelper.DrawSpaceBetweenCards();

            DrawDataEditorHelper.DrawRuntimeDataCard(
                serializedObject: serializedObject,
                funcToGetPrintableData: ()=> SafeExecuteFuncWithFocusReset(()=> Printer.GetStringData(scriptable.GetRuntimeDataEditorOnly())),
                actionOnClear: ()=> SafeExecuteWithFocusReset(scriptable.ClearRuntimeDataEditorOnly),
                enabled: Application.isPlaying
                );
        }

        /// <summary>
        /// Draws the inspector GUI when multiple target objects are selected.
        /// Override this method to implement custom inspector logic for multi-object editing.
        /// </summary>
        protected override void OnMultipleTargetsInspectorGUI()
        {
            DrawEditorHelper.DrawSpaceBetweenCards();

            DrawDataEditorHelper.DrawRuntimeDataCard(
                serializedObject: serializedObject,
                enabled: Application.isPlaying,
                mode: DrawMode.Multiple,
                actionOnClear: () =>
                {
                    ExecuteOnTargets((scriptable) => SafeExecute(scriptable.ClearRuntimeDataEditorOnly));
                }
            );
        }
    }
}
#endif