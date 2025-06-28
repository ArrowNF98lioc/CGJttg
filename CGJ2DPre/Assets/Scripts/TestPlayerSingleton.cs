using UnityEngine;

/// <summary>
/// 测试Player单例模式的脚本
/// </summary>
public class TestPlayerSingleton : MonoBehaviour
{
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    private void Start()
    {
        TestPlayerSingletonAccess();
    }
    
    /// <summary>
    /// 测试Player单例访问
    /// </summary>
    public void TestPlayerSingletonAccess()
    {
        Debug.Log("=== 开始测试Player单例访问 ===");
        
        // 测试Instance访问
        if (Player.Instance != null)
        {
            Debug.Log("✓ Player.Instance 访问成功");
            
            // 测试属性访问
            int maxHealth = Player.Instance.MaxHealth;
            int currentHealth = Player.Instance.CurrentHealth;
            float healthPercentage = Player.Instance.HealthPercentage;
            
            Debug.Log($"✓ Player属性访问成功:");
            Debug.Log($"  - MaxHealth: {maxHealth}");
            Debug.Log($"  - CurrentHealth: {currentHealth}");
            Debug.Log($"  - HealthPercentage: {healthPercentage:P1}");
            
            // 测试方法调用
            Player.Instance.ShowHealthInfo();
            Player.Instance.UpdateHealthStage();
            
            Debug.Log("✓ Player方法调用成功");
        }
        else
        {
            Debug.LogError("✗ Player.Instance 访问失败");
        }
        
        Debug.Log("=== Player单例测试完成 ===");
    }
    
    /// <summary>
    /// 测试设置健康值
    /// </summary>
    public void TestSetHealth()
    {
        if (Player.Instance != null)
        {
            int oldHealth = Player.Instance.CurrentHealth;
            int newHealth = oldHealth + 10;
            
            Player.Instance.SetHealth(newHealth);
            
            Debug.Log($"测试设置健康值: {oldHealth} -> {newHealth}");
        }
    }
    
    /// <summary>
    /// 测试健康阶段
    /// </summary>
    public void TestHealthStage()
    {
        if (Player.Instance != null)
        {
            Player.HealthStage stage = Player.Instance.GetHealthStage();
            string stageName = Player.Instance.GetHealthStageName(stage);
            
            Debug.Log($"当前健康阶段: {stageName} ({stage})");
        }
    }
    
    /// <summary>
    /// 运行所有测试
    /// </summary>
    [ContextMenu("运行所有测试")]
    public void RunAllTests()
    {
        TestPlayerSingletonAccess();
        TestSetHealth();
        TestHealthStage();
    }
} 