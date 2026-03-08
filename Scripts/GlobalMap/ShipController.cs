using UnityEngine;

namespace AlkariEvolution.GlobalMap
{
    /// <summary>
    /// Управление кораблём на глобальной карте
    /// </summary>
    public class ShipController : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 360f;

        private Vector2 targetPosition;
        private bool isMoving = false;

        private void Start()
        {
            targetPosition = transform.position;
        }

        private void Update()
        {
            if (isMoving)
            {
                MoveToTarget();
            }
        }

        /// <summary>
        /// Начать движение к цели
        /// </summary>
        public void MoveTo(Vector2 target)
        {
            targetPosition = target;
            isMoving = true;
        }
        // Событие окончания движения
        public System.Action OnMoveCompleted;

        private void MoveToTarget()
        {
            // Плавное движение к цели
            Vector2 currentPos = transform.position;
            Vector2 direction = (targetPosition - currentPos).normalized;

            // Поворот корабля
            if (direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.deltaTime
                );
            }

            // Движение вперёд
            transform.position = Vector2.MoveTowards(
                currentPos, 
                targetPosition, 
                moveSpeed * Time.deltaTime
            );

            // Проверка достижения цели
            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;

                OnMoveCompleted?.Invoke();
            }
        }
    }
}