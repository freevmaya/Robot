using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.IK;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Components
{
    public class RotationElement : SimpleChainLink, IRotationElement
    {
        [SerializeField]
        private Limit _limit;

        [SerializeField]
        private Vector3 _baseDirect = Vector3.forward;

        [SerializeField]
        private Vector3 _axis = Vector3.left;

        private float _angle;
        public bool Threshold => (_angle == _limit.min) || (_angle == _limit.min);

        [HideInInspector]
        public Vector3 StartOffset;

        public IChainLink Parent => GetParent();

        private void OnValidate()
        {
            _baseDirect = _baseDirect.normalized;
        }

        private void Start()
        {
            if (Parent != null)
                StartOffset = getRelativePos();
            else StartOffset = default;

            _angle = CalculateAngle();
        }

        private Vector3 getRelativePos()
        {
            if (Parent != null)
                return transform.position - (Parent as Component).transform.position;

            return transform.localPosition;
        }

        public float getAngle()
        {
            return _angle;
        }

        public float CalculateAngle()
        {
            return Vector3.SignedAngle(getBaseDirect(), getDirect(), getAxis());
        }

        public Limit getAngleLimiter()
        {
            return _limit;
        }

        public Vector3 getAxis()
        {
            return transform.TransformDirection(_axis);
        }

        private Vector3 getDirect()
        {
            return transform.TransformDirection(_baseDirect);
        }

        public Vector3 getBaseDirect()
        {
            return transform.parent.transform.TransformDirection(_baseDirect);
        }

        public void setAngle(float angle)
        {
            if (_angle != angle)
            {
                if (_limit.size > 0) _angle = _limit.Clamp(angle);
                else _angle = angle;

                transform.localRotation = Quaternion.AngleAxis(_angle, Vector3.left);
            }
        }

        public bool rotateAvailable()
        {
            return true;
        }
    }
}
