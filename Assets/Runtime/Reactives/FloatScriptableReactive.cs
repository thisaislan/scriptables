using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating FloatScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(FloatScriptableReactive), menuName = RuntimeConsts.ScriptableReactiveMenuPath + nameof(FloatScriptableReactive), order = 4)]
    /// <summary>
    /// Reactive scriptable object for float values with change notifications
    /// </summary>
#endif
    public class FloatScriptableReactive : ScriptableReactive<float>
    {
        // Inherits all functionality from ScriptableReactive
    }
}