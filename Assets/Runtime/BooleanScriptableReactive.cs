using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating BooleanScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(BooleanScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(BooleanScriptableReactive), order = 2)]
#endif
    /// <summary>
    /// Reactive scriptable object for boolean values with change notifications
    /// </summary>
    /// <remarks>
    /// This class extends ScriptableReactive<bool> to provide a specialized implementation
    /// for boolean values with editor debugging support and change notifications.
    /// </remarks>
    public class BooleanScriptableReactive : ScriptableReactive<bool>
    {
        // Inherits all functionality from ScriptableReactive<bool>
    }
}