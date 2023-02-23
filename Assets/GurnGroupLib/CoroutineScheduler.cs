    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For scheduling coroutines outside of a MonoBehaviour
// Have one instance of CoroutineScheduler in the scene and call scheduler.SetTimeout(...)
public class CoroutineScheduler : MonoBehaviour {
    public void SetTimeout(float seconds, Action action) {
        StartCoroutine(_SetTimeout(seconds, action));
    }
    private IEnumerator _SetTimeout(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }
}