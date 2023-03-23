using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Components
{
    public class RobotAgent : Agent
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private Transform _sensor;
        [SerializeField]
        private Transform _robot;
        [SerializeField]
        private BoundsComponent _boundSpace;
        [SerializeField]
        private float _goalDistance;
        [SerializeField]
        private bool useVecObs;

        private RotationElement[] _elements;
        private float[] _saveAngle;
        private Vector3 _prevTargetPos;

        private bool HeuristicInput = false;


        private Vector3 direct => _sensor.position - _prevTargetPos;
        private float targetDistance => (_sensor.position - _target.position).magnitude;


        private void OnValidate()
        {
            if (_robot)
            {
                _elements = _robot.GetComponentsInChildren<RotationElement>();
            }
        }
        public override void CollectObservations(VectorSensor sensor)
        {
            if (useVecObs)
            {
                sensor.AddObservation(direct);
                sensor.AddObservation(targetDistance);
                for (int i = 0; i < _elements.Length; i++)
                    sensor.AddObservation(_elements[i].getAngle());

                _prevTargetPos = _sensor.position;
            }
        }

        private void saveAngles()
        {
            _saveAngle = new float[_elements.Length];
            for (int i = 0; i < _elements.Length; i++)
                _saveAngle[i] = _elements[i].getAngle();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

            int count = Mathf.Min(continuousActions.Length, _elements.Length);
            for (int i = 0; i < count; i++)
                continuousActions[i] = _elements[i].getAngle() - _saveAngle[i];
            /*
            continuousActions[0] = Input.GetAxisRaw("Horizontal");
            continuousActions[1] = Input.GetAxisRaw("Vertical");
            continuousActions[2] = Input.GetAxisRaw("Move");
            */

            /*
            continuousActions[0] = Random.Range(-1f, 1f);
            continuousActions[1] = Random.Range(-1f, 1f);
            continuousActions[2] = Random.Range(-1f, 1f);
            */

            HeuristicInput = true;
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            int count = Mathf.Min(actionBuffers.ContinuousActions.Length, _elements.Length);

            if (!HeuristicInput)
            {
                for (int i = 0; i < count; i++)
                {
                    float v = Mathf.Clamp(actionBuffers.ContinuousActions[i], -1f, 1f);
                    _elements[i].setAngle(_elements[i].getAngle() + v);
                }
                saveAngles();
            } else HeuristicInput = false;

            float distance = targetDistance;
            if (distance <= _goalDistance)
            {
                Debug.Log("Goal!");
                SetReward(1f);
                EndEpisode();
            }
            else if (direct.magnitude > 0) {

                Vector3 dt = (_sensor.position - _target.position).normalized;
                Vector3 dn = direct.normalized;

                Debug.DrawRay(_target.position, dt, Color.red);
                Debug.DrawRay(_sensor.position, dn);

                float d = Vector3.Dot(dn, dt);
                if (d < -0.5f)
                {
                    SetReward(0.1f);
                }
                else if (d > 0.5f)
                {
                    SetReward(-0.1f);
                }
                for (int i = 0; i < count; i++)
                    if (_elements[i].Threshold)
                    {
                        SetReward(-0.02f);
                        break;
                    }

                if (GetCumulativeReward() < -1)
                {
                    Debug.Log("Wrong!");
                    SetReward(-1f);
                    EndEpisode();
                }
                //Debug.Log(GetCumulativeReward());
            }
        }

        public override void Initialize()
        {
            ResetScene();
        }

        private void ResetScene()
        {
            int count = 0;
            do
            {
                foreach (RotationElement elem in _elements)
                    elem.setAngle(elem.getAngleLimiter().Random());

                count++;
                if (count > 50) break;
            } while (!_boundSpace.Contained(_target.position));

            saveAngles();
            _prevTargetPos = _sensor.position;

            Bounds bounds = _boundSpace.bounds;

            _target.position = _boundSpace.transform.TransformPoint(new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
             ));
        }

        public override void OnEpisodeBegin()
        {
            ResetScene();
        }


    }
}
