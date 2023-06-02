using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MutCommon
{
    public abstract class Raycaster : MonoBehaviour
    {
        [SerializeField] protected Camera camera;

        [SerializeField] private LayerMask layer = 0;

        [SerializeField] private float maxDistance = 1000;

        private RaycastTarget currentTarget;
        private Vector3 lastHitPoint;

        private bool _enabled = true;
        public bool Enabled {
            get => _enabled;
            protected set {
                if (_enabled != value) {
                    if (!value) {
                        OnDisable();
                    }
                }
                _enabled = value;
            }
        }

        private void OnValidate()
        {
            if (camera == null) { camera = Camera.main; }
        }

        // Update is called once per frame
        void Update()
        {
            if ( !Enabled ) return;

            var ray = Ray();
            bool acquiredTarget = false;
            if (Physics.Raycast(ray, out var hitInfo, maxDistance, layer))
            {
                var target = hitInfo.transform.GetComponent<RaycastTarget>();
                if (target == null) return;
                target = hitInfo.transform.GetComponentInParent<RaycastTarget>();
                if (target == null) return;
                acquiredTarget = true;
                lastHitPoint = hitInfo.point;

                if (target != currentTarget)
                {
                    currentTarget?.OnCollisionExit.Invoke(hitInfo.point);
                    currentTarget = target;
                    currentTarget.OnCollisionEnter.Invoke(hitInfo.point);
                }
                else
                {
                    currentTarget = target;
                    currentTarget.OnCollisionStay.Invoke(hitInfo.point);
                }
            }

            if (!acquiredTarget && currentTarget != null)
            {
                currentTarget.OnCollisionExit.Invoke(lastHitPoint);
                currentTarget = null;
            }
        }

        private void OnDisable() {
            if (currentTarget != null)
            {
                currentTarget.OnCollisionExit.Invoke(lastHitPoint);
                currentTarget = null;
            }
        }

        protected abstract Ray Ray();

        private void OnDrawGizmos()
        {
            var r = Ray();
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(r.origin, r.origin + r.direction * maxDistance);
        }
    }
}