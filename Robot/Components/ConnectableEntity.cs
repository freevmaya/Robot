using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Entity;
using Vmaya.Robot.Catalog;
using Vmaya.Robot.Components;

namespace Vmaya.Robot
{
    [RequireComponent(typeof(ConnectableElement))]
    public class ConnectableEntity : BaseEntity
    {
        public ConnectableElement Element => GetComponent<ConnectableElement>();

        [System.Serializable]
        protected class CEData
        {
            public Indent parent;
            public int idxSlot;
            public float size;
        }

        public override string getJson()
        {
            CEData data = new CEData();
            data.parent = new Indent(Element.GetParent() as Component);
            data.idxSlot = Element.GetMainSlotIdx();
            data.size = Element.Size;
            return JsonUtility.ToJson(data);
        }

        public override void SetRecord(entityRecord record)
        {
            CEData data = JsonUtility.FromJson<CEData>(record.json_params);

            if (!Indent.isNull(data.parent))
            {
                Vmaya.Utils.PendingCondition(this, () =>
                {
                    return data.parent.FindInterface<IConnectableElement>() != null;
                }, () =>
                {
                    base.SetRecord(record);
                    Element.Size = data.size;
                });
            }
            else base.SetRecord(record);
        }

        public override void setJson(string json_params)
        {
            CEData data = JsonUtility.FromJson<CEData>(json_params);
            if (!Indent.isNull(data.parent))
            {
                IConnectableElement parent = data.parent.FindInterface<IConnectableElement>();
                parent.toSlot(data.idxSlot, Element);
            }
        }
    }
}