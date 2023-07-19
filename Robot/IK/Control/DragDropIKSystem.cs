using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Robot.Components;
using Vmaya.Scene3D;
using Vmaya.Util;
using MyBox;

namespace Vmaya.Robot.IK.Control
{
    [RequireComponent(typeof(RotationElement))]
    public class DragDropIKSystem : BaseDragDrop
    {
        private Vector3 _startPos;

        [SerializeField]
        private IKSystemOther _system;

        [SerializeField]
        private bool _adaptivePlane;

        [ConditionalField("_adaptivePlane", true)] [SerializeField] private Vector3 _planeNormal;

        private List<IChainLink> chain;
        private Plane _dragPlane;

        private Vector3 getMousePosition()
        {
            Ray ray = hitDetector.mRay();
            float enter;

            if (_dragPlane.Raycast(ray, out enter))
                return ray.GetPoint(enter);

            return transform.position;
        }

        public override void doMouseDown()
        {
            base.doMouseDown();
            _startPos = VMouse.mousePosition;
            if (_adaptivePlane)
                _dragPlane = new Plane((transform.position - CameraManager.getCurrent().transform.position).normalized, transform.position);
            else _dragPlane = new Plane(_planeNormal, transform.position);

            Debug.Log(_dragPlane.normal);
        }

        protected override void beginDrag()
        {
            base.beginDrag();
            _startPos = getMousePosition();

            chain = GetComponent<IChainLink>().GetChain();
            /*
            List<RotationElement> inverse = GetComponent<RotationElement>().getChain();
            chain = new List<RotationElement>();
            for (int i = 0; i < inverse.Count; i++)
                chain.Insert(chain.Count, inverse[i]);
            */
        }

        protected override void doDrag()
        {
            //Vector3 delta = getMousePosition() - _startPos;
            Vector3 target = getMousePosition();

            //_system.InverseKinematics(chain, target);
        }

        protected override bool Dragging()
        {
            return (_startPos - VMouse.mousePosition).magnitude > 1;
        }
    }
}
