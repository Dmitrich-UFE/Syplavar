using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterGenerator : MonoBehaviour
{
    public static MonsterGenerator instance;

    [Header("Настройки монстров")]
    [SerializeField] private GameObject monsterType1;
    [SerializeField] private GameObject monsterType2;
    [SerializeField] private int maxCountType1 = 5;
    [SerializeField] private int maxCountType2 = 5;

    [Header("Зоны запрета")]
    [SerializeField] private ProhibitedArea[] prohibitedAreas;

    [Header("Дистанции")]
    [SerializeField] private float minMonsterGenDistance = 10f;
    [SerializeField] private float maxMonsterGenDistance = 30f;
    [SerializeField] private float maxMonsterExistDistance = 50f;

    private List<GameObject> activeMonsters = new List<GameObject>();
    private List<GameObject> monstersBuffer = new List<GameObject>();

    private bool isGenerationTime = false;

    #region Свойства
    public float MinMonsterGenDistance
    {
        get => minMonsterGenDistance;
        set => minMonsterGenDistance = Mathf.Clamp(value, 0, maxMonsterGenDistance - 0.1f);
    }

    public float MaxMonsterGenDistance
    {
        get => maxMonsterGenDistance;
        set
        {
            maxMonsterGenDistance = Mathf.Clamp(value, minMonsterGenDistance + 0.1f, maxMonsterExistDistance - 0.1f);
        }
    }

    public float MaxMonsterExistDistance
    {
        get => maxMonsterExistDistance;
        set => maxMonsterExistDistance = Mathf.Max(value, maxMonsterGenDistance + 0.1f);
    }

    public int MaxCountType1 => maxCountType1;
    public int MaxCountType2 => maxCountType2;
    #endregion

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Подписка на событие из твоего DayLightHandler
        DayLightHandler._OnTimeReached += CheckTime;

        // Первоначальная проверка текущего времени
        CheckTime((DayLightHandler.Hours, DayLightHandler.Minutes));

        StartCoroutine(MonsterLifecycleRoutine());
    }

    void OnDestroy()
    {
        // Обязательная отписка, чтобы не было ошибок при смене сцены
        DayLightHandler._OnTimeReached -= CheckTime;
    }

    private void CheckTime((int hh, int mm) time)
    {
        // Монстры генерируются с 18:00 до 06:00
        //isGenerationTime = (time.hh >= 18 || time.hh < 6);
        isGenerationTime = true;
    }

    IEnumerator MonsterLifecycleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            Transform player = PlayerSeeker.GetPlayerTransform();
            if (player == null) continue;

            // 1. Проверка активных монстров: дистанция или неактивность
            for (int i = activeMonsters.Count - 1; i >= 0; i--)
            {
                GameObject monster = activeMonsters[i];
                if (monster == null)
                {
                    activeMonsters.RemoveAt(i);
                    continue;
                }

                float dist = Vector3.Distance(monster.transform.position, player.position);

                if (dist > maxMonsterExistDistance || !monster.activeSelf)
                {
                    DeactivateMonster(monster);
                }
            }

            // 2. Попытка генерации, если сейчас подходящее время суток
            if (isGenerationTime)
            {
                TrySpawnMonster(monsterType1, maxCountType1);
                TrySpawnMonster(monsterType2, maxCountType2);
            }
        }
    }

    private void TrySpawnMonster(GameObject prefab, int maxCount)
    {
        if (prefab == null) return;

        // Считаем сколько монстров этого типа сейчас активно
        int currentCount = 0;
        foreach (var m in activeMonsters)
            if (m.name.StartsWith(prefab.name)) currentCount++;

        if (currentCount < maxCount)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            if (spawnPos != Vector3.zero)
            {
                SpawnOrRetrieve(prefab, spawnPos);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Transform player = PlayerSeeker.GetPlayerTransform();

        // Делаем до 10 попыток найти валидную точку
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minMonsterGenDistance, maxMonsterGenDistance);
            Vector3 candidatePos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            bool inProhibited = false;
            if (prohibitedAreas != null)
            {
                foreach (var area in prohibitedAreas)
                {
                    if (area == null) continue;

                    // Создаем временную проверку позиции (без создания лишних объектов)
                    if (IsPositionInArea(area, candidatePos))
                    {
                        inProhibited = true;
                        break;
                    }
                }
            }

            if (!inProhibited) return candidatePos;
        }
        return Vector3.zero;
    }

    // Вспомогательный метод для проверки позиции в зоне без передачи Transform
    private bool IsPositionInArea(ProhibitedArea area, Vector3 position)
    {
        // Для работы InArea нужен Transform. Создаем один временный "пустышку" или используем существующий.
        // Чтобы не спамить Instantiate, просто проверим через логику ProhibitedArea напрямую, 
        // но так как InArea внутренний метод класса, воспользуемся временным объектом.
        GameObject temp = new GameObject();
        temp.transform.position = position;
        bool result = area.InArea(temp.transform);
        Destroy(temp);
        return result;
    }

    private void SpawnOrRetrieve(GameObject prefab, Vector3 position)
    {
        GameObject monster = monstersBuffer.Find(m => m != null && m.name.StartsWith(prefab.name));

        if (monster != null)
        {
            monstersBuffer.Remove(monster);
            monster.transform.position = position;
            monster.SetActive(true);
        }
        else
        {
            monster = Instantiate(prefab, position, Quaternion.identity);
            monster.name = prefab.name; // Убираем (Clone) из имени для поиска в буфере
        }
        activeMonsters.Add(monster);
    }

    private void DeactivateMonster(GameObject monster)
    {
        if (monster == null) return;
        monster.SetActive(false);
        activeMonsters.Remove(monster);
        monstersBuffer.Add(monster);
    }
}
