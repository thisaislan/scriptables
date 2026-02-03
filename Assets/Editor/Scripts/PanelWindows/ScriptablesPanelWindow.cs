#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Search;

namespace Thisaislan.Scriptables.Editor.PanelWidows
{
    /// <summary>
    /// Editor window for managing and organizing ScriptableObjects in the project
    /// Provides functionality to view, rename, and delete assets by category
    /// </summary>
    public class ScriptablesPanelWindow : EditorWindow
    {
        /// <summary>
        /// Available tabs for filtering ScriptableObjects
        /// </summary>
        private enum Tab { Debuggable, Settings, Runtime, Reactive, All }
        
        // Window configuration
        private const float MinWindowWidth = 900f;
        private const float MinWindowHeight = 500f;
        private const float AssetColumnWidth = 300f;
        private const float ActionsColumnWidth = 186f;
        private const float ButtonWidth = 55f;
        private const float TabHeight = 30f;
        private const float HeaderHeight = 20f;
        private const float IconSize = 20f;
        private const float MinPathWidth = 100f;
        private Tab currentTab = Tab.Debuggable;
        private Vector2 scrollPos;
        private GUIContent[] tabContents;
        private Object assetBeingRenamed = null;
        private string newName = string.Empty;
        private bool isRenaming = false;

        // Custom styles for tabs
        private GUIStyle selectedTabStyle;
        private GUIStyle normalTabStyle;
        
        /// <summary>
        /// Menu item to open the Scriptables Panel window
        /// </summary>
        [MenuItem(Consts.MenuItemPath)]
        private static void ShowWindow()
        {
            var window = GetWindow<ScriptablesPanelWindow>(false, Consts.WindowTitle, false);
            window.minSize = new Vector2(MinWindowWidth, MinWindowHeight);
            window.Show();
        }

        /// <summary>
        /// Initialize the window when enabled
        /// </summary>
        private void OnEnable()
        {
            tabContents = new GUIContent[] {
                new GUIContent(Consts.TabScriptablesLabel, LoadTabIcon(Consts.TabScriptablesIcon)),
                new GUIContent(Consts.TabSettingsLabel, LoadTabIcon(Consts.TabSettingsIcon)),
                new GUIContent(Consts.TabRuntimeLabel, LoadTabIcon(Consts.TabRuntimeIcon)),
                new GUIContent(Consts.TabReactiveLabel, LoadTabIcon(Consts.TabReactiveIcon)),
                new GUIContent(Consts.TabAllLabel, LoadTabIcon(Consts.TabAllIcon))
            };
             // Create custom tab styles
            CreateTabStyles();
        }
        
        /// <summary>
        /// Main GUI rendering method
        /// </summary>
        private void OnGUI()
        {
            RenderTabs();
            RenderHeader();
            RenderAssetList();
        }
        
