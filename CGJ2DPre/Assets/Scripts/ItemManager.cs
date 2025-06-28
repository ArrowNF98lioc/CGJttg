using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 简化的物品管理器
/// 只管理物品的基本属性：健康值、是否有生命、名称
/// </summary>
public class ItemManager : MonoBehaviour
{
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    // 单例模式
    public static ItemManager Instance { get; private set; }
    
    // 物品数据库
    private Dictionary<string, Item> itemDatabase = new Dictionary<string, Item>();
    
    private void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeItemDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        Debug.Log($"[ItemManager] 物品管理器初始化完成 - 物品总数: {itemDatabase.Count}");
    }
    
    /// <summary>
    /// 初始化物品数据库
    /// </summary>
    private void InitializeItemDatabase()
    {
        Debug.Log("[ItemManager] 开始添加物品到数据库...");
        
        // 添加一些示例物品
        AddItem("项链", 10, false);
        AddItem("猫", 40, true);
        AddItem("植物", 20, true);
        AddItem("茶壶", 80, false);
        
        Debug.Log($"[ItemManager] 物品数据库初始化完成，共添加 {itemDatabase.Count} 个物品");
    }
    
    /// <summary>
    /// 添加物品到数据库
    /// </summary>
    /// <param name="name">物品名称</param>
    /// <param name="health">健康值</param>
    /// <param name="hasLife">是否有生命</param>
    public void AddItem(string name, int health, bool hasLife)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("[ItemManager] 物品名称不能为空");
            return;
        }
        
        Item item = new Item
        {
            name = name,
            health = health,
            hasLife = hasLife
        };
        
        if (itemDatabase.ContainsKey(name))
        {
            Debug.LogWarning($"[ItemManager] 物品 {name} 已存在，将被覆盖");
        }
        
        itemDatabase[name] = item;
        
        if (showDebugInfo)
        {
            Debug.Log($"[ItemManager] 添加物品: {name} (健康值: {health}, 有生命: {hasLife})");
        }
    }
    
    /// <summary>
    /// 根据名称获取物品
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>物品对象，如果不存在则返回null</returns>
    public Item GetItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogError("[ItemManager] 物品名称不能为空");
            return null;
        }
        
        if (itemDatabase.TryGetValue(itemName, out Item item))
        {
            return item;
        }
        
        if (showDebugInfo)
        {
            Debug.LogWarning($"[ItemManager] 物品不存在: {itemName}");
        }
        return null;
    }
    
    /// <summary>
    /// 检查物品是否存在
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>是否存在</returns>
    public bool HasItem(string itemName)
    {
        return itemDatabase.ContainsKey(itemName);
    }
    
    /// <summary>
    /// 获取所有物品
    /// </summary>
    /// <returns>所有物品的列表</returns>
    public List<Item> GetAllItems()
    {
        return new List<Item>(itemDatabase.Values);
    }
    
    /// <summary>
    /// 获取物品数量
    /// </summary>
    /// <returns>物品总数</returns>
    public int GetItemCount()
    {
        return itemDatabase.Count;
    }
    
    /// <summary>
    /// 显示物品数据库信息（调试用）
    /// </summary>
    public void ShowDatabaseInfo()
    {
        string info = $"[ItemManager] 物品数据库信息:\n";
        info += $"总物品数: {itemDatabase.Count}\n";
        
        foreach (var item in itemDatabase.Values)
        {
            info += $"- {item.name}: 健康值={item.health}, 有生命={item.hasLife}\n";
        }
        
        Debug.Log(info);
    }
}

/// <summary>
/// 简化的物品数据类
/// 只包含三个基本属性
/// </summary>
[System.Serializable]
public class Item
{
    public string name;       // 物品名称
    public int health;        // 健康值
    public bool hasLife;      // 是否有生命
} 