using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

//using LitJson;

public class ILInjectorConfig : EditorWindow {

	//[DllImport("kernel32")]
	//private static extern long GetPrivateProfileString(string section,string key, string def,StringBuilder retVal,int size,string filePath);
	//[DllImport("kernel32")]
	//private static extern long WritePrivateProfileString(string section,string key, string val,string filePath);
    /*
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
	static List<string> _pathToResolve = new List<string>();
	static ILInjectorConfig _cfgWindow;
	static string _configFilePathName = null;
	static string ConfigFilePathName{
		get{
			if (string.IsNullOrEmpty (_configFilePathName)) {                
                MonoScript ms = MonoScript.FromScriptableObject(CreateInstance<ILInjectorConfig>());
				string scriptFilePath = AssetDatabase.GetAssetPath( ms );
				FileInfo fi = new FileInfo( scriptFilePath);
				_configFilePathName = fi.Directory.ToString() + "ILInjectorCfg.json";
				_configFilePathName.Replace( '\\', '/');
				Debug.Log( _configFilePathName );
			}
			return _configFilePathName;
		}
	}
	static void ResetDefConfig(){
		_assembliesPathToInject = new List<string> (DefAssembliesPathToInject);
		_methodsToInject = new List<string> (DefMethodsToInject);
		_pathToResolve.Clear ();
	}
    */

    static void ReadConfig() {
        /*
		if (File.Exists (ConfigFilePathName)) {
			using (StreamReader ss = new StreamReader (ConfigFilePathName)) {
                List<string>[] _stringArrayToStore = new List<string>[] {
                        _assembliesPathToInject,
                        _methodsToInject,
                        _pathToResolve,
                    };
                int idx = 0;
                LitJson.JsonReader reader = new JsonReader (ss);
				while (reader.Read ()) {
                    switch (reader.Token) {
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
            ResetDefConfig();
        }
        */
    }
    static void WriteConfig() {
        /*
        using (StreamWriter sw = new StreamWriter(ConfigFilePathName)) {
            List<string>[] _stringArrayToStore = new List<string>[] {
                    _assembliesPathToInject,
                    _methodsToInject,
                    _pathToResolve,
                };
            LitJson.JsonWriter writer = new JsonWriter(sw);
            foreach(List<string> strArray in _stringArrayToStore) {
                strArray.RemoveAll(ite => string.IsNullOrEmpty(ite));
                writer.WriteArrayStart();
                foreach (string a in strArray) {
                    writer.Write(a);
                }
                writer.WriteArrayEnd();
            }
        }
        */
    }

    
    [MenuItem("IL Injector/Inject")]
    static void Configuration() {

        ILInjectorConfig cfg = CreateInstance<ILInjectorConfig>();
        MonoScript ms = MonoScript.FromScriptableObject(cfg);
        CompileTest instance = CreateInstance<CompileTest>();
        ms = MonoScript.FromScriptableObject(instance);
        string scriptFilePath = AssetDatabase.GetAssetPath(ms);
        //_cfgWindow = GetWindow<ILInjectorConfig>(true, "IL Inject");
		//ReadConfig ();
        //_cfgWindow.Show();
    }

    /*
    void OnGUI() {
        EditorGUILayout.LabelField("Assemblies to inject:");
        for (int i = 0; i < _assembliesPathToInject.Count; i++) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("-", GUILayout.Width (20))) {
				_assembliesPathToInject.RemoveAt (i);
			} else {
				_assembliesPathToInject [i] = EditorGUILayout.TextField ("Assembly No." + i, _assembliesPathToInject [i]);
				if (GUILayout.Button ("Select", GUILayout.Width (50))) {
					string filePath = EditorUtility.OpenFilePanel ("Open a dll", "", "dll");
					if (!string.IsNullOrEmpty (filePath)) {
						_assembliesPathToInject [i] = filePath;
					}
				}
			}
			GUILayout.EndHorizontal ();
        }
		if (GUILayout.Button("+", GUILayout.Width(20))) {
            _assembliesPathToInject.Add(string.Empty);
        }

        EditorGUILayout.LabelField("Methods to inject:");
        for (int i = 0; i < _methodsToInject.Count; i++) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("-", GUILayout.Width (20))) {
				_methodsToInject.RemoveAt (i);
			} else {
				_methodsToInject [i] = EditorGUILayout.TextField ("Method No." + i, _methodsToInject [i]);
			}
			GUILayout.EndHorizontal ();
        }
        if (GUILayout.Button("+", GUILayout.Width(20))) {
            _methodsToInject.Add(string.Empty);
        }

		EditorGUILayout.LabelField("Additional path to resolve:");
		for (int i = 0; i < _pathToResolve.Count; i++) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("-", GUILayout.Width (20))) {
				_pathToResolve.RemoveAt (i);
			} else {
				_pathToResolve [i] = EditorGUILayout.TextField ("Path No." + i, _pathToResolve [i]);
				if (GUILayout.Button ("Select", GUILayout.Width (50))) {
					string folderPath = EditorUtility.OpenFolderPanel ("Open a folder", "", "");
					if (!string.IsNullOrEmpty (folderPath)) {
						_pathToResolve [i] = folderPath;
					}
				}
			}
			GUILayout.EndHorizontal ();
		}
		if (GUILayout.Button ("+", GUILayout.Width (20))) {
			_pathToResolve.Add (string.Empty);
		}

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button("Reset", GUILayout.Width(100))) {
			ResetDefConfig();
		}
        if (GUILayout.Button("Cancel", GUILayout.Width(100))) {
            ReadConfig();
            this.Close();
        }
        if (GUILayout.Button("Ok", GUILayout.Width(100))) {
            WriteConfig();
            for(int i = 0; i < _assembliesPathToInject.Count; i++) {
				ILInjector.Inject(_assembliesPathToInject[i], _methodsToInject, _pathToResolve);
            }
			UnityEditorInternal.InternalEditorUtility.RequestScriptReload();
            this.Close();
        }
		GUILayout.EndHorizontal ();
    }
    */
}
