using UnityEngine;

public class CentralSaveSystem : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private InventoryAI inventory;
    [SerializeField] private PlowedLandManager plowedLandManager;
    [SerializeField] private BushManager bushManager;
    [SerializeField] private TreeManager treeManager;
    

    public void SaveData()
    {
        playerManager.SaveData();
        inventory.SaveInventory();
        plowedLandManager.SavePlowedLands();
        DayLightHandlerManager.Save();
        bushManager.SaveBushes();
        treeManager.SaveTrees();
        Debug.Log("Сохранены данные");
    }
}
