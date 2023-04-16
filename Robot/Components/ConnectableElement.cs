using Vmaya.Entity;
using UnityEngine;
using System.Collections.Generic;
using System;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Components
{
    [ExecuteInEditMode]
    public class ConnectableElement : MonoBehaviour, IConnectableElement
    {
        [SerializeField]
        private ConnectType _connectType;

        [SerializeField]
        private ConfigurableJoint _myJoint;
        public ConfigurableJoint MyJoint => _myJoint;

        [SerializeField]
        private Rigidbody _myRigidBody;
        public Rigidbody MyRigidBody => _myRigidBody;

        [SerializeField]
        protected Component[] _connectPoints;

        [SerializeField]
        protected BoundsComponent _workingBounds;

        public IJoinPoint this[int index] => _connectPoints[index] ? _connectPoints[index].GetComponent<IJoinPoint>() : null;

        public float Size { get => getSize(); set => setSize(value); }
        public Limit SizeLimit = Limit.init(0, 2);

        private float _lowSafe;
        private float _highSafe;
        private ConfigurableJointMotion _motionSafe;

        private void OnValidate()
        {
            if (!_myJoint) _myJoint = GetComponent<ConfigurableJoint>();
            if (!_myRigidBody) _myRigidBody = GetComponent<Rigidbody>();

            if (_connectPoints != null)
            {
                for (int i = 0; i < _connectPoints.Length; i++)
                    if (_connectPoints[i])
                        _connectPoints[i] = _connectPoints[i].GetComponent<IJoinPoint>() as Component;
            }
            else
            {
                IJoinPoint[] children = GetComponentsInChildren<IJoinPoint>();
                _connectPoints = new Component[children.Length];

                for (int i = 0; i < _connectPoints.Length; i++)
                    _connectPoints[i] = children[i] as Component;
            }
        }

        protected virtual void Start()
        {
            ConfigurableJoint joint = GetJoint();
            if (joint)
            {
                _lowSafe = joint.lowAngularXLimit.limit;
                _highSafe = joint.highAngularXLimit.limit;
                _motionSafe = joint.angularXMotion;
            }
        }

        private void OnDisable()
        {

        }

        public void UpdateConnectPointsPosition()
        {
            foreach (ConnectPoint point in _connectPoints)
            {
                if (point.Connected != null)
                {
                    point.UpdateConnectedPosition();
                    ConnectableElement elem = point.Connected as ConnectableElement;
                    if (elem) elem.UpdateConnectPointsPosition();
                }
            }
        }

        protected virtual void setSize(float value)
        {
            if (value > 0)
            {
                Physics.autoSimulation = false;

                Vector3 scale = new Vector3(value, value, value);
                transform.localScale = scale;
                UpdateConnectPointsPosition();

                Vmaya.Utils.afterEndOfFrame(this, () =>
                {
                    Physics.autoSimulation = true;
                });
            }
        }

        protected virtual float getSize()
        {
            return (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f;
        }

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
                if (this[i].GetConnected() == child)
                    return i;
            return -1;
        }

        protected virtual void toSlotA(int slot, IConnectableElement other)
        {
            this[slot].SetConnect(other);
        }

        public void toSlot(int slot, IConnectableElement other)
        {
            if ((slot >= 0) && (slot < SlotCount()))
            {
                if ((other != this as IConnectableElement) && (other.GetConnectType() == this[slot].GetConnectType()))
                    toSlotA(slot, other);
            }
            else Debug.Log("Not found slot " + slot + " in " + new Indent(this));
        }

        public int FindFree(ConnectType a_type, Vector3 point)
        {

            float minDistance = float.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < _connectPoints.Length; i++)
                if ((this[i].GetConnected() == null) && (this[i].GetConnectType() == a_type))
                {
                    float distance = (this[i].Trans().position - point).magnitude;
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minIndex = i;
                    }
                }
            return minIndex;
        }

        public void Free(int slot)
        {
            this[slot].SetConnect(null);
        }

        public bool isFree(int slot)
        {
            return this[slot].GetConnected() == null;
        }

        public ConfigurableJoint GetJoint()
        {
            return _myJoint;
        }

        public Rigidbody GetRigidBody()
        {
            return _myRigidBody;
        }

        public ConnectType GetConnectType()
        {
            return _connectType;
        }

        public IJoinPoint GetSlot(int idx)
        {
            return this[idx];
        }

        public IConnectableElement GetParent()
        {
            return GetJoint() && (GetJoint().connectedBody != null) ? GetJoint().connectedBody.GetComponent<IConnectableElement>() : null;
        }

        public IJoinPoint GetMainSlot()
        {
            int idx = GetMainSlotIdx();
            if (idx > -1)
                return GetParent().GetSlot(idx);

            return null;
        }

        public int GetMainSlotIdx()
        {
            IConnectableElement parent = GetParent();
            if (parent != null)
            {
                for (int i = 0; i < parent.SlotCount(); i++)
                    if (parent.GetSlot(i).GetConnected() == this as IConnectableElement)
                        return i;
            }
            return -1;
        }

        public void Restrict(float valueRestrict)
        {
            ConfigurableJoint joint = GetJoint();
            IConnectableElement parent = GetParent();
            if (joint && (parent != null) && (_motionSafe != ConfigurableJointMotion.Locked))
            {
                Limit limit = Limit.init(_lowSafe, _highSafe);
                if (valueRestrict > 0)
                {
                    float angle = parent.GetCurrentAngle();

                    limit.min = angle - valueRestrict;
                    limit.max = angle + valueRestrict;
                    joint.angularXMotion = ConfigurableJointMotion.Limited;
                }
                else joint.angularXMotion = _motionSafe;

                SoftJointLimit sl = joint.lowAngularXLimit;
                sl.limit = limit.min;
                joint.lowAngularXLimit = sl;

                SoftJointLimit hl = joint.highAngularXLimit;
                hl.limit = limit.max;
                joint.highAngularXLimit = hl;
            }
        }

        public Bounds getWorkingBounds()
        {
            if (_workingBounds) return _workingBounds.bounds;
            return new Bounds();
        }

        public Vector3 GetBaseVector()
        {
            ConfigurableJoint joint = GetJoint();
            if (joint)
                return Vector3.Cross(joint.axis, joint.secondaryAxis);
            return default;
        }

        public float GetCurrentAngle()
        {
            ConfigurableJoint joint = GetJoint();
            IJoinPoint mainSlot = GetMainSlot();
            if (joint && (mainSlot != null))
            {
                Vector3 localDirect = GetBaseVector();
                Vector3 direct = joint.transform.TransformDirection(localDirect);
                Vector3 baseDirect = mainSlot.Trans().rotation * localDirect;
                return Vector3.SignedAngle(direct, baseDirect, joint.transform.TransformDirection(joint.axis));
            }
            return 0;
        }
    }
}
