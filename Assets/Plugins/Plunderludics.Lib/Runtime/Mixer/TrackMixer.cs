using System.Collections.Generic;
using System.Linq;

namespace Plunderludics.Lib
{
    
public class TrackMixer : MixTextures
{
    Track[] m_Tracks;

    Dictionary<Track, int> m_TrackMap;

    public Track[] Tracks => m_Tracks;

    void OnValidate() { }

    private void Awake() {
        m_Tracks = GetComponentsInChildren<Track>();
        m_TrackMap = m_Tracks
            .Select((t, i) => new { t, i })
            .ToDictionary(t => t.t, t => t.i);
    }

    public void SetTrackMix(Track track, float value) => SetTrackMix(m_TrackMap[track], value);
}

}