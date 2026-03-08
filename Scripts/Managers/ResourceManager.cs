using UnityEngine;
using System;

namespace AlkariEvolution.Managers
{
    /// <summary>
    /// Управление ресурсами игрока.
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        #region Singleton
        public static ResourceManager Instance { get; private set; }

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

        [Header("Текущие ресурсы")]
        [SerializeField] private int metal = 100;
        [SerializeField] private int element = 50;
        [SerializeField] private int organic = 30;
        [SerializeField] private int fuel = 200;
        [SerializeField] private int water = 150;

        [Header("Максимальные лимиты (0 = безлимит)")]
        [SerializeField] private int maxMetal = 0;
        [SerializeField] private int maxElement = 0;
        [SerializeField] private int maxOrganic = 0;
        [SerializeField] private int maxFuel = 0;
        [SerializeField] private int maxWater = 0;

        // Удалено: public event Action OnResourcesUpdated;

        public int Metal => metal;
        public int Element => element;
        public int Organic => organic;
        public int Fuel => fuel;
        public int Water => water;

        private void Start()
        {
            // При старте вызываем событие для обновления UI
            TriggerResourcesChanged();
        }

        #region Вспомогательные методы
        /// <summary>
        /// Вызвать событие изменения ресурсов через EventManager.
        /// </summary>
        private void TriggerResourcesChanged()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.TriggerEvent(EventManager.Events.OnResourcesChanged,
                    new object[] { metal, element, organic, fuel, water });
            }
        }
        #endregion

        #region Проверка наличия ресурсов
        public bool HasEnoughResources(int metalCost, int elementCost, int organicCost, int fuelCost, int waterCost)
        {
            if (metal < metalCost) return false;
            if (element < elementCost) return false;
            if (organic < organicCost) return false;
            if (fuel < fuelCost) return false;
            if (water < waterCost) return false;
            return true;
        }

        public bool HasEnoughMetal(int amount) => metal >= amount;
        public bool HasEnoughElement(int amount) => element >= amount;
        public bool HasEnoughOrganic(int amount) => organic >= amount;
        public bool HasEnoughFuel(int amount) => fuel >= amount;
        public bool HasEnoughWater(int amount) => water >= amount;
        #endregion

        #region Добавление ресурсов
        public void AddMetal(int amount)
        {
            if (amount <= 0) return;
            
            int newValue = metal + amount;
            if (maxMetal > 0 && newValue > maxMetal)
                newValue = maxMetal;
            
            metal = newValue;
            TriggerResourcesChanged();
        }

        public void AddElement(int amount)
        {
            if (amount <= 0) return;
            
            int newValue = element + amount;
            if (maxElement > 0 && newValue > maxElement)
                newValue = maxElement;
            
            element = newValue;
            TriggerResourcesChanged();
        }

        public void AddOrganic(int amount)
        {
            if (amount <= 0) return;
            
            int newValue = organic + amount;
            if (maxOrganic > 0 && newValue > maxOrganic)
                newValue = maxOrganic;
            
            organic = newValue;
            TriggerResourcesChanged();
        }

        public void AddFuel(int amount)
        {
            if (amount <= 0) return;
            
            int newValue = fuel + amount;
            if (maxFuel > 0 && newValue > maxFuel)
                newValue = maxFuel;
            
            fuel = newValue;
            TriggerResourcesChanged();
        }

        public void AddWater(int amount)
        {
            if (amount <= 0) return;
            
            int newValue = water + amount;
            if (maxWater > 0 && newValue > maxWater)
                newValue = maxWater;
            
            water = newValue;
            TriggerResourcesChanged();
        }
        #endregion

        #region Списание ресурсов
        public bool SpendMetal(int amount)
        {
            if (!HasEnoughMetal(amount)) return false;
            
            metal -= amount;
            TriggerResourcesChanged();
            return true;
        }

        public bool SpendElement(int amount)
        {
            if (!HasEnoughElement(amount)) return false;
            
            element -= amount;
            TriggerResourcesChanged();
            return true;
        }

        public bool SpendOrganic(int amount)
        {
            if (!HasEnoughOrganic(amount)) return false;
            
            organic -= amount;
            TriggerResourcesChanged();
            return true;
        }

        public bool SpendFuel(int amount)
        {
            if (!HasEnoughFuel(amount)) return false;
            
            fuel -= amount;
            TriggerResourcesChanged();
            return true;
        }

        public bool SpendWater(int amount)
        {
            if (!HasEnoughWater(amount)) return false;
            
            water -= amount;
            TriggerResourcesChanged();
            return true;
        }

        public bool SpendResources(int metalCost, int elementCost, int organicCost, int fuelCost, int waterCost)
        {
            if (!HasEnoughResources(metalCost, elementCost, organicCost, fuelCost, waterCost))
                return false;

            metal -= metalCost;
            element -= elementCost;
            organic -= organicCost;
            fuel -= fuelCost;
            water -= waterCost;
            
            TriggerResourcesChanged();
            return true;
        }
        #endregion

        #region Установка значений (для загрузки сохранения)
        public void SetResources(int newMetal, int newElement, int newOrganic, int newFuel, int newWater)
        {
            metal = newMetal;
            element = newElement;
            organic = newOrganic;
            fuel = newFuel;
            water = newWater;
            
            TriggerResourcesChanged();
        }
        #endregion
    }
}