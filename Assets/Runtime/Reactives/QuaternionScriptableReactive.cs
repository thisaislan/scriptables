using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating QuaternionScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(QuaternionScriptableReactive), menuName = Meta.ScriptableReactiveMenuPath + nameof(QuaternionScriptableReactive), order = 9)]
    /// <summary>
    /// Reactive scriptable object for Quaternion values with change notifications
    /// </summary>
#endif
    public class QuaternionScriptableReactive : ScriptableReactive<Quaternion>
    {
        // Inherits all functionality from ScriptableReactive
    }
}