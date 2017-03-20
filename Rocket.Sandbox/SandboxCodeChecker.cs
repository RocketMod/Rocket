using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;
using ThreadPool = System.Threading.ThreadPool;

namespace Rocket.Sandbox
{
    //When I wrote this, only God and I understood what I was doing
    //Now, God only knows   


    //Todo: Check libraries & external references (for now this will fail on *any* external library which is not whitelisted already), 
    //^ A reference from a plugin to another should work
    public class SandboxCodeChecker
    {
        public bool CheckBaseClass = true;

        private readonly Dictionary<Assembly, List<Type>> AllowedTypesFromAssembly = new Dictionary<Assembly, List<Type>>();
        private readonly Disassembler Disassembler = new Disassembler();
        private readonly List<string> AllowedNamespaces = new List<string>
        {
            "Rocket.API.*",
            "Rocket.Core.*",
            "Rocket.Unturned.*",
            "UnityEngine.*",
            "System.Text.*",
            "System.Xml.Serialization.*",
            "System.Linq.*",
            "System.Globalization.*",
            "System.Collections.*",
            "System.Threading.*",
            "SDG.*" //todo: block in future. SDG is too unsafe

            //"Steamworks.*" //might be a bad idea? Can be abused for achievements etc
        };

        private readonly List<string> DisallowedNamespaces = new List<string>
        {
            "UnityEditor.*",
            "UnityEngine.Windows.*",
            "UnityEngine.Tizen.*",
            "UnityEngine.iOS.*",
            "UnityEngine.Purchasing.*",
            "UnityEngine.Network.*",
            "UnityEngine.SceneManagment.*"
        };

        private readonly List<Type> AllowedTypes = new List<Type>
        {
            typeof(object),
            typeof(void),
            typeof(string),
            typeof(Math),
            typeof(Enum),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(uint),
            typeof(ulong),
            typeof(double),
            typeof(float),
            typeof(bool),
            typeof(char),
            typeof(byte),
            typeof(sbyte),
            typeof(decimal),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Array),
            typeof(MemberInfo),
            typeof(RuntimeHelpers),
            typeof(UnityEngine.Debug),
            typeof(TextWriter),
            typeof(TextReader),
            typeof(BinaryWriter),
            typeof(BinaryReader),
            typeof(Directory),
            typeof(File),
            typeof(Path),
            typeof(FileSystemInfo),
            typeof(NullReferenceException),
            typeof(ArgumentException),
            typeof(ArgumentNullException),
            typeof(FormatException),
            typeof(Exception),
            typeof(DivideByZeroException),
            typeof(InvalidCastException),
            typeof(FileNotFoundException),
            typeof(System.Random),
            typeof(Convert),
            typeof(Path),
            typeof(Convert),
            typeof(Nullable<>),
            typeof(StringComparer),
            typeof(StringComparison),
            typeof(StringBuilder),
            typeof(IComparable<>),
            typeof(Type),
            typeof(IDisposable),
            typeof(Delegate),
            typeof(ValueType),
            typeof(MulticastDelegate),
            typeof(Interlocked)
        };

        private readonly List<Type> DisallowedTypes = new List<Type>
        {
            typeof(Network),
            typeof(Process),
            typeof(ProcessStartInfo),
            typeof(DllImportAttribute),
            typeof(Activator),
            typeof(Application),
            typeof(AsyncOperation),
            typeof(Thread),
            typeof(Resources),
            typeof(ScriptableObject),
            typeof(SystemInfo),
            typeof(WebCamDevice),
            typeof(AddComponentMenu),
            typeof(ContextMenu),
            typeof(ExecuteInEditMode),
            typeof(RPC),
            typeof(Timer),
            typeof(System.Timers.Timer),
            typeof(AsyncOperation),
            typeof(System.ComponentModel.AsyncOperation),
            typeof(ThreadPool),
            typeof(Resources)
        };

        private readonly Dictionary<Type, List<string>> DisallowedMethods = new Dictionary<Type, List<string>>
        {
            {typeof(Object), new List<string> { "Destroy", "DestroyImmediate", "DestroyObject", "DontDestroyOnLoad" }},
            {typeof(Behaviour), new List<string> {"set_enabled" }}, //dont allow disabling critical components like PluginManager
            {typeof(GameObject), new List<string> { "set_active, SetActive" }}
        };

        private readonly Dictionary<Type, List<string>> AllowedMethods = new Dictionary<Type, List<string>>
        {
            //used for allowing single methods on blacklisted types
        };

