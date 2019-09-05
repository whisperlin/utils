using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TimeBarHelper  {

    public static void DrawTimeBar(float length , float delta,float fps, float ChannelHeight,float height,Vector2 scrollPosition ,float curPosition)
    {
        float _l = 10f;
        float _s = 5f;
        int count =  (int)(length * fps) +1;
        length = count * delta;
        Handles.color = Color.gray;
        for (int i = 0; i < count; i++)
        {
            float x = i * delta  + scrollPosition.x ;
            Handles.DrawLine(new Vector3(x, 0), new Vector3(  x, height));
        }
        Handles.color = Color.black;
        Handles.DrawLine(new Vector3(0, 0), new Vector3( count * delta , 0));
        for (int i = 0; i < count; i++)
        {

            float x = i * delta + scrollPosition.x;
            if (i % fps == 0)
            {
                Handles.DrawLine(new Vector3(x, 0), new Vector3(x, _l));
                Handles.Label(  new Vector3(x, _l), i.ToString());
            }
            else
            {
                Handles.DrawLine(new Vector3(x, 0), new Vector3(x, _s));
            }
            
        }
        Handles.color = Color.red;
        float cp = curPosition*fps* delta;
        Handles.DrawLine(new Vector3(cp + scrollPosition.x, 0), new Vector3(cp + scrollPosition.x, height));
        //curPosition

        //curPosition
    }

    public static void DrawChannels(float length, float delta, float fps, float ChannelHeight, Vector2 scrollPosition, float curPosition, int channelCount,int curSelectChannel,int normalChanelCount)
    {

        Color backColor = new Color(0.678f, 0.678f, 0.678f);
       
        Color backColor2 = new Color(0.678f, 0.678f, 0.778f);
        Color eventolor = new Color(0.678f, 0.6f, 0.6f);
        Color eventolor2 = new Color(0.878f, 0.678f, 0.678f);

        int count = (int)(length * fps) + 1;
        length = count* delta;
        
        for (int i = 0,len = channelCount; i < len; i++)
        {
            float y0 = i * ChannelHeight;
            float y1 = y0+ ChannelHeight;
            if (curSelectChannel == i)
            {
                if (curSelectChannel < normalChanelCount)
                {
                    EditorGUI.DrawRect(new Rect(0, y0 + 1, length, ChannelHeight - 2), backColor2);
                }
                else
                {
                    EditorGUI.DrawRect(new Rect(0, y0 + 1, length, ChannelHeight - 2), eventolor2);
                }
                
            }
            else
            {
                if (i < normalChanelCount)
                {
                    EditorGUI.DrawRect(new Rect(0, y0 + 1, length, ChannelHeight - 2), backColor);
                }
                else
                {
                    EditorGUI.DrawRect(new Rect(0, y0 + 1, length, ChannelHeight - 2), eventolor);
                }
                
            }
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector3(0, y1), new Vector3(length, y1));

        }
         
    }

    public static void DrawKeyFrames(float length, float delta, float fps, float ChannelHeight, Vector2 scrollPosition, int curSelectChannel, int curSelectFrame, List<int>[] times,int normalChanelCount)
    {
        float _l = 10f;
        float _s = 5f;
        int count = (int)(length * fps) + 1;
        length = count * delta;
        Color backColor = new Color(0.4f, 0.4f, 0.4f);
        Color backColor3 = new Color(0.2f, 0.2f, 1f);

        Color eventColor3 = new Color(1f,0.2f, 0.2f);

        {
            float _y0 = curSelectChannel * ChannelHeight;
            float _y1 = _y0 + ChannelHeight;
            float curSelePos = curSelectFrame;
            curSelePos = curSelePos * delta;
            Color backColor2 = new Color(0.4f, 0.4f, 1f);
            EditorGUI.DrawRect(new Rect(curSelePos, _y0 , delta, ChannelHeight ), backColor2);
        }
        float _d = delta > 5f ? 2f : 0f;
        float _d2 = _d * 2f;
        for (int i = 0; i < times.Length; i++)
        {
            List<int> _times = times[i];
            for (int j = 0, _len = _times.Count; j < _len; j++)
            {
                float _y0 = i * ChannelHeight;
                float _y1 = _y0 + ChannelHeight;
                int _t = _times[j];
                float curSelePos = _t ;
                curSelePos = curSelePos * delta;
                if (i == curSelectChannel && _t == curSelectFrame)
                {
                    if (curSelectChannel < normalChanelCount)
                    {
                        EditorGUI.DrawRect(new Rect(curSelePos + _d, _y0 + 1, delta - _d2, ChannelHeight - 2), backColor3);
                    }
                    else
                    {
                        EditorGUI.DrawRect(new Rect(curSelePos + _d, _y0 + 1, delta - _d2, ChannelHeight - 2), eventColor3);
                    }
                }
                else
                {
                    EditorGUI.DrawRect(new Rect(curSelePos + _d, _y0 + 1, delta - _d2, ChannelHeight - 2), backColor);
                }
            }
        }
    }

    public static void DrawHeater(float length,  float ChannelHeight, Vector2 scrollPosition, float curPosition, int curSelectChannel, string[] names,int normalChannelCount)
    {
        int channelCount = names.Length;

        Color eventolor2 = new Color(0.878f, 0.678f, 0.678f);
        // Color backColor = new Color(0.678f, 0.678f, 0.678f);
        Color backColor2 = new Color(0.678f, 0.678f, 0.778f);
 
        for (int i = 0, len = channelCount; i < len; i++)
        {
            float y0 = i * ChannelHeight;
            float y1 = y0 + ChannelHeight;
            if (curSelectChannel == i)
            {
                if (i < normalChannelCount)
                {
                    EditorGUI.DrawRect(new Rect(0, y0 + 1, length, ChannelHeight - 2), backColor2);
                }
                else
                {
                    EditorGUI.DrawRect(new Rect(0, y0 + 1, length, ChannelHeight - 2), eventolor2);
                }
                
            }
            /*else
            {
                EditorGUI.DrawRect(new Rect(0, y0 + 1, length, ChannelHeight - 2), backColor);
            }*/
            GUI.Label(new Rect(0, y0 + 1, length, ChannelHeight - 2),names[i]);
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector3(0, y1), new Vector3(length, y1));

        }
    }
}
