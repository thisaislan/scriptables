using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating ColorScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(ColorScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(ColorScriptableReactive), order = 12)]
#endif
    /// <summary>
    /// Reactive scriptable object for Color values with change notifications
    /// </summary>
    /// <remarks>
    /// This class extends ScriptableReactive<Color> to provide a specialized implementation
    /// for Color values with editor debugging support and change notifications.
    /// </remarks>
    public class ColorScriptableReactive : ScriptableReactive<Color>
    {
        // Inherits all functionality from ScriptableReactive<Color>
    }
}