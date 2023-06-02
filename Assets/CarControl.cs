using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public FloatReference MaxDist;
    public FloatReference Speed;
    public FloatReference ReturnSpeed;

    private Vector3 initialPos;

    private float offset = 0.0f;

    void Start()
    {
        initialPos = transform.position;
    }

    void Update()
    {
       var h = Input.GetAxis("Horizontal");
       if(Mathf.Abs(h) > 0.0f) {
        var baseSpeed = Mathf.Sign(h) == Mathf.Sign(offset) ? 0.0f : ReturnSpeed;
        var speed = baseSpeed + Speed;
        offset = Mathf.Clamp(offset + speed * h * Time.deltaTime, -MaxDist, MaxDist);
       } else {
        offset = Mathf.MoveTowards(offset, 0.0f, ReturnSpeed * Time.deltaTime);
       }

       transform.position = initialPos + Vector3.right * offset;
    }
}
