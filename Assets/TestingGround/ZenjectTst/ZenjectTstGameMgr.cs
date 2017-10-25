using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ZenjectTstGameMgr : ITickable {
    static int _n = 0;

    //[Inject]
    Enemy _enemy0;
    //[Inject]
    //Enemy _enemy1;

    [Inject]
    Enemy.Factory _enemyFactory;

    public ZenjectTstGameMgr() {
        _n++;
        Debug.Log("ZenjectTstGameMgr:"+_n);
        _enemy0 = _enemyFactory.Create();
    }

    void ITickable.Tick() {
        if (Input.GetKey(KeyCode.A)) {
            Debug.Log("A Pressed!!!!!!");
        }
    }
}

public class Enemy {
    public Enemy() {
        Debug.Log("Create a Enemy");
    }
    public class Factory : Factory<Enemy> { }
}