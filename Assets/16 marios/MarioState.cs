using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityHawk;

public class MarioState : MonoBehaviour
{
    [SerializeField] Emulator _emulator;

    [Serializable]
    public struct Data {
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
    }

    public Data D;

	// todo Soil.buffer?
	public List<Data> _buffer = new();

    void OnValidate() {
		_emulator = GetComponent<Emulator>();
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
    }

    string GetState(string json) {
	    JsonSerializer serializer = new JsonSerializer();
	    var sr = new StringReader(json);
	    var jr = new JsonTextReader(sr);
	    D = serializer.Deserialize<Data>(jr);

	    return "";
    }

    string SetHealth(string json) {
		if(_buffer.Count == 0) {
			return "";
		}

	    // JsonSerializer serializer = new jsonserializer();
	    // var s = new stringbuilder();
	    // var sr = new stringwriter(s);
	    // var jw = new jsontextwriter(sr);
	    // serializer.serialize(jw, _buffer[0]);
	    // _buffer.removeat(0);
	    // debug.log($"serialized: {s}");
	    // return sr.ToString();
	    return D.health.ToString();
    }

    public void SetState() {
		_buffer.Add(D);
    }
}