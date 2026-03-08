using UnityEngine;
using System.IO;
using System;
using System.Runtime.CompilerServices;

namespace AlkariEvolution.Managers
{
  /// <summary>
  /// Управление сохранением и загрузкой
  /// </summary>
  
  public class SaveManager : MonoBehaviour
  {
    #region Singleton
    public static SaveManager Instance {get; private set;}

    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else
      {
        Destroy(gameObject);
        return;
      }
    }
    #endregion

    [Header("Насторйки сохранения")]
    [SerializeField] private string saveFileName = "save.json";
    [SerializeField] private bool useEncrypton = false;
    [SerializeField] private int maxSaveSlots = 3;

    private string savePath;

    private void Start()
    {
      //Определяем путь к файлу сохранения
      savePath = Path.Combine(Application.persistentDataPath, saveFileName);
      Debug.Log($"Save path: {savePath}");
    }

    #region Сохранение
    /// <summary>
    /// Сохранить текущее состояние игры
    /// </summary>
    public void SaveGame()
    {
      try
      {
        //Создаем обьект с данными
        GameData data = new GameData();
        //Заполняем данные из ResourceManager
        if (ResourceManager.Instance != null)
        {
          data.metal = ResourceManager.Instance.Metal;
          data.element = ResourceManager.Instance.Element;
          data.organic = ResourceManager.Instance.Organic;
          data.fuel = ResourceManager.Instance.Fuel;
          data.water = ResourceManager.Instance.Water;
        }

        // TODO: добавить сохранение позиции корабля
        // TODO: добавить сохранение выживших
        // TODO: добавить сохранение исследований

        //Сериализуем Json
        string json = JsonUtility.ToJson(data, true);

        //Шифрование
        if (useEncrypton)
        {
          json = EncryptDecrypt(json);
        }

        //Записываем в файл
        File.WriteAllText(savePath, json);

        Debug.Log($"Game saved to {savePath}");

        if (UIManager.Instance != null)
        {
          UIManager.Instance.ShowMessage("Игра сохранена");
        }
      }
      catch (Exception e)
      {
        Debug.LogError($"Error saving game: {e.Message}");
      }
    }

    /// <summary>
    /// Сохранить в указанный слот.
    /// </summary>
    public void SaveGameToSlot(int slotIndex)
    {
      if (slotIndex <0 || slotIndex >= maxSaveSlots) return;

      string slotPath = Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.json");

      try
      {
        GameData data = new GameData();
        if (ResourceManager.Instance != null)
        {
          data.metal = ResourceManager.Instance.Metal;
          data.element = ResourceManager.Instance.Element;
          data.organic = ResourceManager.Instance.Organic;
          data.fuel = ResourceManager.Instance.Fuel;
          data.water = ResourceManager.Instance.Water;
        }

        // TODO: добавить сохранение позиции корабля
        // TODO: добавить сохранение выживших
        // TODO: добавить сохранение исследований

        //Сериализуем Json
        string json = JsonUtility.ToJson(data, true);

        //Шифрование
        if (useEncrypton)
        {
          json = EncryptDecrypt(json);
        }

        //Записываем в файл
        File.WriteAllText(savePath, json);

        Debug.Log($"Game saved to slot{savePath}");

        if (UIManager.Instance != null)
        {
          UIManager.Instance.ShowMessage("Игра сохранена");
        }
      }
      catch (Exception e)
      {
        Debug.LogError($"Error saving game to slot: {e.Message}");
      }
    }
    #endregion

    #region Загрузка
    /// <summary>
    /// Загрузить игру из указанного слота
    /// </summary>
    public void LoadGameFromSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSaveSlots) return;

            string slotPath = Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.json");
            
            try
            {
                if (!File.Exists(slotPath))
                {
                    Debug.LogWarning($"Save file not found at {slotPath}");
                    return;
                }

                string json = File.ReadAllText(slotPath);
                GameData data = JsonUtility.FromJson<GameData>(json);

                if (ResourceManager.Instance != null)
                {
                    ResourceManager.Instance.SetResources(
                        data.metal,
                        data.element,
                        data.organic,
                        data.fuel,
                        data.water
                    );
                }

                Debug.Log($"Game loaded from slot {slotIndex}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading from slot {slotIndex}: {e.Message}");
            }
        }
        #endregion

        #region Проверка и удаление
        /// <summary>
        /// Проверить, существует ли сохранение.
        /// </summary>
        public bool HasSave()
        {
            return File.Exists(savePath);
        }

        /// <summary>
        /// Проверить, существует ли сохранение в указанном слоте.
        /// </summary>
        public bool HasSaveInSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSaveSlots) return false;
            string slotPath = Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.json");
            return File.Exists(slotPath);
        }

        /// <summary>
        /// Удалить файл сохранения.
        /// </summary>
        public void DeleteSave()
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("Save file deleted");
            }
        }

        /// <summary>
        /// Получить информацию о сохранении в слоте.
        /// </summary>
        public GameData GetSaveInfo(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSaveSlots) return null;
            
            string slotPath = Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.json");
            
            if (!File.Exists(slotPath)) return null;
            
            try
            {
                string json = File.ReadAllText(slotPath);
                return JsonUtility.FromJson<GameData>(json);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Вспомогательные методы
        /// <summary>
        /// Простое XOR-шифрование (для базовой защиты от редактирования).
        /// </summary>
        private string EncryptDecrypt(string data)
        {
            string key = "alkari2026"; // простой ключ
            string result = "";
            
            for (int i = 0; i < data.Length; i++)
            {
                result += (char)(data[i] ^ key[i % key.Length]);
            }
            
            return result;
        }

        /// <summary>
        /// Получить путь к папке сохранений.
        /// </summary>
        public string GetSavePath()
        {
            return savePath;
        }
        #endregion
    }
}