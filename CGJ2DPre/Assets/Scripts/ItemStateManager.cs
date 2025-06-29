using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 物品状态管理系统
/// 管理四种物品（植物、项链、猫、水壶）的三种状态
/// </summary>
public class ItemStateManager : MonoBehaviour
{
    [Header("物品状态配置")]
    [SerializeField] private List<ItemStateConfig> itemConfigs = new List<ItemStateConfig>();
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    // 单例模式
    public static ItemStateManager Instance { get; private set; }
    
    // 当前物品状态
    private Dictionary<string, int> currentItemStates = new Dictionary<string, int>();
    
    // 事件
    public UnityEvent<string, int> OnItemStateChanged = new UnityEvent<string, int>();
    
    private void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeItemStates();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        Debug.Log($"[ItemStateManager] 物品状态管理器初始化完成");
        ApplyAllItemStates();
    }
    
    /// <summary>
    /// 初始化物品状态
    /// </summary>
    private void InitializeItemStates()
    {
        // 初始化四种物品的状态为0
        currentItemStates["植物"] = 0;
        currentItemStates["项链"] = 0;
        currentItemStates["猫"] = 0;
        currentItemStates["水壶"] = 0;
        
        Debug.Log("[ItemStateManager] 物品状态初始化完成");
    }
    
    /// <summary>
    /// 设置物品状态
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <param name="state">状态值 (0-2)</param>
    public void SetItemState(string itemName, int state)
    {
        if (!currentItemStates.ContainsKey(itemName))
        {
            Debug.LogError($"[ItemStateManager] 未知物品: {itemName}");
            return;
        }
        
        if (state < 0 || state > 2)
        {
            Debug.LogError($"[ItemStateManager] 无效状态值: {state}，应为 0-2");
            return;
        }
        
        int oldState = currentItemStates[itemName];
        currentItemStates[itemName] = state;
        
        // 应用状态变化
        ApplyItemState(itemName, state);
        
        // 触发事件
        OnItemStateChanged?.Invoke(itemName, state);
        
        if (showDebugInfo)
        {
            Debug.Log($"[ItemStateManager] {itemName} 状态从 {oldState} 变为 {state}");
        }
    }
    
    /// <summary>
    /// 获取物品当前状态
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>当前状态值</returns>
    public int GetItemState(string itemName)
    {
        if (currentItemStates.TryGetValue(itemName, out int state))
        {
            return state;
        }
        
        Debug.LogWarning($"[ItemStateManager] 未知物品: {itemName}");
        return 0;
    }
    
    /// <summary>
    /// 应用物品状态
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <param name="state">状态值</param>
    private void ApplyItemState(string itemName, int state)
    {
        ItemStateConfig config = GetItemConfig(itemName);
        if (config == null)
        {
            Debug.LogError($"[ItemStateManager] 未找到物品配置: {itemName}");
            return;
        }
        
        if (state >= config.states.Count)
        {
            Debug.LogError($"[ItemStateManager] 状态 {state} 超出范围，最大状态: {config.states.Count - 1}");
            return;
        }
        
        ItemState itemState = config.states[state];
        
        // 应用状态变化
        ApplyStateChanges(itemState);
    }
    
    /// <summary>
    /// 应用状态变化
    /// </summary>
    /// <param name="itemState">物品状态配置</param>
    private void ApplyStateChanges(ItemState itemState)
    {
        // 处理GameObject激活状态
        foreach (var gameObjectState in itemState.gameObjectStates)
        {
            if (gameObjectState.target != null)
            {
                gameObjectState.target.SetActive(gameObjectState.isActive);
                
                if (showDebugInfo)
                {
                    Debug.Log($"[ItemStateManager] 设置 {gameObjectState.target.name} 激活状态为: {gameObjectState.isActive}");
                }
            }
        }
        
        // 处理SpriteRenderer贴图变化
        foreach (var spriteState in itemState.spriteStates)
        {
            if (spriteState.renderer != null && spriteState.sprite != null)
            {
                spriteState.renderer.sprite = spriteState.sprite;
                
                if (showDebugInfo)
                {
                    Debug.Log($"[ItemStateManager] 设置 {spriteState.renderer.name} 贴图为: {spriteState.sprite.name}");
                }
            }
        }
        
        // 处理Image贴图变化
        foreach (var imageState in itemState.imageStates)
        {
            if (imageState.image != null && imageState.sprite != null)
            {
                imageState.image.sprite = imageState.sprite;
                
                if (showDebugInfo)
                {
                    Debug.Log($"[ItemStateManager] 设置 {imageState.image.name} 贴图为: {imageState.sprite.name}");
                }
            }
        }
        
        // 执行自定义逻辑
        if (itemState.customLogic != null)
        {
            itemState.customLogic.Invoke();
            
            if (showDebugInfo)
            {
                Debug.Log($"[ItemStateManager] 执行自定义逻辑");
            }
        }
    }
    
    /// <summary>
    /// 获取物品配置
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>物品配置</returns>
    private ItemStateConfig GetItemConfig(string itemName)
    {
        return itemConfigs.Find(config => config.itemName == itemName);
    }
    
    /// <summary>
    /// 应用所有物品状态
    /// </summary>
    private void ApplyAllItemStates()
    {
        foreach (var kvp in currentItemStates)
        {
            ApplyItemState(kvp.Key, kvp.Value);
        }
    }
    
    /// <summary>
    /// 重置所有物品状态为0
    /// </summary>
    public void ResetAllItemStates()
    {
        foreach (var itemName in currentItemStates.Keys)
        {
            SetItemState(itemName, 0);
        }
    }
    
    /// <summary>
    /// 获取所有物品状态信息
    /// </summary>
    /// <returns>状态信息字符串</returns>
    public string GetItemStatesInfo()
    {
        string info = "[ItemStateManager] 当前物品状态:\n";
        foreach (var kvp in currentItemStates)
        {
            info += $"- {kvp.Key}: 状态 {kvp.Value}\n";
        }
        return info;
    }
    
    /// <summary>
    /// 显示物品状态信息
    /// </summary>
    public void ShowItemStatesInfo()
    {
        Debug.Log(GetItemStatesInfo());
    }
    
    /// <summary>
    /// 添加物品配置
    /// </summary>
    /// <param name="config">物品配置</param>
    public void AddItemConfig(ItemStateConfig config)
    {
        if (config == null)
        {
            Debug.LogError("[ItemStateManager] 配置不能为空");
            return;
        }
        
        // 移除已存在的配置
        itemConfigs.RemoveAll(c => c.itemName == config.itemName);
        
        // 添加新配置
        itemConfigs.Add(config);
        
        Debug.Log($"[ItemStateManager] 添加物品配置: {config.itemName}");
    }
    
    /// <summary>
    /// 移除物品配置
    /// </summary>
    /// <param name="itemName">物品名称</param>
    public void RemoveItemConfig(string itemName)
    {
        int removed = itemConfigs.RemoveAll(c => c.itemName == itemName);
        if (removed > 0)
        {
            Debug.Log($"[ItemStateManager] 移除物品配置: {itemName}");
        }
    }
}

