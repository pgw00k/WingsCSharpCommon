/*
 * FileName:    SingleEditorWindow
 * Author:      Wings
 * CreateTime:  2021_11_24
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GenOcean.Unity.Editor
{
    public class SingleEditorWindow<T> : EditorWindow
        where T : EditorWindow
    {
        #region Protected Fields

        protected static T _Instance;

        #endregion --Protected Fields

        #region Public Fields

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        public static void OpenWindow()
        {
            if(_Instance)
            {
                _Instance.Close();
                _Instance = null;
            }

            _Instance = (T)EditorWindow.GetWindow<T>();
            _Instance.Show();
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods
        #endregion --Protected Methods

        #region Unity Methods

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void OnDestroy()
        {

        }
        #endregion --Unity Methods
    }
}
