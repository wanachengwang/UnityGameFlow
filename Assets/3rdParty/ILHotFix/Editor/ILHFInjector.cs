using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ILHotFix {
    public class ILHFInjector {
        static readonly string InjectedFlagNameSpace = "_@ILHFInjector@_";
        static readonly string InjectedFlagTypeName = "Injected";
        static List<string> _methodNameFilter;
        static bool MethodFilter(MethodDefinition method) {
            if (_methodNameFilter == null || _methodNameFilter.Count == 0)
                return false;
            return _methodNameFilter.Contains(method.Name);
        }
        static bool TypeFilter(TypeDefinition type) {
            if (type.Namespace.Contains("ILRuntime"))
                return false;
            if (type.FullName.Contains("LitJson"))
                return false;
            if (type.FullName.StartsWith("<") && type.FullName.EndsWith(">"))
                return false;
            return true;
        }
        static string GenerateMethodName(MethodDefinition method) {
            string delegateFieldName = ILHFLoader.DelegatePrefix + method.Name;
            for (int i = 0; i < method.Parameters.Count; i++) {
                delegateFieldName += "_" + method.Parameters[i].ParameterType.Name;
            }
            delegateFieldName = delegateFieldName.Replace(".", "_").Replace("`", "_");
            return delegateFieldName;
        }

        public static void Inject(string assemblyPath, List<string> methods, List<string> additionalAsmResolverPath) {
            _methodNameFilter = methods;
            Debug.Log(InjectedFlagNameSpace + ": Start to inject " + assemblyPath);

            var readerParameters = new ReaderParameters { ReadSymbols = true };
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath, readerParameters);

            var resolver = assembly.MainModule.AssemblyResolver as BaseAssemblyResolver;
            foreach (var path in
                (from asm in AppDomain.CurrentDomain.GetAssemblies() select asm.ManifestModule.FullyQualifiedName)
                 .Distinct()) {
                try {
                    resolver.AddSearchDirectory(System.IO.Path.GetDirectoryName(path));
                } catch (Exception) {
                    // Do nothing
                }
            }
            if (additionalAsmResolverPath != null) {
                foreach (var path in additionalAsmResolverPath) {
                    resolver.AddSearchDirectory(path);
                }
            }

            if (assembly.Modules.Any(m => m.Types.Any(t => t.Namespace == InjectedFlagNameSpace && t.Name == InjectedFlagTypeName))) {
                Debug.LogError(InjectedFlagNameSpace + ": This assembly is already injected!");
                return;
            }
            foreach (var module in assembly.Modules) {
                foreach (var typ in module.Types.Where(TypeFilter)) {
                    foreach (var method in typ.Methods.Where(MethodFilter)) {
                        InjectMethod(typ, method);
                    }
                }
            }
            var objType = assembly.MainModule.ImportReference(typeof(object));
            assembly.MainModule.Types.Add(new TypeDefinition(InjectedFlagNameSpace, InjectedFlagTypeName, TypeAttributes.Class, objType));

            FileUtil.CopyFileOrDirectory(assemblyPath, assemblyPath + ".bak");

            var writerParameters = new WriterParameters { WriteSymbols = true };
            assembly.Write(assemblyPath, writerParameters);

            Debug.Log(InjectedFlagNameSpace + ": Inject Success!!!");

            if (assembly.MainModule.SymbolReader != null) {
                assembly.MainModule.SymbolReader.Dispose();
            }
        }

        public static void InjectMethod(TypeDefinition type, MethodDefinition method) {
            if (type.Name.Contains("<") || type.IsInterface || type.Methods.Count == 0) // skip anonymous type and interface
                return;
            if (method.Name == ".cctor" || method.HasGenericParameters)
                return;
            if (method.Parameters.Any(p => p.IsIn || p.IsOut || p.ParameterType.IsByReference))
                return;

            TypeDefinition delegateTypeRef = type.Module.Types.Single(t => t.FullName == ILHFInvoker.TypeFullName);
            if (delegateTypeRef != null) {
                delegateTypeRef = type.Module.Types.Single(x => x.Name == ILHFInvoker.TypeName);
                string delegateFieldName = GenerateMethodName(method);
                FieldDefinition item = new FieldDefinition(delegateFieldName, FieldAttributes.Static | FieldAttributes.Public, delegateTypeRef);
                FieldReference parameter = item.FieldType.Resolve().Fields.Single(field => field.Name == ILHFInvoker.ParamFieldName);

                type.Fields.Add(item);

                var invokeDeclare = type.Module.ImportReference(delegateTypeRef.Methods.Single(x => x.Name == ILHFInvoker.InvokeFuncName));
                if (!method.HasBody)
                    return;
                var insertPoint = method.Body.Instructions[0];
                var ilGenerator = method.Body.GetILProcessor();
                ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Ldsfld, item));
                ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Brfalse, insertPoint));

                ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Ldsfld, item));
                ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Dup));
                ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Ldfld, parameter));
                ilGenerator.InsertBefore(insertPoint, CreateLoadIntConst(ilGenerator, 0));
                if (method.IsStatic) {
                    ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Ldnull));
                } else {
                    ilGenerator.InsertBefore(insertPoint, CreateLoadArg(ilGenerator, 0));
                }
                ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Stelem_Ref));

                for (int i = 0; i < method.Parameters.Count; i++) {
                    ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Dup));
                    //ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Ldsfld, item));
                    ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Ldfld, parameter));

                    int index = (i + (method.IsStatic ? 0 : 1));
                    ilGenerator.InsertBefore(insertPoint, CreateLoadIntConst(ilGenerator, i + 1));
                    ilGenerator.InsertBefore(insertPoint, CreateLoadArg(ilGenerator, index));

                    if (method.Parameters[i].ParameterType.IsValueType) {
                        ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Box, method.Parameters[i].ParameterType));
                    }

                    ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Stelem_Ref));
                }

                //ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Ldsfld, item));
                ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Call, invokeDeclare));

                if (method.ReturnType.Name == "Void")
                    ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Pop));
                else if (method.ReturnType.IsValueType) {
                    ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Unbox_Any, method.ReturnType));
                }
                ilGenerator.InsertBefore(insertPoint, ilGenerator.Create(OpCodes.Ret));

                Debug.Log(InjectedFlagNameSpace + "." + type.Name + "." + method.Name + " injected!");
            }
        }

        public static Instruction CreateLoadIntConst(ILProcessor ilGenerator, int c) {
            switch (c) {
                case 0:
                    return ilGenerator.Create(OpCodes.Ldc_I4_0);
                case 1:
                    return ilGenerator.Create(OpCodes.Ldc_I4_1);
                case 2:
                    return ilGenerator.Create(OpCodes.Ldc_I4_2);
                case 3:
                    return ilGenerator.Create(OpCodes.Ldc_I4_3);
                case 4:
                    return ilGenerator.Create(OpCodes.Ldc_I4_4);
                case 5:
                    return ilGenerator.Create(OpCodes.Ldc_I4_5);
                case 6:
                    return ilGenerator.Create(OpCodes.Ldc_I4_6);
                case 7:
                    return ilGenerator.Create(OpCodes.Ldc_I4_7);
                case 8:
                    return ilGenerator.Create(OpCodes.Ldc_I4_8);
                case -1:
                    return ilGenerator.Create(OpCodes.Ldc_I4_M1);
            }
            if (c >= sbyte.MinValue && c <= sbyte.MaxValue)
                return ilGenerator.Create(OpCodes.Ldc_I4_S, (sbyte)c);

            return ilGenerator.Create(OpCodes.Ldc_I4, c);
        }
        public static Instruction CreateLoadArg(ILProcessor ilGenerator, int c) {
            switch (c) {
                case 0:
                    return ilGenerator.Create(OpCodes.Ldarg_0);
                case 1:
                    return ilGenerator.Create(OpCodes.Ldarg_1);
                case 2:
                    return ilGenerator.Create(OpCodes.Ldarg_2);
                case 3:
                    return ilGenerator.Create(OpCodes.Ldarg_3);
            }
            if (c > 0 && c < byte.MaxValue)
                return ilGenerator.Create(OpCodes.Ldarg_S, (byte)c);

            return ilGenerator.Create(OpCodes.Ldarg, c);
        }

        public static Type GetTypeByFullName(string fullName) {
            Type type = Type.GetType(fullName);
            if (type != null)
                return type;

            return null;
            //return externalAssembly.GetType(fullName);
        }
    }
}
