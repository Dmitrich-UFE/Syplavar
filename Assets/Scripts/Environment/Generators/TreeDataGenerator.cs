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
    

    private HashSet<Vector3Int> existCoords;

    void Awake()
    {
        existCoords = new HashSet<Vector3Int>(_CountOfTrees);
    }

    internal List<TreeSaveData> GenerateTreeSaveData()
    {
        List<TreeSaveData> TreeSaveDatas = new List<TreeSaveData>(_CountOfTrees);

        while(TreeSaveDatas.Count < _CountOfTrees)
        {
            int X = UnityEngine.Random.Range(_LeftTop.x, _RightBottom.x);
            int Z = UnityEngine.Random.Range(_LeftBottom.y, _RightTop.y);

            Vector3Int treeCandidatePosition = new Vector3Int(X, _Height, Z);
            
            if (!existCoords.Add(treeCandidatePosition) && !InRestrictedArea(treeCandidatePosition))
            {
                TreeSaveData treeSaveData = new TreeSaveData();
                treeSaveData.Position = treeCandidatePosition;
                treeSaveData.GrowPhase = 1;
                treeSaveData.Type = UnityEngine.Random.Range(0, _maxIndexOfTreeType);

                TreeSaveDatas.Add(treeSaveData);
            }
        }

        return TreeSaveDatas;
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
