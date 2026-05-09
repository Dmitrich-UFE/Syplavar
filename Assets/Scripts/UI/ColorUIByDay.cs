using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ColorUIByDay : MonoBehaviour
{
    private Image _img;
    [SerializeField] private Color _color;
    [SerializeField] private Color _totalColor;
    [SerializeField] private bool IsBlendWithStartColor;
    Coroutine colorCoroutine;
    bool _isInitialized;

    void Awake() // Или в Start, но только один раз
{
    if (!_isInitialized)
    {
        _img = GetComponent<Image>();
        _color = IsBlendWithStartColor ? _img.color : Color.white;
        _isInitialized = true;
    }
}

    void Start()
    {
        colorCoroutine = StartCoroutine(ColorUI());
    }
    
    IEnumerator ColorUI()
    {
        while(_img != null)
        {
            _img.color = DayLightHandler.ActualDayColor * _color;
            yield return new WaitForSecondsRealtime(1.5f);
        }
    }

    void OnDestroy()
    {
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
    }

    void OnDisable()
    {
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
    }

    void OnEnable()
    {
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(ColorUI());
    }
}
