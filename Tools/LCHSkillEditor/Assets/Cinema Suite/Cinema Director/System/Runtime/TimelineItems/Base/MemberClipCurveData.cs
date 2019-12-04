using CinemaSuite.Common;
using System;
using System.Reflection;
using UnityEngine;

namespace CinemaDirector
{
    [System.Serializable]
    public class MemberClipCurveData
    {
        public string Type;
        public string PropertyName;
        public bool IsProperty = true;
        public PropertyTypeInfo PropertyType = PropertyTypeInfo.None;

        public AnimationCurve Curve1 = new AnimationCurve();
        public AnimationCurve Curve2 = new AnimationCurve();
        public AnimationCurve Curve3 = new AnimationCurve();
        public AnimationCurve Curve4 = new AnimationCurve();

        //private object cachedProperty;

        public AnimationCurve GetCurve(int i)
        {
            if (i == 1) return Curve2;
            else if (i == 2) return Curve3;
            else if (i == 3) return Curve4;
            else return Curve1;
        }

        public void SetCurve(int i, AnimationCurve newCurve)
        {
            if (i == 1)
            {
                Curve2 = newCurve;
            }
            else if (i == 2)
            {
                Curve3 = newCurve;
            }
            else if (i == 3)
            {
                Curve4 = newCurve;
            }
            else
            {
                Curve1 = newCurve;
            }
        }

        public void Initialize(GameObject Actor)
        {
        }

        internal void Reset(GameObject Actor)
        {
        }

        internal object getCurrentValue(Component component)
        {
            if (component == null || this.PropertyName == string.Empty)
            {
                return null;
            }
            Type type = component.GetType();
            object value = null;
            if (this.IsProperty)
            {
                PropertyInfo propertyInfo = ReflectionHelper.GetProperty(type, this.PropertyName);
#if !UNITY_IOS
                value = propertyInfo.GetValue(component, null);
#else
                value = propertyInfo.GetGetMethod().Invoke(component, null);
#endif
            }
            else
            {
                FieldInfo fieldInfo = ReflectionHelper.GetField(type, this.PropertyName);
                value = fieldInfo.GetValue(component);
            }
            return value;
        }
    }
}