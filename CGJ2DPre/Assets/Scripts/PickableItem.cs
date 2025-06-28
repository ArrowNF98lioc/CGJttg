using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [Header("物品设置")]
    public string itemName;           // 物品名称
    
    [Header("贴图设置")]
    public Sprite normalSprite;       // 正常距离的贴图
    public Sprite closeSprite;        // 近距离的贴图
    public float closeDistance = 2f;  // 切换贴图的距离
    
    [Header("拾取设置")]
    public Vector3 pickupScale = new Vector3(0.5f, 0.5f, 1f);  // 拾取时的大小
    
    [Header("UI设置")]
    public Transform uiTarget;        // UI目标位置（拖拽UI中的目标Transform）
    public float moveToUITime = 0.5f; // 移动到UI的时间
    public GameObject uiGameObject;   // 要激活的UI GameObject
    
    [Header("拾取模式")]
    public bool requireTrigger = false;  // 是否需要进入触发区域才能拾取
    public float maxPickupDistance = 5f; // 最大拾取距离（当不需要触发时）
    
    private SpriteRenderer spriteRenderer;
    private bool canPickUp = false;
    private Transform playerTransform;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private bool isPickedUp = false;
    private bool isMovingToUI = false;
    private Vector3 startPosition;
    private float moveTimer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("[PickableItem] 未找到SpriteRenderer组件");
            return;
        }
        
        // 设置初始贴图
        if (normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
        
        // 保存原始大小和位置
        originalScale = transform.localScale;
        originalPosition = transform.position;
        
        // 查找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("[PickableItem] 未找到Player标签的对象");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = false;
        }
    }

    void Update()
    {
        if (isPickedUp && !isMovingToUI) return;
        
        // 如果正在移动到UI
        if (isMovingToUI)
        {
            MoveToUI();
            return;
        }
        
        // 检查距离并切换贴图
        CheckDistanceAndUpdateSprite();
        
        // 检查点击拾取
        if (Input.GetMouseButtonDown(0))
        {
            CheckClickPickup();
        }
    }
    
    /// <summary>
    /// 检查点击拾取
    /// </summary>
    void CheckClickPickup()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        
        // 检查是否点击到了这个物品
        if (hit != null && hit.gameObject == this.gameObject)
        {
            // 检查拾取条件
            if (CanPickup())
            {
                PickUp();
            }
            else
            {
                // 显示拾取失败信息
                if (requireTrigger)
                {
                    Debug.Log("[PickableItem] 需要靠近物品才能拾取");
                }
                else if (playerTransform != null)
                {
                    float distance = Vector2.Distance(transform.position, playerTransform.position);
                    if (distance > maxPickupDistance)
                    {
                        Debug.Log($"[PickableItem] 距离太远，无法拾取 (距离: {distance:F1})");
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 检查是否可以拾取
    /// </summary>
    /// <returns>是否可以拾取</returns>
    bool CanPickup()
    {
        if (playerTransform == null) return false;
        
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        
        if (requireTrigger)
        {
            // 需要触发模式：必须进入触发区域
            return canPickUp;
        }
        else
        {
            // 直接点击模式：检查距离
            return distance <= maxPickupDistance;
        }
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

    /// <summary>
    /// 拾取物品
    /// </summary>
    void PickUp()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[PickableItem] 未找到Inventory组件");
            return;
        }
        
        // 检查ItemManager状态
        if (ItemManager.Instance == null)
        {
            Debug.LogError("[PickableItem] 未找到ItemManager组件");
            return;
        }
        
        // 检查物品是否存在
        if (!ItemManager.Instance.HasItem(itemName))
        {
            Debug.LogError($"[PickableItem] 物品 '{itemName}' 在ItemManager中不存在");
            return;
        }
        
        // 添加到背包
        bool success = Inventory.Instance.AddItem(itemName);
        
        if (success)
        {
            // 标记为已拾取
            isPickedUp = true;
            
            // 改变大小
            transform.localScale = pickupScale;
            
            // 开始移动到UI
            StartMoveToUI();
            
            // 同步到GameDataManager
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.AddCollectedItem(itemName);
                GameDataManager.Instance.SetGameFlag($"hasCollected{itemName}", true);
            }
            
            Debug.Log($"[PickableItem] 成功拾取物品: {itemName}");
        }
        else
        {
            Debug.LogWarning($"[PickableItem] 拾取物品失败: {itemName}，背包可能已满");
        }
    }
    
    /// <summary>
    /// 开始移动到UI
    /// </summary>
    void StartMoveToUI()
    {
        if (uiTarget == null)
        {
            Debug.LogWarning("[PickableItem] 未设置UI目标位置，直接销毁物品");
            Destroy(gameObject, 0.5f);
            return;
        }
        
        isMovingToUI = true;
        moveTimer = 0f;
        startPosition = transform.position;
        
        // 激活指定的UI GameObject
        if (uiGameObject != null)
        {
            uiGameObject.SetActive(true);
            Debug.Log($"[PickableItem] 激活UI GameObject: {uiGameObject.name}");
        }
        
        // 禁用碰撞器，避免继续触发
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
    }
    
    /// <summary>
    /// 移动到UI
    /// </summary>
    void MoveToUI()
    {
        moveTimer += Time.deltaTime;
        float progress = moveTimer / moveToUITime;
        
        if (progress >= 1f)
        {
            // 移动完成，销毁物品
            Destroy(gameObject);
            return;
        }
        
        // 使用缓动效果
        float easeProgress = 1f - Mathf.Pow(1f - progress, 3f); // 缓出效果
        
        // 计算目标位置（世界坐标）
        Vector3 targetWorldPos = uiTarget.position;
        
        // 插值移动
        transform.position = Vector3.Lerp(startPosition, targetWorldPos, easeProgress);
        
        // 逐渐缩小
        float scaleProgress = Mathf.Lerp(1f, 0.3f, easeProgress);
        transform.localScale = new Vector3(scaleProgress, scaleProgress, 1f);
    }
    
    /// <summary>
    /// 重置物品状态（用于调试）
    /// </summary>
    public void ResetItem()
    {
        isPickedUp = false;
        isMovingToUI = false;
        moveTimer = 0f;
        transform.localScale = originalScale;
        transform.position = originalPosition;
        
        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
        
        // 重新启用碰撞器
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }
    }
}