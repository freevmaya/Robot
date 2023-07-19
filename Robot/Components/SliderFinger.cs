using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Components
{
    public class SliderFinger : BaseFinger
    {
        public float Power;
        public Limit MoveLimit;
        public Vector3 Direct = Vector3.left;
        public Collider boundsCollider;
        private Collider _touch;

        private Rigidbody rb => GetComponent<Rigidbody>();
        private ConfigurableJoint joint => GetComponent<ConfigurableJoint>();

        private bool _alreadyTouch;

        private void OnValidate()
        {
            Direct = Direct.normalized;
        }

        public override Collider getTouch()
        {
            return _touch;
        }

        public bool findFutureCollisions(ref Vector3 a_delta)
        {
            bool result = false;
            _touch = null;
            if (boundsCollider != null)
            {
                Collider[] cols = boundsCollider.transform.root.GetComponentsInChildren<Collider>();
                foreach (Collider col in cols)
                {
                    if (!col.transform.IsChildOf(Hand.transform))
                    {
                        Vector3 direct;
                        float distance;
                        if (Physics.ComputePenetration(boundsCollider, boundsCollider.transform.position + a_delta, transform.rotation,
                                                    col, col.transform.position, col.transform.rotation, out direct, out distance))
                        {
                            _touch = col;
                            a_delta += direct * distance;
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        protected override void moveStep(float speed, bool checkPlace = true)
        {
            base.moveStep(speed, checkPlace);

            void moveToDelta(Vector3 a_newDelta)
            {
                Vector3 velocity = Vector3.Project(transform.parent.InverseTransformVector(a_newDelta), Direct);
                if (rb) rb.velocity = velocity * Power;
                else transform.localPosition += velocity;
            }

            Vector3 moveTo = Direct * Mathf.Lerp(MoveLimit.min, MoveLimit.max, opennessTo);

            if (boundsCollider && checkPlace)
            {
                Vector3 _newDelta = moveTo - transform.localPosition;
                _newDelta = transform.parent.TransformVector(Vector3.Project(_newDelta, Direct));

                if (!findFutureCollisions(ref _newDelta)) {
                    moveToDelta(_newDelta);
                    _alreadyTouch = false;
                } else
                {
                    if (!_alreadyTouch)
                        moveToDelta(_newDelta);

                    _alreadyTouch = true;
                }
            }
            else transform.localPosition = moveTo;
        }
    }
}
