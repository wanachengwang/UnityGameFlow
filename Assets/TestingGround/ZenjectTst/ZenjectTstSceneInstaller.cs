using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ZenjectTstSceneInstaller : MonoInstaller<ZenjectTstSceneInstaller> {

    public override void InstallBindings() {
        Container.Bind<float>().FromInstance(1.0f);
        Container.BindFactory<int, Enemy, Enemy.Factory>();
        Container.BindInterfacesTo<ZenjectTstGameMgr>().AsTransient();
        Container.Bind<int>().WithId("Enemy").FromInstance(1024);
        Container.BindFactory<ZTstEnemy, ZTstEnemy.Factory>().FromComponentInNewPrefabResource("ZTstEnemyPrefab").UnderTransformGroup("ZTstEnemies");
    }
}
