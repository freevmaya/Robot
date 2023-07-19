using Vmaya.UI.UIBlocks.RW;
using UnityEngine;

namespace Vmaya.UI.UITabs
{
    public class TabPanelCreator : MonoBehaviour
    {
        public RWPanelSpawner PanelSpawner;

        private int index = -1;
        public void createPanel()
        {
            TabsPanel panel = PanelSpawner.createRandom(transform) as TabsPanel;
            index = (index + 1) % panel.ContentSpawner.Templates.Length;
            panel.addContent(panel.ContentSpawner.Templates[index].name);
        }

        public void createPanel(string contentName)
        {
            if (!findInstantiated(contentName))
            {
                TabsPanel panel = PanelSpawner.createPanel(transform) as TabsPanel;
                panel.addContent(contentName);
            }
        }

        private TabContent findInstantiated(string contentNamePrefab)
        {
            TabContent[] list = GetComponentsInChildren<TabContent>();
            foreach (TabContent content in list)
                if (contentNamePrefab.Equals(content.Origin.name)) return content;

            return null;
        }
    }
}
