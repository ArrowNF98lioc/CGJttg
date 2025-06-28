using UnityEngine;

/// <summary>
/// 背包系统测试脚本
/// 演示当铺系统的使用方法和键盘操作
/// </summary>
public class InventoryExample : MonoBehaviour
{
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    private void Start()
    {
        Debug.Log("[InventoryExample] 背包系统测试脚本已启动");
        Debug.Log("键盘操作说明:");
        Debug.Log("A键: 添加测试物品 (旧书)");
        Debug.Log("R键: 添加随机可交易物品");
        Debug.Log("T键: 在当铺交易当前物品");
        Debug.Log("P键: 显示交易预览");
        Debug.Log("U键: 使用当前物品 (药水)");
        Debug.Log("D键: 丢弃当前物品");
        Debug.Log("C键: 清空背包");
        Debug.Log("1键: 添加生活物品");
        Debug.Log("2键: 添加材料");
        Debug.Log("3键: 添加工具");
        Debug.Log("4键: 添加任务物品");
        Debug.Log("H键: 显示背包信息");
        Debug.Log("S键: 搜索物品");
        Debug.Log("V键: 显示交易价值范围物品");
        Debug.Log("B键: 显示最佳交易建议");
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    /// <summary>
    /// 处理键盘输入
    /// </summary>
    private void HandleInput()
    {
        // A键: 添加测试物品 (旧书)
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddTestItem();
        }
        
        // R键: 添加随机可交易物品
        if (Input.GetKeyDown(KeyCode.R))
        {
            AddRandomTradeableItem();
        }
        
        // T键: 在当铺交易当前物品
        if (Input.GetKeyDown(KeyCode.T))
        {
            TradeCurrentItem();
        }
        
        // P键: 显示交易预览
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShowTradePreview();
        }
        
        // U键: 使用当前物品 (药水)
        if (Input.GetKeyDown(KeyCode.U))
        {
            UseCurrentItem();
        }
        
        // D键: 丢弃当前物品
        if (Input.GetKeyDown(KeyCode.D))
        {
            DropCurrentItem();
        }
        
