using UnityEngine;
using UnityEngine.SceneManagement;

public class PickableItem : MonoBehaviour
{
    [Header("物品设置")]
    public string itemName;           // 物品名称
    
    [Header("贴图设置")]
    public Sprite normalSprite;       // 正常距离的贴图
    public Sprite closeSprite;        // 近距离的贴图
    public float closeDistance = 2f;  // 切换贴图的距离
    
    
    [Header("UI设置")]
    public GameObject uiGameObjectAtHome;   // 要激活的UI GameObject
    public GameObject uiGameObjectBox;   // 要激活的UI GameObject
    public GameObject uiGameObjectShop;   // 要激活的UI GameObject
    public GameObject uiGameObjectSolved; // 要激活的UI GameObject
    
    [Header("拾取模式")]
    public bool requireTrigger = false;  // 是否需要进入触发区域才能拾取
    public float maxPickupDistance = 5f; // 最大拾取距离（当不需要触发时）
    
    [Header("物品状态")]
    public ItemStateType currentState = ItemStateType.AtHome;  // 当前物品状态
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    private SpriteRenderer spriteRenderer;
    private bool canPickUp = false;
    private Transform playerTransform;

    public enum ItemStateType   // 物品状态类型
    {
        AtHome,
        Selected,
        Solved
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"[PickableItem] 未找到SpriteRenderer组件: {itemName}");
            return;
        }
        
        // 设置初始贴图
        if (normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
            Debug.Log($"[PickableItem] 设置初始贴图: {itemName}");
        }
        else
        {
            Debug.LogWarning($"[PickableItem] normalSprite未设置: {itemName}");
        }
        
        // 查找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log($"[PickableItem] 找到玩家: {itemName}");
        }
        else
        {
            Debug.LogWarning($"[PickableItem] 未找到Player标签的对象: {itemName}");
        }
        
        // 检查物品名称
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogError($"[PickableItem] 物品名称未设置: {gameObject.name}");
        }
        
        // 检查碰撞器
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogWarning($"[PickableItem] 未找到Collider2D组件，点击检测可能不工作: {itemName}");
        }
        
        // 从GameDataManager同步物品状态
        if (GameDataManager.Instance != null && GameDataManager.Instance.itemStates.ContainsKey(itemName))
        {
            currentState = GameDataManager.Instance.itemStates[itemName];
            Debug.Log($"[PickableItem] 从GameDataManager同步状态: {itemName} = {currentState}");
        }
        
        // 检查UI GameObject是否已设置
        if (uiGameObjectAtHome == null || uiGameObjectBox == null || uiGameObjectShop == null || uiGameObjectSolved == null)
        {
            Debug.LogWarning($"[PickableItem] UI GameObject未完全设置: {itemName} (AtHome: {uiGameObjectAtHome != null}, Box: {uiGameObjectBox != null}, Shop: {uiGameObjectShop != null}, Solved: {uiGameObjectSolved != null})");
        }
        
        // 根据状态更新显示
        SetItemSprite();
        
        Debug.Log($"[PickableItem] 初始化完成: {itemName}");
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
        // 持续从GameDataManager同步物品状态
        SyncStateFromGameDataManager();
        
        // 始终检查距离并更新贴图（无论物品状态如何）
        CheckDistanceAndUpdateSprite();
        
        // 根据物品状态执行不同逻辑
        if (currentState == ItemStateType.AtHome)
        {
            // 检查点击拾取
            if (Input.GetMouseButtonDown(0))
            {
                CheckClickPickup();
            }
        }
        else if (currentState == ItemStateType.Selected)
        {
            CheckBoxClick();
        }
        else if (currentState == ItemStateType.Solved)
        {
            // Solved状态下只更新贴图，不处理点击
        }
    }

    /// <summary>
    /// 从GameDataManager同步物品状态
    /// </summary>
    private void SyncStateFromGameDataManager()
    {
        if (GameDataManager.Instance != null && GameDataManager.Instance.itemStates.ContainsKey(itemName))
        {
            PickableItem.ItemStateType newState = GameDataManager.Instance.itemStates[itemName];
            if (newState != currentState)
            {
                currentState = newState;
                SetItemSprite();
                
                if (showDebugInfo)
                {
                    Debug.Log($"[PickableItem] 状态已从GameDataManager同步: {itemName} = {currentState}");
                }
            }
        }
    }

    /// <summary>
    /// 检查盒子点击, 为实现，如果点击了盒子，且在SceneManager中当前场景为Home，就把物品状态设置成AtHome
    /// </summary>
    void CheckBoxClick()
    {
        if (SceneManager.GetActiveScene().name == "Home")
        {
            currentState = ItemStateType.AtHome;
            // 同步到GameDataManager
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.UpdateItemState(itemName, currentState);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            CheckClickPickup();
        }
    }
    
    /// <summary>
    /// 根据物品状态更新物品贴图
    /// </summary>
    public void SetItemSprite()
    {
        if (currentState == ItemStateType.AtHome)
        {
            if (uiGameObjectAtHome != null) uiGameObjectAtHome.SetActive(true);
            if (uiGameObjectBox != null) uiGameObjectBox.SetActive(false);
            if (uiGameObjectShop != null) uiGameObjectShop.SetActive(false);
            if (uiGameObjectSolved != null) uiGameObjectSolved.SetActive(false);
        }
        else if (currentState == ItemStateType.Selected)    
        {
            if (uiGameObjectAtHome != null) uiGameObjectAtHome.SetActive(false);
            if (uiGameObjectBox != null) uiGameObjectBox.SetActive(true);
            if (uiGameObjectShop != null) uiGameObjectShop.SetActive(false);
            if (uiGameObjectSolved != null) uiGameObjectSolved.SetActive(false);
        }
        else if (currentState == ItemStateType.Solved)      
        {
            if (uiGameObjectAtHome != null) uiGameObjectAtHome.SetActive(false);
            if (uiGameObjectBox != null) uiGameObjectBox.SetActive(false);
            if (uiGameObjectShop != null) uiGameObjectShop.SetActive(true);
            if (uiGameObjectSolved != null) uiGameObjectSolved.SetActive(true);
        }
    }
    
    /// <summary>
    /// 检查PickableItem是否已经准备好（UI GameObject已设置）
    /// </summary>
    /// <returns>是否已准备好</returns>
    public bool IsReady()
    {
        return uiGameObjectAtHome != null || uiGameObjectBox != null || uiGameObjectShop != null || uiGameObjectSolved != null;
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
            Debug.Log($"[PickableItem] 成功点击到物品: {itemName}");
            
            // 检查拾取条件
            if (CanPickup())
            {
                Debug.Log($"[PickableItem] 满足拾取条件，开始拾取: {itemName}");
                PickUp();
            }
            else
            {
                // 显示拾取失败信息
                if (GameDataManager.Instance != null && GameDataManager.Instance.HasOtherItemSelected(itemName))
                {
                    string selectedItemName = GameDataManager.Instance.GetSelectedItemName();
                    Debug.Log($"[PickableItem] 无法拾取 {itemName}，因为 {selectedItemName} 已经被选中");
                }
                else if (requireTrigger)
                {
                    Debug.Log("[PickableItem] 需要靠近物品才能拾取");
                }
                else if (playerTransform != null)
                {
                    float distance = Vector2.Distance(transform.position, playerTransform.position);
                    Debug.Log($"[PickableItem] 距离太远，无法拾取 (距离: {distance:F1}, 最大距离: {maxPickupDistance})");
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
        
        // 检查是否有其他物品已经处于Selected状态
        if (GameDataManager.Instance != null && GameDataManager.Instance.HasOtherItemSelected(itemName))
        {
            string selectedItemName = GameDataManager.Instance.GetSelectedItemName();
            Debug.Log($"[PickableItem] 无法拾取 {itemName}，因为 {selectedItemName} 已经被选中");
            return false;
        }
        
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
        if (playerTransform == null)
        {
            Debug.LogWarning($"[PickableItem] 玩家Transform未找到: {itemName}");
            return;
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"[PickableItem] SpriteRenderer未找到: {itemName}");
            return;
        }
        
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        
        if (distance <= closeDistance)
        {
            // 近距离，使用closeSprite
            if (closeSprite != null && spriteRenderer.sprite != closeSprite)
            {
                spriteRenderer.sprite = closeSprite;
                Debug.Log($"[PickableItem] 切换到近距离贴图: {itemName}");
            }
        }
        else
        {
            // 正常距离，使用normalSprite
            if (normalSprite != null && spriteRenderer.sprite != normalSprite)
            {
                spriteRenderer.sprite = normalSprite;
                Debug.Log($"[PickableItem] 切换到正常距离贴图: {itemName}");
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
        
        // 更新物品状态
        currentState = ItemStateType.Selected;
        SetItemSprite();
        
        // 同步到GameDataManager
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.UpdateItemState(itemName, currentState);
            Debug.Log($"[PickableItem] 物品状态已同步到GameDataManager: {itemName} = {currentState}");
        }
        
        // 开始移动到UI
        // StartMoveToUI();
    }
    
    
    /// <summary>
    /// 重置物品状态（用于调试）
    /// </summary>
    public void ResetItem()
    {
        currentState = ItemStateType.AtHome;
        SetItemSprite();
        
        // 同步到GameDataManager
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.UpdateItemState(itemName, currentState);
        }
        
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