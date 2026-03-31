using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating Vector3ScriptableSettings assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(Vector3ScriptableSettings), menuName = RuntimeConsts.ScriptableSettingsMenuPath + nameof(Vector3ScriptableSettings), order = 11)]
    /// <summary>
    /// Settings scriptable object for Vector3 values
    /// </summary>
#endif
    public class Vector3ScriptableSettings : ScriptableSettings<Vector3ScriptableSettings.Vector3Data>
    {
        [Serializable]
        public class Vector3Data : Data
        {
            public Vector3 Vector3Value;
        }
    }
}