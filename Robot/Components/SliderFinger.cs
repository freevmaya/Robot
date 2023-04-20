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
        private float _openness;

        private Rigidbody rb => GetComponent<Rigidbody>();

        private void OnValidate()
        {
            Direct = Direct.normalized;
        }

        public override void setOpenness(float value)
        {
            if (_openness != value)
            {
                Vector3 moveTo = Direct * Mathf.Lerp(MoveLimit.min, MoveLimit.max, value);
                if (!rb) transform.localPosition = moveTo;

                _openness = value;
            }
        }

        private void Update()
        {
            if (rb)
            {
                Vector3 moveTo = transform.parent.TransformPoint(Direct * Mathf.Lerp(MoveLimit.min, MoveLimit.max, _openness));
                Vector3 worldDelta = moveTo - transform.position;
                if (worldDelta.sqrMagnitude > 0.01f)
                {
                    rb.AddForce(worldDelta * Power, ForceMode.VelocityChange);
                }
            }
        }
    }
}
