#if UNITY_EDITOR
using System;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.Widgets
{
    /// <summary>
    /// A custom modal dialog window for Unity Editor that provides a flexible message display
    /// with configurable positive and negative action buttons. Supports keyboard shortcuts
    /// (Enter for positive action, Escape for negative action).
    /// </summary>
    internal class DisplayDialog : EditorWindow
    {
        private const float DialogWidth = 420f;
        private const float MinDialogHeight = 140f;
        private const float ButtonSpacing = 8f;
        private const float BottomSpacing = 12f;
        private const float EndButtonSpacing = 12f;
        private const float ButtonHeight = 30f;
        private const float ButtonWidth = 100f;

        private string message;
        private DialogButtonSettings negativeButtonSettings;
        private DialogButtonSettings positiveButtonSettings;

        /// <summary>
        /// Displays a modal dialog window with the specified title, message, and button configurations.
        /// </summary>
        /// <param name="title">Dialog window title</param>
        /// <param name="message">Message content to display in the dialog</param>
        /// <param name="negativeButtonSettings">Configuration for the negative/cancel button (required)</param>
        /// <param name="positiveButtonSettings">Configuration for the positive/confirm button (optional)</param>
        internal static void Show(
            string title,
            string message,
            DialogButtonSettings negativeButtonSettings,
            DialogButtonSettings positiveButtonSettings = default)
        {
            DisplayDialog window = CreateInstance<DisplayDialog>();
            window.titleContent = new GUIContent(title);
            window.message = message;
            window.negativeButtonSettings = negativeButtonSettings;
            window.positiveButtonSettings = positiveButtonSettings;
            ConfigureWindow(window);
            window.ShowModalUtility();
        }

        /// <summary>
        /// Immutable configuration structure for dialog button settings.
        /// </summary>
        internal readonly struct DialogButtonSettings
        {
            internal ScriptablesStylesColors.ButtonColorStyle buttonColorStyle { get; }
            internal ScriptablesStylesIcons.ButtonIcon buttonIcon { get; }
            internal string label { get; }
            internal string tooltip { get; }
            internal Action onClickAction { get; }

            /// <summary>
            /// Initializes a new instance of DialogButtonSettings.
            /// </summary>
            /// <param name="buttonColorStyle">Color styling for the button (Neutral/Quiet/Urgent/Growth)</param>
            /// <param name="buttonIcon">Icon to display on the button</param>
            /// <param name="label">Button label text</param>
            /// <param name="tooltip">Tooltip text shown on hover</param>
            /// <param name="onClickAction">Callback invoked when button is clicked</param>
            internal DialogButtonSettings(
                ScriptablesStylesColors.ButtonColorStyle buttonColorStyle,
                ScriptablesStylesIcons.ButtonIcon buttonIcon,
                string label,
                string tooltip,
                Action onClickAction = null)
            {
                this.buttonColorStyle = buttonColorStyle;
                this.buttonIcon = buttonIcon;
                this.label = label;
                this.tooltip = tooltip;
                this.onClickAction = onClickAction;
            }
        }

        private static void ConfigureWindow(DisplayDialog window)
        {
            float textHeight = ScriptablesStyles.LabelMessageStyle.CalcHeight(
                new GUIContent(window.message), DialogWidth);

            float dialogHeight = Mathf.Max(
                MinDialogHeight,
                textHeight + 90f);

            window.minSize = new Vector2(DialogWidth, dialogHeight);
            window.maxSize = new Vector2(DialogWidth, dialogHeight);

            window.position = new Rect(
                (Screen.currentResolution.width - DialogWidth) / 2f,
                (Screen.currentResolution.height - dialogHeight) / 2f,
                DialogWidth,
                dialogHeight);
        }

        private void OnGUI()
        {
            HandleKeyboardEvents();

            GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            {
                GUILayout.FlexibleSpace();

                GUILayout.Label(message, ScriptablesStyles.LabelMessageStyle, GUILayout.ExpandWidth(true));

                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();

                if (!positiveButtonSettings.Equals(default(DialogButtonSettings)))
                {
                    DrawButton(positiveButtonSettings);

                    GUILayout.Space(ButtonSpacing);
                }

                DrawButton(negativeButtonSettings);

                GUILayout.Space(EndButtonSpacing);

                GUILayout.EndHorizontal();

                GUILayout.Space(BottomSpacing);
            }
            GUILayout.EndVertical();
        }

        private void DrawButton(DialogButtonSettings buttonSettings)
        {
            ButtonPalette.DrawButton(
                label: buttonSettings.label,
                action: () => { buttonSettings.onClickAction?.Invoke(); Close(); },
                buttonIcon: buttonSettings.buttonIcon,
                style: buttonSettings.buttonColorStyle,
                tooltip: buttonSettings.tooltip,
                fixedWidth: ButtonWidth,
                customHeight: ButtonHeight
            );
        }

        private void HandleKeyboardEvents()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
                {
                    positiveButtonSettings.onClickAction?.Invoke();
                    AffterKeyEvent();
                }
                else if (Event.current.keyCode == KeyCode.Escape)
                {
                    negativeButtonSettings.onClickAction?.Invoke();
                    AffterKeyEvent();
                }
            }
        }

        private void AffterKeyEvent()
        {
            Close();
            Event.current.Use();
        }
    }
}
#endif
