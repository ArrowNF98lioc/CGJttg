using UnityEngine;

/// <summary>
/// 简化的商店交易按钮脚本
/// 专门用于按钮点击事件，执行物品典当交易
/// </summary>
public class ShopTradeButton : MonoBehaviour
{
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    /// <summary>
    /// 执行交易（用于按钮点击事件）
    /// </summary>
    public void ExecuteTrade()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[ShopTradeButton] Inventory实例未找到");
            return;
        }
        
        if (Player.Instance == null)
        {
            Debug.LogError("[ShopTradeButton] Player实例未找到");
            return;
        }
        
        if (Inventory.Instance.IsEmpty)
        {
            Debug.LogWarning("[ShopTradeButton] 背包为空，无法进行交易");
            return;
        }
        
        // 获取交易前的状态
        int itemCount = Inventory.Instance.CurrentItemCount;
        int totalHealth = Inventory.Instance.GetTotalHealthValue();
        int oldHealth = Player.Instance.CurrentHealth;
        
        // 执行交易
        // 1. 将物品的健康值加到玩家健康值上
        int newHealth = oldHealth + totalHealth;
        Player.Instance.SetHealth(newHealth);
        
        // 2. 清空背包
        Inventory.Instance.ClearInventory();
        
        // 3. 同步数据到GameDataManager
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SyncInventoryData();
        }
        
        // 显示交易结果
        string tradeResult = $"交易完成！\n";
        tradeResult += $"典当了 {itemCount} 个物品\n";
        tradeResult += $"获得 {totalHealth} 点健康值\n";
        tradeResult += $"健康值: {oldHealth} → {newHealth}";
        
        Debug.Log($"[ShopTradeButton] {tradeResult}");
        
        if (showDebugInfo)
        {
            Debug.Log($"[ShopTradeButton] 交易详情: 物品数={itemCount}, 健康值={totalHealth}, 旧健康值={oldHealth}, 新健康值={newHealth}");
        }
    }
    
    /// <summary>
    /// 获取当前交易信息
    /// </summary>
    /// <returns>交易信息字符串</returns>
    public string GetTradeInfo()
    {
        if (Inventory.Instance == null)
        {
            return "Inventory实例未找到";
        }
        
        if (Inventory.Instance.IsEmpty)
        {
            return "背包为空，无法交易";
        }
        
        string info = "当前可交易物品:\n";
        info += Inventory.Instance.GetItemsDetails();
        
        if (Player.Instance != null)
        {
            int totalHealth = Inventory.Instance.GetTotalHealthValue();
            int currentHealth = Player.Instance.CurrentHealth;
            int newHealth = currentHealth + totalHealth;
            
            info += $"\n交易效果:\n";
            info += $"当前健康值: {currentHealth}\n";
            info += $"交易后健康值: {newHealth}\n";
            info += $"健康值增加: +{totalHealth}";
        }
        
        return info;
    }
    
    /// <summary>
    /// 显示交易信息（调试用）
    /// </summary>
    [ContextMenu("显示交易信息")]
    public void ShowTradeInfo()
    {
        Debug.Log(GetTradeInfo());
    }
    
    /// <summary>
    /// 测试交易功能（调试用）
    /// </summary>
    [ContextMenu("测试交易")]
    public void TestTrade()
    {
        Debug.Log("=== 开始测试交易功能 ===");
        
        // 显示当前状态
        ShowTradeInfo();
        
        // 执行交易
        ExecuteTrade();
        
        Debug.Log("=== 交易测试完成 ===");
    }
} 