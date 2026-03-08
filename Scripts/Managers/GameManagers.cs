using UnityEngine;

namespace AlkariEvolution.Managers
{
    /// <summary>
    /// Глобальный менеджер игры. Реализует паттерн синглтон.
    /// Управляет состоянием игры (пауза, переход между режимами).
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Сохраняем при загрузке сцен
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        #endregion

        #region State
        public enum GameState
        {
            GlobalMap,      // Глобальная карта
            TacticalCombat, // Тактический бой
            BaseManagement  // Управление базой
        }

        [SerializeField] private GameState currentState = GameState.GlobalMap;

        public GameState CurrentState => currentState;

        // Событие для оповещения других систем о смене состояния
        public event System.Action<GameState> OnStateChanged;

        /// <summary>
        /// Переключение состояния игры.
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            OnStateChanged?.Invoke(newState);

            // Здесь можно добавить логику при входе/выходе из состояния
            Debug.Log($"Game state changed to: {newState}");
        }
        #endregion

        #region Pause
        private bool isPaused = false;

        public bool IsPaused => isPaused;

        public event System.Action<bool> OnPauseChanged;

        /// <summary>
        /// Переключить паузу (для тактического режима).
        /// </summary>
        public void TogglePause()
        {
            SetPause(!isPaused);
        }

        public void SetPause(bool pause)
        {
            if (isPaused == pause) return;

            isPaused = pause;
            Time.timeScale = isPaused ? 0f : 1f;
            OnPauseChanged?.Invoke(isPaused);
            Debug.Log($"Pause set to: {isPaused}");
        }
        #endregion

        #region Quit
        /// <summary>
        /// Выход из игры (для кнопки в меню).
        /// </summary>
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        #endregion
    }
}