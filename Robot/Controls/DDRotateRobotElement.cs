using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Robot.Components;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Controls
{
    [RequireComponent(typeof(IConnectableElement))]
    public class DDRotateRobotElement : DragDropRotate
    {
        protected IConnectableElement _element => GetComponent<IConnectableElement>();

        [SerializeField]
        private bool _freezeTop = false;

        protected override void beginDrag()
        {
            base.beginDrag();
            if (_freezeTop) setFreezeTop(_element, true);

            if (ProtractorTool.instance && (_element.GetMainSlot() != null))
                ProtractorTool.instance.Show(_element.GetMainSlot());
        }

        protected override void Drop()
        {
            base.Drop();
            if (_freezeTop) setFreezeTop(_element, false);

            if (ProtractorTool.instance)
                ProtractorTool.instance.Hide();
        }

        private static void setFreezeTop(IConnectableElement a_element, bool value)
        {

            for (int i=0; i< a_element.SlotCount(); i++)
            {
                IConnectableElement ce = a_element.GetSlot(i).GetConnected();
                if (ce != null)
                {
                    ce.Restrict(value ? 5 : 0);
                    setFreezeTop(ce, value);
                }
            }
        }
    }
}
