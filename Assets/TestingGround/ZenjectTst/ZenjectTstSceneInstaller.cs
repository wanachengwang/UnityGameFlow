using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ZenjectTstSceneInstaller : MonoInstaller<ZenjectTstSceneInstaller> {
    public override void InstallBindings() {
        Container.BindFactory<Enemy, Enemy.Factory>();
        Container.BindInterfacesTo<ZenjectTstGameMgr>().AsTransient();
    }
}
