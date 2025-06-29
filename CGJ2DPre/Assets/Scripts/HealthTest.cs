using UnityEngine;

/// <summary>
/// 生命值初始化测试脚本
/// 用于验证游戏启动时生命值是否正确设置为59
/// </summary>
public class HealthTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool enableTesting = true;
    [SerializeField] private bool showDebugInfo = true;
    
    private void Start()
    {
        if (enableTesting)
        {
            TestHealthInitialization();
        }
    }
    
    private void Update()
    {
        if (!enableTesting) return;
        
        // 按H键显示当前生命值信息
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowCurrentHealthInfo();
        }
        
        // 按R键重置生命值为59
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetHealthTo59();
        }
    }
    
    /// <summary>
    /// 测试生命值初始化
    /// </summary>
    private void TestHealthInitialization()
    {
        Debug.Log("[HealthTest] ===== 生命值初始化测试 =====");
        
        // 检查GameDataManager
        if (GameDataManager.Instance != null)
        {
            Debug.Log($"[HealthTest] GameDataManager生命值: {GameDataManager.Instance.playerCurrentHealth}/{GameDataManager.Instance.playerMaxHealth}");
            
            if (GameDataManager.Instance.playerCurrentHealth != 59)
            {
                Debug.LogError($"[HealthTest] GameDataManager生命值错误！期望59，实际{GameDataManager.Instance.playerCurrentHealth}");
            }
            else
            {
                Debug.Log("[HealthTest] GameDataManager生命值正确 ✓");
            }
        }
        else
        {
            Debug.LogWarning("[HealthTest] GameDataManager实例未找到");
        }
        
        // 检查Player
        if (Player.Instance != null)
        {
            Debug.Log($"[HealthTest] Player生命值: {Player.Instance.CurrentHealth}/{Player.Instance.MaxHealth}");
            
            if (Player.Instance.CurrentHealth != 59)
            {
                Debug.LogError($"[HealthTest] Player生命值错误！期望59，实际{Player.Instance.CurrentHealth}");
            }
            else
            {
                Debug.Log("[HealthTest] Player生命值正确 ✓");
            }
        }
        else
        {
            Debug.LogWarning("[HealthTest] Player实例未找到");
        }
        
        // 检查PlayerController
        if (PlayerController.Instance != null && PlayerController.Instance.playerHealth != null)
        {
            Debug.Log($"[HealthTest] PlayerController生命值: {PlayerController.Instance.playerHealth.CurrentHealth}/{PlayerController.Instance.playerHealth.MaxHealth}");
            
            if (PlayerController.Instance.playerHealth.CurrentHealth != 59)
            {
                Debug.LogError($"[HealthTest] PlayerController生命值错误！期望59，实际{PlayerController.Instance.playerHealth.CurrentHealth}");
            }
            else
            {
                Debug.Log("[HealthTest] PlayerController生命值正确 ✓");
            }
        }
        else
        {
            Debug.LogWarning("[HealthTest] PlayerController实例未找到");
        }
        
        Debug.Log("[HealthTest] =================================");
    }
    
    /// <summary>
    /// 显示当前生命值信息
    /// </summary>
    private void ShowCurrentHealthInfo()
    {
        Debug.Log("[HealthTest] ===== 当前生命值信息 =====");
        
        if (GameDataManager.Instance != null)
        {
            Debug.Log($"[HealthTest] GameDataManager: {GameDataManager.Instance.playerCurrentHealth}/{GameDataManager.Instance.playerMaxHealth}");
        }
        
        if (Player.Instance != null)
        {
            Debug.Log($"[HealthTest] Player: {Player.Instance.CurrentHealth}/{Player.Instance.MaxHealth}");
        }
        
        if (PlayerController.Instance != null && PlayerController.Instance.playerHealth != null)
        {
            Debug.Log($"[HealthTest] PlayerController: {PlayerController.Instance.playerHealth.CurrentHealth}/{PlayerController.Instance.playerHealth.MaxHealth}");
        }
        
        Debug.Log("[HealthTest] =========================");
    }
    
    /// <summary>
    /// 重置生命值为59
    /// </summary>
    private void ResetHealthTo59()
    {
        Debug.Log("[HealthTest] 重置生命值为59...");
        
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.playerCurrentHealth = 59;
            Debug.Log("[HealthTest] GameDataManager生命值已重置为59");
        }
        
        if (Player.Instance != null)
        {
            Player.Instance.CurrentHealth = 59;
            Debug.Log("[HealthTest] Player生命值已重置为59");
        }
        
        if (PlayerController.Instance != null && PlayerController.Instance.playerHealth != null)
        {
            PlayerController.Instance.playerHealth.CurrentHealth = 59;
            Debug.Log("[HealthTest] PlayerController生命值已重置为59");
        }
        
        // 同步数据
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SyncAllGameData();
            Debug.Log("[HealthTest] 数据同步完成");
        }
    }
} 