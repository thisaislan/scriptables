#if UNITY_EDITOR
using System;
using Thisaislan.Scriptables.Editor.Utilities.DrawHelpers.Enums;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using Thisaislan.Scriptables.Editor.Utilities.Widgets;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.DrawHelpers
{
    internal static class DrawDataEditorHelper
    {
        private const string DefaultDataLabel = "Data";
        private const string EditorDataMessage = " - Persistent Data";
        private const string RuntimeDataMessage = " - Editable Once Initialized";
        private const string MultipleEditorDataMessage = " - Multiple Data";
        private const string MultipleRuntimeDataMessage = " - Multi-data. Editable Once init";
        private const string NotSerializedOrUnsupportedMessage = "Not serialized or unsupported type";
        private const string RuntimeDataFieldName = "runtimeData";
        private const string PrintRuntimeDataLabel = "Print Live Data";
        private const string ClearRuntimeDataLabel = "Clean Live Data";
        private const string DataFieldName = "data";
        private const string PrintDataLabel = "Print Stored Data";
        private const string EditorDataLabel = "Stored Data";
        private const string RuntimeDataLabel = "Live Data";
        private const string BridgeLeftButtonLabel = "▲";
        private const string BridgeRightButtonLabel = "▼";
        private const string BridgeLeftButtonTooltip = "Reset Stored Data";
        private const string BridgeRightButtonTooltip = "Reset Live Data";

        private static readonly GUIContent dataCustomLabel = new GUIContent(DefaultDataLabel);

        /// <summary>
        /// Draws a card for editor (persistent) data using a SerializedProperty.
        /// Automatically adapts to single or multiple object editing based on the provided mode
        /// or the SerializedObject state.
        /// </summary>
        /// <param name="serializedObject">The SerializedObject containing the target property.</param>
        /// <param name="actionOnModifiedProperties">Callback invoked when the property is modified and applied (only outside play mode).</param>
        /// <param name="actionOnPrint">Callback invoked when the print button is pressed (single mode only).</param>
        /// <param name="mode">Defines how the card behaves (single or multiple). If multiple objects are being edited,
        /// multiple mode is enforced automatically.</param>
        /// <returns>
        /// True if the property exists and is valid; otherwise, false.
        /// </returns>
        internal static bool DrawEditorDataCard(
            SerializedObject serializedObject,
            Action actionOnModifiedProperties,
            Func<string> funcToGetPrintableData = null,
            DrawMode mode = DrawMode.Single)
        {
            SerializedProperty dataProperty = serializedObject.FindProperty(DataFieldName);

            bool isDataPropertyNotNull = dataProperty != null;

            DrawEditorHelper.BeginVerticalCard();

            DrawEditorHelper.SetGuiEnableState(isDataPropertyNotNull);

            string message = mode == DrawMode.Multiple ? MultipleEditorDataMessage : EditorDataMessage;

            DrawDataCardHeader(EditorDataLabel, dataProperty, message);

            if (isDataPropertyNotNull)
            {
                DrawEditorData(serializedObject, dataProperty, actionOnModifiedProperties);
            }
            else
            {
                EditorGUILayout.Space();
                DrawEditorHelper.DrawMessage(NotSerializedOrUnsupportedMessage);
            }

            EditorGUILayout.Space();

            if (funcToGetPrintableData != null)
            {
                ButtonPalette.DrawPrintButton(PrintDataLabel, () =>
                {
                    string stringData = funcToGetPrintableData.Invoke();

                    if (!string.IsNullOrEmpty(stringData))
                    {
                        Printer.PrintData(EditorDataLabel, stringData);
                    }
                });
            }

            DrawEditorHelper.EndVerticalCard();

            DrawEditorHelper.RestoreGuiEnableState();

            return isDataPropertyNotNull;
        }

        /// <summary>
        /// Draws a card for runtime (mutable) data using a SerializedProperty.
        /// Automatically adapts to single or multiple object editing based on the provided mode
        /// or the SerializedObject state.
        /// </summary>
        /// <param name="serializedObject">The SerializedObject containing the target property.</param>
        /// <param name="funcToGetPrintableData">Callback invoked when the print button is pressed (single mode only).</param>
        /// <param name="actionOnClear">Callback invoked when the clear button is pressed.</param>
        /// <param name="enabled">Determines whether the UI is interactable.</param>
        /// <param name="mode">Defines how the card behaves (single or multiple). If multiple objects are being edited,
        /// multiple mode is enforced automatically.</param>
        /// <returns>
        /// True if the property exists and is valid; otherwise, false.
        /// </returns>
        internal static bool DrawRuntimeDataCard(
            SerializedObject serializedObject,
            Action actionOnClear,
            bool enabled,
            Func<string> funcToGetPrintableData = null,
            DrawMode mode = DrawMode.Single)
        {
            SerializedProperty runtimeDataProperty = serializedObject.FindProperty(RuntimeDataFieldName);

            bool isRuntimeDataPropertyNotNull = runtimeDataProperty != null;

            DrawEditorHelper.SetGuiEnableState(enabled);

            DrawEditorHelper.BeginVerticalCard();

            
            string message = mode == DrawMode.Multiple ? MultipleRuntimeDataMessage : RuntimeDataMessage;

            DrawDataCardHeader(RuntimeDataLabel, runtimeDataProperty, message);

            if (isRuntimeDataPropertyNotNull)
            {
                DrawDataProperty(runtimeDataProperty);
            }
            else
            {
                EditorGUILayout.Space();
                DrawEditorHelper.DrawMessage(NotSerializedOrUnsupportedMessage);
            }

            EditorGUILayout.Space();

            if (funcToGetPrintableData != null)
            {
                ButtonPalette.DrawPrintButton(PrintRuntimeDataLabel, () =>
                {
                    string stringData = funcToGetPrintableData.Invoke();

                    if (!string.IsNullOrEmpty(stringData))
                    {
                        Printer.PrintData(RuntimeDataLabel, stringData);
                    }
                });
            }

            ButtonPalette.DrawClearButton(ClearRuntimeDataLabel, actionOnClear);

            DrawEditorHelper.EndVerticalCard();

            DrawEditorHelper.RestoreGuiEnableState();

            return isRuntimeDataPropertyNotNull;
        }

        /// <summary>
        /// Draws two small buttons horizontally centered.
        /// </summary>
        /// <param name="onLeftClick">Action for left button</param>
        /// <param name="onRightClick">Action for right button</param>
        /// <param name="enabled">Determines whether the UI is interactable.</param>
        internal static void DrawBridge(Action onLeftClick, Action onRightClick, bool enabled)
        {
            DrawEditorHelper.SetGuiEnableState(enabled);
            
            EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(DrawEditorHelper.MinCardSize), GUILayout.MaxWidth(DrawEditorHelper.MaxCardSize));

            ButtonPalette.DrawButton(
                    label: BridgeLeftButtonLabel,
                    action: onLeftClick,
                    style: ScriptablesStylesColors.ButtonColorStyle.Plain,
                    tooltip: BridgeLeftButtonTooltip
                );

            ButtonPalette.DrawButton(
                label: BridgeRightButtonLabel,
                action: onRightClick,
                style: ScriptablesStylesColors.ButtonColorStyle.Plain,
                tooltip: BridgeRightButtonTooltip
            );
            
            EditorGUILayout.EndHorizontal();

            DrawEditorHelper.RestoreGuiEnableState();
        }

        private static void DrawEditorData(
            SerializedObject serializedObject,
            SerializedProperty serializedProperty,
            Action actionOnModifiedProperties)
        {
            DrawDataProperty(serializedProperty);

            if (serializedObject.ApplyModifiedProperties())
            {
                if (!Application.isPlaying)
                {
                    try
                    {
                        actionOnModifiedProperties?.Invoke();
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"DrawDataEditorHelper.DrawEditorData encountered an error: {e.Message}");
                    }
                }
            }
        }

        private static void DrawDataProperty(SerializedProperty serializedProperty)
        {
            serializedProperty.isExpanded = true;

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedProperty, dataCustomLabel, true);

            EditorGUI.indentLevel--;
        }

        private static void DrawDataCardHeader(
            string label,
            SerializedProperty dataProperty,
            string message)
        {
            string propertyType = TypeNameSimplifier.GetFormattedSerializedPropertyName(dataProperty);

            DrawEditorHelper.DrawHeaderWithInfo(label + " " + propertyType, message);
        }
    }
}
#endif