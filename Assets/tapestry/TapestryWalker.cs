using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TapestryWalker : MonoBehaviour
{
    private class WalkerMix {
        public int Track;
        public float Value;
    }

    [Header("tuning")]
    [SerializeField] float m_Speed;
    [SerializeField] float m_WrapLength;
    [SerializeField] AnimationCurve m_SqrDistCurve;

    [Header("mixing")]
    [Header("number of bizhawk instances")]
    [SerializeField] int m_Tracks = 4;

    [Header("number of expected mixing channels")]
    [SerializeField] int m_Channels = 3;

    [SerializeField] Dictionary<string, WalkerMix> m_Mix;
    [SerializeField] MixTextures m_TextureMixer;
    [SerializeField] SampleLoader m_SampleLoader;
    [SerializeField] Material m_Mat;


    private IEnumerable<TapestryEmitter> m_Emitters;
    private List<int> m_AvailableTracks = new List<int>() {1, 2, 3, 4};

    // Start is called before the first frame update
    void Awake()
    {
        m_Emitters = FindObjectsOfType<TapestryEmitter>().ToList();
        m_Mix = new Dictionary<string, WalkerMix>();
        m_AvailableTracks = Enumerable.Range(0, m_Tracks).Select(x => x+1).ToList();
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
            if (!m_Emitters.Take(m_Channels).Any(e => e.Name == key)) {
                var removed = m_Mix[key];
                m_AvailableTracks.Add(removed.Track);
                m_TextureMixer.SetTrackMix(removed.Track, 0.0f);
                print($"removed {key} tex{removed.Track}");
                keysToRemove.Add(key);
            }
        }

        foreach(var key in keysToRemove) {
            m_Mix.Remove(key);
        }

        for (var i = 0; i < m_Tracks; i++) {
            var emitter = m_Emitters.ElementAt(i);
            // see if already using name
            var value = m_SqrDistCurve.Evaluate(
                Mathf.InverseLerp(
                    closestDist,
                    lastChannelDist,
                    Dist(emitter)
                )
            );

            if (m_Mix.TryGetValue(emitter.Name, out var mix)) {
                m_Mix[emitter.Name].Value = value;
                continue;
            }

            // event here for newly added key
            var track = m_AvailableTracks.First();
            m_AvailableTracks.RemoveAt(0);
            mix = new WalkerMix() { Track=track, Value=value };
            print($"added {emitter.Name} @ track: {track}");

            m_Mat.SetTexture($"_tex{track}", emitter.TestTexture);
            m_SampleLoader.LoadSample($"track{track}", emitter.Name);

            m_Mix[emitter.Name] = mix;
        }

        // already clean mix
        foreach(var key in m_Mix.Keys) {
            var mix = m_Mix[key];
            m_TextureMixer.SetTrackMix(mix.Track, mix.Value);
        }
    }

    float Dist(TapestryEmitter other) =>
        (Wrap(transform.position) - Wrap(other.transform.position)).sqrMagnitude;

    Vector3 Wrap(Vector3 pos) => new Vector3 (
        Mathf.Repeat(pos.x, m_WrapLength),
        Mathf.Repeat(pos.y, m_WrapLength),
        Mathf.Repeat(pos.z, m_WrapLength)
    );

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        var dim =Vector3.one * m_WrapLength;
        Gizmos.DrawWireCube(Vector3.zero + 0.5f*dim, dim);

        if(m_Emitters == null || m_Emitters.Count() == 0) return;

        m_Emitters = m_Emitters.OrderBy(Dist).ToList();
        var closest = m_Emitters.First();
        var closestDist = Dist(closest);
        Gizmos.color = Color.magenta;
        for (var i = 0; i < 3; i++) {
            if(i > 0) {
                Gizmos.color = Color.cyan;
            }
            var em = m_Emitters.ElementAt(i);
            Gizmos.DrawLine(transform.position, em.transform.position);
            Gizmos.DrawSphere(em.transform.position, closestDist / Dist(em));
        }

    }
}
