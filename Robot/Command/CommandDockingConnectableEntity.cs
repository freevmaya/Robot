using System;
using UnityEngine;
using Vmaya.Command;
using Vmaya.Language;
using Vmaya.Robot.Catalog;
using Vmaya.Robot.Entity;

namespace Vmaya.Robot.Command
{
    internal class CommandDockingConnectableEntity : BaseConnectableEntityCommand
    {
        [SerializeField]
        private Indent _dropZoneIndent;
        [SerializeField]
        private int _idx;
        private ConnectableDropZone connectableDropZone => _dropZoneIndent.Find<ConnectableDropZone>();
        private IConnectableElement _own => connectableDropZone.GetComponent<IConnectableElement>();

        public CommandDockingConnectableEntity()
        {
        }

        public CommandDockingConnectableEntity(ConnectableDropZone connectableDropZone, ConnectableEntity entity, int idx): base (entity)
        {
            _dropZoneIndent = new Indent(connectableDropZone);
            _idx = idx;
        }

        public override string commandName()
        {
            return Lang.instance["Docking Connectable Entity"];
        }

        public override bool execute()
        {
            _own.toSlot(_idx, entity.Element);
            return true;
        }

        public override void redo()
        {
            entity.gameObject.SetActive(true);
            _own.toSlot(_idx, entity.Element);
        }

        public override void undo()
        {
            entity.gameObject.SetActive(false);
        }

        public override void destroy()
        {
            if (!Vmaya.Utils.IsDestroyed(entity) && !entity.gameObject.activeSelf) 
                GameObject.Destroy(entity.gameObject);
        }
    }
}