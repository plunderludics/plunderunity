using System.Collections.Generic;
using extOSC;
using Plunderludics.Lib;
using UnityEngine;
using UnityHawk;

namespace MutPlunders {
    public class OscInputProvider: BasicInputProvider {
        [Header("osc")]
        [SerializeField] int m_Track;
        [SerializeField] int m_Controllers;
        [SerializeField] OSCReceiver m_Receiver;
        [SerializeField] bool m_BasicInputEnabled;
        [SerializeField] bool m_Log;

        const string k_Channel = "/{0}";
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

        protected void Awake() {
            Debug.Log($"doing track {m_Track}");
            for (var i = 0; i < m_Controllers; i++) {
                Debug.Log($"doing controller {i}");
                foreach (var (input, map) in m_InputMap) {
                    var channel = Channel(input);
                    m_Receiver.Bind(channel, (msg) => InputReceiver(map, msg));
                }

                var enableChannel = Channel("toggle");
                m_Receiver.Bind(enableChannel, EnableReceiver);
            }

            string Channel(string input) {
                var channel = string.Format(OscTracks.Channel, m_Track, "i") + string.Format(k_Channel, input);
                return channel;
            }
        }


        protected override void Update() {
            if (m_BasicInputEnabled) {
                base.Update();
            }
        }

        protected override void FixedUpdate() {
            if (m_BasicInputEnabled) {
                base.FixedUpdate();
            }
        }

        // -- events --
        void EnableReceiver(OSCMessage msg) {
            if(m_Log) {
                Debug.Log($"received {msg}");
            }

            int value = -1;

            if(!msg.ToFloat(out float valueF)) {
                return;
            }

            m_BasicInputEnabled = valueF > 0.5;

        }

        void InputReceiver(InputEventMapping m, OSCMessage msg) {
            if(m_Log) {
                Debug.Log($"received {msg}");
            }

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
                value = valueF > 0 ? 1 : 0;
            }

            if (value < 0) {
                return;
            }

            if(m_Log) {
                Debug.Log($"msg {msg} => {value}");
            }

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