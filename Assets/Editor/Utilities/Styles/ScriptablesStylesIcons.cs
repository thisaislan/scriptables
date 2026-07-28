#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.Styles
{
    /// <summary>
    /// Centralized icon provider for custom editor UI elements.
    /// Provides icon type enums and unified methods for loading small custom icons,
    /// built-in editor icons, and button icon names.
    /// </summary>
    internal static class ScriptablesStylesIcons
    {
        /// <summary>
        /// Available icon types for editor UI elements.
        /// </summary>
        internal enum IconType
        {
            /// <summary>Main scriptables tab icon.</summary>
            SmallMainScriptables,
            /// <summary>Settings tab icon.</summary>
            SmallSettings,
            /// <summary>Runtime tab icon.</summary>
            SmallRuntime,
            /// <summary>Reactive tab icon.</summary>
            SmallReactive,
            /// <summary>ScriptableObject tab icon.</summary>
            SmallScriptableObject,
            /// <summary>Default ScriptableObject icon.</summary>
            Default,
            /// <summary>Create Add New icon.</summary>
            CreateAddNew,
            /// <summary>Script file icon.</summary>
            Script,
            /// <summary>Info icon used in info cards and messages.</summary>
            Info,
            /// <summary>Pin icon.</summary>
            Pin
        }

        /// <summary>
        /// Represents the available icon types used for button visuals in the editor interface.
        /// </summary>
        internal enum ButtonIcon
        {
            /// <summary>No icon at all.</summary>
            None,
            /// <summary>Pencil or edit symbol - used for editing/rename actions.</summary>
            EditIcon,
            /// <summary>Floppy disk or save symbol - used for save/persist actions.</summary>
            SaveIcon,
            /// <summary>Printer symbol - used for print/export actions.</summary>
            PrintIcon,
            /// <summary>Play symbol - used for alerts/notifications.</summary>
            PlayIcon,
            /// <summary>X or clear symbol - used for delete/clear/reset actions.</summary>
            ClearIcon,
            /// <summary>Standard refresh icon - used for refresh/reload actions.</summary>
            RefreshIcon,
            /// <summary>Selection icon - used for selection/mark actions.</summary>
            SelectIcon,
            /// <summary>Magnifying glass icon - used for search/find actions.</summary>
            SearchIcon,
            /// <summary>Stop icon - used for stop/interrupt actions.</summary>
            StopIcon
        }

        private const string SmallMainScriptablesIcon = "small_main_scriptables_icon";
        private const string SmallSettingsIcon = "small_settings_icon";
        private const string SmallRuntimeIcon = "small_runtime_icon";
        private const string SmallReactiveIcon = "small_reactive_icon";
        private const string SmallScriptableObjectIcon = "small_scriptable_object_icon";

        private const string DefaultIcon = "d_ScriptableObject Icon";
        private const string CreateAddNewIcon = "d_CreateAddNew";
        private const string ScriptIcon = "d_cs Script Icon";
        private const string InfoIcon = "console.infoicon.sml";
        private const string PinIcon = "Pin";

        private const string EditButtonIcon = "d_editicon.sml";
        private const string SaveButtonIcon = "d_SaveAs";
        private const string PrintButtonIcon = "d_UnityEditor.ConsoleWindow";
        private const string PlayButtonIcon = "d_Animation.Play";
        private const string ClearButtonIcon = "d_clear";
        private const string RefreshButtonIcon = "d_Refresh";
        private const string SelectButtonIcon = "d_FilterSelectedOnly";
        private const string SearchButtonIcon = "d_Search Icon";
        private const string StopButtonIcon = "d_StopButton";

        /// <summary>
        /// Loads an icon by its type, either from Resources/EditorIcons/ or from built-in Unity editor icons.
        /// </summary>
        internal static Texture GetIconTexture(IconType iconType)
        {
            string iconName = GetIconName(iconType);

            Texture2D smallIcon = Resources.Load<Texture2D>(iconName);

            if (smallIcon != null)
            {
                return smallIcon;
            }

            return EditorGUIUtility.IconContent(iconName).image;
        }

        /// <summary>
        /// Gets the Unity Editor built-in icon content name for the specified button icon.
        /// </summary>
        internal static string GetButtonIconName(ButtonIcon buttonIcon)
        {
            switch (buttonIcon)
            {
                case ButtonIcon.EditIcon: return EditButtonIcon;
                case ButtonIcon.SaveIcon: return SaveButtonIcon;
                case ButtonIcon.PrintIcon: return PrintButtonIcon;
                case ButtonIcon.PlayIcon: return PlayButtonIcon;
                case ButtonIcon.ClearIcon: return ClearButtonIcon;
                case ButtonIcon.RefreshIcon: return RefreshButtonIcon;
                case ButtonIcon.SelectIcon: return SelectButtonIcon;
                case ButtonIcon.SearchIcon: return SearchButtonIcon;
                case ButtonIcon.StopIcon: return StopButtonIcon;
                default: return null;
            }
        }

        /// <summary>
        /// Gets the built-in Unity Editor icon name string for the specified icon type.
        /// </summary>
        /// <param name="iconType">The icon type to retrieve the name for.</param>
        internal static string GetIconName(IconType iconType)
        {
            switch (iconType)
            {
                case IconType.SmallMainScriptables: return SmallMainScriptablesIcon;
                case IconType.SmallSettings: return SmallSettingsIcon;
                case IconType.SmallRuntime: return SmallRuntimeIcon;
                case IconType.SmallReactive: return SmallReactiveIcon;
                case IconType.SmallScriptableObject: return SmallScriptableObjectIcon;
                case IconType.Default: return DefaultIcon;
                case IconType.CreateAddNew: return CreateAddNewIcon;
                case IconType.Script: return ScriptIcon;
                case IconType.Info: return InfoIcon;
                case IconType.Pin: return PinIcon;
                default: return null;
            }
        }
    }
}
#endif
