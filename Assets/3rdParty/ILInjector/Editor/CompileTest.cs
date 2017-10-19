using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.IO;


public class CompileTest : EditorWindow {
    [MenuItem("Assets/Run test11")]
    static void Test11() {
        CompileTest instance = CreateInstance<CompileTest>();
        MonoScript ms = MonoScript.FromScriptableObject(instance);
        string path = Application.dataPath + AssetDatabase.GetAssetPath(ms).Substring("Assets".Length);
        Debug.Log(path);
    }
/*
    //[MenuItem("Assets/Run compiler test")]
    static void Test() {
        CompileTest instance = CreateInstance<CompileTest>();
        string path = Application.dataPath + AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(instance)).Substring("Assets".Length);
        DestroyImmediate(instance);

        string arguments = "-target:library ";
        arguments += path;
        arguments += " -r:\"" + Path.Combine(UnityAssemblyPath, "UnityEngine.dll") + "\"";
        arguments += " -r:\"" + Path.Combine(UnityAssemblyPath, "UnityEditor.dll") + "\"";

        Process process = null;
        try {
            process = Process.Start(CreateCompilerStartInfo(arguments));
            process.WaitForExit();
        } catch (System.Exception e) {
            string errorMessage = e.GetType().Name + ": ";

            if (e is System.ComponentModel.Win32Exception) {
                errorMessage += "Win32 error code " + ((System.ComponentModel.Win32Exception)e).NativeErrorCode + ": ";
            }

            errorMessage += e.Message;

            Debug.LogError("Process exception (" + e.GetType().Name + "): " + errorMessage);

            return;
        }

        if (!process.StandardError.EndOfStream) {
            string error = process.StandardError.ReadToEnd();

            Debug.LogError("Process error: " + error);

            return;
        }

        Debug.Log("Success");

        AssetDatabase.Refresh();
    }



    public static bool OsIsX
    // Assuming we are never executing on non-OS X Unix
    {
        get {
            int p = (int)System.Environment.OSVersion.Platform;
            return (p == 4 || p == 6 || p == 128);
        }
    }


    public static string CompilerPath {
        get {
            return EditorApplication.applicationContentsPath +
                (OsIsX ? "/Frameworks/Mono/bin/gmcs" : "/Mono/bin/gmcs.bat");
        }
    }


    public static string UnityAssemblyPath {
        get {
            return (Application.platform == RuntimePlatform.WindowsEditor) ?
                Path.Combine(
                    EditorApplication.applicationContentsPath,
                    "Managed"
                )
            :
                Path.Combine(
                    Path.Combine(
                        EditorApplication.applicationContentsPath,
                        "Frameworks"
                    ),
                    "Managed"
                );
        }
    }


    static string ProperPath(string path) {
        return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
    }


    public static ProcessStartInfo CreateCompilerStartInfo(string arguments) {
        ProcessStartInfo startInfo;

        if (OsIsX) {
            startInfo = new ProcessStartInfo() {
                WorkingDirectory = Path.GetDirectoryName(CompilerPath),
                FileName = Path.GetFileName(CompilerPath)
            };

            string currentPath = System.Environment.GetEnvironmentVariable("PATH");
            if (currentPath == null || !currentPath.Contains(Path.GetDirectoryName(CompilerPath))) {
                System.Environment.SetEnvironmentVariable("PATH", (currentPath != null ? currentPath + ":" : "") + Path.GetDirectoryName(CompilerPath));
            }
        } else {
            startInfo = new ProcessStartInfo() {
                FileName = ProperPath(CompilerPath)
            };
        }

        startInfo.Arguments = arguments;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;

        return startInfo;
    }
    */
}