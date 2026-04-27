using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class BushGenerator : MonoBehaviour
{
    [HeaderAttribute("Координаты для генерации")]
    [SerializeField] private Vector2Int _LeftBottom;
    [SerializeField] private Vector2Int _LeftTop;
    [SerializeField] private Vector2Int _RightBottom;
    [SerializeField] private Vector2Int _RightTop;
    [SerializeField] private int _Height;

    [HeaderAttribute("Параметры генерации")]
    [SerializeField] private int bushTypes;
    [SerializeField] private int _CountOfBushes;
    [SerializeField] private ShapeChecker[] _restrictedAreas;

    [HeaderAttribute("Прочее")]
    [SerializeField] private TreeManager treeMan;


    internal HashSet<Vector3Int> existCoords {get; private set;}

    void Awake()
    {
        existCoords = new HashSet<Vector3Int>(_CountOfBushes);
        existCoords.UnionWith(treeMan.GetTreeCoords());
    }

    internal List<BushSaveData> GenerateBushSaveData()
    {
        List<BushSaveData> BushSaveDatas = new List<BushSaveData>(_CountOfBushes);

        int safetyBreak = 0;
        int maxIterations = _CountOfBushes * 100;

        while(BushSaveDatas.Count < _CountOfBushes && safetyBreak < maxIterations)
        {
            safetyBreak++;

            int X = UnityEngine.Random.Range(_LeftTop.x, _RightBottom.x);
            int Z = UnityEngine.Random.Range(_LeftBottom.y, _RightTop.y);

            Vector3Int candidatePosition = new Vector3Int(X, _Height, Z);
            if (!InRestrictedArea(candidatePosition) && existCoords.Add(candidatePosition))
            {
                AddBush(candidatePosition);
            }
        }

        return BushSaveDatas;

        void AddBush(Vector3Int pos)
        {
            BushSaveData bushSaveData = new BushSaveData();
            bushSaveData.Position = pos;
            bushSaveData.Type = UnityEngine.Random.Range(0, bushTypes);
            BushSaveDatas.Add(bushSaveData);
        }
    }

    private bool InRestrictedArea(Vector3Int candidatePosition)
    {
        foreach (ShapeChecker restrictedArea in _restrictedAreas)
        {
            if (restrictedArea.InArea(candidatePosition))
            {
                return true;
            }
        }

        return false;
    }
}


public static class BushSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "bushes.json");

    public static void SaveBushes(List<BushSaveData> bushes)
    {
        BushDataWrapper wrapper = new BushDataWrapper { Bushes = bushes };
        string json = JsonUtility.ToJson(wrapper, true); // true для красивого форматирования
        File.WriteAllText(SavePath, json);
        Debug.Log($"Сохранено в: {SavePath}");
    }

    public static List<BushSaveData> LoadBushes()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Файл сохранения не найден");
            return new List<BushSaveData>(0);
        }

        string json = File.ReadAllText(SavePath);
        BushDataWrapper wrapper = JsonUtility.FromJson<BushDataWrapper>(json);
        Debug.Log("Файл с деревьями загружен");
        return wrapper.Bushes;
    }
}


[System.Serializable]
public class BushSaveData
{
    public Vector3Int Position;
    public int Type;
}

[System.Serializable]
public class BushDataWrapper
{
    public List<BushSaveData> Bushes;
}

