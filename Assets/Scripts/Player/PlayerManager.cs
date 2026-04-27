using UnityEngine;
using System.IO;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject cursor;
    [SerializeField] private GameObject lowerInventory;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject deathWindow;
    [SerializeField] private UIHandler uiHandler;
    private CanvasGroup deathCanvasGroup;

    private Vector3 playerPosition;
    private PlayerHealth playerHealth;
    private PlayerMind playerMind;
    private WaitForSecondsRealtime tick;
    internal Transform SpawnPoint
    {
        get {return  spawnPoint;}
        set {spawnPoint = value;}
    }

    internal Vector3 SpawnPointPosition
    {
        get {return  spawnPoint.position;}
        set {spawnPoint.position = value;}
    }


    void Awake()
    {
        tick = new WaitForSecondsRealtime(0.01f);

        playerHealth = PlayerSeeker.GetPlayerHealth();
        playerMind = PlayerSeeker.GetPlayerMind();

        deathCanvasGroup = deathWindow.GetComponent<CanvasGroup>();

        playerHealth.OnHealthChanged += Death;

        LoadData();
    }

    public void SaveData()
    {
        PlayerParamsSaveData data = new PlayerParamsSaveData();
        data.Mind = playerMind.CurrentMind;
        data.Health = playerHealth.Health;
        data.IsDeath = playerHealth.isDeath;
        data.Position = player.transform.position;
        data.Spawnpoint = SpawnPoint.position;

        PlayerCharacteristicsSaveSystem.SavePlayerParams(data);
    }

    public void LoadData()
    {
        PlayerParamsSaveData data = PlayerCharacteristicsSaveSystem.LoadPlayerParams();

        if (data != null)
        {
            playerMind.CurrentMind = data.Mind;
            playerHealth.Health = data.Health;
            SpawnPoint.position = data.Spawnpoint;
            playerPosition = data.Position;

            if (data.IsDeath)
            {
                //player.transform.position = data.Spawnpoint;
                Respawn();
            }
            else
            {
                player.transform.position = data.Position;
            }
        }
    }

    public void Respawn()
    {
        if (playerHealth.isDeath)
        {
            Time.timeScale = 1f;

            uiHandler.SetDeath(false);
            player.transform.position = SpawnPoint.position;
            player.SetActive(true);

            playerMind.ResetMind();
            playerHealth.ResetHealth();
            
            cursor.SetActive(true);
            lowerInventory.SetActive(true);
            deathWindow.SetActive(false);
        }
    }

    void Death()
    {
        if (playerHealth.Health <= 0)
        {
            deathCanvasGroup.alpha = 0f;
            uiHandler.SetDeath(true);
            playerMind.StopMindDrain();
            player.SetActive(false);
            cursor.SetActive(false);
            lowerInventory.SetActive(false);
            deathWindow.SetActive(true);

            StartCoroutine(AnimateDeathUI());
            Time.timeScale = 0f;
        }
    }

    IEnumerator AnimateDeathUI()
    {
        while (deathCanvasGroup.alpha < 1f)
        {
            deathCanvasGroup.alpha+=0.04f;
            yield return tick;
        }
    }
}

public static class PlayerCharacteristicsSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "playerCharacteristics.json");

    public static void SavePlayerParams(PlayerParamsSaveData data)
    {
        string json = JsonUtility.ToJson(data, true); // true для красивого форматирования
        File.WriteAllText(SavePath, json);
        Debug.Log($"Сохранено в: {SavePath}");
    }

    public static PlayerParamsSaveData LoadPlayerParams()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Файл сохранения не найден");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        PlayerParamsSaveData data = JsonUtility.FromJson<PlayerParamsSaveData>(json);
        Debug.Log("Файл с характеристиками игрока загружен");
        return data;
    }
}


[System.Serializable]
public class PlayerParamsSaveData
{
    public int Mind;
    public int Health;
    public bool IsDeath;
    public Vector3 Position;
    public Vector3 Spawnpoint;
}
