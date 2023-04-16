using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vmaya.Language;
using Vmaya.Params;
using Vmaya.Robot.Components;

namespace Vmaya.Robot.Params
{
    [RequireComponent(typeof(ConnectableElement))]
    public class ElementParams : MonoBehaviour, IParamProvider
    {
        protected ConnectableElement element => GetComponent<ConnectableElement>();

        public void addChangeListener(UnityAction listener)
        {
        }

        public bool getActive()
        {
            return element.isActiveAndEnabled;
        }

        public void setJson(string json)
        {
        }

        public string getJson()
        {
            return null;
        }

        public List<Parameter> getParams()
        {
            List<Parameter> list = new List<Parameter>();

            list.Add(new Parameter("_size", Lang.instance["size"], TypeParam.TFloat, element.Size, element.SizeLimit));
            list.Add(new Parameter("_position", Lang.instance["position"], TypeParam.TFloat, 0));

            return list;
        }

        public IParamProvider getProvider()
        {
            return this;
        }

        public string getShortInfo()
        {
            return Lang.instance["Connectable element"];
        }

        public string getTitle()
        {
            return Lang.instance["Connectable element"];
        }

        public void removeChangeListener(UnityAction listener)
        {
        }

        protected void updateParam(Parameter value)
        {
            switch (value.name)
            {
                case "_size":
                    element.Size = value.NumberValue;
                    break;
            }
        }

        public void setFinal(Parameter value)
        {
            updateParam(value);
        }

        public void setPrep(Parameter value)
        {
            updateParam(value);
        }

        public void setStart(Parameter value)
        {
        }
    }
}