/// <summary>
/// 物品状态配置
/// </summary>
[System.Serializable]
public class ItemStateConfig
{
    [Header("物品基本信息")]
    public string itemName;           // 物品名称
    
    [Header("状态配置")]
    public List<ItemState> states = new List<ItemState>();  // 三种状态配置
}

/// <summary>
/// 物品状态
/// </summary>
[System.Serializable]
public class ItemState
{
    [Header("状态名称")]
    public string stateName = "状态";  // 状态名称
    
    [Header("GameObject激活状态")]
    public List<GameObjectState> gameObjectStates = new List<GameObjectState>();
    
    [Header("SpriteRenderer贴图变化")]
    public List<SpriteState> spriteStates = new List<SpriteState>();
    
    [Header("Image贴图变化")]
    public List<ImageState> imageStates = new List<ImageState>();
    
    [Header("自定义逻辑")]
    public UnityEvent customLogic = new UnityEvent();
}

/// <summary>
/// GameObject状态配置
/// </summary>
[System.Serializable]
public class GameObjectState
{
    public GameObject target;     // 目标GameObject
    public bool isActive = true;  // 是否激活
}

/// <summary>
/// Sprite状态配置
/// </summary>
[System.Serializable]
public class SpriteState
{
    public SpriteRenderer renderer;  // SpriteRenderer组件
    public Sprite sprite;           // 目标贴图
}

/// <summary>
/// Image状态配置
/// </summary>
[System.Serializable]
public class ImageState
{
    public UnityEngine.UI.Image image;  // Image组件
    public Sprite sprite;               // 目标贴图
} 