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
    /// Base class for ScriptableObject-based dictionary containers with runtime persistence and editor debugging support.
    /// </summary>
    /// <typeparam name="T">The value type stored in the dictionary, where string is used as the key</typeparam>
    public abstract class ScriptableSettings<T> : SettingsEditorDebuggableScriptable,
        IInitializable<T>, IPinnable
#else
    /// <summary>
    /// Base class for ScriptableObject-based dictionary containers with runtime persistence and editor debugging support.
    /// </summary>
    /// <typeparam name="T">The value type stored in the dictionary, where string is used as the key</typeparam>
    public abstract class ScriptableSettings<T> : ScriptableObject,
        IInitializable<T>, IPinnable
#endif
    {
#if UNITY_EDITOR
        private const string EditorDataTooltip = "Default data used when no external source has initialized it.";
        private const string RuntimeDataTooltip = "Data used only in Play Mode within the Unity Editor. Reset when Play Mode ends.";
#endif

        [SerializeField]
#if UNITY_EDITOR
        [Tooltip(EditorDataTooltip)]
#endif
        private T data;

#if UNITY_EDITOR
        [SerializeField]
        [Tooltip(RuntimeDataTooltip)]
        private T runtimeData;
#endif

        /// <summary>
        /// Gets the settings data with proper editor/runtime separation.
        /// </summary>
        public T Data
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    if (runtimeData == null)
                    {
                        ResetRuntimeDataEditorOnly();
                    }
                    return runtimeData;
                }

                return data;
#else
                return data;
#endif
            }
        }

        /// <summary>
        /// Initializes the settings data with a new Data instance.
        /// </summary>
        /// <param name="data">The data to initialize with</param>
        public virtual void Initialize(T data)
        {
#if UNITY_EDITOR
            if (trackActivity)
            {
                Printer.PrintData($"{name} - {nameof(Initialize)}", data);
            }

            if (Application.isPlaying)
            {
                runtimeData = data;
            }
            else
            {
                this.data = data;
            }
#else
            this.data = (T)data;
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
        /// Resets runtime data to a copy of the current editor data.
        /// Called when entering play mode or when explicitly resetting.
        /// </summary>
        internal override void ResetRuntimeDataEditorOnly()
        {
            if (data != null)
            {
                runtimeData = CreateCopy(data);
            }
            else
            {
                runtimeData = default;
            }
        }

        /// <summary>
        /// Resets the object's data to its default state.
        /// </summary>
        internal override void ResetDataEditorOnly()
        {
            if (runtimeData != null)
            {
                data = CreateCopy(runtimeData);
            }
            else
            {
                data = default;
            }
        }

        /// <summary>
        /// Gets the appropriate data instance based on current mode.
        /// Returns runtime data during play mode, editor data during edit mode.
        /// </summary>
        internal override object GetDataEditorOnly()
        {            
            return data;
        }

        /// <summary>
        /// Gets the current runtime data instance for editor display.
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