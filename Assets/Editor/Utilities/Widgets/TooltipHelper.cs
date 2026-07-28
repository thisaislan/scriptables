#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.Widgets
{
    /// <summary>
    /// Provides helper methods for adding tooltips to any EditorGUI control by overlaying
    /// Useful when the standard EditorGUI tooltip system encounters issues.
    /// a GUIContent with tooltip text when the mouse hovers over the control's rectangle.
    /// </summary>
    internal static class TooltipHelper
    {
        private static readonly GUIContent TooltipContent = new GUIContent();

        /// <summary>
        /// Draws a tooltip overlay on the specified rectangle when the mouse is hovering over it.
        /// Must be called after the target control is drawn so the tooltip appears on top.
        /// </summary>
        /// <param name="rect">The rectangle of the control to attach the tooltip to.</param>
        /// <param name="tooltip">The tooltip text to display on hover.</param>
        internal static void DrawTooltip(Rect rect, string tooltip)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                TooltipContent.tooltip = tooltip;
                EditorGUI.LabelField(rect, TooltipContent);
            }
        }
    }
}
#endif
