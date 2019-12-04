using CinemaDirector.Helpers;
using CinemaSuite.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CinemaDirector
{
    [Serializable, CutsceneItemAttribute("Curve Clip", "MultiActor Curve Clip", CutsceneItemGenre.MultiActorCurveClipItem)]
    public class CinemaMultiActorCurveClip : CinemaClipCurve, IRevertable
    {
        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        public List<Component> Components = new List<Component>();
        public List<string> Properties = new List<string>();

        public CinemaMultiActorCurveClip()
        {
            CurveData.Add(new MemberClipCurveData());
        }

        public void SampleTime(float time)
        {
            if (Firetime <= time && time <= Firetime + Duration)
            {
                MemberClipCurveData data = CurveData[0];
                if (data == null) return;

                if (data.PropertyType == PropertyTypeInfo.None)
                {
                    return;
                }

                for (int i = 0; i < Components.Count; i++)
                {
                    object value = null;
                    switch (data.PropertyType)
                    {
                        case PropertyTypeInfo.Color:
                            Color c;
                            c.r = data.Curve1.Evaluate(time);
                            c.g = data.Curve2.Evaluate(time);
                            c.b = data.Curve3.Evaluate(time);
                            c.a = data.Curve4.Evaluate(time);
                            value = c;
                            break;

                        case PropertyTypeInfo.Double:
                        case PropertyTypeInfo.Float:
                        case PropertyTypeInfo.Long:
                            value = data.Curve1.Evaluate(time);
                            break;
                        case PropertyTypeInfo.Int:
                            value = Mathf.RoundToInt(data.Curve1.Evaluate(time));
                            break;
                        case PropertyTypeInfo.Quaternion:
                            Quaternion q;
                            q.x = data.Curve1.Evaluate(time);
                            q.y = data.Curve2.Evaluate(time);
                            q.z = data.Curve3.Evaluate(time);
                            q.w = data.Curve4.Evaluate(time);
                            value = q;
                            break;

                        case PropertyTypeInfo.Vector2:
                            Vector2 v2;
                            v2.x = data.Curve1.Evaluate(time);
                            v2.y = data.Curve2.Evaluate(time);
                            value = v2;
                            break;

                        case PropertyTypeInfo.Vector3:
                            Vector3 v3;
                            v3.x = data.Curve1.Evaluate(time);
                            v3.y = data.Curve2.Evaluate(time);
                            v3.z = data.Curve3.Evaluate(time);
                            value = v3;
                            break;

                        case PropertyTypeInfo.Vector4:
                            Vector4 v4;
                            v4.x = data.Curve1.Evaluate(time);
                            v4.y = data.Curve2.Evaluate(time);
                            v4.z = data.Curve3.Evaluate(time);
                            v4.w = data.Curve4.Evaluate(time);
                            value = v4;
                            break;
                    }
                    if (Components[i] != null && Properties[i] != null && Properties[i] != "None")
                    {
                        PropertyInfo propertyInfo = ReflectionHelper.GetProperty(Components[i].GetType(), Properties[i]);
                        propertyInfo.SetValue(Components[i], value, null);
                    }
                }
            }
        }

        public List<Transform> Actors
        {
            get
            {
                List<Transform> actors = new List<Transform>();
                if (transform.parent != null)
                {
                    MultiCurveTrack track = transform.parent.GetComponent<MultiCurveTrack>();
                    MultiActorTrackGroup trackgroup = (track.TrackGroup as MultiActorTrackGroup);
                    actors = trackgroup.Actors;
                }
                return actors;
            }
        }


        /// <summary>
        /// Cache the initial state of the curve clip manipulated values.
        /// </summary>
        /// <returns>The Info necessary to revert this clip.</returns>
        public RevertInfo[] CacheState()
        {
            List<RevertInfo> reverts = new List<RevertInfo>();
            for (int i = 0; i < Actors.Count; i++)
            {
                if (i >= Components.Count || i >= Properties.Count)
                    continue;

                if (Components[i] != null && Properties[i] != null && Properties[i] != "None")
                {
                    Component component = Components[i];
                    PropertyInfo propertyInfo = ReflectionHelper.GetProperty(Components[i].GetType(), Properties[i]);
                    reverts.Add(new RevertInfo(this, component, Properties[i], 
                        propertyInfo.GetValue(Components[i], null)));
                }
            }
            return reverts.ToArray();
        }

        internal void Revert()
        {
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