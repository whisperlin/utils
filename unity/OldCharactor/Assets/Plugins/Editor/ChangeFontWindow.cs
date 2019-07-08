using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

//[InitializeOnLoad]
public class ChangeFontWindow : EditorWindow
{

    static ChangeFontWindow()
    {
        //toChangeFont = new Font("Arial");
        //toChangeFontStyle = FontStyle.Normal;
    }

    [MenuItem("UIUtil/ChangeFont")]
    private static void ShowWindow()
    {
        ChangeFontWindow cw = EditorWindow.GetWindow<ChangeFontWindow>(true, "Util/ChangeFont");

    }
    Font toFont = new Font("Arial");
    static Font toChangeFont;
    FontStyle toFontStyle;
    static FontStyle toChangeFontStyle;
    static string[] fontSizesKey = {"A:12",
                                    "B:14",
                                    "C:16",
                                   "D:18",
                                   "E:20",
                                   "F:22",
                                   "G:24",
                                   "H:26",
                                   "I:28",
                                   "J:30",
                                   "K:32",
                                   "L:34",};
    static string[] fontColorsKey = {"1:f28f46",
                                    "2:e1935a",
                                    "3:e7724e",
                                   "4:f57b21",
                                   "5:f6d27c",
                                   "6:d0b28c",
                                   "7:fef0cb",
                                   "8:e9dfd8",
                                   "9:ffffff",
                                   "10:b0a7a5",
                                    "11:88afc5",
                                    "12:bdc1e4",
                                    "13:e9e8f3",
                                   "14:cbc8d5",
                                   "15:c1df7d",
                                   "16:81d536",
                                   "17:ff0000",
                                   "18:fa514d",
                                   "19:a28e87",
                                   "20:efc956",
                                    "21:7d7c85",
                                    "22:000000",
                                    "23:78d0ed","24:f3dede", "25:e6d7c8", "26:613728", "27:e7ad68"}; 
    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("目标字体:");
        toFont = (Font)EditorGUILayout.ObjectField(toFont, typeof(Font), true, GUILayout.MinWidth(100f));
        toChangeFont = toFont;
        GUILayout.Space(10);
        GUILayout.Label("类型:");
        Color defaultBgColor = GUI.backgroundColor;
        toFontStyle = (FontStyle)EditorGUILayout.EnumPopup(toFontStyle, GUILayout.MinWidth(100f));
        toChangeFontStyle = toFontStyle;
        GUILayout.BeginHorizontal();
        for (int i = 0; i < fontSizesKey.Length;i++ )
        {
            string valueFontSize = fontSizesKey[i];
            var sizeF = valueFontSize.Split(':');
            string sizeK = sizeF[0];
            int fontsizeV = int.Parse(sizeF[1]);
            GUILayout.BeginVertical();
            for (int j = 0; j < fontColorsKey.Length; j++)
            {
                string valueFontColor = fontColorsKey[j];
                var colorL = valueFontColor.Split(':');
                string fontColorKey = colorL[0];
                string fontColorV = colorL[1];
                Color color = hexToColor(fontColorV);
                GUI.backgroundColor = color;
                if (GUILayout.Button(sizeK + fontColorKey, GUILayout.Width(50)))
                {
                    
                    Change(fontsizeV, fontColorV);
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        GUI.backgroundColor = defaultBgColor;
    }
    public static void Change(int fontsize,string color)
    {
        //获取所有UILabel组件
        if (Selection.objects == null || Selection.objects.Length == 0)
        {
            Debug.Log("没选择字体!");
            return;
        }
        //如果是UGUI讲UILabel换成Text就可以
        Object[] labels = Selection.GetFiltered(typeof(Text), SelectionMode.TopLevel);
        foreach (Object item in labels)
        {
            //如果是UGUI讲UILabel换成Text就可以
            Text label = (Text)item;
            //label.font = toChangeFont;
            //label.fontStyle = toChangeFontStyle;
            label.fontSize = fontsize;
            label.color = hexToColor(color);
            //label.font = toChangeFont;（UGUI）
            Debug.Log(item.name + ":" + label.text);
            //
            EditorUtility.SetDirty(item);//重要
        }
    }
    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }



    private void Update()
    {
    }

    private void OnDestroy()
    {
    }


    public static string colorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }
    [MenuItem("UIUtil/TestColor")]
    public static void TestColor() {
        //c843df
        hexToColor("c843df");
    }

    public static Color hexToColor(string hex)
    {

        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

}
