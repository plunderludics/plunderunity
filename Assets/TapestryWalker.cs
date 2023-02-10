using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TapestryWalker : MonoBehaviour
{
    private class WalkerMix {
        public int Index;
        public float Value;
    }

    [Header("tuning")]
    [SerializeField] float m_Speed;
    [SerializeField] float m_WrapLength;
    [SerializeField] AnimationCurve m_SqrDistCurve;

    [Header("mixing")]
    [SerializeField] Dictionary<string, WalkerMix> m_Mix;
    [SerializeField] MixTextures m_TextureMixer;
    [SerializeField] Material m_Mat;

    private IEnumerable<TapestryEmitter> m_Emitters;
    private List<int> m_AvailableTextures = new List<int>() {1, 2, 3, 4};

    // Start is called before the first frame update
    void Awake()
    {
        m_Emitters = FindObjectsOfType<TapestryEmitter>().ToList();
        m_Mix = new Dictionary<string, WalkerMix>();
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

        // free textures
        var keysToRemove = new List<string>();
        foreach(var key in m_Mix.Keys) {
            if (!m_Emitters.Take(3).Any(e => e.Name == key)) {
                var removed = m_Mix[key];
                m_AvailableTextures.Add(removed.Index);
                m_TextureMixer.SetTexMix(removed.Index, 0.0f);
                print($"removed {key} tex{removed.Index}");
                keysToRemove.Add(key);
            }
        }

        foreach(var key in keysToRemove) {
            m_Mix.Remove(key);
        }

        for (var i = 0; i < 3; i++) {
            var emitter = m_Emitters.ElementAt(i);
            // see if already using name
            var value = m_SqrDistCurve.Evaluate(closestDist / Dist(emitter));
            if (m_Mix.TryGetValue(emitter.Name, out var mix)) {
                m_Mix[emitter.Name].Value = value;
                continue;
            }

            // event here for newly added key
            var index = m_AvailableTextures.First();
            m_AvailableTextures.RemoveAt(0);
            mix = new WalkerMix() { Index=index, Value=value };
            print($"added {emitter.Name} tex{index}");

            m_Mat.SetTexture($"_tex{index}", emitter.TestTexture);

            m_Mix[emitter.Name] = mix;
        }

        // already clean mix
        foreach(var key in m_Mix.Keys) {
            var mix = m_Mix[key];
            m_TextureMixer.SetTexMix(mix.Index, mix.Value);
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
