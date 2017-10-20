﻿using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : Version validate, Hotfix check and download and validate and apply

public class ILHFLoader : MonoBehaviour {

    public static readonly string DelegatePrefix = "_@Delegate@_";

    string _dllPath;
    string _pdbPath;
    //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
    //大家在正式项目中请全局只创建一个AppDomain
    ILRuntime.Runtime.Enviorment.AppDomain appdomain;

    //public ILHFLoader SetD

    void Start() {
        StartCoroutine(LoadHotFixAssembly());
    }

    IEnumerator LoadHotFixAssembly() {
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        //正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
        //正式发布的时候需要大家自行从其他地方读取dll

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //这个DLL文件是直接编译HotFix_Project.sln生成的，已经在项目中设置好输出目录为StreamingAssets，在VS里直接编译即可生成到对应目录，无需手动拷贝

        WWW www = new WWW(dllPath);

        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] dll = www.bytes;
        www.Dispose();

        //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
        www = new WWW(pdbPath);
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] pdb = www.bytes;
        using (System.IO.MemoryStream fs = new MemoryStream(dll)) {
            using (System.IO.MemoryStream p = new MemoryStream(pdb)) {
                appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
            }
        }

        InitializeILRuntime();
        OnHotFixLoaded();
    }

    void InitializeILRuntime() {
        //这里做一些ILRuntime的注册，HelloWorld示例暂时没有需要注册的
    }

    void OnHotFixLoaded() {
        ILHFInvoker.ILRTDomain = appdomain;
        //Debug.Log(appdomain.LoadedTypes.Count);
        foreach (var type in appdomain.LoadedTypes.ToArray()) {
            Type origionalType = Type.GetType(type.Value.Name);
            if (origionalType != null) {
                foreach (var method in type.Value.GetMethods()) {
                    string delegateName = GenerateMethodName(method);
                    FieldInfo field = origionalType.GetField(delegateName);
                    if (field != null) {
                        field.SetValue(null, new ILHFInvoker(method));
                        Debug.Log(delegateName + " Found and Set !!!");
                    }
                }
            }
        }
    }
    static string GenerateMethodName(IMethod method) {
        string delegateFieldName = DelegatePrefix + method.Name;
        for (int i = 1; i < method.Parameters.Count; i++) {
            delegateFieldName += "_" + method.Parameters[i].ReflectionType.Name;
        }
        delegateFieldName = delegateFieldName.Replace(".", "_").Replace("`", "_");
        return delegateFieldName;
    }
}
