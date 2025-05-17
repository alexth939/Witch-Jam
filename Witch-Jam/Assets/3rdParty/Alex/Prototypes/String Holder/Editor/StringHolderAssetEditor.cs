using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using Prototypes.StringHolder;

namespace Prototypes.StringHolder.Editor
{
    namespace Prototypes.MainMenu
    {
        [CustomEditor(typeof(StringHolderAsset))]
        public class StringHolderAssetEditor : UnityEditor.Editor
        {
            private GUIContent _labelContent;
            private SerializedProperty _valueProperty;

            [Shortcut("Create String Holder", KeyCode.V, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
            [MenuItem("Tools/Create String Holder", isValidateFunction: false)]
            public static void CreateInstance()
            {
                var asset = CreateInstance<StringHolderAsset>();
                Debug.Log("[StringHolderAsset] Instance of StringHolderAsset created");

                string targetProjectPath = GetActiveProjectWindowPath();
                Debug.Log("[StringHolderAsset] Resolved project window path: " + targetProjectPath);

                string uniquePath = AssetDatabase.GenerateUniqueAssetPath(targetProjectPath + "/StringHolder.asset");
                Debug.Log("[StringHolderAsset] Unique asset path generated: " + uniquePath);

                AssetDatabase.CreateAsset(asset, uniquePath);
                Debug.Log("[StringHolderAsset] Asset created at: " + uniquePath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("[StringHolderAsset] AssetDatabase saved and refreshed");

                OpenProjectWindowIfClosed();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
                Debug.Log("[StringHolderAsset] New asset selected in Project window");

                EditorApplication.delayCall += () => EditorGUIUtility.PingObject(asset);
            }

            public override void OnInspectorGUI()
            {
                const string ButtonText = "Dump *.txt";

                _ = EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_labelContent, EditorStyles.boldLabel);

                if(GUILayout.Button(ButtonText, GUILayout.Width(100)))
                    DumpToTextFile();

                EditorGUILayout.EndHorizontal();

                _valueProperty.stringValue = EditorGUILayout.TextArea(_valueProperty.stringValue);
                _ = serializedObject.ApplyModifiedProperties();
            }

            private static string GetActiveProjectWindowPath()
            {
                Type projectWindowUtilType = typeof(ProjectWindowUtil);
                string methodName = "GetActiveFolderPath";
                BindingFlags methodFlags = BindingFlags.Static | BindingFlags.NonPublic;
                MethodInfo getActiveFolderPathMethod = projectWindowUtilType.GetMethod(methodName, methodFlags);

                if(getActiveFolderPathMethod != null)
                {
                    string path = (string)getActiveFolderPathMethod.Invoke(null, null);

                    if(!string.IsNullOrEmpty(path) && AssetDatabase.IsValidFolder(path))
                    {
                        return path;
                    }
                }

                return "Assets";
            }

            private static void OpenProjectWindowIfClosed()
            {
                const string ProjectWindowMenuItem = "Window/General/Project";

                _ = EditorApplication.ExecuteMenuItem(ProjectWindowMenuItem);
                Debug.Log("[StringHolderAsset] Project window opened (if it was closed)");
            }

            private void DumpToTextFile()
            {
                StringHolderAsset asset = (StringHolderAsset)target;
                string assetPath = AssetDatabase.GetAssetPath(asset);
                string directory = Path.GetDirectoryName(assetPath);
                string fileName = Path.GetFileNameWithoutExtension(assetPath) + ".txt";
                string filePath = Path.Combine(directory, fileName);

                File.WriteAllText(filePath, asset.Value);
                Debug.Log("[StringHolderAsset] Dumped value to " + filePath);
                AssetDatabase.Refresh();
            }

            private void OnEnable()
            {
                _labelContent = new GUIContent("Value", "Value can be resolved by (StringHolderAsset).Value; and GetValue();");
                _valueProperty = serializedObject.FindProperty(StringHolderAsset.ValueFieldName);
            }
        }
    }
}
