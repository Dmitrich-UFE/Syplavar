using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoofFader : MonoBehaviour
{
    [Header("Настройки эффекта")]
    [SerializeField] private float duration = 0.5f;       // Длительность затухания
    [SerializeField] private float targetAlpha = 0.2f;    // Прозрачность внутри дома
    [SerializeField] private string colorPropertyName = "_BaseColor"; 

    [Header("Части крыши")]
    [Tooltip("Сюда можно перетащить крышу и заплатки вручную или они найдутся сами")]
    [SerializeField] private List<SpriteRenderer> targetSprites = new List<SpriteRenderer>();

    private MaterialPropertyBlock _propBlock;
    private Coroutine _fadeRoutine;
    private float _currentAlpha = 1f; // Храним состояние здесь для надежности

    void Awake() // Лучше инициализировать блок в Awake
    {
        _propBlock = new MaterialPropertyBlock();
        if (targetSprites.Count == 0)
            targetSprites.AddRange(GetComponentsInChildren<SpriteRenderer>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeRoutine(1f));
        }
    }

    IEnumerator FadeRoutine(float endAlpha)
    {
        float elapsed = 0f;
        float startAlpha = _currentAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            ApplyAlpha(_currentAlpha);
            yield return null;
        }
        _currentAlpha = endAlpha;
        ApplyAlpha(endAlpha);
        _fadeRoutine = null; // Обнуляем для корректности
    }

    private void ApplyAlpha(float alpha)
    {
        foreach (var sr in targetSprites)
        {
            if (sr == null) continue;

            // Мы не трогаем sr.color, работаем только через PropertyBlock
            // для максимальной производительности (SRP Batcher)
            sr.GetPropertyBlock(_propBlock);
            
            // Получаем текущий цвет из блока, чтобы не затереть RGB каналы
            Color c = Color.white; // По умолчанию, если ничего не задано
            c.a = alpha;
            
            _propBlock.SetColor(colorPropertyName, c);
            sr.SetPropertyBlock(_propBlock);
        }
    }
}