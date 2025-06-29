using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 全局游戏数据管理器
/// 统一管理所有需要在场景间保持一致的游戏数据
/// 实现单例模式，确保数据持久化
/// </summary>
public class GameDataManager : MonoBehaviour
{
    // 单例模式
    public static GameDataManager Instance { get; private set; }
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    // 添加初始化标志，确保只在第一次初始化
    private static bool isInitialized = false;
    
    // 游戏状态数据
    [Header("游戏状态")]
    public bool isFirstTimePlaying = true;  // 是否首次游戏
    public int currentSceneIndex = 0;       // 当前场景索引
    public float totalPlayTime = 0f;        // 总游戏时间
    
    // 玩家数据
    [Header("玩家数据")]
    public int playerMaxHealth = 100;       // 玩家最大生命值
    public int playerCurrentHealth = 59;   // 玩家当前生命值
    public string playerCurrentItem = "";   // 玩家当前物品
    
    // 游戏进度数据
    [Header("游戏进度")]
    public List<string> collectedItems = new List<string>();  // 已收集的物品
    public List<string> unlockedScenes = new List<string>();  // 已解锁的场景
    public Dictionary<string, bool> gameFlags = new Dictionary<string, bool>();  // 游戏标记
    
    // 设置数据
    [Header("游戏设置")]
    public float masterVolume = 1f;         // 主音量
    public float musicVolume = 0.8f;       // 音乐音量
    public float sfxVolume = 0.9f;         // 音效音量
    public bool enableTimeDecay = true;     // 是否启用时间流逝
    public float timeDecayInterval = 6f;    // 时间流逝间隔
    public int timeDecayAmount = 2;         // 时间流逝数量

    // 物品状态数据
    [Header("物品状态")]
    public Dictionary<string, PickableItem.ItemStateType> itemStates = new Dictionary<string, PickableItem.ItemStateType>();  // 物品状态
    
