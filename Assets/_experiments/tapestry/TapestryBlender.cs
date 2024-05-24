using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GK;
using NaughtyAttributes;
using Soil;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Plunderludics.Lib;

namespace Tapestry
{
    
public class TapestryBlender : MonoBehaviour
{
    [Serializable]
    public record WalkerMix {
        public Track Track;
        public TapestryEmitter Emitter;
        public float Value;
    }

    [Header("config")]
    
    [Tooltip("if the sample should reload on pause")]
    [SerializeField] bool m_ReloadSampleOnUnpause = false;
    
    [Tooltip("if the sample should be saved when unloaded from a track")]
    [SerializeField] bool m_SaveSampleOnUnload = false;

    [Header("tuning")]
    [SerializeField] FloatReference m_WrapLength;
    [SerializeField] MapOutCurve m_AlphaCurve;
    [SerializeField] MapOutCurve m_VolumeCurve;

    [Header("refs")]
    [FormerlySerializedAs("m_TextureMixer")]
    [Header("the track mixer")]
    [SerializeField] TrackMixer m_TrackMixer;
    
    [Header("atom with number of loaded tracks")]
    [SerializeField] IntReference m_LoadedTracks;

    [Header("debug")]
    [SerializeField] float m_GizmoRadius;
    [SerializeField] TMPro.TMP_Text m_Debug;

    public SerializedDictionary<Guid, WalkerMix> m_Mix;

    // a game, emitting videogame, sorted by distance
    [ShowNonSerializedField, Readonly] List<TapestryEmitter> m_Emitters;

    Dictionary<string, string> m_SavedGames;

    // the raw vertices emitting videogame
    List<TapestryEmitter> m_Verts;

    // a representation of the triangulation of the emitter map
    DelaunayTriangulation m_Triangulation;

    int TrackCount => Tracks.Count();
    
    // number of expected mixing channels
    const int k_Channels = 3;

    public float WrapLength => m_WrapLength.Value;
    
    public IEnumerable<Track> AllTracks => m_Mix.Values.Select(m => m.Track);
    
    // the tracks being mixed
    Track[] Tracks => m_TrackMixer.Tracks;

    
    // the list of currently available tracks
    List<Track> m_AvailableTracks;
    
    // if the loading is done
    bool m_IsLoaded;

    // Start is called before the first frame update
    void Awake() {
        m_Mix = new ();
    }

    void Start() {
        m_AvailableTracks = new List<Track>(Tracks);
        
        m_Emitters = FindObjectsOfType<TapestryEmitter>().ToList();
        m_Verts = FindObjectsOfType<TapestryEmitter>().ToList();
        var calculator = new DelaunayCalculator();
        m_Triangulation = calculator.CalculateTriangulation(
            m_Verts.Select(
                e => {
                    Vector3 position;
                    return new Vector2(
                        (position = e.transform.position).x,
                        position.z
                    );
                }
                )
            .ToList());


        // initializeTracks
        StartCoroutine(LoadTracksSync());
    }

