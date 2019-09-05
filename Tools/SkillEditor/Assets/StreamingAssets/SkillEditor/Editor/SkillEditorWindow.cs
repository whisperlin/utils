using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class SkillEditorWindow : EditorWindow
{
    
    [MenuItem("TA/技能编辑器/关键帧")]
    public static void Init()
    {
        SkillEditorWindow window = (SkillEditorWindow)EditorWindow.GetWindow(typeof(SkillEditorWindow));
        SkillEditorWindow.CurActive = window;
        window.Show();
         
    }
    public bool hasFocus = true;
    public static SkillEditorWindow CurActive;
    private void Awake()
    {
        CurActive = this;
    }
    void OnFocus()
    {
        hasFocus = true;
    }
    void OnLostFocus()
    {
        hasFocus = false;
    }

    public Vector2 scrollPosition = Vector2.zero;
    public float maxLength = 3f;
    public float scale = 1;
    public float viewHeight = 600f;
    public float ChannelHeight = 30f; 
    public float fps = 10;
    Rect scrollViewRect;
    Vector2 offset = new Vector3(300,50);
    float scrollSize = 30f;
 


    void SpeceLine()
    {
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }
    
    float lastTime = 0;
 
    private void Update()
    {
        UpdateColliderRender();
        if (null == SkillEditorMainWindow.golbalWindow)
        {
            Close();
        }
       
        float deltaTime = Time.realtimeSinceStartup - lastTime;
        lastTime = Time.realtimeSinceStartup;
        if (SkillEditorData.Instance.playing)
        {
            SkillEditorData.Instance.curTime += deltaTime;
            if (SkillEditorData.Instance.curTime > maxLength)
            {
                SkillEditorData.Instance.curTime = maxLength;
                SkillEditorData.Instance.playing = false;
            }
        }
        SkillEditorMainWindow.golbalWindow.UpdateSkill();
        Repaint();
    }
    private void OnEnable()
    {
        EventManager.AddEvent((int)SkillEvent.AddChanneled, AddChanneled);
    }

    private void AddChanneled(object[] args)
    {
         
    }

    private void OnDestroy()
    {
        EventManager.RemoveEvent((int)SkillEvent.AddChanneled, AddChanneled);
    }
    string[] channelNames;

    public static ObjDictionary selectEvent = null;
    public static LCHChannelData selectNormalChannel = null;
    public static LCHEventChannelData selectEventChannel = null;
    public static int selNormalKeyFrameIndex = -1;
    public static bool autoApdateKeyFrame = true;
    void OnGUI()
    {
        var width150 = GUILayout.Width(150f);
        var width50 = GUILayout.Width(50f);
        var width80 = GUILayout.Width(80f);
        var width30 = GUILayout.Width(30f);
        EditorGUI.BeginDisabledGroup(SkillEditorMainWindow.golbalWindow == null||  SkillEditorData.Instance.CurSkillId.Length == 0);
        LCHSkillData skill = null;
        if (null == SkillEditorMainWindow.golbalWindow)
        {
            viewHeight = 600f;
        }
        else
        {
            skill = SkillEditorMainWindow.golbalWindow.GetSkill();
            if (null != skill)
            {
                viewHeight = Mathf.Max(600f, ChannelHeight * skill.channels.Count + 100f);
                int c = skill.channels.Count + skill.events.Count;
                curSelectChannel = Mathf.Min(curSelectChannel, c - 1);
                int c1 = skill.channels.Count;
                int c2 = skill.events.Count;
                if (SkillEditorWindow.curSelectChannel >= 0)
                {
                    if (SkillEditorWindow.curSelectChannel < c1)
                    {
                        selectNormalChannel = skill.channels[SkillEditorWindow.curSelectChannel];
                        selectEvent = null;
                        selectEventChannel = null;
                        selNormalKeyFrameIndex = selectNormalChannel.GetKeyframeIndex(SkillEditorWindow.curSelectFrame);
                    }
                    else if (SkillEditorWindow.curSelectChannel < c2 + c1)
                    {
                        selectNormalChannel = null;
                        int index = SkillEditorWindow.curSelectChannel - c1;
                        selectEventChannel = skill.events[index];
                        selectEvent = selectEventChannel.GetKeyFrame(SkillEditorWindow.curSelectFrame);
                    }
                }
            }
            else
            {
                selectNormalChannel = null;
                selectEvent = null;
                selectEventChannel = null;
                selNormalKeyFrameIndex = -1;
            }
        }
        GUILayout.BeginArea(new Rect(0, 0, position.width, offset.y));
        GUILayout.BeginHorizontal();
        //GUI.SetNextControlName("FocusControl01");
        if (GUILayout.Button("添加", width50))
        {
            Rect buttonRect = new Rect(0f,0f,1f,1f);
            PopupWindow.Show(buttonRect, new MenuDialog());
        };
        if (GUILayout.Button("删除", width50))
        {
            EventManager.CallEvent((int)SkillEvent.RemoveChannel, skill.id, curSelectChannel);
        }
        GUILayout.Space(20f);
        if (SkillEditorData.Instance.playing)
        {
            if(GUILayout.Button("▍▍", width50))
            {
                SkillEditorData.Instance.playing = false;
            }
        }
        else
        {
            if (GUILayout.Button("▶", width50))
            {
                if (SkillEditorData.Instance.curTime == maxLength) SkillEditorData.Instance.curTime = 0f;
                SkillEditorData.Instance.playing = true;
            }
        }
        if (GUILayout.Button("■", width50))
        {
            SkillEditorData.Instance.curTime = 0f;
            SkillEditorData.Instance.playing = false;
        }
        GUILayout.Space(50f);
        showCollider = EditorGUILayout.ToggleLeft("显示碰撞体",showCollider);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("技能长度", width50);
        maxLength = EditorGUILayout.Slider( maxLength, 0f, 20f, width150);
        GUILayout.Label("工具条缩放", width50);
        scale = EditorGUILayout.Slider(  scale, 0.2f, 2f, width150);

        GUILayout.Space(30f);
        EditorGUI.BeginDisabledGroup(null == skill);
        GUI.SetNextControlName("FocusControl01");
        if (GUILayout.Button("+", width30))
        {
            if (null != skill)
            {
                EventManager.CallEvent((int)SkillEvent.AddKeyFrame, skill.id, curSelectChannel, curSelectFrame);
            }
        }
        if (GUILayout.Button("-", width30))
        {
            if (null != skill)
            {
                EventManager.CallEvent((int)SkillEvent.RemoveKeyFrame, skill.id, curSelectChannel, curSelectFrame);
            }
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Label("关键帧值:", width80);
        if (selectNormalChannel == null || selNormalKeyFrameIndex == -1)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.FloatField(0f, width80);
            
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            selectNormalChannel.values[selNormalKeyFrameIndex] = EditorGUILayout.FloatField(selectNormalChannel.values[selNormalKeyFrameIndex], width80);
        }
        GUILayout.Label("自动更新:", width80);
        autoApdateKeyFrame = EditorGUILayout.Toggle(autoApdateKeyFrame);

        
        GUILayout.Label("");
        GUILayout.EndHorizontal();
        SpeceLine();
        GUILayout.EndArea();
        float delta = 20f * scale;
        float width = maxLength * 10f * delta+2f;
        scrollViewRect = new Rect(0, offset.y, position.width, position.height - offset.y);
        Rect scrollAreaRect = new Rect(0, 0, offset.x + width, viewHeight);
        scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, scrollAreaRect);
        GUILayout.BeginArea(new Rect(offset.x+ scrollPosition.x, 0, width- scrollPosition.x, viewHeight));
        int channelCount = 0;
        int normalChanelCount = 0;
        int eventCount = 0;
        List<int> [] times;
        if (skill == null)
        {
            times = new List<int>[0] ;
        }
        else
        {
            normalChanelCount = skill.channels.Count;
            eventCount = skill.events.Count;
            channelCount = normalChanelCount+eventCount;
            times = new List<int>[channelCount];
            for (int i = 0; i < normalChanelCount; i++)
            {
                times[i] = skill.channels[i].times;
            }
            for (int i = 0; i < eventCount; i++)
            {
                times[i+normalChanelCount] = skill.events[i].times;
            }
        }
        TimeBarHelper.DrawChannels(maxLength, delta, fps, ChannelHeight, new Vector2(-scrollPosition.x, scrollPosition.y), SkillEditorData.Instance.curTime, channelCount, curSelectChannel, normalChanelCount);
        TimeBarHelper.DrawKeyFrames(maxLength, delta, fps, ChannelHeight, new Vector2(-scrollPosition.x, scrollPosition.y), curSelectChannel,curSelectFrame, times, normalChanelCount);
        TimeBarHelper.DrawTimeBar(maxLength, delta, fps, ChannelHeight, viewHeight, new Vector2(-scrollPosition.x, scrollPosition.y), SkillEditorData.Instance.curTime);
        GUILayout.EndArea();
   
        GUILayout.BeginArea(new Rect(scrollPosition.x, 0,  offset.x, viewHeight));

        if (skill == null)
        {
            ArrayHelper.ResizeArray<string>(ref channelNames, 0);
        }
        else
        {
            int c0 = skill.channels.Count;
            int c1 = skill.events.Count;
            ArrayHelper.ResizeArray<string>(ref channelNames, c0+c1);
            for (int i = 0; i < c0; i++)
            {
                var c = skill.channels[i];
                channelNames[i] = skill.GetObjectName(c.objId)+c.GetTypeName();
            }
            for (int i = 0; i < c1; i++)
            {
                var c = skill.events[i];
                channelNames[i+c0] = skill.GetObjectName(c.objId) + c.GetTypeName();
            }
        }
        TimeBarHelper.DrawHeater(offset.x,   ChannelHeight, new Vector2(-scrollPosition.x, scrollPosition.y), SkillEditorData.Instance.curTime, curSelectChannel, channelNames, normalChanelCount);
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(offset.x-1, 0), new Vector2(offset.x-1, viewHeight));
        GUILayout.EndArea();
        GUI.EndScrollView();

        if (scrollViewRect.Contains(Event.current.mousePosition))
        {
            Vector2 pos = Event.current.mousePosition + scrollPosition - offset;
            if (Event.current.mousePosition.x >= offset.x)
            {
                OnKeyFrameAreaEvent(pos );
            }
            if (Event.current.type == EventType.MouseUp)
            {
                dragging = false;
            }
        }
        EditorGUI.EndDisabledGroup();
        
    }
    void UpdateTime(float mousePos,bool forKeyFrame)
    {
        float delta = 20f * scale;
        float width = maxLength * 10f * delta;
        SkillEditorData.Instance.curTime = maxLength * mousePos / width;
        if (forKeyFrame)
        {
            SkillEditorData.Instance.curTime =    0.1f*((int)(SkillEditorData.Instance.curTime * 10f))  ;
        }
    }
    bool dragging = false;
    public static int curSelectFrame;
    public static int curSelectChannel;
    bool showCollider = true;

    void UpdateCurFramePos(Vector2 pos)
    {
        if (null == SkillEditorMainWindow.golbalWindow)
            return;
        LCHSkillData skill = SkillEditorMainWindow.golbalWindow.GetSkill();
        int c = skill.channels.Count + skill.events.Count;
        curSelectChannel = (int)(pos.y / ChannelHeight);
        curSelectChannel = Mathf.Min(curSelectChannel, c - 1);
        float delta = 20f * scale;
        float width = maxLength * 10f * delta;
        float _curSelectFrame = maxLength * pos.x / width;
        curSelectFrame = ((int)((_curSelectFrame+0.0000001f) * 10f));
        SkillEditorMainWindow.toolbarInt = 1;

    }
    void OnKeyFrameAreaEvent(Vector2 pos )
    {
        if (Event.current.button == 1)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                UpdateTime(pos.x,false);
                dragging = true;
                if (Event.current.control)
                {
                    UpdateCurFramePos(pos);
                }
            }
            if (Event.current.type == EventType.MouseDrag)
            {
                UpdateTime(pos.x,false);
            }
            curSelectChannel = -1;
            curSelectFrame = -1;
            selectNormalChannel = null;
            selectEventChannel = null;
            GUI.FocusControl("FocusControl01");
        }
        if (Event.current.button == 0)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                
                UpdateCurFramePos(pos);
                UpdateTime(pos.x,true);
                GUI.FocusControl("FocusControl01");
            }
            
        }
    }
    
    private void UpdateColliderRender()
    {
        if (null == ColliderRender.colliderRender)
        {
            GameObject g = new GameObject();
            g.name = "画线框";
            g.hideFlags = HideFlags.DontSave;
            ColliderRender.colliderRender = g.AddComponent<ColliderRender>();
        }
        if (showCollider)
        {
            ColliderRender.colliderRender.gameObject.SetActive(true);
            var skill = SkillEditorData.Instance.skill;
            if (null == skill)
            {
                ArrayHelper.ResizeArray<Collider>(ref ColliderRender.colliderRender.colliders, 0);
            }
            else
            {
                int count = 0;
                //
                for (int i = 0, c = skill.objs.Count; i < c; i++)
                {
                    var o = skill.objs[i];
                    if ((o.type == 2 || o.type == 3) && o.gameobject != null && o.gameobject.activeInHierarchy)
                    {
                        count++;
                    }
                }
                ArrayHelper.ResizeArray<Collider>(ref ColliderRender.colliderRender.colliders, count);
                count = 0;
                for (int i = 0, c = skill.objs.Count; i < c; i++)
                {
                    var o = skill.objs[i];
                    if ((o.type == 2 || o.type == 3) && o.gameobject != null && o.gameobject.activeInHierarchy)
                    {
                        ColliderRender.colliderRender.colliders[count] = o.gameobject.GetComponent<Collider>();
                        count++;
                    }
                }
                //skill.objs;
            }
        }
        else
        {
            ColliderRender.colliderRender.gameObject.SetActive(false);
        }
        
    }
}

 