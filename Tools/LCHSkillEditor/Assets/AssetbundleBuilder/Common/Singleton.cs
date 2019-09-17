using UnityEngine;
using System;
using System.Collections;

namespace Common
{
    /// <summary>
    /// 单例模板类
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class Singleton<T> where T : class, new()
    {
        //
        // Static Fields
        //
        protected static T m_Instance;

        //
        // Static Properties
        //
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new T();
                }
                return m_Instance;
            }
        }

        //
        // Static Methods
        //
        public static T GetInstance()
        {
            return Instance;
        }
    }
}
