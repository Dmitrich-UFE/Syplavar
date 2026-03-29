using UnityEngine;

public class Following : MonoBehaviour
{
    [SerializeField] private Transform follow;
    private Transform thisTransform;
    void Start()
    {
        thisTransform = GetComponent<Transform>();
    }
    
    void Update()
    {
        thisTransform.position = follow.position;
    }
}