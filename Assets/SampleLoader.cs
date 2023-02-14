using System.Collections;
using System.Collections.Generic;
using OscJack;
using UnityEngine;

// this uses OSC, sends a message to lua to load a specific sample for a game
public class SampleLoader : MonoBehaviour
{
    public string m_OscAddress = "/{0}/sample";
    public OscConnection m_Connection;

    public void LoadSample(string trackName, string sampleName) {
        // IP address, port number
        using (var client = new OscClient(m_Connection.host, m_Connection.port))
        {
            // Send two-component float values ten times.
            client.Send(string.Format(m_OscAddress, trackName), sampleName); // Second element
        }
    }
}
