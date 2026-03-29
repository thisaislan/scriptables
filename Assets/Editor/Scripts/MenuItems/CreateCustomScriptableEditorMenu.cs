using UnityEditor;
using System.IO;

namespace Thisaislan.Scriptables.Editor.MenuItems
{
    public static class CreateCustomScriptableEditorMenu
    {
        // Root menu
        private const string RootMenuPath = "Assets/Create/Scripting/Custom Scriptable/";
        private const int RootMenuOrder = 20;

        // Menu names
        private const string SettingsMenuName = "Settings";
        private const string RuntimeMenuName = "Runtime";
        private const string ReactiveMenuName = "Reactive";

        // Suffixes
        private const string SettingsSuffix = "ScriptableSettings";
        private const string RuntimeSuffix = "ScriptableRuntime";
        private const string DataSuffix = "Data";

        // Save dialog
        private const string SaveExtension = "cs";

        // Asset menu roots
        private const string SettingsAssetMenuRoot = "Custom Scriptables/Settings/";
        private const string RuntimeAssetMenuRoot = "Custom Scriptables/Runtime/";
        private const string ReactiveAssetMenuRoot = "Custom Scriptables/Reactive/";
        private const int AssetMenuRootOrder = 10;

        // Dialog constants
        private const string CreateSettingsTitle = "Create Scriptable Settings";
        private const string CreateRuntimeTitle = "Create Scriptable Runtime";
        private const string CreateReactiveTitle = "Create Scriptable Reactive";
        private const string DefaultSettingsName = "ScriptableSettings";
        private const string DefaultRuntimeName = "ScriptableRuntime";
        private const string DefaultReactiveName = "ScriptableReactive";
        private const string CreateSettingsMessage = "Enter name for the new scriptable settings";
        private const string CreateRuntimeMessage = "Enter name for the new scriptable runtime";
        private const string CreateReactiveMessage = "Enter name for the new scriptable reactive";
        private const string FolderCreationDefaultPath = "Assets";
        private const string ScriptableRuntimeType = "ScriptableRuntime";
        private const string ScriptableSettingsType = "ScriptableSettings";

        [MenuItem(RootMenuPath + SettingsMenuName, false, RootMenuOrder)]
        public static void CreateSettings()
        {
            CreateWithDialog(
                title: CreateSettingsTitle,
                defaultName: DefaultSettingsName,
                message: CreateSettingsMessage,
                creator: CreateSettingsScript
            );
        }

        [MenuItem(RootMenuPath + RuntimeMenuName, false, RootMenuOrder)]
        public static void CreateRuntime()
        {
            CreateWithDialog(
                title: CreateRuntimeTitle,
                defaultName: DefaultRuntimeName,
                message: CreateRuntimeMessage,
                creator: CreateRuntimeScript
            );
        }

        [MenuItem(RootMenuPath + ReactiveMenuName, false, RootMenuOrder)]
        public static void CreateReactive()
        {
            CreateWithDialog(
                title: CreateReactiveTitle,
                defaultName: DefaultReactiveName,
                message: CreateReactiveMessage,
                creator: CreateReactiveScript
            );
        }

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
                SaveExtension,
                message,
                folder
            );

            if (string.IsNullOrEmpty(path))
                return;

            creator(path);
        }

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

[CreateAssetMenu(fileName = nameof({className}), menuName = ""{assetMenuRoot}"" + nameof({className}), order = {AssetMenuRootOrder})]
public class {className} : {baseType}<{className}.{dataName}>
{{
    // 🎮 Here is a nice place to put your methods

    [Serializable]
    public class {dataName} : Data
    {{
        // 📝 Add your serialized fields here
    }}
}}
";

            WriteAndSelectFile(path, content);
        }

        private static void CreateSettingsScript(string path)
        {
            CreateDataScriptable(
                path,
                SettingsSuffix,
                ScriptableSettingsType,
                SettingsAssetMenuRoot
            );
        }

        private static void CreateRuntimeScript(string path)
        {
            CreateDataScriptable(
                path,
                RuntimeSuffix,
                ScriptableRuntimeType,
                RuntimeAssetMenuRoot
            );
        }

        private static void CreateReactiveScript(string path)
        {
            string className = Path.GetFileNameWithoutExtension(path);

            string content = $@"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;

[CreateAssetMenu(fileName = nameof({className}), menuName = ""{ReactiveAssetMenuRoot}"" + nameof({className}), order = {AssetMenuRootOrder})]
public class {className} : ScriptableReactive<{className}>
{{
    // ⚠️ IMPORTANT: Replace <ScriptableReactive> with the actual class
    // 📦 Add methods, enums, or nested classes here if needed
}}
";

            WriteAndSelectFile(path, content);
        }

        private static string GetSelectedFolder()
        {
            string result = GetFolderFromSelection();

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            
            result = GetFolderFromProjectWindow();
            
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            
            return FolderCreationDefaultPath;
        }

        private static string GetFolderFromSelection()
        {
            foreach (UnityEngine.Object obj in Selection.objects)
            {
                if (obj == null)
                    continue;

                string path = AssetDatabase.GetAssetPath(obj);

                if (string.IsNullOrEmpty(path))
                    continue;

                return AssetDatabase.IsValidFolder(path) 
                    ? path 
                    : Path.GetDirectoryName(path);
            }
            return null;
        }

        private static string GetFolderFromProjectWindow()
        {
            EditorWindow projectWindow = EditorWindow.GetWindow<EditorWindow>();

            if (projectWindow == null)
                return null;
            
            if (Selection.activeObject != null)
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);

                if (!string.IsNullOrEmpty(path))
                    return AssetDatabase.IsValidFolder(path) 
                        ? path 
                        : Path.GetDirectoryName(path);
            }
            
            return null;
        }

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
            
            if (string.IsNullOrEmpty(rootName))
            {
                rootName = className;
            }

            dataName = rootName + DataSuffix;
        }

        private static void WriteAndSelectFile(string path, string content)
        {
            File.WriteAllText(path, content);
            AssetDatabase.Refresh();
            
            EditorApplication.delayCall += () =>
            {
                UnityEngine.Object createdAsset = AssetDatabase.LoadMainAssetAtPath(path);
                
                if (createdAsset != null)
                {
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = createdAsset;
                    EditorGUIUtility.PingObject(createdAsset);
                }
                else
                {
                    EditorUtility.FocusProjectWindow();
                }
            };
        }
    }
}