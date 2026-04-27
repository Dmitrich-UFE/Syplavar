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

    void Start()
    {
        _propBlock = new MaterialPropertyBlock();

        if (targetSprites.Count == 0)
        {
            targetSprites.AddRange(GetComponentsInChildren<SpriteRenderer>());
        }
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
        float startAlpha = GetCurrentAlpha();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            ApplyAlpha(newAlpha);
            yield return null;
        }
        ApplyAlpha(endAlpha);
    }

    private float GetCurrentAlpha()
    {
        if (targetSprites.Count > 0 && targetSprites[0] != null) 
            return targetSprites[0].color.a;
        return 1f;
    }

    private void ApplyAlpha(float alpha)
    {
        foreach (var sr in targetSprites)
        {
            if (sr == null) continue;

            Color c = sr.color;
            c.a = alpha;
            sr.color = c;

            sr.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(colorPropertyName, c);
            sr.SetPropertyBlock(_propBlock);
        }
    }
}