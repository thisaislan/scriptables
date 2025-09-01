using Thisaislan.Scriptables.Interfaces;
using UnityEngine;
using System.Reflection;

using System;
using UnityEditor;




#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Utilities;
using Thisaislan.Scriptables.Editor.Consts;
#endif

namespace Thisaislan.Scriptables.Abstracts
{
    public abstract class ScriptableSettings<T> : Scriptable<T>, IDataInitializable where T : Data
    {
        [SerializeField]
#if UNITY_EDITOR
        [Tooltip(MetadataEditor.ScriptableSettings.EDITOR_DATA_TOOLTIP)]
#endif
        private T editorData;

        // Add a runtime data instance that's separate from editor data
        [System.NonSerialized]
        private T runtimeData;

        public override T Data
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    // Use runtime data during play mode
                    if (runtimeData == null && editorData != null)
                    {
                        // Create a copy of editor data for runtime
                        runtimeData = ReflectionUtility.CreateCopy(editorData);
                    }
                    return runtimeData;
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
                    runtimeData = value;
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
                runtimeData = CloneEditorData();
            }
            else
            {
                runtimeData = null;
            }
        }

        internal override object GetDataObject()
        {
            if (Application.isPlaying)
            {
                return runtimeData;
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

            SetIcon();
        }

        private void SetIcon()
        {
            Texture2D icon = Resources.Load<Texture2D>(MetadataEditor.ScriptableSettings.EDITOR_ICON_NAME);

            if (icon == null)
            {
                Debug.LogWarning(MetadataEditor.ScriptableSettings.EDITOR_ICON_NOT_FOUND_MESSAGE);
                icon = EditorGUIUtility.IconContent(MetadataEditor.Scriptable.EDITOR_DEFAULT_ICON_NAME).image as Texture2D;
            }

            EditorGUIUtility.SetIconForObject(this, icon);
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
                Debug.LogError($"Failed to clone editor data: {e.Message}");
                return null;
            }
        }
#endif
    }
}