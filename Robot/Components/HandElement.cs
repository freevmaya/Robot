using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Components
{
    //[ExecuteInEditMode]
    public class HandElement : ConnectableRotateElement
    {
        [SerializeField]
        private BaseFinger[] _fingers;
        [SerializeField]
        [Range(0, 1)]
        private float _openness;
        private float _lastOpenness;

        public float Openness { get => _openness; set => setOpenness(value); }

        protected void setOpenness(float value)
        {
            if (_lastOpenness != value)
            {
                _lastOpenness = _openness = value;
                updateOpenness();
                OnChange.Invoke();
            }
        }

        protected virtual void updateOpenness()
        {
            foreach (BaseFinger _finger in _fingers)
                if (_finger)
                    _finger.setOpenness(_openness);
        }

        protected bool checkChange()
        {
            return _openness != _lastOpenness;
        }

        private void Update()
        {
            if (checkChange()) updateHand();
        }

        private void updateHand()
        {
            setOpenness(_openness);
        }
    }
}