    private void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameDataManager] 全局游戏数据管理器初始化完成");
            
            // 立即设置正确的初始值，避免被场景文件覆盖
            playerMaxHealth = 100;
            playerCurrentHealth = 59;
            playerCurrentItem = "";
            
            // 只在第一次初始化时调用，避免场景重新加载时重置数据
            if (!isInitialized)
            {
                InitializeDefaultData();
                isInitialized = true;
                Debug.Log("[GameDataManager] 首次初始化默认数据完成");
            }
            
            // 订阅场景加载事件
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Debug.Log("[GameDataManager] 检测到重复的GameDataManager实例，销毁当前实例");
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        Debug.Log("[GameDataManager] 游戏数据管理器启动完成");
        
        if (showDebugInfo)
        {
            ShowCurrentGameData();
        }
    }
    
    /// <summary>
    /// 场景加载完成时的回调
    /// </summary>
    /// <param name="scene">加载的场景</param>
    /// <param name="mode">加载模式</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameDataManager] 场景加载完成: {scene.name}");
        
        // 更新当前场景索引
        currentSceneIndex = scene.buildIndex;
        
        // 延迟一帧执行数据同步，确保所有组件都已初始化
        StartCoroutine(SyncDataOnSceneLoad());
    }
    
    /// <summary>
    /// 场景加载后同步数据
    /// </summary>
    private System.Collections.IEnumerator SyncDataOnSceneLoad()
    {
        // 等待一帧，确保所有组件都已初始化
        yield return null;
        
        Debug.Log("[GameDataManager] 开始同步场景数据...");
        
        // 同步所有游戏数据到各个组件
        SyncAllGameData();
        
        Debug.Log("[GameDataManager] 场景数据同步完成");
    }

    // void Update()
    // {
    //     SyncAllGameData();
    // }
    
    /// <summary>
    /// 同步所有游戏数据到各个组件
    /// </summary>
    public void SyncAllGameData()
    {
        // 同步Player数据
        SyncDataToPlayer();
        
        // 同步PlayerController数据
        SyncDataToPlayerController();

        // 同步物品状态到场景（包含SetItemSprite调用）
        SyncItemStatesToScene();

        // 同步Inventory数据, 暂时尝试不用
        // SyncDataToInventory();
        
        // 同步UI数据, 暂时尝试不用
        // SyncDataToUI();
        
        // 同步ItemManager数据, 暂时尝试不用
        // SyncDataToItemManager();
        
        if (showDebugInfo)
        {
            Debug.Log("[GameDataManager] 所有游戏数据同步完成");
        }
    }
    
    /// <summary>
    /// 同步数据到Player组件
    /// </summary>
    public void SyncDataToPlayer()
    {
        if (Player.Instance != null)
        {
            // 检查Player是否已经初始化完成
            // 如果Player还没有从GameDataManager加载过数据，则强制同步
            // 如果Player已经初始化，则只在必要时同步
            
            // 同步生命值 - 优先使用GameDataManager的数据
            Player.Instance.MaxHealth = playerMaxHealth;
            Player.Instance.CurrentHealth = playerCurrentHealth;
            
            Player.Instance.UpdateHealthStage();
            
            if (showDebugInfo)
            {
                Debug.Log($"[GameDataManager] 同步Player数据: 生命值={playerCurrentHealth}/{playerMaxHealth}, 物品={playerCurrentItem}");
            }
        }
        else
        {
            Debug.LogWarning("[GameDataManager] Player实例未找到，无法同步数据");
        }
    }

    /// <summary>
    /// 同步数据到PlayerController组件
    /// </summary> 
    public void SyncDataToPlayerController()
    {
        if (PlayerController.Instance != null)
        {
            // 同步生命值
            PlayerController.Instance.playerHealth.MaxHealth = playerMaxHealth;
            PlayerController.Instance.playerHealth.CurrentHealth = playerCurrentHealth;
            
            // 同步当前物品
            if (!string.IsNullOrEmpty(playerCurrentItem))
            {
                Item item = ItemManager.Instance?.GetItem(playerCurrentItem);
                if (item != null)
                {
                    PlayerController.Instance.playerHealth.currentItem = item;
                }
            }
            
            // 更新健康阶段
            PlayerController.Instance.playerHealth.UpdateHealthStage();
            
            if (showDebugInfo)
            {
                Debug.Log($"[GameDataManager] 同步PlayerController数据: 生命值={playerCurrentHealth}/{playerMaxHealth}, 物品={playerCurrentItem}");
            }
        }
        else
        {
            Debug.LogWarning("[GameDataManager] PlayerController实例未找到，无法同步数据");
        }
    }
    
    
    /// <summary>
    /// 同步数据到UI组件, 暂时尝试不用
    /// </summary>
    public void SyncDataToUI()
    {
        // 查找并更新背包UI控制器
        InventoryUIController uiController = FindObjectOfType<InventoryUIController>();
        if (uiController != null)
        {
            uiController.RefreshInventoryDisplay();
            uiController.UpdateInventoryInfo();
            
            if (showDebugInfo)
            {
                Debug.Log("[GameDataManager] 同步UI数据完成");
            }
        }
        
        // 查找并更新Player的UI映射
        Player player = Player.Instance;
        if (player != null)
        {
            player.UpdateItemUIMappings();
            
            if (showDebugInfo)
            {
                Debug.Log("[GameDataManager] 同步Player UI映射完成");
            }
        }
    }
    
    /// <summary>
    /// 同步数据到ItemManager组件, 暂时尝试不用
    /// </summary>
    public void SyncDataToItemManager()
    {
        if (ItemManager.Instance != null)
        {
            // 更新已收集物品的状态
            foreach (string itemName in collectedItems)
            {
                Item item = ItemManager.Instance.GetItem(itemName);
                if (item != null)
                {
                    // 标记物品为已收集
                    gameFlags[$"hasCollected{itemName}"] = true;
                }
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[GameDataManager] 同步ItemManager数据: 已收集物品={collectedItems.Count}个");
            }
        }
        else
        {
            Debug.LogWarning("[GameDataManager] ItemManager实例未找到，无法同步数据");
        }
    }
    
    /// <summary>
    /// 同步物品状态数据
    /// </summary>
    public void SyncDataToItemStates()
    {
        // 查找场景中所有的PickableItem
        PickableItem[] pickableItems = FindObjectsOfType<PickableItem>();
        
        foreach (PickableItem pickableItem in pickableItems)
        {
            if (pickableItem != null && !string.IsNullOrEmpty(pickableItem.itemName))
            {
                // 如果GameDataManager中有该物品的状态记录，则同步到场景
                if (itemStates.ContainsKey(pickableItem.itemName))
                {
                    pickableItem.currentState = itemStates[pickableItem.itemName];
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"[GameDataManager] 同步物品状态到场景: {pickableItem.itemName} = {itemStates[pickableItem.itemName]}");
                    }
                }
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[GameDataManager] 物品状态同步完成，共同步 {pickableItems.Length} 个物品");
        }
    }
    
    /// <summary>
    /// 将GameDataManager中的物品状态同步到场景中的PickableItem
    /// </summary>
    public void SyncItemStatesToScene()
    {
        // 查找场景中所有的PickableItem
        PickableItem[] pickableItems = FindObjectsOfType<PickableItem>();
        
        foreach (PickableItem pickableItem in pickableItems)
        {
            if (pickableItem != null && !string.IsNullOrEmpty(pickableItem.itemName))
            {
                // 如果GameDataManager中有该物品的状态记录，则同步到场景
                if (itemStates.ContainsKey(pickableItem.itemName))
                {
                    pickableItem.currentState = itemStates[pickableItem.itemName];
                    
                    // 检查PickableItem是否已经初始化完成
                    if (pickableItem.IsReady())
                    {
                        // 根据状态更新物品的显示
                        pickableItem.SetItemSprite();
                        
                        if (showDebugInfo)
                        {
                            Debug.Log($"[GameDataManager] 同步到场景物品状态: {pickableItem.itemName} = {pickableItem.currentState}");
                        }
                    }
                    else
                    {
                        if (showDebugInfo)
                        {
                            Debug.LogWarning($"[GameDataManager] PickableItem {pickableItem.itemName} 的UI GameObject未设置，跳过SetItemSprite调用");
                        }
                    }
                }
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[GameDataManager] 场景物品状态同步完成，共同步 {pickableItems.Length} 个物品");
        }
    }
    
    /// <summary>
    /// 更新特定物品的状态
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <param name="newState">新状态</param>
    public void UpdateItemState(string itemName, PickableItem.ItemStateType newState)
    {
        if (!string.IsNullOrEmpty(itemName))
        {
            // 如果要设置为Selected，先把其它物品的Selected取消
            if (newState == PickableItem.ItemStateType.Selected)
            {
                foreach (var key in itemStates.Keys)
                {
                    if (key != itemName && itemStates[key] == PickableItem.ItemStateType.Selected)
                    {
                        itemStates[key] = PickableItem.ItemStateType.AtHome; // 或其它非Selected状态
                    }
                }
            }
            itemStates[itemName] = newState;
            
            // 立即同步到场景中的PickableItem
            SyncItemStatesToScene();
            
            // 如果物品状态变为Solved，检查是否所有物品都被当掉
            if (newState == PickableItem.ItemStateType.Solved)
            {
                CheckAllItemsSold();
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[GameDataManager] 更新物品状态: {itemName} = {newState}");
            }
        }
    }
    
    /// <summary>
    /// 检查是否所有物品都被当掉
    /// </summary>
    private void CheckAllItemsSold()
    {
        int totalItems = 0;
        int soldItems = 0;
        
        foreach (var itemState in itemStates)
        {
            totalItems++;
            if (itemState.Value == PickableItem.ItemStateType.Solved)
            {
                soldItems++;
            }
        }
        
        // 如果所有物品都被当掉（状态为Solved）
        if (totalItems > 0 && soldItems == totalItems)
        {
            Debug.Log("[GameDataManager] 所有物品都被当掉，触发游戏结束");
            
            // 触发游戏结束
            if (GameEndManager.Instance != null)
            {
                GameEndManager.Instance.EndGame(GameEndManager.GameEndReason.AllItemsSold);
            }
            else
            {
                Debug.LogWarning("[GameDataManager] GameEndManager实例未找到，无法触发游戏结束");
            }
        }
    }
    
    /// <summary>
    /// 获取特定物品的状态
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>物品状态，如果不存在则返回AtHome</returns>
    public PickableItem.ItemStateType GetItemState(string itemName)
    {
        if (!string.IsNullOrEmpty(itemName) && itemStates.ContainsKey(itemName))
        {
            return itemStates[itemName];
        }
        return PickableItem.ItemStateType.AtHome; // 默认状态
    }
    
    /// <summary>
    /// 检查是否有其他物品处于Selected状态
    /// </summary>
    /// <param name="excludeItemName">要排除的物品名称（通常是当前物品）</param>
    /// <returns>是否有其他物品处于Selected状态</returns>
    public bool HasOtherItemSelected(string excludeItemName = "")
    {
        foreach (var itemState in itemStates)
        {
            if (itemState.Key != excludeItemName && itemState.Value == PickableItem.ItemStateType.Selected)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 获取当前Selected状态的物品名称
    /// </summary>
    /// <returns>Selected状态的物品名称，如果没有则返回空字符串</returns>
    public string GetSelectedItemName()
    {
        foreach (var itemState in itemStates)
        {
            if (itemState.Value == PickableItem.ItemStateType.Selected)
            {
                return itemState.Key;
            }
        }
        return "";
    }
    
    /// <summary>
    /// 初始化默认数据
    /// </summary>
    private void InitializeDefaultData()
    {
        // 初始化游戏标记
        gameFlags["hasVisitedHome"] = false;
        gameFlags["hasVisitedShop"] = false;
        gameFlags["hasVisitedGallery"] = false;
        gameFlags["hasCollectedNecklace"] = false;
        gameFlags["hasCollectedBottle"] = false;
        gameFlags["hasCollectedPlant"] = false;
        gameFlags["hasCollectedCat"] = false;
        gameFlags["hasCollectedBird"] = false;
        gameFlags["hasCollectedDress"] = false;
        gameFlags["hasCollectedOscar"] = false;
        gameFlags["hasCollectedDiary"] = false;
        
        // 初始化物品状态
        itemStates["Necklace"] = PickableItem.ItemStateType.AtHome;
        itemStates["Bottle"] = PickableItem.ItemStateType.AtHome;
        itemStates["Plant"] = PickableItem.ItemStateType.AtHome;
        itemStates["Cat"] = PickableItem.ItemStateType.AtHome;
        itemStates["Bird"] = PickableItem.ItemStateType.AtHome;
        itemStates["Dress"] = PickableItem.ItemStateType.AtHome;
        itemStates["Oscar"] = PickableItem.ItemStateType.AtHome;
        itemStates["Diary"] = PickableItem.ItemStateType.AtHome;
        
        // 初始化已解锁场景
        unlockedScenes.Add("MainMenu");
        unlockedScenes.Add("Home");
        unlockedScenes.Add("Gallery");
        unlockedScenes.Add("Shop");
        
        // 确保玩家数据使用正确的初始值
        playerMaxHealth = 100;
        playerCurrentHealth = 59;  // 强制设置为59，覆盖场景文件中的值
        playerCurrentItem = "";
        
        Debug.Log("[GameDataManager] 默认数据初始化完成");
    }
    
    /// <summary>
    /// 同步Player数据到GameDataManager
    /// </summary>
    public void SyncPlayerData()
    {
        if (Player.Instance != null)
        {
            playerMaxHealth = Player.Instance.MaxHealth;
            playerCurrentHealth = Player.Instance.CurrentHealth;
            playerCurrentItem = Player.Instance.currentItem != null ? Player.Instance.currentItem.name : "";
            
            if (showDebugInfo)
            {
                Debug.Log($"[GameDataManager] 同步Player数据: 生命值={playerCurrentHealth}/{playerMaxHealth}, 物品={playerCurrentItem}");
            }
        }
    }
    
    /// <summary>
    /// 同步Inventory数据到GameDataManager
    /// </summary>
    public void SyncInventoryData()
    {
        if (Inventory.Instance != null)
        {
            // 同步已收集的物品列表
            collectedItems.Clear();
            for (int i = 0; i < Inventory.Instance.CurrentItemCount; i++)
            {
                Item item = Inventory.Instance.GetItemAtSlot(i);
                if (item != null && !collectedItems.Contains(item.name))
                {
                    collectedItems.Add(item.name);
                }
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[GameDataManager] 同步Inventory数据: 物品数量={Inventory.Instance.CurrentItemCount}");
            }
        }
    }
    
    /// <summary>
    /// 添加收集的物品
    /// </summary>
    /// <param name="itemName">物品名称</param>
    public void AddCollectedItem(string itemName)
    {
        if (!string.IsNullOrEmpty(itemName) && !collectedItems.Contains(itemName))
        {
            collectedItems.Add(itemName);
            gameFlags[$"hasCollected{itemName}"] = true;
            
            Debug.Log($"[GameDataManager] 添加收集物品: {itemName}");
        }
    }
    
    /// <summary>
    /// 检查是否已收集指定物品
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>是否已收集</returns>
    public bool HasCollectedItem(string itemName)
    {
        return collectedItems.Contains(itemName);
    }
    
    /// <summary>
    /// 解锁场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public void UnlockScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName) && !unlockedScenes.Contains(sceneName))
        {
            unlockedScenes.Add(sceneName);
            Debug.Log($"[GameDataManager] 解锁场景: {sceneName}");
        }
    }
    
    /// <summary>
    /// 检查场景是否已解锁
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <returns>是否已解锁</returns>
    public bool IsSceneUnlocked(string sceneName)
    {
        return unlockedScenes.Contains(sceneName);
    }
    
    /// <summary>
    /// 设置游戏标记
    /// </summary>
    /// <param name="flagName">标记名称</param>
    /// <param name="value">标记值</param>
    public void SetGameFlag(string flagName, bool value)
    {
        gameFlags[flagName] = value;
        
        if (showDebugInfo)
        {
            Debug.Log($"[GameDataManager] 设置游戏标记: {flagName} = {value}");
        }
    }
    
    /// <summary>
    /// 获取游戏标记
    /// </summary>
    /// <param name="flagName">标记名称</param>
    /// <returns>标记值</returns>
    public bool GetGameFlag(string flagName)
    {
        return gameFlags.ContainsKey(flagName) && gameFlags[flagName];
    }
    
    /// <summary>
    /// 更新游戏时间
    /// </summary>
    /// <param name="deltaTime">时间增量</param>
    public void UpdatePlayTime(float deltaTime)
    {
        totalPlayTime += deltaTime;
    }
    
    /// <summary>
    /// 显示当前游戏数据
    /// </summary>
    public void ShowCurrentGameData()
    {
        string info = "[GameDataManager] 当前游戏数据:\n";
        info += $"总游戏时间: {totalPlayTime:F1}秒\n";
        info += $"当前场景: {currentSceneIndex}\n";
        info += $"玩家生命值: {playerCurrentHealth}/{playerMaxHealth}\n";
        info += $"当前物品: {playerCurrentItem}\n";
        info += $"已收集物品: {collectedItems.Count}个\n";
        info += $"已解锁场景: {unlockedScenes.Count}个\n";
        info += $"游戏标记: {gameFlags.Count}个";
        
        Debug.Log(info);
    }
    
    /// <summary>
    /// 重置所有游戏数据
    /// </summary>
    public void ResetAllGameData()
    {
        Debug.Log("[GameDataManager] 重置所有游戏数据");
        
        // 重置游戏状态
        isFirstTimePlaying = true;
        currentSceneIndex = 0;
        totalPlayTime = 0f;
        
        // 重置玩家数据
        playerMaxHealth = 100;
        playerCurrentHealth = 59;
        playerCurrentItem = "";
        
        // 重置游戏进度
        collectedItems.Clear();
        unlockedScenes.Clear();
        gameFlags.Clear();
        
        // 重置初始化标志，允许重新初始化
        isInitialized = false;
        
        // 重新初始化默认数据
        InitializeDefaultData();
        isInitialized = true;
        
        Debug.Log("[GameDataManager] 游戏数据重置完成");
    }
    
    /// <summary>
    /// 保存游戏数据（可以扩展为实际的文件保存）
    /// </summary>
    public void SaveGameData()
    {
        // 同步当前数据
        SyncPlayerData();
        // SyncInventoryData();
        
        Debug.Log("[GameDataManager] 游戏数据已保存");
        
        if (showDebugInfo)
        {
            ShowCurrentGameData();
        }
    }
    
    // / <summary>
    // / 加载游戏数据（可以扩展为实际的文件加载）
    // / 你妈的跟本没用？？？
    // / </summary>
    public void LoadGameData()
    {
        Debug.Log("[GameDataManager] 游戏数据已加载");
        
        // 同步数据到各个组件
        SyncAllGameData();
        
        if (showDebugInfo)
        {
            ShowCurrentGameData();
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅场景加载事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
} 