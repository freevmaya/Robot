using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Robot.Components;

namespace Vmaya.Robot.IK
{
    public class IKSystemOther : MonoBehaviour
    {
        private List<IRotationElement> _joints;

        public float SamplingDistance = 0.1f;
        public float DistanceThreshold = 0.1f;
        public float LearningRate = 0.1f;

        [SerializeField]
        private Transform _debugMarker;

        public Vector3 ForwardKinematics(float[] angles)
        {
            Vector3 prevPoint = (_joints[0] as Component).transform.position;
            Quaternion rotation = Quaternion.identity;
            for (int i = 1; i < _joints.Count; i++)
            {
                // Rotates around a new axis
                rotation *= Quaternion.AngleAxis(angles[i - 1], _joints[i - 1].getAxis());
                Vector3 nextPoint = prevPoint + rotation * (_joints[i] as RotationElement).StartOffset;

                prevPoint = nextPoint;
            }
            return prevPoint;
        }

        public float DistanceFromTarget(Vector3 target, float[] angles)
        {
            Vector3 point = ForwardKinematics(angles);
            return Vector3.Distance(point, target);
        }

        public float PartialGradient(Vector3 target, float[] angles, int i)
        {
            float angle = angles[i];

            float f_x = DistanceFromTarget(target, angles);
            angles[i] += SamplingDistance;
            float f_x_plus_d = DistanceFromTarget(target, angles);
            float gradient = (f_x_plus_d - f_x) / SamplingDistance;

            angles[i] = angle;
            return gradient;
        }

        public void InverseKinematics(Vector3 target, float[] angles)
        {
            if (DistanceFromTarget(target, angles) < DistanceThreshold)
                return;

            for (int i = _joints.Count - 1; i >= 0; i--)
            {
                float gradient = PartialGradient(target, angles, i);
                angles[i] -= LearningRate * gradient;
                if (DistanceFromTarget(target, angles) < DistanceThreshold)
                    return;
            }
        }

        private void DebugMarker(Vector3 pos)
        {
            if (_debugMarker)
                _debugMarker.transform.position = pos;
        }

        internal void InverseKinematics(List<IRotationElement> a_chain, Vector3 target)
        {
            _joints = a_chain;

            float[] angles = new float[_joints.Count];
            for (int i = 0; i < _joints.Count; i++)
                angles[i] = _joints[i].getAngle();

            //Utils.debugPoint(target, 0.1f);
            DebugMarker(target);
            if (_debugMarker)
                _debugMarker.transform.position = target;
            InverseKinematics(target, angles);

            for (int i = 0; i < _joints.Count; i++)
                _joints[i].setAngle(angles[i]);
        }
    }
}
