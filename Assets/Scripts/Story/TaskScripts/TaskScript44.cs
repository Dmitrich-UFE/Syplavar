using UnityEngine;
using System.Collections;

public class TaskScript44 : MonoBehaviour
{
    
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameObject DangerSlime;

    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Galiver;
    [SerializeField] private float speed;

    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;
    private Transform playerTransform;

    void OnEnable()
    {
        playerTransform = PlayerSeeker.GetPlayerTransform();
        vnl.StartPrint(text);
        cor = StartCoroutine(checkStatus());
    }

    void OnDisable()
    {
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }

    IEnumerator checkStatus()
    {
        while (!(vnl.Status == VNLprintStatus.ended))
        {
            yield return tick;
        }

        cor = StartCoroutine(DeadPlayer());
    }

    IEnumerator DeadPlayer()
    {
        DangerSlime.SetActive(true);

        while (Mathf.Abs(DangerSlime.transform.position.x - playerTransform.position.x) > 0.005f &&
            Mathf.Abs(DangerSlime.transform.position.z - playerTransform.position.z) > 0.005f)
        {
            Vector3 Dir = Vector3.MoveTowards(DangerSlime.transform.position, playerTransform.position, speed * Time.deltaTime);
            DangerSlime.transform.position = new Vector3(Dir.x, 0.6f, Dir.z); 
            yield return null;
        }
        
        taskManager.OpenBlackPanelNoFade();
        Player.SetActive(false);
        DangerSlime.SetActive(false);
        cor = StartCoroutine(End());
    }

    IEnumerator End()
    {
        Galiver.transform.position = playerTransform.position;
        Galiver.SetActive(true);
        taskManager.CloseBlackPanel();

        yield return new WaitForSecondsRealtime(5f);

        taskManager.OpenBlackPanel();

        yield return new WaitForSecondsRealtime(4f);

        Player.SetActive(true);
        Galiver.SetActive(false);
        taskManager.CompleteTask(task.ID);
    }

}
