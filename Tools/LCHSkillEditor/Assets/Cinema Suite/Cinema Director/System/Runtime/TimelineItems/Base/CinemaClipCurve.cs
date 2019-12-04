using System;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Curve Clip Items tie Actor component data to animation curves, so that they can be controlled
    /// by curves over time.
    /// </summary>
    public abstract class CinemaClipCurve : TimelineAction
    {
        // The curve data
        [SerializeField]
        private List<MemberClipCurveData> curveData = new List<MemberClipCurveData>();

        /// <summary>
        /// Return the Curve Clip data.
        /// </summary>
        public List<MemberClipCurveData> CurveData
        {
            get { return curveData; }
        }

        protected virtual bool initializeClipCurves(MemberClipCurveData data, Component component) { return false; }

        public void AddClipCurveData(Component component, string name, bool isProperty, Type type)
        {
            MemberClipCurveData data = new MemberClipCurveData();
            data.Type = component.GetType().Name;
            data.PropertyName = name;
            data.IsProperty = isProperty;
            data.PropertyType = UnityPropertyTypeInfo.GetMappedType(type);
            if (initializeClipCurves(data, component))
            {
                curveData.Add(data);
            }
            else
            {
                Debug.LogError("Could not initialize curve clip, invalid initial values.");
            }
        }

        protected object evaluate(MemberClipCurveData memberData, float time)
        {
            object value = null;
            switch (memberData.PropertyType)
            {
                case PropertyTypeInfo.Color:
                    Color c;
                    c.r = memberData.Curve1.Evaluate(time);
                    c.g = memberData.Curve2.Evaluate(time);
                    c.b = memberData.Curve3.Evaluate(time);
                    c.a = memberData.Curve4.Evaluate(time);
                    value = c;
                    break;

                case PropertyTypeInfo.Double:
                case PropertyTypeInfo.Float:
                case PropertyTypeInfo.Int:
                case PropertyTypeInfo.Long:
                    value = memberData.Curve1.Evaluate(time);
                    break;

                case PropertyTypeInfo.Quaternion:
                    Quaternion q;
                    q.x = memberData.Curve1.Evaluate(time);
                    q.y = memberData.Curve2.Evaluate(time);
                    q.z = memberData.Curve3.Evaluate(time);
                    q.w = memberData.Curve4.Evaluate(time);
                    value = q;
                    break;

                case PropertyTypeInfo.Vector2:
                    Vector2 v2;
                    v2.x = memberData.Curve1.Evaluate(time);
                    v2.y = memberData.Curve2.Evaluate(time);
                    value = v2;
                    break;

                case PropertyTypeInfo.Vector3:
                    Vector3 v3;
                    v3.x = memberData.Curve1.Evaluate(time);
                    v3.y = memberData.Curve2.Evaluate(time);
                    v3.z = memberData.Curve3.Evaluate(time);
                    value = v3;
                    break;

                case PropertyTypeInfo.Vector4:
                    Vector4 v4;
                    v4.x = memberData.Curve1.Evaluate(time);
                    v4.y = memberData.Curve2.Evaluate(time);
                    v4.z = memberData.Curve3.Evaluate(time);
                    v4.w = memberData.Curve4.Evaluate(time);
                    value = v4;
                    break;
            }
            return value;
        }

        private void updateKeyframeTime(float oldTime, float newTime)
        {
            for (int i = 0; i < curveData.Count; i++)
            {
                int curveCount = UnityPropertyTypeInfo.GetCurveCount(curveData[i].PropertyType);
                for (int j = 0; j < curveCount; j++)
                {
                    AnimationCurve animationCurve = curveData[i].GetCurve(j);
                    for (int k = 0; k < animationCurve.length; k++)
                    {
                        Keyframe kf = animationCurve.keys[k];

                        if (Mathf.Abs(kf.time - oldTime) < 0.00001)
                        {
                            Keyframe newKeyframe = new Keyframe(newTime, kf.value, kf.inTangent, kf.outTangent);
                            newKeyframe.tangentMode = kf.tangentMode;
                            AnimationCurveHelper.MoveKey(animationCurve, k, newKeyframe);
                        }
                    }
                }
            }
        }

        public void TranslateCurves(float amount)
        {
            base.Firetime += amount;
            for (int i = 0; i < curveData.Count; i++)
            {
                int curveCount = UnityPropertyTypeInfo.GetCurveCount(curveData[i].PropertyType);
                for (int j = 0; j < curveCount; j++)
                {
                    AnimationCurve animationCurve = curveData[i].GetCurve(j);
                    if (amount > 0)
                    {
                        for (int k = animationCurve.length - 1; k >= 0; k--)
                        {
                            Keyframe kf = animationCurve.keys[k];
                            Keyframe newKeyframe = new Keyframe(kf.time + amount, kf.value, kf.inTangent, kf.outTangent);
                            newKeyframe.tangentMode = kf.tangentMode;
                            AnimationCurveHelper.MoveKey(animationCurve, k, newKeyframe);
                        }
                    }
                    else
                    {
                        for (int k = 0; k < animationCurve.length; k++)
                        {
                            Keyframe kf = animationCurve.keys[k];
                            Keyframe newKeyframe = new Keyframe(kf.time + amount, kf.value, kf.inTangent, kf.outTangent);
                            newKeyframe.tangentMode = kf.tangentMode;
                            AnimationCurveHelper.MoveKey(animationCurve, k, newKeyframe);
                        }
                    }
                }
            }
        }

        public void AlterFiretime(float firetime, float duration)
        {
            updateKeyframeTime(base.Firetime, firetime);
            base.Firetime = firetime;

            //updateKeyframeTime(base.Firetime + base.Duration, base.Firetime + duration);
            base.Duration = duration;
        }

        public void AlterDuration(float duration)
        {
            updateKeyframeTime(base.Firetime + base.Duration, base.Firetime + duration);
            base.Duration = duration;
        }
    }
}