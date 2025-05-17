using UnityEditor;
using UnityEngine;

namespace Prototypes.Interactables.Triggers.Editor
{
    internal sealed class TriggersGizmos
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawTriggerGizmos(EnterExitTrigger trigger, GizmoType gizmoType)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // orange-ish
            string triggerName = trigger.GetType().Name;

            foreach(var collider in trigger.GetComponents<Collider>())
            {
                Bounds bounds;
                string label;
                Matrix4x4 matrix;
                Vector3 labelWorldTop;

                switch(collider)
                {
                    case BoxCollider box:
                        matrix = box.transform.localToWorldMatrix;
                        bounds = new Bounds(box.center, box.size);
                        Gizmos.matrix = matrix;
                        Gizmos.DrawWireCube(bounds.center, bounds.size);
                        label = $"{triggerName}\n(Box)";
                        labelWorldTop = matrix.MultiplyPoint3x4(bounds.center + new Vector3(0, bounds.extents.y, 0));
                        break;

                    case SphereCollider sphere:
                        matrix = sphere.transform.localToWorldMatrix;
                        bounds = new Bounds(sphere.center, Vector3.one * sphere.radius * 2);
                        Gizmos.matrix = matrix;
                        Gizmos.DrawWireSphere(bounds.center, sphere.radius);
                        label = $"{triggerName}\n(Sphere)";
                        labelWorldTop = matrix.MultiplyPoint3x4(bounds.center + new Vector3(0, bounds.extents.y, 0));
                        break;

                    case CapsuleCollider capsule:
                        bounds = capsule.bounds;
                        Gizmos.matrix = Matrix4x4.identity;
                        Gizmos.DrawWireCube(bounds.center, bounds.size);
                        label = $"{triggerName}\n(Capsule)";
                        labelWorldTop = bounds.center + new Vector3(0, bounds.extents.y, 0);
                        break;

                    case MeshCollider mesh when mesh.sharedMesh != null:
                        matrix = mesh.transform.localToWorldMatrix;
                        bounds = mesh.sharedMesh.bounds;
                        Gizmos.matrix = matrix;
                        Gizmos.DrawWireCube(bounds.center, bounds.size);
                        label = $"{triggerName}\n(Mesh)";
                        labelWorldTop = matrix.MultiplyPoint3x4(bounds.center + new Vector3(0, bounds.extents.y, 0));
                        break;

                    default:
                        bounds = collider.bounds;
                        Gizmos.matrix = Matrix4x4.identity;
                        Gizmos.DrawWireCube(bounds.center, bounds.size);
                        label = $"{triggerName}\n({collider.GetType().Name})";
                        labelWorldTop = bounds.center + new Vector3(0, bounds.extents.y, 0);
                        break;
                }

                Gizmos.matrix = Matrix4x4.identity;

                Handles.Label(labelWorldTop, label, EditorStyles.boldLabel);
            }
        }
    }
}
