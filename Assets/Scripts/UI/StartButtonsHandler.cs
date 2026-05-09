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

    // public void DeleteData()
    // {
    //     string path = Application.persistentDataPath;

    //     if (Directory.Exists(path))
    //     {
    //         // 1. Удаляем все файлы
    //         string[] files = Directory.GetFiles(path);
    //         foreach (string file in files)
    //         {
    //             File.Delete(file);
    //         }

    //         // 2. Удаляем все подпапки (true — рекурсивно)
    //         string[] folders = Directory.GetDirectories(path);
    //         foreach (string folder in folders)
    //         {
    //         Directory.Delete(folder, true);
    //         }
    //     }
    // }

    public void DeleteData()
{
    string path = Application.persistentDataPath;
    if (!Directory.Exists(path)) return;

    DirectoryInfo di = new DirectoryInfo(path);

    // Удаляем файлы
    foreach (FileInfo file in di.GetFiles())
    {
        try {
            file.Delete();
        } catch (System.IO.IOException e) {
            Debug.LogWarning($"Файл занят и не может быть удален: {file.Name}. Ошибка: {e.Message}");
        }
    }

    // Удаляем папки
    foreach (DirectoryInfo dir in di.GetDirectories())
    {
        try {
            dir.Delete(true);
        } catch (System.IO.IOException e) {
            Debug.LogWarning($"Папка занята: {dir.Name}");
        }
    }
}

}
