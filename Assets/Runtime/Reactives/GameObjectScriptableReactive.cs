using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating GameObjectScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(GameObjectScriptableReactive), menuName = Meta.ScriptableReactiveMenuPath + nameof(GameObjectScriptableReactive), order = 7)]
    /// <summary>
    /// Reactive scriptable object for GameObject references with change notifications
    /// </summary>
#endif
    public class GameObjectScriptableReactive : ScriptableReactive<GameObject>
    {
        // Inherits all functionality from ScriptableReactive
    }
}