    // TODO: don't let emulators load the same file at the same time
    IEnumerator LoadTracksSync() {
        m_Emitters = m_Emitters.OrderBy(Dist).ToList();
        for (var i = 0; i < TrackCount; i++) {
            var track = Tracks[i];

            var sample = m_Emitters.ElementAt(i).Sample;
            print($"initializing track {track.name} with sample {sample}");

            // TODO: make this load sample from name?
            // TODO: make this use streaming assets
            // create sample object?
            // emu.SetFromSample($"Assets/StreamingAssets/samples/{sample}/rompath.txt");
            track.gameObject.SetActive(true);
            track.LoadSample(sample);
        }

        while (m_LoadedTracks.Value < TrackCount)
        {
            m_LoadedTracks.Value = Tracks.Count(t => t.IsRunning);
            yield return null;
        }
        
        foreach (var track in Tracks) {
            track.SetVolume(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // wait for all tracks to load
        foreach (var track in Tracks) {
            if (!track.IsRunning) {
                return;
            }
        }

        // find which triangle we are inside in the triangulated emitter mesh
        var tris = m_Triangulation.Triangles;
        var verts = m_Triangulation.Vertices;
        var l0 = 0.0f;
        var l1 = 0.0f;
        var l2 = 0.0f;
        var pos = transform.position;
        var p = new Vector2(pos.x, pos.z);
        var v0 = tris[0];
        var v1 = tris[1];
        var v2 = tris[2];
        for (int i = 0; i < tris.Count; i+=3) {
            v0 = tris[i];
            v1 = tris[i+1];
            v2 = tris[i+2];
            var c0 = verts[v0];
            var c1 = verts[v1];
            var c2 = verts[v2];

            // https://en.wikipedia.org/wiki/Barycentric_coordinate_system
            // thanks @pancelor for helping with the math
            var detT = (c1.y - c2.y) * (c0.x - c2.x) + (c2.x - c1.x) * (c0.y - c2.y);

            l0 = ((c1.y - c2.y) * (p.x - c2.x) + (c2.x - c1.x) * (p.y - c2.y)) / detT;
            l1 = ((c2.y - c0.y) * (p.x - c2.x) + (c0.x - c2.x) * (p.y - c2.y)) / detT;
            l2 = 1 - l0 - l1;

            if(l0 < 0 || l0 > 1) continue;
            if(l1 < 0 || l1 > 1) continue;
            if(l2 < 0 || l2 > 1) continue;
            break;
        }

        // TODO: optimize
        // distance is a decent parameter here, but the three closest points aren't necessarily
        // the ones in the current triangle we are inside
        var others = m_Emitters
            .Where(e => e.Id != m_Verts.ElementAt(v0).Id)
            .Where(e => e.Id != m_Verts.ElementAt(v1).Id)
            .Where(e => e.Id != m_Verts.ElementAt(v2).Id)
            .OrderBy(Dist)
            .GroupBy(e => e.Id) // remove duplicates
            .Select(g => g.First()) 
            .ToArray();
        
        // free tracks
        var emittersToRemove = new List<TapestryEmitter>();
        foreach(var key in m_Mix.Keys)
        {
            // any of the emitters beyond of the maximum tracks (minus the mixed ones), should be removed
            var toRemove = others.Skip(TrackCount-k_Channels).FirstOrDefault(e => e.Id == key);
            if (toRemove == null) continue;
            // if (m_Emitters.Take(TrackCount).Any(e => e.Id == key)) continue;
            
            var removed = m_Mix[toRemove.Id];
            m_AvailableTracks.Add(removed.Track);
            print($"removed mix {toRemove} track-{removed.Track} : {removed.Value}");
            emittersToRemove.Add(toRemove);
        }

        foreach(var emitter in emittersToRemove) {
            // unload tracks
            if (m_Mix.TryGetValue(emitter.Id, out var mix))
            {
                print($"tapestry: unloading mix {emitter.Sample} with value {mix.Value}");
                if (m_SaveSampleOnUnload)
                {
                    // TODO: reuse samples
                    var sampleName = $"{emitter.name}-savedSamples";
                    emitter.Save(sampleName);
                    mix.Track.SaveSample(sampleName);
                }
            }
            
            m_Mix.Remove(emitter.Id);
        }

        // update all tracks
        for (var i = 0; i < TrackCount; i++) {
            var emitter = i switch {
                0 => m_Verts.ElementAt(v0),
                1 => m_Verts.ElementAt(v1),
                2 => m_Verts.ElementAt(v2),
                _ => others.ElementAt(i-3)
            };

            var value = i switch  {
                0 => l0,
                1 => l1,
                2 => l2,
                _ => 0
            };

            // if the sample is already in the mixer return
            var id = emitter.Id;
            if (m_Mix.TryGetValue(id, out var mix)) {
                mix = m_Mix[id];
                mix.Value = value;
                mix.Emitter = emitter;
                MixTrack(mix);
                continue;
            }

            // TODO: event here for newly added key
            if (!m_AvailableTracks.Any()) {
                Debug.Log($"[walker] {emitter.Sample} attempting to request track with no more available tracks");
                continue;
            }

            // pop next available track
            var track = m_AvailableTracks.First();
            m_AvailableTracks.RemoveAt(0);
            
            mix = new WalkerMix() { Track=track, Value=value, Emitter = emitter};
            m_Mix[id] = mix;
            
            print($"added {emitter.Sample} @ track: {track}");

            MixTrack(mix);
        }

        // already clean mix
        foreach(var key in m_Mix.Keys) {
            var mix = m_Mix[key];
            m_TrackMixer.SetTrackMix(mix.Track, m_AlphaCurve.Evaluate(mix.Value));
        }

        m_Debug.text = string.Concat(Tracks.Select((track) => {
            var sample = track.Sample;

            var mixInfo = "disabled";
            var mix = m_Mix.FirstOrDefault(k => k.Value.Emitter.Sample == sample).Value;
            if(mix != null) {
                mixInfo = $"{mix.Value}";
            }

            return $"track {track} | {sample} : {mixInfo}\n";
        }));
    }

    void MixTrack(WalkerMix mix) {
        var track = mix.Track;
        var emitter = mix.Emitter;

        track.SetVolume(m_VolumeCurve.Evaluate(mix.Value));
        if (track.Sample != emitter.Sample) {
            track.LoadSample(emitter.Sample);
        }

        if (mix.Value > 0) {
            if(track.IsPaused) {
                track.IsPaused = false;
                if (m_ReloadSampleOnUnpause) {
                    track.LoadSample(emitter.Sample);
                }
            }
        } else {
            if(!track.IsPaused) {
                track.IsPaused = true;
            }
        }
    }

    float Dist(TapestryEmitter other) =>
        (Wrap(transform.position) - other.transform.position).sqrMagnitude;

    Vector3 Wrap(Vector3 pos) => Wrapper.Wrap(pos, m_WrapLength.Value);

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        var dim = Vector3.one * m_WrapLength.Value;
        Gizmos.DrawWireCube(Vector3.zero + 0.5f*dim, dim);

        if(m_Emitters == null || !m_Emitters.Any()) return;

        // draw all the triangles
        var verts = m_Triangulation.Vertices;
        var tris = m_Triangulation.Triangles;
        Gizmos.color = Color.white;
        for (int i = 0; i < tris.Count; i += 3) {
            var v0 = tris[i];
            var v1 = tris[i + 1];
            var v2 = tris[i + 2];
            var c0 = verts[v0];
            var c1 = verts[v1];
            var c2 = verts[v2];
            Vector3 pos;
            var p0 = new Vector3(c0.x, 0, c0.y);
            var p1 = new Vector3(c1.x, 0, c1.y);
            var p2 = new Vector3(c2.x, 0, c2.y);
            
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p0);
        }

        // this is bad
        foreach (var mix in m_Mix.Values)
        {
            Gizmos.color = Color.blue;
            var size = 0f;
            if (mix.Value > 0) {
                Gizmos.color = Color.magenta;
                size = mix.Value;
            }

            Gizmos.DrawLine(transform.position, mix.Emitter.transform.position);
            Gizmos.DrawSphere(mix.Emitter.transform.position, size * m_GizmoRadius);
        }
    }

    string TrackName(string track) => $"track{track}";
    string TrackName(int track) => $"track{track}";
}

}
