using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ILHotFix {
    public class ILHFInjectorCfg : EditorWindow {

        static readonly string[] DefAssembliesPathToInject = new string[]{
            @"Library\ScriptAssemblies\Assembly-CSharp.dll",
        };
        static readonly string[] DefMethodsToInject = new string[]{
            "Awake","Start","Update","LateUpdate","OnEnable","OnDisable","OnDestroy",
            "OnTriggerEnter","OnTriggerStay","OnTriggerExit",
            "OnCollisionEnter","OnCollisionStay","OnCollisionExit",
        };
        static List<string> _assembliesPathToInject = new List<string>();
        static List<string> _methodsToInject = new List<string>();
        static List<string> _asmResolvePath = new List<string>();
        static ILHFInjectorCfg _cfgWindow;
        static string _configFilePathName = null;
        static string ConfigFilePathName {
            get {
                if (string.IsNullOrEmpty(_configFilePathName)) {
                    ILHFInjectorCfg inst = _cfgWindow ?? CreateInstance<ILHFInjectorCfg>();
                    MonoScript ms = MonoScript.FromScriptableObject(inst);
                    string scriptFilePath = AssetDatabase.GetAssetPath(ms);
                    FileInfo fi = new FileInfo(scriptFilePath);
                    _configFilePathName = fi.Directory.ToString() + "/" + inst.GetType().Name + ".json";
                    _configFilePathName.Replace('\\', '/');
                    Debug.Log(_configFilePathName);
                    if (inst != _cfgWindow) {
                        DestroyImmediate(inst);
                    }
                }
                return _configFilePathName;
            }
        }
        public static List<string> AssembliesPathToInject { get { return _assembliesPathToInject; } }
        public static List<string> MethodsToInject { get { return _methodsToInject; } }
        public static List<string> AsmResolvePath { get { return _asmResolvePath; } }

        static void ResetDefConfig() {
            _assembliesPathToInject = new List<string>(DefAssembliesPathToInject);
            _methodsToInject = new List<string>(DefMethodsToInject);
            _asmResolvePath.Clear();
        }
        public static void ReadConfig() {
            if (File.Exists(ConfigFilePathName)) {
                using (StreamReader ss = new StreamReader(ConfigFilePathName)) {
                    List<string>[] _stringArrayToStore = new List<string>[] {
                        _assembliesPathToInject,
                        _methodsToInject,
                        _asmResolvePath,
                    };
                    int idx = 0;
                    LitJson.JsonReader reader = new JsonReader(ss);
                    while (reader.Read()) {
                        switch (reader.Token) {
                            case JsonToken.ArrayStart:
                                _stringArrayToStore[idx].Clear();
                                break;
                            case JsonToken.ArrayEnd:
                                idx++;
                                break;
                            case JsonToken.String:
                                _stringArrayToStore[idx].Add(reader.Value as string);
                                break;
                        }
                    }
                }
            } else {
                Debug.LogError("[ILInjector]:No configuration file found!");
                ResetDefConfig();
            }
        }
        public static void WriteConfig() {
            using (StreamWriter sw = new StreamWriter(ConfigFilePathName)) {
                List<string>[] _stringArrayToStore = new List<string>[] {
                    _assembliesPathToInject,
                    _methodsToInject,
                    _asmResolvePath,
                };
                LitJson.JsonWriter writer = new JsonWriter(sw);
                writer.WriteArrayStart();
                foreach (List<string> strArray in _stringArrayToStore) {
                    strArray.RemoveAll(ite => string.IsNullOrEmpty(ite));
                    writer.WriteArrayStart();
                    foreach (string a in strArray) {
                        writer.Write(a);
                    }
                    writer.WriteArrayEnd();
                }
                writer.WriteArrayEnd();
            }
        }
        public static void Configuration() {
            _cfgWindow = GetWindow<ILHFInjectorCfg>(true, "IL Inject");
            ReadConfig();
            _cfgWindow.Show();
        }

        void OnGUI() {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Default", GUILayout.Width(100))) {
                ResetDefConfig();
            }
            if (GUILayout.Button("Revert", GUILayout.Width(100))) {
                ReadConfig();
            }
            if (GUILayout.Button("Apply", GUILayout.Width(100))) {
                WriteConfig();
            }
            GUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Assemblies to inject:");
            for (int i = 0; i < _assembliesPathToInject.Count; i++) {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(20))) {
                    _assembliesPathToInject.RemoveAt(i);
                } else {
                    _assembliesPathToInject[i] = EditorGUILayout.TextField("Assembly No." + i, _assembliesPathToInject[i]);
                    if (GUILayout.Button("Select", GUILayout.Width(50))) {
                        string filePath = EditorUtility.OpenFilePanel("Open a dll", "", "dll");
                        if (!string.IsNullOrEmpty(filePath)) {
                            _assembliesPathToInject[i] = filePath;
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+", GUILayout.Width(20))) {
                _assembliesPathToInject.Add(string.Empty);
            }

            EditorGUILayout.LabelField("Methods to inject:");
            for (int i = 0; i < _methodsToInject.Count; i++) {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(20))) {
                    _methodsToInject.RemoveAt(i);
                } else {
                    _methodsToInject[i] = EditorGUILayout.TextField("Method No." + i, _methodsToInject[i]);
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+", GUILayout.Width(20))) {
                _methodsToInject.Add(string.Empty);
            }

            EditorGUILayout.LabelField("Additional path to resolve:");
            for (int i = 0; i < _asmResolvePath.Count; i++) {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(20))) {
                    _asmResolvePath.RemoveAt(i);
                } else {
                    _asmResolvePath[i] = EditorGUILayout.TextField("Path No." + i, _asmResolvePath[i]);
                    if (GUILayout.Button("Select", GUILayout.Width(50))) {
                        string folderPath = EditorUtility.OpenFolderPanel("Open a folder", "", "");
                        if (!string.IsNullOrEmpty(folderPath)) {
                            _asmResolvePath[i] = folderPath;
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+", GUILayout.Width(20))) {
                _asmResolvePath.Add(string.Empty);
            }
            if (EditorGUI.EndChangeCheck()) {
                Debug.Log("GUI Changed.");
            }
            _cfgWindow.position = new Rect(_cfgWindow.position.position, new Vector2(400, (_assembliesPathToInject.Count + 2 + _methodsToInject.Count + 2 + _asmResolvePath.Count + 2 + 1) * 20));
        }
    }
}

