using Vmaya.Entity;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Vmaya.Robot.Components
{
    [ExecuteInEditMode]
    public class ConnectableElement : BaseEntity, IConnectableElement
    {
        protected ConnectableElement _own => transform.parent ? transform.parent.GetComponentInParent<ConnectableElement>() : null;
        protected ConnectPoint[] _connectPoints => GetComponentsInChildren<ConnectPoint>();

        public int SlotCount()
        {
            return _connectPoints.Length;
        }

        public Transform Trans()
        {
            return transform;
        }

        public int NearestSlot(Vector3 position)
        {
            int result = -1;
            float minDistance = float.MaxValue;
            for (int i=0; i<_connectPoints.Length; i++)
            {
                float distance = (_connectPoints[i].transform.position - position).magnitude;
                if (minDistance > distance)
                {
                    minDistance = distance;
                    result = i;
                }
            }
            return result;
        }

        public int Slot(IConnectableElement child)
        {
            for (int i = 0; i < _connectPoints.Length; i++)
                if (_connectPoints[i].Connected == child)
                    return i;
            return -1;
        }
    }
}
