using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 场景边界管理器
/// 负责管理不同场景的移动边界配置
/// </summary>
public class SceneBoundsManager : MonoBehaviour
{
    public static SceneBoundsManager Instance { get; private set; }
    
    [System.Serializable]
    public class SceneBounds
    {
        [Header("场景设置")]
        public string sceneName;           // 场景名称
        [Header("边界设置")]
        public Vector2 minBounds;          // 最小边界
        public Vector2 maxBounds;          // 最大边界
        [Header("调试")]
        public bool showDebugInfo = false; // 是否显示调试信息
    }
    
    [Header("场景边界配置")]
    [SerializeField] private List<SceneBounds> sceneBoundsList = new List<SceneBounds>();
    
    [Header("默认边界设置")]
    [SerializeField] private Vector2 defaultMinBounds = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 10f);
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    private void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[SceneBoundsManager] 场景边界管理器初始化完成");
            
            // 初始化默认场景边界
            InitializeDefaultSceneBounds();
        }
        else
        {
            Debug.Log("[SceneBoundsManager] 检测到重复的SceneBoundsManager实例，销毁当前实例");
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 验证场景边界配置
        ValidateSceneBounds();
        
        // 显示当前场景边界信息
        ShowCurrentSceneBounds();
    }
    
    /// <summary>
    /// 初始化默认场景边界
    /// </summary>
    private void InitializeDefaultSceneBounds()
    {
        // 如果列表为空，添加默认的场景边界
        if (sceneBoundsList.Count == 0)
        {
            AddSceneBounds("Home", new Vector2(-8f, -6f), new Vector2(8f, 6f));
            AddSceneBounds("Gallery", new Vector2(-12f, -8f), new Vector2(12f, 8f));
            AddSceneBounds("Shop", new Vector2(-6f, -4f), new Vector2(6f, 4f));
            AddSceneBounds("MainMenu", new Vector2(-5f, -3f), new Vector2(5f, 3f));
            
            Debug.Log("[SceneBoundsManager] 默认场景边界已初始化");
        }
    }
    
    /// <summary>
    /// 获取指定场景的移动边界
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <returns>边界信息</returns>
    public (Vector2 min, Vector2 max) GetSceneBounds(string sceneName)
    {
        foreach (var sceneBounds in sceneBoundsList)
        {
            if (sceneBounds.sceneName == sceneName)
            {
                if (showDebugInfo || sceneBounds.showDebugInfo)
                {
                    Debug.Log($"[SceneBoundsManager] 获取场景 '{sceneName}' 边界: {sceneBounds.minBounds} -> {sceneBounds.maxBounds}");
                }
                return (sceneBounds.minBounds, sceneBounds.maxBounds);
            }
        }
        
        // 如果没有找到指定场景的边界，返回默认边界
        if (showDebugInfo)
        {
            Debug.LogWarning($"[SceneBoundsManager] 未找到场景 '{sceneName}' 的边界配置，使用默认边界");
        }
        return (defaultMinBounds, maxBounds);
    }
    
    /// <summary>
    /// 获取当前场景的移动边界
    /// </summary>
    /// <returns>边界信息</returns>
    public (Vector2 min, Vector2 max) GetCurrentSceneBounds()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        return GetSceneBounds(currentScene);
    }
    
    /// <summary>
    /// 添加场景边界配置
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="minBounds">最小边界</param>
    /// <param name="maxBounds">最大边界</param>
    /// <param name="showDebug">是否显示调试信息</param>
    public void AddSceneBounds(string sceneName, Vector2 minBounds, Vector2 maxBounds, bool showDebug = false)
    {
        // 检查是否已存在
        for (int i = 0; i < sceneBoundsList.Count; i++)
        {
            if (sceneBoundsList[i].sceneName == sceneName)
            {
                // 更新现有配置
                sceneBoundsList[i].minBounds = minBounds;
                sceneBoundsList[i].maxBounds = maxBounds;
                sceneBoundsList[i].showDebugInfo = showDebug;
                
                Debug.Log($"[SceneBoundsManager] 更新场景边界: {sceneName} -> {minBounds} ~ {maxBounds}");
                return;
            }
        }
        
        // 添加新配置
        SceneBounds newBounds = new SceneBounds
        {
            sceneName = sceneName,
            minBounds = minBounds,
            maxBounds = maxBounds,
            showDebugInfo = showDebug
        };
        
        sceneBoundsList.Add(newBounds);
        Debug.Log($"[SceneBoundsManager] 添加场景边界: {sceneName} -> {minBounds} ~ {maxBounds}");
    }
    
    /// <summary>
    /// 移除场景边界配置
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public void RemoveSceneBounds(string sceneName)
    {
        for (int i = sceneBoundsList.Count - 1; i >= 0; i--)
        {
            if (sceneBoundsList[i].sceneName == sceneName)
            {
                sceneBoundsList.RemoveAt(i);
                Debug.Log($"[SceneBoundsManager] 移除场景边界: {sceneName}");
                return;
            }
        }
        
        Debug.LogWarning($"[SceneBoundsManager] 未找到场景 '{sceneName}' 的边界配置");
    }
    
    /// <summary>
    /// 设置默认边界
    /// </summary>
    /// <param name="minBounds">最小边界</param>
    /// <param name="maxBounds">最大边界</param>
    public void SetDefaultBounds(Vector2 minBounds, Vector2 maxBounds)
    {
        defaultMinBounds = minBounds;
        this.maxBounds = maxBounds;
        
        Debug.Log($"[SceneBoundsManager] 设置默认边界: {minBounds} -> {maxBounds}");
    }
    
    /// <summary>
    /// 验证场景边界配置
    /// </summary>
    public void ValidateSceneBounds()
    {
        Debug.Log("[SceneBoundsManager] 开始验证场景边界配置...");
        
        if (sceneBoundsList.Count == 0)
        {
            Debug.LogWarning("[SceneBoundsManager] 场景边界列表为空");
            return;
        }
        
        for (int i = 0; i < sceneBoundsList.Count; i++)
        {
            var sceneBounds = sceneBoundsList[i];
            
            // 检查场景名称
            if (string.IsNullOrEmpty(sceneBounds.sceneName))
            {
                Debug.LogError($"[SceneBoundsManager] 边界配置 {i}: 场景名称为空");
                continue;
            }
            
            // 检查边界有效性
            if (sceneBounds.minBounds.x > sceneBounds.maxBounds.x || 
                sceneBounds.minBounds.y > sceneBounds.maxBounds.y)
            {
                Debug.LogError($"[SceneBoundsManager] 边界配置 {i}: 场景 '{sceneBounds.sceneName}' 边界无效 (最小边界大于最大边界)");
                continue;
            }
            
            Debug.Log($"[SceneBoundsManager] 边界配置 {i}: {sceneBounds.sceneName} -> {sceneBounds.minBounds} ~ {sceneBounds.maxBounds} ✓");
        }
        
        Debug.Log($"[SceneBoundsManager] 验证完成，共 {sceneBoundsList.Count} 个场景边界配置");
    }
    
    /// <summary>
    /// 显示当前场景边界信息
    /// </summary>
    public void ShowCurrentSceneBounds()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var (min, max) = GetCurrentSceneBounds();
        
        Debug.Log($"[SceneBoundsManager] 当前场景 '{currentScene}' 边界: {min} -> {max}");
    }
    
    /// <summary>
    /// 获取所有场景边界信息
    /// </summary>
    /// <returns>边界信息字符串</returns>
    public string GetAllSceneBoundsInfo()
    {
        string info = "[SceneBoundsManager] 所有场景边界信息:\n";
        info += $"总配置数: {sceneBoundsList.Count}\n";
        info += $"默认边界: {defaultMinBounds} -> {maxBounds}\n\n";
        
        if (sceneBoundsList.Count == 0)
        {
            info += "场景边界列表为空\n";
        }
        else
        {
            for (int i = 0; i < sceneBoundsList.Count; i++)
            {
                var sceneBounds = sceneBoundsList[i];
                string debugStatus = sceneBounds.showDebugInfo ? "调试开启" : "调试关闭";
                info += $"{i + 1}. {sceneBounds.sceneName}: {sceneBounds.minBounds} -> {sceneBounds.maxBounds} ({debugStatus})\n";
            }
        }
        
        return info;
    }
    
    /// <summary>
    /// 显示所有场景边界信息
    /// </summary>
    public void ShowAllSceneBoundsInfo()
    {
        Debug.Log(GetAllSceneBoundsInfo());
    }
    
    /// <summary>
    /// 清空所有场景边界配置
    /// </summary>
    public void ClearAllSceneBounds()
    {
        int count = sceneBoundsList.Count;
        sceneBoundsList.Clear();
        Debug.Log($"[SceneBoundsManager] 清空所有场景边界配置，共移除 {count} 个配置");
    }
    
    /// <summary>
    /// 场景切换时的处理
    /// </summary>
    public void OnSceneChanged()
    {
        ShowCurrentSceneBounds();
        
        // 通知PlayerController更新边界
        if (PlayerController.Instance != null)
        {
            var (min, max) = GetCurrentSceneBounds();
            PlayerController.Instance.SetMovementBounds(min, max);
        }
    }
    
    /// <summary>
    /// 测试方法：添加示例场景边界
    /// </summary>
    [ContextMenu("添加示例场景边界")]
    public void AddExampleSceneBounds()
    {
        Debug.Log("[SceneBoundsManager] 添加示例场景边界...");
        
        AddSceneBounds("Home", new Vector2(-8f, -6f), new Vector2(8f, 6f), true);
        AddSceneBounds("Gallery", new Vector2(-12f, -8f), new Vector2(12f, 8f), true);
        AddSceneBounds("Shop", new Vector2(-6f, -4f), new Vector2(6f, 4f), true);
        AddSceneBounds("MainMenu", new Vector2(-5f, -3f), new Vector2(5f, 3f), true);
        AddSceneBounds("TestScene", new Vector2(-15f, -10f), new Vector2(15f, 10f), true);
        
        Debug.Log("[SceneBoundsManager] 示例场景边界已添加");
        ShowAllSceneBoundsInfo();
    }
    
    /// <summary>
    /// 测试方法：验证所有场景边界
    /// </summary>
    [ContextMenu("验证所有场景边界")]
    public void ValidateAllSceneBounds()
    {
        ValidateSceneBounds();
    }
} 