using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating Vector3ScriptableRuntime assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(Vector3ScriptableRuntime), menuName = RuntimeConsts.ScriptableRuntimeMenuPath + nameof(Vector3ScriptableRuntime), order = 11)]
    /// <summary>
    /// Runtime scriptable object for Vector3 values
    /// </summary>
#endif
    public class Vector3ScriptableRuntime : ScriptableRuntime<Vector3ScriptableRuntime.Vector3Data>
    {
        [Serializable]
        public class Vector3Data : Data
        {
            public Vector3 Vector3Value;
        }
    }
}