        /// <summary>
        /// Renders the tab navigation bar
        /// </summary>
        private void RenderTabs()
        {
            EditorGUILayout.BeginHorizontal();
            
            float tabWidth = position.width / tabContents.Length;
            
            for (int i = 0; i < tabContents.Length; i++)
            {
                bool isSelected = (int)currentTab == i;
                GUIStyle tabStyle = isSelected ? selectedTabStyle : normalTabStyle;
                tabStyle.fixedWidth = tabWidth;
                
                if (GUILayout.Toggle(isSelected, tabContents[i], tabStyle, GUILayout.Width(tabWidth)))
                {
                    if (!isSelected)
                    {
                        currentTab = (Tab)i;
                        scrollPos = Vector2.zero;
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates custom styles for the tabs to avoid modifying global styles
        /// </summary>
        private void CreateTabStyles()
        {
            try
            {
                // Selected tab style
                selectedTabStyle = new GUIStyle(EditorStyles.toolbarButton);
                selectedTabStyle.normal.background = selectedTabStyle.active.background;
                selectedTabStyle.normal.textColor = selectedTabStyle.active.textColor;
                selectedTabStyle.fixedHeight = TabHeight;

                // Normal tab style
                normalTabStyle = new GUIStyle(EditorStyles.toolbarButton);
                normalTabStyle.fixedHeight = TabHeight;
            }
            catch
            { 
                
            }
        }

        /// <summary>
        /// Renders the column headers (fixed position, not scrollable)
        /// </summary>
        private void RenderHeader()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.textArea, GUILayout.Height(HeaderHeight));
            GUILayout.Label(Consts.AssetColumnHeader, EditorStyles.boldLabel, GUILayout.Width(AssetColumnWidth));
            GUILayout.Label(Consts.PathColumnHeader, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            GUILayout.Label(Consts.ActionsColumnHeader, EditorStyles.boldLabel, GUILayout.Width(ActionsColumnWidth));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Renders the scrollable list of assets
        /// </summary>
        private void RenderAssetList()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true);

            List<Object> assets = GetAssetsForCurrentTab()
                                .OrderBy(a => a.name)
                                .ToList();

            if (assets.Count == 0)
            {
                GUILayout.Space(20);
                GUILayout.Label(Consts.NoItemsLabel, EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                foreach (var asset in assets)
                {
                    RenderAssetRow(asset);
                }
            }

            EditorGUILayout.EndScrollView();
        }
        
        /// <summary>
        /// Renders a single asset row with icon, name, path, and action buttons
        /// </summary>
        /// <param name="asset">The asset to render</param>
        private void RenderAssetRow(Object asset)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            Rect rowRect = EditorGUILayout.BeginHorizontal();

            // Highlight selected row
            if (Selection.activeObject == asset)
                EditorGUI.DrawRect(rowRect, new Color(0.24f, 0.48f, 0.90f, 0.3f));

            // Calculate available width for path
            float availableWidth = position.width - AssetColumnWidth - ActionsColumnWidth - 30;
            
            RenderAssetNameAndIcon(asset, path, Mathf.Min(AssetColumnWidth, position.width * 0.5f));
            RenderAssetPath(path, Mathf.Max(MinPathWidth, availableWidth));
            RenderActionButtons(asset, path);

            EditorGUILayout.EndHorizontal();

            HandleRowClick(asset, rowRect);
        }
        
        /// <summary>
        /// Renders the asset icon and name (with rename functionality)
        /// </summary>
        /// <param name="asset">The asset being rendered</param>
        /// <param name="path">Path to the asset</param>
        /// <param name="maxWidth">Maximum width for the asset name</param>
        private void RenderAssetNameAndIcon(Object asset, string path, float maxWidth)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(maxWidth));
            GUILayout.Space(10);
            GUILayout.Label(AssetPreview.GetMiniThumbnail(asset), GUILayout.Width(IconSize), GUILayout.Height(IconSize));

            if (isRenaming && assetBeingRenamed == asset)
            {
                GUI.SetNextControlName(Consts.RenameFieldName);
                newName = EditorGUILayout.TextField(newName);
                EditorGUI.FocusTextInControl(Consts.RenameFieldName);

                // Handle Enter key to confirm, Escape to cancel
                if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                {
                    ConfirmRename(asset, path);
                }
                else if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
                {
                    CancelRename();
                }
            }
            else
            {
                // Shorten the asset name if it's too long
                string displayName = ShortenName(asset.name, maxWidth - IconSize - 20);
                GUILayout.Label(displayName, GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Renders the asset path with ellipsis truncation
        /// </summary>
        /// <param name="path">The full asset path</param>
        /// <param name="availableWidth">Available width for the path</param>
        private void RenderAssetPath(string path, float availableWidth)
        {
            string displayPath = ShortenPath(path, availableWidth);
            GUILayout.Label(new GUIContent(displayPath, path), 
                           GUILayout.ExpandWidth(true), 
                           GUILayout.MinWidth(MinPathWidth));
        }
        
        /// <summary>
        /// Renders the action buttons (Search, Rename, Delete)
        /// </summary>
        /// <param name="asset">The asset the buttons will act upon</param>
        /// <param name="path">Path to the asset</param>
        private void RenderActionButtons(Object asset, string path)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(ActionsColumnWidth));
            
            // Ensure buttons have a fixed width and don't get squeezed
            bool clickedSearch = GUILayout.Button(Consts.SearchButtonLabel, GUILayout.Width(ButtonWidth), GUILayout.ExpandWidth(false));
            bool clickedRename = GUILayout.Button(Consts.RenameButtonLabel, GUILayout.Width(ButtonWidth), GUILayout.ExpandWidth(false));
            bool clickedDelete = GUILayout.Button(Consts.DeleteButtonLabel, GUILayout.Width(ButtonWidth), GUILayout.ExpandWidth(false));
            
            EditorGUILayout.EndHorizontal();

            if (clickedSearch)
            {
                FindReferencesInScene(asset, path);
            }
            else if (clickedRename)
            {
                StartRenaming(asset);
            }
            else if (clickedDelete)
            {
                HandleDeleteAction(asset, path);
            }
        }
        
        /// <summary>
        /// Shortens a name with ellipsis at the end to fit available width
        /// </summary>
        /// <param name="name">The full name to shorten</param>
        /// <param name="availableWidth">Available width for display</param>
        /// <returns>The shortened name with ellipsis</returns>
        private string ShortenName(string name, float availableWidth)
        {
            if (string.IsNullOrEmpty(name)) return name;

            // Calculate if the full name fits
            float fullWidth = GUI.skin.label.CalcSize(new GUIContent(name)).x;
            if (fullWidth <= availableWidth) return name;

            // Start with the full name and gradually shorten from the end
            string shortened = name;
            int removeCount = 1;

            while (removeCount < shortened.Length)
            {
                // Remove characters from the end and add ellipsis
                string testString = shortened.Substring(0, shortened.Length - removeCount) + "...";
                float testWidth = GUI.skin.label.CalcSize(new GUIContent(testString)).x;

                if (testWidth <= availableWidth)
                {
                    return testString;
                }

                removeCount++;

                // If we've removed too much, just return ellipsis
                if (removeCount > shortened.Length - 3)
                {
                    return "...";
                }
            }

            return shortened;
        }
        
        /// <summary>
        /// Shortens a path with ellipsis at the end to fit available width
        /// </summary>
        /// <param name="path">The full path to shorten</param>
        /// <param name="availableWidth">Available width for display</param>
        /// <returns>The shortened path with ellipsis</returns>
        private string ShortenPath(string path, float availableWidth)
        {
            if (string.IsNullOrEmpty(path)) return path;

            // Calculate if the full path fits
            float fullWidth = GUI.skin.label.CalcSize(new GUIContent(path)).x;
            if (fullWidth <= availableWidth) return path;

            // Start with the full path and gradually shorten from the end
            string shortened = path;
            int removeCount = 1;

            while (removeCount < shortened.Length)
            {
                // Remove characters from the end and add ellipsis
                string testString = shortened.Substring(0, shortened.Length - removeCount) + "...";
                float testWidth = GUI.skin.label.CalcSize(new GUIContent(testString)).x;

                if (testWidth <= availableWidth)
                {
                    return testString;
                }

                removeCount++;

                // If we've removed too much, just return ellipsis
                if (removeCount > shortened.Length - 3)
                {
                    return "...";
                }
            }

            return shortened;
        }
        
        /// <summary>
        /// Handles click events on the asset row
        /// </summary>
        /// <param name="asset">The clicked asset</param>
        /// <param name="rowRect">The rectangle defining the row area</param>
        private void HandleRowClick(Object asset, Rect rowRect)
        {
            if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
            {
                if (isRenaming)
                {
                    // If clicking outside during rename, confirm it
                    ConfirmRename(assetBeingRenamed, AssetDatabase.GetAssetPath(assetBeingRenamed));
                }
                Selection.activeObject = asset;
                Event.current.Use();
            }
        }
        
        /// <summary>
        /// Initiates the renaming process for an asset
        /// </summary>
        /// <param name="asset">The asset to rename</param>
        private void StartRenaming(Object asset)
        {
            Selection.activeObject = asset;
            assetBeingRenamed = asset;
            newName = asset.name;
            isRenaming = true;
        }

        /// <summary>
        /// Finds all references to an asset in the current scene using Unity's Search window
        /// </summary>
        /// <param name="asset">The asset to find references for</param>
        private void FindReferencesInScene(Object asset, string path)
        {
            string searchQuery = $"ref:\"{path}\"";

            // Create search context and open window for scene references
            var context = SearchService.CreateContext(searchQuery);
            SearchService.ShowWindow(context);
        }
        
        /// <summary>
        /// Confirms and executes the rename operation
        /// </summary>
        /// <param name="asset">The asset being renamed</param>
        /// <param name="path">Path to the asset</param>
        private void ConfirmRename(Object asset, string path)
        {
            if (newName != asset.name && !string.IsNullOrEmpty(newName))
            {
                AssetDatabase.RenameAsset(path, newName);
                AssetDatabase.Refresh();
            }
            CancelRename();
        }

        /// <summary>
        /// Cancels the ongoing rename operation
        /// </summary>
        private void CancelRename()
        {
            isRenaming = false;
            assetBeingRenamed = null;
            newName = string.Empty;
            Repaint();
        }
        
        /// <summary>
        /// Handles the delete action with confirmation dialog
        /// </summary>
        /// <param name="asset">The asset to delete</param>
        /// <param name="path">Path to the asset</param>
        private void HandleDeleteAction(Object asset, string path)
        {
            Selection.activeObject = asset;
            
            // Force a repaint to show the selection highlight immediately
            Repaint();

            EditorApplication.delayCall += () =>
            {
                if (EditorUtility.DisplayDialog(
                    Consts.ConfirmDeleteTitle, 
                    string.Format(Consts.ConfirmDeleteMessage, asset.name), 
                    Consts.DeleteButtonLabel, 
                    Consts.CancelButtonLabel))
                {
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.Refresh();
                }
            };
        }
        
        /// <summary>
        /// Retrieves assets based on the currently selected tab
        /// </summary>
        /// <returns>List of assets filtered by the current tab</returns>
        private List<Object> GetAssetsForCurrentTab()
        {
            string[] filters = currentTab switch
            {
                Tab.Debuggable => new[] { 
                    Consts.FilterDebuggable1, 
                    Consts.FilterDebuggable2, 
                    Consts.FilterDebuggable3 
                },
                Tab.Settings => new[] { Consts.FilterSettings },
                Tab.Runtime => new[] { Consts.FilterRuntime },
                Tab.Reactive => new[] { Consts.FilterReactive },
                Tab.All => new[] { Consts.FilterAll },
                _ => new string[0]
            };

            var list = new List<Object>();
            
            foreach (string filter in filters)
            {
                foreach (string guid in AssetDatabase.FindAssets(filter))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                    // Filter out non-ScriptableObject assets
                    if (asset != null && asset is ScriptableObject && !list.Contains(asset))
                    {
                        // Additional filtering to exclude specific file types
                        if (ShouldIncludeAsset(assetPath))
                        {
                            list.Add(asset);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Determines if an asset should be included based on its file extension
        /// </summary>
        /// <param name="assetPath">Path to the asset</param>
        /// <returns>True if the asset should be included, false otherwise</returns>
        private bool ShouldIncludeAsset(string assetPath)
        {       
            // Get the file extension
            string extension = System.IO.Path.GetExtension(assetPath).ToLower();
            
            // Exclude if it's in our exclusion list
            return extension.Equals(".asset");
        }
        
        /// <summary>
        /// Safely loads a tab icon from the Resources folder
        /// </summary>
        /// <param name="resourceName">Name of the icon resource</param>
        /// <returns>The loaded texture or null if not found</returns>
        private Texture2D LoadTabIcon(string resourceName)
        {
            var tex = Resources.Load<Texture2D>($"{Consts.EditorIconsPath}{resourceName}");
            return tex;
        }
    }
}
#endif