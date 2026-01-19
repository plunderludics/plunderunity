using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityHawk;

public partial class MarioState: MonoBehaviour {
	class MemoryFloat: Memory<float> {
		public MemoryFloat(long address, bool isBigEndian = true, string domain = "RDRAM") : base(address, 4, WatchType.Float, isBigEndian, domain) { }

		public override void Watch(Emulator emulator, Action<float> callback) {
			emulator.WatchFloat(Address, IsBigEndian, Domain, callback);
		}
	}

	class MemoryUnsigned: Memory<uint> {
		public MemoryUnsigned(long address, int size, bool isBigEndian = true, string domain = "RDRAM") : base(address, size, WatchType.Unsigned, isBigEndian, domain) { }

		public override void Watch(Emulator emulator, Action<uint> callback) {
			emulator.WatchUnsigned(Address, Size, IsBigEndian, Domain, callback);
		}
	}

	class MemorySigned: Memory<int> {
		public MemorySigned(long address, int size, bool isBigEndian = true, string domain = "RDRAM") : base(address, size, WatchType.Signed, isBigEndian, domain) { }

		public override void Watch(Emulator emulator, Action<int> callback) {
			emulator.WatchSigned(Address, Size, IsBigEndian, Domain, callback);
		}
	}

	abstract class Memory<T> {
		public readonly long Address;
		public readonly int Size;
		public readonly WatchType Type;
		public readonly bool IsBigEndian;
		public readonly string Domain;

		Memory() {

		}

		protected Memory(long address,
			int size,
			WatchType type,
			bool isBigEndian = true,
			string domain = "RDRAM") {
			Address = address;
			Size = size;
			Type = type;
			IsBigEndian = isBigEndian;
			Domain = domain;
		}

		public abstract void Watch(Emulator emulator, Action<T> callback);
	}

	// Y is the up axis
	MemoryUnsigned coins    = new(address: 0x33B219, size: 2);
	MemoryUnsigned stars    = new(address: 0x33B21A, size: 2); // writing doesn't do anything
	MemoryUnsigned level    = new(address: 0x33B249, size: 1); // writing doesn't do anything
	MemoryUnsigned status   = new(address: 0x33B172, size: 2);
	MemoryUnsigned health   = new(address: 0x33B21E, size: 1);
	MemoryUnsigned sfx      = new(address: 0x00FF, size: 1);
	MemoryFloat    posX     = new(address: 0x33B1AC);
	MemoryFloat    posY     = new(address: 0x33B1B0);
	MemoryFloat    posZ     = new(address: 0x33B1B4);
	MemoryFloat    offY     = new(address: 0x33B220);
	MemoryFloat    speed      = new(address: 0x33B1C4);
	MemoryFloat    velX     = new(address: 0x33B1B8);
	MemoryFloat    velY     = new(address: 0x33B1BC);
	MemoryFloat    velZ     = new(address: 0x33B1C0);
	MemoryFloat    camX     = new(address: 0x33C6A4);
	MemoryFloat    camY     = new(address: 0x33C6A8);
	MemoryFloat    camZ     = new(address: 0x33C6AC);
	MemoryUnsigned phase    = new(address: 0x33B17C, size: 4); // writing doesn't do anything
	MemoryUnsigned cycle    = new(address: 0x33B18A, size: 2); // writing doesn't do anything

	// TODO: is in cannon

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
	    // _emulator.RegisterLuaCallback("GetState", GetState);

	    // TODO:
	    // _emulator.RegisterLuaCallback("SetState", SetState);
	    _emulator.RegisterLuaCallback("SetHealth", SetHealth);
	    _Queue.Fill(new Data());
	    _emulator.OnRunning += () => {
		    coins.Watch(_emulator, v => Curr.coins = v);
		    stars.Watch(_emulator, v => Curr.stars = v);
		    level.Watch(_emulator, v => Curr.level = v);
		    status.Watch(_emulator, v => Curr.status = v);
		    health.Watch(_emulator, v => Curr.health = v);
		    sfx.Watch(_emulator, v => Curr.sfx = v);
		    posX.Watch(_emulator, v => Curr.pos.x = v);
		    posY.Watch(_emulator, v => Curr.pos.y = v);
		    posZ.Watch(_emulator, v => Curr.pos.z = v);
		    offY.Watch(_emulator, v => Curr.offY = v);
		    speed.Watch(_emulator, v => Curr.speed = v);
		    velX.Watch(_emulator, v => Curr.vel.x = v);
		    velY.Watch(_emulator, v => Curr.vel.y = v);
		    velZ.Watch(_emulator, v => Curr.vel.z = v);
		    camX.Watch(_emulator, v => Curr.cam.x = v);
		    camY.Watch(_emulator, v => Curr.cam.y = v);
		    camZ.Watch(_emulator, v => Curr.cam.z = v);
		    phase.Watch(_emulator, v => Curr.phase = v);
		    cycle.Watch(_emulator, v => Curr.cycle = v);
	    };

		#if UNITY_EDITOR
		CurrData = Curr;
		#endif
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

    // TODO: tell lua to send only what is needed, maybe lua sends separate methods for each prop?
    [Serializable]
    public class Data {
		public uint coins;
		public uint stars;

		public uint level;
		public uint status;
		public uint health;

		public uint sfx;

		// animation
		public uint phase;
		public uint cycle;

		public Vector3 pos;
		public float posX => pos.x;
		public float posY => pos.y;
		public float posZ => pos.z;

		public float offY;
		public float speed;

		public Vector3 vel;
		public float velX => vel.x;
		public float velY => vel.y;
		public float velZ => vel.z;

		public Vector3 cam;
		public float camX => cam.x;
		public float camY => cam.y;
		public float camZ => cam.z;
    }
}