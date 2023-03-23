using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vmaya.Robot.Components
{
    [ExecuteInEditMode]
    public class ConnectPoint : MonoBehaviour
    {
        protected IConnectableElement _own => GetComponentInParent<ConnectableElement>();
        private IConnectableElement _connected => GetComponentInChildren<ConnectableElement>();

        public IConnectableElement Connected => _connected;

        public void setConnect(IConnectableElement element)
        {
            if (_connected != element)
            {
                if (_connected != null) disconnect();
                if (element != null)
                {
                    element.Trans().parent = transform;
                    if (_connected != null) updateConnected();
                }
            }
        }

        private void disconnect()
        {
            _connected.Trans().parent = transform.root;
        }

        private void updateConnected()
        {
            Vector3 lp = _connected.Trans().localPosition;
            if (lp.magnitude > 0)
            {
                Component component = _connected as Component;

                _connected.Trans().localPosition = Vector3.zero;
                _connected.Trans().localRotation = Quaternion.identity;
            }
        }

        private void OnDrawGizmos()
        {
            Utils.DrawArrowGismo(transform, -transform.right);
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (_connected != null) updateConnected();
#endif
        }
    }
}
