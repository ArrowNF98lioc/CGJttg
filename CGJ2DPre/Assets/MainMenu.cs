using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    public void PlayGame()
    {
        Debug.Log("[MainMenu] 开始游戏");
        LoadSceneWithDataSync(1);
    }

    public void QuitGame()
    {
        // 保存游戏数据
        SaveGameDataBeforeQuit();
        Debug.Log("[MainMenu] 退出游戏");
        Application.Quit();
    }

    public void OpenMenu()
    {
        Debug.Log("[MainMenu] 打开主菜单");
        LoadSceneWithDataSync(0);
    }

    public void ReturnHome()
    {
        Debug.Log("[MainMenu] 返回家中");
        LoadSceneWithDataSync(1);
    }

    public void OpenGallery()
    {
        Debug.Log("[MainMenu] 打开画廊");
        LoadSceneWithDataSync(2);
    }
    
    /// <summary>
    /// 使用数据同步加载场景
    /// </summary>
    /// <param name="sceneIndex">场景索引</param>
    private void LoadSceneWithDataSync(int sceneIndex)
    {
        // 同步游戏数据
        SyncGameDataBeforeSceneChange();
        
        // 使用SceneDataManager切换场景（如果存在）
        if (SceneDataManager.Instance != null)
        {
            if (showDebugInfo)
            {
                Debug.Log($"[MainMenu] 使用SceneDataManager切换场景: 索引 {sceneIndex}");
            }
            SceneDataManager.Instance.LoadScene(sceneIndex);
        }
        else
        {
            // 如果没有SceneDataManager，使用传统方式
            if (showDebugInfo)
            {
                Debug.LogWarning("[MainMenu] SceneDataManager未找到，使用传统场景切换方式");
            }
            SceneManager.LoadSceneAsync(sceneIndex);
        }
    }
    
    /// <summary>
    /// 使用数据同步加载场景（通过名称）
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    private void LoadSceneWithDataSync(string sceneName)
    {
        // 同步游戏数据
        SyncGameDataBeforeSceneChange();
        
        // 使用SceneDataManager切换场景（如果存在）
        if (SceneDataManager.Instance != null)
        {
            if (showDebugInfo)
            {
                Debug.Log($"[MainMenu] 使用SceneDataManager切换场景: {sceneName}");
            }
            SceneDataManager.Instance.LoadScene(sceneName);
        }
        else
        {
            // 如果没有SceneDataManager，使用传统方式
            if (showDebugInfo)
            {
                Debug.LogWarning("[MainMenu] SceneDataManager未找到，使用传统场景切换方式");
            }
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
    
    /// <summary>
    /// 场景切换前同步游戏数据
    /// </summary>
    private void SyncGameDataBeforeSceneChange()
    {
        if (showDebugInfo)
        {
            Debug.Log("[MainMenu] 开始同步游戏数据...");
        }
        
        // 同步Player数据
        if (Player.Instance != null)
        {
            Player.Instance.SyncToGameDataManager();
        }
        else
        {
            Debug.LogWarning("[MainMenu] Player实例未找到");
        }
        
        // 同步Inventory数据
        if (Inventory.Instance != null)
        {
            Inventory.Instance.SyncToGameDataManager();
        }
        else
        {
            Debug.LogWarning("[MainMenu] Inventory实例未找到");
        }
        
        // 保存游戏数据
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveGameData();
        }
        else
        {
            Debug.LogWarning("[MainMenu] GameDataManager实例未找到");
        }
        
        if (showDebugInfo)
        {
            Debug.Log("[MainMenu] 场景切换前数据同步完成");
        }
    }
    
    /// <summary>
    /// 退出游戏前保存数据
    /// </summary>
    private void SaveGameDataBeforeQuit()
    {
        if (showDebugInfo)
        {
            Debug.Log("[MainMenu] 开始保存游戏数据...");
        }
        
        // 同步Player数据
        if (Player.Instance != null)
        {
            Player.Instance.SyncToGameDataManager();
        }
        
        // 同步Inventory数据
        if (Inventory.Instance != null)
        {
            Inventory.Instance.SyncToGameDataManager();
        }
        
        // 保存游戏数据
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveGameData();
        }
        
        if (showDebugInfo)
        {
            Debug.Log("[MainMenu] 退出游戏前数据保存完成");
        }
    }
    
    /// <summary>
    /// 显示当前游戏状态
    /// </summary>
    [ContextMenu("显示游戏状态")]
    public void ShowGameStatus()
    {
        string status = "[MainMenu] 当前游戏状态:\n";
        status += $"GameDataManager: {(GameDataManager.Instance != null ? "已连接" : "未连接")}\n";
        status += $"SceneDataManager: {(SceneDataManager.Instance != null ? "已连接" : "未连接")}\n";
        status += $"Player: {(Player.Instance != null ? "已连接" : "未连接")}\n";
        status += $"Inventory: {(Inventory.Instance != null ? "已连接" : "未连接")}\n";
        
        if (Player.Instance != null)
        {
            status += $"玩家生命值: {Player.Instance.CurrentHealth}/{Player.Instance.MaxHealth}\n";
            status += $"当前物品: {(Player.Instance.currentItem != null ? Player.Instance.currentItem.name : "无")}\n";
        }
        
        if (Inventory.Instance != null)
        {
            status += $"背包物品数: {Inventory.Instance.CurrentItemCount}/{Inventory.Instance.MaxSlots}\n";
        }
        
        Debug.Log(status);
    }
    
    /// <summary>
    /// 强制刷新所有数据
    /// </summary>
    [ContextMenu("强制刷新数据")]
    public void ForceRefreshAllData()
    {
        if (SceneDataManager.Instance != null)
        {
            SceneDataManager.Instance.ForceRefreshAllData();
        }
        else
        {
            Debug.LogWarning("[MainMenu] SceneDataManager未找到，无法强制刷新数据");
        }
    }
}