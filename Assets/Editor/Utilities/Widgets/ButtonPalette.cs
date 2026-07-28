#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Thisaislan.Scriptables.Editor.Utilities.DrawHelpers;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.Widgets
{
    /// <summary>
    /// Provides a unified interface for drawing styled buttons with optional icons in Unity Editor GUIs.
    /// Supports both GUILayout and GUI.Rect-based rendering with predefined icon sets and color styles.
    /// </summary>
    internal static class ButtonPalette
    {
        private const float DefaultButtonHeight = 28f;

        private static readonly GUIContent CachedContent = new GUIContent();

        /// <summary>
        /// Draws a print/console button using GUILayout with quiet styling.
        /// </summary>
        /// <param name="label">Button label text</param>
        /// <param name="action">Callback invoked when button is clicked</param>
        /// <param name="fixedWidth">Optional fixed width for the button</param>
        /// <param name="customHeight">Optional custom height (defaults to 28px)</param>
        internal static void DrawPrintButton(string label, Action action, float? fixedWidth = null, float? customHeight = null)
        {
            DrawButton(
                    label: label,
                    action: action,
                    buttonIcon: ScriptablesStylesIcons.ButtonIcon.PrintIcon,
                    style: ScriptablesStylesColors.ButtonColorStyle.Quiet,
                    tooltip: label,
                    fixedWidth: fixedWidth,
                    customHeight: customHeight
                );
        }

        internal static void DrawClearButton(string label, Action action, float? fixedWidth = null, float? customHeight = null)
        {
            DrawButton(
                    label: label,
                    action: action,
                    buttonIcon: ScriptablesStylesIcons.ButtonIcon.ClearIcon,
                    style: ScriptablesStylesColors.ButtonColorStyle.Urgent,
                    tooltip: label,
                    fixedWidth: fixedWidth,
                    customHeight: customHeight
                );
        }

        /// <summary>
        /// Draws a notification/play button using GUILayout with growth (accent/highlight) styling.
        /// </summary>
        /// <param name="label">Button label text</param>
        /// <param name="action">Callback invoked when button is clicked</param>
        /// <param name="fixedWidth">Optional fixed width for the button</param>
        /// <param name="customHeight">Optional custom height (defaults to 28px)</param>
        internal static void DrawEmitButton(string label, Action action, float? fixedWidth = null, float? customHeight = null)
        {   
            DrawButton(
                    label: label,
                    action: action,
                    buttonIcon: ScriptablesStylesIcons.ButtonIcon.PlayIcon,
                    style: ScriptablesStylesColors.ButtonColorStyle.Growth,
                    tooltip: label,
                    fixedWidth: fixedWidth,
                    customHeight: customHeight
                );
        }

        internal static void DrawPrintButton(Rect rect, string label, Action action)
        {
            DrawButton(
                    rect: rect,
                    label: label,
                    action: action,
                    buttonIcon: ScriptablesStylesIcons.ButtonIcon.PrintIcon,
                    tooltip: label,
                    style: ScriptablesStylesColors.ButtonColorStyle.Quiet
                );
        }

        /// <summary>
        /// Draws a clear/delete button within a specified Rect using urgent (red/danger) styling.
        /// </summary>
        /// <param name="rect">Rectangle defining button position and size</param>
        /// <param name="label">Button label text</param>
        /// <param name="action">Callback invoked when button is clicked</param>
        internal static void DrawClearButton(Rect rect, string label, Action action)
        {
            DrawButton(
                    rect: rect,
                    label: label,
                    action: action,
                    buttonIcon: ScriptablesStylesIcons.ButtonIcon.ClearIcon,
                    tooltip: label,
                    style: ScriptablesStylesColors.ButtonColorStyle.Urgent
                );
        }

        internal static void DrawEmitButton(Rect rect, string label, Action action)
        {
            DrawButton(
                    rect: rect,
                    label: label,
                    action: action,
                    buttonIcon: ScriptablesStylesIcons.ButtonIcon.PlayIcon,
                    tooltip: label,
                    style: ScriptablesStylesColors.ButtonColorStyle.Growth
                );
        }

        /// <summary>
        /// Core method for drawing a button using GUILayout with full customization options.
        /// </summary>
        /// <param name="label">Button label text</param>
        /// <param name="action">Callback invoked when button is clicked</param>
        /// <param name="buttonIcon">Icon to display on the button</param>
        /// <param name="style">Color styling for the button (Neutral/Quiet/Urgent/Growth)</param>
        /// <param name="tooltip">Tooltip text shown on hover</param>
        /// <param name="fixedWidth">Optional fixed width for the button</param>
        /// <param name="customHeight">Optional custom height (defaults to 28px)</param>
        internal static void DrawButton(
            string label,
            Action action,
            ScriptablesStylesIcons.ButtonIcon buttonIcon = ScriptablesStylesIcons.ButtonIcon.None,
            ScriptablesStylesColors.ButtonColorStyle style = ScriptablesStylesColors.ButtonColorStyle.Neutral,
            string tooltip = null,
            float? fixedWidth = null,
            float? customHeight = null)
        {
            string iconName = ScriptablesStylesIcons.GetButtonIconName(buttonIcon);

            GUIContent content = BuildContent(label, iconName, tooltip);
            GUIStyle buttonStyle = GetButtonStyle(iconName, label);

            List<GUILayoutOption> options = new();

            if (fixedWidth.HasValue)
            {
                options.Add(GUILayout.Width(fixedWidth.Value));
            }

            options.Add(GUILayout.Height(customHeight ?? DefaultButtonHeight));

            DrawButton(
                drawFunc: () => GUILayout.Button(content, buttonStyle, options.ToArray()),
                action: action,
                style: style
            );
        }

        /// <summary>
        /// Core method for drawing a button within a specified Rect with full customization options.
        /// </summary>
        /// <param name="rect">Rectangle defining button position and size</param>
        /// <param name="label">Button label text</param>
        /// <param name="action">Callback invoked when button is clicked</param>
        /// <param name="buttonIcon">Icon to display on the button</param>
        /// <param name="tooltip">Tooltip text shown on hover</param>
        /// <param name="style">Color styling for the button (Neutral/Quiet/Urgent/Growth)</param>
        internal static void DrawButton(
            Rect rect,
            string label,
            Action action,
            ScriptablesStylesIcons.ButtonIcon buttonIcon = ScriptablesStylesIcons.ButtonIcon.None,
            string tooltip = "",
            ScriptablesStylesColors.ButtonColorStyle style = ScriptablesStylesColors.ButtonColorStyle.Neutral)
        {
            string iconName = ScriptablesStylesIcons.GetButtonIconName(buttonIcon);

            GUIContent content = BuildContent(label, iconName, tooltip);
            GUIStyle buttonStyle = GetButtonStyle(iconName, label);

            DrawButton(
                drawFunc: () => GUI.Button(rect, content, buttonStyle),
                action: action,
                style: style
            );
        }

        private static GUIContent BuildContent(string label, string iconName, string tooltip)
        {
            GUIContent content;

            if (!string.IsNullOrEmpty(iconName))
            {
                content = EditorGUIUtility.IconContent(iconName);
                content.text = label;
            }
            else
            {
                CachedContent.text = label;
                CachedContent.tooltip = null;
                content = CachedContent;
            }

            if (!string.IsNullOrEmpty(tooltip))
            {
                content.tooltip = tooltip;
            }

            return content;
        }

        private static GUIStyle GetButtonStyle(string iconName, string label)
        {
            if (!string.IsNullOrEmpty(label))
            {
                return ScriptablesStyles.ButtonWithIconPaddingStyle;
            }
            else
            {
                return ScriptablesStyles.ButtonStyle;
            }
        }

        private static void DrawButton(Func<bool> drawFunc, Action action, ScriptablesStylesColors.ButtonColorStyle style)
        {
            DrawEditorHelper.SetGuiButtonColor(style);

            if (drawFunc())
            {
                action?.Invoke();
            }

            DrawEditorHelper.RestoreGuiButtonColor();
        }
    }
}
#endif