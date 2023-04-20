using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Entity;
using Vmaya.Robot.Catalog;
using Vmaya.Robot.Components;

namespace Vmaya.Robot.Entity
{
    [RequireComponent(typeof(ConnectableElement))]
    public class ConnectableEntity : BaseEntity
    {
        public ConnectableElement Element => GetComponent<ConnectableElement>();

        [System.Serializable]
        protected class CEData
        {
            public float size;
            public string[] slotsData;
        }

        public override string getJson()
        {
            CEData data = new CEData();
            data.size = Element.Size;
            data.slotsData = new string[Element.SlotCount()];
            for (int i = 0; i < Element.SlotCount(); i++)
                data.slotsData[i] = Element.GetSlot(i).getJson();
            return JsonUtility.ToJson(data);
        }

        public override void SetRecord(entityRecord record)
        {
            CEData data = JsonUtility.FromJson<CEData>(record.json_params);
            Element.Size = data.size;
            base.SetRecord(record);
        }

        public override void setJson(string json_params)
        {
            CEData data = JsonUtility.FromJson<CEData>(json_params);

            for (int i = 0; i < data.slotsData.Length; i++)
                if (!string.IsNullOrEmpty(data.slotsData[i]))
                    Element.GetSlot(i).setJson(data.slotsData[i]);
            /*
            if (!Indent.isNull(data.parent))
            {
                IConnectableElement parent = data.parent.Find() as IConnectableElement;
                parent.toSlot(data.idxSlot, Element);
            }
            */
        }
    }
}