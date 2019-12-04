using CinemaDirector.Helpers;
using CinemaSuite.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CinemaDirector
{
    [Serializable, CutsceneItemAttribute("Curve Clip", "Actor Curve Clip", CutsceneItemGenre.CurveClipItem)]
    public class CinemaActorClipCurve : CinemaClipCurve, IRevertable
    {
        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        public GameObject Actor
        {
            get
            {
                GameObject actor = null;
                if (transform.parent != null)
                {
                    CurveTrack track = transform.parent.GetComponent<CurveTrack>();
                    if (track != null && track.Actor != null)
                    {
                        actor = track.Actor.gameObject;
                    }
                }
                return actor;
            }
        }

        protected override bool initializeClipCurves(MemberClipCurveData data, Component component)
        {
            object value = GetCurrentValue(component, data.PropertyName, data.IsProperty);
            PropertyTypeInfo typeInfo = data.PropertyType;
            float startTime = Firetime;
            float endTime = Firetime + Duration;

            if (typeInfo == PropertyTypeInfo.Int || typeInfo == PropertyTypeInfo.Long || typeInfo == PropertyTypeInfo.Float || typeInfo == PropertyTypeInfo.Double)
            {
                float x;
                float.TryParse(value.ToString(), out x);

                if (float.IsInfinity(x) || float.IsNaN(x))
                    return false;

                data.Curve1 = AnimationCurve.Linear(startTime, x, endTime, x);
            }
            else if (typeInfo == PropertyTypeInfo.Vector2)
            {
                Vector2 vec2 = (Vector2)value;

                if (float.IsInfinity(vec2.x) || float.IsNaN(vec2.x) ||
                    float.IsInfinity(vec2.y) || float.IsNaN(vec2.y))
                    return false;

                data.Curve1 = AnimationCurve.Linear(startTime, vec2.x, endTime, vec2.x);
                data.Curve2 = AnimationCurve.Linear(startTime, vec2.y, endTime, vec2.y);
            }
            else if (typeInfo == PropertyTypeInfo.Vector3)
            {
                Vector3 vec3 = (Vector3)value;

                if (float.IsInfinity(vec3.x) || float.IsNaN(vec3.x) ||
                    float.IsInfinity(vec3.y) || float.IsNaN(vec3.y) ||
                    float.IsInfinity(vec3.z) || float.IsNaN(vec3.z))
                    return false;

                data.Curve1 = AnimationCurve.Linear(startTime, vec3.x, endTime, vec3.x);
                data.Curve2 = AnimationCurve.Linear(startTime, vec3.y, endTime, vec3.y);
                data.Curve3 = AnimationCurve.Linear(startTime, vec3.z, endTime, vec3.z);
            }
            else if (typeInfo == PropertyTypeInfo.Vector4)
            {
                Vector4 vec4 = (Vector4)value;

                if (float.IsInfinity(vec4.x) || float.IsNaN(vec4.x) ||
                    float.IsInfinity(vec4.y) || float.IsNaN(vec4.y) ||
                    float.IsInfinity(vec4.z) || float.IsNaN(vec4.z) ||
                    float.IsInfinity(vec4.w) || float.IsNaN(vec4.w))
                    return false;

                data.Curve1 = AnimationCurve.Linear(startTime, vec4.x, endTime, vec4.x);
                data.Curve2 = AnimationCurve.Linear(startTime, vec4.y, endTime, vec4.y);
                data.Curve3 = AnimationCurve.Linear(startTime, vec4.z, endTime, vec4.z);
                data.Curve4 = AnimationCurve.Linear(startTime, vec4.w, endTime, vec4.w);
            }
            else if (typeInfo == PropertyTypeInfo.Quaternion)
            {
                Quaternion quaternion = (Quaternion)value;

                if (float.IsInfinity(quaternion.x) || float.IsNaN(quaternion.x) ||
                    float.IsInfinity(quaternion.y) || float.IsNaN(quaternion.y) ||
                    float.IsInfinity(quaternion.z) || float.IsNaN(quaternion.z) ||
                    float.IsInfinity(quaternion.w) || float.IsNaN(quaternion.w))
                    return false;

                data.Curve1 = AnimationCurve.Linear(startTime, quaternion.x, endTime, quaternion.x);
                data.Curve2 = AnimationCurve.Linear(startTime, quaternion.y, endTime, quaternion.y);
                data.Curve3 = AnimationCurve.Linear(startTime, quaternion.z, endTime, quaternion.z);
                data.Curve4 = AnimationCurve.Linear(startTime, quaternion.w, endTime, quaternion.w);
            }
            else if (typeInfo == PropertyTypeInfo.Color)
            {
                Color color = (Color)value;

                if (float.IsInfinity(color.r) || float.IsNaN(color.r) ||
                    float.IsInfinity(color.g) || float.IsNaN(color.g) ||
                    float.IsInfinity(color.b) || float.IsNaN(color.b) ||
                    float.IsInfinity(color.a) || float.IsNaN(color.a))
                    return false;

                data.Curve1 = AnimationCurve.Linear(startTime, color.r, endTime, color.r);
                data.Curve2 = AnimationCurve.Linear(startTime, color.g, endTime, color.g);
                data.Curve3 = AnimationCurve.Linear(startTime, color.b, endTime, color.b);
                data.Curve4 = AnimationCurve.Linear(startTime, color.a, endTime, color.a);
            }

            return true;
        }

        public object GetCurrentValue(Component component, string propertyName, bool isProperty)
        {
            if (component == null || propertyName == string.Empty) return null;
            Type type = component.GetType();
            object value = null;
            if (isProperty)
            {
#if UNITY_2017_2_OR_NEWER
                // Deal with a special case, use the new TransformUtils to get the rotation value from the editor field.
                if(type == typeof(Transform) && propertyName == "localEulerAngles")
                {
                    value = UnityEditor.TransformUtils.GetInspectorRotation(component.transform);
                    return value;
                }
#endif
                PropertyInfo propertyInfo = ReflectionHelper.GetProperty(type, propertyName);
                value = propertyInfo.GetValue(component, null);
            }
            else
            {
                FieldInfo fieldInfo = ReflectionHelper.GetField(type, propertyName);
                value = fieldInfo.GetValue(component);
            }
            return value;
        }

        public override void Initialize()
        {
            for (int i = 0; i < CurveData.Count; i++)
            {
                CurveData[i].Initialize(Actor);
            }
        }

        /// <summary>
        /// Cache the initial state of the curve clip manipulated values.
        /// </summary>
        /// <returns>The Info necessary to revert this event.</returns>
        public RevertInfo[] CacheState()
        {
            List<RevertInfo> reverts = new List<RevertInfo>();
            if (Actor != null)
            {
                for (int i = 0; i < CurveData.Count; i++)
                {
                    Component component = Actor.GetComponent(CurveData[i].Type);
                    if (component != null)
                    {
                        RevertInfo info = new RevertInfo(this, component,
                            CurveData[i].PropertyName, CurveData[i].getCurrentValue(component));
                   
                        reverts.Add(info);
                    }
                }
            }
            return reverts.ToArray();
        }

        /// <summary>
        /// Sample the curve clip at the given time.
        /// </summary>
        /// <param name="time">The time to evaulate for.</param>
        public void SampleTime(float time)
        {
            if (Actor == null) return;
            if (Firetime <= time && time <= Firetime + Duration)
            {
                for (int i = 0; i < CurveData.Count; i++)
                {
                    if (CurveData[i].Type == string.Empty || CurveData[i].PropertyName == string.Empty) continue;

                    Component component = Actor.GetComponent(CurveData[i].Type);
                    if (component == null) return;

                    Type componentType = component.GetType();
                    object value = evaluate(CurveData[i], time);

                    if (CurveData[i].IsProperty)
                    {

                        PropertyInfo propertyInfo = ReflectionHelper.GetProperty(componentType, CurveData[i].PropertyName);
#if !UNITY_IOS
                        propertyInfo.SetValue(component, value, null);
#else
                        propertyInfo.GetSetMethod().Invoke(component, new object[] { value });
#endif
                    }
                    else
                    {
                        FieldInfo fieldInfo = ReflectionHelper.GetField(componentType, CurveData[i].PropertyName);

                        try{fieldInfo.SetValue(component, value);}
                        catch (ArgumentException){fieldInfo.SetValue(component, Mathf.RoundToInt((float)value));}
                    }
                }
            }
        }

        internal void Reset()
        {
            //foreach (MemberClipCurveData memberData in CurveData)
            //{
            //    memberData.Reset(Actor);
            //}
        }

        /// <summary>
        /// Option for choosing when this curve clip will Revert to initial state in Editor.
        /// </summary>
        public RevertMode EditorRevertMode
        {
            get { return editorRevertMode; }
            set { editorRevertMode = value; }
        }

        /// <summary>
        /// Option for choosing when this curve clip will Revert to initial state in Runtime.
        /// </summary>
        public RevertMode RuntimeRevertMode
        {
            get { return runtimeRevertMode; }
            set { runtimeRevertMode = value; }
        }
    }
}