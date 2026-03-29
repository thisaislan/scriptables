using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating Vector3ScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(Vector3ScriptableReactive), menuName = Meta.ScriptableReactiveMenuPath + nameof(Vector3ScriptableReactive), order = 11)]
    /// <summary>
    /// Reactive scriptable object for Vector3 values with change notifications
    /// </summary>
#endif
    public class Vector3ScriptableReactive : ScriptableReactive<Vector3>
    {
        // Inherits all functionality from ScriptableReactive
    }
}