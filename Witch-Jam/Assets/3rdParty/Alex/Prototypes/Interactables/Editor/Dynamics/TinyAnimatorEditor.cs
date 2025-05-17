using UnityEditor;
using UnityEngine;

namespace Prototypes.Interactables.Dynamics.Editor
{
    [CustomEditor(typeof(TinyAnimator))]
    public class TinyAnimatorEditor : UnityEditor.Editor
    {
        private const string DefaultClipName = "New TinyAnimation";

        public override void OnInspectorGUI()
        {
            _ = DrawDefaultInspector();

            GUILayout.Space(10);

            if(GUILayout.Button("Edit Animation"))
            {
                SerializedProperty serializedAnimation = serializedObject.FindProperty(TinyAnimator.ClipFieldName);
                AnimationClip clip = serializedAnimation.objectReferenceValue as AnimationClip;

                if(clip == null)
                {
                    clip = new AnimationClip();
                    clip.name = DefaultClipName;
                    clip.legacy = true;

                    // Embed it in the component (can be undone via Undo system)
                    Undo.RecordObject(serializedObject.targetObject, "Create Embedded AnimationClip");
                    serializedAnimation.objectReferenceValue = clip;
                    serializedObject.ApplyModifiedProperties();
                }

                TinyAnimationEditorWindow.OpenWindow(clip);
            }
        }
    }
}
