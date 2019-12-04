
using System;

namespace CinemaDirector
{
    /// <summary>
    /// Enumeration of all Property types in Unity
    /// </summary>
    using UnityEngine;
    public enum PropertyTypeInfo
    {
        Color,
        Double,
        Float,
        Int,
        Long,
        None,
        Quaternion,
        Vector2,
        Vector3,
        Vector4
    }

    public static class UnityPropertyTypeInfo
    {
        public static PropertyTypeInfo GetMappedType(Type type)
        {
            if (type == typeof(int))
            {
                return PropertyTypeInfo.Int;
            }
            if (type == typeof(long))
            {
                return PropertyTypeInfo.Long;
            }
            if (type == typeof(float))
            {
                return PropertyTypeInfo.Float;
            }
            if (type == typeof(double))
            {
                return PropertyTypeInfo.Double;
            }
            if (type == typeof(Vector2))
            {
                return PropertyTypeInfo.Vector2;
            }
            if (type == typeof(Vector3))
            {
                return PropertyTypeInfo.Vector3;
            }
            if (type == typeof(Vector4))
            {
                return PropertyTypeInfo.Vector4;
            }
            if (type == typeof(Quaternion))
            {
                return PropertyTypeInfo.Quaternion;
            }
            if (type == typeof(Color))
            {
                return PropertyTypeInfo.Color;
            }
            return PropertyTypeInfo.None;
        }

        public static int GetCurveCount(int p)
        {
            PropertyTypeInfo info = (PropertyTypeInfo)p;
            return GetCurveCount(info);
        }

        public static int GetCurveCount(PropertyTypeInfo info)
        {

            if (info == PropertyTypeInfo.Int || info == PropertyTypeInfo.Long || info == PropertyTypeInfo.Float ||
                info == PropertyTypeInfo.Double)
            {
                return 1;
            }
            else if (info == PropertyTypeInfo.Vector2)
            {
                return 2;
            }
            else if (info == PropertyTypeInfo.Vector3)
            {
                return 3;
            }
            else if (info == PropertyTypeInfo.Vector4 || info == PropertyTypeInfo.Quaternion || info == PropertyTypeInfo.Color)
            {
                return 4;
            }
            return 0;
        }

        public static Color GetCurveColor(int i)
        {
            Color c = Color.white;
            if (i == 0)
            {
                c = Color.red;
            }
            else if (i == 1)
            {
                c = Color.green;
            }
            else if (i == 2)
            {
                c = Color.blue;
            }
            else if (i == 3)
            {
                c = Color.yellow;
            }

            return c;
        }

        public static Color GetCurveColor(string Type, string PropertyName, string label, int i)
        {
            Color c = Color.white;
            if (Type == "Transform")
            {
                if (PropertyName == "localPosition" || PropertyName == "position")
                {
                    if (label == "x" || i == 0)
                    {
                        c = Color.red;
                    }
                    if (label == "y" || i == 1)
                    {
                        c = Color.green;
                    }
                    if (label == "z" || i == 2)
                    {
                        c = Color.blue;
                    }
                }
                if (PropertyName == "localEulerAngles")
                {
                    if (label == "x" || i == 0)
                    {
                        c = Color.magenta;
                    }
                    if (label == "y" || i == 1)
                    {
                        c = Color.yellow;
                    }
                    if (label == "z" || i == 2)
                    {
                        c = Color.cyan;
                    }
                }
                if (PropertyName == "localScale")
                {
                    if (label == "x" || i == 0)
                    {
                        c = new Color(0.6745f, 0.4392f, 0.4588f, 1f);
                    }
                    if (label == "y" || i == 1)
                    {
                        c = new Color(0.447f, 0.6196f, 0.4588f, 1f);
                    }
                    if (label == "z" || i == 2)
                    {
                        c = new Color(0.447f, 0.4392f, 0.7294f, 1f);
                    }
                }
            }
            else
            {
                c = GetCurveColor(i);
            }

            return c;
        }

        public static string GetCurveName(PropertyTypeInfo info, int i)
        {
            string retVal = "x";
            if (i == 1)
            {
                retVal = "y";
            }
            else if (i == 2)
            {
                retVal = "z";
            }
            else if (i == 3)
            {
                retVal = "w";
            }


            if (info == PropertyTypeInfo.Int || info == PropertyTypeInfo.Long || info == PropertyTypeInfo.Float ||
                info == PropertyTypeInfo.Double)
            {
                retVal = "value";
            }
            else if (info == PropertyTypeInfo.Color)
            {
                if (i == 0)
                {
                    retVal = "r";
                }
                else if (i == 1)
                {
                    retVal = "g";
                }
                else if (i == 2)
                {
                    retVal = "b";
                }
                else if (i == 3)
                {
                    retVal = "a";
                }
            }

            return retVal;
        }

        public static Type GetUnityType(string typeName)
        {
            if (typeName == "Transform")
            {
                return typeof(Transform);
            }
            else if (typeName == "Camera")
            {
                return typeof(Camera);
            }
            else if (typeName == "Light")
            {
                return typeof(Light);
            }

            return typeof(Transform);
        }
    }
}