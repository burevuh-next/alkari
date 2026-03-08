using UnityEngine;
using UnityEngine.UI;
using TMPro; // если используете TextMeshPro (рекомендуется)

namespace AlkariEvolution.Managers
{
    /// <summary>
    /// Управление интерфейсом пользователя.
    /// Подписывается на события ResourceManager и GameManager.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Singleton
        public static UIManager Instance { get; private set; }

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

        [Header("Панели интерфейса")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject inventoryPanel;

        [Header("Текстовые поля ресурсов")]
        [SerializeField] private TextMeshProUGUI metalText;
        [SerializeField] private TextMeshProUGUI elementText;
        [SerializeField] private TextMeshProUGUI organicText;
        [SerializeField] private TextMeshProUGUI fuelText;
        [SerializeField] private TextMeshProUGUI waterText;

        [Header("Сообщения")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float messageDisplayTime = 3f;

        private void Start()
        {
            // Подписываемся на события GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
                GameManager.Instance.OnPauseChanged += OnPauseChanged;
            }

            // Показываем главное меню при старте
            ShowMainMenu();
        }

        private void UpdateResourceUI()
        {
            if (ResourceManager.Instance != null)
            {
                UpdateResourceUI(
                    ResourceManager.Instance.Metal,
                    ResourceManager.Instance.Element,
                    ResourceManager.Instance.Organic,
                    ResourceManager.Instance.Fuel,
                    ResourceManager.Instance.Water
                );
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
                GameManager.Instance.OnPauseChanged -= OnPauseChanged;
            }
        }
 
        private void OnEnable()
        {
            // Подписываемся на события GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
                GameManager.Instance.OnPauseChanged += OnPauseChanged;
            }

            // Подписываемся на события через EventManager
            if (EventManager.Instance != null)
            {
                EventManager.Instance.StartListening(EventManager.Events.OnResourcesChanged, OnResourcesChanged);
            }

            // Показываем главное меню при старте
            ShowMainMenu();
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
                GameManager.Instance.OnPauseChanged -= OnPauseChanged;
            }

            if (EventManager.Instance != null)
            {
                EventManager.Instance.StopListening(EventManager.Events.OnResourcesChanged, OnResourcesChanged);
            }
        }

        // Новый метод-обработчик события
        private void OnResourcesChanged(object[] parameters)
        {
            if (parameters != null && parameters.Length >= 5)
            {
                try
                {
                    int metal = (int)parameters[0];
                    int element = (int)parameters[1];
                    int organic = (int)parameters[2];
                    int fuel = (int)parameters[3];
                    int water = (int)parameters[4];
                    
                    UpdateResourceUI(metal, element, organic, fuel, water);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error processing resources changed event: {e.Message}");
                }
            }
        }

        public void ShowGlobalMap()
        {
            mainMenuPanel?.SetActive(false);
            hudPanel?.SetActive(false);
            // Здесь активируем панель карты (создайте её позже)
        }

        private void OnGameStateChanged(GameManager.GameState newState)
        {
            // Здесь можно обновлять интерфейс в зависимости от состояния
            Debug.Log($"UI: Game state changed to {newState}");
        }

        private void OnPauseChanged(bool isPaused)
        {
            pausePanel?.SetActive(isPaused);
        }

        #region Панели интерфейса
        public void ShowMainMenu()
        {
            mainMenuPanel?.SetActive(true);
            hudPanel?.SetActive(false);
            inventoryPanel?.SetActive(false);
            pausePanel?.SetActive(false);
        }

        public void ShowHUD()
        {
            mainMenuPanel?.SetActive(false);
            hudPanel?.SetActive(true);
            inventoryPanel?.SetActive(false);
            pausePanel?.SetActive(false);
        }

        public void ToggleInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            }
        }
        #endregion

        #region Обновление ресурсов
        /// <summary>
        /// Обновляет отображение ресурсов на HUD.
        /// </summary>
        public void UpdateResourceUI(int metal, int element, int organic, int fuel, int water)
        {
            if (metalText != null) metalText.text = metal.ToString();
            if (elementText != null) elementText.text = element.ToString();
            if (organicText != null) organicText.text = organic.ToString();
            if (fuelText != null) fuelText.text = fuel.ToString();
            if (waterText != null) waterText.text = water.ToString();
        }
        #endregion

        #region Сообщения
        /// <summary>
        /// Показывает временное сообщение в центре экрана.
        /// </summary>
        public void ShowMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
                messageText.gameObject.SetActive(true);
                CancelInvoke(nameof(HideMessage));
                Invoke(nameof(HideMessage), messageDisplayTime);
            }
        }

        private void HideMessage()
        {
            if (messageText != null)
            {
                messageText.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Кнопки (для привязки в инспекторе)
        public void OnStartGameClicked()
        {
            ShowHUD();
            GameManager.Instance?.ChangeState(GameManager.GameState.GlobalMap);
        }

        public void OnResumeClicked()
        {
            GameManager.Instance?.SetPause(false);
        }

        public void OnQuitClicked()
        {
            GameManager.Instance?.QuitGame();
        }
        #endregion
    }
}