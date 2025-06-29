using UnityEngine;

/// <summary>
/// 游戏结束系统测试脚本
/// 用于测试各种游戏结束条件
/// </summary>
public class GameEndTester : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool enableTesting = true;
    [SerializeField] private bool showDebugInfo = true;
    
    [Header("生命值测试")]
    [SerializeField] private bool testHealthZero = true;
    [SerializeField] private int testHealthValue = 0;
    
    [Header("物品测试")]
    [SerializeField] private bool testAllItemsSold = true;
    [SerializeField] private string testItemName = "项链";
    
    [Header("放弃测试")]
    [SerializeField] private bool testPlayerGiveUp = true;
    
    private void Start()
    {
        if (enableTesting)
        {
            Debug.Log("[GameEndTester] 游戏结束系统测试器启动");
            ShowSystemStatus();
        }
    }
    
    private void Update()
    {
        if (!enableTesting) return;
        
        // 测试生命值归零
        if (testHealthZero && Input.GetKeyDown(KeyCode.H))
        {
            TestHealthZero();
        }
        
        // 测试所有物品被当掉
        if (testAllItemsSold && Input.GetKeyDown(KeyCode.I))
        {
            TestAllItemsSold();
        }
        
        // 测试玩家放弃
        if (testPlayerGiveUp && Input.GetKeyDown(KeyCode.G))
        {
            TestPlayerGiveUp();
        }
        
        // 显示帮助信息
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ShowHelpInfo();
        }
    }
    
    /// <summary>
    /// 显示系统状态
    /// </summary>
    private void ShowSystemStatus()
    {
        if (!showDebugInfo) return;
        
        string status = "[GameEndTester] 系统状态:\n";
        status += $"GameEndManager: {(GameEndManager.Instance != null ? "已连接" : "未连接")}\n";
        status += $"GameDataManager: {(GameDataManager.Instance != null ? "已连接" : "未连接")}\n";
        status += $"Player: {(Player.Instance != null ? "已连接" : "未连接")}\n";
        
        if (Player.Instance != null)
        {
            status += $"当前生命值: {Player.Instance.CurrentHealth}/{Player.Instance.MaxHealth}\n";
        }
        
        if (GameDataManager.Instance != null)
        {
            status += $"物品状态数量: {GameDataManager.Instance.itemStates.Count}\n";
            foreach (var itemState in GameDataManager.Instance.itemStates)
            {
                status += $"  - {itemState.Key}: {itemState.Value}\n";
            }
        }
        
        Debug.Log(status);
    }
    
    /// <summary>
    /// 测试生命值归零
    /// </summary>
    [ContextMenu("测试生命值归零")]
    public void TestHealthZero()
    {
        if (!testHealthZero) return;
        
        Debug.Log("[GameEndTester] 开始测试生命值归零");
        
        if (Player.Instance != null)
        {
            int oldHealth = Player.Instance.CurrentHealth;
            Player.Instance.SetHealth(testHealthValue);
            
            if (showDebugInfo)
            {
                Debug.Log($"[GameEndTester] 生命值设置: {oldHealth} -> {testHealthValue}");
            }
        }
        else
        {
            Debug.LogWarning("[GameEndTester] Player实例未找到");
        }
    }
    
    /// <summary>
    /// 测试所有物品卖掉后的游戏结束
    /// </summary>
    [ContextMenu("测试所有物品卖掉")]
    public void TestAllItemsSold()
    {
        Debug.Log("=== 开始测试所有物品卖掉 ===");
        
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("GameDataManager实例未找到");
            return;
        }
        
        if (GameEndManager.Instance == null)
        {
            Debug.LogError("GameEndManager实例未找到");
            return;
        }
        
        // 显示当前物品状态
        Debug.Log("当前物品状态:");
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            Debug.Log($"- {itemState.Key}: {itemState.Value}");
        }
        
        // 将所有物品状态设置为Solved
        Debug.Log("将所有物品状态设置为Solved...");
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            GameDataManager.Instance.UpdateItemState(itemState.Key, PickableItem.ItemStateType.Solved);
        }
        
        // 显示更新后的物品状态
        Debug.Log("更新后的物品状态:");
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            Debug.Log($"- {itemState.Key}: {itemState.Value}");
        }
        
        // 检查游戏结束状态
        if (GameEndManager.Instance.IsGameOver())
        {
            Debug.Log($"游戏已结束，原因: {GameEndManager.Instance.GetCurrentEndReason()}");
        }
        else
        {
            Debug.Log("游戏未结束");
        }
        
        Debug.Log("=== 测试完成 ===");
    }
    
    /// <summary>
    /// 测试玩家放弃
    /// </summary>
    [ContextMenu("测试玩家放弃")]
    public void TestPlayerGiveUp()
    {
        if (!testPlayerGiveUp) return;
        
        Debug.Log("[GameEndTester] 开始测试玩家放弃");
        
        if (GameEndManager.Instance != null)
        {
            GameEndManager.Instance.EndGame(GameEndManager.GameEndReason.PlayerGaveUp);
        }
        else
        {
            Debug.LogWarning("[GameEndTester] GameEndManager实例未找到");
        }
    }
    
    /// <summary>
    /// 重置所有物品状态为AtHome
    /// </summary>
    [ContextMenu("重置所有物品状态")]
    public void ResetAllItemStates()
    {
        Debug.Log("=== 重置所有物品状态 ===");
        
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("GameDataManager实例未找到");
            return;
        }
        
        // 将所有物品状态重置为AtHome
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            GameDataManager.Instance.UpdateItemState(itemState.Key, PickableItem.ItemStateType.AtHome);
        }
        
        // 重置游戏结束状态
        if (GameEndManager.Instance != null)
        {
            GameEndManager.Instance.ResetGameOverState();
        }
        
        Debug.Log("所有物品状态已重置为AtHome");
        Debug.Log("=== 重置完成 ===");
    }
    
    /// <summary>
    /// 恢复玩家生命值
    /// </summary>
    [ContextMenu("恢复玩家生命值")]
    public void RestorePlayerHealth()
    {
        Debug.Log("[GameEndTester] 恢复玩家生命值");
        
        if (Player.Instance != null)
        {
            Player.Instance.SetHealth(Player.Instance.MaxHealth);
            
            if (showDebugInfo)
            {
                Debug.Log($"[GameEndTester] 生命值恢复: {Player.Instance.CurrentHealth}/{Player.Instance.MaxHealth}");
            }
        }
    }
    
    /// <summary>
    /// 重置游戏结束状态
    /// </summary>
    [ContextMenu("重置游戏结束状态")]
    public void ResetGameOverState()
    {
        Debug.Log("[GameEndTester] 重置游戏结束状态");
        
        if (GameEndManager.Instance != null)
        {
            GameEndManager.Instance.ResetGameOverState();
        }
    }
    
    /// <summary>
    /// 显示帮助信息
    /// </summary>
    private void ShowHelpInfo()
    {
        string help = "[GameEndTester] 测试快捷键:\n";
        help += "H - 测试生命值归零\n";
        help += "I - 测试所有物品被当掉\n";
        help += "G - 测试玩家放弃\n";
        help += "F1 - 显示此帮助信息\n";
        help += "\nContext Menu选项:\n";
        help += "- 测试生命值归零\n";
        help += "- 测试所有物品被当掉\n";
        help += "- 测试玩家放弃\n";
        help += "- 重置所有物品状态\n";
        help += "- 恢复玩家生命值\n";
        help += "- 重置游戏结束状态";
        
        Debug.Log(help);
    }
    
    /// <summary>
    /// 运行完整测试
    /// </summary>
    [ContextMenu("运行完整测试")]
    public void RunFullTest()
    {
        Debug.Log("[GameEndTester] 开始运行完整测试");
        
        // 显示系统状态
        ShowSystemStatus();
        
        // 显示帮助信息
        ShowHelpInfo();
        
        Debug.Log("[GameEndTester] 完整测试完成，请使用快捷键进行具体测试");
    }
    
    /// <summary>
    /// 显示当前状态信息
    /// </summary>
    [ContextMenu("显示当前状态")]
    public void ShowCurrentStatus()
    {
        Debug.Log("=== 当前状态信息 ===");
        
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("GameDataManager实例未找到");
            return;
        }
        
        if (GameEndManager.Instance == null)
        {
            Debug.LogError("GameEndManager实例未找到");
            return;
        }
        
        // 显示物品状态
        Debug.Log("物品状态:");
        int totalItems = 0;
        int soldItems = 0;
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            Debug.Log($"- {itemState.Key}: {itemState.Value}");
            totalItems++;
            if (itemState.Value == PickableItem.ItemStateType.Solved)
            {
                soldItems++;
            }
        }
        
        Debug.Log($"总计: {totalItems}个物品，已卖掉: {soldItems}个");
        
        // 显示游戏结束状态
        if (GameEndManager.Instance.IsGameOver())
        {
            Debug.Log($"游戏已结束，原因: {GameEndManager.Instance.GetCurrentEndReason()}");
        }
        else
        {
            Debug.Log("游戏未结束");
        }
        
        // 显示玩家生命值
        if (Player.Instance != null)
        {
            Debug.Log($"玩家生命值: {Player.Instance.CurrentHealth}/{Player.Instance.MaxHealth}");
        }
        
        Debug.Log("=== 状态信息显示完成 ===");
    }
    
    /// <summary>
    /// 详细检查GameEndManager设置
    /// </summary>
    [ContextMenu("详细检查GameEndManager")]
    public void DetailedCheckGameEndManager()
    {
        Debug.Log("=== 详细检查GameEndManager ===");
        
        // 检查GameEndManager实例
        if (GameEndManager.Instance == null)
        {
            Debug.LogError("❌ GameEndManager.Instance 为 null");
            Debug.Log("请确保在场景中添加了GameEndManager组件");
            return;
        }
        else
        {
            Debug.Log("✅ GameEndManager.Instance 存在");
        }
        
        // 检查GameEndManager组件
        GameEndManager gameEndManager = GameEndManager.Instance;
        
        // 检查游戏结束状态
        Debug.Log($"游戏结束状态: {(gameEndManager.IsGameOver() ? "已结束" : "未结束")}");
        Debug.Log($"当前结束原因: {gameEndManager.GetCurrentEndReason()}");
        
        // 检查UI组件设置
        var gameOverPanel = gameEndManager.GetType().GetField("gameOverPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(gameEndManager);
        Debug.Log($"游戏结束面板: {(gameOverPanel != null ? "已设置" : "未设置")}");
        
        // 检查启用状态
        var enableItemCheck = gameEndManager.GetType().GetField("enableItemCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(gameEndManager);
        Debug.Log($"物品检查启用: {enableItemCheck}");
        
        var showDebugInfo = gameEndManager.GetType().GetField("showDebugInfo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(gameEndManager);
        Debug.Log($"调试信息启用: {showDebugInfo}");
        
        // 检查GameDataManager
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("❌ GameDataManager.Instance 为 null");
        }
        else
        {
            Debug.Log("✅ GameDataManager.Instance 存在");
            
            // 检查物品状态
            Debug.Log($"物品总数: {GameDataManager.Instance.itemStates.Count}");
            int soldItems = 0;
            foreach (var itemState in GameDataManager.Instance.itemStates)
            {
                if (itemState.Value == PickableItem.ItemStateType.Solved)
                {
                    soldItems++;
                }
                Debug.Log($"  - {itemState.Key}: {itemState.Value}");
            }
            Debug.Log($"已卖掉物品数: {soldItems}");
            
            // 手动检查是否应该触发游戏结束
            if (GameDataManager.Instance.itemStates.Count > 0 && soldItems == GameDataManager.Instance.itemStates.Count)
            {
                Debug.Log("⚠️ 所有物品都已卖掉，应该触发游戏结束！");
                
                // 手动触发游戏结束
                Debug.Log("手动触发游戏结束...");
                gameEndManager.EndGame(GameEndManager.GameEndReason.AllItemsSold);
            }
            else
            {
                Debug.Log("ℹ️ 不是所有物品都已卖掉");
            }
        }
        
        Debug.Log("=== 详细检查完成 ===");
    }
    
    /// <summary>
    /// 测试游戏结束UI显示
    /// </summary>
    [ContextMenu("测试游戏结束UI")]
    public void TestGameOverUI()
    {
        Debug.Log("=== 测试游戏结束UI ===");
        
        if (GameEndManager.Instance == null)
        {
            Debug.LogError("GameEndManager实例未找到");
            return;
        }
        
        // 检查UI组件
        var type = GameEndManager.Instance.GetType();
        var gameOverPanelField = type.GetField("gameOverPanel", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var gameOverPanel = gameOverPanelField?.GetValue(GameEndManager.Instance);
        
        if (gameOverPanel == null)
        {
            Debug.LogError("❌ 游戏结束面板未设置");
            Debug.Log("请使用AutoGameEndManager脚本创建UI组件");
            return;
        }
        
        Debug.Log("✅ 游戏结束面板已设置");
        
        // 测试显示游戏结束UI
        Debug.Log("触发游戏结束...");
        GameEndManager.Instance.EndGame(GameEndManager.GameEndReason.AllItemsSold);
        
        Debug.Log("=== 测试完成 ===");
    }
} 