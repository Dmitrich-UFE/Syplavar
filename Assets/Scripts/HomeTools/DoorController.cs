using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f; 
    public float speed = 5f;
    [Header("Настройка направления")]
    public bool invertDirection = false; 
    
    private Quaternion closedRotation;
    private float currentOpenAngle = 0f;
    private bool isPlayerNear = false;

    void Start()
    {
        closedRotation = transform.rotation;
        if (GetComponent<Collider>() == null)
            Debug.LogError($"На объекте {name} нет коллайдера!");
    }

    void Update()
    {
        Quaternion targetRotation = isPlayerNear ? 
            closedRotation * Quaternion.Euler(0, currentOpenAngle, 0) : closedRotation;
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.CompareTag("Player")) 
        {
            Vector3 localPlayerPos = transform.InverseTransformPoint(other.transform.position);

            float calculatedAngle = (localPlayerPos.z > 0) ? openAngle : -openAngle;

            currentOpenAngle = invertDirection ? -calculatedAngle : calculatedAngle;

            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.CompareTag("Player")) 
        {
            isPlayerNear = false;
        }
    }
}