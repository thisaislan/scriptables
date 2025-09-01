using Thisaislan.Scriptables.Interfaces;
using UnityEngine;
using System;
using Thisaislan.Scriptables.Editor.Consts;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Thisaislan.Scriptables.Abstracts
{
    public abstract class ScriptableRuntime<T> : Scriptable<T>, IDataResettable where T : Data
    {
        public override T Data
        {
            get
            {
                if (base.Data == null)
                {
                    ResetData();
                }

                return base.Data;
            }

            protected set
            {
                base.Data = value;
            }
        }

        public virtual void ResetData()
        {
            Data = (T)Activator.CreateInstance(typeof(T));
        }

#if UNITY_EDITOR
        internal override void ResetToDefaultState()
        {
            // For ScriptableRuntime, we want to reset the data
            ResetData();
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
            Texture2D icon = Resources.Load<Texture2D>(MetadataEditor.ScriptableRuntime.EDITOR_ICON_NAME);

            if (icon == null)
            {
                Debug.LogWarning(MetadataEditor.ScriptableRuntime.EDITOR_ICON_NOT_FOUND_MESSAGE);
                icon = EditorGUIUtility.IconContent(MetadataEditor.Scriptable.EDITOR_DEFAULT_ICON_NAME).image as Texture2D;
            }

            EditorGUIUtility.SetIconForObject(this, icon);
        }
#endif
    }
}