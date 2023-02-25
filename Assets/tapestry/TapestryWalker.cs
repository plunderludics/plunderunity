using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GK;
using UnityEngine;

public class TapestryWalker : MonoBehaviour
{
    private class WalkerMix {
        public int Track;
        public float Value;
    }

    [Header("tuning")]
    [SerializeField] float m_Radius;
    [SerializeField] float m_Speed;
    [SerializeField] float m_WrapLength;
    [SerializeField] AnimationCurve m_SqrDistCurve;

    [Header("mixing")]
    [Header("number of bizhawk instances")]
    [SerializeField] int m_Tracks = 4;

    [Header("number of expected mixing channels")]
    [SerializeField] int m_Channels = 3;

    [Header("debug")]
    [SerializeField] TMPro.TMP_Text m_Debug;

    [SerializeField] MixTextures2 m_TextureMixer;
    [SerializeField] SampleLoader m_SampleLoader;
    [SerializeField] Material m_Mat;


    Dictionary<string, WalkerMix> m_Mix;
    IEnumerable<TapestryEmitter> m_Emitters;
    IEnumerable<TapestryEmitter> m_Verts;
    DelaunayTriangulation m_Triangulation;
    List<int> m_AvailableTracks;

    public float WrapLength => m_WrapLength;

    // Start is called before the first frame update
    void Awake()
    {
        m_Mix = new Dictionary<string, WalkerMix>();
        m_AvailableTracks = Enumerable.Range(0, m_Tracks).Select(x => x+1).ToList();
    }

    void Start() {
        m_Emitters = FindObjectsOfType<TapestryEmitter>().ToList();
        m_Verts = FindObjectsOfType<TapestryEmitter>().ToList();
        var calculator = new DelaunayCalculator();
        m_Triangulation = calculator.CalculateTriangulation(
            m_Verts.Select(
                e => new Vector2(
                    e.transform.position.x,
                    e.transform.position.z
            ))
            .ToList());
    }

    // Update is called once per frame
    void Update()
    {
        // move walker
        var velocity =
            (Input.GetAxis("Vertical") * Vector3.forward +
            Input.GetAxis("Horizontal") * Vector3.right)
            * m_Speed;

        transform.position += velocity * Time.deltaTime;
        transform.position = Wrap(transform.position);

        // TODO: optimize
        m_Emitters = m_Emitters.OrderBy(Dist).ToList();

        var closest = m_Emitters.First();
        var closestDist = Dist(closest);
        // the thing that is furtest away determines where zero is
        // TODO: maybe all this stuff should be absolute to make life easier
        var lastChannelDist = Dist(m_Emitters.ElementAt(m_Channels+1));

        // free textures
        var keysToRemove = new List<string>();
        foreach(var key in m_Mix.Keys) {
            if (!m_Emitters.Take(m_Tracks).Any(e => e.Sample == key)) {
                var removed = m_Mix[key];
                m_AvailableTracks.Add(removed.Track);
                print($"removed {key} tex{removed.Track}");
                keysToRemove.Add(key);
            }
        }

        foreach(var key in keysToRemove) {
            m_Mix.Remove(key);
        }

        var tris = m_Triangulation.Triangles;
        var verts = m_Triangulation.Vertices;
        var l0 = 0.0f;
        var l1 = 0.0f;
        var l2 = 0.0f;
        var P = new Vector2(transform.position.x, transform.position.z);
        var v0 = tris[0];
        var v1 = tris[1];
        var v2 = tris[2];
        // go through all triangles to find which one we are inside
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
            .Where(e => e != m_Verts.ElementAt(v2));

        for (var i = 0; i < m_Tracks; i++) {
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
                var tr = $"track{mix.Track}";
                if (value > 0) {
                    m_SampleLoader.UnPause(tr);

                } else {
                    m_SampleLoader.Pause(tr);
                }
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
            print($"added {emitter.Sample} @ track: {track}");

            var t = $"track{track}";
            m_SampleLoader.LoadSample(t, emitter.Sample);
            if (value > 0) {
                m_SampleLoader.UnPause(t);

            } else {
                m_SampleLoader.Pause(t);
            }

            m_Mix[emitter.Sample] = mix;
        }

        // already clean mix
        foreach(var key in m_Mix.Keys) {
            var mix = m_Mix[key];
            m_TextureMixer.SetTrackMix(mix.Track, mix.Value);
        }

        m_Debug.text = string.Concat(m_SampleLoader.Tracks.Select((kvp) => {
            var track = kvp.Key;
            var sample = kvp.Value;

            var mixInfo = "disabled";
            if(m_Mix.TryGetValue(sample, out var mix)) {
                mixInfo = $"{mix.Value}";
            }

            return $"track {track} | {sample} : {mixInfo}\n";
        }));
    }

    float Dist(TapestryEmitter other) =>
        (Wrap(transform.position) - other.transform.position).sqrMagnitude;

    Vector3 Wrap(Vector3 pos) => new Vector3 (
        Mathf.Repeat(pos.x, m_WrapLength),
        Mathf.Repeat(pos.y, m_WrapLength),
        Mathf.Repeat(pos.z, m_WrapLength)
    );

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        var dim =Vector3.one * m_WrapLength;
        Gizmos.DrawWireCube(Vector3.zero + 0.5f*dim, dim);

        if(m_Emitters == null || m_Emitters.Count() == 0) return;

        m_Emitters = m_Emitters.OrderBy(Dist).ToList();
        var closest = m_Emitters.First();
        var closestDist = Dist(closest);
        Gizmos.color = Color.magenta;
        for (var i = 0; i < m_Tracks; i++) {
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
}
