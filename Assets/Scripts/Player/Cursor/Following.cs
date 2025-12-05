using UnityEngine;

public class Following : MonoBehaviour
{
    [SerializeField] private Transform follow;
    [SerializeField] private float timePerScanPos;
    [SerializeField] private float interParam; //0f to 1f
    private Transform thisTransform;
    private Vector3 pos;

    void Start()
    {
        thisTransform = GetComponent<Transform>();
        InvokeRepeating("SetPos", 0f, timePerScanPos);
    }
    
    void Update()
    {
        thisTransform.position = Vector3.Lerp(thisTransform.position, pos, interParam);
    }

    void SetPos()
    {
        pos = follow.position;
    }
}