using UnityEngine;
using System.IO;

public class StartButtonsHandler : MonoBehaviour
{
    
    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject ContResetButt;

    void Awake()
    {
        // Формируем полный путь к файлу
        string filePath = Path.Combine(Application.persistentDataPath, "bushes.json");

        // Проверяем существование файла
        if (File.Exists(filePath))
        {
            StartButton.SetActive(false);
            ContResetButt.SetActive(true);
        }
        else
        {
            StartButton.SetActive(true);
            ContResetButt.SetActive(false);
        }
    }

    public void DeleteData()
    {
        string path = Application.persistentDataPath;

        if (Directory.Exists(path))
        {
            // 1. Удаляем все файлы
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                File.Delete(file);
            }

            // 2. Удаляем все подпапки (true — рекурсивно)
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
            Directory.Delete(folder, true);
            }
        }
    }
}
