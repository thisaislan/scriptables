using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating TransformScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(TransformScriptableReactive), menuName = Meta.ScriptableReactiveMenuPath + nameof(TransformScriptableReactive), order = 8)]
    /// <summary>
    /// Reactive scriptable object for Transform references with change notifications
    /// </summary>
#endif
    public class TransformScriptableReactive : ScriptableReactive<Transform>
    {
        // Inherits all functionality from ScriptableReactive
    }
}