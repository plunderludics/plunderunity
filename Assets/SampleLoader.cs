using System.Collections;
using System.Collections.Generic;
using OscJack;
using UnityEngine;
using UnityEngine.Serialization;

// this uses OSC, sends a message to lua to load a specific sample for a game
public class SampleLoader : MonoBehaviour
{
    [Header("osc addresses")]
    [SerializeField] string m_SampleAddress = "sample";
    [SerializeField] string m_PauseAddress = "pause";
    [SerializeField] string m_UnpauseAddress = "unpause";
    [SerializeField] string m_BaseAddress = "/emu/{0}/{1}";
    [SerializeField] OscConnection m_Connection;

    Dictionary<string, string> m_Tracks = new Dictionary<string, string>();
    Dictionary<string, bool> m_Paused = new Dictionary<string, bool>();

    public Dictionary<string, string> Tracks => m_Tracks;

    public void TogglePause(string trackName, bool value) {
        var isPaused = m_Paused.GetValueOrDefault(trackName);
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

    public void LoadSample(string trackName, string sampleName) {
        m_Tracks[trackName] = sampleName;
        using (var client = new OscClient(m_Connection.host, m_Connection.port))
        {
            client.Send(string.Format(m_BaseAddress, trackName, m_SampleAddress), sampleName);
        }
    }
}
