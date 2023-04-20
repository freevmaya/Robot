using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Drawable;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Components
{
    public class ProtractorTool : MonoBehaviour
    {
        public Protractor _protractor;
        public ProtractorMeasureLines _measureLines;
        public Transform _anglePointer;

        private static ProtractorTool _instance;
        public static ProtractorTool instance => _instance ? _instance : _instance = FindObjectOfType<ProtractorTool>(true);

        private IJoinPoint _point;
        private bool _show;
        protected ConnectableRotateElement connected => _point.GetConnected() as ConnectableRotateElement;

        public void Show(IJoinPoint point)
        {
            _show = true;
            _point = point;
            gameObject.SetActive(true);

            Limit limit = connected.getAngleLimiter();
            if (limit.size > 0)
                setLimit(limit.min, limit.max);
            else setLimit(0, 360);

            updatePosition();
        }

        protected void updatePosition()
        {

            if (connected != null)
            {
                transform.position = _point.Trans().position;

                Limit limit = connected.getAngleLimiter();
                Quaternion q = default;
                if (limit.size > 0)
                    q = Quaternion.AngleAxis(90 + limit.min, Vector3.up);

                transform.rotation = _point.Trans().rotation;

                _protractor.transform.localRotation = q;
                _measureLines.transform.localRotation = q;
                _anglePointer.localRotation = Quaternion.AngleAxis(connected.getAngle(), Vector3.up);
            }
        }

        public void Hide()
        {
            _show = false;
            Vmaya.Utils.setTimeout(this, () =>
            {
                if (!_show)
                {
                    gameObject.SetActive(false);
                    _point = null;
                }
            }, 1);
        }

        public void setLimit(float min, float max)
        {
            float total = (max - min) / 360f;
            _protractor.setSector(total);
            _measureLines.setSector(total);
        }

        private void Update()
        {
            if (_point != null) updatePosition();
        }
    }
}
