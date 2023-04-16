using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Catalog;
using Vmaya.Catalog.Commands;
using Vmaya.Catalog.Entity;
using Vmaya.Command;
using Vmaya.Entity;
using Vmaya.Language;
using Vmaya.Robot.Command;
using Vmaya.Robot.Components;
using Vmaya.RW;

namespace Vmaya.Robot.Catalog
{
    [RequireComponent(typeof(ConnectableElement))]
    public class ConnectableDropZone : MonoBehaviour, IDropLayer
    {
        [SerializeField]
        private Templates _compatibleList;
        [SerializeField]
        private EntityList _entityList;
        protected IConnectableElement _own => GetComponent<IConnectableElement>();

        protected CZones GroupZones => GetComponentInParent<CZones>();
        protected Transform Target => getTarget();

        protected virtual Transform getTarget()
        {
            return GroupZones ? GroupZones.transform : transform.parent;
        }

        public virtual ConnectableEntity createEndDock(string a_sourceName, string setName)
        {
            ConnectableEntity entity = null;
            Transform copy = Vmaya.Utils.FindChild<Transform>(Target, setName);
            if (!copy)
            {
                GameObject template = _entityList.PrefabForDropZone(a_sourceName);
                if (template && template.GetComponent<ConnectableEntity>())
                {
                    copy = Instantiate(template, Target).transform;
                    copy.name = setName;

                    Vector3 scale = copy.transform.localScale;
                    copy.transform.localScale = new Vector3(scale.x / Target.localScale.x, scale.y / Target.localScale.y, scale.z / Target.localScale.z);

                    BaseDropZone[] childDropZones = copy.GetComponentsInChildren<BaseDropZone>();
                    foreach (BaseDropZone dz in childDropZones)
                        if (!dz.entityList) dz.entityList = _entityList;
                    
                    entity = copy.GetComponent<ConnectableEntity>();

                    entity.sourceName = a_sourceName;
                    //entity.ResetConnected(this);
                }
                else if (Target == transform) Debug.LogError(Lang.instance.get("Template \"{0}\" not found", a_sourceName));
            }
            else if (Target == transform) Debug.LogError(Lang.instance.get("Duplicate name \"{0}\" found", setName));

            return entity;
        }

        public virtual void Docking(string a_sourceName, Vector3 point, Vector3 normal)
        {
            if (_own.SlotCount() > 0)
            {
                ConnectableEntity entity = createEndDock(a_sourceName, checkNextName(a_sourceName));
                if (entity)
                    CommandManager.ExecuteCmd(new CommandDockingConnectableEntity(this, entity, _own.FindFree(entity.Element.GetConnectType(), point)));
                else Debug.Log("Failed attempt to create entity");
            }
        }

        private string checkNextName(string a_sourceName)
        {
            if (GroupZones && GroupZones.NameProvider != null)
                return GroupZones.NameProvider.checkNextName(a_sourceName);

            return a_sourceName;
        }

        public Vector3 dockPoint(Vector3 hitPoint)
        {
            int idx = _own.NearestSlot(hitPoint);
            if (idx > -1)
                return _own.GetSlot(idx).Trans().position;

            return hitPoint;
        }

        public bool Compatibility(string a_sourceName)
        {
            bool result = false;
            GameObject prefab = _entityList.PrefabForDropZone(a_sourceName);
            if (prefab)
            {
                IConnectableElement prefabConnectable = prefab.GetComponent<IConnectableElement>();

                if (prefabConnectable != null)
                    result = (_own.FindFree(prefabConnectable.GetConnectType(), default) > -1) && (_compatibleList ? _compatibleList.Find(a_sourceName) : true);
            }

            return result;
        }
    }
}
