using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Robot.Components;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Controls
{
    [RequireComponent(typeof(IConnectableElement))]
    public class DragDropRobotElement : DragDropRB
    {

        protected IConnectableElement element => GetComponent<IConnectableElement>();

        protected override void beginDrag()
        {
            base.beginDrag();
            freezeParents(false);
        }

        protected override void Drop()
        {
            base.Drop();
            freezeParents(true);
        }

        protected void freezeParents(bool value)
        {
            List<IChainLink> chain = element.GetChain();
            foreach (IChainLink link in chain)
            {
                if (link is IConnectableElement)
                    (link as IConnectableElement).Freeze(value);
            }
        }
    }
}
