using UnityEngine;
using System.Collections.Generic;

public class TreeManager : MonoBehaviour
{
    internal Dictionary<int, TreeData> Trees {get; private set;}
    [SerializeField] private GameObject _TreePrefab;
    private static int _IDForNewTree;

    void Awake()
    {
        Trees = new Dictionary<int, TreeData>();
        GenerateTrees();
    }

    internal void GenerateTrees()
    {
        PlantTree tree = Instantiate(_TreePrefab, this.transform).GetComponent<PlantTree>();

        if (tree != null)
        {
            TreeData data = new TreeData(_IDForNewTree, 1, 6, tree);
            Trees[_IDForNewTree] = data;
            tree.InitTree(data);
            _IDForNewTree++;
        }
    }
}
