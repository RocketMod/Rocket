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

        private readonly List<string> AllowedTypes = new List<string>
        {
            typeof(object).FullName,
            typeof(void).FullName,
            typeof(string).FullName,
            typeof(Math).FullName,
            typeof(Enum).FullName,
            typeof(short).FullName,
            typeof(int).FullName,
            typeof(long).FullName,
            typeof(uint).FullName,
            typeof(ulong).FullName,
            typeof(double).FullName,
            typeof(float).FullName,
            typeof(bool).FullName,
            typeof(char).FullName,
            typeof(byte).FullName,
            typeof(sbyte).FullName,
            typeof(decimal).FullName,
            typeof(DateTime).FullName,
            typeof(TimeSpan).FullName,
            typeof(Array).FullName,
            typeof(MemberInfo).FullName,
            typeof(RuntimeHelpers).FullName,
            typeof(UnityEngine.Debug).FullName,
            typeof(TextWriter).FullName,
            typeof(TextReader).FullName,
            typeof(BinaryWriter).FullName,
            typeof(BinaryReader).FullName,
            typeof(Directory).FullName,
            typeof(Path).FullName,
            typeof(FileSystemInfo).FullName,
            typeof(NullReferenceException).FullName,
            typeof(ArgumentException).FullName,
            typeof(ArgumentNullException).FullName,
            typeof(FormatException).FullName,
            typeof(Exception).FullName,
            typeof(DivideByZeroException).FullName,
            typeof(InvalidCastException).FullName,
            typeof(FileNotFoundException).FullName,
            typeof(System.Random).FullName,
            typeof(Convert).FullName,
            typeof(Path).FullName,
            typeof(Convert).FullName,
            typeof(Nullable<>).FullName,
            typeof(StringComparer).FullName,
            typeof(StringComparison).FullName,
            typeof(StringBuilder).FullName,
            typeof(IComparable<>).FullName,
            typeof(Type).FullName,
            typeof(IDisposable).FullName,
            typeof(Delegate).FullName,
            typeof(ValueType).FullName,
            typeof(MulticastDelegate).FullName,
            typeof(Interlocked).FullName,
            "Steamworks.CSteamID"
        };

        private readonly List<string> DisallowedTypes = new List<string>
        {
            typeof(Network).FullName,
            typeof(Process).FullName,
            typeof(ProcessStartInfo).FullName,
            typeof(DllImportAttribute).FullName,
            typeof(Activator).FullName,
            typeof(Application).FullName,
            typeof(AsyncOperation).FullName,
            typeof(Thread).FullName,
            typeof(Resources).FullName,
            typeof(ScriptableObject).FullName,
            typeof(SystemInfo).FullName,
            typeof(WebCamDevice).FullName,
            typeof(AddComponentMenu).FullName,
            typeof(ContextMenu).FullName,
            typeof(ExecuteInEditMode).FullName,
            typeof(RPC).FullName,
            typeof(Timer).FullName,
            typeof(System.Timers.Timer).FullName,
            typeof(AsyncOperation).FullName,
            typeof(System.ComponentModel.AsyncOperation).FullName,
            typeof(ThreadPool).FullName,
            typeof(Resources).FullName
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
            var types = asm.GetTypes();
            foreach (Type type in types)
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
            if (CheckBaseClass && type.BaseType != null && type.BaseType.Assembly != type.Assembly /* do not check if in the same assembly, as it will be checked anyway */ && !IsAllowedType(type.BaseType, ref result))
            {
                result.IllegalInstruction = new ReadableInstruction(type, true);
                return false;
            }

            return true;
        }

        public bool IsAllowedType(Assembly asm, Type type, ref CheckResult result, bool deepCheck = true)
        {
            ValidateResult(ref result);

            if (type == null)
                return true;

#if DEBUG
            if (asm.Equals(type.Assembly))
                Console.WriteLine("Processing " + new ReadableInstruction(type).InstructionName);
#endif

            if (IsDelegate(type))
                return true;

            if (!IsAllowedType(type, ref result))
            {
                if (result.Position == null)
                    result.Position = new ReadableInstruction(asm);
                return false;
            }

            if (!deepCheck)
                return true;

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!IsAllowedMethod(asm, type, method, ref result))
                {
                    result.Position = new ReadableInstruction(method);
                    return false;
                }
            }

            foreach (PropertyInfo def in type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var getMethod = def.GetGetMethod() ?? def.GetGetMethod(true);
                if (getMethod != null && !IsAllowedMethod(asm, type, getMethod, ref result))
                {
                    result.IllegalInstruction = new ReadableInstruction(def, getMethod);
                    result.Position = new ReadableInstruction(type);
                    return false;
                }

                var setMethod = def.GetSetMethod() ?? def.GetSetMethod(true);
                if (setMethod != null && !IsAllowedMethod(asm, type, setMethod, ref result))
                {
                    result.IllegalInstruction = new ReadableInstruction(def, setMethod);
                    result.Position = new ReadableInstruction(type);
                    return false;
                }
            }

            foreach (
                ConstructorInfo inf in
                type.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                     BindingFlags.NonPublic))
            {
                if (!IsAllowedMethod(asm, type, inf, ref result))
                {
                    result.Position = new ReadableInstruction(inf);
                    return false;
                }
            }

            if (type.TypeInitializer != null && !IsAllowedMethod(asm, type, type.TypeInitializer, ref result))
            {
                result.Position = new ReadableInstruction(type.TypeInitializer);
                return false;
            }

            return true;
        }

        public bool IsAllowedMethod(Assembly asm, Type type, MethodBase method, ref CheckResult result, bool deepCheck = true)
        {
            ValidateResult(ref result);

#if DEBUG
            Console.WriteLine("Processing " + new ReadableInstruction(method).InstructionName);
#endif

            if (!method.IsVirtual && method.DeclaringType != type)
                return true; // this method is from super class

            // check if type of method is allowed
            if (type != null && !IsAllowedType(type, ref result))
            {
                result.Position = new ReadableInstruction(method);
                result.IllegalInstruction = new ReadableInstruction(type);
                return false;
            }

            // check if method is blacklisted
            if (!CheckWhitelistedMethodByName(type, method.Name))
            {
                result.Position = new ReadableInstruction(method);
                result.IllegalInstruction = new ReadableInstruction(method);
                return false;
            }

            // check attributes (DllImport etc)
            if (!CheckMethodAttributes(method.Attributes, ref result))
            {
                result.IllegalInstruction = new ReadableInstruction(method.Attributes);
                result.Position = new ReadableInstruction(method);
                return false;
            }

            // check generic parameters
            if (method is MethodInfo && type != null && !type.IsGenericTypeDefinition && type.IsGenericType &&
                CheckGenericType(asm, type.GetGenericTypeDefinition(), (MethodInfo)method, ref result, asm.Equals(method.DeclaringType?.Assembly)))
            {
                result.IllegalInstruction = new ReadableInstruction(type.GetGenericTypeDefinition());
                result.Position = new ReadableInstruction(method);
                return false;
            }

            if (!deepCheck)
                return true;

            var instructions = Disassembler.ReadInstructions(method);
            foreach (Instruction ins in instructions)
            {
                Type t = ins.Operand as Type;
                MethodInfo m = ins.Operand as MethodInfo;

                if (m == method)
                    continue;

                if (m != null) // check referenced  methods
                {
                    if (m.DeclaringType != null && type != m.DeclaringType && m.DeclaringType != null &&
                        !IsAllowedType(m.DeclaringType, ref result))
                    {
                        result.Position = new ReadableInstruction(method);
                        result.IllegalInstruction = new ReadableInstruction(m.DeclaringType);
                        return false;
                    }
                    if (!CheckWhitelistedMethodByName(m.DeclaringType, m.Name))
                    {
                        result.Position = new ReadableInstruction(method);
                        result.IllegalInstruction = new ReadableInstruction(m);
                        return false;
                    }
                }

                if (t != null && t != type) // check referenced types in method
                {
                    if (!IsAllowedType(t, ref result))
                    {
                        result.Position = new ReadableInstruction(method);
                        result.IllegalInstruction = new ReadableInstruction(t);
                        return false;
                    }
                }

                if (ins.OpCode == OpCodes.Calli) // calli allows calls to unmanaged code
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
                AllowedTypes.Add(string.IsNullOrEmpty(type.FullName) ? type.Name : type.FullName);
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
                AllowedTypes.Remove(string.IsNullOrEmpty(t.FullName) ? t.Name : t.FullName);
            }
            AllowedTypesFromAssembly.Remove(asm);
        }

        private bool CheckGenericType(Assembly asm, Type type, MethodInfo method, ref CheckResult result, bool checkMethods = true)
        {
            ValidateResult(ref result);

            if (method?.DeclaringType == null)
                return true;

            foreach (var gType in method.DeclaringType.GetGenericArguments())
            {
                if (!IsAllowedType(asm, gType, ref result, checkMethods))
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
            bool allowedTypeContains = AllowedTypes.Any(t => t.Equals(fullName));

            bool disallowedTypeContains = DisallowedTypes.Any(t => t.Equals(fullName));
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