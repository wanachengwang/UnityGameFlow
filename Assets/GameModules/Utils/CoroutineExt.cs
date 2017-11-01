using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    public class CoroutineStoppable : IEnumerator {
        public bool Stop { get; set; }
        public Coroutine Coroutine { get; private set; }
        IEnumerator _target;
        public CoroutineStoppable(MonoBehaviour owner, IEnumerator target) {
            Stop = false;
            _target = target;
            Coroutine = owner.StartCoroutine(this);
        }

        // Interface implementations
        public object Current { get { return _target.Current; } }
        public bool MoveNext() { return !Stop && _target.MoveNext(); }
        public void Reset() { _target.Reset(); }
    }

    public class CoroutineSkippableWait : IEnumerator {
        public Coroutine Coroutine { get; private set; }
        float _statTime;
        float _timeToWait;
        Func<bool> _deleCheckSkip;
        public CoroutineSkippableWait(MonoBehaviour owner, float timeToWait, Func<bool> checkToSkip) {
            _statTime = Time.time;
            _timeToWait = timeToWait;
            _deleCheckSkip = checkToSkip;
            Coroutine = owner.StartCoroutine(this);
        }
        // Interface implementations
        public object Current { get { return null; } }
        public bool MoveNext() { return (Time.time - _statTime) < _timeToWait && !_deleCheckSkip();  }
        public void Reset() { _statTime = Time.time; }
    }

    /* There is another implementation from http://answers.unity3d.com/questions/24640/how-do-i-return-a-value-from-a-coroutine.html
     * But it should use yield return CoroutineWithResult.Coroutine, so we don't use that.
     */
    public class CoroutineWithResult : IEnumerator {
        public System.Object Result { get; private set; }
        public Coroutine Coroutine { get; private set; }
        IEnumerator _target;
        public CoroutineWithResult(MonoBehaviour owner, IEnumerator target) {
            _target = target;
            Coroutine = owner.StartCoroutine(this);
        }

        // Interface implementations
        public object Current { get { Result = _target.Current; return Result; } }
        public bool MoveNext() { return _target.MoveNext(); }
        public void Reset() { _target.Reset(); }
    }
}
