#if UNITY_EDITOR
namespace Thisaislan.Scriptables.Editor.Windows.Data.Enums
{
    /// <summary>
    /// Specifies the type of Scriptables Wizard category.
    /// </summary>
    internal enum WizardScriptablesCategory
    {
        /// <summary>
        /// Represents a reactive scriptable category.
        /// </summary>
        Reactive,

        /// <summary>
        /// Represents a settings scriptable category.
        /// </summary>
        Settings,

        /// <summary>
        /// Represents a runtime scriptable category.
        /// </summary>
        Runtime,

        /// <summary>
        /// Represents a standard scriptable object category.
        /// </summary>
        ScriptableObject,

        /// <summary>
        /// Represents a custom project scriptable category.
        /// </summary>
        Custom
    }
}
#endif