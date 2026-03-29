using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating Vector2ScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(Vector2ScriptableReactive), menuName = Meta.ScriptableReactiveMenuPath + nameof(Vector2ScriptableReactive), order = 10)]
    /// <summary>
    /// Reactive scriptable object for Vector2 values with change notifications
    /// </summary>
#endif
    public class Vector2ScriptableReactive : ScriptableReactive<Vector2>
    {
        // Inherits all functionality from ScriptableReactive
    }
}