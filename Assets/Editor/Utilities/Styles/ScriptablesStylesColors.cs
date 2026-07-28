#if UNITY_EDITOR
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.Styles
{
    /// <summary>
    /// Centralized color provider for custom editor UI elements.
    /// Defines color schemes for cards, help boxes, buttons, and windows.
    /// </summary>
    internal static class ScriptablesStylesColors
    {
        /// <summary>
        /// Button color scheme options for different visual purposes.
        /// </summary>
        internal enum ButtonColorStyle
        {
            /// <summary>Flat/transparent for actions with no visual emphasis</summary>
            Plain,
            /// <summary>Neutral gray for standard actions</summary>
            Neutral,
            /// <summary>Calm blue for confirmation/save actions</summary>
            Calm,
            /// <summary>Growth green for success/apply/complete actions</summary>
            Growth,
            /// <summary>Alert orange for caution/warning actions</summary>
            Alert,
            /// <summary>Urgent red for destructive/delete/remove actions</summary>
            Urgent,
            /// <summary>Quiet gray-blue for informational/cancel actions</summary>
            Quiet
        }

        // Card colors
        /// <summary>Selection highlight color for list items.</summary>
        internal static readonly Color SelectionColor = new Color(0.24f, 0.48f, 0.90f, 0.3f);
        /// <summary>Hover highlight color for interactive elements.</summary>
        internal static readonly Color HoverColor = new Color(0.30f, 0.30f, 0.30f, 0.5f);
        /// <summary>Background color for alternating list rows.</summary>
        internal static readonly Color RowBackgroundColor = new Color(0.20f, 0.20f, 0.20f);
        /// <summary>Card container background color.</summary>
        internal static readonly Color CardBackground = new Color(0.205f, 0.205f, 0.205f);
        /// <summary>Card container border color.</summary>
        internal static readonly Color CardBorder = new Color(0.25f, 0.25f, 0.25f);

        // HelpBox colors
        /// <summary>Dark help box background color.</summary>
        internal static readonly Color HelpBoxBackground = new Color(0.235f, 0.235f, 0.235f);
        /// <summary>Dark help box border color.</summary>
        internal static readonly Color HelpBoxBorder = new Color(0.35f, 0.35f, 0.35f);

        // Window colors
        /// <summary>Background color for selected window tabs.</summary>
        internal static readonly Color WindowTabSelectedBgColor = new Color(0.24f, 0.42f, 0.75f, 0.5f);
        /// <summary>Background color for unselected window tabs.</summary>
        internal static readonly Color WindowTabUnselectedBgColor = new Color(0.18f, 0.18f, 0.18f);
        /// <summary>Background color for window containers.</summary>
        internal static readonly Color WindowContainerBgColor = new Color(0.15f, 0.15f, 0.15f);
        /// <summary>Text color for empty state labels in windows.</summary>
        internal static readonly Color WindowEmptyLabelColor = new Color(0.5f, 0.5f, 0.5f);
        /// <summary>Background color for selected window items.</summary>
        internal static readonly Color WindowItemSelectedBgColor = new Color(0.24f, 0.42f, 0.75f, 0.35f);
        /// <summary>Background color for the window bottom bar.</summary>
        internal static readonly Color WindowBottomBarBgColor = new Color(0.13f, 0.13f, 0.13f);
        /// <summary>Text color for selected window items.</summary>
        internal static readonly Color WindowItemSelectedTextColor = new Color(0.6f, 0.8f, 1f);
        /// <summary>Text color for window description labels.</summary>
        internal static readonly Color WindowDescriptionTextColor = new Color(0.7f, 0.7f, 0.7f);
        /// <summary>Text color for pinned labels in windows.</summary>
        internal static readonly Color WindowPinnedLabelColor = new Color(0.3f, 0.8f, 0.3f);

        // Button colors (muted/desaturated for professional look)
        /// <summary>Calm action button color.</summary>
        internal static readonly Color CalmButtonColor = new Color(0.60f, 0.66f, 0.78f);
        /// <summary>Growth action button color.</summary>
        internal static readonly Color GrowthButtonColor = new Color(0.60f, 0.73f, 0.60f);
        /// <summary>Alert action button color.</summary>
        internal static readonly Color AlertButtonColor = new Color(0.80f, 0.73f, 0.60f);
        /// <summary>Urgent action button color.</summary>
        internal static readonly Color UrgentButtonColor = new Color(0.80f, 0.60f, 0.60f);
        /// <summary>Quiet action button color.</summary>
        internal static readonly Color QuietButtonColor = new Color(0.66f, 0.71f, 0.78f);
        /// <summary>Neutral action button color.</summary>
        internal static readonly Color NeutralButtonColor = new Color(0.75f, 0.75f, 0.75f);
        /// <summary>Plain action button color.</summary>
        internal static readonly Color PlainButtonColor = new Color(0.6f, 0.6f, 0.6f);
        /// <summary>Defult line color.</summary>
        internal static readonly Color DefaultLineColor = Color.gray3;

        /// <summary>
        /// Gets the background color for a button based on its visual style.
        /// </summary>
        /// <param name="style">The button color scheme</param>
        /// <returns>The corresponding button background color</returns>
        internal static Color GetButtonColor(ButtonColorStyle style)
        {
            switch (style)
            {
                case ButtonColorStyle.Calm:
                    return CalmButtonColor;
                case ButtonColorStyle.Growth:
                    return GrowthButtonColor;
                case ButtonColorStyle.Alert:
                    return AlertButtonColor;
                case ButtonColorStyle.Urgent:
                    return UrgentButtonColor;
                case ButtonColorStyle.Quiet:
                    return QuietButtonColor;
                case ButtonColorStyle.Plain:
                    return PlainButtonColor;
                default:
                    return NeutralButtonColor;
            }
        }

        /// <summary>
        /// Gets the selection color.
        /// </summary>
        internal static Color GetSelectionColor()
        {
            return SelectionColor;
        }

        /// <summary>
        /// Gets the hover color.
        /// </summary>
        internal static Color GetHoverColor()
        {
            return HoverColor;
        }

        /// <summary>
        /// Gets the row background color.
        /// </summary>
        internal static Color GetRowBackgroundColor()
        {
            return RowBackgroundColor;
        }
    }
}
#endif
