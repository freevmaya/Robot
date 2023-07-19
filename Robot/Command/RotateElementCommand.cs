using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Command;
using Vmaya.Language;

namespace Vmaya.Robot.Command
{
    public class RotateElementCommand : BaseCommand
    {
        [SerializeField]
        private Indent _elemIndent;
        [SerializeField]
        private float _startAngle;
        [SerializeField]
        private float _endAngle;
        protected Component element => _elemIndent.Find<Component>();
        private IRotationElement _rotation => element as IRotationElement;

        public RotateElementCommand()
        {
        }

        public RotateElementCommand(Indent elemIndent, float a_startAngle, float a_endAngle)
        {
            _elemIndent = elemIndent;
            _startAngle = a_startAngle;
            _endAngle = a_endAngle;
        }


        public override string commandName()
        {
            return Lang.instance["Rotate"];
        }

        public override bool execute()
        {
            _rotation.setAngle(_endAngle);
            return true;
        }

        public override void redo()
        {
            _rotation.setAngle(_endAngle);
        }

        public override void undo()
        {
            _rotation.setAngle(_startAngle);
        }
    }
}
