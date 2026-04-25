using System;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.VisualScripting;

//[ExecuteInEditMode]
public class DayLightHandler : MonoBehaviour
{
    [SerializeField] private Light _directLight;

    //Пресеты градиентов
    [SerializeField] private Gradient LightGradient;
    [SerializeField] private Gradient RainyGradient;

    //Управляемый градиент
    private Gradient mainGradient;

    //Градиенты для UI
    [SerializeField] private Gradient LightUIGradient;
    [SerializeField] private Gradient RainyUIGradient;
    private Gradient mainUIGradient;

    [SerializeField] private int dayDuration;
    [SerializeField] private Transform lightTransform;
    [SerializeField] private float delta;
    [SerializeField, Range(0f, 1f)] private float dayProgress;

    private static float daySpeedMultiple = 1;
    private static float timeSpeedDuringSleep;
    private static int StaticDayDuration;
    [SerializeField] private float NonStaticTimeSpeedDuringSleep;

    //События
    public delegate void OnTimeReached((int hh, int mm) time);
    public static event OnTimeReached _OnTimeReached;
    private static Dictionary<(int hh, int mm), bool> Times = new Dictionary<(int hh, int mm), bool>();

    //Время
    public static int Hours {get; private set;}
    public static int Minutes {get; private set;}
    public static int DayDuration => StaticDayDuration;
    private static float daySpeedMultipleStatic = 1; 
    private static Coroutine AddMindCoroutine;

    //Для раскраски интерфейса 
    public static Color ActualDayColor {get; private set;}

    //Одиночка
    private static DayLightHandler instance;


    void Awake()
    {
        instance = this;
        Times.Clear();
        DayLightHandler.timeSpeedDuringSleep = NonStaticTimeSpeedDuringSleep;
        StaticDayDuration = dayDuration;
        mainGradient = LightGradient;
        mainUIGradient = LightUIGradient;
        _directLight.color = mainGradient.Evaluate(dayProgress);
        ActualDayColor = mainUIGradient.Evaluate(dayProgress);

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
        
    }

    
    void FixedUpdate()
    {
        //движение солнца и счёт времени
        lightTransform.localEulerAngles = new Vector3(0, (dayProgress * 360 + delta) % 360, 0);
        dayProgress += Time.deltaTime / dayDuration * daySpeedMultiple;
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

            mainUIGradient = num switch  
            {
                2 or 8 => RainyUIGradient,
                _ => LightUIGradient
            };
        }
        
        _directLight.color = mainGradient.Evaluate(dayProgress);
        ActualDayColor = mainUIGradient.Evaluate(dayProgress);

        //обработка временных событий
        (int hh, int mm) time = GetReachedTime();
        if (time.hh != -1 && !Times[time]) 
        {
            Times[time] = true;
            _OnTimeReached?.Invoke(time);
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
        if (timeSpeedDuringSleep > 0 && StaticDayDuration / timeSpeedDuringSleep < 5)
            Debug.LogWarning("Time Skip is too fast. Please increase DayDuration or decrease NonStatictimeSpeedDuringSleep");

        if (timeSpeedDuringSleep > 0)
        {
            daySpeedMultiple = timeSpeedDuringSleep;
            daySpeedMultipleStatic = daySpeedMultiple;
            AddMindCoroutine = instance.StartCoroutine(AddMind());
            DayLightHandler._OnTimeReached += CheckWakeTime;
            
            //обратиться к рассудку
            //включить свою корутину, чтобы увеличить рассудок
        }
        else
        {
            Debug.LogWarning("Time multiple is negative. Please make it positive");
        }
    }

    static IEnumerator AddMind()
    {
        PlayerMind _playerMind = PlayerSeeker.GetPlayerMind();
        _playerMind.StopMindDrain();
        while (_playerMind.MindPercent < 99.9f)
        {
            _playerMind.ChangeMind(_playerMind.MaxMind / 100);
            yield return new WaitForSecondsRealtime(DayDuration / daySpeedMultipleStatic / 150f);
        }
    }

    //вспомогательный метод для ускорения времени во время сна
    private static void CheckWakeTime((int hh, int mm) time)
    {
        //Debug.Log($"{time.hh} {time.mm}");
        if (time == (07, 00))
        {
            daySpeedMultiple = 1;
            daySpeedMultipleStatic = daySpeedMultiple;
            DayLightHandler._OnTimeReached -= CheckWakeTime;

            if (AddMindCoroutine != null) instance.StopCoroutine(AddMindCoroutine);
            //отключить корутину для накрутки рассудка
        }

    }

    //если время в контейнере совпадает с нынешним, то возвращается подходящее время
    internal (int hh, int mm) GetReachedTime()
    {
        foreach (var time in Times)
        {
            if (math.abs((time.Key.hh * 60f + time.Key.mm) - (Hours * 60f + Minutes)) <= 3f)
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

    //Метод для глушения цветов
    private Color GetMutedColor(Color originalColor, float saturationFactor = 0.5f, float valueFactor = 0.9f)
    {
        // 1. Переводим RGB в HSV
        Color.RGBToHSV(_directLight.color, out float h, out float s, out float v);

        // 2. Снижаем насыщенность (делаем цвет "серым")
        s *= saturationFactor; 

        // 3. Слегка приглушаем яркость (чтобы не был слишком светлым)
        v *= valueFactor;

        // 4. Возвращаем обратно в RGB
        return Color.HSVToRGB(h, s, v);
    }

    void OnDestroy()
    {
        DayLightHandler._OnTimeReached -= CheckWakeTime;
    }
}
