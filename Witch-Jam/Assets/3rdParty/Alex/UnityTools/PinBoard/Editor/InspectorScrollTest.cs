using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PinBoard.Editor
{
    [System.Obsolete("Refactor me please.", error: false)]
    public static class InspectorScrollTest
    {
        //[MenuItem("Window/Scroll Inspector Up")]
        public static void ScrollUIToolkitInspector()
        {
            var inspectorType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            var inspectors = Resources.FindObjectsOfTypeAll(inspectorType);

            if(inspectors.Length == 0)
            {
                Debug.LogWarning("No InspectorWindow found.");
                return;
            }

            var inspector = inspectors[0] as EditorWindow;
            var root = inspector.rootVisualElement;

            if(root == null)
            {
                Debug.LogWarning("No rootVisualElement found.");
                return;
            }

            // Traverse to find a ScrollView
            var scrollView = root.Q<ScrollView>();
            if(scrollView == null)
            {
                Debug.LogWarning("No ScrollView found in inspector.");
                return;
            }

            // Scroll it upward by some amount
            var currentScroll = scrollView.scrollOffset;
            Debug.Log($"[Before] scrollOffset: {currentScroll}");

            currentScroll.y -= 96f;
            scrollView.scrollOffset = currentScroll;

            Debug.Log($"[After] scrollOffset: {scrollView.scrollOffset}");
        }
    }
}
