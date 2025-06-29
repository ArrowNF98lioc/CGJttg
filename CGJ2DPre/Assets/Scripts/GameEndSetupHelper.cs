using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏结束系统设置助手
/// 帮助快速设置GameEndManager和相关UI组件
/// </summary>
public class GameEndSetupHelper : MonoBehaviour
{
    [Header("设置选项")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool createUIComponents = true;
    
    [Header("UI预制体设置")]
    [SerializeField] private GameObject gameOverPanelPrefab;
    [SerializeField] private Button giveUpButtonPrefab;
    [SerializeField] private Button restartButtonPrefab;
    [SerializeField] private Button quitButtonPrefab;
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupGameEndSystem();
        }
    }
    
    /// <summary>
    /// 设置游戏结束系统
    /// </summary>
    [ContextMenu("设置游戏结束系统")]
    public void SetupGameEndSystem()
    {
        Debug.Log("=== 开始设置游戏结束系统 ===");
        
        // 1. 检查GameEndManager是否存在
        if (GameEndManager.Instance == null)
        {
            Debug.Log("GameEndManager不存在，正在创建...");
            CreateGameEndManager();
        }
        else
        {
            Debug.Log("GameEndManager已存在");
        }
        
        // 2. 检查UI组件
        // if (createUIComponents)
        // {
        //     SetupUIComponents();
        // }
        
        // 3. 验证设置
        ValidateSetup();
        
        Debug.Log("=== 游戏结束系统设置完成 ===");
    }
    
    /// <summary>
    /// 创建GameEndManager
    /// </summary>
    private void CreateGameEndManager()
    {
        // 查找GameDataManager
        GameDataManager gameDataManager = GameDataManager.Instance;
        if (gameDataManager == null)
        {
            Debug.LogError("GameDataManager不存在，无法创建GameEndManager");
            return;
        }
        
        // 在GameDataManager的GameObject下创建GameEndManager
        GameObject gameEndManagerObj = new GameObject("GameEndManager");
        gameEndManagerObj.transform.SetParent(gameDataManager.transform);
        
        // 添加GameEndManager组件
        GameEndManager gameEndManager = gameEndManagerObj.AddComponent<GameEndManager>();
        
        Debug.Log("GameEndManager创建完成");
    }
    
    /// <summary>
    /// 设置UI组件
    /// </summary>
    private void SetupUIComponents()
    {
        if (GameEndManager.Instance == null)
        {
            Debug.LogError("GameEndManager不存在，无法设置UI组件");
            return;
        }
        
        // 查找Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("未找到Canvas，无法创建UI组件");
            return;
        }
        
        Debug.Log("开始设置UI组件...");
        
        // 创建游戏结束面板
        GameObject gameOverPanel = CreateGameOverPanel(canvas.transform);
        
        // 创建按钮
        Button giveUpButton = CreateButton(canvas.transform, "GiveUpButton", "放弃");
        Button restartButton = CreateButton(canvas.transform, "RestartButton", "重新开始");
        Button quitButton = CreateButton(canvas.transform, "QuitButton", "退出");
        
        // 设置GameEndManager的UI引用
        SetGameEndManagerUIReferences(gameOverPanel, giveUpButton, restartButton, quitButton);
        
        Debug.Log("UI组件设置完成");
    }
    
    /// <summary>
    /// 创建游戏结束面板
    /// </summary>
    private GameObject CreateGameOverPanel(Transform parent)
    {
        GameObject panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(parent, false);
        
        // 添加Image组件作为背景
        Image background = panel.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.8f);
        
        // 设置RectTransform
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        // 创建标题文本
        CreateText(panel.transform, "TitleText", "游戏结束", 24, TextAnchor.MiddleCenter);
        
        // 创建原因文本
        CreateText(panel.transform, "ReasonText", "游戏结束原因", 18, TextAnchor.MiddleCenter);
        
        // 创建描述文本
        CreateText(panel.transform, "DescriptionText", "游戏结束描述", 14, TextAnchor.MiddleCenter);
        
        // 默认隐藏
        panel.SetActive(false);
        
        return panel;
    }
    
    /// <summary>
    /// 创建文本组件
    /// </summary>
    private Text CreateText(Transform parent, string name, string content, int fontSize, TextAnchor alignment)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        Text text = textObj.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        
        // 设置RectTransform
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.1f, 0.6f);
        rectTransform.anchorMax = new Vector2(0.9f, 0.9f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        return text;
    }
    
    /// <summary>
    /// 创建按钮
    /// </summary>
    private Button CreateButton(Transform parent, string name, string text)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        // 添加Button组件
        Button button = buttonObj.AddComponent<Button>();
        
        // 添加Image组件
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // 创建按钮文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.fontSize = 16;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
        
        // 设置文本RectTransform
        RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.offsetMin = Vector2.zero;
        textRectTransform.offsetMax = Vector2.zero;
        
        // 设置按钮RectTransform
        RectTransform buttonRectTransform = buttonObj.GetComponent<RectTransform>();
        buttonRectTransform.anchorMin = new Vector2(0.3f, 0.1f);
        buttonRectTransform.anchorMax = new Vector2(0.7f, 0.2f);
        buttonRectTransform.offsetMin = Vector2.zero;
        buttonRectTransform.offsetMax = Vector2.zero;
        
        return button;
    }
    
    /// <summary>
    /// 设置GameEndManager的UI引用
    /// </summary>
    private void SetGameEndManagerUIReferences(GameObject gameOverPanel, Button giveUpButton, Button restartButton, Button quitButton)
    {
        // 使用反射设置私有字段
        var gameEndManager = GameEndManager.Instance;
        var type = gameEndManager.GetType();
        
        // 设置游戏结束面板
        var gameOverPanelField = type.GetField("gameOverPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        gameOverPanelField?.SetValue(gameEndManager, gameOverPanel);
        
        // 设置按钮
        var giveUpButtonField = type.GetField("giveUpButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        giveUpButtonField?.SetValue(gameEndManager, giveUpButton);
        
        var restartButtonField = type.GetField("restartButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        restartButtonField?.SetValue(gameEndManager, restartButton);
        
        var quitButtonField = type.GetField("quitButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        quitButtonField?.SetValue(gameEndManager, quitButton);
        
        // 设置文本组件
        var titleTextField = type.GetField("gameOverTitleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        titleTextField?.SetValue(gameEndManager, gameOverPanel.transform.Find("TitleText")?.GetComponent<Text>());
        
        var reasonTextField = type.GetField("gameOverReasonText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        reasonTextField?.SetValue(gameEndManager, gameOverPanel.transform.Find("ReasonText")?.GetComponent<Text>());
        
        var descriptionTextField = type.GetField("gameOverDescriptionText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        descriptionTextField?.SetValue(gameEndManager, gameOverPanel.transform.Find("DescriptionText")?.GetComponent<Text>());
        
        // 启用调试信息
        var showDebugInfoField = type.GetField("showDebugInfo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        showDebugInfoField?.SetValue(gameEndManager, true);
        
        Debug.Log("GameEndManager UI引用设置完成");
    }
    
    /// <summary>
    /// 验证设置
    /// </summary>
    private void ValidateSetup()
    {
        Debug.Log("=== 验证设置 ===");
        
        if (GameEndManager.Instance == null)
        {
            Debug.LogError("❌ GameEndManager设置失败");
        }
        else
        {
            Debug.Log("✅ GameEndManager设置成功");
            
            // 检查UI组件
            var type = GameEndManager.Instance.GetType();
            var gameOverPanel = type.GetField("gameOverPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(GameEndManager.Instance);
            
            if (gameOverPanel != null)
            {
                Debug.Log("✅ 游戏结束面板设置成功");
            }
            else
            {
                Debug.LogWarning("⚠️ 游戏结束面板未设置");
            }
        }
        
        Debug.Log("=== 验证完成 ===");
    }
    
    /// <summary>
    /// 测试游戏结束功能
    /// </summary>
    [ContextMenu("测试游戏结束")]
    public void TestGameEnd()
    {
        if (GameEndManager.Instance == null)
        {
            Debug.LogError("GameEndManager未设置，无法测试");
            return;
        }
        
        Debug.Log("测试游戏结束功能...");
        GameEndManager.Instance.EndGame(GameEndManager.GameEndReason.PlayerGaveUp);
    }
} 