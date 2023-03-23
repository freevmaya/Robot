using System.Collections.Generic;
using UnityEngine;
using Vmaya.Scene3D;

namespace Vmaya.Robot
{
    public interface IConnectableElement
    {
        public Transform Trans();
        public int SlotCount();
        public int NearestSlot(Vector3 position);
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