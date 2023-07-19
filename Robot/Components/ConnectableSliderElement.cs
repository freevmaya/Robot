using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Entity;

namespace Vmaya.Robot.Components
{
    public class ConnectableSliderElement : ConnectableElement
    {

        public override void Freeze(bool v)
        {
            RobotUtils.SliderFreeze(GetJoint(), GetMainSlot().Trans().position, getBaseDirect(), v);
            base.Freeze(v);
        }

        protected override void setSize(float value)
        {
            base.setSize(value);
            UpdateLimit();
        }

        public void UpdateLimit()
        {
            ConnectPointSlider point = GetMainSlot() as ConnectPointSlider;
            if (point)
            {
                SoftJointLimit jl = GetJoint().linearLimit;
                float scale = Trans().localScale.x;
                jl.limit = point.Range / 2f - point.fromGuide(getWorkingBounds().size * scale) / 2;
                GetJoint().linearLimit = jl;
            }
        }
    }
}
