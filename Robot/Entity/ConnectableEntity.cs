using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Entity;
using Vmaya.Params;
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
            public string[] slotsData;
            public string paramsData;
        }

        public override string getJson()
        {
            CEData data = new CEData();
            data.slotsData = new string[Element.SlotCount()];
            for (int i = 0; i < Element.SlotCount(); i++)
                data.slotsData[i] = Element.GetSlot(i).getJson();

            IParamProvider pp = GetComponent<IParamProvider>();
            if (pp != null) data.paramsData = pp.getJson();

            return JsonUtility.ToJson(data);
        }

        public override void setJson(string json_params)
        {
            CEData data = JsonUtility.FromJson<CEData>(json_params);

            for (int i = 0; i < data.slotsData.Length; i++)
                if (!string.IsNullOrEmpty(data.slotsData[i]))
                    Element.GetSlot(i).setJson(data.slotsData[i]);

            IParamProvider pp = GetComponent<IParamProvider>();
            if (pp != null) pp.setJson(data.paramsData);
        }
    }
}