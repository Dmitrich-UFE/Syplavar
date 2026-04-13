using System;
using System.Collections;
using UnityEngine;

public class PlayerMind : MonoBehaviour
{
    // ===== FIELDS =====
    [SerializeField] private int maxMind;
    [SerializeField] private int startMind;

    private int currentMind; // не в инспекторе

    private PlayerHealth playerHealth;
    private Coroutine mindCoroutine;

    // ===== EVENT =====
    public event Action OnMindChanged;

    // ===== PROPERTIES =====
    internal int CurrentMind => currentMind;
    internal int MaxMind => maxMind;

    internal float MindPercent => maxMind > 0 ? (float)currentMind / maxMind : 0f;

    // ===== UNITY =====
    private void Awake()
    {
        currentMind = startMind;

        playerHealth = PlayerSeeker.GetPlayerHealth();

        OnMindChanged?.Invoke();
    }

    private void Start()
    {
        mindCoroutine = StartCoroutine(MindDrainCoroutine());
    }

    // ===== METHODS =====

    // Изменение рассудка
    internal void ChangeMind(int value)
    {
        currentMind += value;
        currentMind = Mathf.Clamp(currentMind, 0, maxMind);

        OnMindChanged?.Invoke();
    }

    // Сброс рассудка
    internal void ResetMind()
    {
        currentMind = maxMind;

        StopMindDrain();
        ResumeMindDrain();

        OnMindChanged?.Invoke();
    }

    // Остановить корутину
    internal void StopMindDrain()
    {
        if (mindCoroutine != null)
        {
            StopCoroutine(mindCoroutine);
            mindCoroutine = null;
        }
    }

    // Возобновить корутину
    internal void ResumeMindDrain()
    {
        if (mindCoroutine == null && currentMind > 0)
        {
            mindCoroutine = StartCoroutine(MindDrainCoroutine());
        }
    }

    // ===== COROUTINE =====
    private IEnumerator MindDrainCoroutine()
    {
        if (maxMind <= 0)
            yield break;

        float totalTime = DayLightHandler.DayDuration * 3f;
        float delay = totalTime / maxMind;

        while (currentMind > 0)
        {
            yield return new WaitForSeconds(delay);

            currentMind--;

            OnMindChanged?.Invoke();
        }

        // Рассудок закончился: если игрок неожиданно умирает  - проблема может быть здесь
        currentMind = 0;
        playerHealth.Health = 0;


        //Debug.Log("Чел умер");
    }
}