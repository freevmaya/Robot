using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya;
using Vmaya.Command;

namespace Vmaya.Robot.Command
{
    public abstract class BaseConnectableEntityCommand : BaseCommand
    {
        [SerializeField]
        private Indent _elemIndent;
        protected ConnectableEntity entity => _elemIndent.Find<ConnectableEntity>();

        public BaseConnectableEntityCommand()
        {
        }

        public BaseConnectableEntityCommand(ConnectableEntity entity)
        {
            _elemIndent = new Indent(entity);
        }
    }
}
