using Thisaislan.Scriptables.Interfaces;
using UnityEngine;
using System;

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
        }
#endif
    }
}