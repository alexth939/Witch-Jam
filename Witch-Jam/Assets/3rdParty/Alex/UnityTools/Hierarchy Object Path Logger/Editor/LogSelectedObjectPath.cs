using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace HierarchyObjectPathLogger.Editor
{
    public static class LogSelectedObjectPath
    {
        [Shortcut("Tools/Log Selected Object Path", KeyCode.B, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        [MenuItem("Tools/Log Selected Object Path")]
        private static void LogHierarchyPath()
        {
            if(Selection.activeTransform == null)
            {
                Debug.LogWarning("No GameObject selected in the Hierarchy.");

                return;
            }

            var transform = Selection.activeTransform;
            string sceneName = transform.gameObject.scene.name;
            string path = sceneName + "/" + GetHierarchyPath(transform);

            Debug.Log($"Hierarchy Path: {path}", transform.gameObject);
        }

        private static string GetHierarchyPath(Transform transform)
        {
            string path = transform.name;

            while(transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }
    }
}
