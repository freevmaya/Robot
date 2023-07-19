using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Components
{
    public abstract class BaseFinger : MonoBehaviour
    {
        protected float openness => Hand.Openness;
        private float _opennessTo;
        protected float opennessTo => _opennessTo;
        public HandElement Hand => GetComponentInParent<HandElement>();

        public virtual Collider getTouch()
        {
            return null;
        }

        protected virtual void moveStep(float speed, bool checkPlace = true)
        {
            _opennessTo = Mathf.LerpUnclamped(_opennessTo, openness, speed);
        }

        private void Start()
        {
            moveStep(1f, false);
        }

        private void Update()
        {
            if (_opennessTo != openness) moveStep(0.25f, true);
        }
    }
}
