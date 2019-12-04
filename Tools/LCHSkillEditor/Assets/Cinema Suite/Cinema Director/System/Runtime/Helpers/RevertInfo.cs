using CinemaSuite.Common;
// Cinema Suite
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CinemaDirector.Helpers
{
    /// <summary>
    /// Holds info related to reverting objects to a former state.
    /// </summary>
    public class RevertInfo
    {
        private MonoBehaviour MonoBehaviour;
        private Type Type;
        private object Instance;
        private MemberInfo[] MemberInfo;
        private object value;

        /// <summary>
        /// Set up a revert info for a static object.
        /// </summary>
        /// <param name="monoBehaviour">The MonoBehaviour that is making this RevertInfo.</param>
        /// <param name="type">The type of the static object</param>
        /// <param name="memberName">The member name of the field/property/method to be called on revert.</param>
        /// <param name="value">The current value you want to save.</param>
        public RevertInfo(MonoBehaviour monoBehaviour, Type type, string memberName, object value)
        {
            this.MonoBehaviour = monoBehaviour;
            this.Type = type;
            this.value = value;
            this.MemberInfo = ReflectionHelper.GetMemberInfo(type, memberName);
        }

        /// <summary>
        /// Set up Revert Info for an instance object.
        /// </summary>
        /// <param name="monoBehaviour">The MonoBehaviour that is making this RevertInfo.</param>
        /// <param name="obj">The instance of the object you want to save.</param>
        /// <param name="memberName">The member name of the field/property/method to be called on revert.</param>
        /// <param name="value">The current value you want to save.</param>
        public RevertInfo(MonoBehaviour monoBehaviour, object obj, string memberName, object value)
        {
            this.MonoBehaviour = monoBehaviour;
            this.Instance = obj;
            this.Type = obj.GetType();
            this.value = value;
            this.MemberInfo = ReflectionHelper.GetMemberInfo(Type, memberName);
        }

        /// <summary>
        /// Revert the given object to its former state.
        /// </summary>
        public void Revert()
        {
            if (MemberInfo != null && MemberInfo.Length > 0)
            {
                if (MemberInfo[0] is FieldInfo)
                {
                    FieldInfo fi = (MemberInfo[0] as FieldInfo);
                    if (fi.IsStatic || (!fi.IsStatic && Instance != null))
                    {
                        fi.SetValue(Instance, value);
                    }
                }
                else if (MemberInfo[0] is PropertyInfo)
                {
                    PropertyInfo pi = (MemberInfo[0] as PropertyInfo);
                    //if (Instance != null)
                    {
                        
                        pi.SetValue(Instance, value, null);
                    }
                }
                else if (MemberInfo[0] is MethodInfo)
                {
                    Type[] paramTypes = new Type[] { };

                    //Initialize array of parameter types
                    if (value.GetType().IsArray)
                    {
                        object[] values = (object[])value;

                        paramTypes = new Type[values.Length];
                        for (int i = 0; i < values.Length; i++)
                        {
                            paramTypes[i] = values[i].GetType();
                        }
                    }
                    else if (value != null)
                    {
                        paramTypes = new Type[] { value.GetType() };
                    }

                    //Look for a match
                    int matchIndex = -1;
                    for (int i = 0; i < MemberInfo.Length; i++)
                    {
                        ParameterInfo[] pi = (MemberInfo[i] as MethodInfo).GetParameters();
                        Type[] methodParams = new Type[pi.Length];
                        for (int j = 0; j < pi.Length; j++)
                        {
                            methodParams[j] = pi[j].ParameterType;
                        }
                        if (Enumerable.SequenceEqual(paramTypes, methodParams))
                        {
                            matchIndex = i;
                            break;
                        }
                    }

                    if (matchIndex != -1)
                    {
                        //Invoke the matching MethodInfo
                        MethodInfo mi = (MemberInfo[matchIndex] as MethodInfo);

                        if (mi.IsStatic || (!mi.IsStatic && Instance != null))
                        {
                            if (value == null)
                                mi.Invoke(Instance, null);
                            else if (value.GetType().IsArray)
                                mi.Invoke(Instance, (object[])value);
                            else
                                mi.Invoke(Instance, new object[] { value });
                        }
                    }
                    else
                    {
                        Debug.LogError("Error while reverting: Could not find method \"" + MemberInfo[0].Name +
                            "\" that accepts parameters {" + string.Join(", ", paramTypes.Select(v => v.ToString()).ToArray()) + "}.");
                    }
                }
            }
        }

        /// <summary>
        /// Should we apply this revert in runtime.
        /// </summary>
        public RevertMode RuntimeRevert
        {
            get
            {
                return (MonoBehaviour as IRevertable).RuntimeRevertMode;
            }
        }

        /// <summary>
        /// Should we apply this revert in the editor.
        /// </summary>
        public RevertMode EditorRevert
        {
            get
            {
                return (MonoBehaviour as IRevertable).EditorRevertMode;
            }
        }
    }
}
