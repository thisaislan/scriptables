#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Windows.Data
{
    /// <summary>
    /// Represents an item displayed in the Scriptables wizard, containing display info
    /// and a reference to the underlying script type and asset path.
    /// </summary>
    internal class WizardItemData
    {
        /// <summary>
        /// The display name of the item (matches the script class name).
        /// </summary>
        internal string Name;

        /// <summary>
        /// The icon texture shown alongside the item name in the wizard list.
        /// </summary>
        internal Texture Icon;

        /// <summary>
        /// Whether this item is pinned (the always-visible "Create New" entry).
        /// </summary>
        internal bool IsPinned;

        /// <summary>
        /// The <see cref="Type"/> of the script class this item represents.
        /// </summary>
        internal Type ScriptType;

        /// <summary>
        /// The project-relative asset path of the script file.
        /// </summary>
        internal string AssetPath;
    }
}
#endif
