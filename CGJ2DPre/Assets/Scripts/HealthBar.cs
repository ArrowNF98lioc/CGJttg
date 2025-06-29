using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthFillImage; // 拖拽血条的Fill部分Image

    void Update()
    {
        if (Player.Instance != null && healthFillImage != null)
        {
            float fillAmount = (float)Player.Instance.CurrentHealth / Player.Instance.MaxHealth;
            healthFillImage.fillAmount = Mathf.Clamp01(fillAmount);
            
            // 检查生命值并设置血条显示状态
            if (Player.Instance.CurrentHealth <= 0)
            {
                healthFillImage.enabled = false;
            }
            else
            {
                healthFillImage.enabled = true;
            }
        }
        else
        {
            // 如果Player实例或healthFillImage不存在，隐藏血条
            if (healthFillImage != null)
            {
                healthFillImage.enabled = false;
            }
        }
    }
}