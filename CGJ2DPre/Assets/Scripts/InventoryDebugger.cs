using UnityEngine;

/// <summary>
/// 背包调试器
/// 用于诊断背包物品丢失问题
/// </summary>
public class InventoryDebugger : MonoBehaviour
{
    [Header("调试设置")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool autoDebugOnStart = true;
    
    private void Start()
    {
        if (autoDebugOnStart)
        {
            DebugInventoryState();
        }
    }
    
    /// <summary>
    /// 调试背包状态
    /// </summary>
    [ContextMenu("调试背包状态")]
    public void DebugInventoryState()
    {
        Debug.Log("=== 背包状态调试 ===");
        
        // 检查Inventory实例
        if (Inventory.Instance == null)
        {
            Debug.LogError("✗ Inventory实例未找到");
            return;
        }
        Debug.Log("✓ Inventory实例存在");
        
        // 检查GameDataManager实例
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("✗ GameDataManager实例未找到");
            return;
        }
        Debug.Log("✓ GameDataManager实例存在");
        
        // 显示Inventory状态
        Debug.Log($"--- Inventory状态 ---");
        Debug.Log($"当前物品数量: {Inventory.Instance.CurrentItemCount}");
        Debug.Log($"最大槽位数: {Inventory.Instance.MaxSlots}");
        Debug.Log($"背包是否为空: {Inventory.Instance.IsEmpty}");
        Debug.Log($"背包是否已满: {Inventory.Instance.IsFull}");
        
        // 显示GameDataManager中的物品
        Debug.Log($"--- GameDataManager状态 ---");
        Debug.Log($"collectedItems数量: {GameDataManager.Instance.collectedItems.Count}");
        
        if (GameDataManager.Instance.collectedItems.Count > 0)
        {
            Debug.Log("collectedItems中的物品:");
            foreach (string itemName in GameDataManager.Instance.collectedItems)
            {
                Debug.Log($"  - {itemName}");
            }
        }
        else
        {
            Debug.LogWarning("collectedItems为空！这可能是问题所在。");
        }
        
        // 显示Inventory中的物品
        if (!Inventory.Instance.IsEmpty)
        {
            Debug.Log("--- Inventory中的物品 ---");
            for (int i = 0; i < Inventory.Instance.CurrentItemCount; i++)
            {
                Item item = Inventory.Instance.GetItemAtSlot(i);
                if (item != null)
                {
                    Debug.Log($"  {i + 1}. {item.name} (健康值: {item.health})");
                }
            }
        }
        else
        {
            Debug.LogWarning("Inventory为空！");
        }
        
        // 检查数据同步状态
        CheckDataSyncStatus();
        
        Debug.Log("=== 调试完成 ===");
    }
    
    /// <summary>
    /// 检查数据同步状态
    /// </summary>
    private void CheckDataSyncStatus()
    {
        Debug.Log("--- 数据同步状态检查 ---");
        
        // 检查Inventory到GameDataManager的同步
        bool inventoryToGameDataSync = true;
        for (int i = 0; i < Inventory.Instance.CurrentItemCount; i++)
        {
            Item item = Inventory.Instance.GetItemAtSlot(i);
            if (item != null && !GameDataManager.Instance.collectedItems.Contains(item.name))
            {
                inventoryToGameDataSync = false;
                Debug.LogError($"✗ Inventory中的物品 '{item.name}' 不在GameDataManager的collectedItems中");
            }
        }
        
        if (inventoryToGameDataSync)
        {
            Debug.Log("✓ Inventory到GameDataManager同步正常");
        }
        
        // 检查GameDataManager到Inventory的同步
        bool gameDataToInventorySync = true;
        foreach (string itemName in GameDataManager.Instance.collectedItems)
        {
            if (!Inventory.Instance.HasItem(itemName))
            {
                gameDataToInventorySync = false;
                Debug.LogError($"✗ GameDataManager中的物品 '{itemName}' 不在Inventory中");
            }
        }
        
        if (gameDataToInventorySync)
        {
            Debug.Log("✓ GameDataManager到Inventory同步正常");
        }
        
        // 总结
        if (inventoryToGameDataSync && gameDataToInventorySync)
        {
            Debug.Log("✓ 数据同步状态正常");
        }
        else
        {
            Debug.LogError("✗ 数据同步状态异常");
        }
    }
    
