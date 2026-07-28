#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.Widgets
{
    /// <summary>
    /// Draws an EditorGUI popup with an automatic tooltip that appears when the mouse hovers
    /// over the control. Internally uses <see cref="TooltipHelper.WithTooltip"/> for the overlay.
    /// </summary>
    internal static class PopupWithTooltip
    {
        /// <summary>
        /// Draws a popup with a tooltip using a predefined rectangle.
        /// </summary>
        /// <param name="rect">Rectangle defining the popup position and size.</param>
        /// <param name="selectedIndex">The index of the currently selected option.</param>
        /// <param name="displayedOptions">The array of option text strings shown in the popup.</param>
        /// <param name="tooltip">Tooltip text displayed on hover.</param>
        /// <returns>The index of the newly selected option, or the original value if unchanged.</returns>
        internal static int Draw(
            Rect rect,
            int selectedIndex,
            string[] displayedOptions,
            string tooltip)
        {
            int result = EditorGUI.Popup(rect, selectedIndex, displayedOptions, EditorStyles.popup);
            TooltipHelper.DrawTooltip(rect, tooltip);
            return result;
        }

        /// <summary>
        /// Draws a popup with a tooltip using a predefined rectangle and a custom style.
        /// </summary>
        /// <param name="rect">Rectangle defining the popup position and size.</param>
        /// <param name="selectedIndex">The index of the currently selected option.</param>
        /// <param name="displayedOptions">The array of option text strings shown in the popup.</param>
        /// <param name="tooltip">Tooltip text displayed on hover.</param>
        /// <param name="style">Custom GUIStyle for the popup (e.g. EditorStyles.popup).</param>
        /// <returns>The index of the newly selected option, or the original value if unchanged.</returns>
        internal static int Draw(
            Rect rect,
            int selectedIndex,
            string[] displayedOptions,
            string tooltip,
            GUIStyle style)
        {
            int result = EditorGUI.Popup(rect, selectedIndex, displayedOptions, style);
            TooltipHelper.DrawTooltip(rect, tooltip);
            return result;
        }

        /// <summary>
        /// Draws a popup with a tooltip using GUILayout auto-layout.
        /// </summary>
        /// <param name="selectedIndex">The index of the currently selected option.</param>
        /// <param name="displayedOptions">The array of option text strings shown in the popup.</param>
        /// <param name="tooltip">Tooltip text displayed on hover.</param>
        /// <param name="options">Optional GUILayout options for sizing (e.g. GUILayout.Width()).</param>
        /// <returns>The index of the newly selected option, or the original value if unchanged.</returns>
        internal static int Draw(
            int selectedIndex,
            string[] displayedOptions,
            string tooltip,
            params GUILayoutOption[] options)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, options);
            return Draw(rect, selectedIndex, displayedOptions, tooltip);
        }
    }
}
#endif
