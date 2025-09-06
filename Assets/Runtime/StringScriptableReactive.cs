using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating StringScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(StringScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(StringScriptableReactive), order = 6)]
#endif
    /// <summary>
    /// Reactive scriptable object for string values with change notifications
    /// </summary>
    /// <remarks>
    /// This class extends ScriptableReactive<string> to provide a specialized implementation
    /// for string values with editor debugging support and change notifications.
    /// </remarks>
    public class StringScriptableReactive : ScriptableReactive<string>
    {
        // Inherits all functionality from ScriptableReactive<string>
    }
}