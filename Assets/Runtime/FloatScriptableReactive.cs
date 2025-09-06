using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating FloatScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(FloatScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(FloatScriptableReactive), order = 4)]
#endif
    /// <summary>
    /// Reactive scriptable object for float values with change notifications
    /// </summary>
    /// <remarks>
    /// This class extends ScriptableReactive<float> to provide a specialized implementation
    /// for float values with editor debugging support and change notifications.
    /// </remarks>
    public class FloatScriptableReactive : ScriptableReactive<float>
    {
        // Inherits all functionality from ScriptableReactive<float>
    }
}