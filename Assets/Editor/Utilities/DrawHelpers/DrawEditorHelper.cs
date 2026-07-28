#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.DrawHelpers
{
    /// <summary>
    /// Utility class for drawing common editor UI elements.
    /// </summary>
    internal static class DrawEditorHelper
    {
        internal const float MinCardSize = 300;
        internal const float MaxCardSize = int.MaxValue;
        
        private const float DefaultSpaceBetweenCards = 1;
        private static Color originalBgColor;
        private static bool previousGUIState;

        private static readonly GUIContent TitleContent = new GUIContent();
        private static readonly GUIContent InfoContent = new GUIContent();

        /// <summary>
        /// Stores the current GUI.enabled state to true and sets a new enabled state.
        /// </summary>
        /// <remarks>
        /// This method must be paired with a call to <see cref="RestoreGuiEnabledState"/> after drawing the UI elements
        /// to restore the original GUI.enabled state.
        /// </remarks>
        internal static void EnableGuiEnableState()
        {
            previousGUIState = GUI.enabled;
            GUI.enabled = true;
        }

        /// <summary>
        /// Stores the current GUI.enabled state to false and sets a new enabled state.
        /// </summary>
        /// <remarks>
        /// This method must be paired with a call to <see cref="RestoreGuiEnabledState"/> after drawing the UI elements
        /// to restore the original GUI.enabled state.
        /// </remarks>
        internal static void DisableGuiEnableState()
        {
            previousGUIState = GUI.enabled;
            GUI.enabled = false;
        }

        /// <summary>
        /// Stores the current GUI.enabled state and sets a new enabled state.
        /// </summary>
        /// <param name="guiEnabled">The new enabled state to set (true = enabled, false = disabled)</param>
        /// <remarks>
        /// This method must be paired with a call to <see cref="RestoreGuiEnabledState"/> after drawing the UI elements
        /// to restore the original GUI.enabled state.
        /// </remarks>
        internal static void SetGuiEnableState(bool guiEnabled)
        {
            previousGUIState = GUI.enabled;
            GUI.enabled = guiEnabled;
        }
        
        /// <summary>
        /// Restores the GUI.enabled state to the value stored by the last call to <see cref="SetGuiEnabledState"/>.
        /// </summary>
        /// <remarks>
        /// Always call this method after a paired <see cref="SetGuiEnabledState"/> to prevent
        /// the disabled/enabled state from affecting other UI elements.
        /// </remarks>
        internal static void RestoreGuiEnableState()
        {
             GUI.enabled = previousGUIState;
        }
        
        /// <summary>
        /// Sets the GUI button color to the specified style and stores the original color for later restoration.
        /// </summary>
        /// <param name="buttonColorStyle">The button color style to apply (Default, Confirmation, Success, Warning, Danger, Info)</param>
        internal static void SetGuiButtonColor(ScriptablesStylesColors.ButtonColorStyle buttonColorStyle)
        {
            originalBgColor = GUI.backgroundColor;
            GUI.backgroundColor = ScriptablesStylesColors.GetButtonColor(buttonColorStyle);
        }
        
        /// <summary>
        /// Restores the GUI.backgroundColor to the value stored by the last call to <see cref="SetGuiButtonColor"/>.
        /// </summary>
        internal static void RestoreGuiButtonColor()
        {
            GUI.backgroundColor = originalBgColor;
        }

        /// <summary>
        /// Draw rightsized space between cards.
        /// </summary>
        internal static void DrawSpaceBetweenCards()
        {
            EditorGUILayout.Space(DefaultSpaceBetweenCards);
        }

        /// <summary>
        /// Starts a vertical card container with the predefined card style.
        /// </summary>
        /// <remarks>
        /// This method opens a vertical layout group using <see cref="ScriptablesStyles.CardStyle"/>
        /// and adds a space at the top for consistent card separation.
        /// Must be paired with a call to <see cref="EndVerticalCard"/>.
        /// </remarks>
        internal static void BeginVerticalCard()
        {
            EditorGUILayout.BeginVertical(ScriptablesStyles.CardStyle, GUILayout.MinWidth(MinCardSize), GUILayout.MaxWidth(MaxCardSize));
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Ends the vertical card container and adds bottom spacing.
        /// </summary>
        /// <remarks>
        /// This method adds a space at the bottom and closes the vertical layout group
        /// started by <see cref="BeginVerticalCard"/>.
        /// </remarks>
        internal static void EndVerticalCard()
        {
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws an empty message box when no subscribers are registered.
        /// </summary>
        /// <param name="message">The message to display</param>
        internal static void DrawMessage(string message)
        {
            EditorGUILayout.BeginHorizontal(ScriptablesStyles.DarkHelpBox);
            EditorGUILayout.LabelField(message, ScriptablesStyles.LabelInfoFieldStyle, GUILayout.Height(20));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws a header row with a title on the left and an informational label on the right.
        /// Useful for section headers that display metadata like version, count, or status.
        /// </summary>
        /// <param name="title">The title text displayed on the left side.</param>
        /// <param name="info">The informational text displayed on the right side.</param>
        internal static void DrawHeaderWithInfo(string title, string info)
        {
            Rect rect = EditorGUILayout.GetControlRect();

            float spacing = 6f;

            TitleContent.text = title;
            InfoContent.text = info;

            float titleWidth = ScriptablesStyles.LabelTitleFieldStyle.CalcSize(TitleContent).x;
            float infoWidth = ScriptablesStyles.LabelInfoFieldStyle.CalcSize(InfoContent).x;

            Rect infoRect = new Rect(
                rect.xMax - infoWidth,
                rect.y,
                infoWidth,
                rect.height
            );

            Rect titleRect = new Rect(
                rect.x,
                rect.y,
                rect.width,
                rect.height
            );

            float fadeStartOffset = 40f;
            float fadeRange = 60f;

            float fadeStart = infoRect.x - spacing - fadeStartOffset;

            float fade = 1f;

            if (titleWidth > fadeStart)
            {
                float t = (titleWidth - fadeStart) / fadeRange;
                fade = 1f - Mathf.Clamp01(t);
            }

            // Apply fade
            Color originalColor = GUI.color;
            GUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, fade);

            EditorGUI.LabelField(infoRect, info, ScriptablesStyles.LabelInfoFieldStyle);

            GUI.color = originalColor;

            // Title always on top
            EditorGUI.LabelField(titleRect, title, ScriptablesStyles.LabelTitleFieldStyle);
        }
    }
}
#endif