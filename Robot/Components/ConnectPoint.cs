using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vmaya.Robot.Components
{
    [ExecuteInEditMode]
    public class ConnectPoint : MonoBehaviour, IJoinPoint
    {
        protected IConnectableElement _own => GetComponentInParent<IConnectableElement>();

        [SerializeField]
        private ConnectType _connectType;

        [SerializeField]
        private float _scaleFactor = 1;

        [SerializeField]
        private Component connected;

        [SerializeField]
        private bool _makeChild;

        private IConnectableElement _connected;

        public IConnectableElement Connected { get => _connected; set => SetConnect(value); }

        protected virtual void OnValidate()
        {
            if (connected)
            {
                IConnectableElement component = connected.GetComponent<IConnectableElement>();
                if ((connected = component as Component) != null)
                {
                    _connected = component;
                    updateConnected();
                }
            }
        }

        public void SetConnect(IConnectableElement element)
        {
            if (_connected != element)
            {
                if (_connected != null) disconnect();

                if ((element != _own) && (_connected = element) != null)
                    updateConnected();
                else _connected = null;
            }
        }

        public void UpdateConnectedPosition()
        {
            if (_connected != null) updateConnected(false);
        }

        public virtual Vector3 getWorldConnectPoint()
        {
            return transform.position;
        }

        protected virtual void updateConnected(bool resetRotateAndScale = true)
        {
            if ((_own != null) && (_connected != null))
            {
                if (_makeChild) _connected.Trans().parent = transform;
                connected = _connected as Component;
                if (resetRotateAndScale)
                {
                    _connected.Trans().rotation = transform.rotation;
                    _connected.Trans().localScale = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);
                }
                _connected.GetRigidBody().useGravity = false;

                ConfigurableJoint joint = _connected.GetJoint();
                if (joint)
                {
                    _connected.Trans().position = getWorldConnectPoint() - _connected.Trans().TransformVector(joint.anchor);
                    joint.connectedAnchor = _connected.Trans().InverseTransformPoint(transform.position);
                    joint.connectedBody = _own.GetRigidBody();
                }
                else _connected.Trans().position = transform.position;
            }
        }

        private void disconnect()
        {
            if (_connected != null)
            {
                ConfigurableJoint joint = _connected.GetJoint();
                joint.connectedBody = null;
                joint.xMotion = ConfigurableJointMotion.Free;
                joint.yMotion = ConfigurableJointMotion.Free;
                joint.zMotion = ConfigurableJointMotion.Free;
                _connected.GetRigidBody().useGravity = true;
                _connected = null;
            }
        }

        private void OnDrawGizmos()
        {
            Utils.DrawArrowGismo(transform, -transform.right);
        }

        public ConnectType GetConnectType()
        {
            return _connectType;
        }

        public Transform Trans()
        {
            return transform;
        }

        public IConnectableElement GetConnected()
        {
            if (connected && !connected.gameObject.activeSelf)
                return null;

            return _connected;
        }
    }
}
