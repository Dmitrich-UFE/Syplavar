using System;
using UnityEngine;

//[ExecuteInEditMode]
public class DayLightHandler : MonoBehaviour
{
    [SerializeField] private Light _directLight;

    [SerializeField] private Gradient LightGradient;
    //[SerializeField] private Transform lightTransform;
    //[SerializeField] private Gradient CloudyGradient;

    private Gradient mainGradient;
    [SerializeField] private int DayDuration;
    [SerializeField, Range(0f, 1f)] private float dayProgress;
    //private int IsDayCloudy;

    void Start()
    {
        mainGradient = LightGradient;
    }

    // Update is called once per frame
    void Update()
    {
        //lightTransform.localEulerAngles = new Vector3(dayProgress * 360 - 90, dayProgress * 360, dayProgress * 360 - 90);
        dayProgress += Time.deltaTime / DayDuration;

        if (dayProgress > 1f) dayProgress = 0f;

        
        _directLight.color = mainGradient.Evaluate(dayProgress);
    }
}
