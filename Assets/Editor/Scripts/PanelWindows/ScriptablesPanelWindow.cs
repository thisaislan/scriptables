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
        private const string AssetFileExtension = ".asset";
        private const string RefQuerySuffix = "ref:";
        private const string TildeSymbol = "...";

        private Tab currentTab = Tab.Debuggable;
        private Vector2 scrollPos;
        private GUIContent[] tabContents;
        private Object assetBeingRenamed = null;
        private string newName = string.Empty;
        private bool isRenaming = false;

        // Custom styles for tabs
        private GUIStyle selectedTabStyle;
        private GUIStyle normalTabStyle;

        // Selection tracking for keyboard navigation
        private List<Object> currentAssetsList = new List<Object>();
        private int selectedIndex = -1;
        private bool isKeyboardNavigation = false;
        
        /// <summary>
        /// Menu item to open the Scriptables Panel window
        /// </summary>
        [MenuItem(EditorConsts.MenuItemPath)]
        private static void ShowWindow()
        {
            var window = GetWindow<ScriptablesPanelWindow>(false, EditorConsts.WindowTitle, true);
            window.minSize = new Vector2(MinWindowWidth, MinWindowHeight);
            window.Show();
        }

        /// <summary>
        /// Initialize the window when enabled
        /// </summary>
        private void OnEnable()
        {
            tabContents = new GUIContent[] {
                new GUIContent(EditorConsts.TabScriptablesLabel, LoadTabIcon(EditorConsts.TabScriptablesIcon)),
                new GUIContent(EditorConsts.TabSettingsLabel, LoadTabIcon(EditorConsts.TabSettingsIcon)),
                new GUIContent(EditorConsts.TabRuntimeLabel, LoadTabIcon(EditorConsts.TabRuntimeIcon)),
                new GUIContent(EditorConsts.TabReactiveLabel, LoadTabIcon(EditorConsts.TabReactiveIcon)),
                new GUIContent(EditorConsts.TabAllLabel, LoadTabIcon(EditorConsts.TabAllIcon))
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
                        // Reset navigation state when switching tabs
                        selectedIndex = -1;
                        isKeyboardNavigation = false;
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
            GUILayout.Label(EditorConsts.AssetColumnHeader, EditorStyles.boldLabel, GUILayout.Width(AssetColumnWidth));
            GUILayout.Label(EditorConsts.PathColumnHeader, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            GUILayout.Label(EditorConsts.ActionsColumnHeader, EditorStyles.boldLabel, GUILayout.Width(ActionsColumnWidth));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Renders the scrollable list of assets
        /// </summary>
        private void RenderAssetList()
        {
            // Get and store the current asset list
            currentAssetsList = GetAssetsForCurrentTab()
                                .OrderBy(a => a.name)
                                .ToList();

            // Handle keyboard navigation
            if (Event.current.type == EventType.KeyDown && 
                (Event.current.keyCode == KeyCode.UpArrow || 
                Event.current.keyCode == KeyCode.DownArrow))
            {
                HandleKeyboardNavigation(Event.current.keyCode);
                Event.current.Use();
                isKeyboardNavigation = true;
                Repaint();
            }

            // If we have keyboard navigation active, ensure selection is in view
            if (isKeyboardNavigation && selectedIndex >= 0 && selectedIndex < currentAssetsList.Count)
            {
                // The actual scrolling happens in the scroll view automatically
                // but we need to ensure the selected item is visible
                Object selectedAsset = currentAssetsList[selectedIndex];
                Selection.activeObject = selectedAsset;
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true);

            if (currentAssetsList.Count == 0)
            {
                GUILayout.Space(20);
                GUILayout.Label(EditorConsts.NoItemsLabel, EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                for (int i = 0; i < currentAssetsList.Count; i++)
                {
                    var asset = currentAssetsList[i];
                    RenderAssetRow(asset, i);
                }
            }

            EditorGUILayout.EndScrollView();
        }
        
        /// <summary>
        /// Renders a single asset row with icon, name, path, and action buttons
        /// </summary>
        /// <param name="asset">The asset to render</param>
        /// <param name="index">The index of the asset in the list</param>
        private void RenderAssetRow(Object asset, int index)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            Rect rowRect = EditorGUILayout.BeginHorizontal();

            // Highlight selected row (either by mouse click or keyboard navigation)
            bool isSelected = (Selection.activeObject == asset) || (isKeyboardNavigation && selectedIndex == index);
            
            if (isSelected)
            {
                EditorGUI.DrawRect(rowRect, new Color(0.24f, 0.48f, 0.90f, 0.3f));
                // Sync the selected index when mouse selection changes
                if (Selection.activeObject == asset && (!isKeyboardNavigation || selectedIndex != index))
                {
                    selectedIndex = index;
                    isKeyboardNavigation = false;
                }
            }

            // Calculate available width for path
            float availableWidth = position.width - AssetColumnWidth - ActionsColumnWidth - 30;
            
            RenderAssetNameAndIcon(asset, path, Mathf.Min(AssetColumnWidth, position.width * 0.5f));
            RenderAssetPath(path, Mathf.Max(MinPathWidth, availableWidth));
            RenderActionButtons(asset, path);

            EditorGUILayout.EndHorizontal();

            HandleRowClick(asset, rowRect, index);
        }

        /// <summary>
        /// Handles keyboard navigation with arrow keys
        /// </summary>
        /// <param name="keyCode">The key that was pressed</param>
        private void HandleKeyboardNavigation(KeyCode keyCode)
        {
            if (currentAssetsList.Count == 0) return;

            if (selectedIndex == -1)
            {
                // Initialize selection to the first item if nothing is selected
                selectedIndex = 0;
            }
            else if (keyCode == KeyCode.UpArrow)
            {
                selectedIndex = Mathf.Max(0, selectedIndex - 1);
            }
            else if (keyCode == KeyCode.DownArrow)
            {
                selectedIndex = Mathf.Min(currentAssetsList.Count - 1, selectedIndex + 1);
            }

            // Ensure the selected item is visible in the scroll view
            if (selectedIndex >= 0 && selectedIndex < currentAssetsList.Count)
            {
                Object selectedAsset = currentAssetsList[selectedIndex];
                Selection.activeObject = selectedAsset;
                
                // Calculate the row height for scrolling (adjust this value based on your row height)
                float rowHeight = 20f; // Approximate height of each row
                float targetScrollY = selectedIndex * rowHeight;
                
                // Smoothly adjust scroll position (optional)
                // scrollPos.y = Mathf.Lerp(scrollPos.y, targetScrollY, 0.1f);
                // For immediate scrolling, uncomment the line below:
                // scrollPos.y = targetScrollY;
            }
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
                GUI.SetNextControlName(EditorConsts.RenameFieldName);
                newName = EditorGUILayout.TextField(newName);
                EditorGUI.FocusTextInControl(EditorConsts.RenameFieldName);

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
            bool clickedSearch = GUILayout.Button(EditorConsts.SearchButtonLabel, GUILayout.Width(ButtonWidth), GUILayout.ExpandWidth(false));
            bool clickedRename = GUILayout.Button(EditorConsts.RenameButtonLabel, GUILayout.Width(ButtonWidth), GUILayout.ExpandWidth(false));
            bool clickedDelete = GUILayout.Button(EditorConsts.DeleteButtonLabel, GUILayout.Width(ButtonWidth), GUILayout.ExpandWidth(false));
            
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
                string testString = shortened.Substring(0, shortened.Length - removeCount) + TildeSymbol;
                float testWidth = GUI.skin.label.CalcSize(new GUIContent(testString)).x;

                if (testWidth <= availableWidth)
                {
                    return testString;
                }

                removeCount++;

                // If we've removed too much, just return ellipsis
                if (removeCount > shortened.Length - 3)
                {
                    return TildeSymbol;
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
                string testString = shortened.Substring(0, shortened.Length - removeCount) + TildeSymbol;
                float testWidth = GUI.skin.label.CalcSize(new GUIContent(testString)).x;

                if (testWidth <= availableWidth)
                {
                    return testString;
                }

                removeCount++;

                // If we've removed too much, just return ellipsis
                if (removeCount > shortened.Length - 3)
                {
                    return TildeSymbol;
                }
            }

            return shortened;
        }
        
        /// <summary>
        /// Handles click events on the asset row
        /// </summary>
        /// <param name="asset">The clicked asset</param>
        /// <param name="rowRect">The rectangle defining the row area</param>
        /// <param name="index">The index of the clicked asset</param>
        private void HandleRowClick(Object asset, Rect rowRect, int index)
        {
            if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
            {
                if (isRenaming)
                {
                    // If clicking outside during rename, confirm it
                    ConfirmRename(assetBeingRenamed, AssetDatabase.GetAssetPath(assetBeingRenamed));
                }
                Selection.activeObject = asset;
                selectedIndex = index;
                isKeyboardNavigation = false; // Switch back to mouse mode
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
            string searchQuery = $"{RefQuerySuffix}\"{path}\"";

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
                    EditorConsts.ConfirmDeleteTitle, 
                    string.Format(EditorConsts.ConfirmDeleteMessage, asset.name), 
                    EditorConsts.DeleteButtonLabel, 
                    EditorConsts.CancelButtonLabel))
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
                    EditorConsts.FilterDebuggable1, 
                    EditorConsts.FilterDebuggable2, 
                    EditorConsts.FilterDebuggable3 
                },
                Tab.Settings => new[] { EditorConsts.FilterSettings },
                Tab.Runtime => new[] { EditorConsts.FilterRuntime },
                Tab.Reactive => new[] { EditorConsts.FilterReactive },
                Tab.All => new[] { EditorConsts.FilterAll },
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
            return extension.Equals(AssetFileExtension);
        }
        
        /// <summary>
        /// Safely loads a tab icon from the Resources folder
        /// </summary>
        /// <param name="resourceName">Name of the icon resource</param>
        /// <returns>The loaded texture or null if not found</returns>
        private Texture2D LoadTabIcon(string resourceName)
        {
            var tex = Resources.Load<Texture2D>($"{EditorConsts.EditorIconsPath}{resourceName}");
            return tex;
        }
    }
}
#endif