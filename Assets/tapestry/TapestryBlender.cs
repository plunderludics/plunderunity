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
using UnityHawk;

public class TapestryBlender : MonoBehaviour
{
    [Serializable]
    record WalkerMix {
        public int Track;
        public float Value;
    }

    [Header("config")]
    [SerializeField] bool m_ReloadSampleOnUnpause = false;

    [Header("tuning")]
    [SerializeField] FloatReference m_WrapLength;
    [SerializeField] MapOutCurve m_AlphaCurve;
    [SerializeField] MapOutCurve m_VolumeCurve;

    [Header("mixing")]
    [Header("the tracks being mixed")]
    [SerializeField] Track[] m_Tracks;

    [Header("number of expected mixing channels")]
    [SerializeField] int m_Channels = 3;

    [Header("refs")]
    [SerializeField] MixTextures2 m_TextureMixer;
    [SerializeField] IntReference m_LoadedTracks;

    [Header("debug")]
    [SerializeField] TMPro.TMP_Text m_Debug;

    [ShowNonSerializedField, Readonly] SerializedDictionary<string, WalkerMix> m_Mix;

    // a game, emitting videogame, sorted by distance
    [ShowNonSerializedField, Readonly] List<TapestryEmitter> m_Emitters;

    // the raw vertices emitting videogame
    List<TapestryEmitter> m_Verts;

    DelaunayTriangulation m_Triangulation;
    List<int> m_AvailableTracks;

    int TrackCount => m_Tracks.Count();
    public float WrapLength => m_WrapLength.Value;
    public IEnumerable<string> AllTracks => m_Mix.Values.Select(m => TrackName(m.Track));

    // Start is called before the first frame update
    void Awake()
    {
        m_Mix = new ();
        m_AvailableTracks = Enumerable.Range(0, TrackCount).ToList();
    }

    void Start() {
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

    // HACK: to prevent:  System.IO.IOException: The process cannot access the file because it is being used by another process.
    IEnumerator LoadTracksSync() {
        m_Emitters = m_Emitters.OrderBy(Dist).ToList();
        for (var i = 0; i < TrackCount; i++) {
            var track = m_Tracks[i];

            track.Volume = 0;
            var emu = track.GetComponent<Emulator>();
            var sample = m_Emitters.ElementAt(i).Sample;
            print($"initializing track {track.name} with sample {sample}");

            // TODO: make this load sample from name?
            // TODO: make this use streaming assets
            // create sample object?
            // emu.SetFromSample($"Assets/StreamingAssets/samples/{sample}/rompath.txt");
            track.gameObject.SetActive(true);
            track.Sample = sample;
            track.LoadSample(sample);

            while (!track.IsLoaded) {
                yield return null;
            }

            m_LoadedTracks.Value += 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // wait for all tracks to load
        foreach (var track in m_Tracks) {
            if (!track.IsLoaded) {
                return;
            }
        }

        // TODO: optimize
        m_Emitters = m_Emitters.OrderBy(Dist).ToList();

        // free tracks
        var keysToRemove = new List<string>();
        foreach(var key in m_Mix.Keys) {
            // any of the things to mix outside of the maximum tracks, should be removed
            if (m_Emitters.Take(TrackCount).All(e => e.Sample != key)) {
                var removed = m_Mix[key];
                m_AvailableTracks.Add(removed.Track);
                print($"removed mix {key} track-{removed.Track} : {removed.Value}");
                keysToRemove.Add(key);
            }
        }

        foreach(var key in keysToRemove) {
            m_Mix.Remove(key);
        }

        // triangulate the emitter mesh and find which triangle we are inside of
        var tris = m_Triangulation.Triangles;
        var verts = m_Triangulation.Vertices;
        var l0 = 0.0f;
        var l1 = 0.0f;
        var l2 = 0.0f;
        var pos = transform.position;
        var P = new Vector2(pos.x, pos.z);
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

            l0 = ((c1.y - c2.y) * (P.x - c2.x) + (c2.x - c1.x) * (P.y - c2.y)) / detT;
            l1 = ((c2.y - c0.y) * (P.x - c2.x) + (c0.x - c2.x) * (P.y - c2.y)) / detT;
            l2 = 1 - l0 - l1;

            if(l0 < 0 || l0 > 1) continue;
            if(l1 < 0 || l1 > 1) continue;
            if(l2 < 0 || l2 > 1) continue;
            break;
        }

        var others = m_Emitters
            .Where(e => e != m_Verts.ElementAt(v0))
            .Where(e => e != m_Verts.ElementAt(v1))
            .Where(e => e != m_Verts.ElementAt(v2))
            .ToArray();

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

            if (m_Mix.TryGetValue(emitter.Sample, out var mix)) {
                m_Mix[emitter.Sample].Value = value;
                MixTrack(mix, emitter.Sample);
                continue;
            }

            // event here for newly added key
            if (!m_AvailableTracks.Any()) {
                Debug.Log($"[walker] {emitter.Sample} attempting to request track with no more available tracks");
                continue;
            }

            var track = m_AvailableTracks.First();
            m_AvailableTracks.RemoveAt(0);
            mix = new WalkerMix() { Track=track, Value=value };
            m_Mix[emitter.Sample] = mix;
            print($"added {emitter.Sample} @ track: {track}");

            MixTrack(mix, emitter.Sample);
        }

        // already clean mix
        foreach(var key in m_Mix.Keys) {
            var mix = m_Mix[key];
            m_TextureMixer.SetTrackMix(mix.Track, m_AlphaCurve.Evaluate(mix.Value));
        }

        m_Debug.text = string.Concat(m_Tracks.Select((track) => {
            var sample = track.Sample;

            var mixInfo = "disabled";
            if(m_Mix.TryGetValue(sample, out var mix)) {
                mixInfo = $"{mix.Value}";
            }

            return $"track {track} | {sample} : {mixInfo}\n";
        }));
    }

    void MixTrack(WalkerMix mix, string sample) {
        var trk = m_Tracks[mix.Track];

        trk.Volume = m_VolumeCurve.Evaluate(mix.Value);

        if (mix.Value > 0) {
            if(trk.IsPaused) {
                trk.IsPaused = false;
                if (m_ReloadSampleOnUnpause) {
                    trk.LoadSample(sample);
                }
            }

        } else {
            if(!trk.IsPaused) {
                trk.IsPaused = true;
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

        m_Emitters = m_Emitters.OrderBy(Dist).ToList();
        var closest = m_Emitters.First();
        var closestDist = Dist(closest);
        Gizmos.color = Color.magenta;
        for (var i = 0; i < TrackCount; i++) {
            if(i > 2) {
                Gizmos.color = Color.cyan;
            }

            if (i >= m_Channels) {
                Gizmos.color = Color.blue;
            }

            var em = m_Emitters.ElementAt(i);
            Gizmos.DrawLine(transform.position, em.transform.position);
            Gizmos.DrawSphere(em.transform.position, closestDist / Dist(em));
        }

    }

    string TrackName(string track) => $"track{track}";
    string TrackName(int track) => $"track{track}";
}