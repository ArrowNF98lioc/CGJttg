using UnityEngine;

/// <summary>
/// 物品类型枚举
/// </summary>
public enum ItemType
{
    DailyItem,  // 生活物品（用于交易）
    Potion,     // 药水（恢复生命值）
    Material,   // 材料（可交易）
    Tool,       // 工具（可交易）
    Quest       // 任务物品（不可交易）
}

/// <summary>
/// 物品数据类
/// 定义物品的基本属性和行为
/// </summary>
[System.Serializable]
public class Item
{
    [Header("基本信息")]
    public string name;           // 物品名称
    public string description;    // 物品描述
    public ItemType type;         // 物品类型
    public Sprite icon;           // 物品图标
    
    [Header("交易属性")]
    public int tradeValue;        // 交易价值（用于当铺交易）
    
    [Header("药水属性")]
    public int healAmount;        // 恢复生命值（仅药水类型有效）
    
    [Header("其他属性")]
    public bool isStackable;      // 是否可堆叠
    public int maxStackSize;      // 最大堆叠数量
    
    /// <summary>
    /// 创建生活物品
    /// </summary>
    public static Item CreateDailyItem(string name, string description, int tradeValue, Sprite icon = null)
    {
        return new Item
        {
            name = name,
            description = description,
            type = ItemType.DailyItem,
            tradeValue = tradeValue,
            icon = icon,
            isStackable = false,
            maxStackSize = 1
        };
    }
    
    /// <summary>
    /// 创建药水
    /// </summary>
    public static Item CreatePotion(string name, string description, int healAmount, Sprite icon = null)
    {
        return new Item
        {
            name = name,
            description = description,
            type = ItemType.Potion,
            healAmount = healAmount,
            icon = icon,
            isStackable = false,
            maxStackSize = 1
        };
    }
    
    /// <summary>
    /// 创建材料
    /// </summary>
    public static Item CreateMaterial(string name, string description, int tradeValue, Sprite icon = null)
    {
        return new Item
        {
            name = name,
            description = description,
            type = ItemType.Material,
            tradeValue = tradeValue,
            icon = icon,
            isStackable = true,
            maxStackSize = 99
        };
    }
    
    /// <summary>
    /// 创建工具
    /// </summary>
    public static Item CreateTool(string name, string description, int tradeValue, Sprite icon = null)
    {
        return new Item
        {
            name = name,
            description = description,
            type = ItemType.Tool,
            tradeValue = tradeValue,
            icon = icon,
            isStackable = false,
            maxStackSize = 1
        };
    }
    
    /// <summary>
    /// 创建任务物品
    /// </summary>
    public static Item CreateQuestItem(string name, string description, Sprite icon = null)
    {
        return new Item
        {
            name = name,
            description = description,
            type = ItemType.Quest,
            tradeValue = 0,
            icon = icon,
            isStackable = false,
            maxStackSize = 1
        };
    }
    
    /// <summary>
    /// 检查物品是否可交易
    /// </summary>
    public bool IsTradeable()
    {
        return tradeValue > 0 && type != ItemType.Quest;
    }
    
    /// <summary>
    /// 检查物品是否为药水
    /// </summary>
    public bool IsPotion()
    {
        return type == ItemType.Potion;
    }
    
    /// <summary>
    /// 获取物品信息字符串
    /// </summary>
    public string GetInfoString()
    {
        string info = $"名称: {name}\n";
        info += $"类型: {type}\n";
        info += $"描述: {description}\n";
        
        if (tradeValue > 0)
        {
            info += $"交易价值: {tradeValue}\n";
        }
        
        if (type == ItemType.Potion)
        {
            info += $"恢复生命值: {healAmount}\n";
        }
        
        return info;
    }
} 