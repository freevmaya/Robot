using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Robot.Components;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Controls
{
    public class IKTarget : MonoBehaviour
    {
        [SerializeField]
        private Component _topElement;
        [SerializeField]
        private float _tolerace = 0.1f;
        public IChainLink TopElement => _topElement ? _topElement.GetComponent<IChainLink>() : null;

        private List<IChainLink> _joints;

        private Vector3 _prevPos;

        private float[] angles;
        private List<IRotationElement> reList;

        private void OnValidate()
        {
            _topElement = TopElement as Component;
        }

        private void Update()
        {
            Vector3 pos = transform.position;
            if (!pos.Equals(_prevPos))
            {
                if (_topElement.GetComponent<IRotationElement>() != null)
                    RefreshRotation();
                else RefreshChain();

                _prevPos = pos;
            }

            if (reList != null)
            {
                for (int i = 0; i < angles.Length; i++)
                    reList[i].setAngle(Mathf.LerpAngle(reList[i].getAngle(), angles[i], 0.1f));
            }
        }

        private void RefreshRotation()
        {
            _joints = _topElement.GetComponent<IChainLink>().GetChain();

            reList = new List<IRotationElement>();
            for (int i = 0; i < _joints.Count; i++)
                reList.Add(_joints[i] as IRotationElement);

            Vector3[] points = IKSystem.NodesFabrik(reList, 0, reList.Count, out angles, transform.position);

            for (int i = 0; i < points.Length; i++)
                Vmaya.Utils.debugPoint(points[i], 0.5f, Color.black);
        }

        private void RefreshChain()
        {
            _joints = _topElement.GetComponent<IChainLink>().GetChain();

            Vector3[] p = new Vector3[_joints.Count];
            Vector3[] axis = new Vector3[_joints.Count];
            float[,] limits = new float[_joints.Count, 2];

            for (int i = 0; i < _joints.Count; i++)
            {
                p[i] = (_joints[i] as Component).transform.position;
                axis[i] = Vector3.left;// Vector3.Lerp(Vector3.right, Vector3.forward, (1 - (float)i / (_joints.Count - 1)));
                limits[i, 0] = -90;
                limits[i, 1] = 90;
            }

            IKSystem.SimpleFabrik2(ref p, axis, limits, transform.position, (_joints[0] as Component).transform.up, _tolerace);

            for (int i = 0; i < p.Length - 1; i++)
            {
                Transform t = (_joints[i] as Component).transform;
                Vector3 direct = p[i + 1] - p[i];
                t.position = p[i];
                t.rotation = Quaternion.LookRotation(direct.normalized);
                Debug.DrawLine(p[i], p[i + 1]);
            }
        }
    }
}
