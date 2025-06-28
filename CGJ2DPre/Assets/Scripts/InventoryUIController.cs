using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包UI控制器
/// 管理背包界面的显示、交互和动画
/// </summary>
public class InventoryUIController : MonoBehaviour
{
    [Header("背包UI组件")]
    [SerializeField] private GameObject inventoryPanel;        // 背包面板
    [SerializeField] private Transform slotsParent;            // 背包槽位父对象
    [SerializeField] private GameObject slotPrefab;            // 背包槽位预制体
    [SerializeField] private Button openButton;                // 打开背包按钮
    [SerializeField] private Button closeButton;               // 关闭背包按钮
    
    [Header("背包信息显示")]
    [SerializeField] private Text itemCountText;               // 物品数量文本
    [SerializeField] private Text maxSlotsText;                // 最大槽位文本
    [SerializeField] private Text selectedItemText;            // 选中物品信息文本
    
    [Header("背包设置")]
    [SerializeField] private bool showItemTooltip = true;       // 是否显示物品提示
    [SerializeField] private GameObject tooltipPanel;          // 提示面板
    [SerializeField] private Text tooltipText;                 // 提示文本
    
    [Header("动画设置")]
    [SerializeField] private bool useAnimation = true;         // 是否使用动画
    [SerializeField] private float animationDuration = 0.3f;   // 动画持续时间
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    // 私有变量
    private Inventory inventory;
    private CanvasGroup canvasGroup;
    private RectTransform panelRect;
    private Vector3 originalScale;
    private bool isAnimating = false;
    
    private void Start()
    {
        // 获取Inventory实例
        inventory = Inventory.Instance;
        if (inventory == null)
        {
            Debug.LogError("[InventoryUIController] 未找到Inventory实例");
            return;
        }
        
        // 初始化UI组件
        InitializeUIComponents();
        
        // 设置按钮事件
        SetupButtonEvents();
        
        // 设置背包UI
        inventory.SetInventoryUI(inventoryPanel, slotsParent, slotPrefab);
        
        Debug.Log("[InventoryUIController] 背包UI控制器初始化完成");
    }
    
    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitializeUIComponents()
    {
        // 获取CanvasGroup组件
        if (inventoryPanel != null)
        {
            canvasGroup = inventoryPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = inventoryPanel.AddComponent<CanvasGroup>();
            }
            
            panelRect = inventoryPanel.GetComponent<RectTransform>();
            originalScale = panelRect.localScale;
        }
        
        // 初始化提示面板
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
        
