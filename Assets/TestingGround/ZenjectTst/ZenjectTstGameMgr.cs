using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ZenjectTst {
    public class ZenjectTstGameMgr : ITickable {
        static int _n = 0;
        [Inject]
        Enemy.Factory _enemyFactory;
        [Inject]
        ZTstEnemy.Factory _ztEnemyFactory;

        readonly Settings _settings;

        [Inject]
        float _fField;
        //[Inject]
        float FVal {
            get { return _fField;        }
            set { _fField = value * 4;    }
        }
        //[Inject]
        void Init() {
            _fField = 4.4f;
        }
        public ZenjectTstGameMgr(float fVal, Settings settings) {
            _n++;
            _fField = 2 * fVal;
            _settings = settings;
            Debug.Log("ZenjectTstGameMgr:"+_n+","+ _fField+","+_settings.Speed);
        }

        List<ZTstEnemy> _ztEnemyLst = new List<ZTstEnemy>();
        void ITickable.Tick() {
            if (Input.GetKey(KeyCode.A)) {
                Enemy e = new Enemy();              // Not Injected
                Enemy e0 = _enemyFactory.Create(71);  // Injected
                Debug.Log("_fField:" + _fField);
                Debug.Log("Enemy Id:" + e0.Id + "," + e.Id);

                ZTstEnemy e1 = _ztEnemyFactory.Create();
                _ztEnemyLst.Add(e1);
                Debug.Log("A Pressed!!!!!!");
            }
        }

        [Serializable]
        public class Settings {
            public float Speed;
        }
    }

    public class Enemy {
        [Inject(Id = "Enemy")]
        public int Id { get; private set; }

        public Enemy() {
            Debug.Log("Create a Enemy");
        }
    
        public class Factory : Factory<int, Enemy> {
            DiContainer _container;
            public Factory(DiContainer container) {
                _container = container;
            }
            public new Enemy Create(int lv) {
                Debug.Log("Create an enemy:"+lv);
                return _container.Instantiate<Enemy>();
            }
        }

        public class MemoryPool : MemoryPool<int, Enemy> {
        }
    }
}