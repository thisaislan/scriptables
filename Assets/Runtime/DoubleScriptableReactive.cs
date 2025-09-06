using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating DoubleScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(DoubleScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(DoubleScriptableReactive), order = 5)]
#endif
    /// <summary>
    /// Reactive scriptable object for double values with change notifications
    /// </summary>
    /// <remarks>
    /// This class extends ScriptableReactive<double> to provide a specialized implementation
    /// for double values with editor debugging support and change notifications.
    /// </remarks>
    public class DoubleScriptableReactive : ScriptableReactive<double>
    {
        // Inherits all functionality from ScriptableReactive<double>
    }
}