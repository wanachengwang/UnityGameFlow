using UnityEngine;
using Zenject;

namespace ZenjectTst {
    [CreateAssetMenu(fileName = "ZenjectTstSettingInstaller", menuName = "Installers/ZenjectTstSettingInstaller")]
    public class ZenjectTstSettingInstaller : ScriptableObjectInstaller<ZenjectTstSettingInstaller> {
        public Player.Settings PlayerSettings;

        public override void InstallBindings() {
            Container.Bind<Player.Settings>().FromInstance(PlayerSettings);
        }
    }

    public class Player : ITickable {
        Settings S;
        public Player(Settings s) {
            S = s;
            Debug.Log(S.Speed);
        }

        void ITickable.Tick() {
            //throw new System.NotImplementedException();
        }

        public class Settings {
            public float Speed;
        }
    }

}