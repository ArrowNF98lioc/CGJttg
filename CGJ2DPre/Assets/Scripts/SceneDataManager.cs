using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 场景数据管理器
/// 负责在场景切换时同步所有游戏数据
/// </summary>
public class SceneDataManager : MonoBehaviour
{
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    // 单例模式
    public static SceneDataManager Instance { get; private set; }
    
    // 场景切换状态
    private bool isSceneTransitioning = false;
    private string currentSceneName = "";
    
    private void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[SceneDataManager] 场景数据管理器初始化完成");
        }
        else
        {
            Debug.Log("[SceneDataManager] 检测到重复的SceneDataManager实例，销毁当前实例");
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"[SceneDataManager] 当前场景: {currentSceneName}");
        
        // 订阅场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    
    /// <summary>
    /// 场景加载完成时的回调
    /// </summary>
    /// <param name="scene">加载的场景</param>
    /// <param name="mode">加载模式</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        Debug.Log($"[SceneDataManager] 场景加载完成: {currentSceneName}");
        
        // 延迟执行数据同步，确保所有组件都已初始化
        StartCoroutine(SyncDataAfterSceneLoad());
    }
    
    /// <summary>
    /// 场景卸载时的回调
    /// </summary>
    /// <param name="scene">卸载的场景</param>
    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"[SceneDataManager] 场景卸载: {scene.name}");
        
        // 保存当前场景的数据
        SaveCurrentSceneData();
    }
    
    /// <summary>
    /// 场景加载后同步数据
    /// </summary>
    private IEnumerator SyncDataAfterSceneLoad()
    {
        // 等待一帧，确保所有组件都已初始化
        yield return null;
        
        Debug.Log("[SceneDataManager] 开始同步场景数据...");
        
        // 同步所有游戏数据
        SyncAllGameData();
        
        // 更新UI显示
        UpdateAllUI();
        
        Debug.Log("[SceneDataManager] 场景数据同步完成");
    }
    
    /// <summary>
    /// 同步所有游戏数据
    /// </summary>
    public void SyncAllGameData()
    {
        if (GameDataManager.Instance != null)
        {
            // 使用GameDataManager同步所有数据
            GameDataManager.Instance.SyncAllGameData();
        }
        else
        {
            Debug.LogWarning("[SceneDataManager] GameDataManager实例未找到");
        }
        
        if (showDebugInfo)
        {
            Debug.Log("[SceneDataManager] 所有游戏数据同步完成");
        }
    }
    
    /// <summary>
    /// 更新所有UI显示
    /// </summary>
    public void UpdateAllUI()
    {
        // 更新Player的UI映射
        if (Player.Instance != null)
        {
            Player.Instance.ForceRefreshUI();
        }
        
        // 更新背包UI
        InventoryUIController uiController = FindObjectOfType<InventoryUIController>();
        if (uiController != null)
        {
            uiController.RefreshInventoryDisplay();
        }
        
        // 更新血槽UI（如果有的话）
        UpdateHealthUI();
        
        if (showDebugInfo)
        {
            Debug.Log("[SceneDataManager] 所有UI更新完成");
        }
    }
    
    /// <summary>
    /// 更新血槽UI
    /// </summary>
    private void UpdateHealthUI()
    {
        // 查找血槽相关的UI组件
        // 这里可以根据你的血槽UI实现来添加具体的更新逻辑
        
        if (Player.Instance != null)
        {
            // 示例：更新血槽显示
            float healthPercentage = Player.Instance.HealthPercentage;
            string healthStage = Player.Instance.GetHealthStageName();
            
            if (showDebugInfo)
            {
                Debug.Log($"[SceneDataManager] 血槽状态: {healthPercentage:P0} - {healthStage}");
            }
        }
    }
    
    /// <summary>
    /// 保存当前场景的数据
    /// </summary>
    public void SaveCurrentSceneData()
    {
        if (GameDataManager.Instance != null)
        {
            // 同步当前数据到GameDataManager
            GameDataManager.Instance.SyncPlayerData();
            GameDataManager.Instance.SyncInventoryData();
            
            if (showDebugInfo)
            {
                Debug.Log($"[SceneDataManager] 场景 {currentSceneName} 数据已保存");
            }
        }
    }
    
    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    public void LoadScene(string sceneName)
    {
        if (isSceneTransitioning)
        {
            Debug.LogWarning("[SceneDataManager] 场景切换中，请等待当前切换完成");
            return;
        }
        
        Debug.Log($"[SceneDataManager] 开始切换场景: {sceneName}");
        
        // 保存当前场景数据
        SaveCurrentSceneData();
        
        // 设置切换状态
        isSceneTransitioning = true;
        
        // 加载新场景
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    
    /// <summary>
    /// 场景加载协程
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // 异步加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // 重置切换状态
        isSceneTransitioning = false;
        
        Debug.Log($"[SceneDataManager] 场景切换完成: {sceneName}");
    }
    
    /// <summary>
    /// 切换场景（通过索引）
    /// </summary>
    /// <param name="sceneIndex">目标场景索引</param>
    public void LoadScene(int sceneIndex)
    {
        if (isSceneTransitioning)
        {
            Debug.LogWarning("[SceneDataManager] 场景切换中，请等待当前切换完成");
            return;
        }
        
        Debug.Log($"[SceneDataManager] 开始切换场景: 索引 {sceneIndex}");
        
        // 保存当前场景数据
        SaveCurrentSceneData();
        
        // 设置切换状态
        isSceneTransitioning = true;
        
        // 加载新场景
        StartCoroutine(LoadSceneByIndexCoroutine(sceneIndex));
    }
    
    /// <summary>
    /// 通过索引加载场景的协程
    /// </summary>
    /// <param name="sceneIndex">场景索引</param>
    private IEnumerator LoadSceneByIndexCoroutine(int sceneIndex)
    {
        // 异步加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        
        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // 重置切换状态
        isSceneTransitioning = false;
        
        Debug.Log($"[SceneDataManager] 场景切换完成: 索引 {sceneIndex}");
    }
    
    /// <summary>
    /// 重新加载当前场景
    /// </summary>
    public void ReloadCurrentScene()
    {
        LoadScene(currentSceneName);
    }
    
    /// <summary>
    /// 获取当前场景名称
    /// </summary>
    /// <returns>当前场景名称</returns>
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
    
    /// <summary>
    /// 检查是否正在切换场景
    /// </summary>
    /// <returns>是否正在切换场景</returns>
    public bool IsSceneTransitioning()
    {
        return isSceneTransitioning;
    }
    
    /// <summary>
    /// 强制刷新所有数据
    /// </summary>
    public void ForceRefreshAllData()
    {
        Debug.Log("[SceneDataManager] 强制刷新所有数据...");
        
        // 同步所有游戏数据
        SyncAllGameData();
        
        // 更新所有UI
        UpdateAllUI();
        
        Debug.Log("[SceneDataManager] 强制刷新完成");
    }
    
    /// <summary>
    /// 显示当前场景状态
    /// </summary>
    public void ShowCurrentSceneStatus()
    {
        string status = $"[SceneDataManager] 当前场景状态:\n";
        status += $"场景名称: {currentSceneName}\n";
        status += $"切换状态: {(isSceneTransitioning ? "切换中" : "空闲")}\n";
        status += $"GameDataManager: {(GameDataManager.Instance != null ? "已连接" : "未连接")}\n";
        status += $"Player: {(Player.Instance != null ? "已连接" : "未连接")}\n";
        status += $"Inventory: {(Inventory.Instance != null ? "已连接" : "未连接")}";
        
        Debug.Log(status);
    }
    
    private void OnDestroy()
    {
        // 取消订阅场景事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
} 