using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 简化的生命值系统
/// 只负责存储健康值和根据健康值划分三个阶段
/// 实现单例模式，确保数据在场景间保持一致
/// </summary>
public class Player : MonoBehaviour
{
    // 单例模式
    public static Player Instance { get; private set; }
    
    [Header("生命值设置")]
    [SerializeField] private int maxHealth = 100;        // 最大生命值
    [SerializeField] private int currentHealth = 59;    // 当前生命值
    
    [Header("时间流逝设置")]
    [SerializeField] private bool enableTimeDecay = true;  // 是否启用时间流逝
    [SerializeField] private float decayInterval = 6f;     // 健康值减少间隔（秒）
    [SerializeField] private int decayAmount = 8;          // 每次减少的健康值
    
    [Header("背包检测设置")]
    [SerializeField] public bool enableInventoryCheck = true;  // 是否启用背包检测
    
    [System.Serializable]
    public class ItemUIMapping
    {
        [Header("物品设置")]
        public string itemName;           // 物品名称（必须与ItemManager中的名称一致）
        [Header("UI设置")]
        public GameObject uiGameObject;   // 对应的UI GameObject
        [Header("调试")]
        public bool showDebugInfo = false; // 是否显示调试信息
    }
    
    [Header("物品UI映射列表")]
    [SerializeField] public List<ItemUIMapping> itemUIMappings = new List<ItemUIMapping>();
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;

    [Header("背包设置")]
    public Item currentItem; // 当前携带的物品
    
