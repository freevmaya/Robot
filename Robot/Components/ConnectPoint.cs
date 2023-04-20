using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vmaya.Robot.Components
{
    [ExecuteInEditMode]
    public class ConnectPoint : MonoBehaviour, IJoinPoint
    {
        [System.Serializable]
        protected class PointRecord
        {
            public Indent Connected;
            public Vector3 ConnectedPosition;
            public Vector3 ConnectedEuler;
        }

        protected IConnectableElement _own => GetComponentInParent<IConnectableElement>();

        [SerializeField]
        private ConnectType _connectType;

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

        protected virtual void updateConnected(bool resetRotate = true)
        {
            if ((_own != null) && (_connected != null))
            {
                if (_makeChild) _connected.Trans().parent = transform;
                connected = _connected as Component;
                if (resetRotate)
                    _connected.Trans().rotation = transform.rotation;

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

        protected virtual void restoreFromData(PointRecord pr)
        {
            if (!Indent.isNull(pr.Connected))
            {
                SetConnect(pr.Connected.Find() as IConnectableElement);
                _connected.Trans().position = pr.ConnectedPosition;
                _connected.Trans().rotation = Quaternion.Euler(pr.ConnectedEuler);
                _connected.Freeze(true);
            }
        }

        protected virtual PointRecord createPointRecord()
        {
            return new PointRecord();
        }

        protected virtual PointRecord getRestoreData()
        {
            PointRecord pr = createPointRecord();
            if (connected)
            {
                pr.Connected = new Indent(connected);
                pr.ConnectedPosition = connected.transform.position;
                pr.ConnectedEuler = connected.transform.rotation.eulerAngles;
            }
            return pr;
        }

        public virtual void setJson(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                PointRecord pr = createPointRecord();
                JsonUtility.FromJsonOverwrite(jsonData, pr);
                if (!Indent.isNull(pr.Connected))
                    Indent.AfterInstance(this, pr.Connected, () =>
                    {
                        restoreFromData(pr);
                    });
                else restoreFromData(pr);
            }
        }

        public virtual string getJson()
        {
            return JsonUtility.ToJson(getRestoreData());
        }

        public void setActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public bool getActive()
        {
            return gameObject.activeSelf;
        }
    }
}
