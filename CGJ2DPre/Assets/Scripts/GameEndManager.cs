using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 游戏结束管理器
/// 负责检测和处理游戏结束的各种条件
/// </summary>
public class GameEndManager : MonoBehaviour
{
    [Header("游戏结束条件")]
    [SerializeField] private bool enableHealthCheck = true;        // 是否启用生命值检查
    [SerializeField] private bool enableItemCheck = true;          // 是否启用物品检查
    [SerializeField] private bool enableGiveUpCheck = true;        // 是否启用放弃检查
    
    [Header("UI设置")]
    [SerializeField] private GameObject gameOverPanel;             // 游戏结束面板
    [SerializeField] private Button giveUpButton;                  // 放弃按钮
    [SerializeField] private Button restartButton;                 // 重新开始按钮
    [SerializeField] private Button quitButton;                    // 退出按钮
    
    [Header("游戏结束文本")]
    [SerializeField] private Text gameOverTitleText;               // 游戏结束标题
    [SerializeField] private Text gameOverReasonText;              // 游戏结束原因
    [SerializeField] private Text gameOverDescriptionText;         // 游戏结束描述
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    [Header("结局专用面板")]
    [SerializeField] private GameObject healthZeroPanel;
    [SerializeField] private GameObject allItemsSoldPanel;
    [SerializeField] private GameObject playerGiveUpPanel;
    
    // 单例模式
    public static GameEndManager Instance { get; private set; }
    
    // 游戏结束状态
    private bool isGameOver = false;
    private GameEndReason currentEndReason = GameEndReason.None;
    
    // 游戏结束原因枚举
    public enum GameEndReason
    {
        None,           // 无
        HealthZero,     // 生命值归零
        AllItemsSold,   // 所有物品被当掉
        PlayerGaveUp    // 玩家放弃
    }
    
