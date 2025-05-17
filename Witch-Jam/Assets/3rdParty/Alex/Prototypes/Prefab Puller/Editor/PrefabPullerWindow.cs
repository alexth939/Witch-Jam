using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Prototypes.PrefabPuller.Editor
{
    public class PrefabPullerWindow : EditorWindow
    {
        private List<GameObject> _prefabs = new();
        private List<string> _tags = new();
        private Vector2 _scrollPos;

        private int _selectedTagIndex = 0;

        [MenuItem("Tools/Prefab Puller")]
        public static void ShowWindow()
        {
            GetWindow<PrefabPullerWindow>("Prefab Puller");
        }

        private void OnEnable()
        {
            RescanAssetDatabase();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            if(GUILayout.Button("Rescan"))
            {
                RescanAssetDatabase();
            }

            _selectedTagIndex = EditorGUILayout.Popup("Tag", _selectedTagIndex, _tags.ToArray());

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            string selectedTag = _tags[_selectedTagIndex];

            foreach(var prefab in _prefabs)
            {
                if(selectedTag != "None" && prefab.tag != selectedTag)
                    continue;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(prefab.name);

                if(GUILayout.Button("Ping", GUILayout.Width(50)))
                {
                    EditorGUIUtility.PingObject(prefab);
                }

                if(GUILayout.Button("Preview", GUILayout.Width(60)))
                {
                    PrefabPreviewWindow.ShowWindow(prefab);
                }

                if(GUILayout.Button("Spawn", GUILayout.Width(60)))
                {
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void RescanAssetDatabase()
        {
            _prefabs.Clear();

            string[] guids = AssetDatabase.FindAssets("t:Prefab");

            foreach(var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if(prefab != null)
                {
                    _prefabs.Add(prefab);
                }
            }

            // Collect unique tags + None
            HashSet<string> tags = new() { "None" };
            foreach(var prefab in _prefabs)
            {
                if(!string.IsNullOrEmpty(prefab.tag) && prefab.tag != "Untagged")
                    tags.Add(prefab.tag);
            }

            _tags = tags.ToList();
        }
    }
}
