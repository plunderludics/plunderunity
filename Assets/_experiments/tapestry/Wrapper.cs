using UnityAtoms.BaseAtoms;
using UnityEngine;

public class Wrapper : MonoBehaviour
{
    [Header("tuning")]
    [SerializeField] FloatReference m_WrapLength;

    void Update() {
        transform.position = Wrap(transform.position, m_WrapLength.Value);
    }

    public static Vector3 Wrap(Vector3 pos, float wrapLength) => new Vector3 (
        Mathf.Repeat(pos.x, wrapLength),
        Mathf.Repeat(pos.y, wrapLength),
        Mathf.Repeat(pos.z, wrapLength)
    );
}