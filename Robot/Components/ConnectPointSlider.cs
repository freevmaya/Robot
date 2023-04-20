using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Components
{
    [ExecuteInEditMode]
    public class ConnectPointSlider : ConnectPoint
    {
        [System.Serializable]
        protected class PointSliderRecord: PointRecord
        {
            public float Range;
        }

        [System.Serializable]
        public enum DirectGuide { X, Y, Z };
        [System.Serializable]
        public enum Align { Left, Center, Right };

        [SerializeField]
        private Limit _rangeLimit;

        [SerializeField]
        private float _range;
        private float _rangeLast;
        public float Range { get => _range; set => SetRange(value); }

        [SerializeField]
        private Transform _guides;

        [SerializeField]
        private Transform _capLeft;

        [SerializeField]
        private Transform _capRight;

        [SerializeField]
        private float _limitLeft;

        [SerializeField]
        private float _limitRight;

        [SerializeField]
        private DirectGuide _guideDirect = DirectGuide.Z;

        [SerializeField]
        private Align _align = Align.Center;

        private Vector3 _startPosCap;
        private Vector3 _startPos;

        protected override void OnValidate()
        {
            _range = _rangeLimit.Clamp(_range);
            base.OnValidate();
            safeStartPositions();
        }

        public Limit getRangeLimit()
        {
            Limit result = _rangeLimit;
            if (Connected != null)
            {
                float minFromGuide = fromGuide(Connected.getWorkingBounds().size) / 2;
                result.min = Mathf.Max(result.min, minFromGuide);
                result.max = Mathf.Max(result.max, result.min);
            }
            return result;
        }

        protected virtual void Awake()
        {
            safeStartPositions();
        }

        protected Vector3 toGuide(Vector3 v, float value)
        {
            switch (_guideDirect)
            {
                case DirectGuide.Z: 
                    v.z = value;
                    break;
                case DirectGuide.X:
                    v.x = value;
                    break;
                case DirectGuide.Y:
                    v.y = value;
                    break;
            }
            return v;
        }

        protected float fromGuide(Vector3 v)
        {
            switch (_guideDirect)
            {
                case DirectGuide.Z: return v.z;
                case DirectGuide.X: return v.x;
                case DirectGuide.Y: return v.y;
            }
            return 0;
        }

        internal void SetRange(float value)
        {
            if (value != _range)
            {
                _range = value;
                updateRange();
            }
        }

        private void safeStartPositions()
        {
            if (_capRight) _startPosCap = _capRight.localPosition;
            _startPos = transform.localPosition;
        }

        public override Vector3 getWorldConnectPoint()
        {
            return base.getWorldConnectPoint() + Connected.getWorkingBounds().center;
        }

        protected override void updateConnected(bool resetRotateAndScale = true)
        {
            base.updateConnected(resetRotateAndScale);
            SoftJointLimit jl = Connected.GetJoint().linearLimit;
            jl.limit = Range / 2f - fromGuide(Connected.getWorkingBounds().size) / 2;
            Connected.GetJoint().linearLimit = jl;
        }

        protected bool checkChange()
        {
            return _rangeLast != _range;
        }

        private void Update()
        {
            if (checkChange()) updateRange();
        }

        private void updateRange()
        {
            float allLimit = _limitLeft + _limitRight;
            if (_guides)
            {
                _guides.localScale = toGuide(new Vector3(1, 1, 1), (allLimit + Range) / 2);
                switch (_align)
                {
                    case Align.Left: 
                        _guides.localPosition = Vector3.zero;
                        break;
                    case Align.Center:
                        _guides.localPosition = toGuide(Vector3.zero, -Range / 2 - _limitLeft);
                        break;
                    case Align.Right:
                        _guides.localPosition = toGuide(Vector3.zero, -Range - allLimit);
                        break;
                }
            }

            if (_capLeft)
            {
                switch (_align)
                {
                    case Align.Left:
                        _capLeft.localPosition = Vector3.zero;
                        break;
                    case Align.Center:
                        _capLeft.localPosition = toGuide(Vector3.zero, -Range / 2 - _limitLeft);
                        break;
                    case Align.Right:
                        _capLeft.localPosition = toGuide(Vector3.zero, -Range - allLimit);
                        break;
                }
            }

            if (_capRight)
            {
                switch (_align)
                {
                    case Align.Left:
                        _capRight.localPosition = toGuide(Vector3.zero, Range + allLimit);
                        break;
                    case Align.Center:
                        _capRight.localPosition = toGuide(Vector3.zero, Range / 2 + _limitRight);
                        break;
                    case Align.Right:
                        _capRight.localPosition = Vector3.zero;
                        break;
                }
            }

            if (!((_capLeft && transform.IsChildOf(_capLeft)) ||
                (_capRight && transform.IsChildOf(_capRight)))) {
                switch (_align)
                {
                    case Align.Left:
                        transform.localPosition = toGuide(_startPos, _limitLeft + Range / 2);
                        break;
                    case Align.Center:
                        transform.localPosition = toGuide(_startPos, 0);
                        break;
                    case Align.Right:
                        transform.localPosition = toGuide(_startPos, -(_limitRight + Range / 2));
                        break;
                }
            }

            _rangeLast = _range;

            UpdateConnectedPosition();
        }

        protected override PointRecord createPointRecord()
        {
            return new PointSliderRecord();
        }

        protected override PointRecord getRestoreData()
        {
            PointSliderRecord result = base.getRestoreData() as PointSliderRecord;
            result.Range = Range;
            return result;
        }

        protected override void restoreFromData(PointRecord pr)
        {
            PointSliderRecord psr = pr as PointSliderRecord;
            Range = psr.Range;
            base.restoreFromData(pr);
        }
    }
}
