using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHawk;

public class CannonballMario : MonoBehaviour {

    [SerializeField]
    MarioState _State;

    [SerializeField]
    Emulator _Emulator;

    [SerializeField]
    LoadRandomSavestate _StateLoader;

    // Start is called before the first frame update
    void Start() {
        _Emulator.OnRunning += () => {
            StartCoroutine(CannonLoop());
        };
    }

    IEnumerator CannonLoop() {
        while (true) {
            _State.D.health = 0;
            _State.SetState();

            yield return new WaitForSeconds(0.1f);

            while (_State.D.health == 0) {
                yield return null;
            }
            _StateLoader.Load();

            yield return new WaitForSeconds(0.1f);
        }
    }
}