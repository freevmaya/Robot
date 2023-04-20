using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Robot.Components;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Controls
{
    [RequireComponent(typeof(IConnectableElement))]
    public class DragDropSliderElement : DragDropRB
    {
        protected IConnectableElement _element => GetComponent<IConnectableElement>();

        protected override void beginDrag()
        {
            base.beginDrag();
            _element.Freeze(false);
        }

        protected override void Drop()
        {
            base.Drop();
            _element.Freeze(true);
        }
    }
}
