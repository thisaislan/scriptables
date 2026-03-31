using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating IntScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(IntScriptableReactive), menuName = RuntimeConsts.ScriptableReactiveMenuPath + nameof(IntScriptableReactive), order = 3)]
    /// <summary>
    /// Reactive scriptable object for integer values with change notifications
    /// </summary>
#endif
    public class IntScriptableReactive : ScriptableReactive<int>
    {
        // Inherits all functionality from ScriptableReactive
    }
}