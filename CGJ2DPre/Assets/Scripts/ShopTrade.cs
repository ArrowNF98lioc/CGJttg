using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 商店交易系统
/// 负责处理物品与健康值的交易
/// </summary>
public class ShopTrade : MonoBehaviour
{
    [Header("交易设置")]
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private bool showTradeConfirmation = true;  // 是否显示交易确认
    
    [Header("UI设置")]
    [SerializeField] private Button tradeButton;                 // 交易按钮
    [SerializeField] private Text tradeButtonText;              // 交易按钮文本
    [SerializeField] private GameObject confirmationPanel;       // 确认面板
    [SerializeField] private Text confirmationText;             // 确认文本
    [SerializeField] private Button confirmButton;              // 确认按钮
    [SerializeField] private Button cancelButton;               // 取消按钮
    
    [Header("交易信息")]
    [SerializeField] private string tradeButtonDefaultText = "典当物品";
    [SerializeField] private string tradeButtonEmptyText = "背包为空";
    
    private void Start()
    {
        // 初始化UI
        InitializeUI();
        
        // 更新交易按钮状态
        UpdateTradeButtonState();
        
        if (showDebugInfo)
        {
            Debug.Log("[ShopTrade] 商店交易系统初始化完成");
        }
    }
    
    private void Update()
    {
        InitializeUI();
        // 实时更新交易按钮状态
        UpdateTradeButtonState();
    }
    
    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitializeUI()
    {
        // 设置交易按钮点击事件
        if (tradeButton != null)
        {
            tradeButton.onClick.AddListener(OnTradeButtonClicked);
        }
        
        // 设置确认和取消按钮事件
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ConfirmTrade);
        }
        
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CancelTrade);
        }
        
        // 默认隐藏确认面板
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 更新交易按钮状态
    /// </summary>
    private void UpdateTradeButtonState()
    {
        if (tradeButton == null || tradeButtonText == null) return;
        
        bool hasSelectedItems = HasSelectedItems();
        
        // 更新按钮可交互状态
        tradeButton.interactable = hasSelectedItems;
        
        // 更新按钮文本
        if (hasSelectedItems)
        {
            var selectedItemsInfo = GetSelectedItemsInfo();
            tradeButtonText.text = $"{tradeButtonDefaultText} ({selectedItemsInfo.count}个物品, +{selectedItemsInfo.totalHealth}健康值)";
        }
        else
        {
            tradeButtonText.text = tradeButtonEmptyText;
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
    /// 交易按钮点击事件
    /// </summary>
    public void OnTradeButtonClicked()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("[ShopTrade] GameDataManager实例未找到");
            return;
        }
        
        if (!HasSelectedItems())
        {
            Debug.LogWarning("[ShopTrade] 没有Selected状态的物品，无法进行交易");
            return;
        }
        
        if (showTradeConfirmation)
        {
            ShowTradeConfirmation();
        }
        else
        {
            ExecuteTrade();
        }
    }
    
    /// <summary>
    /// 显示交易确认面板
    /// </summary>
    private void ShowTradeConfirmation()
    {
        if (confirmationPanel == null || confirmationText == null)
        {
            Debug.LogError("[ShopTrade] 确认面板或文本组件未设置");
            return;
        }
        
        // 生成确认文本
        string itemsDetails = GetSelectedItemsDetails();
        var selectedItemsInfo = GetSelectedItemsInfo();
        int currentHealth = Player.Instance != null ? Player.Instance.CurrentHealth : 0;
        int newHealth = currentHealth + selectedItemsInfo.totalHealth;
        
        string confirmationMessage = $"确认典当所有Selected状态的物品吗？\n\n";
        confirmationMessage += $"当前Selected物品:\n{itemsDetails}\n\n";
        confirmationMessage += $"当前健康值: {currentHealth}\n";
        confirmationMessage += $"交易后健康值: {newHealth}\n\n";
        confirmationMessage += "交易后Selected物品状态将变为Solved。";
        
        confirmationText.text = confirmationMessage;
        confirmationPanel.SetActive(true);
        
        if (showDebugInfo)
        {
            Debug.Log("[ShopTrade] 显示交易确认面板");
        }
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
    /// 确认交易
    /// </summary>
    public void ConfirmTrade()
    {
        ExecuteTrade();
        HideTradeConfirmation();
    }
    
    /// <summary>
    /// 取消交易
    /// </summary>
    public void CancelTrade()
    {
        HideTradeConfirmation();
        
        if (showDebugInfo)
        {
            Debug.Log("[ShopTrade] 交易已取消");
        }
    }
    
    /// <summary>
    /// 隐藏交易确认面板
    /// </summary>
    private void HideTradeConfirmation()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 执行交易
    /// </summary>
    public void ExecuteTrade()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("[ShopTrade] GameDataManager实例未找到");
            return;
        }
        
        if (Player.Instance == null)
        {
            Debug.LogError("[ShopTrade] Player实例未找到");
            return;
        }
        
        if (!HasSelectedItems())
        {
            Debug.LogWarning("[ShopTrade] 没有Selected状态的物品，无法进行交易");
            return;
        }
        
        // 获取交易前的状态
        var selectedItemsInfo = GetSelectedItemsInfo();
        int oldHealth = Player.Instance.CurrentHealth;
        
        // 执行交易
        // 1. 将物品的健康值加到玩家健康值上
        int newHealth = oldHealth + selectedItemsInfo.totalHealth;
        Player.Instance.SetHealth(newHealth);

        Debug.Log("[ShopTrade] 交易完成，玩家健康值: " + newHealth);
        
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
                    Debug.Log($"[ShopTrade] 物品状态更新: {itemState.Key} Selected → Solved");
                }
            }
        }
        
        // 3. 同步数据到GameDataManager
        GameDataManager.Instance.SyncPlayerData();
        
        // 4. 更新UI
        UpdateTradeButtonState();
        
        // 显示交易结果
        string tradeResult = $"交易完成！\n";
        tradeResult += $"典当了 {selectedItemsInfo.count} 个Selected物品\n";
        tradeResult += $"获得 {selectedItemsInfo.totalHealth} 点健康值\n";
        tradeResult += $"健康值: {oldHealth} → {newHealth}\n";
        tradeResult += $"交易物品: {string.Join(", ", tradedItems)}";
        
        Debug.Log($"[ShopTrade] {tradeResult}");
        
        if (showDebugInfo)
        {
            Debug.Log($"[ShopTrade] 交易详情: 物品数={selectedItemsInfo.count}, 健康值={selectedItemsInfo.totalHealth}, 旧健康值={oldHealth}, 新健康值={newHealth}");
        }

        // 更新商店物品的Active状态
        ShopItemStateController shopController = FindObjectOfType<ShopItemStateController>();
        if (shopController != null)
            shopController.UpdateShopItemsActiveState();
    }
    
    /// <summary>
    /// 设置交易按钮
    /// </summary>
    /// <param name="button">交易按钮</param>
    public void SetTradeButton(Button button)
    {
        tradeButton = button;
        if (button != null)
        {
            button.onClick.AddListener(OnTradeButtonClicked);
        }
    }
    
    /// <summary>
    /// 设置交易按钮文本
    /// </summary>
    /// <param name="text">文本组件</param>
    public void SetTradeButtonText(Text text)
    {
        tradeButtonText = text;
    }
    
    /// <summary>
    /// 设置确认面板
    /// </summary>
    /// <param name="panel">确认面板</param>
    /// <param name="text">确认文本</param>
    /// <param name="confirmBtn">确认按钮</param>
    /// <param name="cancelBtn">取消按钮</param>
    public void SetConfirmationPanel(GameObject panel, Text text, Button confirmBtn, Button cancelBtn)
    {
        confirmationPanel = panel;
        confirmationText = text;
        confirmButton = confirmBtn;
        cancelButton = cancelBtn;
        
        if (confirmBtn != null)
        {
            confirmBtn.onClick.AddListener(ConfirmTrade);
        }
        
        if (cancelBtn != null)
        {
            cancelBtn.onClick.AddListener(CancelTrade);
        }
        
        if (panel != null)
        {
            panel.SetActive(false);
        }
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