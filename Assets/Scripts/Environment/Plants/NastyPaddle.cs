using UnityEngine;
using System.Collections;

public class NastyPaddle : MonoBehaviour
{
    private PlayerMind mind;
    private GameObject player;
    private Coroutine coroutine;
    [SerializeField] private int damage;
    private WaitForSecondsRealtime timeInstruct;
    void Awake()
    {
        mind = PlayerSeeker.GetPlayerMind();
        player = PlayerSeeker.GetPlayer();
        timeInstruct = new WaitForSecondsRealtime(1f);
    }

    private void OnTriggerEnter(Collider interactableObject)
    {
        if (interactableObject.CompareTag("Player"))
        {
            if (coroutine == null) coroutine = StartCoroutine(damagePlayer());
        }
    }

    private void OnTriggerExit(Collider interactableObject)
    {
        if (interactableObject.CompareTag("Player"))
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private IEnumerator damagePlayer()
    {
        while (mind.CurrentMind > 0)
        {
            if (player.activeSelf) mind.ChangeMind(-damage);
            yield return timeInstruct;
        }
    }
}
