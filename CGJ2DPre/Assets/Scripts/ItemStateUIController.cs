using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品状态UI控制器
/// 提供UI界面来管理物品状态
/// </summary>
public class ItemStateUIController : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private GameObject statePanel;           // 状态管理面板
    [SerializeField] private Button togglePanelButton;        // 切换面板按钮
    [SerializeField] private Button closePanelButton;         // 关闭面板按钮
    
    [Header("物品状态按钮")]
    [SerializeField] private Button plantStateButton;         // 植物状态按钮
    [SerializeField] private Button necklaceStateButton;      // 项链状态按钮
    [SerializeField] private Button catStateButton;           // 猫状态按钮
    [SerializeField] private Button teapotStateButton;        // 水壶状态按钮
    [SerializeField] private Button birdStateButton;           // 鸟状态按钮
    [SerializeField] private Button dressStateButton;          // 连衣裙状态按钮
    [SerializeField] private Button oscarStateButton;          // 奥斯卡状态按钮
    [SerializeField] private Button diaryStateButton;          // 日记状态按钮
    
    [Header("重置按钮")]
    [SerializeField] private Button resetAllButton;           // 重置所有状态按钮
    [SerializeField] private Button showInfoButton;           // 显示信息按钮
    
    [Header("状态显示")]
    [SerializeField] private Text plantStateText;             // 植物状态文本
    [SerializeField] private Text necklaceStateText;          // 项链状态文本
    [SerializeField] private Text catStateText;               // 猫状态文本
    [SerializeField] private Text teapotStateText;            // 水壶状态文本
    [SerializeField] private Text birdStateText;               // 鸟状态文本
    [SerializeField] private Text dressStateText;              // 连衣裙状态文本
    [SerializeField] private Text oscarStateText;              // 奥斯卡状态文本
    [SerializeField] private Text diaryStateText;              // 日记状态文本
    
    [Header("设置")]
    [SerializeField] private bool showPanelOnStart = false;   // 启动时是否显示面板
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab; // 切换面板的按键
    
    private ItemStateManager itemStateManager;
    private bool isPanelOpen = false;
    
    private void Start()
    {
        // 获取ItemStateManager
        itemStateManager = ItemStateManager.Instance;
        if (itemStateManager == null)
        {
            Debug.LogError("[ItemStateUIController] ItemStateManager未找到");
            return;
        }
        
        // 初始化UI
        InitializeUI();
        
        // 注册事件
        RegisterEvents();
        
        // 设置初始状态
        SetPanelState(showPanelOnStart);
        UpdateStateTexts();
    }
    
    private void Update()
    {
        // 按键切换面板
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePanel();
        }
    }
    
    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitializeUI()
    {
        // 设置按钮事件
        if (togglePanelButton != null)
        {
            togglePanelButton.onClick.AddListener(TogglePanel);
        }
        
        if (closePanelButton != null)
        {
            closePanelButton.onClick.AddListener(() => SetPanelState(false));
        }
        
        if (plantStateButton != null)
        {
            plantStateButton.onClick.AddListener(() => CycleItemState("植物"));
        }
        
        if (necklaceStateButton != null)
        {
            necklaceStateButton.onClick.AddListener(() => CycleItemState("项链"));
        }
        
        if (catStateButton != null)
        {
            catStateButton.onClick.AddListener(() => CycleItemState("猫"));
        }
        
        if (teapotStateButton != null)
        {
            teapotStateButton.onClick.AddListener(() => CycleItemState("水壶"));
        }
        
        if (birdStateButton != null)
        {
            birdStateButton.onClick.AddListener(() => CycleItemState("鸟"));
        }
        
        if (dressStateButton != null)
        {
            dressStateButton.onClick.AddListener(() => CycleItemState("连衣裙"));
        }
        
        if (oscarStateButton != null)
        {
            oscarStateButton.onClick.AddListener(() => CycleItemState("奥斯卡"));
        }
        
        if (diaryStateButton != null)
        {
            diaryStateButton.onClick.AddListener(() => CycleItemState("日记"));
        }
        
        if (resetAllButton != null)
        {
            resetAllButton.onClick.AddListener(ResetAllStates);
        }
        
        if (showInfoButton != null)
        {
            showInfoButton.onClick.AddListener(ShowItemStatesInfo);
        }
    }
    
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        if (itemStateManager != null)
        {
            itemStateManager.OnItemStateChanged.AddListener(OnItemStateChanged);
        }
    }
    
    /// <summary>
    /// 物品状态变化事件处理
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <param name="newState">新状态</param>
    private void OnItemStateChanged(string itemName, int newState)
    {
        UpdateStateTexts();
    }
    
    /// <summary>
    /// 切换面板状态
    /// </summary>
    public void TogglePanel()
    {
        SetPanelState(!isPanelOpen);
    }
    
    /// <summary>
    /// 设置面板状态
    /// </summary>
    /// <param name="isOpen">是否打开</param>
    public void SetPanelState(bool isOpen)
    {
        isPanelOpen = isOpen;
        
        if (statePanel != null)
        {
            statePanel.SetActive(isOpen);
        }
        
        if (isOpen)
        {
            UpdateStateTexts();
        }
    }
    
    /// <summary>
    /// 循环切换物品状态
    /// </summary>
    /// <param name="itemName">物品名称</param>
    public void CycleItemState(string itemName)
    {
        if (itemStateManager == null) return;
        
        int currentState = itemStateManager.GetItemState(itemName);
        int newState = (currentState + 1) % 3;
        itemStateManager.SetItemState(itemName, newState);
        
        Debug.Log($"[ItemStateUIController] {itemName} 状态从 {currentState} 变为 {newState}");
    }
    
    /// <summary>
    /// 重置所有物品状态
    /// </summary>
    public void ResetAllStates()
    {
        if (itemStateManager == null) return;
        
        itemStateManager.ResetAllItemStates();
        UpdateStateTexts();
        Debug.Log("[ItemStateUIController] 所有物品状态已重置");
    }
    
    /// <summary>
    /// 显示物品状态信息
    /// </summary>
    public void ShowItemStatesInfo()
    {
        if (itemStateManager == null) return;
        
        itemStateManager.ShowItemStatesInfo();
    }
    
    /// <summary>
    /// 更新状态文本显示
    /// </summary>
    private void UpdateStateTexts()
    {
        if (itemStateManager == null) return;
        
        UpdateStateText(plantStateText, "植物");
        UpdateStateText(necklaceStateText, "项链");
        UpdateStateText(catStateText, "猫");
        UpdateStateText(teapotStateText, "水壶");
        UpdateStateText(birdStateText, "鸟");
        UpdateStateText(dressStateText, "连衣裙");
        UpdateStateText(oscarStateText, "奥斯卡");
        UpdateStateText(diaryStateText, "日记");
    }
    
    /// <summary>
    /// 更新单个状态文本
    /// </summary>
    /// <param name="textComponent">文本组件</param>
    /// <param name="itemName">物品名称</param>
    private void UpdateStateText(Text textComponent, string itemName)
    {
        if (textComponent == null) return;
        
        int state = itemStateManager.GetItemState(itemName);
        string stateName = GetStateName(itemName, state);
        textComponent.text = $"{itemName}: {stateName} (状态{state})";
    }
    
    /// <summary>
    /// 获取状态名称
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <param name="state">状态值</param>
    /// <returns>状态名称</returns>
    private string GetStateName(string itemName, int state)
    {
        switch (itemName)
        {
            case "植物":
                switch (state)
                {
                    case 0: return "正常";
                    case 1: return "枯萎";
                    case 2: return "死亡";
                    default: return "未知";
                }
            case "项链":
                switch (state)
                {
                    case 0: return "普通";
                    case 1: return "发光";
                    case 2: return "损坏";
                    default: return "未知";
                }
            case "猫":
                switch (state)
                {
                    case 0: return "正常";
                    case 1: return "受伤";
                    case 2: return "消失";
                    default: return "未知";
                }
            case "水壶":
                switch (state)
                {
                    case 0: return "普通";
                    case 1: return "发光";
                    case 2: return "破碎";
                    default: return "未知";
                }
            default:
                return "未知";
        }
    }
    
    /// <summary>
    /// 设置特定物品状态
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <param name="state">状态值</param>
    public void SetItemState(string itemName, int state)
    {
        if (itemStateManager == null) return;
        
        itemStateManager.SetItemState(itemName, state);
    }
    
    /// <summary>
    /// 获取物品当前状态
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>当前状态值</returns>
    public int GetItemState(string itemName)
    {
        if (itemStateManager == null) return 0;
        
        return itemStateManager.GetItemState(itemName);
    }
    
    private void OnDestroy()
    {
        // 取消事件注册
        if (itemStateManager != null)
        {
            itemStateManager.OnItemStateChanged.RemoveListener(OnItemStateChanged);
        }
    }
} 