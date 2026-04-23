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

    void Start()
    {
        _img = GetComponent<Image>();
        
        if (IsBlendWithStartColor)
            _color = new Color(_img.color.r, _img.color.g, _img.color.b, _img.color.a);
        else
            _color = new Color(1f, 1f, 1f, 1f);

        colorCoroutine = StartCoroutine(ColorUI());
    }
    
    IEnumerator ColorUI()
    {
        while(_img != null)
        {
            _totalColor = DayLightHandler.ActualDayColor * _color;
            _img.color = _totalColor;
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
        if (_img == null) 
        {
            _img = GetComponent<Image>();

            if (IsBlendWithStartColor)
                _color = _color = new Color(_img.color.r, _img.color.g, _img.color.b, _img.color.a);
            else
                _color = new Color(1f, 1f, 1f, 1f);
        }

        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(ColorUI());
    }
}
