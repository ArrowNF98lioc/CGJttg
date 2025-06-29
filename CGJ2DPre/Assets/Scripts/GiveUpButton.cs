using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 放弃按钮脚本
/// 点击时触发游戏结束
/// </summary>
public class GiveUpButton : MonoBehaviour
{
    [Header("按钮设置")]
    [SerializeField] private Button giveUpButton;                  // 放弃按钮
    [SerializeField] private bool showConfirmation = true;         // 是否显示确认对话框
    [SerializeField] private GameObject confirmationPanel;         // 确认面板
    [SerializeField] private Button confirmButton;                 // 确认按钮
    [SerializeField] private Button cancelButton;                  // 取消按钮
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    private void Start()
    {
        // 如果没有指定按钮，尝试获取当前GameObject上的Button组件
        if (giveUpButton == null)
        {
            giveUpButton = GetComponent<Button>();
        }
        
        // 设置按钮事件
        if (giveUpButton != null)
        {
            giveUpButton.onClick.AddListener(OnGiveUpButtonClicked);
        }
        else
        {
            Debug.LogError("[GiveUpButton] 未找到Button组件");
        }
        
        // 设置确认和取消按钮事件
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
        
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
        
        // 隐藏确认面板
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
        
        if (showDebugInfo)
        {
            Debug.Log("[GiveUpButton] 放弃按钮初始化完成");
        }
    }
    
    /// <summary>
    /// 放弃按钮点击事件
    /// </summary>
    public void OnGiveUpButtonClicked()
    {
        if (showDebugInfo)
        {
            Debug.Log("[GiveUpButton] 玩家点击了放弃按钮");
        }
        
        if (showConfirmation && confirmationPanel != null)
        {
            // 显示确认对话框
            ShowConfirmationDialog();
        }
        else
        {
            // 直接触发游戏结束
            TriggerGameEnd();
        }
    }
    
    /// <summary>
    /// 显示确认对话框
    /// </summary>
    private void ShowConfirmationDialog()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
        
        if (showDebugInfo)
        {
            Debug.Log("[GiveUpButton] 显示确认对话框");
        }
    }
    
    /// <summary>
    /// 确认按钮点击事件
    /// </summary>
    public void OnConfirmButtonClicked()
    {
        if (showDebugInfo)
        {
            Debug.Log("[GiveUpButton] 玩家确认放弃");
        }
        
        // 隐藏确认面板
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
        
        // 触发游戏结束
        TriggerGameEnd();
    }
    
    /// <summary>
    /// 取消按钮点击事件
    /// </summary>
    public void OnCancelButtonClicked()
    {
        if (showDebugInfo)
        {
            Debug.Log("[GiveUpButton] 玩家取消放弃");
        }
        
        // 隐藏确认面板
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 触发游戏结束
    /// </summary>
    private void TriggerGameEnd()
    {
        if (GameEndManager.Instance != null)
        {
            GameEndManager.Instance.EndGame(GameEndManager.GameEndReason.PlayerGaveUp);
        }
        else
        {
            Debug.LogWarning("[GiveUpButton] GameEndManager实例未找到，无法触发游戏结束");
            
            // 如果没有GameEndManager，直接退出到主菜单
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
    
    /// <summary>
    /// 设置是否显示确认对话框
    /// </summary>
    /// <param name="show">是否显示</param>
    public void SetShowConfirmation(bool show)
    {
        showConfirmation = show;
    }
    
    /// <summary>
    /// 强制触发放弃（用于调试）
    /// </summary>
    [ContextMenu("强制触发放弃")]
    public void ForceGiveUp()
    {
        TriggerGameEnd();
    }
} 