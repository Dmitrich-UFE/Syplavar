using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ProhibitedArea
{
    [SerializeField] private Transform point1, point2, point3, point4;

    internal bool InArea(Transform target)
    {
        Vector3 pos = target.position;
        List<Transform> activePoints = new List<Transform>();

        if (point1 != null) activePoints.Add(point1);
        if (point2 != null) activePoints.Add(point2);
        if (point3 != null) activePoints.Add(point3);
        if (point4 != null) activePoints.Add(point4);

        if (activePoints.Count == 0) return false;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;

        foreach (var p in activePoints)
        {
            if (p.position.x < minX) minX = p.position.x;
            if (p.position.x > maxX) maxX = p.position.x;
            if (p.position.z < minZ) minZ = p.position.z;
            if (p.position.z > maxZ) maxZ = p.position.z;
        }

        // Захватываем по 0.5 с каждой стороны
        return (pos.x >= minX - 0.5f && pos.x <= maxX + 0.5f &&
                pos.z >= minZ - 0.5f && pos.z <= maxZ + 0.5f);
    }
}
