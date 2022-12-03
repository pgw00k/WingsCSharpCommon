using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Wings.Unity.Editor
{
    public class MeshInfoEditor
    {
        protected Object[] _SelectionObjects;
        protected long _VertexCount = 0;
        protected long _FaceCount = 0;

        #region GUI

        protected Vector2 _ResultPos;
        protected bool _IsAutoRefresh = true;

        public virtual void RenderGUI()
        {
            GUILayout.Label($"Select {Selection.objects.Length} objects.");
            _IsAutoRefresh = GUILayout.Toggle(_IsAutoRefresh, "AutoRefresh");

            if (!_IsAutoRefresh)
            {
                if (GUILayout.Button("Refresh"))
                {
                    UpdateInfo();
                }
            }

            _ResultPos = GUILayout.BeginScrollView(_ResultPos);

            GUILayout.Label($"Vertex:{_VertexCount}");
            GUILayout.Label($"Face:{_FaceCount}");

            GUILayout.EndScrollView();

        }

        #endregion

        public MeshInfoEditor()
        {
            Awake();
        }

        protected virtual void Awake()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        protected virtual void OnDestroy()
        {
            try
            {
                Selection.selectionChanged -= OnSelectionChanged;
            }
            catch (System.Exception err)
            {
                Debug.LogError(err.Message);
            }
        }

        public virtual void UpdateInfo()
        {
            _VertexCount = 0;
            _FaceCount = 0;
            foreach (var item in Selection.objects)
            {
                GameObject go = item as GameObject;
                if (!go)
                {
                    continue;
                }


                List<Renderer> frs = new List<Renderer>();

                var rs = go.GetComponentsInChildren<Renderer>();

                var lods = go.GetComponentsInChildren<LODGroup>();

                foreach (var lod in lods)
                {
                    var ls = lod.GetLODs();
                    for (int i = 1; i < ls.Length; i++)
                    {
                        frs.AddRange(ls[i].renderers);
                    }
                }

                foreach (Renderer r in rs)
                {
                    if (frs.Contains(r))
                    {
                        continue;
                    }

                    SkinnedMeshRenderer smr = r as SkinnedMeshRenderer;
                    MeshRenderer mr = r as MeshRenderer;

                    if (mr)
                    {
                        MeshFilter mf = mr.GetComponent<MeshFilter>();
                        if (mf)
                        {
                            _VertexCount += mf.sharedMesh.vertices.Length;
                            _FaceCount += mf.sharedMesh.triangles.Length / 3;
                        }
                    }

                    if (smr)
                    {
                        _VertexCount += smr.sharedMesh.vertices.Length;
                        _FaceCount += smr.sharedMesh.triangles.Length / 3;
                    }
                }
            }
        }

        protected virtual void OnSelectionChanged()
        {
            if (!_IsAutoRefresh)
            {
                return;
            }

            if (_SelectionObjects != null && _SelectionObjects.Length > 0)
            {

            }

            UpdateInfo();
        }

        public virtual void Destroy()
        {
            OnDestroy();
        }
    }

    public class MeshInfoEditorWindow : EditorWindow
    {
        #region Instance
        protected static MeshInfoEditorWindow _WIN;
        public MeshInfoEditorWindow()
        {
            titleContent = new GUIContent("Mesh Info");
            Init();
        }

        [MenuItem("Wings/Anlysis/Mesh Info")]
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

            _WIN = (MeshInfoEditorWindow)EditorWindow.GetWindow(typeof(MeshInfoEditorWindow));
            _WIN.Show();
        }
        #endregion

        protected MeshInfoEditor _MeshInfoEditor;

        protected virtual void Init()
        {
            _MeshInfoEditor = new MeshInfoEditor();
        }

        protected virtual void OnGUI()
        {
            _MeshInfoEditor.RenderGUI();
        }

        protected virtual void OnDestroy()
        {
            _MeshInfoEditor.Destroy();
        }
    }
}
