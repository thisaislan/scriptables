using UnityEditor;
using System.IO;

namespace Thisaislan.Scriptables.Editor.MenuItems
{
    public static class CreateCustomScriptableEditorMenu
    {
        /* =========================================================
        * CONSTANTS
        * ========================================================= */

        // Root menu
        private const string ROOT_MENU_PATH = "Assets/Create/Scripting/Custom Scriptable/";
        private const int ROOT_MENU_ORDER = 20;

        // Menu names
        private const string SETTINGS_MENU_NAME = "Settings";
        private const string RUNTIME_MENU_NAME  = "Runtime";
        private const string REACTIVE_MENU_NAME = "Reactive";

        // Suffixes
        private const string SETTINGS_SUFFIX = "ScriptableSettings";
        private const string RUNTIME_SUFFIX  = "ScriptableRuntime";
        private const string DATA_SUFFIX     = "Data";

        // Save dialog
        private const string SAVE_EXTENSION = "cs";

        // Asset menu roots
        private const string SETTINGS_ASSET_MENU_ROOT = "Custom Scriptables/Settings/";
        private const string RUNTIME_ASSET_MENU_ROOT  = "Custom Scriptables/Runtime/";
        private const string REACTIVE_ASSET_MENU_ROOT = "Custom Scriptables/Reactive/";
        private const int ASSET_MENU_ROOT_ORDER = 10;


        /* =========================================================
        * MENU ENTRIES
        * ========================================================= */

        [MenuItem(ROOT_MENU_PATH + SETTINGS_MENU_NAME, false, ROOT_MENU_ORDER)]
        public static void CreateSettings()
        {
            CreateWithDialog(
                title: "Create Scriptable Settings",
                defaultName: "ScriptableSettings",
                message: "Enter name for the new scriptable settings",
                creator: CreateSettingsScript
            );
        }

        [MenuItem(ROOT_MENU_PATH + RUNTIME_MENU_NAME, false, ROOT_MENU_ORDER)]
        public static void CreateRuntime()
        {
            CreateWithDialog(
                title: "Create Scriptable Runtime",
                defaultName: "ScriptableRuntime",
                message: "Enter name for the new scriptable runtime",
                creator: CreateRuntimeScript
            );
        }

        [MenuItem(ROOT_MENU_PATH + REACTIVE_MENU_NAME, false, ROOT_MENU_ORDER)]
        public static void CreateReactive()
        {
            CreateWithDialog(
                title: "Create Scriptable Reactive",
                defaultName: "ScriptableReactive",
                message: "Enter name for the new scriptable reactive",
                creator: CreateReactiveScript
            );
        }


        /* =========================================================
        * DIALOG + DISPATCH
        * ========================================================= */

        /// <summary>
        /// Shows a save dialog and delegates creation logic.
        /// </summary>
        private static void CreateWithDialog(
            string title,
            string defaultName,
            string message,
            System.Action<string> creator
        )
        {
            string folder = GetSelectedFolder();

            string path = EditorUtility.SaveFilePanelInProject(
                title,
                defaultName,
                SAVE_EXTENSION,
                message,
                folder
            );

            if (string.IsNullOrEmpty(path))
                return;

            creator(path);
        }


        /* =========================================================
        * CREATION LOGIC
        * ========================================================= */

        /// <summary>
        /// Shared creator for Scriptables that have a nested Data class
        /// (Settings & Runtime).
        /// </summary>
        private static void CreateDataScriptable(
            string path,
            string suffixToStrip,
            string baseType,
            string assetMenuRoot
        )
        {
            string userName = Path.GetFileNameWithoutExtension(path);

            ResolveNames(
                userName,
                suffixToStrip,
                out string className,
                out string dataName
            );

            string content = $@"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;
using System;

[CreateAssetMenu(fileName = ""{className}"", menuName = ""{assetMenuRoot}{className}"", order = {ASSET_MENU_ROOT_ORDER})]
public class {className} : {baseType}<{className}.{dataName}>
{{
    [Serializable]
    public class {dataName} : Data
    {{
        // Your properties go here
    }}
}}
";

            WriteAndSelectFile(path, content);
        }

        /// <summary>
        /// Creates a ScriptableSettings script.
        /// </summary>
        private static void CreateSettingsScript(string path)
        {
            CreateDataScriptable(
                path,
                SETTINGS_SUFFIX,
                "ScriptableSettings",
                SETTINGS_ASSET_MENU_ROOT
            );
        }

        /// <summary>
        /// Creates a ScriptableRuntime script.
        /// </summary>
        private static void CreateRuntimeScript(string path)
        {
            CreateDataScriptable(
                path,
                RUNTIME_SUFFIX,
                "ScriptableRuntime",
                RUNTIME_ASSET_MENU_ROOT
            );
        }

        /// <summary>
        /// Creates a ScriptableReactive script (self-referenced generic).
        /// </summary>
        private static void CreateReactiveScript(string path)
        {
            string className = Path.GetFileNameWithoutExtension(path);

            string content = $@"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;

[CreateAssetMenu(fileName = ""{className}"", menuName = ""{REACTIVE_ASSET_MENU_ROOT}{className}"", order = {ASSET_MENU_ROOT_ORDER})]
public class {className} : ScriptableReactive<{className}>
{{
    // Methods here if needed
}}
";

            WriteAndSelectFile(path, content);
        }


        /* =========================================================
        * HELPERS
        * ========================================================= */

        /// <summary>
        /// Returns the currently selected folder or Assets.
        /// </summary>
        private static string GetSelectedFolder()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            return string.IsNullOrEmpty(path) ? "Assets" : path;
        }

        /// <summary>
        /// Resolves class name and data name, stripping suffix if present.
        /// </summary>
        private static void ResolveNames(
            string userName,
            string suffixToStrip,
            out string className,
            out string dataName
        )
        {
            className = userName;

            string rootName = userName.EndsWith(suffixToStrip)
                ? userName.Substring(0, userName.Length - suffixToStrip.Length)
                : userName;

            dataName = rootName + DATA_SUFFIX;
        }

        /// <summary>
        /// Writes the file and selects it in the Project window.
        /// </summary>
        private static void WriteAndSelectFile(string path, string content)
        {
            File.WriteAllText(path, content);
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
        }
    }
}