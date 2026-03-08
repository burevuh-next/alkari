using System.Collections.Generic;
using AlkariEvolution.Managers;
using UnityEngine;

namespace AlkariEvolution.GlobalMap
{
    /// <summary>
    /// Управление глобальной картой
    /// </summary>
    public class GlobalMapManager : MonoBehaviour
    {
        public static GlobalMapManager Instance { get; private set; }

        [Header("Настройки карты")]
        [SerializeField] private int mapWidth = 20;
        [SerializeField] private int mapHeight = 20;
        [SerializeField] private int numberOfLocations = 8;  // ИСПРАВЛЕНО: было numberOfLocation
        [SerializeField] private float minDistanceBetweenLocations = 2f;

        [Header("Префабы")]
        [SerializeField] private GameObject shipPrefab;       // Корабль игрока
        [SerializeField] private GameObject locationPrefab;   // Визуальное представление локации

        [Header("Точки на карте")]
        [SerializeField] private List<LocationSO> availableLocations;  // Доступные типы локаций

        private List<GameObject> spawnedLocations = new List<GameObject>();
        private GameObject playerShip;
        private Vector2 currentShipPosition;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;  // ИСПРАВЛЕНО: было Instance == this
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);  // ИСПРАВЛЕНО: было Destroy(GameObject)
                return;
            }
        }

        private void Start()
        {
            GenerateMap();
        }

        /// <summary>
        /// Генерация случайной карты
        /// </summary>
        public void GenerateMap()
        {
            // Очищаем предыдущую карту
            ClearMap();  // ИСПРАВЛЕНО: было CleanMap()

            // Генерация позиций для локаций
            List<Vector2> positions = GeneratePositions();  // ИСПРАВЛЕНО: было GeneratePosition()

            // Создаем локации
            for (int i = 0; i < positions.Count; i++)
            {
                CreateLocation(positions[i], i);
            }

            // Создаем корабль в нужных координатах
            CreateShip(GetStartPosition(positions));  // ИСПРАВЛЕНО: было GetStartePosition

            Debug.Log($"Карта сгенерирована: {spawnedLocations.Count} локаций");
        }

        /// <summary>
        /// Генерация случайных позиций для локаций
        /// </summary>
        private List<Vector2> GeneratePositions()
        {
            List<Vector2> positions = new List<Vector2>();

            for (int i = 0; i < numberOfLocations; i++)  // ИСПРАВЛЕНО: было numberOfLocation
            {
                int attempts = 0;
                Vector2 newPos;

                do
                {
                    newPos = new Vector2(
                        Random.Range(-mapWidth / 2f, mapWidth / 2f),
                        Random.Range(-mapHeight / 2f, mapHeight / 2f)
                    );
                    attempts++;
                }
                while (!IsPositionValid(newPos, positions) && attempts < 100);  // ИСПРАВЛЕНО: было IsPositionalValid

                positions.Add(newPos);
            }

            return positions;  // ИСПРАВЛЕНО: был пропущен return
        }

        /// <summary>
        /// Проверка, не слишком ли близко к другим локациям
        /// </summary>
        private bool IsPositionValid(Vector2 pos, List<Vector2> existingPositions)
        {
            foreach (Vector2 existing in existingPositions)
            {
                if (Vector2.Distance(pos, existing) < minDistanceBetweenLocations)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Создание визуального представления локации
        /// </summary>
        private void CreateLocation(Vector2 position, int index)
        {
            if (locationPrefab == null) return;

            GameObject locationObj = Instantiate(locationPrefab, position, Quaternion.identity);
            locationObj.name = $"Location_{index}";
            locationObj.transform.SetParent(transform);

            // Выбираем случайный тип локации
            if (availableLocations != null && availableLocations.Count > 0)  // Добавлена проверка на null
            {
                LocationSO locationData = availableLocations[Random.Range(0, availableLocations.Count)];

                // Настраиваем визуал
                SpriteRenderer renderer = locationObj.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    if (locationData.iconSprite != null)
                        renderer.sprite = locationData.iconSprite;
                    renderer.color = locationData.iconColor;
                }

                // Сохраняем данные в компоненте
                LocationView view = locationObj.GetComponent<LocationView>();
                if (view == null)
                    view = locationObj.AddComponent<LocationView>();

                view.Initialize(locationData);
            }

            spawnedLocations.Add(locationObj);
        }

        /// <summary>
        /// Получить стартовую позицию (ближе к центру)
        /// </summary>
        private Vector2 GetStartPosition(List<Vector2> positions)
        {
            if (positions.Count == 0)
                return Vector2.zero;

            // Ищем позицию ближе к центру
            Vector2 startPos = positions[0];
            float minDist = startPos.magnitude;

            foreach (Vector2 pos in positions)
            {
                if (pos.magnitude < minDist)
                {
                    minDist = pos.magnitude;
                    startPos = pos;
                }
            }

            return startPos;
        }

        /// <summary>
        /// Создание корабля игрока
        /// </summary>
        private void CreateShip(Vector2 position)
        {
            if (shipPrefab == null) return;

            playerShip = Instantiate(shipPrefab, position, Quaternion.identity);
            playerShip.name = "PlayerShip";
            playerShip.transform.SetParent(transform);

            currentShipPosition = position;

            // Добавляем компонент управления кораблем
            ShipController controller = playerShip.GetComponent<ShipController>();
            if (controller == null)
                controller = playerShip.AddComponent<ShipController>();
        }

        /// <summary>
        /// Переместить корабль в новую локацию
        /// </summary>
        public bool MoveShipToLocation(GameObject targetLocation)
        {
            if (playerShip == null || targetLocation == null) return false;

            // Проверка расстояния и топлива
            float distance = GetDistanceBetweenLocations(playerShip, targetLocation);
            int fuelCost = CalculateFuelCost(distance);

            //Проверяем хватает ли топлива
            if (ResourceManager.Instance != null && !ResourceManager.Instance.HasEnoughFuel(fuelCost))
            {
              Debug.LogWarning($"Недостаточно топлива! Нужно: {fuelCost}");
              if (UIManager.Instance != null)
              {
                UIManager.Instance.ShowMessage($"Недостаточно топлива! Нужно: {fuelCost}");
              }
              return false;
            }

            Vector2 targetPos = targetLocation.transform.position;

            // Используем ShipController для плавного движения, если он есть
            ShipController controller = playerShip.GetComponent<ShipController>();
            if (controller != null)
            {
              //Подписываемся на окончание движения
              controller.OnMoveCompleted = null; //сбрасываем предыдущую подписку
              controller.OnMoveCompleted += () =>
              {
                currentShipPosition = targetPos;
                Debug.Log($"Корабль прибыл в {targetLocation.name}");
                
                //Списываем топливо
                if (ResourceManager.Instance != null)
                {
                  ResourceManager.Instance.SpendFuel(fuelCost);
                  Debug.Log($"Потрачено топлива: {fuelCost}");
                }
                //Отмечаем локацию как исследоанную
                LocationView view = targetLocation.GetComponent<LocationView>();
                if (view != null)
                {
                    view.MarkAsExplored();
                }

                // Вызывает событие через EventManager
                if (EventManager.Instance != null)
                {
                    EventManager.Instance.TriggerEvent(EventManager.Events.OnLocationReached, new object[] { targetLocation });
                }
              };

              //Начинаем движение
              controller.MoveTo(targetPos);
              Debug.Log($"Корабль начинает движение в {targetLocation.name}");
            }
            else
            {
                // Если контроллера нет, телепортируем
                playerShip.transform.position = targetPos;
                currentShipPosition = targetPos;
                
                //Списываем топливо
                if (ResourceManager.Instance != null)
                {
                  ResourceManager.Instance.SpendFuel(fuelCost);
                  Debug.Log($"Потрачено топлива: {fuelCost}");
                }

                // Отмечаем локацию как исследованную
                LocationView view = targetLocation.GetComponent<LocationView>();
                if (view != null)
                {
                    view.MarkAsExplored();
                }

                // Вызываем событие через EventManager
                if (EventManager.Instance != null)
                {
                    EventManager.Instance.TriggerEvent(EventManager.Events.OnLocationReached, 
                        new object[] { targetLocation });
                }
            }
            return true;
        }

        /// <summary>
        /// Получить текущую позицию корабля
        /// </summary>
        public Vector2 GetShipPosition() => currentShipPosition;  // ИСПРАВЛЕНО: был пропущен этот метод

        /// <summary>
        /// Очистить карту
        /// </summary>
        private void ClearMap()  // ИСПРАВЛЕНО: имя метода (было ClearMap, а в вызове было CleanMap)
        {
            foreach (GameObject loc in spawnedLocations)
            {
                if (loc != null)
                    Destroy(loc);
            }
            spawnedLocations.Clear();

            if (playerShip != null)
                Destroy(playerShip);
        }
        /// <summary>
        /// Рассчитать расстояние между двумя точками
        /// </summary>
        private float GetDistanceBetweenLocations(GameObject from, GameObject to)
        {
            if (from == null || to == null) return 0f;
            
            return Vector2.Distance(from.transform.position, to.transform.position);
        }

        /// <summary>
        /// Рассчитать стоимость перемещения
        /// </summary>
        private int CalculateFuelCost(float distance)
        {
            // Базовая стоимость: 1 топлива на единицу расстояния
            return Mathf.CeilToInt(distance);
        }
    }
}