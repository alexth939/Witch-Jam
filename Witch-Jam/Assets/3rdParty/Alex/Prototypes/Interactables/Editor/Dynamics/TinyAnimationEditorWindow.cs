using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prototypes.Interactables.Dynamics.Editor
{
    public class TinyAnimationEditorWindow : EditorWindow
    {
        private AnimationClip _clip;

        [MenuItem("Window/Tiny Animation Editor (Dev Shortcut)")]
        public static void DevOpen()
        {
            OpenWindow(null);
        }

        public static void OpenWindow(AnimationClip clip)
        {
            var window = GetWindow<TinyAnimationEditorWindow>("Tiny Animation Editor");
            window._clip = clip;
            window.minSize = new Vector2(600, 400);
            window.Show();
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();

            // Create the menu bar
            var menuBar = new VisualElement { name = "MenuBar" };
            menuBar.style.flexDirection = FlexDirection.Row;
            menuBar.style.paddingTop = 4;
            menuBar.style.paddingBottom = 4;
            menuBar.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

            // Menu items (buttons for "Option", "Edit", "Help")
            var optionButton = CreateMenuButton("Option", new string[] { "Option A", "Option B", "Option C" });
            var editButton = CreateMenuButton("Edit", new string[] { "Edit A", "Edit B", "Edit C" });
            var helpButton = CreateMenuButton("Help", new string[] { "About", "Docs", "Report Bug" });

            // Adding menu buttons to the menu bar
            menuBar.Add(optionButton);
            menuBar.Add(editButton);
            menuBar.Add(helpButton);

            rootVisualElement.Add(menuBar);

            // Clip state and other UI elements
            if(_clip == null)
            {
                var warning = new HelpBox("No clip assigned.", HelpBoxMessageType.Warning);
                rootVisualElement.Add(warning);
            }
            else
            {
                var clipLabel = new Label($"Editing: {_clip.name}");
                clipLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                clipLabel.style.marginTop = 8;
                rootVisualElement.Add(clipLabel);

                var info = new HelpBox("Editor UI to come... This is just the base window.", HelpBoxMessageType.Info);
                rootVisualElement.Add(info);
            }
        }

        private VisualElement CreateMenuButton(string menuName, string[] options)
        {
            var button = new Button(() =>
            {
                // Create the dropdown menu when button is clicked
                var dropdownMenu = new GenericMenu();

                foreach(var option in options)
                {
                    dropdownMenu.AddItem(new GUIContent(option), false, () => HandleMenuAction(option));
                }

                dropdownMenu.ShowAsContext();
            })
            {
                text = menuName
            };

            return button;
        }

        private void HandleMenuAction(string action)
        {
            Debug.Log($"{action} clicked!");
            if(action == "About")
            {
                ShowAbout();
            }
        }

        private void ShowAbout()
        {
            EditorUtility.DisplayDialog("Tiny Animation Editor", "Version 0.1 — Custom Animation Clip Editor.\nMade with by you.", "Cool");
        }
    }
}
