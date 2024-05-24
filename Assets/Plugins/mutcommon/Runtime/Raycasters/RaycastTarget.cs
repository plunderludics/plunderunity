using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaycastTarget : MonoBehaviour
{
  public UnityEvent<Vector3> OnCollisionEnter;
  public UnityEvent<Vector3> OnCollisionStay;
  public UnityEvent<Vector3> OnCollisionExit;
}
