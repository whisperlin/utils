using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CutsceneItemControlAttribute(typeof(CinemaMultiActorCurveClip))]
public class CinemaMultiActorCurveControl : CinemaCurveControl
{
    public override void UpdateCurveWrappers(CinemaClipCurveWrapper clipWrapper)
    {
        CinemaMultiActorCurveClip clipCurve = clipWrapper.Behaviour as CinemaMultiActorCurveClip;
        if (clipCurve == null) return;

        for (int i = 0; i < clipCurve.CurveData.Count; i++)
        {
            MemberClipCurveData member = clipCurve.CurveData[i];

            CinemaMemberCurveWrapper memberWrapper = null;
            if (!clipWrapper.TryGetValue(member.PropertyType.ToString(), member.PropertyName, out memberWrapper))
            {
                memberWrapper = new CinemaMemberCurveWrapper();
                memberWrapper.Type = member.PropertyType.ToString();
                memberWrapper.PropertyName = member.PropertyName;
                memberWrapper.Texture = EditorGUIUtility.ObjectContent(null, UnityPropertyTypeInfo.GetUnityType(member.Type)).image;
                ArrayUtility.Add<CinemaMemberCurveWrapper>(ref clipWrapper.MemberCurves, memberWrapper);

                int showingCurves = UnityPropertyTypeInfo.GetCurveCount(member.PropertyType);
                memberWrapper.AnimationCurves = new CinemaAnimationCurveWrapper[showingCurves];

                for (int j = 0; j < showingCurves; j++)
                {
                    memberWrapper.AnimationCurves[j] = new CinemaAnimationCurveWrapper();

                    memberWrapper.AnimationCurves[j].Id = j;
                    memberWrapper.AnimationCurves[j].Curve = new AnimationCurve(member.GetCurve(j).keys);
                    memberWrapper.AnimationCurves[j].Color = UnityPropertyTypeInfo.GetCurveColor(j);
                    memberWrapper.AnimationCurves[j].Label = UnityPropertyTypeInfo.GetCurveName(member.PropertyType, j);
                }
            }
            else
            {
                int showingCurves = UnityPropertyTypeInfo.GetCurveCount(member.PropertyType);
                for (int j = 0; j < showingCurves; j++)
                {
                    memberWrapper.AnimationCurves[j].Curve = new AnimationCurve(member.GetCurve(j).keys);
                }
            }
        }

        // Remove missing track items
        List<CinemaMemberCurveWrapper> itemRemovals = new List<CinemaMemberCurveWrapper>();
        for (int i = 0; i < clipWrapper.MemberCurves.Length; i++)
        {
            CinemaMemberCurveWrapper cw = clipWrapper.MemberCurves[i];
            bool found = false;
            for (int j = 0; j < clipCurve.CurveData.Count; j++)
            {
                MemberClipCurveData member = clipCurve.CurveData[j];
                if (member.PropertyType.ToString() == cw.Type && member.PropertyName == cw.PropertyName)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                itemRemovals.Add(cw);
            }
        }
        for (int i = 0; i < itemRemovals.Count; i++)
        {
            ArrayUtility.Remove<CinemaMemberCurveWrapper>(ref clipWrapper.MemberCurves, itemRemovals[i]);
        }
    }

    protected override void CinemaCurveControl_CurvesChanged(object sender, CurveClipWrapperEventArgs e)
    {
        if (e.wrapper == null) return;
        CinemaClipCurveWrapper wrapper = e.wrapper;
        CinemaClipCurve clipCurve = wrapper.Behaviour as CinemaClipCurve;
        if (clipCurve == null) return;

        Undo.RecordObject(clipCurve, string.Format("Changed {0}", clipCurve.name));

        for (int i = 0; i < clipCurve.CurveData.Count; i++)
        {
            MemberClipCurveData member = clipCurve.CurveData[i];

            CinemaMemberCurveWrapper memberWrapper = null;
            if (wrapper.TryGetValue(member.PropertyType.ToString(), member.PropertyName, out memberWrapper))
            {
                int showingCurves = UnityPropertyTypeInfo.GetCurveCount(member.PropertyType);

                for (int j = 0; j < showingCurves; j++)
                {
                    member.SetCurve(j, new AnimationCurve(memberWrapper.AnimationCurves[j].Curve.keys));
                }
            }
        }

        clipCurve.Firetime = wrapper.Firetime;
        clipCurve.Duration = wrapper.Duration;

        EditorUtility.SetDirty(clipCurve);
    }
}
