using UnityEngine;
using Vmaya.Scene3D;
using Vmaya.Util;
using MyBox;
using System;

namespace Vmaya.Robot.Controls
{
    public class DragDropRotate : BaseDragDrop
    {
        [SerializeField]
        private Vector3 _localAxis = Vector3.left;

        [SerializeField]
        private float _centerOffset = 0;

        [SerializeField]
        private bool _useRigidbody = false;

        [SerializeField]
        private float _startThresholdPixels = 0.1f;

        [ConditionalField("_useRigidbody")][SerializeField] private float _powerTorque = 1;
        public float PowerTorque => _powerTorque;

        private Vector3 _startDirect;
        private Vector3 _startLocalDirect;
        protected float _startAngle;
        protected Vector3 _startPosition;
        private Quaternion _startRotate;

        private Rigidbody _rigidBody => GetComponent<Rigidbody>();
        protected Vector3 _worldNormal => transform.TransformDirection(_localAxis);
        protected Plane _worldPlane => new Plane(_worldNormal, _centerPlane);
        protected Vector3 _centerPlane => transform.position + _worldNormal * _centerOffset;

        private void OnValidate()
        {
            if (_localAxis.sqrMagnitude > 0) _localAxis.Normalize();
        }


        protected Vector3 getMousePoint()
        {
            float distance;
            Ray ray = hitDetector.mRay();
            _worldPlane.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }

        public float getMouseAngle()
        {
            Vector3 center = _centerPlane;
            Vector3 point = getMousePoint();
            Vector3 direct = point - center;

            DebugDirect(_startDirect * direct.magnitude, Color.blue);
            DebugDirect(direct, Color.white);

            float angle = Mathf.Round(Vector3.SignedAngle(_startDirect, direct, _worldNormal) * 10f) / 10f; //DEV Rounding

            return angle;
        }

        protected void DebugDirect(Vector3 direct, Color color)
        {
            Debug.DrawLine(_centerPlane, _centerPlane + direct, color);
        }

        public override void doMouseDown()
        {
            base.doMouseDown();
            saveStartData();
        }

        protected override void beginDrag()
        {
            base.beginDrag();
            saveStartData();
        }

        protected virtual void saveStartData()
        {
            _startPosition = VMouse.mousePosition;
            _startDirect    = (getMousePoint() - _centerPlane).normalized;
            _startLocalDirect = transform.InverseTransformDirection(_startDirect);
            _startAngle     = getMouseAngle();
            _startRotate    = transform.rotation;
        }
        protected override void doDrag()
        {
            float delta = getMouseAngle();

            if (_useRigidbody && _rigidBody)
            {
                Vector3 _curDirect = transform.TransformDirection(_startLocalDirect);
                float _curDelta = Vector3.SignedAngle(_startDirect, _curDirect, _worldNormal);

                _rigidBody.AddTorque(_worldNormal * Mathf.DeltaAngle(_curDelta, delta) * _powerTorque);

            }
            else transform.rotation = Quaternion.AngleAxis(delta, _worldNormal) * _startRotate;
        }

        protected override void Drop()
        {
            base.Drop();
        }

        protected float getDeltaAngle()
        {
            return _startAngle + getMouseAngle();
        }

        protected override bool Dragging()
        {
            return Mathf.Abs((VMouse.mousePosition -_startPosition).magnitude) > _startThresholdPixels;
        }
    }
}