using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vmaya.Robot.Components
{
    public class HandElement : ConnectableRotateElement
    {
        [SerializeField]
        private ConfigurableJoint _wrist;
        public ConfigurableJoint Wrist => _wrist;

        public BaseFinger[] Fingers => GetComponentsInChildren<BaseFinger>();

        [SerializeField]
        [Range(0, 1)]
        private float _openness;
        private float _lastOpenness;

        private float _toTurn;

        public float Openness { get => _openness; set => setOpenness(value); }
        public float Turn { get => getTurn(); internal set => setTurn(value); }

        private void setTurn(float value)
        {
            //Quaternion q = Quaternion.AngleAxis(value, Vector3.forward);
            _toTurn = value;
        }
        private float getTurn()
        {
            return Wrist.transform.localEulerAngles.z;
        }

        private void updateTurn()
        {
            Rigidbody rb = Wrist.GetComponent<Rigidbody>();
            float delta = Mathf.DeltaAngle(getTurn(), _toTurn);
            rb.AddRelativeTorque(new Vector3(0, 0, delta) * 0.02f);
        }

        private Collider _connected;

        protected void setOpenness(float value)
        {
            if (_lastOpenness != value)
            {
                _lastOpenness = _openness = value;
                updateHand();
                OnChange.Invoke();
            }
        }

        protected bool checkChange()
        {
            return _openness != _lastOpenness;
        }

        private void Update()
        {
            if (checkChange()) updateHand();

            if (Wrist) updateTurn();
        }

        private void updateHand()
        {
            Collider touch = AllFingersToch();
            if (touch == null)
                setOpenness(_openness);
            
            touchConnect(touch);
        }

        protected void touchConnect(Collider a_touch)
        {
            Rigidbody myRb = Wrist ? Wrist.GetComponent<Rigidbody>() : GetRigidBody();

            if (a_touch != _connected)
            {
                ConfigurableJoint joint;
                if (_connected)
                {
                    joint = _connected.GetComponent<ConfigurableJoint>();
                    if (joint.connectedBody == myRb)
                    {
                        joint.connectedBody = null;
                        joint.zMotion = joint.xMotion = joint.yMotion = ConfigurableJointMotion.Free;
                        joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Free;
                    }
                }

                if (a_touch)
                {
                    joint = a_touch.GetComponent<ConfigurableJoint>();
                    joint.connectedBody = myRb;
                    joint.zMotion = joint.xMotion = joint.yMotion = ConfigurableJointMotion.Limited;
                    joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Limited;
                }

                _connected = a_touch;
            }
        }

        public Collider AllFingersToch()
        {
            Collider touch = null;
            foreach (BaseFinger finger in Fingers)
            {
                Collider col = finger.getTouch();
                if (col)
                {
                    if (touch == null) touch = col;
                    else if (touch != col) return null;
                }
                else return null;
            }
            return touch;
        }
    }
}
