using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 物品管理器
/// 管理所有物品的数据库，提供物品查询、搜索功能
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
        // 生活物品（用于交易）
        AddItem(Item.CreateDailyItem("旧书", "一本破旧的书籍，可能有些价值", 15));
        AddItem(Item.CreateDailyItem("古董花瓶", "精美的古董花瓶，价值不菲", 50));
        AddItem(Item.CreateDailyItem("银器", "纯银制作的器皿", 80));
        AddItem(Item.CreateDailyItem("珠宝", "闪闪发光的珠宝首饰", 120));
        AddItem(Item.CreateDailyItem("古董钟表", "古老的机械钟表", 200));
        AddItem(Item.CreateDailyItem("名画", "著名画家的作品", 300));
        AddItem(Item.CreateDailyItem("古董家具", "精美的古董家具", 150));
        AddItem(Item.CreateDailyItem("瓷器", "精美的瓷器制品", 100));
        AddItem(Item.CreateDailyItem("铜器", "古老的铜制器皿", 40));
        AddItem(Item.CreateDailyItem("布料", "上等的丝绸布料", 25));
        
        // 药水（恢复生命值）
        AddItem(Item.CreatePotion("小生命药水", "恢复少量生命值的药水", 30));
        AddItem(Item.CreatePotion("生命药水", "恢复中等生命值的药水", 60));
        AddItem(Item.CreatePotion("大生命药水", "恢复大量生命值的药水", 120));
        AddItem(Item.CreatePotion("超级生命药水", "恢复超量生命值的药水", 250));
        AddItem(Item.CreatePotion("神秘药水", "神秘的力量药水，恢复大量生命值", 500));
        
        // 材料（可交易）
        AddItem(Item.CreateMaterial("木材", "普通的木材", 5));
        AddItem(Item.CreateMaterial("石头", "坚硬的石头", 8));
        AddItem(Item.CreateMaterial("铁矿石", "含有铁的矿石", 15));
        AddItem(Item.CreateMaterial("金矿石", "含有金的矿石", 30));
        AddItem(Item.CreateMaterial("草药", "具有药用价值的草药", 10));
        
        // 工具（可交易）
        AddItem(Item.CreateTool("铁镐", "用于挖掘的工具", 25));
        AddItem(Item.CreateTool("铁斧", "用于砍伐的工具", 20));
        
        // 任务物品（不可交易）
        AddItem(Item.CreateQuestItem("神秘钥匙", "一把神秘的钥匙，似乎能开启什么"));
        AddItem(Item.CreateQuestItem("古老地图", "一张古老的地图，记载着宝藏的位置"));
        
        if (showDebugInfo)
        {
            Debug.Log($"[ItemManager] 物品数据库初始化完成，共添加 {itemDatabase.Count} 个物品");
        }
    }
    
    /// <summary>
    /// 添加物品到数据库
    /// </summary>
    /// <param name="item">物品对象</param>
    public void AddItem(Item item)
    {
        if (item == null || string.IsNullOrEmpty(item.name))
        {
            Debug.LogError("[ItemManager] 尝试添加无效物品");
            return;
        }
        
        if (itemDatabase.ContainsKey(item.name))
        {
            Debug.LogWarning($"[ItemManager] 物品 {item.name} 已存在，将被覆盖");
        }
        
        itemDatabase[item.name] = item;
        
        if (showDebugInfo)
        {
            Debug.Log($"[ItemManager] 添加物品: {item.name} (类型: {item.type})");
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
        return itemDatabase.Values.ToList();
    }
    
    /// <summary>
    /// 根据类型获取物品
    /// </summary>
    /// <param name="type">物品类型</param>
    /// <returns>指定类型的物品列表</returns>
    public List<Item> GetItemsByType(ItemType type)
    {
        return itemDatabase.Values.Where(item => item.type == type).ToList();
    }
    
    /// <summary>
    /// 获取可交易物品
    /// </summary>
    /// <returns>可交易物品列表</returns>
    public List<Item> GetTradeableItems()
    {
        return itemDatabase.Values.Where(item => item.IsTradeable()).ToList();
    }
    
    /// <summary>
    /// 获取药水物品
    /// </summary>
    /// <returns>药水物品列表</returns>
    public List<Item> GetPotions()
    {
        return GetItemsByType(ItemType.Potion);
    }
    
    /// <summary>
    /// 根据交易价值范围搜索物品
    /// </summary>
    /// <param name="minValue">最小交易价值</param>
    /// <param name="maxValue">最大交易价值</param>
    /// <returns>符合条件的物品列表</returns>
    public List<Item> GetItemsByTradeValue(int minValue, int maxValue)
    {
        return itemDatabase.Values
            .Where(item => item.tradeValue >= minValue && item.tradeValue <= maxValue)
            .OrderBy(item => item.tradeValue)
            .ToList();
    }
    
    /// <summary>
    /// 搜索物品（按名称模糊搜索）
    /// </summary>
    /// <param name="searchTerm">搜索关键词</param>
    /// <returns>匹配的物品列表</returns>
    public List<Item> SearchItems(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return new List<Item>();
        }
        
        string lowerSearchTerm = searchTerm.ToLower();
        return itemDatabase.Values
            .Where(item => item.name.ToLower().Contains(lowerSearchTerm) || 
                          item.description.ToLower().Contains(lowerSearchTerm))
            .ToList();
    }
    
    /// <summary>
    /// 获取最佳交易建议
    /// </summary>
    /// <param name="targetValue">目标交易价值</param>
    /// <returns>最佳匹配的物品</returns>
    public Item GetBestTradeItem(int targetValue)
    {
        var tradeableItems = GetTradeableItems();
        
        if (tradeableItems.Count == 0)
        {
            return null;
        }
        
        // 找到最接近目标价值的物品
        Item bestItem = tradeableItems[0];
        int minDifference = Mathf.Abs(bestItem.tradeValue - targetValue);
        
        foreach (var item in tradeableItems)
        {
            int difference = Mathf.Abs(item.tradeValue - targetValue);
            if (difference < minDifference)
            {
                minDifference = difference;
                bestItem = item;
            }
        }
        
        return bestItem;
    }
    
    /// <summary>
    /// 获取随机可交易物品
    /// </summary>
    /// <returns>随机可交易物品</returns>
    public Item GetRandomTradeableItem()
    {
        var tradeableItems = GetTradeableItems();
        
        if (tradeableItems.Count == 0)
        {
            return null;
        }
        
        int randomIndex = Random.Range(0, tradeableItems.Count);
        return tradeableItems[randomIndex];
    }
    
    /// <summary>
    /// 显示物品数据库信息（调试用）
    /// </summary>
    public void ShowDatabaseInfo()
    {
        string info = $"[ItemManager] 物品数据库信息:\n";
        info += $"总物品数: {itemDatabase.Count}\n";
        
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            var itemsOfType = GetItemsByType(type);
            info += $"{type}: {itemsOfType.Count} 个\n";
        }
        
        var tradeableItems = GetTradeableItems();
        info += $"可交易物品: {tradeableItems.Count} 个\n";
        
        var potions = GetPotions();
        info += $"药水: {potions.Count} 个";
        
        Debug.Log(info);
    }
    
    /// <summary>
    /// 获取物品数量
    /// </summary>
    /// <returns>物品总数</returns>
    public int GetItemCount()
    {
        return itemDatabase.Count;
    }
} 