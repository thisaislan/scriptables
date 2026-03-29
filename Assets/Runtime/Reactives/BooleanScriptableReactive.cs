using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating BooleanScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(BooleanScriptableReactive), menuName = Meta.ScriptableReactiveMenuPath + nameof(BooleanScriptableReactive), order = 2)]
    /// <summary>
    /// Reactive scriptable object for boolean values with change notifications
    /// </summary>
#endif
    public class BooleanScriptableReactive : ScriptableReactive<bool>
    {
        // Inherits all functionality from ScriptableReactive
    }
}