using UnityEngine;
using TMPro;

namespace AlkariEvolution.GlobalMap
{
    /// <summary>
    /// Компонент для визуального представления локации
    /// </summary>
    public class LocationView : MonoBehaviour
    {
        [Header("Компоненты")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TextMeshPro nameLabel;
        [SerializeField] private GameObject exploredIndicator;

        private LocationSO locationData;
        private bool isExplored = false;

        private void Awake()
        {
            // Если компоненты не назначены, пытаемся найти
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (nameLabel == null)
                nameLabel = GetComponentInChildren<TextMeshPro>();
        }

        public void Initialize(LocationSO data)
        {
            locationData = data;

            if (spriteRenderer != null)
            {
                if (data.iconSprite != null)
                    spriteRenderer.sprite = data.iconSprite;
                spriteRenderer.color = data.iconColor;
            }

            if (nameLabel != null)
                nameLabel.text = data.locationName;

            isExplored = data.isExplored;
            UpdateVisuals();
        }

        public void MarkAsExplored()
        {
            isExplored = true;
            if (locationData != null)
                locationData.isExplored = true;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (exploredIndicator != null)
                exploredIndicator.SetActive(isExplored);

            if (spriteRenderer != null)
            {
                // Можно затемнять неисследованные локации
                Color color = spriteRenderer.color;
                color.a = isExplored ? 1f : 0.7f;
                spriteRenderer.color = color;
            }
        }

        private void OnMouseDown()
        {
            // Обработка клика по локации
            if (GlobalMapManager.Instance != null)
            {
                GlobalMapManager.Instance.MoveShipToLocation(gameObject);
            }
        }
    }
}