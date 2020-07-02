using UnityEngine;
using UnityEditor;

using PatrolPath = Platformer.Mechanics.PatrolPath;

namespace Platformer.Editor
{
    [CustomEditor(typeof(PatrolPath))]
    public class PatrolPathGizmo : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            PatrolPath path = target as PatrolPath;
            using (EditorGUI.ChangeCheckScope cc = new EditorGUI.ChangeCheckScope())
            {
                Vector3 sp = path.CachedTransform.InverseTransformPoint(Handles.PositionHandle(path.CachedTransform.TransformPoint(path.stPos), path.CachedTransform.rotation));
                Vector3 ep = path.CachedTransform.InverseTransformPoint(Handles.PositionHandle(path.CachedTransform.TransformPoint(path.edPos), path.CachedTransform.rotation));

                if(cc.changed)
                {
                    path.stPos = sp;
                    path.edPos = ep;
                }
            }

            Handles.Label(path.CachedTransform.position, (path.stPos - path.edPos).magnitude.ToString());
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        static void OnDrawGizmo(PatrolPath path, GizmoType gizmoType)
        {
            Vector3 st = path.CachedTransform.TransformPoint(path.stPos);
            Vector3 ed = path.CachedTransform.TransformPoint(path.edPos);
            Handles.color = Color.yellow;
            Handles.Label(path.CachedTransform.position, path.gameObject.name);
            Handles.Label(path.CachedTransform.position - Vector3.up * 0.3f, (st - ed).magnitude.ToString());
            Handles.Label(st, "ST");
            Handles.Label(ed, "ED");
            Handles.DrawDottedLine(st, ed, 5);
            Handles.DrawSolidDisc(st, path.CachedTransform.forward, 0.1f);
            Handles.DrawSolidDisc(ed, path.CachedTransform.forward, 0.1f);
        }
    }
}


