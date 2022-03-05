/*
 * FileName:    UnityGUIDebugConsole
 * Author:      Wings
 * CreateTime:  2021_12_07
 * 
*/

#if DEBUG_TEST
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenOcean.Common
{
    [DefaultExecutionOrder(-2000)]
    public class UnityGUIDebugConsole : MonoBehaviour
    {
#region Protected Fields

        protected string _data;

        protected Vector2 _scrolViewPos;

#endregion --Protected Fields

#region Public Fields
#endregion --Public Fields

#region Private Fields
#endregion --Private Fields

#region Public Methods

        public virtual void LogCallback(string condition, string stackTrace, LogType type)
        {
            //Log($"[{type.GetType().Name}][{DateTime.Now:yyyy_MM_dd-HH_mm_ssss}]{condition}{Environment.NewLine}{stackTrace}.{Environment.NewLine}");
            Log($"[{type.GetType().Name}][{DateTime.Now:yyyy_MM_dd-HH_mm_ssss}]{condition}{Environment.NewLine}.{Environment.NewLine}");
        }

#endregion --Public Methods

#region Private Methods
#endregion --Public Methods

#region Protected Methods

        protected virtual void Log(string log)
        {
            _data += log;
        }

#endregion --Protected Methods

#region Unity Methods

        protected virtual void Awake()
        {
            Application.logMessageReceived += LogCallback;
            Debug.Log("Debug Mode.");
        }

        protected virtual void OnGUI()
        {
            _scrolViewPos = GUILayout.BeginScrollView(_scrolViewPos);
            _data = GUILayout.TextArea(_data);
            GUILayout.EndScrollView();
        }

#endregion --Unity Methods
    }
}
#endif

