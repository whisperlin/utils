//****************************************************************************
//
//  File:      OptimizeAnimationClipTool.cs
//
//  Copyright (c) SuiJiaBin
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
//****************************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.IO;
using UnityEngine.Profiling;

namespace EditorTool
{
    public class AnimationOpt
    {
        static Dictionary<uint,string> _FLOAT_FORMAT;
        static MethodInfo getAnimationClipStats;
        static FieldInfo sizeInfo;
        static object[] _param = new object[1];

        static AnimationOpt ()
        {
            _FLOAT_FORMAT = new Dictionary<uint, string> ();
            for (uint i = 1; i < 6; i++) {
                _FLOAT_FORMAT.Add (i, "f" + i.ToString ());
            }
            Assembly asm = Assembly.GetAssembly (typeof(Editor));
            getAnimationClipStats = typeof(AnimationUtility).GetMethod ("GetAnimationClipStats", BindingFlags.Static | BindingFlags.NonPublic);
            Type aniclipstats = asm.GetType ("UnityEditor.AnimationClipStats");
            sizeInfo = aniclipstats.GetField ("size", BindingFlags.Public | BindingFlags.Instance);
        }

        AnimationClip _clip;
        string _path;

        public string path { get{ return _path;} }

        public long originFileSize { get; private set; }

        public int originMemorySize { get; private set; }

        public int originInspectorSize { get; private set; }

        public long optFileSize { get; private set; }

        public int optMemorySize { get; private set; }

        public int optInspectorSize { get; private set; }

        public AnimationOpt (string path, AnimationClip clip)
        {
            _path = path;
            _clip = clip;
            _GetOriginSize ();
        }

        void _GetOriginSize ()
        {
            originFileSize = _GetFileZie ();
            originMemorySize = _GetMemSize ();
            originInspectorSize = _GetInspectorSize ();
        }

        void _GetOptSize ()
        {
            optFileSize = _GetFileZie ();
            optMemorySize = _GetMemSize ();
            optInspectorSize = _GetInspectorSize ();
        }

        long _GetFileZie ()
        {
            FileInfo fi = new FileInfo (_path);
            return fi.Length;
        }

        int _GetMemSize ()
        {
            return Profiler.GetRuntimeMemorySize (_clip);
        }

        int _GetInspectorSize ()
        {
            _param [0] = _clip;
            var stats = getAnimationClipStats.Invoke (null, _param);
            return (int)sizeInfo.GetValue (stats);
        }

        void _OptmizeAnimationScaleCurve ()
        {
            if (_clip != null) {
                //去除scale曲线
                foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(_clip)) {
                    string name = theCurveBinding.propertyName.ToLower ();
                    if (name.Contains ("scale")) {
                        AnimationUtility.SetEditorCurve (_clip, theCurveBinding, null);
                        Debug.LogFormat ("关闭{0}的scale curve", _clip.name);
                    }
                } 
            }
        }

        void _OptmizeAnimationFloat_X (uint x)
        {
            if (_clip != null && x > 0) {
                //浮点数精度压缩到f3
                AnimationClipCurveData[] curves = null;
                curves = AnimationUtility.GetAllCurves (_clip);
             
                Keyframe key;
                Keyframe[] keyFrames;
                string floatFormat;
                if (_FLOAT_FORMAT.TryGetValue (x, out floatFormat)) {
                    if (curves != null && curves.Length > 0) {
                        for (int ii = 0; ii < curves.Length; ++ii) {
                            AnimationClipCurveData curveDate = curves [ii];
                            if (curveDate.curve == null || curveDate.curve.keys == null) {
                                //Debug.LogWarning(string.Format("AnimationClipCurveData {0} don't have curve; Animation name {1} ", curveDate, animationPath));
                                continue;
                            }
                            keyFrames = curveDate.curve.keys;
                            for (int i = 0; i < keyFrames.Length; i++) {
                                key = keyFrames [i];
                                key.value = float.Parse (key.value.ToString (floatFormat));
                                key.inTangent = float.Parse (key.inTangent.ToString (floatFormat));
                                key.outTangent = float.Parse (key.outTangent.ToString (floatFormat));
                                keyFrames [i] = key;
                            }

                            
                            curveDate.curve.keys = keyFrames;

                            _clip.SetCurve (curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
                        }
                    }
                } else {
                    Debug.LogErrorFormat ("目前不支持{0}位浮点", x);
                }
            }
        }

        void _OptmizeAnimationNoChange() {
            if (_clip != null) {
                //AnimationClip newClip = new AnimationClip();
                //newClip.name = _clip.name;

                AnimationClipCurveData[] curves = null;
                curves = AnimationUtility.GetAllCurves(_clip, true);
                _clip.ClearCurves();

                Keyframe key;
                Keyframe keyLast;
                Keyframe[] keyFrames;

                List<int> removeIndexs = new List<int>();
 
                if (curves != null && curves.Length > 0) {
                    for (int ii = 0; ii < curves.Length; ++ii) {
                        AnimationClipCurveData curveDate = curves[ii];
                        if (curveDate.curve == null || curveDate.curve.keys == null) {
                            continue;
                        }

                        AnimationCurve animaCurve = curveDate.curve;
                        keyFrames = animaCurve.keys;
                        bool keyLine = true;

                        if (keyFrames.Length <= 2) {
                            keyLine = false;
                        } else {
                            for (int i = 1; i < keyFrames.Length; i++) {
                                keyLast = keyFrames[i-1];
                                key = keyFrames[i];
                                if (Mathf.Abs(key.value - keyLast.value) >= 0.001f || Mathf.Abs(key.outTangent) != 0) {
                                    keyLine = false;
                                    break;
                                }
                            }
                        }

                        if (keyLine) { //是直线的话保留第一帧
                            for (int i = keyFrames.Length - 2; i >= 1; --i) {
                                animaCurve.RemoveKey(i);
                            }

                            // animaCurve.keys = new Keyframe[2] { keyFrames[0], keyFrames[keyFrames.Length - 1] };
                        }

                        curveDate.curve = animaCurve;
                    }

                    for (int i = 0; i < curves.Length; i++) {
                        AnimationClipCurveData curveDate = curves[i];
                        if (curveDate.curve == null || curveDate.curve.keys == null) {
                            continue;
                        }

                        _clip.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
                    }
                }

 
                AssetDatabase.Refresh();
            }
        }

        public void Optimize (bool scaleOpt, uint floatSize)
        {
            if (scaleOpt) {
                _OptmizeAnimationScaleCurve ();
            }

            _OptmizeAnimationNoChange();
            _OptmizeAnimationFloat_X(floatSize);
            
            _GetOptSize ();
        }

        public void OptimizeSize ()
        {
            Optimize (false, 4);
        }

        public void LogOrigin ()
        {
            _logSize (originFileSize, originMemorySize, originInspectorSize);
        }

        public void LogOpt ()
        {
            _logSize (optFileSize, optMemorySize, optInspectorSize);
        }

        public void LogDelta ()
        {

        }

        void _logSize (long fileSize, int memSize, int inspectorSize)
        {
            Debug.LogFormat ("{0} \nSize=[ {1} ]", _path, string.Format ("FSize={0} ; Mem->{1} ; inspector->{2}",
                EditorUtility.FormatBytes (fileSize), EditorUtility.FormatBytes (memSize), EditorUtility.FormatBytes (inspectorSize)));
        }
    }

