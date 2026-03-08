using UnityEngine;
using System.Collections.Generic;
using System;

namespace AlkariEvolution.Managers
{
    /// <summary>
    /// Простая система событий для слабой связанности компонентов.
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        #region Singleton
        public static EventManager Instance { get; private set; }

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

        // Словарь для хранения событий по имени
        private Dictionary<string, Action<object[]>> eventDictionary = new Dictionary<string, Action<object[]>>();

        #region Подписка и отписка
        /// <summary>
        /// Подписаться на событие.
        /// </summary>
        /// <param name="eventName">Имя события</param>
        /// <param name="listener">Метод-слушатель</param>
        public void StartListening(string eventName, Action<object[]> listener)
        {
            if (eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent += listener;
                eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                eventDictionary.Add(eventName, thisEvent);
            }
        }

        /// <summary>
        /// Отписаться от события.
        /// </summary>
        /// <param name="eventName">Имя события</param>
        /// <param name="listener">Метод-слушатель</param>
        public void StopListening(string eventName, Action<object[]> listener)
        {
            if (eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent -= listener;
                eventDictionary[eventName] = thisEvent;
            }
        }
        #endregion

        #region Вызов событий
        /// <summary>
        /// Вызвать событие без параметров.
        /// </summary>
        public void TriggerEvent(string eventName)
        {
            TriggerEvent(eventName, null);
        }

        /// <summary>
        /// Вызвать событие с параметрами.
        /// </summary>
        /// <param name="eventName">Имя события</param>
        /// <param name="parameters">Массив параметров</param>
        public void TriggerEvent(string eventName, object[] parameters)
        {
            if (eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent?.Invoke(parameters);
            }
        }
        #endregion

        #region Предопределённые события (для удобства)
        // Класс с константами имён событий, чтобы не ошибаться в строках
        public static class Events
        {
            // События ресурсов
            public const string OnResourcesChanged = "ON_RESOURCES_CHANGED";
            
            // События базы
            public const string OnModuleBuilt = "ON_MODULE_BUILT";
            public const string OnBaseUpgraded = "ON_BASE_UPGRADED";
            
            // События боя
            public const string OnCombatStarted = "ON_COMBAT_STARTED";
            public const string OnCombatEnded = "ON_COMBAT_ENDED";
            public const string OnUnitDied = "ON_UNIT_DIED";
            public const string OnUnitInfected = "ON_UNIT_INFECTED";
            
            // События миссий
            public const string OnMissionStarted = "ON_MISSION_STARTED";
            public const string OnMissionCompleted = "ON_MISSION_COMPLETED";
            public const string OnObjectiveUpdated = "ON_OBJECTIVE_UPDATED";
            
            // События глобальной карты
            public const string OnLocationReached = "ON_LOCATION_REACHED";
            public const string OnShipMoved = "ON_SHIP_MOVED";
        }
        #endregion
    }
}