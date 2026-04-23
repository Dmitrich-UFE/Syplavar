using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Overlays;

public class TreeManager : MonoBehaviour
{
    private static TreeManager manager;
    internal Dictionary<int, TreeData> Trees {get; private set;}
    [SerializeField] private GameObject _TreePrefab;
    [SerializeField] private TreeDataGenerator _treeDataGen;
    [SerializeField] private Transform parent;
    private static int _IDForNewTree;

    void Awake()
    {
        manager = this;
        Trees = new Dictionary<int, TreeData>();

        if (_treeDataGen != null)
        {
            List<TreeSaveData> saveData = TreeSaveSystem.LoadTrees();
            
            if (saveData == null || saveData.Count <= 0)
            {
                saveData = _treeDataGen.GenerateTreeSaveData();
                TreeSaveSystem.SaveTrees(saveData);
            }

            GenerateTrees(saveData);
        }
    }

    internal void GenerateTrees(List<TreeSaveData> treeSaveDatas)
    {
        if (treeSaveDatas == null) return;
        foreach (TreeSaveData saveData in treeSaveDatas)
        {
            PlantTree tree = Instantiate(_TreePrefab, saveData.Position, Quaternion.identity, parent).GetComponent<PlantTree>();
            if (tree != null)
            {
                TreeData data = new TreeData(_IDForNewTree, saveData.GrowPhase, saveData.Type, tree);
                Trees[_IDForNewTree] = data;
                tree.InitTree(data);
                _IDForNewTree++;
            }
        }
    }

    public void SaveTrees()
    {
        List<TreeSaveData> savingTrees = new List<TreeSaveData>(Trees.Count);

        foreach (TreeData saveData in Trees.Values)
        {
            if (saveData.Tree != null)
                savingTrees.Add(saveData.Tree.GetSaveData());
        }
        TreeSaveSystem.SaveTrees(savingTrees);
    }

    public static void Update(TreeData data)
    {
        if (data != null)
        {
            manager.Trees[data.ID] = data;
        }
    }

    internal static int GetID()
    {
        return _IDForNewTree++;
    }
}
