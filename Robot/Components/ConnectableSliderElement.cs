using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vmaya.Robot.Components
{
    public class ConnectableSliderElement : ConnectableElement
    {

        public override void Freeze(bool v)
        {
            ConfigurableJoint joint = GetJoint();

            if (joint && joint.connectedBody)
            {
                joint.autoConfigureConnectedAnchor = false;

                Vector3 worldAnchor;
                if (v)
                {
                    worldAnchor = transform.position - joint.connectedBody.transform.position;
                }
                else
                {
                    IJoinPoint point = GetMainSlot();
                    worldAnchor = point.Trans().position - joint.connectedBody.transform.position;
                }

                Vector3 direct = getBaseDirect();

                joint.connectedAnchor = joint.connectedBody.transform.InverseTransformVector(worldAnchor);

                ConfigurableJointMotion lv = v ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Limited;
                if (direct.z > 0.5f) joint.zMotion = lv;
                else if (direct.x > 0.5f) joint.xMotion = lv;
                else if (direct.y > 0.5f) joint.yMotion = lv;
            }
            base.Freeze(v);
        }
    }
}
