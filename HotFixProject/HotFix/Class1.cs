using System;
using UnityEngine;

// Patch the same fullname(namespace&classname)
namespace ILHotFix {
    public class ILHFHelloWorld {
        public static void Start(ILHFHelloWorld self) {
            Debug.Log("HotFix:ILHFHelloWorld.Start.");
        }
        static int _cnt = 0;
        public static void Update(ILHFHelloWorld self) {
            if((_cnt++&0x3f) == 0) {
                Debug.Log("HotFix:ILHFHelloWorld.Update.");
            }
        }
    }
}
