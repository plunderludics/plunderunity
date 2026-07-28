using System.Collections.Generic;
using System.IO;
using System.Linq;
using extOSC;
using Plunderludics.Lib;
using UnityEngine;
using UnityHawk;

namespace MutPlunders {
    class OscTracks: MonoBehaviour {
        [SerializeField] OSCReceiver m_Receiver;
        [SerializeField] TrackMixer m_Mixer;

        Dictionary<string, string> m_Roms;
        Dictionary<string, string> m_Samples;

        public const string Channel = "/t/{0}/{1}";

        void Awake() {
            for (int i = 0; i < 8; i++) {
                var i1 = i;
                m_Receiver.Bind(string.Format(Channel, i, "pos"), (msg) => PositionReceiver(i1, msg));
                m_Receiver.Bind(string.Format(Channel, i, "sta"), (msg) => LoadStateReceived(i1, msg));
                m_Receiver.Bind(string.Format(Channel, i, "rom"), (msg) => LoadRomReceived(i1, msg));
                m_Receiver.Bind(string.Format(Channel, i, "pau"), (msg) => PauseReceived(i1, msg));
            }

            m_Samples = Directory
                .EnumerateFiles(Path.Combine(Paths.BizHawkAssetsDir, "samples"), "*.savestate", SearchOption.AllDirectories)
                .Select(
                    f => new {
                        name = Path.GetFileNameWithoutExtension(f),
                        path = f,
                    }
                )
                .GroupBy(f => f.name)
                .Select(g => g.First())
                .ToDictionary(s => s.name, s => s.path);

            // TODO: create rom scriptable objects from folder
            m_Roms = Directory
                .EnumerateFiles(Path.Combine(Paths.BizHawkAssetsDir, "roms"), "*.*", SearchOption.AllDirectories)
                .Select(
                    f => new {
                        name = Path.GetFileNameWithoutExtension(f),
                        ext = Path.GetExtension(f),
                        path = f,
                    }
                )
                .GroupBy(f => f.name)
                .Select(g => g.First(f => f.ext != "bin"))
                .ToDictionary(s => s.name, s => s.path);
        }

        void PauseReceived(int i, OSCMessage msg) {
            var track = m_Mixer.Tracks[i];
            if (msg.ToBool(out var pause)) {
                track.IsPaused = pause;
            }
        }

        void LoadStateReceived(int id, OSCMessage msg) {
            Debug.Log($"received load state message on track {id} with msg {msg}");
            var track = m_Mixer.Tracks[id];
            if (msg.ToString(out var stateName)) {
                if (m_Samples.TryGetValue(stateName, out var statePath)) {
                    track.LoadState(statePath);
                }
            }
        }

        void LoadRomReceived(int id, OSCMessage msg) {
            Debug.Log($"received load rom message on track {id} with msg {msg}");
            var track = m_Mixer.Tracks[id];
            if (msg.ToString(out var romName)) {
                if (m_Roms.TryGetValue(romName, out var romPath)) {
                    track.LoadRom(romPath);
                }
            }
        }

        void PositionReceiver(int track, OSCMessage msg) {
            if (msg.ToVector2(out var pos)) {
            }
        }
    }

}
