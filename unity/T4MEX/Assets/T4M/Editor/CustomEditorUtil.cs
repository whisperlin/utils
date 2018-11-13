using System;
using UnityEditor;
using UnityEngine;

public static class CustomEditorUtil
{
    public static KeyCode GetDownUpKeyCode()
    {
        if (Event.current.isKey && Event.current.type == EventType.KeyUp)
        {
            return Event.current.keyCode;
        }
        return KeyCode.None;
    }

    public static void GetWindow<T>()
    {
        EditorWindow.GetWindow(typeof(T));
    }
    /// <summary>
    /// 状态判断
    /// </summary>
    public static bool Check<T>(T n, T bit)
    {
        return (Convert.ToInt32(n) & Convert.ToInt32(bit)) == Convert.ToInt32(bit);
    }

    /// <summary>
    /// 状态删除
    /// </summary>
    public static T DelState<T>( T n, T bit)
    {
        int _n = Convert.ToInt32(n);
        int _bit = Convert.ToInt32(bit);
        _n &= ~_bit;
        return (T)Enum.ToObject(typeof(T), _n) ;
    }

    /// <summary>
    /// 常用于状态添加
    /// </summary>
    public static T AddState<T>( T n, T bit)
    {
        int _n = Convert.ToInt32(n);
        int _bit = Convert.ToInt32(bit);
        _n |= _bit;
        return (T)Enum.ToObject(typeof(T), _n);
    }

    public static int Log2<T>(T t)
    {
        int n = Convert.ToInt32(t);
        n = n >> 1;
        if (n == 0) return 0;
        return Log2(n)+1;
    }
}
