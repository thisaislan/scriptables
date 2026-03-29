using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating QuaternionScriptableSettings assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(QuaternionScriptableSettings), menuName = Meta.ScriptableSettingsMenuPath + nameof(QuaternionScriptableSettings), order = 9)]
    /// <summary>
    /// Settings scriptable object for Quaternion values
    /// </summary>
#endif
    public class QuaternionScriptableSettings : ScriptableSettings<QuaternionScriptableSettings.QuaternionData>
    {
        [Serializable]
        public class QuaternionData : Data
        {
            public Quaternion QuaternionValue;
        }
    }
}