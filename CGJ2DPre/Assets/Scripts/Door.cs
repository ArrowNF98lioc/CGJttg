using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [Header("门设置")]
    public string targetSceneName = "Scene2";  // 目标场景名称
    
    [Header("贴图设置")]
    public Sprite normalSprite;       // 正常距离的贴图
    public Sprite closeSprite;        // 近距离的贴图
    public float closeDistance = 2f;  // 切换贴图的距离
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;
    private bool isInteracting = false;

    void Start()
    {
        // 检查必要的组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("[Door] 未找到SpriteRenderer组件，请添加SpriteRenderer");
            return;
        }
        
        // 检查Collider2D组件（OnMouseDown需要）
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError("[Door] 未找到Collider2D组件，OnMouseDown需要Collider2D才能工作");
            return;
        }
        
        // 设置初始贴图
        if (normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
        
        // 查找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("[Door] 未找到Player标签的对象");
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[Door] 门初始化完成，目标场景: {targetSceneName}");
        }
    }

    void Update()
    {
        if (isInteracting) return;
        
        // 检查距离并切换贴图
        CheckDistanceAndUpdateSprite();
    }
    
    /// <summary>
    /// 检查距离并更新贴图
    /// </summary>
    void CheckDistanceAndUpdateSprite()
    {
        if (playerTransform == null || spriteRenderer == null) return;
        
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        
        if (distance <= closeDistance)
        {
            // 近距离，使用closeSprite
            if (closeSprite != null && spriteRenderer.sprite != closeSprite)
            {
                spriteRenderer.sprite = closeSprite;
            }
        }
        else
        {
            // 正常距离，使用normalSprite
            if (normalSprite != null && spriteRenderer.sprite != normalSprite)
            {
                spriteRenderer.sprite = normalSprite;
            }
        }
    }

    // 点击门，切换场景
    void OnMouseDown() 
    {
        if (isInteracting) return;
        
        // 检查玩家距离
        if (playerTransform == null)
        {
            Debug.LogWarning("[Door] 未找到玩家，无法进行距离检查");
            return;
        }
        
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (distance > closeDistance)
        {
            Debug.Log($"[Door] 玩家距离太远 ({distance:F2} > {closeDistance})，无法点击门");
            return;
        }
        
        Debug.Log($"[Door] 点击了门，玩家距离: {distance:F2}，准备切换场景！");
        
        // 检查目标场景是否存在
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("[Door] 目标场景名称为空，请设置targetSceneName");
            return;
        }
        
        isInteracting = true;
        
        // 使用SceneDataManager切换场景（如果存在）
        if (SceneDataManager.Instance != null)
        {
            Debug.Log($"[Door] 使用SceneDataManager切换场景: {targetSceneName}");
            SceneDataManager.Instance.LoadScene(targetSceneName);
        }
        else
        {
            // 如果没有SceneDataManager，使用传统方式
            Debug.LogWarning("[Door] SceneDataManager未找到，使用传统场景切换方式");
            SceneManager.LoadScene(targetSceneName);
        }
    }
    
    /// <summary>
    /// 设置目标场景名称
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public void SetTargetScene(string sceneName)
    {
        targetSceneName = sceneName;
        
        if (showDebugInfo)
        {
            Debug.Log($"[Door] 目标场景设置为: {targetSceneName}");
        }
    }
    
    /// <summary>
    /// 重置门状态（用于调试）
    /// </summary>
    public void ResetDoor()
    {
        isInteracting = false;
        
        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
        
        if (showDebugInfo)
        {
            Debug.Log("[Door] 门状态已重置");
        }
    }
    
    /// <summary>
    /// 获取门的状态信息
    /// </summary>
    /// <returns>状态信息字符串</returns>
    public string GetDoorStatus()
    {
        string status = $"门状态信息:\n";
        status += $"目标场景: {targetSceneName}\n";
        status += $"交互状态: {(isInteracting ? "交互中" : "空闲")}\n";
        status += $"距离阈值: {closeDistance}\n";
        status += $"玩家距离: {(playerTransform != null ? Vector2.Distance(transform.position, playerTransform.position).ToString("F2") : "未知")}\n";
        status += $"当前贴图: {(spriteRenderer != null && spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "无")}";
        
        return status;
    }
    
    /// <summary>
    /// 显示门的状态信息
    /// </summary>
    [ContextMenu("显示门状态")]
    public void ShowDoorStatus()
    {
        Debug.Log(GetDoorStatus());
    }
}
