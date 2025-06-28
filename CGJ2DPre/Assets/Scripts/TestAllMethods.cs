using UnityEngine;

/// <summary>
/// 测试Player类所有方法的脚本
/// </summary>
public class TestAllMethods : MonoBehaviour
{
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    private void Start()
    {
        TestAllPlayerMethods();
    }
    
    /// <summary>
    /// 测试Player类的所有方法
    /// </summary>
    public void TestAllPlayerMethods()
    {
        Debug.Log("=== 开始测试Player类所有方法 ===");
        
        if (Player.Instance == null)
        {
            Debug.LogError("✗ Player.Instance 未找到");
            return;
        }
        
        Debug.Log("✓ Player.Instance 访问成功");
        
        // 测试基本属性
        TestBasicProperties();
        
        // 测试健康值相关方法
        TestHealthMethods();
        
        // 测试UI相关方法
        TestUIMethods();
        
        // 测试数据同步方法
        TestSyncMethods();
        
        Debug.Log("=== Player类所有方法测试完成 ===");
    }
    
    /// <summary>
    /// 测试基本属性
    /// </summary>
    private void TestBasicProperties()
    {
        Debug.Log("--- 测试基本属性 ---");
        
        int maxHealth = Player.Instance.MaxHealth;
        int currentHealth = Player.Instance.CurrentHealth;
        float healthPercentage = Player.Instance.HealthPercentage;
        
        Debug.Log($"✓ MaxHealth: {maxHealth}");
        Debug.Log($"✓ CurrentHealth: {currentHealth}");
        Debug.Log($"✓ HealthPercentage: {healthPercentage:P1}");
    }
    
    /// <summary>
    /// 测试健康值相关方法
    /// </summary>
    private void TestHealthMethods()
    {
        Debug.Log("--- 测试健康值相关方法 ---");
        
        // 测试设置健康值
        int oldHealth = Player.Instance.CurrentHealth;
        Player.Instance.SetHealth(oldHealth + 5);
        Debug.Log($"✓ SetHealth: {oldHealth} -> {Player.Instance.CurrentHealth}");
        
        // 测试设置最大健康值
        int oldMaxHealth = Player.Instance.MaxHealth;
        Player.Instance.SetMaxHealth(oldMaxHealth + 10);
        Debug.Log($"✓ SetMaxHealth: {oldMaxHealth} -> {Player.Instance.MaxHealth}");
        
        // 测试健康阶段
        Player.HealthStage stage = Player.Instance.GetHealthStage();
        string stageName = Player.Instance.GetHealthStageName(stage);
        Debug.Log($"✓ GetHealthStage: {stageName} ({stage})");
        
        // 测试健康信息
        string healthInfo = Player.Instance.GetHealthInfo();
        Debug.Log($"✓ GetHealthInfo: {healthInfo}");
    }
    
    /// <summary>
    /// 测试UI相关方法
    /// </summary>
    private void TestUIMethods()
    {
        Debug.Log("--- 测试UI相关方法 ---");
        
        // 测试更新健康阶段
        Player.Instance.UpdateHealthStage();
        Debug.Log("✓ UpdateHealthStage 调用成功");
        
        // 测试更新物品UI映射
        Player.Instance.UpdateItemUIMappings();
        Debug.Log("✓ UpdateItemUIMappings 调用成功");
        
        // 测试强制刷新UI
        Player.Instance.ForceRefreshUI();
        Debug.Log("✓ ForceRefreshUI 调用成功");
    }
    
    /// <summary>
    /// 测试数据同步方法
    /// </summary>
    private void TestSyncMethods()
    {
        Debug.Log("--- 测试数据同步方法 ---");
        
        // 测试同步到GameDataManager
        Player.Instance.SyncToGameDataManager();
        Debug.Log("✓ SyncToGameDataManager 调用成功");
        
        // 测试场景切换数据同步
        Player.Instance.OnSceneChanged();
        Debug.Log("✓ OnSceneChanged 调用成功");
    }
    
    /// <summary>
    /// 测试时间流逝相关方法
    /// </summary>
    public void TestTimeDecayMethods()
    {
        Debug.Log("--- 测试时间流逝相关方法 ---");
        
        // 测试启动时间流逝
        Player.Instance.StartHealthDecay();
        Debug.Log("✓ StartHealthDecay 调用成功");
        
        // 测试停止时间流逝
        Player.Instance.StopHealthDecay();
        Debug.Log("✓ StopHealthDecay 调用成功");
        
        // 测试设置时间流逝参数
        Player.Instance.SetHealthDecay(5f, 1);
        Debug.Log("✓ SetHealthDecay 调用成功");
        
        // 测试设置时间流逝启用状态
        Player.Instance.SetTimeDecayEnabled(false);
        Debug.Log("✓ SetTimeDecayEnabled 调用成功");
    }
    
    /// <summary>
    /// 测试物品UI映射相关方法
    /// </summary>
    public void TestItemUIMappingMethods()
    {
        Debug.Log("--- 测试物品UI映射相关方法 ---");
        
        // 测试验证物品UI映射
        Player.Instance.ValidateItemUIMappings();
        Debug.Log("✓ ValidateItemUIMappings 调用成功");
        
        // 测试显示物品UI映射列表信息
        Player.Instance.ShowItemUIMappingListInfo();
        Debug.Log("✓ ShowItemUIMappingListInfo 调用成功");
        
        // 测试获取物品UI映射信息
        string mappingInfo = Player.Instance.GetItemUIMappingInfo();
        Debug.Log($"✓ GetItemUIMappingInfo: {mappingInfo}");
        
        // 测试获取当前物品信息
        string currentItemInfo = Player.Instance.GetCurrentItemInfo();
        Debug.Log($"✓ GetCurrentItemInfo: {currentItemInfo}");
    }
    
    /// <summary>
    /// 运行所有测试
    /// </summary>
    [ContextMenu("运行所有测试")]
    public void RunAllTests()
    {
        TestAllPlayerMethods();
        TestTimeDecayMethods();
        TestItemUIMappingMethods();
    }
} 