using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;

    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        
        // 确保unlockedLevel不超过按钮数组的长度
        unlockedLevel = Mathf.Clamp(unlockedLevel, 1, buttons.Length);
        
        // 首先禁用所有按钮
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].interactable = false;
            }
        }
        
        // 然后启用已解锁的按钮
        for (int i = 0; i < unlockedLevel && i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].interactable = true;
            }
        }
    }

    public void OpenLevel(int levelId)
    {
        // 添加边界检查
        if (levelId <= 0 || levelId > buttons.Length)
        {
            Debug.LogWarning($"无效的关卡ID: {levelId}");
            return;
        }
        
        string levelName = "Level" + levelId;
        SceneManager.LoadSceneAsync(levelName);
    }
}
