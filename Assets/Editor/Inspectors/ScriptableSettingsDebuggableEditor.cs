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
    /// Custom editor for SettingsEditorDebuggableScriptable objects.
    /// Provides inspector GUI for settings ScriptableObjects with editor data, runtime state, and reset functionality.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SettingsEditorDebuggableScriptable), true)]
    internal class ScriptableSettingsDebuggableEditor : BaseDualDataScriptableDebuggableEditor<SettingsEditorDebuggableScriptable>
    {
        /// <summary>
        /// Draws the inspector GUI when a single target object is selected.
        /// Override this method to implement custom inspector logic for single-object editing.
        /// </summary>
        protected override void OnSingleTargetInspectorGUI()
        {
            SettingsEditorDebuggableScriptable scriptable = Scriptable;

            DrawEditorHelper.DrawSpaceBetweenCards();

            bool isDataPropertyNotNull = DrawDataEditorHelper.DrawEditorDataCard(
                serializedObject: serializedObject,
                actionOnModifiedProperties: ()=> SafeExecute(scriptable.ResetRuntimeDataEditorOnly),
                funcToGetPrintableData: ()=> SafeExecuteFuncWithFocusReset(()=> Printer.GetStringData(scriptable.GetDataEditorOnly()))
            );

            DrawBridge(
                onLeftClick: ()=> SafeExecuteWithFocusReset(scriptable.ResetDataEditorOnly),
                onRightClick: ()=> SafeExecuteWithFocusReset(scriptable.ResetRuntimeDataEditorOnly),
                isDataPropertyNotNull: isDataPropertyNotNull);

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

            bool isDataPropertyNotNull = DrawDataEditorHelper.DrawEditorDataCard(
                serializedObject: serializedObject,
                mode: DrawMode.Multiple,
                actionOnModifiedProperties: () =>
                {
                    ExecuteOnTargets((scriptable) => SafeExecute(scriptable.ResetRuntimeDataEditorOnly));
                }
            );

            DrawDataEditorHelper.DrawRuntimeDataCard(
                serializedObject: serializedObject,
                mode: DrawMode.Multiple,
                enabled: Application.isPlaying,
                actionOnClear: () =>
                {
                    ExecuteOnTargets((scriptable) => SafeExecute(scriptable.ClearRuntimeDataEditorOnly));
                }
            );
        }
    }
}
#endif