using Thisaislan.Scriptables.Interfaces;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Utilities;
using Thisaislan.Scriptables.Editor;
using System.Reflection;
using System;

#endif

namespace Thisaislan.Scriptables.Abstracts
{
    public abstract class ScriptableSettings<T> : Scriptable<T>, IDataInitializable where T : Data
    {
        [SerializeField]
#if UNITY_EDITOR
        [Tooltip(Consts.EDITOR_DATA_TOOLTIP)]
#endif
        private T editorData;
        
        public override T Data
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    // Use runtime data during play mode
                    if (base.Data == null && editorData != null)
                    {
                        // Create a copy of editor data for runtime
                        base.Data = ReflectionUtility.CreateCopy(editorData);
                    }

                    return base.Data;
                }
                else
                {
                    // Use editor data during edit mode
                    return editorData;
                }
#else
                // In builds, always use editor data
                return editorData;
#endif
            }
            protected set
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    // Only set runtime data during play mode
                    base.Data = value;
                }
                else
                {
                    // Set editor data during edit mode
                    editorData = value;
                }
#else
                // In builds, set editor data
                editorData = value;
#endif
            }
        }

        public virtual void InitializeData(Data data)
        {
            Data = data as T;
        }

#if UNITY_EDITOR
        internal override void ResetToDefaultState()
        {
            // For ScriptableSettings, we want to clone the editor data
            if (editorData != null)
            {
                base.Data  = CloneEditorData();
            }
            else
            {
                base.Data  = null;
            }
        }

        internal override object GetDataObject()
        {
            if (Application.isPlaying)
            {
                return base.Data ;
            }
            else
            {
                return editorData;
            }
        }

        protected virtual void OnEnable()
        {
            // Reset runtime data when not in play mode
            if (!Application.isPlaying)
            {
                ResetToDefaultState();
            }
        }

        private T CloneEditorData()
        {
            // Use reflection to create a deep copy of the editorData
            try
            {
                // Create a new instance of the same type
                T clone = (T)Activator.CreateInstance(typeof(T));

                // Copy all fields from editorData to the clone
                FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object value = field.GetValue(editorData);
                    field.SetValue(clone, value);
                }

                // Copy all properties with public getters and setters
                PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo property in properties)
                {
                    if (property.CanRead && property.CanWrite)
                    {
                        object value = property.GetValue(editorData);
                        property.SetValue(clone, value);
                    }
                }

                return clone;
            }
            catch (Exception e)
            {
                Debug.LogError($"{Consts.FAILED_TO_CLONE_EDITOR_DATA}: {e.Message}");
                return null;
            }
        }
#endif
    }
}