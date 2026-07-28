#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Inspectors.Abstracts;
using Thisaislan.Scriptables.Editor.Abstracts;
using Thisaislan.Scriptables.Editor.Utilities;
using Thisaislan.Scriptables.Editor.Utilities.DrawHelpers;
using Thisaislan.Scriptables.Editor.Utilities.DrawHelpers.Enums;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Inspectors
{
    /// <summary>
    /// Custom editor for ReactiveEditorDebuggableScriptable objects.
    /// Provides inspector GUI for reactive ScriptableObjects with runtime observer list and debugging controls.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ReactiveEditorDebuggableScriptable), true)]
    internal class ScriptableReactiveDebuggableEditor :  BaseEventfulScriptableDebuggableEditor<ReactiveEditorDebuggableScriptable>
    {
        private Vector2 scrollPos;
        
        /// <summary>
        /// Determines whether the inspector requires constant repaints.
        /// Returns true during play mode to ensure runtime values are updated continuously.
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
            ReactiveEditorDebuggableScriptable scriptable = Scriptable;

            DrawEditorHelper.DrawSpaceBetweenCards();

            DrawDataEditorHelper.DrawRuntimeDataCard(
                serializedObject: serializedObject,
                funcToGetPrintableData: ()=> SafeExecuteFuncWithFocusReset(()=> Printer.GetStringData(scriptable.GetRuntimeDataEditorOnly())),
                actionOnClear: ()=> SafeExecuteWithFocusReset(scriptable.ClearRuntimeDataEditorOnly),
                enabled: Application.isPlaying
                );

            DrawEditorHelper.DrawSpaceBetweenCards();

            DrawRuntimeObserversSingleModeCard(
                scriptable: scriptable,
                scrollPos: ref scrollPos,
                actionOnNotifyAll: ()=> SafeExecuteWithFocusReset(scriptable.EmitEditorOnly),
                enabled: Application.isPlaying,
                getNotifyData: scriptable.GetRuntimeDataEditorOnly
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

            DrawEditorHelper.DrawSpaceBetweenCards();

            DrawRuntimeObserversMultipleModeCard(
                enabled: Application.isPlaying,
                actionOnNotifyAll: () =>
                {
                    ExecuteOnTargets(scriptable => SafeExecuteWithFocusReset(scriptable.EmitEditorOnly));
                }
            );
        }
    }
}
#endif