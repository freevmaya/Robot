using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Drawable;

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

        public void Show(IJoinPoint point)
        {
            _point = point;
            gameObject.SetActive(true);

            ConfigurableJoint joint = _point.GetConnected().GetJoint();

            if (joint.angularXMotion == ConfigurableJointMotion.Limited)
                setLimit(joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit);
            else setLimit(0, 360);

            updatePosition();
        }

        protected void updatePosition()
        {
            transform.position = _point.Trans().position;

            IConnectableElement connected = _point.GetConnected();

            transform.rotation = _point.Trans().rotation;// * Quaternion.LookRotation(, Vector3.up);

            _anglePointer.localRotation = Quaternion.AngleAxis(connected.GetCurrentAngle(), Vector3.up);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _point = null;
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
