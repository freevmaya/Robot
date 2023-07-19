using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Robot.Controls;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Components
{
    public class ConnectableRotateElement : ConnectableElement, IConnectableRotationElement
    {
        [SerializeField]
        private Limit _angleLimit;

        public Limit getAngleLimiter()
        {
            return _angleLimit;
        }

        public Vector3 getBaseVector()
        {
            ConfigurableJoint joint = GetJoint();
            if (joint)
                return joint.targetVelocity;

            return Vector3.up;
        }

        public Vector3 getAxis()
        {
            ConfigurableJoint joint = GetJoint();
            if (joint)
                return joint.axis;

            return Vector3.left;
        }

        public virtual void Restrict(Limit minMax)
        {
            ConfigurableJoint joint = GetJoint();
            if (joint)
            {
                if ((minMax.min == 0) && (minMax.max == 0))
                    joint.angularXMotion = ConfigurableJointMotion.Free;
                else
                {
                    SoftJointLimit sl = joint.lowAngularXLimit;
                    sl.limit = minMax.min;
                    joint.lowAngularXLimit = sl;

                    SoftJointLimit hl = joint.highAngularXLimit;
                    hl.limit = minMax.max;
                    joint.highAngularXLimit = hl;

                    sl = joint.lowAngularXLimit;
                    sl.limit = minMax.min;
                    joint.lowAngularXLimit = sl;

                    joint.angularXMotion = ConfigurableJointMotion.Limited;
                }
            }
        }

        public bool rotateAvailable()
        {
            return true;
        }

        public float getAngle()
        {
            ConfigurableJoint joint = GetJoint();
            IJoinPoint mainSlot = GetMainSlot();
            if (joint && (mainSlot != null))
            {
                Vector3 localDirect = getBaseDirect();
                Vector3 direct = joint.transform.TransformDirection(localDirect);
                Vector3 baseDirect = mainSlot.Trans().rotation * localDirect;
                return Vector3.SignedAngle(direct, baseDirect, joint.transform.TransformDirection(joint.axis));
            }
            return 0;
        }

        public void setAngle(float value)
        {
            ConfigurableJoint joint = GetJoint();
            if (joint)
            {
                Vector3 _worldNormal = transform.TransformDirection(joint.axis);

                MyRigidBody.rotation = Quaternion.AngleAxis(value, _worldNormal);
                //aimAngle(value);
            }
        }

        public void aimAngle(float AimAngle)
        {
            ConfigurableJoint joint = GetJoint();

            if ((joint != null) && MyRigidBody)
            {
                float angle = getAngle();
                Vector3 _worldNormal = transform.TransformDirection(joint.axis);
                float deltaAngle = Mathf.DeltaAngle(AimAngle, angle);

                float absDelta = Mathf.Abs(deltaAngle);

                float startBreak = 10;
                if (absDelta < startBreak)
                    deltaAngle = Mathf.Pow(absDelta, 3) / Mathf.Pow(startBreak, 3) * startBreak * (deltaAngle < 0 ? -1 : 1);

                //deltaAngle = Mathf.Pow(deltaAngle, 2) * (deltaAngle < 0 ? -1 : 1);

                float force;

                DragDropRotate ddr = GetComponent<DragDropRotate>();
                if (ddr) force = ddr.PowerTorque;
                else force = 10f;

                MyRigidBody.AddTorque(_worldNormal * deltaAngle * force);
            }
        }

        private void OnEnable()
        {
            setAngle(getAngle());
        }

        public override void Freeze(bool v)
        {
            float angle = getAngle();
            setAngle(angle);

            Limit limit;
            if (v)
                limit = Limit.init(angle - 0.01f, angle + 0.01f);
            else limit = getAngleLimiter();

            Restrict(limit);
            base.Freeze(v);
        }
    }
}
