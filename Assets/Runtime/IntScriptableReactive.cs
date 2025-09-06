using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating IntScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(IntScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(IntScriptableReactive), order = 3)]
#endif
    /// <summary>
    /// Reactive scriptable object for integer values with change notifications
    /// </summary>
    /// <remarks>
    /// This class extends ScriptableReactive<int> to provide a specialized implementation
    /// for integer values with editor debugging support and change notifications.
    /// </remarks>
    public class IntScriptableReactive : ScriptableReactive<int>
    {
        // Inherits all functionality from ScriptableReactive<int>
    }
}