        // C键: 清空背包
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearInventory();
        }
        
        // 1键: 添加生活物品
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddDailyItem();
        }
        
        // 2键: 添加材料
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddMaterial();
        }
        
        // 3键: 添加工具
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddTool();
        }
        
        // 4键: 添加任务物品
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddQuestItem();
        }
        
        // H键: 显示背包信息
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowInventoryInfo();
        }
        
        // S键: 搜索物品
        if (Input.GetKeyDown(KeyCode.S))
        {
            SearchItems();
        }
        
        // V键: 显示交易价值范围物品
        if (Input.GetKeyDown(KeyCode.V))
        {
            ShowItemsByTradeValue();
        }
        
        // B键: 显示最佳交易建议
        if (Input.GetKeyDown(KeyCode.B))
        {
            ShowBestTradeAdvice();
        }
    }
    
    /// <summary>
    /// 添加测试物品
    /// </summary>
    private void AddTestItem()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到Inventory组件");
            return;
        }
        
        bool success = Inventory.Instance.AddItem("旧书");
        
        if (showDebugInfo)
        {
            if (success)
            {
                Debug.Log("[InventoryExample] 添加测试物品成功: 旧书");
            }
            else
            {
                Debug.LogWarning("[InventoryExample] 添加测试物品失败");
            }
        }
    }
    
    /// <summary>
    /// 添加随机可交易物品
    /// </summary>
    private void AddRandomTradeableItem()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到Inventory组件");
            return;
        }
        
        Item randomItem = ItemManager.Instance?.GetRandomTradeableItem();
        if (randomItem == null)
        {
            Debug.LogWarning("[InventoryExample] 没有可交易物品");
            return;
        }
        
        bool success = Inventory.Instance.AddItem(randomItem);
        
        if (showDebugInfo)
        {
            if (success)
            {
                Debug.Log($"[InventoryExample] 添加随机物品成功: {randomItem.name}");
            }
            else
            {
                Debug.LogWarning("[InventoryExample] 添加随机物品失败");
            }
        }
    }
    
    /// <summary>
    /// 交易当前物品
    /// </summary>
    private void TradeCurrentItem()
    {
        if (PawnShop.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到PawnShop组件");
            return;
        }
        
        bool success = PawnShop.Instance.TradeCurrentItem();
        
        if (showDebugInfo)
        {
            if (success)
            {
                Debug.Log("[InventoryExample] 交易成功");
            }
            else
            {
                Debug.LogWarning("[InventoryExample] 交易失败");
            }
        }
    }
    
    /// <summary>
    /// 显示交易预览
    /// </summary>
    private void ShowTradePreview()
    {
        if (PawnShop.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到PawnShop组件");
            return;
        }
        
        string preview = PawnShop.Instance.PreviewTrade();
        Debug.Log($"[InventoryExample] {preview}");
    }
    
    /// <summary>
    /// 使用当前物品
    /// </summary>
    private void UseCurrentItem()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到Inventory组件");
            return;
        }
        
        bool success = Inventory.Instance.UseCurrentItem();
        
        if (showDebugInfo)
        {
            if (success)
            {
                Debug.Log("[InventoryExample] 使用物品成功");
            }
            else
            {
                Debug.LogWarning("[InventoryExample] 使用物品失败");
            }
        }
    }
    
    /// <summary>
    /// 丢弃当前物品
    /// </summary>
    private void DropCurrentItem()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到Inventory组件");
            return;
        }
        
        Item droppedItem = Inventory.Instance.RemoveCurrentItem();
        
        if (showDebugInfo)
        {
            if (droppedItem != null)
            {
                Debug.Log($"[InventoryExample] 丢弃物品: {droppedItem.name}");
            }
            else
            {
                Debug.LogWarning("[InventoryExample] 背包为空，无法丢弃物品");
            }
        }
    }
    
    /// <summary>
    /// 清空背包
    /// </summary>
    private void ClearInventory()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到Inventory组件");
            return;
        }
        
        Inventory.Instance.ClearInventory();
        
        if (showDebugInfo)
        {
            Debug.Log("[InventoryExample] 背包已清空");
        }
    }
    
    /// <summary>
    /// 添加生活物品
    /// </summary>
    private void AddDailyItem()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到Inventory组件");
            return;
        }
        
        string[] dailyItems = { "古董花瓶", "银器", "珠宝", "古董钟表", "名画" };
        string randomItem = dailyItems[Random.Range(0, dailyItems.Length)];
        
        bool success = Inventory.Instance.AddItem(randomItem);
        
        if (showDebugInfo)
        {
            if (success)
            {
                Debug.Log($"[InventoryExample] 添加生活物品: {randomItem}");
            }
            else
            {
                Debug.LogWarning("[InventoryExample] 添加生活物品失败");
            }
        }
    }
    
    /// <summary>
    /// 添加材料
    /// </summary>
    private void AddMaterial()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到Inventory组件");
            return;
        }
        
        string[] materials = { "木材", "石头", "铁矿石", "金矿石", "草药" };
        string randomItem = materials[Random.Range(0, materials.Length)];
        
        bool success = Inventory.Instance.AddItem(randomItem);
        
        if (showDebugInfo)
        {
            if (success)
            {
                Debug.Log($"[InventoryExample] 添加材料: {randomItem}");
            }
            else
            {
                Debug.LogWarning("[InventoryExample] 添加材料失败");
            }
        }
    }
    
    /// <summary>
    /// 添加工具
    /// </summary>
    private void AddTool()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到Inventory组件");
            return;
        }
        
        string[] tools = { "铁镐", "铁斧" };
        string randomItem = tools[Random.Range(0, tools.Length)];
        
        bool success = Inventory.Instance.AddItem(randomItem);
        
        if (showDebugInfo)
        {
            if (success)
            {
                Debug.Log($"[InventoryExample] 添加工具: {randomItem}");
            }
            else
            {
                Debug.LogWarning("[InventoryExample] 添加工具失败");
            }
        }
    }
    
    /// <summary>
    /// 添加任务物品
    /// </summary>
    private void AddQuestItem()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到Inventory组件");
            return;
        }
        
        string[] questItems = { "神秘钥匙", "古老地图" };
        string randomItem = questItems[Random.Range(0, questItems.Length)];
        
        bool success = Inventory.Instance.AddItem(randomItem);
        
        if (showDebugInfo)
        {
            if (success)
            {
                Debug.Log($"[InventoryExample] 添加任务物品: {randomItem}");
            }
            else
            {
                Debug.LogWarning("[InventoryExample] 添加任务物品失败");
            }
        }
    }
    
    /// <summary>
    /// 显示背包信息
    /// </summary>
    private void ShowInventoryInfo()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到Inventory组件");
            return;
        }
        
        string info = Inventory.Instance.GetCurrentItemInfo();
        Debug.Log($"[InventoryExample] {info}");
    }
    
    /// <summary>
    /// 搜索物品
    /// </summary>
    private void SearchItems()
    {
        if (ItemManager.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到ItemManager组件");
            return;
        }
        
        string[] searchTerms = { "古董", "药水", "矿石", "工具" };
        string searchTerm = searchTerms[Random.Range(0, searchTerms.Length)];
        
        var results = ItemManager.Instance.SearchItems(searchTerm);
        
        if (showDebugInfo)
        {
            Debug.Log($"[InventoryExample] 搜索 '{searchTerm}' 的结果:");
            foreach (var item in results)
            {
                Debug.Log($"  - {item.name} ({item.type})");
            }
        }
    }
    
    /// <summary>
    /// 显示交易价值范围物品
    /// </summary>
    private void ShowItemsByTradeValue()
    {
        if (ItemManager.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到ItemManager组件");
            return;
        }
        
        int minValue = 50;
        int maxValue = 150;
        
        var items = ItemManager.Instance.GetItemsByTradeValue(minValue, maxValue);
        
        if (showDebugInfo)
        {
            Debug.Log($"[InventoryExample] 交易价值 {minValue}-{maxValue} 的物品:");
            foreach (var item in items)
            {
                Debug.Log($"  - {item.name} (价值: {item.tradeValue})");
            }
        }
    }
    
    /// <summary>
    /// 显示最佳交易建议
    /// </summary>
    private void ShowBestTradeAdvice()
    {
        if (PawnShop.Instance == null)
        {
            Debug.LogError("[InventoryExample] 未找到PawnShop组件");
            return;
        }
        
        string advice = PawnShop.Instance.GetBestTradeAdvice();
        Debug.Log($"[InventoryExample] {advice}");
    }
} 