    // 属性
    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }
    public int CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = value;
    }
    public float HealthPercentage => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    
    // 时间流逝相关
    private Coroutine healthDecayCoroutine;
    
    // 背包检测相关
    private bool lastInventoryState = false; // 上一帧的背包状态
    private string lastItemName = "";        // 上一帧的物品名称
    
    /// <summary>
    /// 健康阶段枚举
    /// </summary>
    public enum HealthStage
    {
        Stage1, // 健康阶段1
        Stage2, // 健康阶段2  
        Stage3  // 健康阶段3
    }
    
    // 添加一个标志来跟踪数据是否已经初始化
    private bool dataInitialized = false;
    
    private void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[Player] 单例模式初始化完成，数据将在场景间保持一致");
            
            // 立即从GameDataManager加载数据（如果存在）
            if (GameDataManager.Instance != null)
            {
                SyncWithGameDataManager();
            }
        }
        else
        {
            Debug.Log("[Player] 检测到重复的Player实例，销毁当前实例");
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // 确保当前生命值不超过最大生命值
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log($"[Player] 生命值系统初始化完成 - 当前生命值: {currentHealth}/{maxHealth}");
        
        // 如果还没有从GameDataManager同步过数据，则现在同步
        if (!dataInitialized && GameDataManager.Instance != null)
        {
            SyncWithGameDataManager();
        }
        
        // 验证背包检测设置
        Debug.Log($"[Player] 背包检测启用状态: {enableInventoryCheck}");
        
        // 显示Inspector设置指导
        Debug.Log("[Player] ===== Inspector设置指导 =====");
        Debug.Log("[Player] 1. 在Player组件的Inspector中找到'物品UI映射列表'部分");
        Debug.Log("[Player] 2. 设置'Item UIMappings'的Size为你需要的映射数量");
        Debug.Log("[Player] 3. 为每个Element设置:");
        Debug.Log("[Player]    - Item Name: 输入物品名称（如'项链'、'茶壶'）");
        Debug.Log("[Player]    - UI GameObject: 拖拽对应的UI GameObject");
        Debug.Log("[Player]    - Show Debug Info: 可选，开启调试信息");
        Debug.Log("[Player] =================================");
        
        // 验证物品UI映射设置
        ValidateItemUIMappings();
        
        // 显示物品UI映射列表信息
        ShowItemUIMappingListInfo();
        
        // 启动健康值时间流逝
        if (enableTimeDecay)
        {
            StartHealthDecay();
        }
        
        // 初始化背包状态
        UpdateInventoryUI();
    }
    
    private void Update()
    {
        // 每帧检测背包状态
        if (enableInventoryCheck)
        {
            CheckInventoryStatus();
        }
        
        // 每帧同步数据到GameDataManager
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.UpdatePlayTime(Time.deltaTime);
            // 每帧同步Player数据到GameDataManager - 但只在数据真正改变时才同步
            SyncToGameDataManagerIfChanged();
        }
    }
    
    /// <summary>
    /// 与GameDataManager同步数据
    /// </summary>
    private void SyncWithGameDataManager()
    {
        if (GameDataManager.Instance != null)
        {
            // 从GameDataManager加载数据
            maxHealth = GameDataManager.Instance.playerMaxHealth;
            currentHealth = GameDataManager.Instance.playerCurrentHealth;
            
            // 加载当前物品
            if (!string.IsNullOrEmpty(GameDataManager.Instance.playerCurrentItem))
            {
                if (ItemManager.Instance != null)
                {
                    currentItem = ItemManager.Instance.GetItem(GameDataManager.Instance.playerCurrentItem);
                }
            }
            
            dataInitialized = true;
            
            Debug.Log($"[Player] 从GameDataManager同步数据: 生命值={currentHealth}/{maxHealth}, 物品={GameDataManager.Instance.playerCurrentItem}");
        }
    }
    
    /// <summary>
    /// 同步数据到GameDataManager（只在数据真正改变时才同步）
    /// </summary>
    public void SyncToGameDataManagerIfChanged()
    {
        if (GameDataManager.Instance != null && dataInitialized)
        {
            // 检查数据是否真的改变了
            bool dataChanged = false;
            
            if (GameDataManager.Instance.playerMaxHealth != maxHealth)
            {
                GameDataManager.Instance.playerMaxHealth = maxHealth;
                dataChanged = true;
            }
            
            if (GameDataManager.Instance.playerCurrentHealth != currentHealth)
            {
                GameDataManager.Instance.playerCurrentHealth = currentHealth;
                dataChanged = true;
            }
            
            string currentItemName = currentItem != null ? currentItem.name : "";
            if (GameDataManager.Instance.playerCurrentItem != currentItemName)
            {
                GameDataManager.Instance.playerCurrentItem = currentItemName;
                dataChanged = true;
            }
            
            if (dataChanged && showDebugInfo)
            {
                Debug.Log($"[Player] 同步数据到GameDataManager: 生命值={currentHealth}/{maxHealth}, 物品={currentItemName}");
            }
        }
    }
    
    /// <summary>
    /// 同步数据到GameDataManager（强制同步，用于兼容性）
    /// </summary>
    public void SyncToGameDataManager()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.playerMaxHealth = maxHealth;
            GameDataManager.Instance.playerCurrentHealth = currentHealth;
            GameDataManager.Instance.playerCurrentItem = currentItem != null ? currentItem.name : "";
            
            Debug.Log($"[Player] 同步数据到GameDataManager: 生命值={currentHealth}/{maxHealth}, 物品={GameDataManager.Instance.playerCurrentItem}");
        }
    }
    
    /// <summary>
    /// 检测背包状态并更新UI
    /// </summary>
    private void CheckInventoryStatus()
    {
        bool hasItem = currentItem != null;
        string currentItemName = hasItem ? currentItem.name : "";
        
        // 如果背包状态或物品发生变化
        if (hasItem != lastInventoryState || currentItemName != lastItemName)
        {
            UpdateInventoryUI();
            lastInventoryState = hasItem;
            lastItemName = currentItemName;
            
            if (showDebugInfo)
            {
                string status = hasItem ? "有物品" : "无物品";
                string itemName = hasItem ? currentItem.name : "无";
                Debug.Log($"[Player] 背包状态变化: {status} - 物品: {itemName}");
            }
        }
    }
    
    /// <summary>
    /// 更新背包UI显示
    /// </summary>
    private void UpdateInventoryUI()
    {
        // 先隐藏所有物品UI
        HideAllItemUIs();
        
        // 如果有物品，激活对应的UI
        if (currentItem != null)
        {
            ActivateItemUI(currentItem.name);
        }
    }
    
    /// <summary>
    /// 隐藏所有物品UI
    /// </summary>
    private void HideAllItemUIs()
    {
        foreach (var mapping in itemUIMappings)
        {
            if (mapping.uiGameObject != null)
            {
                mapping.uiGameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 激活指定物品的UI
    /// </summary>
    /// <param name="itemName">物品名称</param>
    private void ActivateItemUI(string itemName)
    {
        foreach (var mapping in itemUIMappings)
        {
            if (mapping.itemName == itemName && mapping.uiGameObject != null)
            {
                mapping.uiGameObject.SetActive(true);
                
                if (showDebugInfo || mapping.showDebugInfo)
                {
                    Debug.Log($"[Player] 激活物品UI: {itemName} -> {mapping.uiGameObject.name}");
                }
                return;
            }
        }
        
        // 如果没有找到对应的映射
        if (showDebugInfo)
        {
            Debug.LogWarning($"[Player] 未找到物品 '{itemName}' 对应的UI映射");
            Debug.LogWarning($"[Player] 当前映射列表:");
            for (int i = 0; i < itemUIMappings.Count; i++)
            {
                var mapping = itemUIMappings[i];
                string uiName = mapping.uiGameObject != null ? mapping.uiGameObject.name : "null";
                Debug.LogWarning($"[Player]   {i}: {mapping.itemName} -> {uiName}");
            }
        }
    }
    
    /// <summary>
    /// 验证物品UI映射设置
    /// </summary>
    public void ValidateItemUIMappings()
    {
        Debug.Log("[Player] 开始验证物品UI映射设置...");
        
        if (itemUIMappings.Count == 0)
        {
            Debug.LogWarning("[Player] 物品UI映射列表为空，请添加映射关系");
            return;
        }
        
        for (int i = 0; i < itemUIMappings.Count; i++)
        {
            var mapping = itemUIMappings[i];
            
            // 检查物品名称
            if (string.IsNullOrEmpty(mapping.itemName))
            {
                Debug.LogError($"[Player] 映射 {i}: 物品名称为空");
                continue;
            }
            
            // 检查UI GameObject
            if (mapping.uiGameObject == null)
            {
                Debug.LogError($"[Player] 映射 {i}: UI GameObject为空 (物品: {mapping.itemName})");
                continue;
            }
            
            // 检查物品是否在ItemManager中存在
            if (ItemManager.Instance != null && !ItemManager.Instance.HasItem(mapping.itemName))
            {
                Debug.LogWarning($"[Player] 映射 {i}: 物品 '{mapping.itemName}' 在ItemManager中不存在");
            }
            
            Debug.Log($"[Player] 映射 {i}: {mapping.itemName} -> {mapping.uiGameObject.name} ✓");
        }
        
        Debug.Log($"[Player] 验证完成，共 {itemUIMappings.Count} 个映射");
    }
    
    /// <summary>
    /// 添加物品UI映射到列表
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <param name="uiGameObject">对应的UI GameObject</param>
    /// <param name="showDebug">是否显示调试信息</param>
    public void AddItemUIMappingToList(string itemName, GameObject uiGameObject, bool showDebug = false)
    {
        // 检查是否已存在
        foreach (var mapping in itemUIMappings)
        {
            if (mapping.itemName == itemName)
            {
                mapping.uiGameObject = uiGameObject;
                mapping.showDebugInfo = showDebug;
                Debug.Log($"[Player] 更新物品UI映射: {itemName} -> {uiGameObject.name}");
                return;
            }
        }
        
        // 添加新的映射
        ItemUIMapping newMapping = new ItemUIMapping
        {
            itemName = itemName,
            uiGameObject = uiGameObject,
            showDebugInfo = showDebug
        };
        
        itemUIMappings.Add(newMapping);
        Debug.Log($"[Player] 添加物品UI映射: {itemName} -> {uiGameObject.name}");
    }
    
    /// <summary>
    /// 从列表中移除物品UI映射
    /// </summary>
    /// <param name="itemName">物品名称</param>
    public void RemoveItemUIMappingFromList(string itemName)
    {
        for (int i = itemUIMappings.Count - 1; i >= 0; i--)
        {
            if (itemUIMappings[i].itemName == itemName)
            {
                string uiName = itemUIMappings[i].uiGameObject != null ? itemUIMappings[i].uiGameObject.name : "null";
                itemUIMappings.RemoveAt(i);
                Debug.Log($"[Player] 移除物品UI映射: {itemName} -> {uiName}");
                return;
            }
        }
        
        Debug.LogWarning($"[Player] 未找到物品 '{itemName}' 的UI映射");
    }
    
    /// <summary>
    /// 清空物品UI映射列表
    /// </summary>
    public void ClearItemUIMappings()
    {
        int count = itemUIMappings.Count;
        itemUIMappings.Clear();
        Debug.Log($"[Player] 清空物品UI映射列表，共移除 {count} 个映射");
    }
    
    /// <summary>
    /// 获取物品UI映射列表信息
    /// </summary>
    /// <returns>映射信息字符串</returns>
    public string GetItemUIMappingListInfo()
    {
        string info = $"[Player] 物品UI映射列表信息:\n";
        info += $"总映射数: {itemUIMappings.Count}\n";
        
        if (itemUIMappings.Count == 0)
        {
            info += "映射列表为空\n";
        }
        else
        {
            for (int i = 0; i < itemUIMappings.Count; i++)
            {
                var mapping = itemUIMappings[i];
                string uiName = mapping.uiGameObject != null ? mapping.uiGameObject.name : "null";
                string debugStatus = mapping.showDebugInfo ? "调试开启" : "调试关闭";
                info += $"{i + 1}. {mapping.itemName} -> {uiName} ({debugStatus})\n";
            }
        }
        
        return info;
    }
    
    /// <summary>
    /// 显示物品UI映射列表信息
    /// </summary>
    public void ShowItemUIMappingListInfo()
    {
        Debug.Log(GetItemUIMappingListInfo());
    }
    
    /// <summary>
    /// 测试方法：帮助在Inspector中找到设置位置
    /// </summary>
    [ContextMenu("显示Inspector设置指导")]
    public void ShowInspectorSetupGuide()
    {
        Debug.Log("[Player] ===== Inspector设置指导 =====");
        Debug.Log("[Player] 1. 在Player组件的Inspector中找到'物品UI映射列表'部分");
        Debug.Log("[Player] 2. 设置'Item UIMappings'的Size为你需要的映射数量");
        Debug.Log("[Player] 3. 为每个Element设置:");
        Debug.Log("[Player]    - Item Name: 输入物品名称（如'项链'、'茶壶'）");
        Debug.Log("[Player]    - UI GameObject: 拖拽对应的UI GameObject");
        Debug.Log("[Player]    - Show Debug Info: 可选，开启调试信息");
        Debug.Log("[Player] =================================");
        
        Debug.Log($"[Player] 当前映射数量: {itemUIMappings.Count}");
        if (itemUIMappings.Count > 0)
        {
            Debug.Log("[Player] 当前映射列表:");
            for (int i = 0; i < itemUIMappings.Count; i++)
            {
                var mapping = itemUIMappings[i];
                string uiName = mapping.uiGameObject != null ? mapping.uiGameObject.name : "null";
                Debug.Log($"[Player]   {i}: {mapping.itemName} -> {uiName}");
            }
        }
        else
        {
            Debug.LogWarning("[Player] 映射列表为空，请在Inspector中添加映射");
        }
    }
    
    /// <summary>
    /// 测试方法：添加示例映射
    /// </summary>
    [ContextMenu("添加示例映射")]
    public void AddExampleMappings()
    {
        Debug.Log("[Player] 添加示例映射...");
        
        // 注意：这些是示例，你需要用实际的GameObject替换
        AddItemUIMappingToList("项链", null, true);
        AddItemUIMappingToList("茶壶", null, true);
        AddItemUIMappingToList("植物", null, true);
        AddItemUIMappingToList("猫", null, true);
        
        Debug.Log("[Player] 示例映射已添加，请在Inspector中设置对应的UI GameObject");
        ShowItemUIMappingListInfo();
    }
    
    /// <summary>
    /// 启用或禁用背包检测
    /// </summary>
    /// <param name="enable">是否启用</param>
    public void SetInventoryCheckEnabled(bool enable)
    {
        enableInventoryCheck = enable;
        Debug.Log($"[Player] 背包检测: {(enable ? "启用" : "禁用")}");
    }
    
    /// <summary>
    /// 获取当前背包状态
    /// </summary>
    /// <returns>是否有物品</returns>
    public bool HasItem()
    {
        return currentItem != null;
    }
    
    /// <summary>
    /// 获取当前物品信息
    /// </summary>
    /// <returns>物品信息字符串</returns>
    public string GetCurrentItemInfo()
    {
        if (currentItem != null)
        {
            return $"当前物品: {currentItem.name}";
        }
        else
        {
            return "背包为空";
        }
    }
    
    /// <summary>
    /// 获取所有物品UI映射信息
    /// </summary>
    /// <returns>映射信息字符串</returns>
    public string GetItemUIMappingInfo()
    {
        string info = "物品UI映射:\n";
        foreach (var mapping in itemUIMappings)
        {
            string uiName = mapping.uiGameObject != null ? mapping.uiGameObject.name : "null";
            info += $"- {mapping.itemName} -> {uiName}\n";
        }
        return info;
    }
    
    /// <summary>
    /// 启动健康值时间流逝
    /// </summary>
    public void StartHealthDecay()
    {
        if (healthDecayCoroutine != null)
        {
            StopCoroutine(healthDecayCoroutine);
        }
        
        healthDecayCoroutine = StartCoroutine(HealthDecayCoroutine());
        Debug.Log($"[Player] 启动健康值时间流逝 - 每{decayInterval}秒减少{decayAmount}点健康值");
    }
    
    /// <summary>
    /// 停止健康值时间流逝
    /// </summary>
    public void StopHealthDecay()
    {
        if (healthDecayCoroutine != null)
        {
            StopCoroutine(healthDecayCoroutine);
            healthDecayCoroutine = null;
            Debug.Log("[Player] 停止健康值时间流逝");
        }
    }
    
    /// <summary>
    /// 健康值时间流逝协程
    /// </summary>
    private IEnumerator HealthDecayCoroutine()
    {
        while (enableTimeDecay && currentHealth > 0)
        {
            yield return new WaitForSeconds(decayInterval);
            
            // 检查当前场景，在Gallery和Shop中停止时间流逝
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentScene == "Gallery" || currentScene == "Shop" || currentScene == "MainMenu")
            {
                Debug.Log($"[Player] 在{currentScene}场景中，时间流逝暂停");
                continue; // 跳过这次时间流逝
            }

            if (!GameDataManager.Instance.itemStates.Values.Any(value => value == PickableItem.ItemStateType.Solved))
            {
                continue;
            }

            // 减少健康值
            int oldHealth = currentHealth;
            currentHealth = Mathf.Max(currentHealth - decayAmount, 0);
            
            // 立即同步数据到GameDataManager
            SyncToGameDataManager();
            
            // 发送提示信息
            string message = $"时间流逝：健康值减少 {decayAmount} 点 ({oldHealth} -> {currentHealth})";
            Debug.Log($"[Player] {message}");
            
            // 检查健康阶段变化
            Player.HealthStage oldStage = GetHealthStage();
            Player.HealthStage newStage = GetHealthStage();
            
            if (oldStage != newStage)
            {
                string stageMessage = $"健康状态变化：{GetHealthStageName(oldStage)} -> {GetHealthStageName(newStage)}";
                Debug.Log($"[Player] {stageMessage}");
                
                // 通知PlayerController更新移动速度
                PlayerController playerController = GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.ForceUpdateSpeed();
                }
            }
            
            // 检查是否死亡
            if (currentHealth <= 0)
            {
                Debug.Log("[Player] 健康值归零，角色状态恶化");
                
                // 触发游戏结束
                if (GameEndManager.Instance != null)
                {
                    GameEndManager.Instance.EndGame(GameEndManager.GameEndReason.HealthZero);
                }
                else
                {
                    Debug.LogWarning("[Player] GameEndManager实例未找到，无法触发游戏结束");
                }
                
                break;
            }
        }
    }
    
    /// <summary>
    /// 设置生命值
    /// </summary>
    /// <param name="newHealth">新的生命值</param>
    public void SetHealth(int newHealth)
    {
        int oldHealth = currentHealth;
        Player.HealthStage oldStage = GetHealthStage();
        
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        
        // 检查健康阶段是否发生变化
        Player.HealthStage newStage = GetHealthStage();
        if (oldStage != newStage)
        {
            // 通知PlayerController更新移动速度
            PlayerController playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ForceUpdateSpeed();
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[Player] 健康阶段变化: {oldStage} -> {newStage}");
            }
        }
        
        // 立即同步数据到GameDataManager
        SyncToGameDataManager();
        
        // 检查生命值是否归零
        if (currentHealth <= 0 && oldHealth > 0)
        {
            Debug.Log("[Player] 生命值归零，触发游戏结束");
            
            // 触发游戏结束
            if (GameEndManager.Instance != null)
            {
                GameEndManager.Instance.EndGame(GameEndManager.GameEndReason.HealthZero);
            }
            else
            {
                Debug.LogWarning("[Player] GameEndManager实例未找到，无法触发游戏结束");
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[Player] 设置生命值: {oldHealth} -> {currentHealth}");
        }
    }
    
    /// <summary>
    /// 设置最大生命值
    /// </summary>
    /// <param name="newMaxHealth">新的最大生命值</param>
    public void SetMaxHealth(int newMaxHealth)
    {
        if (newMaxHealth <= 0)
        {
            Debug.LogError("[Player] 最大生命值必须大于0");
            return;
        }
        
        int oldMaxHealth = maxHealth;
        maxHealth = newMaxHealth;
        
        // 调整当前生命值
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        // 立即同步数据到GameDataManager
        SyncToGameDataManager();
        
        if (showDebugInfo)
        {
            Debug.Log($"[Player] 设置最大生命值: {oldMaxHealth} -> {maxHealth}");
        }
    }
    
    /// <summary>
    /// 根据当前健康值获取健康阶段
    /// </summary>
    /// <returns>健康阶段</returns>
    public HealthStage GetHealthStage()
    {
        float percentage = HealthPercentage;
        
        if (percentage >= 0.67f) // 67%以上为阶段1
        {
            return HealthStage.Stage1;
        }
        else if (percentage >= 0.34f) // 34%-66%为阶段2
        {
            return HealthStage.Stage2;
        }
        else // 33%以下为阶段3
        {
            return HealthStage.Stage3;
        }
    }
    
    /// <summary>
    /// 获取指定健康阶段的名称
    /// </summary>
    /// <param name="stage">健康阶段</param>
    /// <returns>健康阶段名称</returns>
    public string GetHealthStageName(HealthStage stage)
    {
        switch (stage)
        {
            case HealthStage.Stage1:
                return "健康阶段1";
            case HealthStage.Stage2:
                return "健康阶段2";
            case HealthStage.Stage3:
                return "健康阶段3";
            default:
                return "未知阶段";
        }
    }

    public int GetHealthStageNumber()
    {
        float percentage = HealthPercentage;
        
        if (percentage >= 0.6f) // 60%以上为阶段1
        {
            return 1;
        }
        else if (percentage >= 0.25f) // 25%-59%为阶段2
        {
            return 2;
        }
        else // 24%以下为阶段3
        {
            return 3;
        }
    }
    
    /// <summary>
    /// 获取健康阶段名称
    /// </summary>
    /// <returns>健康阶段名称</returns>
    public string GetHealthStageName()
    {
        return GetHealthStageName(GetHealthStage());
    }
    
    /// <summary>
    /// 获取生命值信息字符串
    /// </summary>
    /// <returns>生命值信息</returns>
    public string GetHealthInfo()
    {
        string info = $"生命值: {currentHealth}/{maxHealth}\n";
        info += $"百分比: {HealthPercentage:P1}\n";
        info += $"健康阶段: {GetHealthStageName()}";
        
        if (enableTimeDecay)
        {
            info += $"\n时间流逝: 每{decayInterval}秒减少{decayAmount}点";
        }
        
        return info;
    }
    
    /// <summary>
    /// 在控制台显示生命值信息
    /// </summary>
    public void ShowHealthInfo()
    {
        Debug.Log(GetHealthInfo());
    }
    
    /// <summary>
    /// 设置时间流逝参数
    /// </summary>
    /// <param name="interval">间隔时间（秒）</param>
    /// <param name="amount">减少量</param>
    public void SetHealthDecay(float interval, int amount)
    {
        decayInterval = interval;
        decayAmount = amount;
        
        // 重新启动时间流逝
        if (enableTimeDecay)
        {
            StartHealthDecay();
        }
        
        Debug.Log($"[Player] 设置时间流逝参数: 每{interval}秒减少{amount}点健康值");
    }
    
    /// <summary>
    /// 设置时间流逝启用状态
    /// </summary>
    /// <param name="enable">是否启用</param>
    public void SetTimeDecayEnabled(bool enable)
    {
        enableTimeDecay = enable;
        
        if (enable)
        {
            StartHealthDecay();
        }
        else
        {
            StopHealthDecay();
        }
        
        Debug.Log($"[Player] 时间流逝状态设置为: {(enable ? "启用" : "禁用")}");
    }
    
    /// <summary>
    /// 更新健康阶段
    /// </summary>
    public void UpdateHealthStage()
    {
        HealthStage currentStage = GetHealthStage();
        string stageName = GetHealthStageName(currentStage);
        
        // 通知PlayerController更新移动速度
        if (PlayerController.Instance != null)
        {
        //    PlayerController.Instance.UpdateSpeedBasedOnHealthStage(currentStage);
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[Player] 健康阶段更新: {stageName} (生命值: {currentHealth}/{maxHealth})");
        }
    }
    
    /// <summary>
    /// 更新物品UI映射
    /// </summary>
    public void UpdateItemUIMappings()
    {
        // 验证所有UI映射
        ValidateItemUIMappings();
        
        // 更新当前物品的UI显示
        UpdateInventoryUI();
        
        if (showDebugInfo)
        {
            Debug.Log($"[Player] 物品UI映射更新完成，当前物品: {(currentItem != null ? currentItem.name : "无")}");
        }
    }
    
    /// <summary>
    /// 强制刷新所有UI状态
    /// </summary>
    public void ForceRefreshUI()
    {
        // 更新健康阶段
        UpdateHealthStage();
        
        // 更新物品UI映射
        UpdateItemUIMappings();
        
        // 更新背包UI
        UpdateInventoryUI();
        
        if (showDebugInfo)
        {
            Debug.Log("[Player] 强制刷新所有UI状态完成");
        }
    }
    
    /// <summary>
    /// 场景切换时的数据同步
    /// </summary>
    public void OnSceneChanged()
    {
        Debug.Log("[Player] 场景切换，开始同步数据...");
        
        // 同步GameDataManager数据
        SyncWithGameDataManager();
        
        // 更新健康阶段
        UpdateHealthStage();
        
        // 更新物品UI映射
        UpdateItemUIMappings();
        
        // 重新启动健康值时间流逝（如果启用）
        if (enableTimeDecay)
        {
            StartHealthDecay();
        }
        
        Debug.Log("[Player] 场景切换数据同步完成");
    }
} 