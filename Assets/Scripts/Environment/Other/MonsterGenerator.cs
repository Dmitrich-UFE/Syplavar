using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterGenerator : MonoBehaviour
{
    public static MonsterGenerator instance;

    [Header("��������� ��������")]
    [SerializeField] private GameObject monsterType1;
    [SerializeField] private GameObject monsterType2;
    [SerializeField] private int maxCountType1 = 5;
    [SerializeField] private int maxCountType2 = 5;

    [Header("���� �������")]
    [SerializeField] private ProhibitedArea[] prohibitedAreas;

    [Header("���������")]
    [SerializeField] private float minMonsterGenDistance = 10f;
    [SerializeField] private float maxMonsterGenDistance = 30f;
    [SerializeField] private float maxMonsterExistDistance = 50f;

    private List<GameObject> activeMonsters = new List<GameObject>();
    private List<GameObject> monstersBuffer = new List<GameObject>();

    private bool isGenerationTime = false;

    #region ��������
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
        // �������� �� ������� �� ������ DayLightHandler
        DayLightHandler._OnTimeReached += CheckTime;

        // �������������� �������� �������� �������
        CheckTime((DayLightHandler.Hours, DayLightHandler.Minutes));

        StartCoroutine(MonsterLifecycleRoutine());
    }

    void OnDestroy()
    {
        // ������������ �������, ����� �� ���� ������ ��� ����� �����
        DayLightHandler._OnTimeReached -= CheckTime;
    }

    private void CheckTime((int hh, int mm) time)
    {
        // ������� ������������ � 18:00 �� 06:00
        isGenerationTime = (time.hh >= 18 || time.hh < 6);
        //isGenerationTime = true;
    }

    IEnumerator MonsterLifecycleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            Transform player = PlayerSeeker.GetPlayerTransform();
            if (player == null) continue;

            // 1. �������� �������� ��������: ��������� ��� ������������
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

            // 2. ������� ���������, ���� ������ ���������� ����� �����
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

        // ������� ������� �������� ����� ���� ������ �������
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

        // ������ �� 10 ������� ����� �������� �����
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

                    // ������� ��������� �������� ������� (��� �������� ������ ��������)
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

    // ��������������� ����� ��� �������� ������� � ���� ��� �������� Transform
    private bool IsPositionInArea(ProhibitedArea area, Vector3 position)
    {
        // ��� ������ InArea ����� Transform. ������� ���� ��������� "��������" ��� ���������� ������������.
        // ����� �� ������� Instantiate, ������ �������� ����� ������ ProhibitedArea ��������, 
        // �� ��� ��� InArea ���������� ����� ������, ������������� ��������� ��������.
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
            monster.name = prefab.name; // ������� (Clone) �� ����� ��� ������ � ������
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
