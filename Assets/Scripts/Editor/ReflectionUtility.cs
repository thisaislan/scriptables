#if UNITY_EDITOR
using System;
using System.Reflection;
using Thisaislan.Scriptables.Abstracts;
using Thisaislan.Scriptables.Editor.Consts;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities
{
    public static class ReflectionUtility
    {
        public static void CopyObjectData(object source, object destination)
        {
            if (source == null || destination == null)
                return;

            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();

            // Only copy if types match
            if (sourceType != destinationType)
                return;

            // Copy all fields
            FieldInfo[] fields = sourceType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                try
                {
                    object value = field.GetValue(source);
                    field.SetValue(destination, value);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"{MetadataEditor.ReflectionUtility.FAILED_TO_COPY_FIELD} {field.Name}: {e.Message}");
                }
            }

            // Copy all properties with public getters and setters
            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    try
                    {
                        object value = property.GetValue(source);
                        property.SetValue(destination, value);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"{MetadataEditor.ReflectionUtility.FAILED_TO_COPY_PROPERTY} {property.Name}: {e.Message}");
                    }
                }
            }
        }

        public static T CreateCopy<T>(T original) where T : Data
        {
            if (original == null) return (T)Activator.CreateInstance(typeof(T));

            T copy = (T)Activator.CreateInstance(typeof(T));
            CopyObjectData(original, copy);
            return copy;
        }

        public static bool AreObjectsEqual(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null) return true;
            if (obj1 == null || obj2 == null) return false;
            if (obj1.GetType() != obj2.GetType()) return false;

            Type type = obj1.GetType();

            // Compare fields
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                object value1 = field.GetValue(obj1);
                object value2 = field.GetValue(obj2);

                if (value1 == null && value2 == null) continue;
                if (value1 == null || value2 == null) return false;
                if (!value1.Equals(value2)) return false;
            }

            return true;
        }
    }
}
#endif