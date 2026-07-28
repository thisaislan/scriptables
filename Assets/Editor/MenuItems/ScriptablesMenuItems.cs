#if UNITY_EDITOR
using UnityEditor;
using Thisaislan.Scriptables.Editor.Windows;

namespace Thisaislan.Scriptables.Editor.MenuItems
{
    /// <summary>
    /// Provides menu items for accessing Scriptables tools from the Unity Editor top menu bar.
    /// </summary>
    internal static class ScriptablesMenuItems
    {
        private const int MenuPanelToolsItemPathsPriority = 11;
        private const int MenuPanelToolsItemWizardPriority = 0;
        private const int MenuPanelCreateItemWizardPriority = 0;

        private const string MenuToolsPath = "Tools/Scriptables/";
        private const string MenuCreatePath = "Assets/Create/Scriptables/";
        private const string MenuPanelItemPath = "Panel";
        private const string MenuPanelItemWizard = "Wizard";


        [MenuItem(MenuToolsPath + MenuPanelItemPath, priority = MenuPanelToolsItemPathsPriority)]
        private static void ShowScriptablePanelWindowTools()
        {
            ScriptablesPanelWindow.ShowWindow();
        }

        [MenuItem(MenuToolsPath + MenuPanelItemWizard, priority = MenuPanelToolsItemWizardPriority)]
        private static void ShowScriptableWizardWindowTools()
        {
            ScriptablesWizardWindow.ShowWizard();
        }

        [MenuItem(MenuCreatePath + MenuPanelItemWizard, priority = MenuPanelCreateItemWizardPriority)]
        private static void ShowScriptableWizardWindowCreate()
        {
            ScriptablesWizardWindow.ShowWizard();
        }

    }
}
#endif