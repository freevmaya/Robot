using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vmaya.Robot.Components
{
    public class SimpleChainLink : MonoBehaviour, IChainLink
    {
        public List<IChainLink> GetChain()
        {
            List<IChainLink> result = new List<IChainLink>();
            IChainLink current = this;
            do
            {
                result.Insert(0, current);
            } while ((current = current.GetParentLink()) != null);

            return result;
        }

        public IChainLink GetParentLink()
        {
            return transform.parent.GetComponentInParent<IChainLink>();
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}
