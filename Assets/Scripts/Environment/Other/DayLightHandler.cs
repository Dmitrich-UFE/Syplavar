using System;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class DayLightHandler : MonoBehaviour
{
    [SerializeField] private Light _directLight;

    //Пресеты градиентов
    [SerializeField] private Gradient LightGradient;
    [SerializeField] private Gradient RainyGradient;

    //Управляемый градиент
    private Gradient mainGradient;

    [SerializeField] private int DayDuration;
    [SerializeField] private Transform lightTransform;
    [SerializeField, Range(0f, 1f)] private float dayProgress;

    //События
    public delegate void OnTimeReached((int hh, int mm) time);
    public static event OnTimeReached? _OnTimeReached;
    private static List<(int hh, int mm)> Times;

    //Время
    public int Hours {get; private set;}
    public int Minutes {get; private set;}

    void Awake()
    {
        Times = new List<(int hh, int mm)>();
    }

    void Start()
    {
        mainGradient = LightGradient;
    }

    
    void Update()
    {
        lightTransform.localEulerAngles = new Vector3(0, dayProgress * 360, 0);
        dayProgress += Time.deltaTime / DayDuration;

        if (dayProgress > 1f) 
        {
            dayProgress = 0f;

            int num = UnityEngine.Random.Range(1, 10);

            mainGradient = num switch  
            {
                2 or 8 => RainyGradient,
                _ => LightGradient
            };
        }
        
        _directLight.color = mainGradient.Evaluate(dayProgress);

        (int hh, int mm) time = GetReachedTime();
        if (time.hh != -1) 
            _OnTimeReached.Invoke(time);
    }

    //Запись времени
    internal static void AddTime(int hh, int mm )
    {
        if ( hh >= 0 && hh <=23 && mm >= 0 && mm <=59)
            Times.Add((hh, mm));
        else 
            throw new ArgumentOutOfRangeException("minutes or/and hours is/are out of range");
    }

    //Удаление времени
    internal static void PopTime((int hh, int mm) time)
    {
        if (!Times.Remove( time ))
            throw new InvalidOperationException("This value cant be deleted because this value is no exists");
    }

    //если время в контейнере совпадает с нынешним, то возвращается подходящее время
    internal (int hh, int mm) GetReachedTime()
    {
        foreach (var time in Times)
        {
            if (time.hh == Hours && time.mm == Minutes)
                return time;
        }

        return (-1, -1);
    }
}
