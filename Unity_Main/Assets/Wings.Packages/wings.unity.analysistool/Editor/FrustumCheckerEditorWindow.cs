using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Wings.Unity.Editor
{
    using UnityEngine;

    public static class RendererExtensions
    {
        public static bool IsVisibleFrom(Renderer renderer, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }
    }

    public class MeshInfoEditorFSC : MeshInfoEditor
    {
        public MeshInfoEditorFSC()
        {
            _IsAutoRefresh = false;
        }
        public override void RenderGUI()
        {
            GUILayout.Label($"Vertex:{_VertexCount}");
            GUILayout.Label($"Face:{_FaceCount}");
        }
    }

    public class FrustumCheckerEditorWindow : EditorWindow
    {
        #region Instance
        protected static FrustumCheckerEditorWindow _WIN;
        public FrustumCheckerEditorWindow()
        {
            titleContent = new GUIContent("Frustum Checker");
            Init();
        }

        [MenuItem("Wings/Anlysis/Frustum Checker")]
        public static void OpenWindow()
        {
            if (_WIN)
            {
                _WIN.Close();
                EditorWindow.DestroyImmediate(_WIN);
                _WIN = null;
            }

            if (Application.isPlaying)
            {
                return;
            }

            _WIN = (FrustumCheckerEditorWindow)EditorWindow.GetWindow(typeof(FrustumCheckerEditorWindow));
            _WIN.Show();
        }
        #endregion

        #region GUI

        protected Camera _CheckerCamera;
        protected Vector2 _ResultPos;
        protected MeshInfoEditor _MeshInfoEditor;

        protected List<GameObject> _VisiableRenderList = new List<GameObject>();

        protected virtual void  Init()
        {
            _MeshInfoEditor = new MeshInfoEditorFSC();
        }
        protected virtual void OnGUI()
        {

            _CheckerCamera = (Camera)EditorGUILayout.ObjectField(_CheckerCamera, typeof(Camera), true);

            if (GUILayout.Button("Refresh"))
            {
                UpdateInfo();
                _MeshInfoEditor.UpdateInfo();
            }

            if(_VisiableRenderList.Count > 0)
            {
                GUILayout.Label($"See {_VisiableRenderList.Count} objects.");

                _MeshInfoEditor.RenderGUI();

                if (GUILayout.Button("Select"))
                {
                    Selection.objects = _VisiableRenderList.ToArray();
                }

                _ResultPos = GUILayout.BeginScrollView(_ResultPos);

                using (new EditorGUI.DisabledScope(true))
                {
                    for (int i = 0; i < _VisiableRenderList.Count; i++)
                    {
                        EditorGUILayout.ObjectField(_VisiableRenderList[i], typeof(GameObject), true);
                    }
                }

                GUILayout.EndScrollView();
            }



        }

        #endregion

        protected virtual void UpdateInfo()
        {
            _VisiableRenderList.Clear();

            foreach (Object obj in Selection.objects)
            {
                Camera cam = obj as Camera;
                if(cam != null)
                {
                    _CheckerCamera = cam;
                }
            }

            if(!_CheckerCamera)
            {
                _CheckerCamera = Camera.main;
            }

            if(!_CheckerCamera)
            {
                return;
            }

            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_CheckerCamera);

            foreach (var r in FindObjectsOfType<Renderer>())
            {
                if (!GeometryUtility.TestPlanesAABB(planes, r.bounds))
                {
                    continue;
                }

                GameObject go = r.gameObject;
                LODGroup[] lgs = r.GetComponentsInParent<LODGroup>();
                foreach (LODGroup l in lgs)
                {
                    foreach(LOD slod in l.GetLODs())
                    {
                        if(slod.renderers.Contains(r))
                        {
                            go = l.gameObject;
                        }
                    }
                }

                if(!_VisiableRenderList.Contains(go))
                {
                    _VisiableRenderList.Add(go);
                }
            }

            Selection.objects = _VisiableRenderList.ToArray();

        }

        protected virtual void OnDestroy()
        {
            _MeshInfoEditor.Destroy();
        }
    }
}
