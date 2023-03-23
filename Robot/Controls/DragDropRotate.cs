using UnityEngine;
using Vmaya.Scene3D;
using Vmaya.Util;

namespace Vmaya.Robot.Controls
{
    public class DragDropRotate : BaseDragDrop
    {
        [SerializeField]
        private Vector3 _localAxis = Vector3.left;

        [SerializeField]
        private float _centerOffset = 0;

        private Vector3 _startDirect;
        private float _startAngle;

        private void OnValidate()
        {
            if (_localAxis.sqrMagnitude > 0) _localAxis.Normalize();
        }

        public float getMouseAngle()
        {
            Vector3 worldDirect = transform.parent.TransformDirection(_localAxis);
            Vector3 pos = transform.position + worldDirect * _centerOffset;
            Plane plane = new Plane(worldDirect, pos);
            float distance;
            Ray ray = hitDetector.mRay();

            if (plane.Raycast(ray, out distance))
            {
                Vector3 point = ray.GetPoint(distance);
                Vector3 direct = point - pos;

                Debug.DrawLine(pos, pos + _startDirect * direct.magnitude, Color.blue);
                Debug.DrawLine(pos, point);
                float angle = Vector3.SignedAngle(_startDirect, direct, transform.parent.TransformDirection(_localAxis));

                Debug.Log(angle);
                return angle;
            }

            return 0;
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

        protected void saveStartData()
        {
            _startDirect = transform.forward;
            _startAngle = getMouseAngle();
        }
        protected override void doDrag()
        {
            float curAngle = getMouseAngle();
            float delta = curAngle - _startAngle;
            transform.localRotation = Quaternion.AngleAxis(delta, _localAxis) * transform.localRotation;
            _startAngle = curAngle;
        }

        protected override bool Dragging()
        {
            return Mathf.Abs(_startAngle - getMouseAngle()) > 0.5f;
        }
    }
}