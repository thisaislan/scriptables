#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Inspectors.Abstracts;
using Thisaislan.Scriptables.Editor.Abstracts;
using Thisaislan.Scriptables.Editor.Utilities.DrawHelpers;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Inspectors
{
    
    /// <summary>
    /// Custom editor for NoParametersScriptableReactive objects.
    /// Provides inspector GUI with info message and emit button for parameterless reactive ScriptableObjects.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ReactiveNoParamsEditorDebuggableScriptable), true)]
    internal class ScriptableReactiveNoParametersEditor : BaseEventfulScriptableDebuggableEditor<ReactiveNoParamsEditorDebuggableScriptable>
    {
        private const string NoParametersScriptableReactiveEditorTitle = "ScriptableReactive without parameters";
        private const string NoParametersScriptableReactiveEditorMessage = "      Usage:\n       - Subscribe(): Register callback methods\n       - Unsubscribe(): Unregister callbacks\n       - Emit(): Trigger all registered callbacks\n";

        private Vector2 scrollPos;

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
            ReactiveNoParamsEditorDebuggableScriptable scriptable = Scriptable;

            DrawEditorHelper.DrawSpaceBetweenCards();

            DrawNoParametersInfoCard();

            DrawEditorHelper.DrawSpaceBetweenCards();

            DrawRuntimeObserversSingleModeCard(
                scriptable: scriptable,
                scrollPos: ref scrollPos,
                actionOnNotifyAll: () => SafeExecuteWithFocusReset(scriptable.EmitEditorOnly),
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

            DrawNoParametersInfoCard();

            DrawEditorHelper.DrawSpaceBetweenCards();

            DrawRuntimeObserversMultipleModeCard(
                enabled: Application.isPlaying,
                actionOnNotifyAll: () =>
                {
                    ExecuteOnTargets(scriptable => SafeExecuteWithFocusReset(scriptable.EmitEditorOnly));
                }
            );
        }

        private static void DrawNoParametersInfoCard()
        {
            DrawEditorHelper.BeginVerticalCard();

            // Header line with icon and title
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(EditorGUIUtility.IconContent(ScriptablesStylesIcons.GetIconName(ScriptablesStylesIcons.IconType.Info)), GUILayout.Width(20), GUILayout.Height(20));
            EditorGUILayout.LabelField(NoParametersScriptableReactiveEditorTitle, ScriptablesStyles.LabelTitleFieldStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);

            // Message content
            EditorGUILayout.LabelField(NoParametersScriptableReactiveEditorMessage, ScriptablesStyles.LabelInfoFieldStyle, GUILayout.Height(70));

            DrawEditorHelper.EndVerticalCard();
        }
    }
}
#endif