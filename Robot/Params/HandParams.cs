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
        
        [HideInInspector]
        [SerializeField]
        private float _turn;
        protected HandElement _hand => GetComponent<HandElement>();
        public override List<Parameter> getParams()
        {
            List<Parameter> result = base.getParams();

            result.Add(new Parameter("_openness", Lang.instance["Openness"], TypeParam.TFloat, (_openness = _hand.Openness) * 100, Limit.init(0, 100)));
            if (_hand.Wrist)
                result.Add(new Parameter("_turn", Lang.instance["Turn"], TypeParam.TFloat, _turn = _hand.Turn, Limit.init(0, 360)));

            return result;
        }

        protected override void updateParam(Parameter value)
        {
            switch (value.name)
            {
                case "_openness":
                    _hand.Openness = _openness = value.NumberValue / 100;
                    break;
                case "_turn":
                    _hand.Turn = _turn = value.NumberValue;
                    break;
                default: base.updateParam(value);
                    break;
            }
        }

        public override void setJson(string json)
        {
            base.setJson(json);
            _hand.Openness = _openness;
            _hand.Turn = _turn;
        }
    }
}