        public CheckResult IsSafeAssembly(Assembly asm)
        {
            CheckResult result = new CheckResult();
            PreWhitelistCheck(asm, ref result);
            if (!result.Passed)
                return result;

            AddWhitelist(asm); //temporary whitelist, will be removed on fail
#if !DEBUG
            try
            {
#endif
                foreach (Type type in asm.GetTypes())
                {
                    /*
                    if ((!type.IsSubclassOf(typeof(MonoBehaviour)) && type != typeof(MonoBehaviour) &&
                         !typeof(MonoBehaviour).IsAssignableFrom(type)) &&
                        (type.IsSubclassOf(typeof(Behaviour)) || type == typeof(Behaviour) ||
                         typeof(Behaviour).IsAssignableFrom(type)))
                    {
                        RemoveWhitelist(asm);
                        illegalInstruction = type.FullName;
                        failReason = "Extending Unitys Behaviour. This is not allowed.";
                        return false;
                    }
                    */

                    if (!IsAllowedType(asm, type, ref result))
                    {
                        RemoveWhitelist(asm);
                        return result;
                    }
                }
#if !DEBUG
            }
            catch (Exception e)
            {
                //remove assembly before throwing exception
                RemoveWhitelist(asm);
                throw e;
            }
#endif
            return result;
        }

        private void PreWhitelistCheck(Assembly asm, ref CheckResult result)
        {
            //Check if a type name is already whitelisted, which is not allowed
            foreach (Type type in asm.GetTypes())
            {
                if (CheckWhitelistByName(string.IsNullOrEmpty(type.FullName) ? type.Name : type.FullName))
                {
                    result.IllegalInstruction = new ReadableInstruction(type, false, BlockReason.ILLEGAL_NAME);
                    result.Position = new ReadableInstruction(asm);
                    return;
                } 
            }
        }

        public bool IsAllowedType(Type type, ref CheckResult result)
        {
            ValidateResult(ref result);

            if (type == null)
                return true;

            if (type.IsGenericParameter)
                return true;

            //generic types
            if (type.IsGenericType)
                return IsAllowedType(type.DeclaringType, ref result);

            //check whitelisted namespace / name
            var val = CheckWhitelistByName(string.IsNullOrEmpty(type.FullName) ? type.Name : type.FullName);
            if (!val)
            {
                result.IllegalInstruction = new ReadableInstruction(type);
                return false;
            }
            //check for super class
            if (CheckBaseClass && type.BaseType != null  && type.BaseType.Assembly != type.Assembly /* do not check if in the same assembly, as it will be checked anyway */ && !IsAllowedType(type.BaseType, ref result))
            {
                result.IllegalInstruction = new ReadableInstruction(type, true);
                return false;
            }

            return true;
        }

        public bool IsAllowedType(Assembly asm, Type type, ref CheckResult result)
        {
            ValidateResult(ref result);

            if (type == null)
                return true;

            if (IsDelegate(type))
                return true;

            if (!IsAllowedType(type, ref result))
            {
                if(result.Position == null)
                    result.Position = new ReadableInstruction(asm);
                return false;
            }

            foreach (MethodInfo method in type.GetMethods())
            {
                if (!IsAllowedMethod(asm, type, method, ref result))
                {
                    result.Position = new ReadableInstruction(type);
                    return false;
                }
            }

            foreach (PropertyInfo def in type.GetProperties())
            {
                if (def.GetGetMethod() != null && !IsAllowedMethod(asm, type, def.GetGetMethod(), ref result))
                {
                    result.Position = new ReadableInstruction(def.GetGetMethod());
                    return false;
                }
                if (def.GetSetMethod() != null && !IsAllowedMethod(asm, type, def.GetSetMethod(), ref result))
                {
                    result.Position = new ReadableInstruction(def.GetSetMethod());
                    return false;
                }
            }

            return true;
        }

