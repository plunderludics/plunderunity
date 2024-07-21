using System.Collections;
using System.Collections.Generic;
using MutCommon;
using Soil;
using UnityEngine;
using UnityEngine.UI;

public class ScrubbingMario : MonoBehaviour {
    [Header("config")]
    [SerializeField] DynamicEase _Ease;
    [SerializeField] FloatRange _RangeY;
    [SerializeField] float _Delay;
    [SerializeField] float _Scale;

    [Header("refs")]
    [SerializeField] AudioSource _Source;
    [SerializeField] MarioState _Mario;
    [SerializeField] Slider _Slider;
    [SerializeField] Slider _Slider2;

    float _Target;
    bool _Started;

    // Start is called before the first frame update
    void Awake() {
        _Mario.Emulator.OnRunning += () => {
            Debug.Log("STARETD");
            Initialize();

        };
    }

    // Update is called once per frame
    void Update() {
        if (!_Started) return;

        var pos = _Mario.Curr.posY;
        var vel = _Mario.Curr.velY / _RangeY.Length;
        _Target = _RangeY.InverseLerp(pos);
        _Ease.Update(Time.deltaTime, Vector3.up * _Target);

        var currPos = _Ease.Value.y;
        var currVel = _Ease.Velocity.y;
        _Source.pitch = currVel * Length;
        Debug.Log($"vel {currVel}");

        _Slider.value = _Source.time / Length;
        _Slider2.value = currPos;

        if (pos < _RangeY.Min) {
            _Mario.Emulator.ReloadState();
            _Source.Stop();
            Initialize();
        }
    }

    void Initialize() {
        _Started = false;
        this.DoAfterTime(_Delay, () => {
            _Source.Stop();
            var pos = _Mario.Curr.posY;
            var relPosY = _RangeY.InverseLerp(_Mario.Curr.posY);
            _Target = relPosY;
            _Source.time = ((_Target +_Source.clip.length * 100) * (_Scale < 0 ? 1-_Scale : _Scale)) % _Source.clip.length;
            _Source.Play();
            // TODO: is this working?
            // _Mario.Emulator.SetVolume(0);
            _Ease.Init(Vector3.up * relPosY);
            _Started = true;
        });
    }

    float Length => _Source.clip.length * _Scale;
}