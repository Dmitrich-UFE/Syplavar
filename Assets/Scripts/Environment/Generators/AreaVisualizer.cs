using UnityEngine;

public class AreaVisualizer : MonoBehaviour
{
    [SerializeField] private ShapeChecker _data;

    private void OnDrawGizmos()
    {
        if (_data != null)
        {
            _data.DrawDebugGizmos(transform);
        }
    }
}
