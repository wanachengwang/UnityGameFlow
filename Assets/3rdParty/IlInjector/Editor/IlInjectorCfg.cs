#if true    //UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
/*
[DllImport("kernel32")]
private static extern long GetPrivateProfileString(string section,string key, string def,StringBuilder retVal,int size,string filePath);
[DllImport("kernel32")]
private static extern long WritePrivateProfileString(string section,string key, string val,string filePath);
*/

public class IlInjectorConfig : EditorWindow {
    static List<string> _assembliesPathToInject = new List<string>();
    static List<string> _methodsToInject = new List<string>();
    static void ReadConfig() {

    }
    static void WriteConfig() {
        _assembliesPathToInject.RemoveAll(ite => string.IsNullOrEmpty(ite));
        _methodsToInject.RemoveAll(ite => string.IsNullOrEmpty(ite));
    }

    
    [MenuItem("Il Injector/Inject")]
    void Configuration(Action<string, List<string>> actInject) {
        IlInjectorConfig cfgWindow = GetWindow<IlInjectorConfig>(true, "Il Inject");
        cfgWindow.Show();
    }

    void OnGUI() {
        EditorGUILayout.LabelField("Assemblies to inject:");
        for (int i = 0; i < _assembliesPathToInject.Count; i++) {
            if (GUILayout.Button("-", GUILayout.Width(200))) {
                _assembliesPathToInject.RemoveAt(i);
                continue;
            }
            _assembliesPathToInject[i] = EditorGUILayout.TextField("Assembly No." + i, _assembliesPathToInject[i]);
            if (GUILayout.Button("Select", GUILayout.Width(200))) {
                string filePath = EditorUtility.OpenFilePanel("Open a dll", "", "dll");
                if (!string.IsNullOrEmpty(filePath)) {
                    _assembliesPathToInject[i] = filePath;
                }
            }
        }
        if (GUILayout.Button("+", GUILayout.Width(200))) {
            _assembliesPathToInject.Add(string.Empty);
        }

        EditorGUILayout.LabelField("Methods to inject:");
        for (int i = 0; i < _methodsToInject.Count; i++) {
            if (GUILayout.Button("-", GUILayout.Width(200))) {
                _methodsToInject.RemoveAt(i);
                continue;
            }
            _methodsToInject[i] = EditorGUILayout.TextField("Method No." + i, _methodsToInject[i]);
        }
        if (GUILayout.Button("+", GUILayout.Width(200))) {
            _methodsToInject.Add(string.Empty);
        }

        if (GUILayout.Button("Cancel", GUILayout.Width(200))) {
            ReadConfig();
            this.Close();
        }
        if (GUILayout.Button("Ok", GUILayout.Width(200))) {
            WriteConfig();
            /*
            if(_actInject != null) {
                for(int i = 0; i < _assembliesPathToInject.Count; i++) {
                    _actInject(_assembliesPathToInject[i], _methodsToInject);
                }
            }
            */
            this.Close();
        }

    }

}
#endif
