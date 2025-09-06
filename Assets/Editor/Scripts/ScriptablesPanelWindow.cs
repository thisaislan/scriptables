#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Thisaislan.Scriptables.Editor
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
        private const float MinWindowWidth = 600f;
        private const float MinWindowHeight = 400f;
        private const float AssetColumnWidth = 250f;
        private const float ActionsColumnWidth = 134f;
        private const float ButtonWidth = 55f;
        private const float TabHeight = 30f;
        private const float HeaderHeight = 20f;
        private const float IconSize = 20f;
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
            float availablePathWidth = position.width - 400; // 400px for other columns

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
                    RenderAssetRow(asset, availablePathWidth);
                }
            }

            EditorGUILayout.EndScrollView();
        }
        
        /// <summary>
        /// Renders a single asset row with icon, name, path, and action buttons
        /// </summary>
        /// <param name="asset">The asset to render</param>
        /// <param name="availablePathWidth">Available width for the path column</param>
        private void RenderAssetRow(Object asset, float availablePathWidth)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            Rect rowRect = EditorGUILayout.BeginHorizontal();

            // Highlight selected row
            if (Selection.activeObject == asset)
                EditorGUI.DrawRect(rowRect, new Color(0.24f, 0.48f, 0.90f, 0.3f));

            RenderAssetNameAndIcon(asset, path);
            RenderAssetPath(path, availablePathWidth);
            RenderActionButtons(asset, path);

            EditorGUILayout.EndHorizontal();

            HandleRowClick(asset, rowRect);
        }
        
        /// <summary>
        /// Renders the asset icon and name (with rename functionality)
        /// </summary>
        /// <param name="asset">The asset being rendered</param>
        /// <param name="path">Path to the asset</param>
        private void RenderAssetNameAndIcon(Object asset, string path)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(AssetColumnWidth));
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
                GUILayout.Label(asset.name, GUILayout.ExpandWidth(true));
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
            GUILayout.Label(new GUIContent(displayPath, path), GUILayout.ExpandWidth(true));
        }
        
        /// <summary>
        /// Renders the action buttons (Rename, Delete)
        /// </summary>
        /// <param name="asset">The asset the buttons will act upon</param>
        /// <param name="path">Path to the asset</param>
        private void RenderActionButtons(Object asset, string path)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
            bool clickedRename = GUILayout.Button(Consts.RenameButtonLabel, GUILayout.Width(ButtonWidth));
            bool clickedDelete = GUILayout.Button(Consts.DeleteButtonLabel, GUILayout.Width(ButtonWidth));
            
            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();

            if (clickedRename)
            {
                StartRenaming(asset);
            }
            else if (clickedDelete)
            {
                HandleDeleteAction(asset, path);
            }
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
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
                    if (asset != null && !list.Contains(asset))
                        list.Add(asset);
                }
            }
            return list;
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