        public bool IsAllowedMethod(Assembly asm, Type type, MethodInfo method, ref CheckResult result, bool recur = true)
        {
            ValidateResult(ref result);

            if (method.DeclaringType != type)
                return true; // this method is from super class

            if (type != null && !IsAllowedType(type, ref result))
            {
                result.Position = new ReadableInstruction(method);
                return false;
            }

            if (!CheckWhitelistedMethodByName(type, method.Name))
            {
                result.Position = new ReadableInstruction(method);
                result.IllegalInstruction = new ReadableInstruction(method);
                return false;
            }

            if (!CheckMethodAttributes(method.Attributes, ref result))
            {
                result.Position = new ReadableInstruction(method);
                return false;
            }

            if (type != null && !type.IsGenericTypeDefinition && type.IsGenericType &&
                CheckGenericType(asm, type.GetGenericTypeDefinition(), method, ref result))
            {
                result.Position = new ReadableInstruction(method);
                return false;
            }

            foreach (Instruction ins in Disassembler.ReadInstructions(method))
            {
                if (recur)
                {
                    Type t = ins.Operand as Type;
                    MethodInfo m = ins.Operand as MethodInfo;

                    if (m == method) continue;

                    if (m != null)
                    {
                        if (m.DeclaringType != null && type != m.DeclaringType && m.DeclaringType != null && !IsAllowedType(m.DeclaringType, ref result))
                        {
                            result.Position = new ReadableInstruction(method);
                            return false;
                        }
                        if (!IsAllowedMethod(asm, type, m, ref result, false))
                        {
                            result.Position = new ReadableInstruction(method);
                            return false;
                        }
                    }

                    if (t != null && t != type)
                    {
                        if (!IsAllowedType(t, ref result))
                        {
                            result.Position = new ReadableInstruction(method);
                            return false;
                        }
                    }

                }
                if (ins.OpCode == OpCodes.Calli) //can call unmanaged code
                {
                    result.Position = new ReadableInstruction(method);
                    result.IllegalInstruction = new ReadableInstruction(method, ins);
                    return false;
                }
            }

            return true;
        }

        public bool CheckMethodAttributes(MethodAttributes attributes, ref CheckResult result)
        {
            ValidateResult(ref result);

            var val = (attributes & (MethodAttributes.PinvokeImpl | MethodAttributes.UnmanagedExport));
            if (val != 0)
            {
                result.IllegalInstruction = new ReadableInstruction(attributes);    
            }

            return val == 0;
        }

        public void AddWhitelist(Assembly asm)
        {
            var list = new List<Type>();
            AllowedTypesFromAssembly[asm] = list;
            foreach (Type type in asm.GetTypes()
                .Where(type => !IsInNamespaceList(type.Namespace ?? type.Name, DisallowedNamespaces)
                            && !IsInNamespaceList(type.Namespace ?? type.Name, AllowedNamespaces)))
            {
                AllowedTypes.Add(type);
                list.Add(type);
            }
        }

        public void RemoveWhitelist(Assembly asm)
        {
            if (!AllowedTypesFromAssembly.ContainsKey(asm))
                return;

            var list = AllowedTypesFromAssembly[asm];
            foreach (Type t in list)
            {
                AllowedTypes.Remove(t);
            }
            AllowedTypesFromAssembly.Remove(asm);
        }

        private bool CheckGenericType(Assembly asm, Type type, MethodInfo method, ref CheckResult result)
        {
            ValidateResult(ref result);

            if (!IsAllowedMethod(asm, type, method, ref result))
                return false;

            if (method?.DeclaringType == null)
                return true;

            foreach (var gType in method.DeclaringType.GetGenericArguments())
            {
                if (!IsAllowedType(asm, gType, ref result))
                {
                    return false;
                }
            }

            return true;
        }

        //http://stackoverflow.com/a/5819935 
        private bool IsDelegate(Type checkType)
        {
            var delegateType = typeof(Delegate);
            return delegateType.IsAssignableFrom(checkType.BaseType)
                || checkType == delegateType
                || checkType == delegateType.BaseType;
        }

        private bool CheckWhitelistByName(string fullName)
        {
            bool allowedTypeContains = AllowedTypes.Any(t => t.FullName.Equals(fullName));

            bool disallowedTypeContains = DisallowedTypes.Any(t => t.FullName.Equals(fullName));
            if (allowedTypeContains &&
                    !disallowedTypeContains)
            {
                return true;
            }

            if (IsInNamespaceList(fullName, DisallowedNamespaces))
            {
                return false;
            }

            if (IsInNamespaceList(fullName, AllowedNamespaces))
            {
                return !disallowedTypeContains;
            }

            return false;
        }

        private bool IsInNamespaceList(string fullName, List<string> namespaces)
        {
            string typeNamespace = fullName;

            bool isInList = false;
            foreach (string @namespace in namespaces)
            {
                var tmp = @namespace;
                if (@namespace.EndsWith(".*"))
                {
                    tmp = @namespace.Substring(0, @namespace.Length - 2);
                    isInList = typeNamespace.StartsWith(tmp);
                    if (isInList) break;
                }
                isInList = typeNamespace.Equals(tmp);
                if (isInList) break;
            }
            return isInList;
        }

        private bool CheckWhitelistedMethodByName(Type type, string method)
        {

            if (DisallowedMethods.ContainsKey(type) && DisallowedMethods[type].Contains(method))
            {
                return false;
            }
            if (AllowedMethods.ContainsKey(type))
            {
                return AllowedMethods[type].Contains(method);
            }

            return true;
            //return IsAllowedType(type, ref result);
        }

        private void ValidateResult(ref CheckResult result)
        {
            if (result == null)
                result = new CheckResult();
        }
    }
}