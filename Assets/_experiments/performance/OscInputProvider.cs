using System.Collections.Generic;
using extOSC;
using Plunderludics.Lib;
using UnityEngine;
using UnityHawk;

namespace MutPlunders {
    public class OscInputProvider: InputProvider {
        [SerializeField] int m_Track;
        [SerializeField] int m_Controllers;
        [SerializeField] OSCReceiver m_Receiver;

        const string k_Channel = "/i/{0}";
        readonly Dictionary<string, InputEventMapping> m_InputMap = new() {
            // n64
            {"a", new ("A")},
            {"b", new ("B")},
            {"z", new ("Z")},
            {"a-x", new ("X Axis", true)},
            {"a-y", new ("Y Axis", true)},
            {"d-r", new("DPad R")},
            {"d-l", new("DPad L")},
            {"d-d", new("DPad D")},
            {"d-u", new("DPad U")},
            {"start", new("Start")},
            {"c-r", new("C Right")},
            {"c-l", new("C Left")},
            {"c-d", new("C Down")},
            {"c-u", new("C Up")},
            {"r", new("R")},
            {"l", new("L")},
        };

        void Awake() {
            Debug.Log($"doing track {m_Track}");
            for (var i = 0; i < m_Controllers; i++) {
                Debug.Log($"doing controller {i}");
                foreach (var (input, map) in m_InputMap) {
                    var channel = string.Format(OscTracks.Channel, m_Track, "i") + string.Format(k_Channel, input);
                    Debug.Log($"doing channel {channel}");
                    m_Receiver.Bind(channel, (msg) => InputReceiver(map, msg));
                }
            }
        }

        void InputReceiver(InputEventMapping m, OSCMessage msg) {
            int value = -1;

            if(!msg.ToFloat(out float valueF)) {
                return;
            }

            if(m.IsAnalog) {
                valueF /= 5f;
                valueF *= 127;
                if (valueF < 0) {
                    valueF = 255 + valueF;
                }
                value = Mathf.FloorToInt(valueF);
            } else {
                value = Mathf.Abs(valueF) > 0.1 ? 1 : 0;
            }

            if (value < 0) {
                return;
            }

            Debug.Log($"msg {msg} => {value}");
            AddInputEvent(new InputEvent(m.Name, value, m.Controller, m.IsAnalog));
        }

        struct InputEventMapping {
            public string Name;
            public Controller Controller;
            public bool IsAnalog;

            public InputEventMapping(string name, bool isAnalog = false, Controller controller = Controller.P1) {
                Name = name;
                Controller = controller;
                IsAnalog = isAnalog;
            }
        }
    }
}
