using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Components
{
    public abstract class BaseFinger : MonoBehaviour
    {
        public abstract void setOpenness(float value);
    }
}
