using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Command;
using Vmaya.Robot.Command;
using Vmaya.Robot.Components;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Controls
{
    [RequireComponent(typeof(IConnectableElement))]
    public class DDRotateRobotElement : DragDropRotate
    {
        protected IConnectableRotationElement _element => GetComponent<IConnectableRotationElement>();

        protected override void saveStartData()
        {
            base.saveStartData();
            _startAngle = _element.getAngle();
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

            float andAngle = getDeltaAngle();
            if (CommandManager.instance)
                CommandManager.instance.executeCmd(new RotateElementCommand(new Indent(_element as Component), _startAngle, andAngle));

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
