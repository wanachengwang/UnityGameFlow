using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILHotFixTst {
    public class ILHFHelloWorld : MonoBehaviour {

        // Use this for initialization
        void Start() {
            Debug.Log("ILHFHelloWorld: Start");
        }

        int _cnt = 0;
        // Update is called once per frame
        void Update() {
            if((_cnt++ & 0x3f) == 0) {
                Debug.Log("ILHFHelloWorld: Update");
            }
        }
    }
}
