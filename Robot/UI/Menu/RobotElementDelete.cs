using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vmaya.Command;
using Vmaya.UI.Menu;

namespace Vmaya.Robot.UI.Menu
{
    public class RobotElementDelete : MonoBehaviour, IExecutableAndRecoverable
    {
        [SerializeField]
        private bool _isDeleted = false;

        private void setDeleted(bool value)
        {
            _isDeleted = value;
            updateFromProperty();
        }

        protected void updateFromProperty()
        {
            gameObject.SetActive(!_isDeleted);
        }

        public void Execute()
        {
            setDeleted(true);
        }

        public bool getPerformed()
        {
            return !gameObject.activeSelf;
        }

        public string getRecoveryData()
        {
            return JsonUtility.ToJson(this);
        }

        public void Recovery(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            updateFromProperty();
        }
    }
}
