using UnityEngine;
using UnityEngine.UI;

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
        
        bool hasItems = Inventory.Instance != null && !Inventory.Instance.IsEmpty;
        
        // 更新按钮可交互状态
        tradeButton.interactable = hasItems;
        
        // 更新按钮文本
        if (hasItems)
        {
            int itemCount = Inventory.Instance.CurrentItemCount;
            int totalHealth = Inventory.Instance.GetTotalHealthValue();
            tradeButtonText.text = $"{tradeButtonDefaultText} ({itemCount}个物品, +{totalHealth}健康值)";
        }
        else
        {
            tradeButtonText.text = tradeButtonEmptyText;
        }
    }
    
    /// <summary>
    /// 交易按钮点击事件
    /// </summary>
    public void OnTradeButtonClicked()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[ShopTrade] Inventory实例未找到");
            return;
        }
        
        if (Inventory.Instance.IsEmpty)
        {
            Debug.LogWarning("[ShopTrade] 背包为空，无法进行交易");
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
        string itemsDetails = Inventory.Instance.GetItemsDetails();
        int totalHealth = Inventory.Instance.GetTotalHealthValue();
        int currentHealth = Player.Instance != null ? Player.Instance.CurrentHealth : 0;
        int newHealth = currentHealth + totalHealth;
        
        string confirmationMessage = $"确认典当所有物品吗？\n\n";
        confirmationMessage += $"当前物品:\n{itemsDetails}\n\n";
        confirmationMessage += $"当前健康值: {currentHealth}\n";
        confirmationMessage += $"交易后健康值: {newHealth}\n\n";
        confirmationMessage += "交易后背包将被清空。";
        
        confirmationText.text = confirmationMessage;
        confirmationPanel.SetActive(true);
        
        if (showDebugInfo)
        {
            Debug.Log("[ShopTrade] 显示交易确认面板");
        }
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
        if (Inventory.Instance == null)
        {
            Debug.LogError("[ShopTrade] Inventory实例未找到");
            return;
        }
        
        if (Player.Instance == null)
        {
            Debug.LogError("[ShopTrade] Player实例未找到");
            return;
        }
        
        if (Inventory.Instance.IsEmpty)
        {
            Debug.LogWarning("[ShopTrade] 背包为空，无法进行交易");
            return;
        }
        
        // 获取交易前的状态
        int itemCount = Inventory.Instance.CurrentItemCount;
        int totalHealth = Inventory.Instance.GetTotalHealthValue();
        int oldHealth = Player.Instance.CurrentHealth;
        
        // 执行交易
        // 1. 将物品的健康值加到玩家健康值上
        int newHealth = oldHealth + totalHealth;
        Player.Instance.SetHealth(newHealth);

        Debug.Log("[ShopTrade] 交易完成，玩家健康值: " + newHealth);
        
        // 2. 清空背包
        Inventory.Instance.ClearInventory();
        
        // 3. 同步数据到GameDataManager
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SyncInventoryData();
        }
        
        // 4. 更新UI
        UpdateTradeButtonState();
        
        // 显示交易结果
        string tradeResult = $"交易完成！\n";
        tradeResult += $"典当了 {itemCount} 个物品\n";
        tradeResult += $"获得 {totalHealth} 点健康值\n";
        tradeResult += $"健康值: {oldHealth} → {newHealth}";
        
        Debug.Log($"[ShopTrade] {tradeResult}");
        
        if (showDebugInfo)
        {
            Debug.Log($"[ShopTrade] 交易详情: 物品数={itemCount}, 健康值={totalHealth}, 旧健康值={oldHealth}, 新健康值={newHealth}");
        }
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
        if (Inventory.Instance == null)
        {
            return "Inventory实例未找到";
        }
        
        if (Inventory.Instance.IsEmpty)
        {
            return "背包为空，无法交易";
        }
        
        string info = "当前可交易物品:\n";
        info += Inventory.Instance.GetItemsDetails();
        
        if (Player.Instance != null)
        {
            int totalHealth = Inventory.Instance.GetTotalHealthValue();
            int currentHealth = Player.Instance.CurrentHealth;
            int newHealth = currentHealth + totalHealth;
            
            info += $"\n交易效果:\n";
            info += $"当前健康值: {currentHealth}\n";
            info += $"交易后健康值: {newHealth}\n";
            info += $"健康值增加: +{totalHealth}";
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