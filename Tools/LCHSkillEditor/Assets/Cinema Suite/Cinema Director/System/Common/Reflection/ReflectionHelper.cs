using System;
using System.Reflection;
#if NETFX_CORE
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endif

namespace CinemaSuite.Common
{
    /// A helper class for reflection calls, that allows for calls on multiple platforms.
    public static class ReflectionHelper
    {
#if NETFX_CORE
        private static List<Assembly> assemblies = new List<Assembly>();
#endif
        public static Assembly[] GetAssemblies()
        {
#if NETFX_CORE
            if(assemblies == null || assemblies.Count == 0)
            {
                var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
			    var files = folder.GetFilesAsync();
			    files.AsTask().Wait();

                foreach (var file in files.GetResults())
                {
				    if (file.FileType == ".dll" || file.FileType == ".exe")
                    {
				        try
				        {
				            var filename = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
				            AssemblyName name = new AssemblyName { Name = filename };
				            Assembly assembly = Assembly.Load(name);
				            assemblies.Add(assembly);
				        }
				        catch (Exception)
				        {
				            continue;
				        }
				    }
			    }

                var typeInfo = typeof(CinemaDirector.Cutscene).GetTypeInfo();
                assemblies.Add(typeInfo.Assembly);  
            }
                return assemblies.ToArray();
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }

        public static Type[] GetTypes(Assembly assembly)
        {
#if NETFX_CORE
            var types = new List<Type>();
            //foreach (var t in assembly.GetTypes())
            foreach (var t in assembly.DefinedTypes.Select(aa => aa.AsType()).ToArray())
            {
                types.Add(t);
            }
            return types.ToArray();
#else
            return assembly.GetTypes();
#endif
        }

        public static bool IsSubclassOf(Type type, Type c)
        {
#if NETFX_CORE
			    return type.GetTypeInfo().IsSubclassOf(c);
#else
                return type.IsSubclassOf(c);
#endif
        }

        public static MemberInfo[] GetMemberInfo(Type type, string name)
        {
#if NETFX_CORE
                var members = new List<MemberInfo>();
                members.Add(GetField(type, name));
                members.Add(GetProperty(type, name));
                return members.ToArray();
#else
                return type.GetMember(name);
#endif
        }

        public static FieldInfo GetField(Type type, string name)
        {
#if NETFX_CORE
                return type.GetTypeInfo().GetDeclaredField(name);
#else
                return type.GetField(name);
#endif
        }

        public static PropertyInfo GetProperty(Type type, string name)
        {
#if NETFX_CORE
			    return type.GetTypeInfo().GetDeclaredProperty(name);
#else
                return type.GetProperty(name);
#endif
        }

        public static T[] GetCustomAttributes<T>(Type type, bool inherited) where T : Attribute
        {
#if NETFX_CORE
			    return (T[]) type.GetTypeInfo().GetCustomAttributes(typeof(T), inherited);
#else
                return (T[]) type.GetCustomAttributes(typeof(T), inherited);
#endif
        }
    }
}