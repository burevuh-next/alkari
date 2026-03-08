using AlkariEvolution.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResourceTester : MonoBehaviour
{
    void Update()
    {
        // Нажмите пробел, чтобы добавить ресурсы
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.AddMetal(10);
                ResourceManager.Instance.AddElement(5);
                ResourceManager.Instance.AddOrganic(2);
                Debug.Log("Ресурсы добавлены!");
            }
        }
        
        // Нажмите R, чтобы потратить ресурсы
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (ResourceManager.Instance != null)
            {
                bool success = ResourceManager.Instance.SpendResources(20, 10, 5, 0, 0);
                Debug.Log(success ? "Ресурсы потрачены" : "Недостаточно ресурсов");
            }
        }
    }
}