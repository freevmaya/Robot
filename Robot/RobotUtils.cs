using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Entity;

namespace Vmaya.Robot
{
    public class RobotUtils
    {
        public static void DelayPhysics()
        {
            if (Physics.autoSimulation)
            {
                Physics.autoSimulation = false;
                AppManager.afterEndOfFrame(() => { Physics.autoSimulation = true; });
            }
        }

        public static void DelayPhysics(float sec)
        {
            if (Physics.autoSimulation)
            {
                Physics.autoSimulation = false;
                AppManager.setTimeout(() => { Physics.autoSimulation = true; }, sec);
            }
        }

        public static void SliderFreeze(ConfigurableJoint joint, Vector3 mountPoint, Vector3 direct, bool v)
        {
            if (joint && joint.connectedBody)
            {
                joint.autoConfigureConnectedAnchor = v;

                ConfigurableJointMotion lv = v ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Limited;
                if (direct.z > 0.5f) joint.zMotion = lv;
                else if (direct.x > 0.5f) joint.xMotion = lv;
                else if (direct.y > 0.5f) joint.yMotion = lv;

                if (!v)
                {
                    Vector3 worldAnchor = mountPoint - joint.connectedBody.transform.position;
                    joint.connectedAnchor = joint.connectedBody.transform.InverseTransformVector(worldAnchor);
                }
            }
        }
    }
}
