using UnityEngine;
using Vmaya.RW;

namespace Vmaya.Robot.Catalog
{
    public class CZones : MonoBehaviour, INameProvider
    {
        [SerializeField]
        private Component _nameProvider;
        public INameProvider NameProvider => _nameProvider ? _nameProvider.GetComponent<INameProvider>() : this;

        private int _nameIndex;

        private void OnValidate()
        {
            _nameProvider = NameProvider as Component;
        }

        public string checkNextName(string origin)
        {
            _nameIndex++;
            return Indent.GetOrigin(origin) + '-' + _nameIndex.ToString();
        }

        public void reset()
        {
            _nameIndex = 0;
        }

        public void ResetNames()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                ConnectableEntity entity = transform.GetChild(i).GetComponent<ConnectableEntity>();
                if (entity != null)
                    entity.name = checkNextName(entity.sourceName);
            }
        }
    }
}
