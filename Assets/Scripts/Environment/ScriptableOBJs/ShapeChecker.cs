using UnityEngine;

[CreateAssetMenu(fileName = "ShapeChecker", menuName = "Shapes/ShapeChecker")]
public class ShapeChecker : ScriptableObject
{
    [SerializeField] internal Vector3 _LB;
    [SerializeField] internal Vector3 _LT;
    [SerializeField] internal Vector3 _RT;
    [SerializeField] internal Vector3 _RB;

    private const float Padding = 0.5f;
    private const float Threshold = 1000000f;

    internal bool InArea(Vector3 pos)
    {
        Vector2 p = new Vector2(pos.x, pos.z);
        Vector2[] vertices = GetValidPoints();
        int count = vertices.Length;

        if (count == 0) return false;
        if (count == 1) return Vector2.Distance(p, vertices[0]) <= Padding;
        if (count == 2) return DistanceToSegment(p, vertices[0], vertices[1]) <= Padding;

        // Для 3 и 4 точек используем расстояние до многоугольника
        return DistanceToPolygon(p, vertices) <= Padding;
    }

    private float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ba = b - a;
        Vector2 pa = p - a;
        float h = Mathf.Clamp01(Vector2.Dot(pa, ba) / Vector2.Dot(ba, ba));
        return (pa - ba * h).magnitude;
    }

    private float DistanceToPolygon(Vector2 p, Vector2[] v)
    {
        float minD = float.MaxValue;
        bool inside = false;

        // 1. Находим минимальное расстояние до границ (для внешних точек)
        for (int i = 0, j = v.Length - 1; i < v.Length; j = i++)
        {
            // Расстояние до ребра
            minD = Mathf.Min(minD, DistanceToSegment(p, v[i], v[j]));

            // Проверка на нахождение внутри (Raycast)
            if (((v[i].y > p.y) != (v[j].y > p.y)) &&
                (p.x < (v[j].x - v[i].x) * (p.y - v[i].y) / (v[j].y - v[i].y) + v[i].x))
            {
                inside = !inside;
            }
        }

        // 2. Если точка внутри, расстояние до "области" равно 0 (или отрицательное в SDF)
        // Нам достаточно вернуть 0, чтобы условие <= Padding всегда выполнялось
        return inside ? 0f : minD;
    }

    public Vector2[] GetValidPoints()
    {
        var all = new[] { _LB, _LT, _RT, _RB };
        int count = 0;
        for (; count < all.Length; count++) if (all[count].y >= Threshold) break;
        
        Vector2[] result = new Vector2[count];
        for (int i = 0; i < count; i++) result[i] = new Vector2(all[i].x, all[i].z);
        return result;
    }

    // Обновленный метод для корректного гизмо (теперь рисует расширенную зону)
    internal void DrawDebugGizmos(Transform owner)
    {
        Vector2[] pts = GetValidPoints();
        if (pts.Length == 0) return;

        Gizmos.color = new Color(1, 0, 0, 0.6f);
        float y = _LB.y < Threshold ? _LB.y : 0;

        // Для визуализации честного Padding 0.5 лучше всего подходит проход по сетке 
        // или рисование толстых линий. Для упрощения нарисуем контур и углы:
        for (int i = 0; i < pts.Length; i++)
        {
            Vector3 pCurrent = new Vector3(pts[i].x, 0.5f, pts[i].y);
            Gizmos.DrawWireSphere(pCurrent, Padding); // Углы

            if (pts.Length > 1)
            {
                if (pts.Length == 2 && i == 1) break;
                Vector2 next = pts[(i + 1) % pts.Length];
                Vector3 pNext = new Vector3(next.x, 0.5f, next.y);
                
                // Рисуем основное ребро
                Gizmos.DrawLine(pCurrent, pNext);
                
                // Рисуем внешние границы отступа (параллельные линии)
                Vector3 dir = (pNext - pCurrent).normalized;
                Vector3 normal = new Vector3(-dir.z, 0.5f, dir.x) * Padding;
                Gizmos.DrawLine(pCurrent + normal, pNext + normal);
                Gizmos.DrawLine(pCurrent - normal, pNext - normal);
            }
        }
    }
}
