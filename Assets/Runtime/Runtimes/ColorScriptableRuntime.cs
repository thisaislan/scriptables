using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating ColorScriptableRuntime assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(ColorScriptableRuntime), menuName = RuntimeConsts.ScriptableRuntimeMenuPath + nameof(ColorScriptableRuntime), order = 12)]
    /// <summary>
    /// Runtime scriptable object for Color values
    /// </summary>
#endif
    public class ColorScriptableRuntime : ScriptableRuntime<ColorScriptableRuntime.ColorData>
    {
        [Serializable]
        public class ColorData : Data
        {
            public Color ColorValue;
        }
    }
}