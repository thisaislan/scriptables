using UnityEngine;
using Thisaislan.Scriptables.Interfaces;
using Thisaislan.Scriptables.Statics;


#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Abstracts.Bases;
using Thisaislan.Scriptables.Editor.Utilities;
#endif

namespace Thisaislan.Scriptables.Abstracts
{
#if UNITY_EDITOR
    /// <summary>
    /// Base class for ScriptableObject-based data containers with runtime persistence.
    /// </summary>
    /// <typeparam name="T">The type of data managed by this container</typeparam>
    public abstract class ScriptableRuntime<T> : RuntimeEditorDebuggableScriptable,
        ISettable<T>, IResettable, IPinnable
#else
    /// <summary>
    /// Base class for ScriptableObject-based data containers with runtime persistence.
    /// </summary>
    /// <typeparam name="T">The type of data managed by this container</typeparam>
    public abstract class ScriptableRuntime<T> : ScriptableObject,
        ISettable<T>, IResettable, IPinnable
#endif
    {
#if UNITY_EDITOR
        private const string RuntimeDataTooltip = "Data used only at runtime. It is reset when exiting Play Mode.";
#endif 
        [SerializeField]
#if UNITY_EDITOR
        [Tooltip(RuntimeDataTooltip)]
#endif
        private T runtimeData;

        /// <summary>
        /// Gets the runtime data. Automatically initializes a new instance if null.
        /// </summary>
        public T Data
        {
            get
            {
                if (runtimeData == null)
                {
                    Reset();
                }
                
                return runtimeData;
            }
        }

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="data">New data to set</param>
        public void Set(T data)
        {
#if UNITY_EDITOR
            if (trackActivity)
            {
                Printer.PrintData($"{name} - {nameof(Set)}", data);
            }
#endif
            runtimeData = data;
        }

        /// <summary>
        /// Resets the runtime data to a new default instance.
        /// </summary>
        public virtual void Reset()
        {
            runtimeData = default;

#if UNITY_EDITOR
            if (trackActivity)
            {
                Printer.PrintData($"{name} - {nameof(Reset)}", runtimeData);
            }
#endif
        }

        /// <summary>
        /// Keeps the ScriptableObject alive by adding it to the reference list
        /// </summary>
        public void Pin()
        {
#if UNITY_EDITOR
            if (trackActivity)
            {
                Printer.PrintData($"{name} - {nameof(Pin)}", runtimeData);
            }
#endif
            ReferenceKeeper.Keep(this);
        }
        
        /// <summary>
        /// Releases the ScriptableObject by removing it from the reference list
        /// </summary>
        public void Unpin()
        {
#if UNITY_EDITOR
            if (trackActivity)
            {
                Printer.PrintData($"{name} - {nameof(Unpin)}", runtimeData);
            }
#endif
            ReferenceKeeper.Unkeep(this);
        }

#if UNITY_EDITOR
        internal override void SetRuntimeDataEditorOnly(object data)
        {
            runtimeData = (T)data;
        }

        /// <summary>
        /// Resets the runtime data to its default state. Called by custom editor system.
        /// </summary>
        internal override void ResetRuntimeDataEditorOnly()
        {
            Reset();
        }

        /// <summary>
        /// Gets the current runtime value for editor display purposes.
        /// </summary>
        internal override object GetRuntimeDataEditorOnly()
        {
            if (runtimeData == null)
            {
                ResetRuntimeDataEditorOnly();
            }

            return runtimeData;
        }

        /// <summary>
        /// Cleans the object's runtime data to its default state.
        /// </summary>
        internal override void ClearRuntimeDataEditorOnly()
        {
            runtimeData = default;
        }
#endif
    }
}