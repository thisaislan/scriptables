using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating TransformScriptableSettings assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(TransformScriptableSettings), menuName = Meta.ScriptableSettingsMenuPath + nameof(TransformScriptableSettings), order = 8)]
    /// <summary>
    /// Settings scriptable object for Transform references
    /// </summary>
#endif
    public class TransformScriptableSettings : ScriptableSettings<TransformScriptableSettings.TransformData>
    {
        [Serializable]
        public class TransformData : Data
        {
            public Transform TransformValue;
        }
    }
}