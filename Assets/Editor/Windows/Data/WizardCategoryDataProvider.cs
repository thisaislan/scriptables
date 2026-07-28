#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Thisaislan.Scriptables.Editor.Abstracts.Bases;
using Thisaislan.Scriptables.Editor.Utilities;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using Thisaislan.Scriptables.Editor.Windows.Data.Enums;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Windows.Data
{
    /// <summary>
    /// Provides category-specific metadata such as titles, descriptions, icons, item providers,
    /// and default values used by the Scriptables wizard.
    /// </summary>
    internal class WizardCategoryDataProvider
    {
        private const string ReactiveNewName = "NewReactive";
        private const string SettingsNewName = "NewSettings";
        private const string RuntimeNewName = "NewRuntime";
        private const string ScriptableObjectNewName = "NewScriptableObject";

        private const string ReactiveTitle = "Reactive Scriptable";
        private const string SettingsTitle = "Settings Scriptable";
        private const string RuntimeTitle = "Runtime Scriptable";
        private const string CustomTitle = "Project Scriptables";
        private const string ScriptableObjectTitle = "Scriptable Object";

        private const string ReactiveDescription = "Create a Reactive Scriptable that is able to notify a subscribers list.";
        private const string SettingsDescription = "Create a Settings Scriptable with editor/runtime data separation.";
        private const string RuntimeDescription = "Runtime Scriptable that resets on play mode exit.";
        private const string CustomDescription = "Creates a new Scriptable from existing project templates.";
        private const string ScriptableObjectDescription = "Creates a standard Scriptable Object.";

        private const string DefaultReactiveType = "Object";

        private const string ReactivesFolderPath = "/Assets/Runtime/Reactives";
        private const string SettingsFolderPath = "/Assets/Runtime/Settings";
        private const string RuntimesFolderPath = "/Assets/Runtime/Runtimes";

        private const string LibraryNamespacePrefix = "Thisaislan.Scriptables";

        private const string ProviderScriptName = "CategoryDataProvider";

        private readonly List<Type> customScriptTypes = new List<Type>();
        private bool customScriptsLoaded;
        private string packageRoot;

        private static readonly string[] LibraryNamespacePrefixes = new[]
        {
            LibraryNamespacePrefix
        };

        /// <summary>
        /// Clears the cached list of custom script types so they will be rescanned on next access.
        /// </summary>
        internal void ClearCustomScriptCache()
        {
            customScriptsLoaded = false;
            customScriptTypes.Clear();
        }

        /// <summary>
        /// Returns all available wizard categories in display order.
        /// </summary>
        internal List<WizardScriptablesCategory> GetCategories()
        {
            return new List<WizardScriptablesCategory>
            {
                WizardScriptablesCategory.Reactive,
                WizardScriptablesCategory.Runtime,
                WizardScriptablesCategory.Settings,
                WizardScriptablesCategory.Custom,
                WizardScriptablesCategory.ScriptableObject
            };
        }

        /// <summary>
        /// Gets the display title for the given category.
        /// </summary>
        internal string GetCategoryTitle(WizardScriptablesCategory category)
        {
            switch (category)
            {
                case WizardScriptablesCategory.Reactive:
                    return ReactiveTitle;
                case WizardScriptablesCategory.Settings:
                    return SettingsTitle;
                case WizardScriptablesCategory.Runtime:
                    return RuntimeTitle;
                case WizardScriptablesCategory.Custom:
                    return CustomTitle;
                case WizardScriptablesCategory.ScriptableObject:
                    return ScriptableObjectTitle;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the description text for the given category.
        /// </summary>
        internal string GetCategoryDescription(WizardScriptablesCategory category)
        {
            switch (category)
            {
                case WizardScriptablesCategory.Reactive:
                    return ReactiveDescription;
                case WizardScriptablesCategory.Settings:
                    return SettingsDescription;
                case WizardScriptablesCategory.Runtime:
                    return RuntimeDescription;
                case WizardScriptablesCategory.Custom:
                    return CustomDescription;
                case WizardScriptablesCategory.ScriptableObject:
                    return ScriptableObjectDescription;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the icon texture associated with the given category.
        /// </summary>
        internal Texture GetCategoryIcon(WizardScriptablesCategory category)
        {
            switch (category)
            {
                case WizardScriptablesCategory.Reactive:
                    return ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.SmallReactive);
                case WizardScriptablesCategory.Settings:
                    return ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.SmallSettings);
                case WizardScriptablesCategory.Runtime:
                    return ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.SmallRuntime);
                case WizardScriptablesCategory.Custom:
                    return ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.SmallMainScriptables);
                case WizardScriptablesCategory.ScriptableObject:
                    return ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.SmallScriptableObject);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns whether the given category supports a "New" item creation entry.
        /// </summary>
        internal bool HasNewItem(WizardScriptablesCategory category)
        {
            return category != WizardScriptablesCategory.Custom;
        }

        /// <summary>
        /// Gets the default asset name for a new item in the given category.
        /// Returns an empty string for categories that do not support a new item.
        /// </summary>
        internal string GetCategoryNewItemName(WizardScriptablesCategory category)
        {
            switch (category)
            {
                case WizardScriptablesCategory.Reactive:
                    return ReactiveNewName;
                case WizardScriptablesCategory.Settings:
                    return SettingsNewName;
                case WizardScriptablesCategory.Runtime:
                    return RuntimeNewName;
                case WizardScriptablesCategory.ScriptableObject:
                    return ScriptableObjectNewName;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the default reactive generic type for the given category.
        /// Only the Reactive category returns a non-empty type name; all others return an empty string.
        /// </summary>
        internal string GetCategoryDefaultReactiveType(WizardScriptablesCategory category)
        {
            return category == WizardScriptablesCategory.Reactive ? DefaultReactiveType : string.Empty;
        }

        /// <summary>
        /// Gets a factory function that returns the list of items for the given category.
        /// Each call to the returned delegate triggers a fresh scan.
        /// </summary>
        internal Func<List<WizardItemData>> GetItemProvider(WizardScriptablesCategory category)
        {
            switch (category)
            {
                case WizardScriptablesCategory.Reactive:
                    return () => ScanFolderToItems(GetPackageRoot() + ReactivesFolderPath);
                case WizardScriptablesCategory.Settings:
                    return () => ScanFolderToItems(GetPackageRoot() + SettingsFolderPath);
                case WizardScriptablesCategory.Runtime:
                    return () => ScanFolderToItems(GetPackageRoot() + RuntimesFolderPath);
                case WizardScriptablesCategory.Custom:
                    return ScanCustomScripts;
                case WizardScriptablesCategory.ScriptableObject:
                    return ScanNonDebuggableScriptableObjects;
                default:
                    return null;
            }
        }

        private List<WizardItemData> ScanFolderToItems(string folderPath)
        {
            List<ScriptScanInfo> scanned = AssetScanner.ScanScriptsInFolder(folderPath);
            return ToItemData(scanned);
        }

        private List<WizardItemData> ScanCustomScripts()
        {
            if (customScriptsLoaded && customScriptTypes.Count > 0)
            {
                return BuildCustomItemsFromCache();
            }

            List<ScriptScanInfo> scanned = AssetScanner.ScanDerivedTypes(
                typeof(BaseEditorDebuggableScriptable), LibraryNamespacePrefixes);

            customScriptTypes.Clear();
            foreach (ScriptScanInfo info in scanned)
            {
                customScriptTypes.Add(info.ScriptType);
            }

            customScriptsLoaded = true;
            return BuildCustomItemsFromCache();
        }

        private List<WizardItemData> BuildCustomItemsFromCache()
        {
            List<WizardItemData> results = new List<WizardItemData>();

            foreach (Type type in customScriptTypes)
            {
                string path = AssetScanner.FindScriptPath(type);

                results.Add(new WizardItemData
                {
                    Name = type.Name,
                    Icon = ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.Script),
                    IsPinned = false,
                    ScriptType = type,
                    AssetPath = path
                });
            }

            results.Sort(CompareByName);
            return results;
        }

        private List<WizardItemData> ScanNonDebuggableScriptableObjects()
        {
            List<ScriptScanInfo> scanned = AssetScanner.ScanNonDebuggableScriptableObjects(
                typeof(BaseEditorDebuggableScriptable),
                LibraryNamespacePrefixes,
                GetPackageRoot());

            return ToItemData(scanned);
        }

        private static List<WizardItemData> ToItemData(List<ScriptScanInfo> scanned)
        {
            List<WizardItemData> results = new List<WizardItemData>();

            foreach (ScriptScanInfo info in scanned)
            {
                results.Add(new WizardItemData
                {
                    Name = info.Name,
                    Icon = ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.Script),
                    IsPinned = false,
                    ScriptType = info.ScriptType,
                    AssetPath = info.AssetPath
                });
            }

            return results;
        }

        private static int CompareByName(WizardItemData a, WizardItemData b)
        {
            return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
        }

        private string GetPackageRoot()
        {
            if (string.IsNullOrEmpty(packageRoot))
            {
                packageRoot = AssetScanner.ResolveScriptRoot(ProviderScriptName);
            }
            return packageRoot;
        }
    }
}
#endif
