using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating Vector3ScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(Vector3ScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(Vector3ScriptableReactive), order = 11)]
#endif
    /// <summary>
    /// Reactive scriptable object for Vector3 values with change notifications
    /// </summary>
    /// <remarks>
    /// This class extends ScriptableReactive<Vector3> to provide a specialized implementation
    /// for Vector3 values with editor debugging support and change notifications.
    /// </remarks>
    public class Vector3ScriptableReactive : ScriptableReactive<Vector3>
    {
        // Inherits all functionality from ScriptableReactive<Vector3>
    }
}