using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Language;
using Vmaya.Params;
using Vmaya.Robot.Components;
using Vmaya.Scene3D;

namespace Vmaya.Robot.Params
{
    [RequireComponent(typeof(HandElement))]
    public class HandParams : ElementParams
    {
        [HideInInspector]
        [SerializeField]
        private float _openness;
        protected HandElement _hand => GetComponent<HandElement>();
        public override List<Parameter> getParams()
        {
            List<Parameter> result = base.getParams();

            result.Add(new Parameter("_openness", Lang.instance["openness"], TypeParam.TFloat, _openness = _hand.Openness, Limit.init(0, 1)));

            return result;
        }

        protected override void updateParam(Parameter value)
        {
            switch (value.name)
            {
                case "_openness":
                    _hand.Openness = _openness = value.NumberValue;
                    break;
                default: base.updateParam(value);
                    break;
            }
        }

        public override void setJson(string json)
        {
            base.setJson(json);
            _hand.Openness = _openness;
        }
    }
}
