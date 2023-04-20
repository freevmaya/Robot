using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vmaya.Command;
using Vmaya.Language;
using Vmaya.Robot.Command;
using Vmaya.Robot.Entity;
using Vmaya.Robot.UI.Menu;
using Vmaya.Scene3D;
using Vmaya.UI.Menu;

namespace Vmaya.Robot.UI
{
    [RequireComponent(typeof(ConnectableEntity))]
    public class RElementMenu : MonoBehaviour, IMenuList, IActionCaller, IAvailableProvider
    {
        [SerializeField]
        private PopupMenu _popupMenu;
        protected PopupMenu PopupMenu => _popupMenu ? _popupMenu : FindObjectOfType<PopupMenu>(true);

        private ConnectableEntity _entity => GetComponent<ConnectableEntity>();

        private RobotElementDelete _deleteComponent => GetComponent<RobotElementDelete>();

        private void Awake()
        {
            hitDetector.instance.onClick.AddListener(doClick);
        }

        private void doClick(baseHitMouse hit)
        {
            if (hit.transform.IsChildOf(transform) && Input.GetMouseButtonUp(1))
                Show();
        }

        public void Show()
        {
            PopupMenu.Show(Input.mousePosition, this);
        }


        public void Call(string activity)
        {
            switch (activity)
            {
                case "Disconnect": Disconnect();
                    break;
                case "Delete": Delete();
                    break;
            }
        }

        private void Delete()
        {
            CommandManager.ExecuteCmd(new ExecuteCommand(_deleteComponent));
        }

        private void Disconnect()
        {
            CommandManager.ExecuteCmd(new DisconnectCommand(_entity));
        }

        public void Set(List<MenuItemData> a_items)
        {
        }

        public List<MenuItemData> Get()
        {
            return new List<MenuItemData>() {
                new MenuItemData(Lang.instance["Disconnect"], "Disconnect"),
                new MenuItemData(Lang.instance["Delete"], "Delete")
            };
        }

        public bool GetAvailable(string activity)
        {
            switch (activity)
            {
                case "Disconnect": return _entity.Element.GetParent() != null;
                case "Delete": return _deleteComponent != null;
            }
            return false;
        }
    }
}
