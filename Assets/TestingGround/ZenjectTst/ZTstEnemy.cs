using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ZTstEnemy : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Create a ZTstEnemy");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public class Factory : Factory<ZTstEnemy> {
    }
}
