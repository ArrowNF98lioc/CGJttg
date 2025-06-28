using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 当铺系统
/// 处理物品交易逻辑，根据交易价值选择合适的药水
/// </summary>
public class PawnShop : MonoBehaviour
{
    [Header("交易设置")]
    [SerializeField] private float tradeRatio = 1.0f;    // 交易比率
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    [Header("事件")]
    public UnityEvent<Item, Item, int> OnTradeCompleted;     // 交易完成事件 (物品, 药水, 数量)
    public UnityEvent<string> OnTradeFailed;                 // 交易失败事件
    public UnityEvent<Item, Item, int> OnTradePreview;       // 交易预览事件
    
    // 单例模式
    public static PawnShop Instance { get; private set; }
    
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
        }
    }
    
    private void Start()
    {
        Debug.Log($"[PawnShop] 当铺系统初始化完成 - 交易比率: {tradeRatio}");
    }
    
    /// <summary>
    /// 交易当前物品
    /// </summary>
    /// <returns>是否交易成功</returns>
    public bool TradeCurrentItem()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[PawnShop] 未找到Inventory组件");
            OnTradeFailed?.Invoke("背包系统未找到");
            return false;
        }
        
        if (Inventory.Instance.IsEmpty)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[PawnShop] 背包为空，无法交易");
            }
            OnTradeFailed?.Invoke("背包为空");
            return false;
        }
        
        Item currentItem = Inventory.Instance.CurrentItem;
        
        if (!currentItem.IsTradeable())
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"[PawnShop] 物品 {currentItem.name} 不可交易");
            }
            OnTradeFailed?.Invoke($"{currentItem.name} 不可交易");
            return false;
        }
        
        // 计算交易价值
        int tradeValue = CalculateTradeValue(currentItem);
        
        // 选择合适的药水
        Item bestPotion = SelectBestPotion(tradeValue);
        
        if (bestPotion == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[PawnShop] 没有合适的药水可交易");
            }
            OnTradeFailed?.Invoke("没有合适的药水");
            return false;
        }
        
        // 计算药水数量
        int potionCount = CalculatePotionCount(tradeValue, bestPotion);
        
        // 执行交易
        return ExecuteTrade(currentItem, bestPotion, potionCount);
    }
    
    /// <summary>
    /// 交易指定物品
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>是否交易成功</returns>
    public bool TradeItem(string itemName)
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[PawnShop] 未找到Inventory组件");
            OnTradeFailed?.Invoke("背包系统未找到");
            return false;
        }
        
        if (!Inventory.Instance.HasItem(itemName))
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"[PawnShop] 背包中没有物品: {itemName}");
            }
            OnTradeFailed?.Invoke($"背包中没有 {itemName}");
            return false;
        }
        
        Item item = ItemManager.Instance?.GetItem(itemName);
        if (item == null)
        {
            OnTradeFailed?.Invoke($"物品 {itemName} 不存在");
            return false;
        }
        
        if (!item.IsTradeable())
        {
            OnTradeFailed?.Invoke($"{itemName} 不可交易");
            return false;
        }
        
        // 计算交易价值
        int tradeValue = CalculateTradeValue(item);
        
        // 选择合适的药水
        Item bestPotion = SelectBestPotion(tradeValue);
        
        if (bestPotion == null)
        {
            OnTradeFailed?.Invoke("没有合适的药水");
            return false;
        }
        
        // 计算药水数量
        int potionCount = CalculatePotionCount(tradeValue, bestPotion);
        
        // 执行交易
        return ExecuteTrade(item, bestPotion, potionCount);
    }
    
    /// <summary>
    /// 预览交易
    /// </summary>
    /// <returns>交易预览信息</returns>
    public string PreviewTrade()
    {
        if (Inventory.Instance == null || Inventory.Instance.IsEmpty)
        {
            return "背包为空，无法预览交易";
        }
        
        Item currentItem = Inventory.Instance.CurrentItem;
        
        if (!currentItem.IsTradeable())
        {
            return $"{currentItem.name} 不可交易";
        }
        
        // 计算交易价值
        int tradeValue = CalculateTradeValue(currentItem);
        
        // 选择合适的药水
        Item bestPotion = SelectBestPotion(tradeValue);
        
        if (bestPotion == null)
        {
            return "没有合适的药水可交易";
        }
        
        // 计算药水数量
        int potionCount = CalculatePotionCount(tradeValue, bestPotion);
        
        // 生成预览信息
        string preview = $"交易预览:\n";
        preview += $"物品: {currentItem.name} (价值: {currentItem.tradeValue})\n";
        preview += $"交易比率: {tradeRatio}\n";
        preview += $"总交易价值: {tradeValue}\n";
        preview += $"获得: {bestPotion.name} x{potionCount}\n";
        preview += $"恢复生命值: {bestPotion.healAmount * potionCount}";
        
        // 触发预览事件
        OnTradePreview?.Invoke(currentItem, bestPotion, potionCount);
        
        return preview;
    }
    
    /// <summary>
    /// 计算交易价值
    /// </summary>
    /// <param name="item">物品</param>
    /// <returns>交易价值</returns>
    private int CalculateTradeValue(Item item)
    {
        int baseValue = item.tradeValue;
        int tradeValue = Mathf.RoundToInt(baseValue * tradeRatio);
        
        if (showDebugInfo)
        {
            Debug.Log($"[PawnShop] 计算交易价值: {baseValue} * {tradeRatio} = {tradeValue}");
        }
        
        return tradeValue;
    }
    
    /// <summary>
    /// 选择合适的药水
    /// </summary>
    /// <param name="tradeValue">交易价值</param>
    /// <returns>最佳药水</returns>
    private Item SelectBestPotion(int tradeValue)
    {
        var potions = ItemManager.Instance?.GetPotions();
        if (potions == null || potions.Count == 0)
        {
            return null;
        }
        
        // 按恢复量排序，选择最合适的药水
        potions.Sort((a, b) => a.healAmount.CompareTo(b.healAmount));
        
        Item bestPotion = potions[0];
        
        // 找到恢复量最接近交易价值的药水
        foreach (var potion in potions)
        {
            if (potion.healAmount <= tradeValue)
            {
                bestPotion = potion;
            }
            else
            {
                break;
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[PawnShop] 选择药水: {bestPotion.name} (恢复: {bestPotion.healAmount})");
        }
        
        return bestPotion;
    }
    
    /// <summary>
    /// 计算药水数量
    /// </summary>
    /// <param name="tradeValue">交易价值</param>
    /// <param name="potion">药水</param>
    /// <returns>药水数量</returns>
    private int CalculatePotionCount(int tradeValue, Item potion)
    {
        int count = tradeValue / potion.healAmount;
        count = Mathf.Max(count, 1); // 最少1个
        
        if (showDebugInfo)
        {
            Debug.Log($"[PawnShop] 计算药水数量: {tradeValue} / {potion.healAmount} = {count}");
        }
        
        return count;
    }
    
    /// <summary>
    /// 执行交易
    /// </summary>
    /// <param name="item">交易物品</param>
    /// <param name="potion">获得药水</param>
    /// <param name="count">药水数量</param>
    /// <returns>是否交易成功</returns>
    private bool ExecuteTrade(Item item, Item potion, int count)
    {
        // 移除原物品
        Item removedItem = Inventory.Instance.RemoveCurrentItem();
        if (removedItem == null)
        {
            OnTradeFailed?.Invoke("移除物品失败");
            return false;
        }
        
        // 添加药水
        for (int i = 0; i < count; i++)
        {
            if (!Inventory.Instance.AddItem(potion))
            {
                // 如果添加失败，回滚交易
                Inventory.Instance.AddItem(removedItem);
                OnTradeFailed?.Invoke("背包已满，无法添加药水");
                return false;
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[PawnShop] 交易成功: {item.name} -> {potion.name} x{count}");
        }
        
        OnTradeCompleted?.Invoke(item, potion, count);
        return true;
    }
    
    /// <summary>
    /// 设置交易比率
    /// </summary>
    /// <param name="newRatio">新的交易比率</param>
    public void SetTradeRatio(float newRatio)
    {
        if (newRatio <= 0)
        {
            Debug.LogError("[PawnShop] 交易比率必须大于0");
            return;
        }
        
        tradeRatio = newRatio;
        
        if (showDebugInfo)
        {
            Debug.Log($"[PawnShop] 交易比率设置为: {tradeRatio}");
        }
    }
    
    /// <summary>
    /// 获取交易比率
    /// </summary>
    /// <returns>当前交易比率</returns>
    public float GetTradeRatio()
    {
        return tradeRatio;
    }
    
    /// <summary>
    /// 获取最佳交易建议
    /// </summary>
    /// <returns>交易建议信息</returns>
    public string GetBestTradeAdvice()
    {
        var tradeableItems = ItemManager.Instance?.GetTradeableItems();
        if (tradeableItems == null || tradeableItems.Count == 0)
        {
            return "没有可交易物品";
        }
        
        string advice = "最佳交易建议:\n";
        
        // 按交易价值排序
        tradeableItems.Sort((a, b) => a.tradeValue.CompareTo(b.tradeValue));
        
        foreach (var item in tradeableItems)
        {
            int tradeValue = CalculateTradeValue(item);
            Item bestPotion = SelectBestPotion(tradeValue);
            
            if (bestPotion != null)
            {
                int potionCount = CalculatePotionCount(tradeValue, bestPotion);
                advice += $"{item.name} ({item.tradeValue}) -> {bestPotion.name} x{potionCount}\n";
            }
        }
        
        return advice;
    }
    
    /// <summary>
    /// 批量交易测试
    /// </summary>
    /// <param name="itemNames">物品名称列表</param>
    public void BatchTradeTest(string[] itemNames)
    {
        if (showDebugInfo)
        {
            Debug.Log("[PawnShop] 开始批量交易测试");
        }
        
        int successCount = 0;
        int failCount = 0;
        
        foreach (string itemName in itemNames)
        {
            if (TradeItem(itemName))
            {
                successCount++;
            }
            else
            {
                failCount++;
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[PawnShop] 批量交易测试完成 - 成功: {successCount}, 失败: {failCount}");
        }
    }
    
    /// <summary>
    /// 显示当铺信息（调试用）
    /// </summary>
    public void ShowPawnShopInfo()
    {
        string info = $"[PawnShop] 当铺信息:\n";
        info += $"交易比率: {tradeRatio}\n";
        info += $"可交易物品数: {ItemManager.Instance?.GetTradeableItems().Count ?? 0}\n";
        info += $"药水种类数: {ItemManager.Instance?.GetPotions().Count ?? 0}";
        
        Debug.Log(info);
    }
} 