    public class OptimizeAnimationClipTool
    {
        static List<AnimationOpt> _AnimOptList = new List<AnimationOpt> ();
        static List<string> _Errors = new List<string>();
        static int _Index = 0;

        [MenuItem("Assets/Animation/裁剪浮点数与去掉不变帧")]
        public static void Optimize()
        {
            _AnimOptList = FindAnims ();
            if (_AnimOptList.Count > 0)
            {
                _Index = 0;
                _Errors.Clear ();
                ScanAnimationClip();
                EditorApplication.update = ScanAnimationClip;
            }
        }

        public static void ScanAnimationClip()
        {
            bool isCancel = false;
            try {
                AnimationOpt _AnimOpt = _AnimOptList[_Index];
                isCancel = EditorUtility.DisplayCancelableProgressBar("优化AnimationClip", _AnimOpt.path, (float)_Index / (float)_AnimOptList.Count);
                _AnimOpt.OptimizeSize();
                _Index++;
            }
            catch(Exception e){
                isCancel = true;
            }
            if (isCancel || _Index >= _AnimOptList.Count)
            {
                EditorUtility.ClearProgressBar();
                Debug.Log(string.Format("--优化完成--    错误数量: {0}    总数量: {1}/{2}    错误信息↓:\n{3}\n----------输出完毕----------", _Errors.Count, _Index, _AnimOptList.Count, string.Join(string.Empty, _Errors.ToArray())));
                Resources.UnloadUnusedAssets();
                GC.Collect();
                AssetDatabase.SaveAssets();
                EditorApplication.update = null;
                _AnimOptList.Clear();
                _cachedOpts.Clear ();
                _Index = 0;
            }
        }

        static Dictionary<string,AnimationOpt> _cachedOpts = new Dictionary<string, AnimationOpt> ();

        public static AnimationOpt GetNewAOpt (string path)
        {
            AnimationOpt opt = null;
            if (!_cachedOpts.ContainsKey(path)) {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip> (path);
                if (clip != null) {
                    opt = new AnimationOpt (path, clip);
                    //_cachedOpts [path] = opt;
                }
            }
            return opt;
        }

        public static List<AnimationOpt> FindAnims()
        {
            string[] guids = null;
            List<string> path = new List<string>();
            List<AnimationOpt> assets = new List<AnimationOpt> ();
            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(object), SelectionMode.Assets);
            if (objs.Length > 0)
            {
                for(int i = 0; i < objs.Length; i++)
                {
                    if (objs [i].GetType () == typeof(AnimationClip))
                    {
                        string p = AssetDatabase.GetAssetPath (objs [i]);
                        AnimationOpt animopt = GetNewAOpt (p);
                        if (animopt != null)
                            assets.Add (animopt);
                    }
                    else
                        path.Add(AssetDatabase.GetAssetPath (objs [i]));
                }
                if(path.Count > 0)
                    guids = AssetDatabase.FindAssets (string.Format ("t:{0}", typeof(AnimationClip).ToString().Replace("UnityEngine.", "")), path.ToArray());
                else
                    guids = new string[]{};
            }
            for(int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath (guids [i]);
                AnimationOpt animopt = GetNewAOpt (assetPath);
                if (animopt != null)
                    assets.Add (animopt);
            }
            return assets;
        }
    }


}