    /// <summary>
    /// 强制同步数据
    /// </summary>
    [ContextMenu("强制同步数据")]
    public void ForceSyncData()
    {
        Debug.Log("=== 强制同步数据 ===");
        
        // 从Inventory同步到GameDataManager
        if (Inventory.Instance != null)
        {
            Inventory.Instance.SyncToGameDataManager();
            Debug.Log("✓ 从Inventory同步到GameDataManager完成");
        }
        
        // 从GameDataManager同步到Inventory
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SyncDataToInventory();
            Debug.Log("✓ 从GameDataManager同步到Inventory完成");
        }
        
        // 重新调试状态
        DebugInventoryState();
    }
    
    /// <summary>
    /// 添加测试物品
    /// </summary>
    [ContextMenu("添加测试物品")]
    public void AddTestItems()
    {
        Debug.Log("=== 添加测试物品 ===");
        
        if (Inventory.Instance != null)
        {
            bool success1 = Inventory.Instance.AddItem("项链");
            bool success2 = Inventory.Instance.AddItem("茶壶");
            bool success3 = Inventory.Instance.AddItem("植物");
            
            Debug.Log($"添加结果: 项链={success1}, 茶壶={success2}, 植物={success3}");
            
            // 同步到GameDataManager
            Inventory.Instance.SyncToGameDataManager();
            
            // 重新调试状态
            DebugInventoryState();
        }
    }
    
    /// <summary>
    /// 清空并重新同步
    /// </summary>
    [ContextMenu("清空并重新同步")]
    public void ClearAndResync()
    {
        Debug.Log("=== 清空并重新同步 ===");
        
        if (Inventory.Instance != null)
        {
            // 清空背包
            Inventory.Instance.ClearInventory();
            Debug.Log("✓ 背包已清空");
            
            // 从GameDataManager重新同步
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.SyncDataToInventory();
                Debug.Log("✓ 从GameDataManager重新同步完成");
            }
            
            // 重新调试状态
            DebugInventoryState();
        }
    }
    
    /// <summary>
    /// 检查场景切换时的数据状态
    /// </summary>
    [ContextMenu("检查场景切换数据状态")]
    public void CheckSceneTransitionData()
    {
        Debug.Log("=== 检查场景切换数据状态 ===");
        
        // 模拟场景切换前的数据保存
        Debug.Log("--- 场景切换前数据保存 ---");
        
        if (Inventory.Instance != null)
        {
            Inventory.Instance.SyncToGameDataManager();
            Debug.Log("✓ Inventory数据已保存到GameDataManager");
        }
        
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveGameData();
            Debug.Log("✓ GameDataManager数据已保存");
        }
        
        // 显示保存后的状态
        Debug.Log($"保存后collectedItems数量: {GameDataManager.Instance.collectedItems.Count}");
        
        // 模拟场景切换后的数据恢复
        Debug.Log("--- 场景切换后数据恢复 ---");
        
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SyncDataToInventory();
            Debug.Log("✓ 数据已恢复到Inventory");
        }
        
        // 显示恢复后的状态
        Debug.Log($"恢复后Inventory物品数量: {Inventory.Instance.CurrentItemCount}");
        
        // 重新调试状态
        DebugInventoryState();
    }
    
    /// <summary>
    /// 测试商店交易功能
    /// </summary>
    [ContextMenu("测试商店交易功能")]
    public void TestShopTrade()
    {
        Debug.Log("=== 测试商店交易功能 ===");
        
        if (Inventory.Instance != null)
        {
            // 显示当前物品详情
            string details = Inventory.Instance.GetItemsDetails();
            Debug.Log($"当前物品详情:\n{details}");
            
            // 显示总健康值
            int totalHealth = Inventory.Instance.GetTotalHealthValue();
            Debug.Log($"总健康值: {totalHealth}");
            
            // 测试清空背包
            if (!Inventory.Instance.IsEmpty)
            {
                Debug.Log("测试清空背包...");
                Inventory.Instance.ClearInventory();
                Debug.Log("✓ 背包已清空");
                
                // 重新调试状态
                DebugInventoryState();
            }
            else
            {
                Debug.Log("背包已为空，无法测试清空功能");
            }
        }
    }
} 