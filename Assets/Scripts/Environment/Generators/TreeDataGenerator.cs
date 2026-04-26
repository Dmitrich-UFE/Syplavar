using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Numerics;

public class TreeDataGenerator : MonoBehaviour
{
    [HeaderAttribute("Координаты для генерации")]
    [SerializeField] private Vector2Int _LeftBottom;
    [SerializeField] private Vector2Int _LeftTop;
    [SerializeField] private Vector2Int _RightBottom;
    [SerializeField] private Vector2Int _RightTop;
    [SerializeField] private int _Height;

    [HeaderAttribute("Параметры генерации")]
    [SerializeField] private int _CountOfTrees;
    [SerializeField] private int _maxIndexOfTreeType;
    [SerializeField] private ShapeChecker[] _restrictedAreas;
    [SerializeField] private ShapeChecker[] _forestAreas;
    [SerializeField] private int _percentsForForestGen;

    internal HashSet<Vector3Int> existCoords {get; private set;}

    void Awake()
    {
        existCoords = new HashSet<Vector3Int>(_CountOfTrees);
    }

    internal List<TreeSaveData> GenerateTreeSaveData()
    {
        List<TreeSaveData> TreeSaveDatas = new List<TreeSaveData>(_CountOfTrees);
        

        int treesInForestTarget = Mathf.RoundToInt(_CountOfTrees * (_percentsForForestGen / 100f));
        int treesOutsideTarget = _CountOfTrees - treesInForestTarget;

        int forestCount = 0;
        int outsideCount = 0;

        int safetyBreak = 0;
        int maxIterations = _CountOfTrees * 100;

        while(TreeSaveDatas.Count < _CountOfTrees && safetyBreak < maxIterations)
        {
            safetyBreak++;

            int X = UnityEngine.Random.Range(_LeftTop.x, _RightBottom.x);
            int Z = UnityEngine.Random.Range(_LeftBottom.y, _RightTop.y);

            Vector3Int treeCandidatePosition = new Vector3Int(X, _Height, Z);

            int forestChance = UnityEngine.Random.Range(0, 101);

            if (!InRestrictedArea(treeCandidatePosition) && existCoords.Add(treeCandidatePosition))
            {
                bool isInForest = InForestArea(treeCandidatePosition);

                // 2. Проверяем, куда попадает дерево и не превышен ли лимит для этой зоны
                if (isInForest && forestCount < treesInForestTarget)
                {
                    AddTree(treeCandidatePosition);
                    forestCount++;
                }
            else if (!isInForest && outsideCount < treesOutsideTarget)
                {
                    AddTree(treeCandidatePosition);
                    outsideCount++;
                }
                else
                {
                    // Если попали в зону, лимит которой исчерпан, 
                    // удаляем из HashSet, чтобы освободить координату для других попыток
                    existCoords.Remove(treeCandidatePosition);
                }
            }
        }

        return TreeSaveDatas;

        // Локальная функция для создания объекта, чтобы не дублировать код
        void AddTree(Vector3Int pos)
        {
            TreeSaveData treeSaveData = new TreeSaveData();
            treeSaveData.Position = pos;
            treeSaveData.GrowPhase = 1;
            treeSaveData.Type = UnityEngine.Random.Range(0, _maxIndexOfTreeType+1);
            TreeSaveDatas.Add(treeSaveData);
        }
    }
    

    private bool InRestrictedArea(Vector3Int treeCandidatePosition)
    {
        foreach (ShapeChecker restrictedArea in _restrictedAreas)
        {
            if (restrictedArea.InArea(treeCandidatePosition))
            {
                return true;
            }
        }

        return false;
    }

    private bool InForestArea(Vector3Int treeCandidatePosition)
    {
        foreach (ShapeChecker forestArea in _forestAreas)
        {
            if (forestArea.InArea(treeCandidatePosition))
            {
                return true;
            }
        }

        return false;
    }
}

public static class TreeSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "trees.json");

    public static void SaveTrees(List<TreeSaveData> trees)
    {
        TreeDataWrapper wrapper = new TreeDataWrapper { Trees = trees };
        string json = JsonUtility.ToJson(wrapper, true); // true для красивого форматирования
        File.WriteAllText(SavePath, json);
        Debug.Log($"Сохранено в: {SavePath}");
    }

    public static List<TreeSaveData> LoadTrees()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Файл сохранения не найден");
            return new List<TreeSaveData>(0);
        }

        string json = File.ReadAllText(SavePath);
        TreeDataWrapper wrapper = JsonUtility.FromJson<TreeDataWrapper>(json);
        Debug.Log("Файл с деревьями загружен");
        return wrapper.Trees;
    }
}


[System.Serializable]
public class TreeSaveData
{
    public Vector3Int Position;
    public int Type;
    //public int ID;
    public int GrowPhase;
}

[System.Serializable]
public class TreeDataWrapper
{
    public List<TreeSaveData> Trees;
}
