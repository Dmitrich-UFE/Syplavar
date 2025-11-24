using System;
using System.Collections.Generic;
using Unity.Mathematics;
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

    private static float daySpeedMultiple = 1;
    private static float timeSpeedDuringSleep;
    [SerializeField] private float NonStaticTimeSpeedDuringSleep;

    //События
    public delegate void OnTimeReached((int hh, int mm) time);
    public static event OnTimeReached? _OnTimeReached;
    private static Dictionary<(int hh, int mm), bool> Times = new Dictionary<(int hh, int mm), bool>();

    //Время
    public int Hours {get; private set;}
    public int Minutes {get; private set;}

    void Awake()
    {
        DayLightHandler.AddTime(12, 00);
        DayLightHandler.AddTime(18, 00);
        DayLightHandler.AddTime(22, 00);
        DayLightHandler.AddTime(00, 00);
        DayLightHandler.AddTime(02, 00);
        DayLightHandler.AddTime(06, 00);
        DayLightHandler.AddTime(07, 00);
    }

    void Start()
    {
        DayLightHandler.timeSpeedDuringSleep = NonStaticTimeSpeedDuringSleep;
        mainGradient = LightGradient;
    }

    
    void Update()
    {
        //движение солнца и счёт времени
        lightTransform.localEulerAngles = new Vector3(0, dayProgress * 360, 0);
        dayProgress += Time.deltaTime / DayDuration * daySpeedMultiple;
        Hours = (int)Math.Floor(dayProgress * 24);
        Minutes = (int)Math.Floor(dayProgress * 1440 % 60);

        //выбор солнечного или облачного дня и действия при смене дня
        if (dayProgress > 1f) 
        {
            dayProgress = 0f;
            ClearUsedTimes();

            int num = UnityEngine.Random.Range(1, 10);

            mainGradient = num switch  
            {
                2 or 8 => RainyGradient,
                _ => LightGradient
            };
        }
        
        _directLight.color = mainGradient.Evaluate(dayProgress);

        //обработка временных событий
        (int hh, int mm) time = GetReachedTime();
        if (time.hh != -1 && !Times[time]) 
        {
            Times[time] = true;
            _OnTimeReached.Invoke(time);
        }
            
    }

    //Запись времени
    internal static void AddTime(int hh, int mm )
    {
        if ( hh >= 0 && hh <=23 && mm >= 0 && mm <=59)
            Times.TryAdd((hh, mm), false);
        else 
            throw new ArgumentOutOfRangeException("minutes or/and hours is/are out of range");
    }

    //Удаление времени
    internal static void PopTime(int hh, int mm)
    {
        if (!Times.Remove( (hh, mm) ))
            throw new InvalidOperationException("This value cant be deleted because this value is no exists");
    }

    //ускорение хода времени для сна
    public static void SpeedupForSleep()
    {
        if (timeSpeedDuringSleep > 0)
        {
            daySpeedMultiple = timeSpeedDuringSleep;
            DayLightHandler._OnTimeReached += CheckWakeTime;
        }
        else
        {
            Debug.LogWarning("Time multiple is negative. Please make it positive");
        }
    }

    //вспомогательный метод для ускорения времени во время сна
    private static void CheckWakeTime((int hh, int mm) time)
    {
        if (time == (07, 00))
        {
            daySpeedMultiple = 1;
            DayLightHandler._OnTimeReached -= CheckWakeTime;
        }

    }

    //если время в контейнере совпадает с нынешним, то возвращается подходящее время
    internal (int hh, int mm) GetReachedTime()
    {
        foreach (var time in Times)
        {
            if (math.abs((time.Key.hh * 60 + time.Key.mm) - (Hours * 60 + Minutes)) < 2)
                return time.Key;
        }

        return (-1, -1);
    }

    //очистка использований времён
    private void ClearUsedTimes()
    {
        List<(int h, int m)> keyTimes = new List<(int h, int m)>(Times.Keys);
        foreach ((int h, int m) time in keyTimes)
        {
            Times[time] = false;
        }
    }
}
