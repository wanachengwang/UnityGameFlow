using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace ILHotFix {
    public class ILHFInjectorHelper {

        [PostProcessScene]
        [MenuItem("ILRuntime/ILHotFix Injector/Inject")]
        static void DoInject() {
            if (EditorApplication.isCompiling || Application.isPlaying) {
                Debug.LogError("Injection failed because Unity is compiling or playing!");
                return;
            }

            ILHFInjectorCfg.ReadConfig();
            for (int i = 0; i < ILHFInjectorCfg.AssembliesPathToInject.Count; i++) {
                ILHFInjector.Inject(ILHFInjectorCfg.AssembliesPathToInject[i], ILHFInjectorCfg.MethodsToInject, ILHFInjectorCfg.AsmResolvePath);
            }
            UnityEditorInternal.InternalEditorUtility.RequestScriptReload();
        }

        [MenuItem("ILRuntime/ILHotFix Injector/UnInject")]
        static void UndoInject() {
            // In fact it is a recompile
            MonoScript[] mss = MonoImporter.GetAllRuntimeMonoScripts();
            foreach (MonoScript ms in mss) {
                if (!string.IsNullOrEmpty(ms.text)) {
                    MonoImporter.SetExecutionOrder(ms, MonoImporter.GetExecutionOrder(ms));
                    break;
                }
            }
        }

        [MenuItem("ILRuntime/ILHotFix Injector/Config")]
        static void Configuration() {
            ILHFInjectorCfg.Configuration();
        }
    }
}
