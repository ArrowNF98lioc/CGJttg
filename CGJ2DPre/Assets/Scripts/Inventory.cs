using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 简化的背包系统
/// 只负责存储拾取的物品
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("背包设置")]
    [SerializeField] private int maxSlots = 1000;  // 背包槽位数
    
    [Header("背包UI设置")]
    [SerializeField] private GameObject inventoryPanel;        // 背包面板
    [SerializeField] private Transform slotsParent;            // 背包槽位父对象
    [SerializeField] private GameObject slotPrefab;            // 背包槽位预制体
    [SerializeField] private bool autoCreateSlots = true;      // 是否自动创建槽位
    
    [Header("物品图片设置")]
    [SerializeField] private Sprite defaultSlotSprite;         // 默认槽位图片
    [SerializeField] private Sprite emptySlotSprite;           // 空槽位图片
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    // 单例模式
    public static Inventory Instance { get; private set; }
    
    // 背包数据
    private List<Item> items = new List<Item>();
    
    // UI相关
    private List<InventorySlot> slots = new List<InventorySlot>();
    private bool isInventoryOpen = false;
    
    // 属性
    public int CurrentItemCount => items.Count;
    public int MaxSlots => maxSlots;
    public bool IsFull => items.Count >= maxSlots;
    public bool IsEmpty => items.Count == 0;
    public bool IsInventoryOpen => isInventoryOpen;
    
    private void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        Debug.Log($"[Inventory] 背包系统初始化完成 - 最大槽位: {maxSlots}");
        
        // 初始化背包UI
        InitializeInventoryUI();
        
        // 同步GameDataManager数据
        SyncWithGameDataManager();
    }
    
    /// <summary>
    /// 初始化背包UI
    /// </summary>
    private void InitializeInventoryUI()
    {
        if (inventoryPanel != null)
        {
            // 默认隐藏背包面板
            inventoryPanel.SetActive(false);
        }
        
        if (autoCreateSlots && slotsParent != null && slotPrefab != null)
        {
            CreateInventorySlots();
        }
        
        // 更新背包显示
        UpdateInventoryDisplay();
    }
    
    /// <summary>
    /// 创建背包槽位
    /// </summary>
    private void CreateInventorySlots()
    {
        // 清除现有槽位
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();
        
        // 创建新槽位
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsParent);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            
            if (slot == null)
            {
                slot = slotObj.AddComponent<InventorySlot>();
            }
            
            slot.InitializeSlot(i, defaultSlotSprite, emptySlotSprite);
            slots.Add(slot);
        }
        
        Debug.Log($"[Inventory] 创建了 {maxSlots} 个背包槽位");
    }
    
    /// <summary>
    /// 更新背包显示
    /// </summary>
    public void UpdateInventoryDisplay()
    {
        if (slots.Count == 0)
        {
            Debug.LogWarning("[Inventory] 背包槽位未初始化");
            return;
        }
        
        // 更新所有槽位
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
            {
                // 有物品的槽位
                slots[i].SetItem(items[i]);
            }
            else
            {
                // 空槽位
                slots[i].ClearSlot();
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[Inventory] 更新背包显示: {items.Count}/{maxSlots} 个物品");
        }
    }
    
    /// <summary>
    /// 切换背包显示状态
    /// </summary>
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isInventoryOpen);
        }
        
        if (isInventoryOpen)
        {
            UpdateInventoryDisplay();
        }
        
        Debug.Log($"[Inventory] 背包状态: {(isInventoryOpen ? "打开" : "关闭")}");
    }
    
    /// <summary>
    /// 打开背包
    /// </summary>
    public void OpenInventory()
    {
        if (!isInventoryOpen)
        {
            ToggleInventory();
        }
    }
    
    /// <summary>
    /// 关闭背包
    /// </summary>
    public void CloseInventory()
    {
        if (isInventoryOpen)
        {
            ToggleInventory();
        }
    }
    
    /// <summary>
    /// 与GameDataManager同步数据
    /// </summary>
    private void SyncWithGameDataManager()
    {
        if (GameDataManager.Instance != null)
        {
            if (showDebugInfo)
            {
                Debug.Log($"[Inventory] 开始从GameDataManager同步数据: 当前背包物品数={CurrentItemCount}/{MaxSlots}");
                Debug.Log($"[Inventory] GameDataManager中的物品数量: {GameDataManager.Instance.collectedItems.Count}");
            }
            
            int successCount = 0;
            int failCount = 0;
            
            // 从GameDataManager加载已收集的物品
            foreach (string itemName in GameDataManager.Instance.collectedItems)
            {
                if (!HasItem(itemName))
                {
                    bool success = AddItem(itemName);
                    if (success)
                    {
                        successCount++;
                        if (showDebugInfo)
                        {
                            Debug.Log($"[Inventory] 成功从GameDataManager添加物品: {itemName}");
                        }
                    }
                    else
                    {
                        failCount++;
                        Debug.LogWarning($"[Inventory] 无法从GameDataManager添加物品: {itemName} - 背包可能已满或物品不存在");
                    }
                }
                else
                {
                    if (showDebugInfo)
                    {
                        Debug.Log($"[Inventory] 物品已存在，跳过: {itemName}");
                    }
                }
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[Inventory] 从GameDataManager同步完成: 成功={successCount}, 失败={failCount}, 最终背包物品数={CurrentItemCount}");
            }
        }
        else
        {
            Debug.LogWarning("[Inventory] GameDataManager实例未找到，无法同步数据");
        }
    }
    
    /// <summary>
    /// 同步数据到GameDataManager
    /// </summary>
    public void SyncToGameDataManager()
    {
        if (GameDataManager.Instance != null)
        {
            // 同步已收集的物品
            GameDataManager.Instance.collectedItems.Clear();
            foreach (var item in items)
            {
                GameDataManager.Instance.AddCollectedItem(item.name);
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[Inventory] 同步了 {items.Count} 个物品到GameDataManager");
            }
        }
    }
    
    /// <summary>
    /// 添加物品到背包
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>是否添加成功</returns>
    public bool AddItem(string itemName)
    {
        if (IsFull)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"[Inventory] 背包已满，无法添加物品: {itemName}");
            }
            return false;
        }
        
        // 从物品管理器获取物品信息
        Item item = ItemManager.Instance?.GetItem(itemName);
        if (item == null)
        {
            Debug.LogError($"[Inventory] 物品不存在: {itemName}");
            return false;
        }
        
        // 添加物品
        items.Add(item);
        
        // 更新背包显示
        UpdateInventoryDisplay();
        
        // 同步到GameDataManager
        SyncToGameDataManager();
        
        if (showDebugInfo)
        {
            Debug.Log($"[Inventory] 添加物品: {item.name} (健康值: {item.health}, 有生命: {item.hasLife})");
        }
        
        return true;
    }
    
    /// <summary>
    /// 移除指定物品
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>是否移除成功</returns>
    public bool RemoveItem(string itemName)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name == itemName)
            {
                Item removedItem = items[i];
                items.RemoveAt(i);
                
                // 更新背包显示
                UpdateInventoryDisplay();
                
                // 同步到GameDataManager
                SyncToGameDataManager();
                
                if (showDebugInfo)
                {
                    Debug.Log($"[Inventory] 移除物品: {removedItem.name}");
                }
                
                return true;
            }
        }
        
        if (showDebugInfo)
        {
            Debug.LogWarning($"[Inventory] 背包中没有物品: {itemName}");
        }
        return false;
    }
    
    /// <summary>
    /// 清空背包
    /// </summary>
    public void ClearInventory()
    {
        if (IsEmpty)
        {
            if (showDebugInfo)
            {
                Debug.Log("[Inventory] 背包已经为空");
            }
            return;
        }
        
        int itemCount = items.Count;
        items.Clear();
        
        // 更新背包显示
        UpdateInventoryDisplay();
        
        // 同步到GameDataManager
        SyncToGameDataManager();
        
        if (showDebugInfo)
        {
            Debug.Log($"[Inventory] 清空背包，移除了 {itemCount} 个物品");
        }
    }
    
    /// <summary>
    /// 检查是否有指定物品
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>是否拥有该物品</returns>
    public bool HasItem(string itemName)
    {
        return items.Exists(item => item.name == itemName);
    }
    
    /// <summary>
    /// 获取指定槽位的物品
    /// </summary>
    /// <param name="slotIndex">槽位索引</param>
    /// <returns>物品对象，如果槽位为空则返回null</returns>
    public Item GetItemAtSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < items.Count)
        {
            return items[slotIndex];
        }
        return null;
    }
    
    /// <summary>
    /// 获取背包信息
    /// </summary>
    /// <returns>背包信息字符串</returns>
    public string GetInventoryInfo()
    {
        string info = $"背包信息: {CurrentItemCount}/{MaxSlots}\n";
        info += $"背包状态: {(isInventoryOpen ? "打开" : "关闭")}\n";
        
        if (IsEmpty)
        {
            info += "背包为空";
        }
        else
        {
            info += "物品列表:\n";
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                info += $"{i + 1}. {item.name}: 健康值={item.health}, 有生命={item.hasLife}\n";
            }
        }
        
        return info;
    }
    
    /// <summary>
    /// 显示背包信息（调试用）
    /// </summary>
    public void ShowInventoryInfo()
    {
        Debug.Log(GetInventoryInfo());
    }
    
    /// <summary>
    /// 测试背包同步功能
    /// </summary>
    [ContextMenu("测试背包同步")]
    public void TestInventorySync()
    {
        Debug.Log("=== 开始测试背包同步功能 ===");
        
        // 显示当前状态
        DiagnoseInventoryIssues();
        
        // 测试添加物品
        Debug.Log("测试添加物品...");
        bool success1 = AddItem("项链");
        bool success2 = AddItem("茶壶");
        bool success3 = AddItem("植物");
        
        Debug.Log($"添加结果: 项链={success1}, 茶壶={success2}, 植物={success3}");
        
        // 显示同步后的状态
        Debug.Log("同步后的状态:");
        DiagnoseInventoryIssues();
        
        Debug.Log("=== 背包同步测试完成 ===");
    }
    
    /// <summary>
    /// 诊断背包状态问题
    /// </summary>
    public void DiagnoseInventoryIssues()
    {
        Debug.Log("=== 背包状态诊断 ===");
        Debug.Log($"当前物品数量: {CurrentItemCount}");
        Debug.Log($"最大槽位数: {MaxSlots}");
        Debug.Log($"背包是否已满: {IsFull}");
        Debug.Log($"背包是否为空: {IsEmpty}");
        
        if (IsFull)
        {
            Debug.LogWarning("背包已满！这可能导致无法添加新物品。");
            Debug.LogWarning("建议检查是否有重复物品或清理背包。");
        }
        
        if (CurrentItemCount > 0)
        {
            Debug.Log("当前背包中的物品:");
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                Debug.Log($"  {i + 1}. {item.name} (健康值: {item.health}, 有生命: {item.hasLife})");
            }
        }
        
        // 检查GameDataManager中的物品
        if (GameDataManager.Instance != null)
        {
            Debug.Log($"GameDataManager中的物品数量: {GameDataManager.Instance.collectedItems.Count}");
            if (GameDataManager.Instance.collectedItems.Count > 0)
            {
                Debug.Log("GameDataManager中的物品:");
                foreach (string itemName in GameDataManager.Instance.collectedItems)
                {
                    Debug.Log($"  - {itemName}");
                }
            }
        }
        
        Debug.Log("=== 诊断完成 ===");
    }
    
    /// <summary>
    /// 获取背包中所有物品的健康值总和
    /// </summary>
    /// <returns>健康值总和</returns>
    public int GetTotalHealthValue()
    {
        int totalHealth = 0;
        foreach (var item in items)
        {
            totalHealth += item.health;
        }
        return totalHealth;
    }
    
    /// <summary>
    /// 获取背包中所有物品的详细信息
    /// </summary>
    /// <returns>物品详细信息字符串</returns>
    public string GetItemsDetails()
    {
        if (IsEmpty)
        {
            return "背包为空";
        }
        
        string details = $"背包中共有 {items.Count} 个物品:\n";
        int totalHealth = 0;
        
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            details += $"{i + 1}. {item.name}: 健康值={item.health}, 有生命={item.hasLife}\n";
            totalHealth += item.health;
        }
        
        details += $"总健康值: {totalHealth}";
        return details;
    }
    
    /// <summary>
    /// 设置背包UI组件
    /// </summary>
    /// <param name="panel">背包面板</param>
    /// <param name="parent">槽位父对象</param>
    /// <param name="prefab">槽位预制体</param>
    public void SetInventoryUI(GameObject panel, Transform parent, GameObject prefab)
    {
        inventoryPanel = panel;
        slotsParent = parent;
        slotPrefab = prefab;
        
        Debug.Log("[Inventory] 设置背包UI组件完成");
        
        // 重新初始化UI
        InitializeInventoryUI();
    }
} 