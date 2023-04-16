using System.Collections.Generic;
using UnityEngine;
using Vmaya.Scene3D;

namespace Vmaya.Robot
{
    [System.Serializable]
    public enum ConnectType { HingeOut, HingeIn, HingeSphere, Sliding, Dovetail };

    public interface IJoinElement
    {
        public ConnectType GetConnectType();
        public Transform Trans();
    }

    public interface IJoinPoint : IJoinElement
    {
        public IConnectableElement GetConnected();
        public void SetConnect(IConnectableElement elem);
    }

    public interface IConnectableElement: IJoinElement
    {
        public int SlotCount();
        public IJoinPoint GetSlot(int idx);
        public IJoinPoint GetMainSlot();
        public int GetMainSlotIdx();
        public int NearestSlot(Vector3 position);
        public void toSlot(int slot, IConnectableElement other);
        public bool isFree(int slot);
        public void Free(int slot);
        public int FindFree(ConnectType a_type, Vector3 point);
        public ConfigurableJoint GetJoint();
        public Rigidbody GetRigidBody();
        public IConnectableElement GetParent();
        public void Restrict(float valueRestrict);
        public Bounds getWorkingBounds();
        public float GetCurrentAngle();
        public Vector3 GetBaseVector();
    }

    public interface IChainLink
    {
        IChainLink GetParent();
        List<IChainLink> GetChain();
        Vector3 GetPosition();
    }

    public interface IRotationElement: IChainLink, IRotatable
    {
        Vector3 getBaseDirect();
        Limit getAngleLimiter();
    }

    public class Utils
    {
        public static void DrawArrowGismo(Transform trans, Vector3 localDirect)
        {
            float size = localDirect.magnitude;

            Quaternion q = Quaternion.LookRotation(localDirect);
            Vector3 f = Vector3.forward;


            Vector3 tp = trans.position + q * f * size;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(trans.position, tp);
            Gizmos.DrawLine(tp, tp - q * (f + Vector3.left * 0.5f) * size * 0.2f);
            Gizmos.DrawLine(tp, tp - q * (f - Vector3.left * 0.5f) * size * 0.2f);
        }
    }

}