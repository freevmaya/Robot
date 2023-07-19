using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vmaya.Command;
using Vmaya.Robot.Entity;
using Vmaya.UI.Menu;

namespace Vmaya.Robot.UI.Menu
{
    public class RobotElementDelete : MonoBehaviour, IExecutableAndRecoverable
    {
        private ConnectableEntity entity => GetComponent<ConnectableEntity>();

        [SerializeField]
        private bool _isDeleted = false;

        [SerializeField]
        private string restoreJson;

        private void setDeleted(bool value)
        {
            _isDeleted = value;
            updateFromProperty();
        }

        protected void updateFromProperty()
        {
            gameObject.SetActive(!_isDeleted);
            //if (!_isDeleted)
                //entity.setJson(restoreJson);
        }

        public void Execute()
        {
            for (int i=0; i<entity.Element.SlotCount(); i++)
            {
          //      entity.Element.GetSlot(i).SetConnect(null);
            }
            setDeleted(true);
        }

        public bool getPerformed()
        {
            return !gameObject.activeSelf;
        }

        public string getRecoveryData()
        {
            //restoreJson = entity.getJson();
            return JsonUtility.ToJson(this);
        }

        public void Recovery(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            updateFromProperty();
        }
    }
}
