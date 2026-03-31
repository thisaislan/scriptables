using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating Vector2ScriptableSettings assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(Vector2ScriptableSettings), menuName = RuntimeConsts.ScriptableSettingsMenuPath + nameof(Vector2ScriptableSettings), order = 10)]
    /// <summary>
    /// Settings scriptable object for Vector2 values
    /// </summary>
#endif
    public class Vector2ScriptableSettings : ScriptableSettings<Vector2ScriptableSettings.Vector2Data>
    {
        [Serializable]
        public class Vector2Data : Data
        {
            public Vector2 Vector2Value;
        }
    }
}