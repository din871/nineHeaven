using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBar; 
    [SerializeField] private PlayerHealth playerHealth; 

    private void Update()
    {
        if (healthBar != null && playerHealth != null)
        {
            // Вычисляем процент здоровья в реальном времени или лучше по другому ?  
            float healthPercentage = (float)playerHealth.GetCurrentHealth() / playerHealth.GetMaxHealth();
            healthBar.fillAmount = healthPercentage;
        }
    }
}