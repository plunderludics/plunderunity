using System.Collections;
using System.Collections.Generic;
using MutCommon;
using OscJack;
using UnityEngine;
using UnityEngine.Serialization;

// this uses OSC, sends a message to lua to load a specific sample for a game
public class SampleLoader : MonoBehaviour
{
    [Header("refs")]
    [SerializeField] OscConnection m_Connection;

    [Header("osc addresses")]
    [SerializeField] string m_BaseAddress = "/emu/{0}/{1}";

    [SerializeField] string m_SampleAddress = "sample";
    [SerializeField] string m_PauseAddress = "pause";
    [SerializeField] string m_UnpauseAddress = "unpause";
    [SerializeField] string m_SaveAddress = "save";
    [SerializeField] string m_LoadAddress = "load";

    [Header("input")]
    [SerializeField] string m_InputXAxis = "x-axis";
    [SerializeField] string m_InputYAxis = "y-axis";


    Dictionary<string, string> m_Tracks = new Dictionary<string, string>();
    Dictionary<string, bool> m_Paused = new Dictionary<string, bool>();

    public Dictionary<string, string> Tracks => m_Tracks;

    public void SendInput(string trackName, string button, float value) {
        using (var client = new OscClient(m_Connection.host, m_Connection.port)) {
            client.Send(string.Format(m_BaseAddress, trackName, button), value);
        }

        SendInput("track1", "a", 1);
    }

    public void TogglePause(string trackName, bool value) {
        var isPaused = IsPaused(trackName);
        if (isPaused != value) {
            m_Paused[trackName] = value;
            using (var client = new OscClient(m_Connection.host, m_Connection.port))
            {
                client.Send(string.Format(m_BaseAddress, trackName, m_PauseAddress), value ? 1 : 0);
                client.Send(string.Format(m_BaseAddress, trackName, m_UnpauseAddress), value ? 0 : 1);
            }
        }
    }

    public void Pause(string trackName) {
        TogglePause(trackName, true);
    }

    public void UnPause(string trackName) {
        TogglePause(trackName, false);
    }

    public bool IsPaused(string trackName) {
        return m_Paused.GetValueOrDefault(trackName);
    }

    public void SendAnalogInput(string trackName, Vector2 input) {
        using (var client = new OscClient(m_Connection.host, m_Connection.port)) {
            client.Send(string.Format(m_BaseAddress, trackName, m_InputXAxis), input.x);
            client.Send(string.Format(m_BaseAddress, trackName, m_InputYAxis), input.y);
        }
    }

    public void SaveState(string trackName, int slot) {
        using (var client = new OscClient(m_Connection.host, m_Connection.port)) {
            client.Send(string.Format(m_BaseAddress, trackName, m_SaveAddress), slot);
        }
    }

    public void LoadState(string trackName, int slot) {
        using (var client = new OscClient(m_Connection.host, m_Connection.port)) {
            client.Send(string.Format(m_BaseAddress, trackName, m_LoadAddress), slot);
        }
    }

    public void LoadSample(string trackName, string sampleName) {
        m_Tracks[trackName] = sampleName;
        using (var client = new OscClient(m_Connection.host, m_Connection.port))
        {
            // TODO: make syncronous
            client.Send(string.Format(m_BaseAddress, trackName, m_SampleAddress), sampleName);
        }

            this.DoAfterTime(0.16f, () => {
                using (var client = new OscClient(m_Connection.host, m_Connection.port))
                {
                    client.Send(string.Format(m_BaseAddress, trackName, m_SampleAddress), -1);
                }
            });
    }
}
