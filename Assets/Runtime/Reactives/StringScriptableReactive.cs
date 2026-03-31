using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating StringScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(StringScriptableReactive), menuName = RuntimeConsts.ScriptableReactiveMenuPath + nameof(StringScriptableReactive), order = 6)]
    /// <summary>
    /// Reactive scriptable object for string values with change notifications
    /// </summary>
#endif
    public class StringScriptableReactive : ScriptableReactive<string>
    {
        // Inherits all functionality from ScriptableReactive
    }
}