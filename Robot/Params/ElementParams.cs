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
        [HideInInspector]
        [SerializeField]
        private float _size;
        protected ConnectableElement element => GetComponent<ConnectableElement>();

        public void addChangeListener(UnityAction listener)
        {
            element.OnChange.AddListener(listener);
        }

        public void removeChangeListener(UnityAction listener)
        {
            element.OnChange.RemoveListener(listener);
        }

        public bool getActive()
        {
            return element.isActiveAndEnabled;
        }

        public virtual void setJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
            element.Size = _size;
        }

        public string getJson()
        {
            return JsonUtility.ToJson(this);
        }

        public virtual List<Parameter> getParams()
        {
            List<Parameter> list = new List<Parameter>();

            list.Add(new Parameter("_size", Lang.instance["size"], TypeParam.TFloat, _size = element.Size, element.SizeLimit));

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

        protected virtual void updateParam(Parameter value)
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
