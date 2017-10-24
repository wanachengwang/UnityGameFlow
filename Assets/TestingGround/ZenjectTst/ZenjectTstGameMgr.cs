using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ZenjectTstGameMgr : ITickable {
    static int _n = 0;
    public ZenjectTstGameMgr() {
        _n++;
        Debug.Log("ZenjectTstGameMgr:"+_n);
    }

    void ITickable.Tick() {
        if (Input.GetKey(KeyCode.A)) {
            Debug.Log("A Pressed!!!!!!");
        }
    }
}
