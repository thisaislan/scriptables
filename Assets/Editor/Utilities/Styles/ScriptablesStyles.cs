#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.Styles
{
    /// <summary>
    /// Centralized style provider for custom editor UI elements.
    /// Provides card containers, help boxes, button styles, and color schemes.
    /// All styles are cached and reused for optimal performance.
    /// </summary>
    internal static class ScriptablesStyles
    {
        // Paths
        private const int LineDefaultThickness = 1;
        private const int LineDefaultWidthPercentage = 1;
        private const int LineDefaultSpace = 0;
        private const float LineDefaultHorizontalMargin = 4;

        // Details
        private const int MessagePadding = 20;
        private const int CardCornerRadius = 8;
        private const int DarkHelpBoxSCornerRadius = 0;
        private const int HelpBoxBorderWidth = 1;
        private const int SearchBarHalfExtraHeight = 2;
        private const int SearchBarHeight = 20;

        // Margins and padding
        private static readonly RectOffset ZeroRectOffset = new RectOffset(0, 0, 0, 0);

        // Card margins and padding
        private static readonly RectOffset CardMargin = new RectOffset(8, 8, 4, 4);
        private static readonly RectOffset CardPadding = new RectOffset(12, 12, 8, 8);
        private static readonly RectOffset HelpBoxPadding = new RectOffset(6, 6, 4, 4);
        private static readonly RectOffset HelpBoxMargin = new RectOffset(2, 2, 2, 2);
        private static readonly RectOffset ButtonIconPadding = new RectOffset(4, 0, 0, 0);

        // Window margins and padding
        private static readonly RectOffset WindowTabPadding = new RectOffset(4, 4, 2, 2);
        private static readonly RectOffset WindowSearchBarPadding = new RectOffset(6, 6, 4, 4);
        private static readonly RectOffset WindowHeaderPadding = new RectOffset(6, 6, 2, 2);
        private static readonly RectOffset WindowHeaderMargin = new RectOffset(0, 0, 0, 1);
        private static readonly RectOffset WindowLabelPadding = new RectOffset(2, 2, 1, 1);
        private static readonly RectOffset WindowCategoryBtnBorder = new RectOffset(1, 1, 1, 1);
        private static readonly RectOffset WindowItemBtnPadding = new RectOffset(6, 6, 3, 3);
        private static readonly RectOffset WindowInfoBoxPadding = new RectOffset(10, 10, 6, 6);
        private static readonly RectOffset WindowInfoBoxBorder = new RectOffset(2, 2, 2, 2);
        private static readonly RectOffset WindowBottomBarPadding = new RectOffset(10, 10, 4, 4);
        private static readonly RectOffset WindowItemAreaPadding = new RectOffset(0, 0, 4, 0);

        // Font sizes
        private const int WindowHeaderLabelFontSize = 11;
        private const int WindowTitleLabelFontSize = 12;
        private const int WindowPinnedLabelFontSize = 9;
        private const int DialogMessageFontSize = 13;

        // GUIStyles
        private static GUIStyle InternalCardStyleStyle;
        private static GUIStyle InternalDarkHelpBoxStyle;
        private static GUIStyle InternalButtonWithIconPaddingStyle;
        private static GUIStyle InternalWrappedTextAreaStyle;
        private static GUIStyle InternalTextAreaStyle;
        private static GUIStyle InternalFoldoutStyle;
        private static GUIStyle InternalUnselectedTabStyle;
        private static GUIStyle InternalSelectedTabStyle;
        private static GUIStyle InternalLabelHighlightedInfoFieldStyle;
        private static GUIStyle InternalButtonStyle;
        private static GUIStyle InternalLabelMessageStyle;
        private static GUIStyle InternalSearchPlaceholderStyle;
        private static GUIStyle InternalToolbarSearchFieldStyle;

        // Window style backing fields
        private static GUIStyle InternalWindowSearchBarBgStyle;
        private static GUIStyle InternalWindowHeaderStyle;
        private static GUIStyle InternalWindowHeaderLabelStyle;
        private static GUIStyle InternalWindowLabelStyle;
        private static GUIStyle InternalWindowEmptyLabelStyle;
        private static GUIStyle InternalWindowCategoryBtnStyle;
        private static GUIStyle InternalWindowCategoryBtnSelectedStyle;
        private static GUIStyle InternalWindowItemBtnStyle;
        private static GUIStyle InternalWindowItemBtnSelectedStyle;
        private static GUIStyle InternalWindowInfoBoxStyle;
        private static GUIStyle InternalWindowBottomAreaStyle;
        private static GUIStyle InternalWindowTitleLabelStyle;
        private static GUIStyle InternalWindowDescLabelStyle;
        private static GUIStyle InternalWindowPinnedLabelStyle;
        private static GUIStyle InternalWindowCategoryAreaStyle;
        private static GUIStyle InternalWindowItemAreaStyle;
        private static GUIStyle InternalWindowBoldLabelStyle;
        private static GUIStyle InternalWindowToolbarButtonStyle;
        private static GUIStyle InternalWindowConfigLabelStyle;
        private static GUIStyle InternalWindowToggleStyle;

        /// <summary>
        /// Gets the wrapped text area style.
        /// </summary>
        internal static GUIStyle FoldoutStyle
        {
            get
            {
                if (InternalFoldoutStyle == null)
                {
                    InternalFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                    {
                        fontStyle = EditorStyles.boldLabel.fontStyle,
                        fontSize = EditorStyles.boldLabel.fontSize
                    };

                    InternalFoldoutStyle.normal.textColor = EditorStyles.boldLabel.normal.textColor;
                }

                return InternalFoldoutStyle;
            }
        }

        /// <summary>
        /// Gets the selected tab style.
        /// </summary>
        internal static GUIStyle SelectedTabStyle
        {
            get
            {
                if (InternalSelectedTabStyle == null)
                {
                    InternalSelectedTabStyle = new GUIStyle
                    {
                        normal = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowTabSelectedBgColor, ref ScriptablesStylesTextures.WindowTabSelectedBgTexture), textColor = Color.white },
                        hover = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowTabSelectedBgColor, ref ScriptablesStylesTextures.WindowTabSelectedBgTexture), textColor = Color.white },
                        alignment = TextAnchor.MiddleCenter,
                        fontStyle = FontStyle.Bold,
                        padding = WindowTabPadding,
                        margin = ZeroRectOffset
                    };
                }

                return InternalSelectedTabStyle;
            }
        }

        /// <summary>
        /// Gets the unselected tab style.
        /// </summary>
        internal static GUIStyle UnselectedTabStyle
        {
            get
            {
                if (InternalUnselectedTabStyle == null)
                {
                    InternalUnselectedTabStyle = new GUIStyle
                    {
                        normal = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowTabUnselectedBgColor, ref ScriptablesStylesTextures.WindowTabUnselectedBgTexture), textColor = Color.white },
                        hover = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.HoverColor, ref ScriptablesStylesTextures.WindowHoverBgTexture), textColor = Color.white },
                        alignment = TextAnchor.MiddleCenter,
                        padding = WindowTabPadding,
                        margin = ZeroRectOffset
                    };
                }

                return InternalUnselectedTabStyle;
            }
        }

        /// <summary>
        /// Gets the wrapped text area style.
        /// </summary>
        internal static GUIStyle WrappedTextAreaStyle
        {
            get
            {
                if (InternalWrappedTextAreaStyle == null)
                {
                    InternalWrappedTextAreaStyle = new GUIStyle(EditorStyles.textArea)
                    {
                        wordWrap = true
                    };
                }

                return InternalWrappedTextAreaStyle;
            }
        }

        /// <summary>
        /// Gets the text area style.
        /// </summary>
        internal static GUIStyle TextAreaStyle
        {
            get
            {
                if (InternalTextAreaStyle == null)
                {
                    InternalTextAreaStyle = GUI.skin.textArea;
                }

                return InternalTextAreaStyle;
            }
        }

        /// <summary>
        /// Gets the card container style with rounded corners, dark background, and optimal spacing.
        /// Ideal for grouping related content in custom inspectors.
        /// </summary>
        internal static GUIStyle CardStyle
        {
            get
            {
                if (InternalCardStyleStyle == null)
                {
                    InternalCardStyleStyle = new GUIStyle(EditorStyles.helpBox)
                    {
                        margin = CardMargin,
                        padding = CardPadding,
                        border = new RectOffset(CardCornerRadius, CardCornerRadius, CardCornerRadius, CardCornerRadius),
                        overflow = ZeroRectOffset,
                    };

                    InternalCardStyleStyle.normal.background = ScriptablesStylesTextures.GetRoundedRectTexture(CardCornerRadius, ScriptablesStylesColors.CardBackground, ScriptablesStylesColors.CardBorder, ref ScriptablesStylesTextures.CardRoundedRectTexture);
                }

                return InternalCardStyleStyle;
            }
        }

        /// <summary>
        /// Gets the dark help box style with rounded corners and subtle border.
        /// Darker than the standard help box for better contrast in custom editors.
        /// </summary>
        internal static GUIStyle DarkHelpBox
        {
            get
            {
                if (InternalDarkHelpBoxStyle == null)
                {
                    InternalDarkHelpBoxStyle = new GUIStyle(EditorStyles.helpBox)
                    {
                        border = new RectOffset(HelpBoxBorderWidth, HelpBoxBorderWidth, HelpBoxBorderWidth, HelpBoxBorderWidth),
                        padding = HelpBoxPadding,
                        margin = HelpBoxMargin
                    };

                    InternalDarkHelpBoxStyle.normal.background = ScriptablesStylesTextures.GetRoundedRectTexture(DarkHelpBoxSCornerRadius, ScriptablesStylesColors.HelpBoxBackground, ScriptablesStylesColors.HelpBoxBorder, ref ScriptablesStylesTextures.HelpBoxRoundedRectTexture);
                }

                return InternalDarkHelpBoxStyle;
            }
        }

        /// <summary>
        /// Gets the button style.
        /// </summary>
        internal static GUIStyle ButtonStyle
        {
            get
            {
                if (InternalButtonStyle == null)
                {
                    InternalButtonStyle = GUI.skin.button;
                }

                return InternalButtonStyle;
            }
        }

        /// <summary>
        /// Gets the base button style with custom padding.
        /// </summary>
        internal static GUIStyle ButtonWithIconPaddingStyle
        {
            get
            {
                if (InternalButtonWithIconPaddingStyle == null)
                {
                    InternalButtonWithIconPaddingStyle = new GUIStyle(ButtonStyle);

                    InternalButtonWithIconPaddingStyle.padding = new RectOffset(
                        ButtonStyle.padding.left + ButtonIconPadding.left,
                        ButtonStyle.padding.right + ButtonIconPadding.right,
                        ButtonStyle.padding.top + ButtonIconPadding.top,
                        ButtonStyle.padding.bottom + ButtonIconPadding.bottom
                    );
                }

                return InternalButtonWithIconPaddingStyle;
            }
        }

        /// <summary>
        /// Gets the base centered grey mini label.
        /// </summary>
        internal static GUIStyle CenteredGreyMiniLabel
        {
            get
            {
                return EditorStyles.centeredGreyMiniLabel;
            }
        }

        /// <summary>
        /// Gets the base title style.
        /// </summary>
        internal static GUIStyle LabelTitleFieldStyle
        {
            get
            {
                return EditorStyles.boldLabel;
            }
        }

        /// <summary>
        /// Gets the base info style.
        /// </summary>
        internal static GUIStyle LabelInfoFieldStyle
        {
            get
            {
                return EditorStyles.miniLabel;
            }
        }

        /// <summary>
        /// Gets the highlighted info style.
        /// </summary>
        internal static GUIStyle LabelHighlightedInfoFieldStyle
        {
            get
            {
                if (InternalLabelHighlightedInfoFieldStyle == null)
                {
                    InternalLabelHighlightedInfoFieldStyle = new GUIStyle(LabelInfoFieldStyle)
                    {
                        fontStyle = FontStyle.Bold
                    };
                }

                return InternalLabelHighlightedInfoFieldStyle;
            }
        }

        /// <summary>
        /// Gets the base info style.
        /// </summary>
        internal static GUIStyle LabelFieldStyle
        {
            get
            {
                return EditorStyles.label;
            }
        }

        /// <summary>
        /// Gets the base info style.
        /// </summary>
        internal static GUIStyle LabelMessageStyle
        {
            get
            {
                if (InternalLabelMessageStyle == null)
                {
                    InternalLabelMessageStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = DialogMessageFontSize,
                        padding = new RectOffset(MessagePadding, MessagePadding, 0, 0)
                    };
                }

                return InternalLabelMessageStyle;
            }
        }

        /// <summary>
        /// Gets the toolbar search field style.
        /// </summary>
        internal static GUIStyle ToolbarSearchFieldStyle
        {
            get
            {
                if (InternalToolbarSearchFieldStyle == null)
                {
                    InternalToolbarSearchFieldStyle = new GUIStyle(EditorStyles.toolbarSearchField)
                    {
                        fixedHeight = SearchBarHeight
                    };

                    InternalToolbarSearchFieldStyle.padding.top += SearchBarHalfExtraHeight;
                    InternalToolbarSearchFieldStyle.padding.bottom += SearchBarHalfExtraHeight;
                }

                return InternalToolbarSearchFieldStyle;
            }
        }

        /// <summary>
        /// Gets the search placeholder style.
        /// </summary>
        internal static GUIStyle SearchPlaceholderStyle
        {
            get
            {
                if (InternalSearchPlaceholderStyle == null)
                {
                    InternalSearchPlaceholderStyle = new GUIStyle(LabelFieldStyle)
                    {
                        alignment = TextAnchor.MiddleLeft
                    };

                    InternalSearchPlaceholderStyle.padding = new RectOffset(
                        ToolbarSearchFieldStyle.padding.left, 0,
                        ToolbarSearchFieldStyle.padding.top, ToolbarSearchFieldStyle.padding.bottom
                    );

                    InternalSearchPlaceholderStyle.normal.textColor = Color.gray;
                }

                return InternalSearchPlaceholderStyle;
            }
        }

        /// <summary>
        /// Gets the window search bar background style.
        /// </summary>
        internal static GUIStyle WindowSearchBarBgStyle
        {
            get
            {
                if (InternalWindowSearchBarBgStyle == null)
                {
                    InternalWindowSearchBarBgStyle = new GUIStyle
                    {
                        normal = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowContainerBgColor, ref ScriptablesStylesTextures.WindowSearchBarBgTexture) },
                        padding = WindowSearchBarPadding,
                        margin = ZeroRectOffset
                    };
                }

                return InternalWindowSearchBarBgStyle;
            }
        }

        /// <summary>
        /// Gets the window header background style.
        /// </summary>
        internal static GUIStyle WindowHeaderStyle
        {
            get
            {
                if (InternalWindowHeaderStyle == null)
                {
                    InternalWindowHeaderStyle = new GUIStyle
                    {
                        normal = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowContainerBgColor, ref ScriptablesStylesTextures.WindowHeaderBgTexture), textColor = Color.white },
                        padding = WindowHeaderPadding,
                        margin = WindowHeaderMargin
                    };
                }

                return InternalWindowHeaderStyle;
            }
        }

        /// <summary>
        /// Gets the window header label style.
        /// </summary>
        internal static GUIStyle WindowHeaderLabelStyle
        {
            get
            {
                if (InternalWindowHeaderLabelStyle == null)
                {
                    InternalWindowHeaderLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        normal = { textColor = Color.white },
                        fontSize = WindowHeaderLabelFontSize
                    };
                }

                return InternalWindowHeaderLabelStyle;
            }
        }

        /// <summary>
        /// Gets the window label style with white text.
        /// </summary>
        internal static GUIStyle WindowLabelStyle
        {
            get
            {
                if (InternalWindowLabelStyle == null)
                {
                    InternalWindowLabelStyle = new GUIStyle(EditorStyles.label)
                    {
                        normal = { textColor = Color.white },
                        padding = WindowLabelPadding
                    };
                }

                return InternalWindowLabelStyle;
            }
        }

        /// <summary>
        /// Gets the window empty state label style.
        /// </summary>
        internal static GUIStyle WindowEmptyLabelStyle
        {
            get
            {
                if (InternalWindowEmptyLabelStyle == null)
                {
                    InternalWindowEmptyLabelStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        normal = { textColor = ScriptablesStylesColors.WindowEmptyLabelColor }
                    };
                }

                return InternalWindowEmptyLabelStyle;
            }
        }

        /// <summary>
        /// Gets the window category button style.
        /// </summary>
        internal static GUIStyle WindowCategoryBtnStyle
        {
            get
            {
                if (InternalWindowCategoryBtnStyle == null)
                {
                    InternalWindowCategoryBtnStyle = new GUIStyle
                    {
                        normal = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowTabUnselectedBgColor, ref ScriptablesStylesTextures.WindowTabUnselectedBgTexture), textColor = Color.white },
                        hover = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.HoverColor, ref ScriptablesStylesTextures.WindowHoverBgTexture), textColor = Color.white },
                        alignment = TextAnchor.MiddleLeft,
                        padding = WindowSearchBarPadding,
                        margin = WindowHeaderMargin,
                        border = WindowCategoryBtnBorder
                    };
                }

                return InternalWindowCategoryBtnStyle;
            }
        }

        /// <summary>
        /// Gets the window category button selected style.
        /// </summary>
        internal static GUIStyle WindowCategoryBtnSelectedStyle
        {
            get
            {
                if (InternalWindowCategoryBtnSelectedStyle == null)
                {
                    InternalWindowCategoryBtnSelectedStyle = new GUIStyle(WindowCategoryBtnStyle)
                    {
                        normal = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowTabSelectedBgColor, ref ScriptablesStylesTextures.WindowTabSelectedBgTexture), textColor = Color.white },
                        hover = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowTabSelectedBgColor, ref ScriptablesStylesTextures.WindowTabSelectedBgTexture), textColor = Color.white },
                        fontStyle = FontStyle.Bold
                    };
                }

                return InternalWindowCategoryBtnSelectedStyle;
            }
        }

        /// <summary>
        /// Gets the window item button style.
        /// </summary>
        internal static GUIStyle WindowItemBtnStyle
        {
            get
            {
                if (InternalWindowItemBtnStyle == null)
                {
                    InternalWindowItemBtnStyle = new GUIStyle
                    {
                        normal = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.RowBackgroundColor, ref ScriptablesStylesTextures.WindowItemNormalBgTexture), textColor = Color.white },
                        hover = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.HoverColor, ref ScriptablesStylesTextures.WindowHoverBgTexture), textColor = Color.white },
                        alignment = TextAnchor.MiddleLeft,
                        padding = WindowItemBtnPadding,
                        margin = WindowHeaderMargin,
                        border = WindowCategoryBtnBorder
                    };
                }

                return InternalWindowItemBtnStyle;
            }
        }

        /// <summary>
        /// Gets the window item button selected style.
        /// </summary>
        internal static GUIStyle WindowItemBtnSelectedStyle
        {
            get
            {
                if (InternalWindowItemBtnSelectedStyle == null)
                {
                    InternalWindowItemBtnSelectedStyle = new GUIStyle(WindowItemBtnStyle)
                    {
                        normal = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowItemSelectedBgColor, ref ScriptablesStylesTextures.WindowItemSelectedBgTexture), textColor = ScriptablesStylesColors.WindowItemSelectedTextColor },
                        hover = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowItemSelectedBgColor, ref ScriptablesStylesTextures.WindowItemSelectedBgTexture), textColor = ScriptablesStylesColors.WindowItemSelectedTextColor },
                        fontStyle = FontStyle.Bold
                    };
                }

                return InternalWindowItemBtnSelectedStyle;
            }
        }

        /// <summary>
        /// Gets the window info box style.
        /// </summary>
        internal static GUIStyle WindowInfoBoxStyle
        {
            get
            {
                if (InternalWindowInfoBoxStyle == null)
                {
                    InternalWindowInfoBoxStyle = new GUIStyle
                    {
                        normal = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowContainerBgColor, ref ScriptablesStylesTextures.WindowInfoBgTexture), textColor = Color.white },
                        padding = WindowInfoBoxPadding,
                        margin = ZeroRectOffset,
                        border = WindowInfoBoxBorder
                    };
                }

                return InternalWindowInfoBoxStyle;
            }
        }

        /// <summary>
        /// Gets the window bottom area style.
        /// </summary>
        internal static GUIStyle WindowBottomAreaStyle
        {
            get
            {
                if (InternalWindowBottomAreaStyle == null)
                {
                    InternalWindowBottomAreaStyle = new GUIStyle
                    {
                        normal = { background = ScriptablesStylesTextures.GetOrCreateSolidTexture(ScriptablesStylesColors.WindowBottomBarBgColor, ref ScriptablesStylesTextures.WindowBottomBarBgTexture) },
                        padding = WindowBottomBarPadding,
                        margin = ZeroRectOffset
                    };
                }

                return InternalWindowBottomAreaStyle;
            }
        }

        /// <summary>
        /// Gets the window title label style.
        /// </summary>
        internal static GUIStyle WindowTitleLabelStyle
        {
            get
            {
                if (InternalWindowTitleLabelStyle == null)
                {
                    InternalWindowTitleLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        normal = { textColor = Color.white },
                        fontSize = WindowTitleLabelFontSize
                    };
                }

                return InternalWindowTitleLabelStyle;
            }
        }

        /// <summary>
        /// Gets the window description label style.
        /// </summary>
        internal static GUIStyle WindowDescLabelStyle
        {
            get
            {
                if (InternalWindowDescLabelStyle == null)
                {
                    InternalWindowDescLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                    {
                        normal = { textColor = ScriptablesStylesColors.WindowDescriptionTextColor },
                        wordWrap = true,
                        fontSize = WindowHeaderLabelFontSize
                    };
                }

                return InternalWindowDescLabelStyle;
            }
        }

        /// <summary>
        /// Gets the window pinned label style.
        /// </summary>
        internal static GUIStyle WindowPinnedLabelStyle
        {
            get
            {
                if (InternalWindowPinnedLabelStyle == null)
                {
                    InternalWindowPinnedLabelStyle = new GUIStyle(EditorStyles.miniBoldLabel)
                    {
                        normal = { textColor = ScriptablesStylesColors.WindowPinnedLabelColor },
                        fontSize = WindowPinnedLabelFontSize
                    };
                }

                return InternalWindowPinnedLabelStyle;
            }
        }

        /// <summary>
        /// Gets the window category area style.
        /// </summary>
        internal static GUIStyle WindowCategoryAreaStyle
        {
            get
            {
                if (InternalWindowCategoryAreaStyle == null)
                {
                    InternalWindowCategoryAreaStyle = new GUIStyle
                    {
                        padding = ZeroRectOffset,
                        margin = ZeroRectOffset
                    };
                }

                return InternalWindowCategoryAreaStyle;
            }
        }

        /// <summary>
        /// Gets the window item area style.
        /// </summary>
        internal static GUIStyle WindowItemAreaStyle
        {
            get
            {
                if (InternalWindowItemAreaStyle == null)
                {
                    InternalWindowItemAreaStyle = new GUIStyle
                    {
                        padding = WindowItemAreaPadding,
                        margin = ZeroRectOffset
                    };
                }

                return InternalWindowItemAreaStyle;
            }
        }

        /// <summary>
        /// Gets the window bold label style.
        /// </summary>
        internal static GUIStyle WindowBoldLabelStyle
        {
            get
            {
                if (InternalWindowBoldLabelStyle == null)
                {
                    InternalWindowBoldLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        normal = { textColor = Color.white }
                    };
                }

                return InternalWindowBoldLabelStyle;
            }
        }

        /// <summary>
        /// Gets the window toolbar button style.
        /// </summary>
        internal static GUIStyle WindowToolbarButtonStyle
        {
            get
            {
                if (InternalWindowToolbarButtonStyle == null)
                {
                    InternalWindowToolbarButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
                }

                return InternalWindowToolbarButtonStyle;
            }
        }

        /// <summary>
        /// Gets the window config label style.
        /// </summary>
        internal static GUIStyle WindowConfigLabelStyle
        {
            get
            {
                if (InternalWindowConfigLabelStyle == null)
                {
                    InternalWindowConfigLabelStyle = new GUIStyle(EditorStyles.label)
                    {
                        normal = { textColor = Color.white },
                    };
                }

                return InternalWindowConfigLabelStyle;
            }
        }

        /// <summary>
        /// Gets the window toggle style.
        /// </summary>
        internal static GUIStyle WindowToggleStyle
        {
            get
            {
                if (InternalWindowToggleStyle == null)
                {
                    InternalWindowToggleStyle = new GUIStyle(EditorStyles.toggle)
                    {
                        fontStyle = FontStyle.Bold,
                        normal = { textColor = Color.white }
                    };
                }

                return InternalWindowToggleStyle;
            }
        }

        /// <summary>
        /// Draws a horizontal line in the Editor with customizable color, thickness, and spacing.
        /// </summary>
        /// <param name="color">The color of the line. Defaults to editor theme color.</param>
        /// <param name="thickness">The height of the line in pixels.</param>
        /// <param name="paddingTop">The vertical space above the line in pixels.</param>
        /// <param name="paddingBottom">The vertical space below the line in pixels. Set to 0 for no spacing.</param>
        /// <param name="widthPercentage">The width of the line as a percentage of the available space (0-1).</param>
        /// <param name="horizontalMargin">The horizontal margin on both sides of the line in pixels.</param>
        internal static void DrawLine(
            Color color = default,
            int thickness = LineDefaultThickness,
            int paddingTop = LineDefaultSpace,
            int paddingBottom = LineDefaultSpace,
            float widthPercentage = LineDefaultWidthPercentage,
            float horizontalMargin = LineDefaultHorizontalMargin)
        {
            if (color.Equals(default))
            {
                color = ScriptablesStylesColors.DefaultLineColor;
            }

            // Calculate total height: top padding + line thickness + bottom padding
            int totalHeight = paddingTop + thickness + paddingBottom;
            
            Rect rec = EditorGUILayout.GetControlRect(false, GUILayout.Height(totalHeight));
            rec.height = thickness;
            rec.y += paddingTop;

            float margin = Mathf.Max(0, horizontalMargin);
            float availableWidth = rec.width - margin * 2;
            if (availableWidth < 0) availableWidth = 0;

            float finalWidth = availableWidth * Mathf.Clamp01(widthPercentage);
            float offset = (rec.width - finalWidth) / 2f;

            rec.x += offset;
            rec.width = finalWidth;

            EditorGUI.DrawRect(rec, color);
        }
    }
}
#endif
