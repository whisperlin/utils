using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CutsceneItemControlAttribute(typeof(CinemaClipCurve))]
public class CinemaCurveControl : CinemaCurveClipItemControl
{
    protected bool hasUndoRedoBeenPerformed = false;
    protected bool hasClipBeenUpdated = false;

    public CinemaCurveControl()
    {
        base.TranslateCurveClipItem += CinemaCurveControl_TranslateCurveClipItem;
        base.SnapScrubber += CinemaCurveControl_SnapScrubber;
        base.CurvesChanged += CinemaCurveControl_CurvesChanged;

        actionIcon = EditorGUIUtility.Load("Cinema Suite/Cinema Director/Director_CurvesIcon.png") as Texture;
    }

    protected virtual void CinemaCurveControl_CurvesChanged(object sender, CurveClipWrapperEventArgs e)
    {
        if(e.wrapper == null) return;
        CinemaClipCurveWrapper wrapper = e.wrapper;
        CinemaClipCurve clipCurve = wrapper.Behaviour as CinemaClipCurve;
        if (clipCurve == null) return;

        Undo.RecordObject(clipCurve, string.Format("Changed {0}", clipCurve.name));

        for (int i = 0; i < clipCurve.CurveData.Count; i++)
        {
            MemberClipCurveData member = clipCurve.CurveData[i];

            CinemaMemberCurveWrapper memberWrapper = null;
            if (wrapper.TryGetValue(member.Type, member.PropertyName, out memberWrapper))
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

    void CinemaCurveControl_SnapScrubber(object sender, CurveClipScrubberEventArgs e)
    {
        CinemaClipCurve curveClip = e.curveClipItem as CinemaClipCurve;
        if (curveClip == null) return;
        
        curveClip.Cutscene.EnterPreviewMode(e.time);
        curveClip.Cutscene.SetRunningTime(e.time); // will always set running time even if already in preview mode.
    }

    void CinemaCurveControl_TranslateCurveClipItem(object sender, CurveClipItemEventArgs e)
    {
        CinemaClipCurve curveClip = e.curveClipItem as CinemaClipCurve;
        if (curveClip == null) return;

        Undo.RecordObject(e.curveClipItem, string.Format("Changed {0}", curveClip.name));

        curveClip.TranslateCurves(e.firetime - curveClip.Firetime);

        EditorUtility.SetDirty(e.curveClipItem);
    }

    public override void UpdateCurveWrappers(CinemaClipCurveWrapper clipWrapper)
    {
        CinemaClipCurve clipCurve = clipWrapper.Behaviour as CinemaClipCurve;
        if (clipCurve == null) return;

        for (int i = 0; i < clipCurve.CurveData.Count; i++)
        {
            MemberClipCurveData member = clipCurve.CurveData[i];

            CinemaMemberCurveWrapper memberWrapper = null;
            if (!clipWrapper.TryGetValue(member.Type, member.PropertyName, out memberWrapper))
            {
                memberWrapper = new CinemaMemberCurveWrapper();
                memberWrapper.Type = member.Type;
                memberWrapper.PropertyName = member.PropertyName;
                memberWrapper.Texture = EditorGUIUtility.ObjectContent(null, UnityPropertyTypeInfo.GetUnityType(member.Type)).image;
                ArrayUtility.Add<CinemaMemberCurveWrapper>(ref clipWrapper.MemberCurves, memberWrapper);

                int showingCurves = UnityPropertyTypeInfo.GetCurveCount(member.PropertyType);
                memberWrapper.AnimationCurves = new CinemaAnimationCurveWrapper[showingCurves];

                for (int j = 0; j < showingCurves; j++)
                {
                    memberWrapper.AnimationCurves[j] = new CinemaAnimationCurveWrapper();

                    memberWrapper.AnimationCurves[j].Id = j;
                    memberWrapper.AnimationCurves[j].Curve = new AnimationCurve(member.GetCurve(j).keys); // Make a deep copy.

                    memberWrapper.AnimationCurves[j].Label = UnityPropertyTypeInfo.GetCurveName(member.PropertyType, j);
                    memberWrapper.AnimationCurves[j].Color = UnityPropertyTypeInfo.GetCurveColor(member.Type, member.PropertyName, memberWrapper.AnimationCurves[j].Label, j);
                }
            }
            else
            {
                int showingCurves = UnityPropertyTypeInfo.GetCurveCount(member.PropertyType);
                for (int j = 0; j < showingCurves; j++)
                {
                    memberWrapper.AnimationCurves[j].Curve = new AnimationCurve(member.GetCurve(j).keys); // Make a deep copy.
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
                if (member.Type == cw.Type && member.PropertyName == cw.PropertyName)
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
}
