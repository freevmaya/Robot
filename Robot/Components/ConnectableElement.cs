using Vmaya.Entity;
using UnityEngine;
using System.Collections.Generic;
using System;
using Vmaya.Scene3D;
using UnityEngine.Events;

namespace Vmaya.Robot.Components
{
    //[ExecuteInEditMode]
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

        public UnityEvent OnChange;

        public IJoinPoint this[int index] => _connectPoints[index] ? _connectPoints[index].GetComponent<IJoinPoint>() : null;

        public float Size { get => getSize(); set => setSize(value); }
        public Limit SizeLimit = Limit.init(0, 2);

        private void OnValidate()
        {
            if (!_myJoint) _myJoint = GetComponent<ConfigurableJoint>();
            if (!_myRigidBody) _myRigidBody = GetComponent<Rigidbody>();
            if (!_workingBounds) _workingBounds = GetComponent<BoundsComponent>();

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
            if ((value > 0) && (Size != value))
            {
                RobotUtils.DelayPhysics();
                Vector3 scale = new Vector3(value, value, value);
                transform.localScale = scale;
                UpdateConnectPointsPosition();

                OnChange.Invoke();
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
            Vmaya.Utils.afterEndOfFrame(this, () =>
            {
                other.Freeze(true);
            });
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

        public Bounds getWorkingBounds()
        {
            if (_workingBounds) return _workingBounds.bounds;
            return new Bounds();
        }

        public Vector3 getBaseDirect()
        {
            ConfigurableJoint joint = GetJoint();
            if (joint)
                return Vector3.Cross(joint.axis, joint.secondaryAxis);
            return default;
        }

        public List<IChainLink> GetChain()
        {
            List<IChainLink> result = new List<IChainLink>();

            IChainLink top = this;
            while (top != null)
            {
                result.Add(top);
                top = top.GetParentLink();
            }

            return result;
        }

        public List<IChainLink> GetChildren()
        {
            List<IChainLink> result = new List<IChainLink>();
            for (int i=0; i<SlotCount(); i++)
            {
                IConnectableElement ce = GetSlot(i).GetConnected();
                if (ce != null) result.Add(ce);
            }

            return result;
        }

        public IChainLink GetParentLink()
        {
            return GetJoint() && (GetJoint().connectedBody != null) ? GetJoint().connectedBody.GetComponent<IChainLink>() : null;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public virtual void Freeze(bool v)
        {
            /*
            Rigidbody body = GetRigidBody();
            if (body) GetRigidBody().useGravity = v;

            for (int i = 0; i < SlotCount(); i++)
                if (GetSlot(i).GetConnected() != null)
                    GetSlot(i).GetConnected().Freeze(v);
            */
        }

        public void setJson(string jsonData)
        {
            this.SetTransformChainData(JsonUtility.FromJson<TranformChainData>(jsonData));
        }

        public string getJson()
        {
            return JsonUtility.ToJson(this.GetTransformChainData());
        }

        public TranformChainData GetTransformChainData()
        {
            Transform trans = transform;
            TranformChainData result = new TranformChainData();
            result.position = trans.position;
            result.scale = trans.localScale;
            result.rotation = trans.rotation;

            result.children = new List<TranformChainData>();
            List<IChainLink> children = GetChildren();
            foreach (IChainLink cl in children)
                result.children.Add(cl.GetTransformChainData());

            return result;
        }

        public virtual void SetTransformChainData(TranformChainData tcd)
        {
            transform.position = tcd.position;
            transform.localScale = tcd.scale;
            transform.rotation = tcd.rotation;

            if (tcd.children != null)
            {
                List<IChainLink> children = GetChildren();
                for (int i = 0; i < tcd.children.Count; i++)
                    children[i].SetTransformChainData(tcd.children[i]);
            }
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
