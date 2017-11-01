using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ZenjectTst {
    public class ZenjectTstSceneInstaller : MonoInstaller<ZenjectTstSceneInstaller> {

        public ZenjectTstGameMgr.Settings GameSettings;

        public override void InstallBindings() {
            Container.Bind<float>().FromInstance(1.0f);
            Container.Bind<ZenjectTstGameMgr.Settings>().FromInstance(GameSettings);
            Container.BindFactory<int, Enemy, Enemy.Factory>();
            Container.BindInterfacesTo<ZenjectTstGameMgr>().AsTransient();
            Container.Bind<int>().WithId("Enemy").FromInstance(1024);
            Container.BindFactory<ZTstEnemy, ZTstEnemy.Factory>().FromComponentInNewPrefabResource("ZTstEnemyPrefab").UnderTransformGroup("ZTstEnemies");
        }

        new IEnumerator Start() {
            Utils.CoroutineWithResult co = new Utils.CoroutineWithResult(this, Test());
            yield return co;
            Debug.Log(co.Result);
        }

        IEnumerator Test() {
            while (true) {
                if (Input.GetKeyDown(KeyCode.A)) {
                    yield return "A";
                    yield break;
                }
                if (Input.GetKeyDown(KeyCode.B)) {
                    yield return "B";
                    yield break;
                }
                yield return 0;
            }
        }
    }
}