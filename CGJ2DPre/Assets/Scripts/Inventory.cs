using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 背包系统 - 单槽位设计
/// 管理玩家的物品存储、使用和交易
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("背包设置")]
    [SerializeField] private int maxSlots = 1;  // 背包槽位数（单槽位设计）
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    [Header("事件")]
    public UnityEvent<Item> OnItemAdded;        // 物品添加事件
    public UnityEvent<Item> OnItemRemoved;      // 物品移除事件
    public UnityEvent<Item> OnItemUsed;         // 物品使用事件
    public UnityEvent OnInventoryFull;          // 背包满事件
    public UnityEvent OnInventoryEmpty;         // 背包空事件
    
    // 单例模式
    public static Inventory Instance { get; private set; }
    
    // 背包数据
    private List<Item> items = new List<Item>();
    private Health healthSystem;
    
    // 属性
    public int CurrentItemCount => items.Count;
    public int MaxSlots => maxSlots;
    public bool IsFull => items.Count >= maxSlots;
    public bool IsEmpty => items.Count == 0;
    public Item CurrentItem => items.Count > 0 ? items[0] : null;
    
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
        
        // 查找生命值系统
        healthSystem = FindObjectOfType<Health>();
        if (healthSystem == null)
        {
            Debug.LogWarning("[Inventory] 未找到Health组件，药水使用功能将不可用");
        }
    }
    
    private void Start()
    {
        Debug.Log($"[Inventory] 背包系统初始化完成 - 最大槽位: {maxSlots}");
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
            OnInventoryFull?.Invoke();
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
        
        if (showDebugInfo)
        {
            Debug.Log($"[Inventory] 添加物品: {item.name} (类型: {item.type})");
        }
        
        OnItemAdded?.Invoke(item);
        return true;
    }
    
    /// <summary>
    /// 添加物品到背包（重载方法）
    /// </summary>
    /// <param name="item">物品对象</param>
    /// <returns>是否添加成功</returns>
    public bool AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("[Inventory] 尝试添加空物品");
            return false;
        }
        
        return AddItem(item.name);
    }
    
    /// <summary>
    /// 移除当前物品
    /// </summary>
    /// <returns>被移除的物品</returns>
    public Item RemoveCurrentItem()
    {
        if (IsEmpty)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[Inventory] 背包为空，无法移除物品");
            }
            return null;
        }
        
        Item removedItem = items[0];
        items.RemoveAt(0);
        
        if (showDebugInfo)
        {
            Debug.Log($"[Inventory] 移除物品: {removedItem.name}");
        }
        
        OnItemRemoved?.Invoke(removedItem);
        
        if (IsEmpty)
        {
            OnInventoryEmpty?.Invoke();
        }
        
        return removedItem;
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
                
                if (showDebugInfo)
                {
                    Debug.Log($"[Inventory] 移除物品: {removedItem.name}");
                }
                
                OnItemRemoved?.Invoke(removedItem);
                
                if (IsEmpty)
                {
                    OnInventoryEmpty?.Invoke();
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
    /// 使用当前物品
    /// </summary>
    /// <returns>是否使用成功</returns>
    public bool UseCurrentItem()
    {
        if (IsEmpty)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[Inventory] 背包为空，无法使用物品");
            }
            return false;
        }
        
        Item currentItem = items[0];
        
        // 检查是否为药水
        if (currentItem.type == ItemType.Potion)
        {
            return UsePotion(currentItem);
        }
        else
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"[Inventory] 物品 {currentItem.name} 不是药水，无法使用");
            }
            return false;
        }
    }
    
    /// <summary>
    /// 使用药水
    /// </summary>
    /// <param name="potion">药水物品</param>
    /// <returns>是否使用成功</returns>
    private bool UsePotion(Item potion)
    {
        if (healthSystem == null)
        {
            Debug.LogError("[Inventory] 未找到Health组件，无法使用药水");
            return false;
        }
        
        // 恢复生命值
        int healAmount = potion.healAmount;
        healthSystem.Heal(healAmount);
        
        if (showDebugInfo)
        {
            Debug.Log($"[Inventory] 使用药水: {potion.name}，恢复生命值: {healAmount}");
        }
        
        // 移除药水
        items.RemoveAt(0);
        
        OnItemUsed?.Invoke(potion);
        OnItemRemoved?.Invoke(potion);
        
        if (IsEmpty)
        {
            OnInventoryEmpty?.Invoke();
        }
        
        return true;
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
        
        if (showDebugInfo)
        {
            Debug.Log($"[Inventory] 清空背包，移除了 {itemCount} 个物品");
        }
        
        OnInventoryEmpty?.Invoke();
    }
    
    /// <summary>
    /// 获取当前物品信息
    /// </summary>
    /// <returns>当前物品信息字符串</returns>
    public string GetCurrentItemInfo()
    {
        if (IsEmpty)
        {
            return "背包为空";
        }
        
        Item item = items[0];
        string info = $"当前物品: {item.name}\n";
        info += $"类型: {item.type}\n";
        info += $"描述: {item.description}\n";
        
        if (item.type == ItemType.Potion)
        {
            info += $"恢复生命值: {item.healAmount}\n";
        }
        else if (item.tradeValue > 0)
        {
            info += $"交易价值: {item.tradeValue}\n";
        }
        
        return info;
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
    /// 检查当前物品是否为可交易物品
    /// </summary>
    /// <returns>是否为可交易物品</returns>
    public bool IsCurrentItemTradeable()
    {
        if (IsEmpty) return false;
        
        Item currentItem = items[0];
        return currentItem.tradeValue > 0 && currentItem.type != ItemType.Quest;
    }
    
    /// <summary>
    /// 获取当前物品的交易价值
    /// </summary>
    /// <returns>交易价值，如果不是可交易物品则返回0</returns>
    public int GetCurrentItemTradeValue()
    {
        if (IsEmpty) return 0;
        
        Item currentItem = items[0];
        return currentItem.tradeValue;
    }
    
    /// <summary>
    /// 显示背包信息（调试用）
    /// </summary>
    public void ShowInventoryInfo()
    {
        string info = $"[Inventory] 背包信息:\n";
        info += $"槽位数: {CurrentItemCount}/{MaxSlots}\n";
        info += $"状态: {(IsFull ? "已满" : IsEmpty ? "为空" : "有物品")}\n";
        
        if (!IsEmpty)
        {
            info += $"当前物品: {CurrentItem.name} (类型: {CurrentItem.type})";
        }
        
        Debug.Log(info);
    }
    
    /// <summary>
    /// 设置背包槽位数
    /// </summary>
    /// <param name="newMaxSlots">新的最大槽位数</param>
    public void SetMaxSlots(int newMaxSlots)
    {
        if (newMaxSlots < 1)
        {
            Debug.LogError("[Inventory] 槽位数不能小于1");
            return;
        }
        
        maxSlots = newMaxSlots;
        
        // 如果当前物品数量超过新的槽位数，移除多余的物品
        while (items.Count > maxSlots)
        {
            Item removedItem = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            OnItemRemoved?.Invoke(removedItem);
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[Inventory] 背包槽位数设置为: {maxSlots}");
        }
    }
} 