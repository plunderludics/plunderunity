using System;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using UnityHawk;

public class MarioState : MonoBehaviour
{
    [SerializeField] Emulator _emulator;

	Soil.Ring<Data> _Queue = new(10);
	List<Data> _SendBuffer = new();

    public Data Curr {
	    get => _Queue[0];
    }

    public Data Prev {
	    get => _Queue[1];
    }

    #if UNITY_EDITOR
    [SerializeField] Data CurrData;
    #endif


    // lifecycle
    void OnValidate() {

        if (!_emulator) {
            _emulator = GetComponentInParent<Emulator>();
        }
        if (!_emulator) {
            _emulator = GetComponentInChildren<Emulator>();
        }
    }

    void Start() {
	    _emulator.RegisterMethod("GetState", GetState);
	    // TODO:
	    // _emulator.RegisterMethod("SetState", SetState);
	    _emulator.RegisterMethod("SetHealth", SetHealth);
	    _Queue.Fill(new Data());
    }

    public Emulator Emulator {
	    get => _emulator;
    }
    string GetState(string json) {
	    JsonSerializer serializer = new JsonSerializer();
	    var sr = new StringReader(json);
	    var jr = new JsonTextReader(sr);
	    var next = serializer.Deserialize<Data>(jr);
	    _Queue.Add(next);
		#if UNITY_EDITOR
		CurrData = next;
		#endif

	    return "";
    }

    string SetHealth(string json) {
		if(_SendBuffer.Count == 0) {
			return "";
		}

		// TODO: figure out why setting everything messes this up
		// maybe be able to send just a patch
	    // JsonSerializer serializer = new jsonserializer();
	    // var s = new stringbuilder();
	    // var sr = new stringwriter(s);
	    // var jw = new jsontextwriter(sr);
	    // serializer.serialize(jw, _buffer[0]);
	    // _buffer.removeat(0);
	    // debug.log($"serialized: {s}");
	    // return sr.ToString();
	    return Curr.health.ToString();
    }

    public void SetState() {
		_SendBuffer.Add(Curr);
    }

    [Serializable]
    public class Data {
		public int coins;
		public int stars;

		public int level;
		public int status;
		public int action;
		public int health;

		// animation
		public int phase;
		public int cycle;

		public float posX;
		public float posY;
		public float posZ;

		public float offY;
		public float vel;

		public float velX;
		public float velY;
		public float velZ;

		public float camX;
		public float camY;
		public float camZ;

		public enum Action {


		}

    }
}