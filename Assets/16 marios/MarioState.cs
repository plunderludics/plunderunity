using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityHawk;

public class MarioState : MonoBehaviour
{
    [SerializeField] Emulator _emulator;

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
    }

    public Data D;

    void Start() {
	    _emulator.RegisterMethod("GetState", GetState);
    }

    string GetState(string json) {
	    JsonSerializer serializer = new JsonSerializer();
	    var sr = new StringReader(json);
	    var jr = new JsonTextReader(sr);
	    D = serializer.Deserialize<Data>(jr);

	    return "";
    }
}