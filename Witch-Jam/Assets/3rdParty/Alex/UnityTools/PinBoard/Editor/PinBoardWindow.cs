using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using Object = UnityEngine.Object;

namespace PinBoard.Editor
{
    [System.Obsolete("Refactor me please.", error: false)]
    public class PinBoardWindow : EditorWindow
    {
        [MenuItem("Window/PinBoard")]
        public static void ShowWindow()
        {
            var window = GetWindow<PinBoardWindow>();
            window.titleContent = new GUIContent("PinBoard");

            // scenario: pinboard is opened. closing editor. open editor.
            // pinboard window is restored, but not showing entries.
            //window.LoadPinsPage();
            //window.RefreshPins();
            // so doing it on creategui
        }

        private enum PinContext { Project, Scene }
        private PinContext _currentContext = PinContext.Project;

        private PinBoardPage _pinBoardPage;
        private readonly Dictionary<PinContext, List<PinReference>> _pins = new()
        {
            { PinContext.Project, new List<PinReference>() },
            { PinContext.Scene, new List<PinReference>() },
        };

        private VisualElement _pinContainer;

        private void LoadPinsPage()
        {
            var guids = AssetDatabase.FindAssets("t:PinBoardPage");

            if(guids.Length <= 0)
            {
                Debug.LogWarning("PinBoardPage not found. Pins won't be persistant.");
            }
            else if(guids.Length > 1)
            {
                Debug.LogWarning($"{guids.Length} PinBoardPages found! Pins won't be persistant.");
            }
            else
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _pinBoardPage = AssetDatabase.LoadAssetAtPath<PinBoardPage>(path);

                _pins[PinContext.Project] = _pinBoardPage.ProjectPins;
                _pins[PinContext.Scene] = _pinBoardPage.ScenePins;
            }
        }

        private void SavePinsPage()
        {
            _pinBoardPage.ProjectPins = _pins[PinContext.Project];
            _pinBoardPage.ScenePins = _pins[PinContext.Scene];

            _pinBoardPage.SetDirty();
            AssetDatabase.SaveAssetIfDirty(_pinBoardPage);
        }

        public void CreateGUI()
        {
            // Root layout
            var root = rootVisualElement;
            root.style.paddingLeft = 10;
            root.style.paddingTop = 10;
            root.style.paddingRight = 10;
            root.style.paddingBottom = 10;

            // Toolbar
            var toolbar = new Toolbar();
            var projectButton = new ToolbarToggle { text = "Project" };
            var sceneButton = new ToolbarToggle { text = "Scene" };

            projectButton.RegisterValueChangedCallback(evt =>
            {
                if(evt.newValue)
                {
                    sceneButton.SetValueWithoutNotify(false);
                    SwitchContext(PinContext.Project);
                }
            });

            sceneButton.RegisterValueChangedCallback(evt =>
            {
                if(evt.newValue)
                {
                    projectButton.SetValueWithoutNotify(false);
                    SwitchContext(PinContext.Scene);
                }
            });

            projectButton.value = true; // Default context

            toolbar.Add(projectButton);
            toolbar.Add(sceneButton);
            root.Add(toolbar);

            // Scroll container for pins
            _pinContainer = new ScrollView(scrollViewMode: ScrollViewMode.Vertical);
            _pinContainer.AddToClassList("pin-container");
            _pinContainer.RegisterCallback<DragPerformEvent>(OnDragPerform);
            _pinContainer.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);

            _pinContainer.RegisterCallback<DragEnterEvent>(evt =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            });

            _pinContainer.pickingMode = PickingMode.Position; // allows receiving drag events

            //_pinContainer.style.minHeight = 200;
            _pinContainer.style.flexGrow = 1f;
            _pinContainer.style.backgroundColor = new Color(0, 0, 0, 0.05f); // light gray for debug

            root.Add(_pinContainer);

            LoadPinsPage();
            RefreshPins();
        }

        private void SwitchContext(PinContext context)
        {
            _currentContext = context;
            RefreshPins();
        }

        private void RefreshPins()
        {
            _pinContainer.Clear();

            foreach(var pin in _pins[_currentContext])
            {
                var resolved = pin.Resolve();
                if(resolved == null)
                    continue;

                var objectField = new ObjectField
                {
                    value = resolved,
                    objectType = typeof(Object),
                    allowSceneObjects = true,
                    enabledSelf = false,
                };
                objectField.style.flexGrow = 1;        // Fills the remaining space
                objectField.style.flexShrink = 1;
                objectField.style.minWidth = 0;        // Prevent overflow/truncation

                var removeButton = new Button(() =>
                {
                    _pins[_currentContext].Remove(pin);
                    RefreshPins();
                })
                {
                    text = "X",
                    style = { width = 20, marginLeft = 5 }
                };

                var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };

                row.style.flexGrow = 1;
                row.style.width = Length.Percent(100); // Optional, ensures it fills horizontally

                row.Add(objectField);
                row.Add(removeButton);

                //row.RegisterCallback<MouseDownEvent>(evt =>
                //{
                //    if(evt.button == 2) // Middle mouse button
                //    {
                //        _pins[_currentContext].Remove(pin);
                //        RefreshPins();
                //        evt.StopPropagation(); // Optional: prevent bubbling if needed
                //    }
                //});

                row.RegisterCallback<MouseDownEvent>(evt =>
                {
                    switch(evt.button)
                    {
                    //case 1: // RMB Ping
                    //    PingObject(resolved);
                    //    evt.StopPropagation();
                    //    break;

                    case 2: // MMB Remove
                        _pins[_currentContext].Remove(pin);
                            RefreshPins();
                            evt.StopPropagation();
                            SavePinsPage();
                            break;
                    }
                });

                _pinContainer.Add(row);
            }
        }

        private void OnDragUpdate(DragUpdatedEvent evt)
        {
            if(DragAndDrop.objectReferences.Length == 0)
                return;

            bool seenScene = false, seenProject = false;

            foreach(var obj in DragAndDrop.objectReferences)
            {
                if(obj == null)
                    continue;

                if(EditorUtility.IsPersistent(obj))
                    seenProject = true;
                else
                    seenScene = true;

                if(seenProject && seenScene)
                    break; // Mixed selection detected
            }

            if(seenProject && seenScene)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                evt.StopPropagation();
                return;
            }

            bool isValidForContext = _currentContext switch
            {
                PinContext.Project => seenProject,
                PinContext.Scene => seenScene,
                _ => false
            };

            DragAndDrop.visualMode = isValidForContext ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
            evt.StopPropagation();
        }

        private void OnDragPerform(DragPerformEvent evt)
        {
            DragAndDrop.AcceptDrag();

            foreach(var obj in DragAndDrop.objectReferences)
            {
                if(obj == null)
                    continue;

                bool isPersistent = EditorUtility.IsPersistent(obj);
                if((_currentContext == PinContext.Project && !isPersistent) ||
                    (_currentContext == PinContext.Scene && isPersistent))
                {
                    continue; // Skip incompatible objects
                }

                var pin = PinReference.FromObject(obj);
                if(!_pins[_currentContext].Exists(p => p._globalIdString == pin._globalIdString))
                {
                    _pins[_currentContext].Add(pin);
                }
            }

            RefreshPins();
            evt.StopPropagation();
            SavePinsPage();
        }
    }
}
