using UnityEngine;
using System.Collections.Generic;
using System;

public class BushManager : MonoBehaviour
{
    private static BushManager manager;
    internal Dictionary<int, BushData> Bushes {get; private set;}
    [SerializeField] private GameObject[] _BushPrefabs;
    [SerializeField] private BushGenerator _bushDataGen;
    [SerializeField] private Transform parent;
    private static int _IDForNewBush;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        manager = this;
        Bushes = new Dictionary<int, BushData>();

        if (_bushDataGen != null)
        {
            List<BushSaveData> saveData = BushSaveSystem.LoadBushes();
            
            if (saveData == null || saveData.Count <= 0)
            {
                saveData = _bushDataGen.GenerateBushSaveData();
                BushSaveSystem.SaveBushes(saveData);
            }

            GenerateBushes(saveData);
        }
    }

    internal void GenerateBushes(List<BushSaveData> bushSaveDatas)
    {
        if (bushSaveDatas == null) return;
        foreach (BushSaveData saveData in bushSaveDatas)
        {
            Bush bush = Instantiate(_BushPrefabs[saveData.Type], saveData.Position, Quaternion.identity, parent).GetComponent<Bush>();
            if (bush != null)
            {
                BushData data = new BushData{ID = GetID(), Bush = bush, Type = saveData.Type};
                Bushes[data.ID] = data;
                bush.Init(data);
            }
        }
    }

    internal HashSet<Vector3Int> GetBushCoords()
    {
        HashSet<Vector3Int> coords = new HashSet<Vector3Int>();
        foreach (BushData data in  Bushes.Values)
        {
            if (data != null && data.Bush != null)
            {
                Vector3 coordFl = data.Bush.transform.position;
                Vector3Int coord = new Vector3Int((int)Math.Round(coordFl.x), 0, (int)Math.Round(coordFl.z));
                coords.Add(coord);
            }
        }
        return coords;
    }

    public void SaveBushes()
    {
        List<BushSaveData> savingBushes = new List<BushSaveData>(Bushes.Count);

        foreach (BushData saveData in Bushes.Values)
        {
            if (saveData.Bush != null)
                savingBushes.Add(saveData.Bush.GetSaveData());
        }
        BushSaveSystem.SaveBushes(savingBushes);
    }

    public static void Update(BushData data)
    {
        if (data != null)
        {
            manager.Bushes[data.ID] = data;
        }
    }

    internal static int GetID()
    {
        return _IDForNewBush++;
    }
}

public class BushData
{
    public Bush Bush;
    public int ID;
    public int Type;
}