        // 更新背包信息显示
        UpdateInventoryInfo();
    }
    
    /// <summary>
    /// 设置按钮事件
    /// </summary>
    private void SetupButtonEvents()
    {
        if (openButton != null)
        {
            openButton.onClick.AddListener(OpenInventory);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }
    }
    
    /// <summary>
    /// 打开背包
    /// </summary>
    public void OpenInventory()
    {
        if (inventory != null && !isAnimating)
        {
            inventory.OpenInventory();
            
            if (useAnimation)
            {
                StartCoroutine(AnimateInventoryOpen());
            }
            else
            {
                ShowInventoryPanel();
            }
            
            if (showDebugInfo)
            {
                Debug.Log("[InventoryUIController] 打开背包");
            }
        }
    }
    
    /// <summary>
    /// 关闭背包
    /// </summary>
    public void CloseInventory()
    {
        if (inventory != null && !isAnimating)
        {
            inventory.CloseInventory();
            
            if (useAnimation)
            {
                StartCoroutine(AnimateInventoryClose());
            }
            else
            {
                HideInventoryPanel();
            }
            
            if (showDebugInfo)
            {
                Debug.Log("[InventoryUIController] 关闭背包");
            }
        }
    }
    
    /// <summary>
    /// 切换背包显示状态
    /// </summary>
    public void ToggleInventory()
    {
        if (inventory != null)
        {
            if (inventory.IsInventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }
    
    /// <summary>
    /// 显示背包面板
    /// </summary>
    private void ShowInventoryPanel()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
        
        UpdateInventoryInfo();
    }
    
    /// <summary>
    /// 隐藏背包面板
    /// </summary>
    private void HideInventoryPanel()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
        
        // 隐藏提示面板
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 更新背包信息显示
    /// </summary>
    public void UpdateInventoryInfo()
    {
        if (inventory == null) return;
        
        // 更新物品数量
        if (itemCountText != null)
        {
            itemCountText.text = inventory.CurrentItemCount.ToString();
        }
        
        // 更新最大槽位
        if (maxSlotsText != null)
        {
            maxSlotsText.text = inventory.MaxSlots.ToString();
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[InventoryUIController] 更新背包信息: {inventory.CurrentItemCount}/{inventory.MaxSlots}");
        }
    }
    
    /// <summary>
    /// 显示物品提示
    /// </summary>
    /// <param name="item">物品对象</param>
    /// <param name="position">显示位置</param>
    public void ShowItemTooltip(Item item, Vector3 position)
    {
        if (!showItemTooltip || tooltipPanel == null || tooltipText == null) return;
        
        if (item != null)
        {
            string tooltip = $"物品: {item.name}\n";
            tooltip += $"健康值: {item.health}\n";
            tooltip += $"有生命: {(item.hasLife ? "是" : "否")}";
            
            tooltipText.text = tooltip;
            tooltipPanel.transform.position = position;
            tooltipPanel.SetActive(true);
        }
        else
        {
            tooltipPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 隐藏物品提示
    /// </summary>
    public void HideItemTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 设置选中物品信息
    /// </summary>
    /// <param name="item">物品对象</param>
    public void SetSelectedItemInfo(Item item)
    {
        if (selectedItemText != null)
        {
            if (item != null)
            {
                selectedItemText.text = $"选中: {item.name}";
            }
            else
            {
                selectedItemText.text = "未选中物品";
            }
        }
    }
    
    /// <summary>
    /// 背包打开动画
    /// </summary>
    private System.Collections.IEnumerator AnimateInventoryOpen()
    {
        isAnimating = true;
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            
            if (panelRect != null)
            {
                panelRect.localScale = Vector3.zero;
            }
            
            float elapsed = 0f;
            
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / animationDuration;
                float curveValue = animationCurve.Evaluate(progress);
                
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = curveValue;
                }
                
                if (panelRect != null)
                {
                    panelRect.localScale = Vector3.Lerp(Vector3.zero, originalScale, curveValue);
                }
                
                yield return null;
            }
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            
            if (panelRect != null)
            {
                panelRect.localScale = originalScale;
            }
        }
        
        UpdateInventoryInfo();
        isAnimating = false;
    }
    
    /// <summary>
    /// 背包关闭动画
    /// </summary>
    private System.Collections.IEnumerator AnimateInventoryClose()
    {
        isAnimating = true;
        
        if (inventoryPanel != null)
        {
            float elapsed = 0f;
            
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / animationDuration;
                float curveValue = animationCurve.Evaluate(progress);
                
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f - curveValue;
                }
                
                if (panelRect != null)
                {
                    panelRect.localScale = Vector3.Lerp(originalScale, Vector3.zero, curveValue);
                }
                
                yield return null;
            }
            
            HideInventoryPanel();
        }
        
        isAnimating = false;
    }
    
    /// <summary>
    /// 处理槽位点击事件
    /// </summary>
    /// <param name="slotIndex">槽位索引</param>
    /// <param name="item">物品对象</param>
    public void OnSlotClicked(int slotIndex, Item item)
    {
        SetSelectedItemInfo(item);
        
        if (showDebugInfo)
        {
            Debug.Log($"[InventoryUIController] 槽位 {slotIndex} 被点击，物品: {(item != null ? item.name : "无")}");
        }
    }
    
    /// <summary>
    /// 处理槽位悬停事件
    /// </summary>
    /// <param name="slotIndex">槽位索引</param>
    /// <param name="item">物品对象</param>
    public void OnSlotHovered(int slotIndex, Item item)
    {
        if (showItemTooltip && item != null)
        {
            Vector3 tooltipPosition = Input.mousePosition;
            ShowItemTooltip(item, tooltipPosition);
        }
    }
    
    /// <summary>
    /// 处理槽位退出事件
    /// </summary>
    /// <param name="slotIndex">槽位索引</param>
    public void OnSlotExited(int slotIndex)
    {
        HideItemTooltip();
    }
    
    /// <summary>
    /// 刷新背包显示
    /// </summary>
    public void RefreshInventoryDisplay()
    {
        if (inventory != null)
        {
            inventory.UpdateInventoryDisplay();
            UpdateInventoryInfo();
        }
    }
    
    private void Update()
    {
        // 键盘快捷键
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
        
        // 更新提示面板位置（跟随鼠标）
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            tooltipPanel.transform.position = Input.mousePosition;
        }
    }
    
    private void OnDestroy()
    {
        // 清理事件订阅
        if (openButton != null)
        {
            openButton.onClick.RemoveListener(OpenInventory);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseInventory);
        }
    }
} 