using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private Task[] tasks;
    [SerializeField] private int index;
    [SerializeField] private int status;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private InventoryAI inventory;
    [SerializeField] private PlowedLandManager plowedLandManager;

    [Header("STORY UI")]
    [SerializeField] private TMP_Text taskNameText;
    [SerializeField] private TMP_Text taskGoalText;
    [SerializeField] private TMP_Text taskHintText;
    [SerializeField] private Image blackImage;

    [Header("OTHER")]
    [SerializeField] private GameObject MonsterGenerator;
    [SerializeField] private GameObject NastyFlowersGenerator;
    [SerializeField] private GameObject BigFlower;
    [SerializeField] private RecipeData[] AddingRecipes;
    [SerializeField] private CraftManager craftManager;

    [Header("HOUSE")]
    [SerializeField] private GameObject StandartHouse;
    [SerializeField] private GameObject BrokenHouse;


    //EventManager;
    internal string TaskHintText {get => taskHintText.text; set { taskHintText.text = value;}}
    internal int Status {get => status; set { status = value; StorySaveSystem.SaveStory(new StorySaveData{index = index, status = Status});}}
    internal string TaskLogoText => tasks[index].Name;
    internal string TaskDescriptionText => tasks[index].Description;
    internal string TaskGoalText => tasks[index].GoalDescription;


    private WaitForSecondsRealtime tick;
    private Coroutine coroutine;
    private bool isAddedRecipes;



    void Awake()
    {
        StorySaveData data = StorySaveSystem.LoadStory();
        tick = new WaitForSecondsRealtime(0.01f);

        if (data != null)
        {
            index = data.index;
            Status = data.status; 
            if (index >= 0 && index < tasks.Length)
            {
                TaskHintText = "";
                taskNameText.text = tasks[index].Name;
                taskGoalText.text = tasks[index].GoalDescription;
                tasks[index].Activate();
            }
            else
            {
                index = 0;
                TaskHintText = "";
                taskNameText.text = tasks[index].Name;
                taskGoalText.text = tasks[index].GoalDescription;
                tasks[index].Activate();
            }

            if (index > 20 && !isAddedRecipes)
            {
                foreach (RecipeData rData in AddingRecipes)
                {
                    craftManager.AddRecipe(rData);
                }
                isAddedRecipes = true;
            }

            if (index >= 23 && index <= 41)
            {
                BigFlower.SetActive(true);
            }
            else
            {
                if (BigFlower != null) BigFlower.SetActive(false);
            }

            if (index > 31)
            {
                MonsterGenerator.SetActive(true);
                NastyFlowersGenerator.SetActive(true);
            }

            if (index > 41 && index != tasks.Length - 1)
            {
                BrokenHouse.SetActive(true);
                StandartHouse.SetActive(false);
            }
            else
            {
                BrokenHouse.SetActive(false);
                StandartHouse.SetActive(true);
            }
        }
        else
        {
            index = 0;
            TaskHintText = "";
            Status = 1;
            taskNameText.text = tasks[index].Name;
            taskGoalText.text = tasks[index].GoalDescription;
            tasks[index].Activate();
        }
    }

    internal void CompleteTask(int ID)
    {
        if (tasks[index].ID == ID)
        {
            tasks[index].Complete();
            index++;
            Status = 1;
            StorySaveSystem.SaveStory(new StorySaveData{index = index, status = Status});

            playerManager.SpawnPointPosition = playerManager.PlayerPosition;
            playerManager.SaveData();
            inventory.SaveInventory();
            plowedLandManager.SavePlowedLands();
            DayLightHandlerManager.Save();
            
            if (index < tasks.Length)
            {
                TaskHintText = "";
                taskNameText.text = tasks[index].Name;
                taskGoalText.text = tasks[index].GoalDescription;
                tasks[index].Activate();
            }

            if (index > 20 && !isAddedRecipes)
            {
                foreach (RecipeData rData in AddingRecipes)
                {
                    craftManager.AddRecipe(rData);
                }
                isAddedRecipes = true;
            }

            if (index >= 23 && index <= 41)
            {
                BigFlower.SetActive(true);
            }
            else
            {
                if (BigFlower != null) BigFlower.SetActive(false);
            }

            if (index > 31)
            {
                MonsterGenerator.SetActive(true);
                NastyFlowersGenerator.SetActive(true);
            }

            if (index > 41 && index != tasks.Length - 1)
            {
                BrokenHouse.SetActive(true);
                StandartHouse.SetActive(false);
            }
            else
            {
                BrokenHouse.SetActive(false);
                StandartHouse.SetActive(true);
            }
        }
    }

    internal bool isStoryCompleted()
    {
        return index == tasks.Length-1;
    } 

    internal void OpenBlackPanel()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(AnimateBlackUI(true));
    }

    internal void OpenBlackPanelNoFade()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        blackImage.color = new Color(0f, 0f, 0f, 1f);
    }

    internal void CloseBlackPanel()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(AnimateBlackUI(false));
    }

    internal void CloseBlackPanelNoFade()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        blackImage.color = new Color(0f, 0f, 0f, 0f);
    }

    IEnumerator AnimateBlackUI(bool fadein)
    {
        if (fadein)
        {
            while (blackImage.color.a < 1f)
            {
                blackImage.color = new Color(0f, 0f, 0f, blackImage.color.a + 0.035f);
                yield return tick;
            }
        }
        else
        {
            while (blackImage.color.a > 0f)
            {
                blackImage.color = new Color(0f, 0f, 0f, blackImage.color.a - 0.035f);
                yield return tick;
            }
        }
        
    }
}

public static class StorySaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "Story.json");

    public static void SaveStory(StorySaveData data)
    {
        string json = JsonUtility.ToJson(data, true); // true для красивого форматирования
        File.WriteAllText(SavePath, json);
        Debug.Log($"Сохранено в: {SavePath}");
    }

    public static StorySaveData LoadStory()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Файл сохранения не найден");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        StorySaveData data = JsonUtility.FromJson<StorySaveData>(json);
        Debug.Log("Файл с сюжетом загружен");
        return data;
    }
}


[System.Serializable]
public class StorySaveData
{
    public int index;
    public int status;
}
