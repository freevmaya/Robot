using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Language;
using Vmaya.Robot.Components;

namespace Vmaya.Robot.Command
{
    public class DisconnectCommand : BaseConnectableEntityCommand
    {
        [SerializeField]
        private int _slot;

        [SerializeField]
        private Indent _parent;
        protected IConnectableElement element => entity.Element;
        protected IConnectableElement parent => _parent.FindInterface<IConnectableElement>();
        public DisconnectCommand(ConnectableEntity entity) : base(entity)
        {
            _slot = element.GetMainSlotIdx();
            _parent = new Indent(element.GetParent() as Component);
        }

        public override string commandName()
        {
            return Lang.instance["Disconnect"];
        }

        public override bool execute()
        {
            parent.Free(_slot);
            return true;
        }

        public override void redo()
        {
            execute();
        }

        public override void undo()
        {
            parent.toSlot(_slot, element);
        }
    }
}
