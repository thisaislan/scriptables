using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating QuaternionScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(QuaternionScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(QuaternionScriptableReactive), order = 9)]
#endif
    /// <summary>
    /// Reactive scriptable object for Quaternion values with change notifications
    /// </summary>
    /// <remarks>
    /// This class extends ScriptableReactive<Quaternion> to provide a specialized implementation
    /// for Quaternion values with editor debugging support and change notifications.
    /// </remarks>
    public class QuaternionScriptableReactive : ScriptableReactive<Quaternion>
    {
        // Inherits all functionality from ScriptableReactive<Quaternion>
    }
}