using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating ColorScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(ColorScriptableReactive), menuName = RuntimeConsts.ScriptableReactiveMenuPath + nameof(ColorScriptableReactive), order = 12)]
    /// <summary>
    /// Reactive scriptable object for Color values with change notifications
    /// </summary>
#endif
    public class ColorScriptableReactive : ScriptableReactive<Color>
    {
        // Inherits all functionality from ScriptableReactive
    }
}