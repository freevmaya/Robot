using UnityEngine;
using Vmaya.Robot.Components;
using Vmaya.Scene3D;
using Vmaya.Util;

namespace Vmaya.Robot.Controls
{
    public class DragDropRotateElement : BaseDragDrop
    {
        [SerializeField]
        private float _smooth;
        public Component rotationElement;
        protected IRotationElement _rotationElement => rotationElement ? rotationElement.GetComponent<IRotationElement>(): null;

        private float _startAngle;
        private float _smoothAngle;

        private void OnValidate()
        {
            rotationElement = _rotationElement as Component;
        }

        public float getMouseAngle()
        {
            Vector3 worldAxis = _rotationElement.getAxis();
            Vector3 pos = _rotationElement.GetPosition();
            Plane plane = new Plane(worldAxis, pos);
            float distance;
            Ray ray = hitDetector.mRay();

            if (plane.Raycast(ray, out distance))
            {
                Vector3 point = ray.GetPoint(distance);
                Vector3 direct = point - pos;

                Debug.DrawLine(pos, pos + _rotationElement.getBaseDirect() * direct.magnitude, Color.blue);
                Debug.DrawLine(pos, point);
                float angle = Vector3.SignedAngle(_rotationElement.getBaseDirect(), direct, worldAxis);
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
            if (_smooth > 0)
                _startAngle = _rotationElement.getAngle() - getMouseAngle();
            else _startAngle = getMouseAngle();
        }

        protected override void doDrag()
        {
            float curAngle = getMouseAngle();
            if (_smooth > 0)
                _smoothAngle = curAngle;
            else
            {
                float delta = curAngle - _startAngle;
                _rotationElement.setAngle(_rotationElement.getAngle() + delta);
                _startAngle = curAngle;
            }
        }

        protected override bool Dragging()
        {
            return Mathf.Abs(_startAngle - getMouseAngle()) > 0.5f;
        }

        protected override void Update()
        {
            base.Update();
            if ((_smooth > 0) && isDrag())
                _rotationElement.setAngle(Mathf.LerpAngle(_rotationElement.getAngle(), _smoothAngle + _startAngle, _smooth));
        }
    }
}