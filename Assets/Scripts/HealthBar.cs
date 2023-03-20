using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Beyblade beyblade;
    [SerializeField] private Image healthFill;
    private bool depleted = false;

    private void Update()
    {
        if (depleted) return;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthFill.fillAmount = beyblade.GetAngularVelocityRatio();

        if (healthFill.fillAmount <= 0.01f)
        {
            healthFill.fillAmount = 0f;
            depleted = true;
        }
    }
}