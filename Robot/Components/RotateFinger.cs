using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Components
{
    public class RotateFinger : BaseFinger
    {
        public Transform Model;
        public Limit AngleLimit;
        public Vector3 Axis = Vector3.left;

        protected override void moveStep(float speed, bool checkPlace = true) 
        {
            transform.localRotation = Quaternion.AngleAxis(Mathf.Lerp(AngleLimit.min, AngleLimit.max, opennessTo), Axis);
        }
    }
}
