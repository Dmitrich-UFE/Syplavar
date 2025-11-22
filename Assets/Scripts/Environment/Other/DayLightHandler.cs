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
    private static Dictionary<(int hh, int mm), bool> Times = new Dictionary<(int hh, int mm), bool>();

    //Время
    public int Hours {get; private set;}
    public int Minutes {get; private set;}

    public int hours;
    public int minutes;

    void Awake()
    {
        
    }

    void Start()
    {
        mainGradient = LightGradient;
    }

    
    void Update()
    {
        //движение солнца и счёт времени
        lightTransform.localEulerAngles = new Vector3(0, dayProgress * 360, 0);
        dayProgress += Time.deltaTime / DayDuration;
        Hours = (int)Math.Floor(dayProgress * 24);
        Minutes = (int)Math.Floor(dayProgress * 1440 % 60);
        hours = Hours;
        minutes = Minutes;


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
            _OnTimeReached.Invoke(time);
            Times[time] = true;
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

    //если время в контейнере совпадает с нынешним, то возвращается подходящее время
    internal (int hh, int mm) GetReachedTime()
    {
        foreach (var time in Times)
        {
            if ((time.Key.hh * 60 + time.Key.mm) == (Hours * 60 + Minutes))
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