    private void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameEndManager] 游戏结束管理器初始化完成");
        }
        else
        {
            Debug.Log("[GameEndManager] 检测到重复的GameEndManager实例，销毁当前实例");
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // 初始化UI
        InitializeUI();
        
        // 订阅场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        Debug.Log("[GameEndManager] 游戏结束管理器启动完成");
    }
    
    private void Update()
    {
        // 如果游戏已经结束，不再检查
        if (isGameOver) return;
        
        // 检查游戏结束条件
        CheckGameEndConditions();
    }
    
    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitializeUI()
    {
        // 设置按钮事件
        if (giveUpButton != null)
        {
            giveUpButton.onClick.AddListener(OnGiveUpButtonClicked);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
        
        // 隐藏游戏结束面板
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 场景加载完成时的回调
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 在新场景中重新初始化UI
        InitializeUI();
        
        if (showDebugInfo)
        {
            Debug.Log($"[GameEndManager] 场景加载完成: {scene.name}");
        }
    }
    
    /// <summary>
    /// 检查游戏结束条件
    /// </summary>
    private void CheckGameEndConditions()
    {
        // 检查生命值
        if (enableHealthCheck && CheckHealthZero())
        {
            if (showDebugInfo)
            {
                Debug.Log("[GameEndManager] 检测到生命值归零，触发游戏结束");
            }
            EndGame(GameEndReason.HealthZero);
            return;
        }
        
        // 检查所有物品是否被当掉
        if (enableItemCheck && CheckAllItemsSold())
        {
            if (showDebugInfo)
            {
                Debug.Log("[GameEndManager] 检测到所有物品被当掉，触发游戏结束");
            }
            EndGame(GameEndReason.AllItemsSold);
            return;
        }
    }
    
    /// <summary>
    /// 检查生命值是否归零
    /// </summary>
    /// <returns>是否归零</returns>
    private bool CheckHealthZero()
    {
        if (Player.Instance != null)
        {
            return Player.Instance.CurrentHealth <= 0;
        }
        
        if (PlayerController.Instance != null && PlayerController.Instance.playerHealth != null)
        {
            return PlayerController.Instance.playerHealth.CurrentHealth <= 0;
        }
        
        return false;
    }
    
    /// <summary>
    /// 检查所有物品是否被当掉
    /// </summary>
    /// <returns>是否所有物品都被当掉</returns>
    private bool CheckAllItemsSold()
    {
        if (GameDataManager.Instance == null) 
        {
            if (showDebugInfo)
            {
                Debug.Log("[GameEndManager] GameDataManager实例未找到，无法检查物品状态");
            }
            return false;
        }
        
        int totalItems = 0;
        int soldItems = 0;
        
        // 显示所有物品的详细状态
        if (showDebugInfo)
        {
            Debug.Log("[GameEndManager] 检查物品状态:");
            foreach (var itemState in GameDataManager.Instance.itemStates)
            {
                Debug.Log($"  - {itemState.Key}: {itemState.Value}");
            }
        }
        
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            totalItems++;
            if (itemState.Value == PickableItem.ItemStateType.Solved)
            {
                soldItems++;
            }
        }
        
        if (showDebugInfo && totalItems > 0)
        {
            Debug.Log($"[GameEndManager] 物品检查: 总计{totalItems}个，已卖掉{soldItems}个");
        }
        
        // 如果所有物品都被当掉（状态为Solved）
        bool allSold = totalItems > 0 && soldItems == totalItems;
        
        if (showDebugInfo)
        {
            if (allSold)
            {
                Debug.Log("[GameEndManager] ✅ 检测到所有物品都已卖掉！触发游戏结束");
            }
            else
            {
                Debug.Log($"[GameEndManager] ℹ️ 物品未全部卖掉: {soldItems}/{totalItems}");
            }
        }
        
        return allSold;
    }
    
    /// <summary>
    /// 结束游戏
    /// </summary>
    /// <param name="reason">结束原因</param>
    public void EndGame(GameEndReason reason)
    {
        if (isGameOver) return;
        
        isGameOver = true;
        currentEndReason = reason;
        
        Debug.Log($"[GameEndManager] 游戏结束 - 原因: {GetReasonText(reason)}");
        
        // 停止时间流逝
        if (Player.Instance != null)
        {
            Player.Instance.StopHealthDecay();
        }
        
        // 显示游戏结束UI
        ShowGameOverUI(reason);
        
        // 保存游戏数据
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveGameData();
        }
    }
    
    /// <summary>
    /// 显示游戏结束UI
    /// </summary>
    /// <param name="reason">结束原因</param>
    private void ShowGameOverUI(GameEndReason reason)
    {
        // 先全部隐藏
        if (healthZeroPanel != null) healthZeroPanel.SetActive(false);
        if (allItemsSoldPanel != null) allItemsSoldPanel.SetActive(false);
        if (playerGiveUpPanel != null) playerGiveUpPanel.SetActive(false);

        // 根据结局类型显示对应Panel
        switch (reason)
        {
            case GameEndReason.HealthZero:
                if (healthZeroPanel != null) healthZeroPanel.SetActive(true);
                break;
            case GameEndReason.AllItemsSold:
                if (allItemsSoldPanel != null) allItemsSoldPanel.SetActive(true);
                break;
            case GameEndReason.PlayerGaveUp:
                if (playerGiveUpPanel != null) playerGiveUpPanel.SetActive(true);
                break;
            default:
                // 兼容老逻辑
                if (gameOverPanel != null) gameOverPanel.SetActive(true);
                break;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[GameEndManager] 显示游戏结束UI - 原因: {GetReasonText(reason)}");
        }
    }
    
    /// <summary>
    /// 获取结束原因文本
    /// </summary>
    /// <param name="reason">结束原因</param>
    /// <returns>原因文本</returns>
    private string GetReasonText(GameEndReason reason)
    {
        switch (reason)
        {
            case GameEndReason.HealthZero:
                return "生命值归零";
            case GameEndReason.AllItemsSold:
                return "所有物品被当掉";
            case GameEndReason.PlayerGaveUp:
                return "玩家放弃";
            default:
                return "未知原因";
        }
    }
    
    /// <summary>
    /// 获取结束描述文本
    /// </summary>
    /// <param name="reason">结束原因</param>
    /// <returns>描述文本</returns>
    private string GetDescriptionText(GameEndReason reason)
    {
        switch (reason)
        {
            case GameEndReason.HealthZero:
                return "你的生命值已经归零，无法继续游戏。\n时间流逝带走了你的生命。";
            case GameEndReason.AllItemsSold:
                return "你已经当掉了所有珍贵的物品。\n虽然获得了生命，但失去了所有回忆。";
            case GameEndReason.PlayerGaveUp:
                return "你选择了放弃。\n也许下次会有不同的选择。";
            default:
                return "游戏结束了。";
        }
    }
    
    /// <summary>
    /// 放弃按钮点击事件
    /// </summary>
    public void OnGiveUpButtonClicked()
    {
        if (showDebugInfo)
        {
            Debug.Log("[GameEndManager] 玩家点击了放弃按钮");
        }
        
        EndGame(GameEndReason.PlayerGaveUp);
    }
    
    /// <summary>
    /// 重新开始按钮点击事件
    /// </summary>
    public void OnRestartButtonClicked()
    {
        if (showDebugInfo)
        {
            Debug.Log("[GameEndManager] 玩家点击了重新开始按钮");
        }
        
        RestartGame();
    }
    
    /// <summary>
    /// 退出按钮点击事件
    /// </summary>
    public void OnQuitButtonClicked()
    {
        if (showDebugInfo)
        {
            Debug.Log("[GameEndManager] 玩家点击了退出按钮");
        }
        
        QuitGame();
    }
    
    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("[GameEndManager] 重新开始游戏");
        
        // 重置游戏状态
        isGameOver = false;
        currentEndReason = GameEndReason.None;
        
        // 重置游戏数据
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.ResetAllGameData();
            
            // 立即同步重置的数据到当前场景中的对象
            GameDataManager.Instance.SyncAllGameData();
            
            Debug.Log("[GameEndManager] 游戏数据已重置并同步");
        }
        
        // 隐藏游戏结束面板
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // 加载主菜单场景
        SceneManager.LoadScene("MainMenu");
    }
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("[GameEndManager] 退出游戏");
        
        // 保存游戏数据
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveGameData();
        }
        
        // 退出应用
        Application.Quit();
    }
    
    /// <summary>
    /// 检查游戏是否已结束
    /// </summary>
    /// <returns>是否已结束</returns>
    public bool IsGameOver()
    {
        return isGameOver;
    }
    
    /// <summary>
    /// 获取当前结束原因
    /// </summary>
    /// <returns>结束原因</returns>
    public GameEndReason GetCurrentEndReason()
    {
        return currentEndReason;
    }
    
    /// <summary>
    /// 强制结束游戏（用于调试）
    /// </summary>
    /// <param name="reason">结束原因</param>
    [ContextMenu("强制结束游戏")]
    public void ForceEndGame(GameEndReason reason = GameEndReason.PlayerGaveUp)
    {
        EndGame(reason);
    }
    
    /// <summary>
    /// 重置游戏结束状态（用于调试）
    /// </summary>
    [ContextMenu("重置游戏结束状态")]
    public void ResetGameOverState()
    {
        isGameOver = false;
        currentEndReason = GameEndReason.None;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        Debug.Log("[GameEndManager] 游戏结束状态已重置");
    }
    
    private void OnDestroy()
    {
        // 取消订阅场景加载事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
} 