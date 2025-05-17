using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Prototypes.PrefabPuller.Editor
{
    public class PrefabPreviewWindow : EditorWindow
    {
        private GameObject _prefab;
        private GameObject _previewInstance;
        private PreviewRenderUtility _previewUtility;

        private float _distance = 5f;
        private float _yaw = 0f;
        private float _pitch = 15f;
        private Vector3 _targetPosition = Vector3.zero;
        private Vector2 _lastMousePosition;

        private Vector3 _initialCameraPosition;

        public static void ShowWindow(GameObject prefab)
        {
            var window = CreateInstance<PrefabPreviewWindow>();
            window._prefab = prefab;
            window.titleContent = new GUIContent($"Preview: {prefab.name}");
            window.Show();
        }

        private void OnEnable()
        {
            Debug.Log("[PreviewWindow] OnEnable");

            _previewUtility = new PreviewRenderUtility();
            _previewUtility.cameraFieldOfView = 30f;
            _previewUtility.camera.nearClipPlane = 0.1f;
            _previewUtility.camera.farClipPlane = 100f;

            _previewUtility.lights[0].intensity = 1.4f;
            _previewUtility.lights[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
            _previewUtility.lights[1].intensity = 1f;

            Debug.Log("[PreviewWindow] PreviewRenderUtility initialized");
        }

        private void OnDisable()
        {
            Debug.Log("[PreviewWindow] OnDisable");

            if(_previewInstance != null)
            {
                DestroyImmediate(_previewInstance);
                _previewInstance = null;
                Debug.Log("[PreviewWindow] Destroyed preview instance");
            }

            _previewUtility?.Cleanup();
            _previewUtility = null;
            Debug.Log("[PreviewWindow] Cleaned up preview utility");
        }

        private void OnGUI()
        {
            if(_previewUtility == null)
            {
                EditorGUILayout.LabelField("Preview utility is not initialized.");
                return;
            }

            if(_prefab == null)
            {
                EditorGUILayout.LabelField("No prefab to preview.");
                return;
            }

            HandleInput();
            DrawPreview(_prefab);
        }

        private void HandleInput()
        {
            Event e = Event.current;
            Vector2 mousePos = e.mousePosition;
            bool repaintNeeded = false;

            // Scroll wheel = zoom
            if(e.type == EventType.ScrollWheel)
            {
                float scrollDelta = e.delta.y * 0.1f;
                float newDistance = Mathf.Clamp(_distance + scrollDelta, 0.1f, 100f);

                if(_distance != newDistance)
                {
                    _distance = newDistance;
                    repaintNeeded = true;  // We need to repaint since the zoom changed
                }

                e.Use();
            }

            // Handling Mouse Drag
            if(e.type == EventType.MouseDrag)
            {
                if(_lastMousePosition == Vector2.zero)  // Initialize only if it's the first drag frame
                {
                    _lastMousePosition = mousePos;
                }

                Vector2 delta = mousePos - _lastMousePosition;

                if(e.button == 0) // LMB = rotate
                {
                    float newYaw = _yaw + delta.x * 0.5f;
                    float newPitch = _pitch + delta.y * 0.5f;
                    newPitch = Mathf.Clamp(newPitch, -89f, 89f); // Clamping pitch to avoid upside-down view

                    if(_yaw != newYaw || _pitch != newPitch)
                    {
                        _yaw = newYaw;
                        _pitch = newPitch;
                        repaintNeeded = true;  // Repaint since the rotation changed
                    }
                }
                else if(e.button == 1) // RMB = pan
                {
                    var cam = _previewUtility.camera;
                    Vector3 right = cam.transform.right;
                    Vector3 up = cam.transform.up;

                    Vector3 newTargetPosition = _targetPosition - (right * delta.x + up * -delta.y) * 0.01f; // Adjust the pan speed

                    if(_targetPosition != newTargetPosition)
                    {
                        _targetPosition = newTargetPosition;
                        repaintNeeded = true;  // Repaint since the pan position changed
                    }
                }

                _lastMousePosition = mousePos; // Update last position after processing drag
            }

            // You can also reset _lastMousePosition on MouseDown to ensure proper dragging start
            if(e.type == EventType.MouseDown)
            {
                _lastMousePosition = mousePos;
            }

            // If anything changed, trigger a repaint
            if(repaintNeeded)
            {
                Repaint();
            }
        }

        private void DrawPreview(GameObject prefab)
        {
            if(_previewInstance == null)
            {
                Debug.Log($"[PreviewWindow] Instantiating preview for: {prefab.name}");

                _previewInstance = Instantiate(prefab);
                _previewInstance.transform.position = Vector3.zero;
                _previewInstance.transform.rotation = Quaternion.identity;

                foreach(var behaviour in _previewInstance.GetComponentsInChildren<MonoBehaviour>())
                {
                    behaviour.enabled = false;
                }

                _previewUtility.AddSingleGO(_previewInstance);

                // Compute bounds from all renderers (not just root)
                Renderer[] renderers = _previewInstance.GetComponentsInChildren<Renderer>();
                if(renderers.Length > 0)
                {
                    Bounds bounds = renderers[0].bounds;
                    foreach(Renderer r in renderers.Skip(1))
                    {
                        bounds.Encapsulate(r.bounds);
                    }

                    Vector3 boundsCenter = bounds.center;
                    float size = bounds.size.magnitude;

                    _previewUtility.camera.transform.position = boundsCenter + new Vector3(0, 0, -size * 2f);
                    _previewUtility.camera.transform.LookAt(boundsCenter);

                    Debug.Log("[PreviewWindow] Preview instance positioned using combined bounds");
                }
                else
                {
                    Debug.LogWarning("[PreviewWindow] No renderers found in prefab");
                }
            }

            // Check again in case no renderer exists
            Renderer[] allRenderers = _previewInstance.GetComponentsInChildren<Renderer>();
            if(allRenderers.Length == 0)
            {
                EditorGUILayout.HelpBox("No preview available (no Renderer found).", MessageType.Info);
                return;
            }

            // Camera control logic: orbit and pan based on input
            Vector3 direction = Quaternion.Euler(_pitch, _yaw, 0) * Vector3.forward;
            _previewUtility.camera.transform.position = _targetPosition - direction * _distance;
            _previewUtility.camera.transform.LookAt(_targetPosition);

            Rect previewRect = GUILayoutUtility.GetRect(position.width, position.height);
            _previewUtility.BeginPreview(previewRect, GUIStyle.none);
            _previewUtility.camera.Render();
            Texture resultRender = _previewUtility.EndPreview();

            if(resultRender != null)
                GUI.DrawTexture(previewRect, resultRender, ScaleMode.ScaleToFit, false);
            else
                EditorGUILayout.HelpBox("Failed to render preview texture.", MessageType.Warning);
        }
    }
}
