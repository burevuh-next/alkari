using UnityEngine;
using AlkariEvolution.Managers;

namespace AlkariEvolution.Tests
{
    /// <summary>
    /// Тестовый скрипт для проверки работы EventManager.
    /// </summary>
    public class EventTester : MonoBehaviour
    {
        [Header("Настройки теста")]
        [SerializeField] private KeyCode testKey = KeyCode.T;
        [SerializeField] private bool logAllEvents = true;

        private void OnEnable()
        {
            // Подписываемся на события при включении объекта
            if (EventManager.Instance != null)
            {
                EventManager.Instance.StartListening(EventManager.Events.OnResourcesChanged, OnResourcesChanged);
                EventManager.Instance.StartListening(EventManager.Events.OnCombatStarted, OnCombatStarted);
                EventManager.Instance.StartListening(EventManager.Events.OnUnitDied, OnUnitDied);
                EventManager.Instance.StartListening(EventManager.Events.OnUnitInfected, OnUnitInfected);
                EventManager.Instance.StartListening(EventManager.Events.OnMissionCompleted, OnMissionCompleted);

                Debug.Log("EventTester: Подписался на события");
            }
            else
            {
                Debug.LogWarning("EventTester: EventManager не найден");
            }
        }

        private void OnDisable()
        {
            // Отписываемся от событий при отключении
            if (EventManager.Instance != null)
            {
                EventManager.Instance.StopListening(EventManager.Events.OnResourcesChanged, OnResourcesChanged);
                EventManager.Instance.StopListening(EventManager.Events.OnCombatStarted, OnCombatStarted);
                EventManager.Instance.StopListening(EventManager.Events.OnUnitDied, OnUnitDied);
                EventManager.Instance.StopListening(EventManager.Events.OnUnitInfected, OnUnitInfected);
                EventManager.Instance.StopListening(EventManager.Events.OnMissionCompleted, OnMissionCompleted);

                Debug.Log("EventTester: Отписался от событий");
            }
        }

        private void Update()
        {
            // Тестовая клавиша для вызова события
            if (Input.GetKeyDown(testKey))
            {
                TestAllEvents();
            }

            // Дополнительные тесты
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TestResourceEvent();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TestCombatEvent();
            }
        }

        #region Обработчики событий
        private void OnResourcesChanged(object[] parameters)
        {
            string message = "Событие OnResourcesChanged получено!";
            if (parameters != null && parameters.Length > 0)
            {
                message += $" Параметры:";
                for (int i = 0; i < parameters.Length; i++)
                {
                    message += $" [{i}:{parameters[i]}]";
                }
            }
            Debug.Log(message);
        }

        private void OnCombatStarted(object[] parameters)
        {
            Debug.Log("Событие OnCombatStarted получено! Начинается бой!");
        }

        private void OnUnitDied(object[] parameters)
        {
            string unitName = parameters != null && parameters.Length > 0 ? parameters[0].ToString() : "Неизвестный юнит";
            Debug.Log($"Событие OnUnitDied получено! Юнит {unitName} погиб.");
        }

        private void OnUnitInfected(object[] parameters)
        {
            string unitName = parameters != null && parameters.Length > 0 ? parameters[0].ToString() : "Неизвестный юнит";
            int stage = parameters != null && parameters.Length > 1 ? (int)parameters[1] : 1;
            Debug.Log($"Событие OnUnitInfected получено! Юнит {unitName} заражён (стадия {stage}).");
        }

        private void OnMissionCompleted(object[] parameters)
        {
            string missionName = parameters != null && parameters.Length > 0 ? parameters[0].ToString() : "Неизвестная миссия";
            Debug.Log($"Событие OnMissionCompleted получено! Миссия '{missionName}' выполнена!");
        }
        #endregion

        #region Тестовые методы
        /// <summary>
        /// Протестировать все события по очереди.
        /// </summary>
        public void TestAllEvents()
        {
            if (EventManager.Instance == null)
            {
                Debug.LogError("EventManager.Instance не найден!");
                return;
            }

            Debug.Log("=== ТЕСТИРОВАНИЕ ВСЕХ СОБЫТИЙ ===");

            // Тест события ресурсов
            TestResourceEvent();

            // Тест события начала боя
            TestCombatEvent();

            // Тест события смерти юнита
            EventManager.Instance.TriggerEvent(EventManager.Events.OnUnitDied, new object[] { "Охранник-1" });

            // Тест события заражения
            EventManager.Instance.TriggerEvent(EventManager.Events.OnUnitInfected, new object[] { "Медик-2", 2 });

            // Тест события завершения миссии
            EventManager.Instance.TriggerEvent(EventManager.Events.OnMissionCompleted, new object[] { "Спасательная операция" });

            Debug.Log("=== ТЕСТИРОВАНИЕ ЗАВЕРШЕНО ===");
        }

        /// <summary>
        /// Тест события изменения ресурсов.
        /// </summary>
        public void TestResourceEvent()
        {
            if (EventManager.Instance == null) return;

            Debug.Log("Тест: событие ресурсов");
            
            // Генерируем случайные значения ресурсов
            int metal = Random.Range(50, 200);
            int element = Random.Range(20, 100);
            int organic = Random.Range(10, 50);
            int fuel = Random.Range(100, 300);
            int water = Random.Range(80, 250);

            EventManager.Instance.TriggerEvent(EventManager.Events.OnResourcesChanged,
                new object[] { metal, element, organic, fuel, water });

            Debug.Log($"Отправлены ресурсы: металл={metal}, элемент={element}, органика={organic}, топливо={fuel}, вода={water}");
        }

        /// <summary>
        /// Тест события начала боя.
        /// </summary>
        public void TestCombatEvent()
        {
            if (EventManager.Instance == null) return;

            Debug.Log("Тест: событие начала боя");
            EventManager.Instance.TriggerEvent(EventManager.Events.OnCombatStarted);
        }
        #endregion
    }
}