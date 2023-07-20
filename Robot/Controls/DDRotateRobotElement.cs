using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Command;
using Vmaya.Entity.Commands;
using Vmaya.Robot.Command;
using Vmaya.Robot.Components;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Controls
{
    [RequireComponent(typeof(IConnectableElement))]
    public class DDRotateRobotElement : DragDropRotate
    {
        protected IConnectableRotationElement _element => GetComponent<IConnectableRotationElement>();

        private JsonObjectCommand _command;

        protected override void saveStartData()
        {
            base.saveStartData();
            _startAngle = _element.getAngle();

            if (CommandManager.instance)
                _command = new JsonObjectCommand(_element);
        }

        protected override void beginDrag()
        {
            base.beginDrag();

            _element.Freeze(false);

            if (ProtractorTool.instance && (_element.GetMainSlot() != null))
                ProtractorTool.instance.Show(_element.GetMainSlot());
        }

        protected override void Drop()
        {
            base.Drop();

            if (CommandManager.instance) 
                CommandManager.instance.executeCmd(_command);

            _element.Freeze(true);

            if (ProtractorTool.instance)
                ProtractorTool.instance.Hide();
        }

        protected override void doDrag()
        {
            _element.aimAngle(getDeltaAngle());
        }
    }
}
