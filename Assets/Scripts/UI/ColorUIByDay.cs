using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ColorUIByDay : MonoBehaviour
{
    private Image _img;
    private Color _color;
    [SerializeField] private bool IsBlendWithStartColor;
    Coroutine colorCoroutine;

    void Start()
    {
        _img = GetComponent<Image>();
        
        if (IsBlendWithStartColor)
            _color = _img.color;
        else
            _color = new Color(1f, 1f, 1f, 1f);

        colorCoroutine = StartCoroutine(ColorUI());
    }
    
    IEnumerator ColorUI()
    {
        while(true)
        {
            _img.color = DayLightHandler.ActualDayColor * _color;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    void OnDestroy()
    {
        StopCoroutine(colorCoroutine);
    }

    void OnDisable()
    {
        StopCoroutine(colorCoroutine);
    }

    void OnEnable()
    {
        if (_img == null) 
        {
            _img = GetComponent<Image>();

            if (IsBlendWithStartColor)
                _color = _img.color;
            else
                _color = new Color(1f, 1f, 1f, 1f);
        }
        colorCoroutine = StartCoroutine(ColorUI());
    }
}
