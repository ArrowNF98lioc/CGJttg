using UnityEngine;
using System.Collections.Generic;

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
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("[ShopTradeButton] GameDataManager实例未找到");
            return;
        }
        
        if (Player.Instance == null)
        {
            Debug.LogError("[ShopTradeButton] Player实例未找到");
            return;
        }
        
        if (!HasSelectedItems())
        {
            Debug.LogWarning("[ShopTradeButton] 没有Selected状态的物品，无法进行交易");
            return;
        }
        
        // 获取交易前的状态
        var selectedItemsInfo = GetSelectedItemsInfo();
        int oldHealth = Player.Instance.CurrentHealth;
        
        // 执行交易
        // 1. 将物品的健康值加到玩家健康值上
        int newHealth = oldHealth + selectedItemsInfo.totalHealth;
        Player.Instance.SetHealth(newHealth);
        
        // 2. 使用UpdateItemState方法更新物品状态，这样会触发CheckAllItemsSold检查
        List<string> tradedItems = new List<string>();
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            if (itemState.Value == PickableItem.ItemStateType.Selected)
            {
                // 使用UpdateItemState方法而不是直接修改字典
                GameDataManager.Instance.UpdateItemState(itemState.Key, PickableItem.ItemStateType.Solved);
                tradedItems.Add(itemState.Key);
                
                if (showDebugInfo)
                {
                    Debug.Log($"[ShopTradeButton] 物品状态更新: {itemState.Key} Selected → Solved");
                }
            }
        }
        
        // 3. 同步数据到GameDataManager
        GameDataManager.Instance.SyncPlayerData();
        
        // 显示交易结果
        string tradeResult = $"交易完成！\n";
        tradeResult += $"典当了 {selectedItemsInfo.count} 个Selected物品\n";
        tradeResult += $"获得 {selectedItemsInfo.totalHealth} 点健康值\n";
        tradeResult += $"健康值: {oldHealth} → {newHealth}\n";
        tradeResult += $"交易物品: {string.Join(", ", tradedItems)}";
        
        Debug.Log($"[ShopTradeButton] {tradeResult}");
        
        if (showDebugInfo)
        {
            Debug.Log($"[ShopTradeButton] 交易详情: 物品数={selectedItemsInfo.count}, 健康值={selectedItemsInfo.totalHealth}, 旧健康值={oldHealth}, 新健康值={newHealth}");
        }
    }
    
    /// <summary>
    /// 检查是否有Selected状态的物品
    /// </summary>
    /// <returns>是否有Selected状态的物品</returns>
    private bool HasSelectedItems()
    {
        if (GameDataManager.Instance == null) return false;
        
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            if (itemState.Value == PickableItem.ItemStateType.Selected)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 获取Selected状态物品的信息
    /// </summary>
    /// <returns>物品信息（数量和总健康值）</returns>
    private (int count, int totalHealth) GetSelectedItemsInfo()
    {
        int count = 0;
        int totalHealth = 0;
        
        if (GameDataManager.Instance == null || ItemManager.Instance == null) 
            return (count, totalHealth);
        
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            if (itemState.Value == PickableItem.ItemStateType.Selected)
            {
                count++;
                Item item = ItemManager.Instance.GetItem(itemState.Key);
                if (item != null)
                {
                    totalHealth += item.health;
                }
            }
        }
        
        return (count, totalHealth);
    }
    
    /// <summary>
    /// 获取当前交易信息
    /// </summary>
    /// <returns>交易信息字符串</returns>
    public string GetTradeInfo()
    {
        if (GameDataManager.Instance == null)
        {
            return "GameDataManager实例未找到";
        }
        
        if (!HasSelectedItems())
        {
            return "没有Selected状态的物品，无法交易";
        }
        
        string info = "当前可交易物品 (Selected状态):\n";
        info += GetSelectedItemsDetails();
        
        if (Player.Instance != null)
        {
            var selectedItemsInfo = GetSelectedItemsInfo();
            int currentHealth = Player.Instance.CurrentHealth;
            int newHealth = currentHealth + selectedItemsInfo.totalHealth;
            
            info += $"\n交易效果:\n";
            info += $"当前健康值: {currentHealth}\n";
            info += $"交易后健康值: {newHealth}\n";
            info += $"健康值增加: +{selectedItemsInfo.totalHealth}";
        }
        
        return info;
    }
    
    /// <summary>
    /// 获取Selected状态物品的详细信息
    /// </summary>
    /// <returns>物品详细信息字符串</returns>
    private string GetSelectedItemsDetails()
    {
        if (GameDataManager.Instance == null || ItemManager.Instance == null)
            return "无法获取物品信息";
        
        string details = "";
        int itemCount = 0;
        
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            if (itemState.Value == PickableItem.ItemStateType.Selected)
            {
                itemCount++;
                Item item = ItemManager.Instance.GetItem(itemState.Key);
                if (item != null)
                {
                    details += $"{itemCount}. {item.name} (+{item.health}健康值)\n";
                }
                else
                {
                    details += $"{itemCount}. {itemState.Key} (健康值未知)\n";
                }
            }
        }
        
        if (string.IsNullOrEmpty(details))
        {
            details = "没有Selected状态的物品";
        }
        
        return details;
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