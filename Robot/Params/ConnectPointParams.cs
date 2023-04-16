using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vmaya.Language;
using Vmaya.Params;
using Vmaya.Robot.Components;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Params
{
    [RequireComponent(typeof(ConnectPointSlider))]
    public class ConnectPointParams : MonoBehaviour, IParamProvider
    {
        protected ConnectPointSlider _point => GetComponent<ConnectPointSlider>();

        public bool getActive()
        {
            return isActiveAndEnabled;
        }

        public string getJson()
        {
            return null;
        }

        public List<Parameter> getParams()
        {
            List<Parameter> list = new List<Parameter>();

            list.Add(new Parameter("_sizeGuid", Lang.instance["SizeGuid"], TypeParam.TFloat, _point.Range, _point.getRangeLimit()));
            return list;
        }

        public IParamProvider getProvider()
        {
            return this;
        }

        public string getShortInfo()
        {
            return Lang.instance["Point params"];
        }

        public string getTitle()
        {
            return Lang.instance["Link params"];
        }
        public void addChangeListener(UnityAction listener)
        {
        }

        public void removeChangeListener(UnityAction listener)
        {
        }

        protected void updateParam(Parameter value)
        {
            switch (value.name)
            {
                case "_sizeGuid": _point.SetRange(value.NumberValue);
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

        public void setJson(string json)
        {
        }

        public void setStart(Parameter value)
        {
        }
    }
}
