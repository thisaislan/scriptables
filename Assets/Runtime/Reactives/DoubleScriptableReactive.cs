using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating DoubleScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(DoubleScriptableReactive), menuName = Meta.ScriptableReactiveMenuPath + nameof(DoubleScriptableReactive), order = 5)]
    /// <summary>
    /// Reactive scriptable object for double values with change notifications
    /// </summary>
#endif
    public class DoubleScriptableReactive : ScriptableReactive<double>
    {
        // Inherits all functionality from ScriptableReactive
    }
}