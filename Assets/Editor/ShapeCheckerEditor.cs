using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeChecker))]
public class ShapeCheckerEditor : Editor
{
    private void OnSceneGUI()
    {
        ShapeChecker t = (ShapeChecker)target;
        Vector2[] pts = t.GetValidPoints();
        if (pts.Length == 0) return;

        Handles.color = new Color(1, 0, 0, 0.5f);
        
        // Рисуем точки и линии с учетом Padding
        for (int i = 0; i < pts.Length; i++)
        {
            Vector3 worldPos = new Vector3(pts[i].x, 1, pts[i].y);
            // Визуализация Padding вокруг углов
            Handles.DrawSolidDisc(worldPos, Vector3.up, 0.5f);

            // Визуализация линий (ребер)
            if (pts.Length > 1)
            {
                Vector2 next = pts[(i + 1) % pts.Length];
                // Если всего 2 точки, не зацикливаем линию
                if (pts.Length == 2 && i == 1) continue;

                Vector3 nextWorldPos = new Vector3(next.x, 0, next.y);
                Handles.DrawAAConvexPolygon(
                    worldPos + GetNormal(worldPos, nextWorldPos) * 0.5f,
                    nextWorldPos + GetNormal(worldPos, nextWorldPos) * 0.5f,
                    nextWorldPos - GetNormal(worldPos, nextWorldPos) * 0.5f,
                    worldPos - GetNormal(worldPos, nextWorldPos) * 0.5f
                );
            }
        }
    }

    private Vector3 GetNormal(Vector3 a, Vector3 b)
    {
        Vector3 dir = (b - a).normalized;
        return new Vector3(-dir.z, 0, dir.x);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
         SceneView.RepaintAll();
        }
    }

}
